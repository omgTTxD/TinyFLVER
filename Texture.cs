using SharpDX;
using System.Collections.Concurrent;

public class Texture
{
	public static SRV Light = new (d, CreateColor([200, 200, 200, 255]));
	public static SRV Gray = new(d, CreateColor([127, 127, 127, 255]));
	public static SRV Black = new(d, CreateColor([0, 0, 0, 255]));

	public static ConcurrentDictionary<Texture, SRV> cache = [];
	public static ConcurrentDictionary<Texture, DateTime> lastModified = [];
	
	public FLVER2.Texture texture;

	public string TexturePath { get => texture.Path.Replace(".tif", ".dds"); set { texture.Path = value; } }
	public string TextureType { get => texture.Type; set { texture.Type = value; } }
	public bool formatSRGB { get; set; }
	public bool usingSRGB { get => sRGB; set { sRGB = value; lastModified[this] = new(); Reload(); } }
	string Name { get => Path.GetFileName(TexturePath); }
	bool sRGB;

	public Texture(FLVER2.Texture texture)
	{
		this.texture = texture;
		if (texture.Path is null) return;
		sRGB = texture.Path.Contains("_a") || texture.Path.Contains("_d");
	}

	static Texture2D CreateColor(byte[] data) {
		var desc = new Texture2DDescription {
			Width = 1, 		Height = 1, 	
			ArraySize = 1,
			Format = Format.R8G8B8A8_UNorm,
			SampleDescription = new(1, 0),
			BindFlags = BindFlags.ShaderResource
		};

		var dataStream = DataStream.Create(data, false, false);
		var dataRect = new DataRectangle(dataStream.DataPointer, 4);
		return new Texture2D(d, desc, dataRect);
	}

	public static SRV LoadDDS(Mesh m, params string[] patterns)
	{
		Texture tex = null;
		foreach (var pattern in patterns) 
			tex = m.textureList.Find(x => x.Name.Contains(pattern)) ?? tex;

		if (tex == null || !File.Exists(tex.Name))
			return null;

		if (cache.ContainsKey(tex) && lastModified[tex] == File.GetLastWriteTime(tex.Name))
			return cache[tex];

		var bytes = File.ReadAllBytes(tex.Name); DataStream data;

		Texture2DDescription desc = new Texture2DDescription
		{
			Height = BitConverter.ToInt32(bytes, 12),
			Width = BitConverter.ToInt32(bytes, 16),
			SampleDescription = new(1, 0),
			MipLevels = 1,
			ArraySize = 1,
			BindFlags = BindFlags.ShaderResource,
			Format = tex.usingSRGB ? Format.B8G8R8A8_UNorm_SRgb : Format.B8G8R8A8_UNorm
		};

		if (bytes[128] == 98)
			desc.Format = Format.BC7_UNorm;

		if (bytes[128] == 99) {
			tex.formatSRGB = true;
			desc.Format = tex.usingSRGB ? Format.BC7_UNorm_SRgb : Format.BC7_UNorm;
		}

		// Direct load to GPU memory withoud decoding like Pfim does. Also checking that texture's resolution divides by 4,
		// since otherwise it will crash. For BC1 and ofther formats I don't care about use Pfim and his slow CPU-unpacking.
		if ((bytes[128] == 98 || bytes[128] == 99) && desc.Width % 4 == 0)
			data = DataStream.Create(bytes, false, false, 148);
		else 
			data = DataStream.Create(Pfim.Dds.Create(bytes, new()).Data, false, false);

		cache[tex] = new SRV(d, new Texture2D(d, desc, new DataRectangle(data.DataPointer, desc.Width * 4)));
		lastModified[tex] = File.GetLastWriteTime(tex.Name);

		return cache[tex];
	}
	
	public static void Reload()
	{
		Directory.SetCurrentDirectory(Path.GetDirectoryName(Flver.path));

		Meshes.ForEach(m => {
			var textures = m.material.Textures;
			m.diffuse = LoadDDS(m, "_a", "_d") ?? Light;
			m.normal = LoadDDS(m, "_n") ?? Gray;
			m.metalness = LoadDDS(m, "_m") ?? Black;
			m.SSS = LoadDDS(m,"_3m", "_1m", "_sss") ?? Black;
		//	m.specularDSR = LoadDDS(m.textureList.Find(x => x.TexturePath.Contains("_s")));
		});
		GC.Collect();
	}
}