using Pfim;
using SharpDX;
using System.Collections.Concurrent;

namespace TinyFLVER;

public abstract class Textures
{
	public static ShaderResourceView defaultDiffuseSRV = new (dev, CreateDefaultTextures([200, 200, 200, 255], true));
	public static ShaderResourceView defaultNormalSRV = new(dev, CreateDefaultTextures([127, 127, 127, 255]));
	public static ShaderResourceView defaultBlackSRV = new(dev, CreateDefaultTextures([0, 0, 0, 255]));

	static ConcurrentDictionary<string, ShaderResourceView> Cache = [];

	public static Texture2D CreateDefaultTextures(byte[] data, bool srgb = false) {
		var desc = new Texture2DDescription {
			Width = 1, Height = 1, ArraySize = 1,
			Format = srgb ? Format.R8G8B8A8_UNorm_SRgb : Format.R8G8B8A8_UNorm,
			SampleDescription = new(1, 0),
			BindFlags = BindFlags.ShaderResource
		};
		
		var dataStream = DataStream.Create(data, true, false);
		var dataRect = new DataRectangle(dataStream.DataPointer, 4);
		return new Texture2D(dev, desc, dataRect);
	}

	public static ShaderResourceView LoadDDS(FLVER2.Texture texture, Format format = Format.B8G8R8A8_UNorm)
	{
		string textureName = Path.GetFileNameWithoutExtension(texture?.Path);
		string fullPath = Path.GetDirectoryName(path) + $"\\{textureName}.dds";

		if (!File.Exists(fullPath))
			return null;

		if (Cache.ContainsKey(textureName))
			return Cache[textureName];

		var image = Dds.Create(File.ReadAllBytes(fullPath), new());

		var desc = new Texture2DDescription
		{
			Width = image.Width, Height = image.Height,
			Format = format, SampleDescription = new(1, 0),
			MipLevels = 1, 	ArraySize = 1,
			BindFlags = BindFlags.ShaderResource
		};

		// For normal maps we should create mips. Otherwise skin will have sharp pixelated look. 
		// Also for this reason we don't create SamplerState, because default filtering is good (unlike Anisotropic and others)
		if (textureName.Contains("_n"))
		{
			desc.MipLevels = 4;
			desc.OptionFlags = ResourceOptionFlags.GenerateMipMaps;
			desc.BindFlags |= BindFlags.RenderTarget;
		}

		var tex = new Texture2D(dev, desc);
		c.UpdateSubresource(image.Data, tex, 0, image.Width * 4);

		var SRV = new ShaderResourceView(dev, tex);
		Cache[textureName] = SRV;

		// Generate mips for normal maps
		if (textureName.Contains("_n"))
			c.GenerateMips(SRV);

		return SRV;
	}

	public static async void ReloadAllAsync()
	{
		var tasks = Meshes.Select(m => Task.Run(() =>
		{
			var textures = m.material.Textures;
			textures.ForEach(x => x.Path = x.Path.ToLower());

			var d = textures.Find(x => x.Path.Contains("_a") || x.Path.Contains("_d") || 
			(x.Path != "" && (x.Type.ToLower().Contains("diffuse") || x.Type.ToLower().Contains("albedo"))));
			m.diffuseBinding = LoadDDS(d, Format.B8G8R8A8_UNorm_SRgb);

			if (d is not null && (d.Path.Contains("hair") || d.Path.Contains("brow") || d.Path.Contains("eyelashes") || d.Path.Contains("alpha")))
				m.Transparent = true;

			m.normalBinding = LoadDDS(textures.Find(x => x.Path.Contains("_n")));

			m.metalnessBinding = LoadDDS(textures.Find(x => x.Path.Contains("_m")));

			m.sssBinding = LoadDDS(textures.Find(x => x.Path.Contains("_3m") || x.Path.Contains("_1m") || x.Type.Contains("sss")));
		}));
	
		await Task.WhenAll(tasks);
		GC.Collect();
	}
}