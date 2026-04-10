using SharpDX;
using System.Collections.Concurrent;

public abstract class Textures
{
	public static ShaderResourceView Light = new (dev, CreateColor([200, 200, 200, 255]));
	public static ShaderResourceView Gray = new(dev, CreateColor([127, 127, 127, 255]));
	public static ShaderResourceView Black = new(dev, CreateColor([0, 0, 0, 255]));

	static ConcurrentDictionary<string, ShaderResourceView> cache = [];

	static Texture2D CreateColor(byte[] data) {

		var desc = new Texture2DDescription {
			Width = 1, 
			Height = 1, 	
			ArraySize = 1,
			Format = Format.R8G8B8A8_UNorm,
			SampleDescription = new(1, 0),
			BindFlags = BindFlags.ShaderResource
		};

		var dataStream = DataStream.Create(data, false, false);
		var dataRect = new DataRectangle(dataStream.DataPointer, 4);
		return new Texture2D(dev, desc, dataRect);
	}

	public static ShaderResourceView LoadDDS(FLVER2.Texture texture, bool diffuse = false)
	{
		string textureName = Path.GetFileNameWithoutExtension(texture?.Path);
		string fullPath = Path.GetDirectoryName(Flver.path) + $"\\{textureName}.dds";

		if (cache.ContainsKey(fullPath))
			return cache[fullPath];

		if (!File.Exists(fullPath))
			return null;

		var bytes = File.ReadAllBytes(fullPath);

		bool hasDX10 = BitConverter.ToInt32(bytes, 84) == 0x30315844; 
		int dxgiFormat = BitConverter.ToInt32(bytes, 128);
		bool isBC7 = hasDX10 && dxgiFormat == 98 || dxgiFormat == 99;

		Texture2D tex;
		Texture2DDescription desc = new Texture2DDescription
		{
			SampleDescription = new(1, 0),
			MipLevels = 1, 
			ArraySize = 1,
			BindFlags = BindFlags.ShaderResource
		};

		// Direct load to GPU memory withoud decoding like Pfim does
		if (isBC7 && BitConverter.ToInt32(bytes, 12) % 4 == 0)
		{
			int headerSize = 148;
			desc.Height = BitConverter.ToInt32(bytes, 12);
			desc.Width = BitConverter.ToInt32(bytes, 16);
			desc.Format = diffuse ? Format.BC7_UNorm_SRgb : Format.BC7_UNorm;

			var dataStream = DataStream.Create(bytes, false, false);
			var dataRect = new DataRectangle(dataStream.DataPointer + headerSize, desc.Width * 4);
			tex = new Texture2D(dev, desc, dataRect);
		}
		else
		{
			var image = Pfim.Dds.Create(bytes, new());
			desc.Width = image.Width;
			desc.Height = image.Height;
			desc.Format = Format.B8G8R8A8_UNorm;
			tex = new Texture2D(dev, desc);
			c.UpdateSubresource(image.Data, tex, 0, image.Width * 4);
		}

		var SRV = new ShaderResourceView(dev, tex);
		cache[fullPath] = SRV;

		return SRV;
	}
	
	public static async void ReloadAll()
	{
		var tasks = Meshes.Select( m => Task.Run(() =>
		{
			var textures = m.material.Textures;
			textures.ForEach(x => x.Path = x.Path.ToLower());

			var d = textures.Find(x => x.Path.Contains("_a") || x.Path.Contains("_d") || ((x.Type.ToLower().Contains("diffuse") || x.Type.ToLower().Contains("albedo")) && x.Path != ""));
			m.diffuse = LoadDDS(d, true);

			// Transparency is determined by file name, since it's literally impossible to know whether BC7 has alpha or not from metadata
			if (d is not null && (d.Path.Contains("hair") || d.Path.Contains("brow") || d.Path.Contains("eye") || d.Path.Contains("alpha")))
				m.Transparent = true;

			m.normal = LoadDDS(textures.Find(x => x.Path.Contains("_n")));
			m.metalness = LoadDDS(textures.Find(x => x.Path.Contains("_m")));
			m.SSS = LoadDDS(textures.Find(x => x.Path.Contains("_3m") || x.Path.Contains("_1m") || x.Type.Contains("_sss")));
			m.specularDSR = LoadDDS(textures.Find(x => x.Path.Contains("_s")));
		}));
		
		await Task.WhenAll(tasks);
		GC.Collect();
	}
}