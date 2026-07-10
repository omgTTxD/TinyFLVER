using BC = System.BitConverter;

public class Textures
{
	public static SRV cubeMap = LoadTexture(AppContext.BaseDirectory + "Textures\\CubeMaps\\Cubemap2.dds");
	
	static FileSystemWatcher watcher;

	public static void CreateFileSystemWatcher()
	{
		watcher = new FileSystemWatcher(".", "*.dds") { EnableRaisingEvents = true };

		// We must wait a little, because when event is raised file is still busy
		watcher.Changed += (s, e) => { Thread.Sleep(1000); ReloadAll(); };
		watcher.Renamed += (s, e) => ReloadAll();
	}

	public static SRV FindTexture(List<FLVER2.Texture> txs, string type, params string[] patterns)
	{
		// First we try to find texture by type. If nothing, try to find by suffix
		var tex = txs.FirstOrDefault(x => (x.Type ?? "").Contains(type) && x.Path != "");
		tex ??= txs.FirstOrDefault(x => (x.Path ?? "").ContainsAny(patterns));

		// If nothing, just get the first texture with needed suffix. If it is only one in folder, it will be correct
		string path = form.GuessTexture.Checked && tex is null 
			? Directory.GetFiles(".", "*.dds").Select(x => Path.GetFileName(x)).Where(x => x.ContainsAny(patterns)).OrderBy(x => x.Length).FirstOrDefault()
			: Path.GetFileNameWithoutExtension(tex?.Path) + ".dds";

		return LoadTexture(path);
	}

	public static Texture2DDescription ParseDescription(string path, out byte[] bytes, out int offset, out int blockSize)
	{
		bytes = File.ReadAllBytes(path);

		var d = new Texture2DDescription() with
		{
			Height = BC.ToInt32(bytes, 12),
			Width = BC.ToInt32(bytes, 16),
			MipLevels = BC.ToInt32(bytes, 28),
			ArraySize = BC.ToInt32(bytes, 136) == 4 ? 6 : 1,
			BindFlags = BindFlags.ShaderResource,
			SampleDescription = new(1, 0),
		};

		if (d.ArraySize == 6)
			d.OptionFlags = ResourceOptionFlags.TextureCube;

		string fourCC = System.Text.Encoding.UTF8.GetString(bytes, 84, 4);

		d.Format = fourCC switch
		{
			"\0\0\0\0" => Format.R8G8B8A8_UNorm,	
			"DXT1" => Format.BC1_UNorm,
			"DXT5" => Format.BC3_UNorm,
			"ATI1" => Format.BC4_UNorm,
			"BC4U" => Format.BC4_UNorm,
			"BC5U" => Format.BC5_UNorm,
			"DX10" => (Format)bytes[128],
			_ => throw new NotImplementedException($"Texture '{Path.GetFileName(path)}' uses DDS format '{fourCC}' which is not added yet.")
		};

		offset = fourCC == "DX10" ? 148 : 128;

		blockSize = d.Format switch
		{
			Format.BC1_UNorm or Format.BC1_UNorm_SRgb or Format.BC4_UNorm => 8,
			_ => 16
		};

		return d;
	}

	public static SRV LoadTexture(string path)
	{
		if (!File.Exists(path))
			return null;

		var d = ParseDescription(path, out var bytes, out var offset, out var blockSize);
		using var data = DataStream.Create(bytes, false, false);
		var mips = new DataBox[d.ArraySize * d.MipLevels];

		for (int face = 0; face < d.ArraySize; face++)
		{
			for (int mip = 0; mip < d.MipLevels; mip++)
			{
				int mipWidth = Math.Max(1, ((d.Width >> mip) + 3) / 4);
				int mipHeight = Math.Max(1, ((d.Height >> mip) + 3) / 4);
				mips[face * d.MipLevels + mip] = new(data.DataPointer + offset, mipWidth * blockSize, 0);
				offset += mipWidth * mipHeight * blockSize;
			}
		}

		return new SRV(dev, new Texture2D(dev, d, mips));
	}

	public static void Reload(Mesh m)
	{
		m.diffuse = FindTexture(m.material.Textures, "AlbedoMap", "_a", "_d");
		m.normal = FindTexture(m.material.Textures, "NormalMap", "_n");
		m.metalness = FindTexture(m.material.Textures, "MetallicMap", "_m");
		m.SSS = FindTexture(m.material.Textures, "Mask", "_3m", "_1m");
		m.specularDSR = FindTexture(m.material.Textures, "Specular", "_s");
		GC.Collect();
	}

	public static void ReloadAll()
	{
		Meshes.ToList().ForEach(m => Reload(m));
		form.dgTextures.Invalidate();
	}
}