using BC = System.BitConverter;
using SharpDX;

public class Textures
{
	static FileSystemWatcher watcher;

	public static void CreateFileSystemWatcher()
	{
		watcher = new FileSystemWatcher(".", "*.dds") { EnableRaisingEvents = true };

		// We must wait a little, because when event is raised file is still busy
		watcher.Changed += (s, e) => { Thread.Sleep(1000); ReloadAll(); };
		watcher.Renamed += (s, e) => ReloadAll();
	}

	public static SRV FindTexture(List<FLVER2.Texture> txs, List<string> types, string[] suffixes)
	{
		string path = "";
		
		// First we try to find texture by type. If nothing, try to find by suffix
		var tex = txs.FirstOrDefault(x => (x.Type ?? "").ContainsAny(types) && x.Path != "");
		tex ??= txs.FirstOrDefault(x => (x.Path ?? "").ContainsAny(suffixes));

		// If nothing, just get the first texture with needed suffix. If it is only one in folder, it will be correct	
		var textures = Directory.GetFiles(".", "*.dds").Select(x => Path.GetFileName(x)).Where(x => x.ContainsAny(suffixes)).ToList();
		
		if (tex is not null)
			path = Path.GetFileNameWithoutExtension(tex?.Path);
	//	else
		//if (textures.Count == 1)
		//	path = Path.GetFileNameWithoutExtension(textures.First());

		return path is null ? null : LoadTexture(path + ".dds");
	}

	public static SRV LoadTexture(string path)
	{
		if (!File.Exists(path))
			return null;

		var desc = ParseDescription(path, out var bytes, out var offset, out var blockSize);
		using var data = DataStream.Create(bytes, false, false);
		var mips = new DataBox[desc.ArraySize * desc.MipLevels];

		for (int face = 0; face < desc.ArraySize; face++)
		{
			for (int mip = 0; mip < desc.MipLevels; mip++)
			{
				int mipWidth = Math.Max(1, ((desc.Width >> mip) + 3) / 4);
				int mipHeight = Math.Max(1, ((desc.Height >> mip) + 3) / 4);
				mips[face * desc.MipLevels + mip] = new(data.DataPointer + offset, mipWidth * blockSize, 0);
				offset += mipWidth * mipHeight * blockSize;
			}
		}

		return new SRV(d, new Texture2D(d, desc, mips));
	}


	public static Texture2DDescription ParseDescription(string path, out byte[] bytes, out int offset, out int blockSize)
	{
		bytes = File.ReadAllBytes(path);
		string fourCC = System.Text.Encoding.UTF8.GetString(bytes, 84, 4);
		
		offset = fourCC == "DX10" ? 148 : 128;

		Format format = fourCC switch
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

		blockSize = format switch { Format.BC1_UNorm or Format.BC1_UNorm_SRgb or Format.BC4_UNorm => 8, _ => 16 };

		return Desc.DDS(bytes, format);
	}

	public static void Reload(Mesh m)
	{
		m.diffuse = FindTexture(m.material.Textures, ["Albedo", "g_Diffuse"], ["_a", "_d"]);
		m.normal = FindTexture(m.material.Textures, ["NormalMap", "g_Bumpmap"], ["_n"]);
		m.metalness = FindTexture(m.material.Textures, ["MetallicMap"], ["_m"]);
		m.SSS = FindTexture(m.material.Textures, ["Mask"], ["_3m", "_1m"]);
		m.specularDSR = FindTexture(m.material.Textures, ["g_Specular"], ["_s"]);
		m.reflectance = FindTexture(m.material.Textures, ["Reflectance"], ["_r"]);
		GC.Collect();
	}

	public static void ReloadAll()
	{
		Meshes.ToList().ForEach( m => Task.Run(() => Reload(m)));
	}
}