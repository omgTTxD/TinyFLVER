using SharpDX;
using System.Collections.Concurrent;
using BC = System.BitConverter;
using ROF = SharpDX.Direct3D11.ResourceOptionFlags;

public class Textures
{
	static string dir = AppContext.BaseDirectory + "Textures\\";
	static FileSystemWatcher watcher;
	public static ConcurrentDictionary<string, SRV> cache = [];

	public static SRV cubeMap = LoadTexture(dir + "CubeMap0.dds");
	public static SRV details = LoadTexture(dir + "AAT100_Skin_01_n.dds");
	public static SRV brdf_lut = LoadTexture(dir + "BRDF_LUT.dds");

	public static bool firstLoad = true;

	public static void PrepareForFlver()
	{
		watcher?.Dispose();
		watcher = new FileSystemWatcher(".", "*.dds") { SynchronizingObject = form, EnableRaisingEvents = true };

		watcher.Changed += Reload;
		watcher.Renamed += Reload;
		watcher.Created += Reload;
		watcher.Deleted += Reload;

		cache.Clear();
		firstLoad = true;
		Textures.ReloadAll();
		firstLoad = false;
	}

	public static void Reload(object s, FileSystemEventArgs e)
	{
		cache.TryRemove(e.Name, out var v); ReloadAll();
	}

	public static void Initialize()
	{
		form.comboBoxCubemaps.Items.Add("None");
		form.comboBoxCubemaps.Items.AddRange(Directory.GetFiles(dir, "Cubemap*.dds").Select(x => Path.GetFileName(x)).ToArray());
		form.comboBoxCubemaps.SelectedIndexChanged += (_, _) =>
		{
			cubeMap = form.comboBoxCubemaps.Text == "None" ? null : Textures.LoadTexture(dir + form.comboBoxCubemaps.Text);
			c.PixelShader.SetShaderResources(5, cubeMap, details, brdf_lut);
		};
		form.comboBoxCubemaps.SelectedIndex = 0;
	}

	static string Convert(string path) => Path.GetFileNameWithoutExtension(path).Split(".")[0];


	// Very bad code in need of refactoring
	public static SRV FindTexture(List<FLVER2.Texture> txs, string[] types, string[] suffixes, string detailNormals = "")
	{
		// First we try to find texture by type. If nothing, try to find by suffix
		var tex = txs.FirstOrDefault(x => x.Type.ContainsAny(types) && x.Path != "" && File.Exists(Convert(x.Path) + ".dds") && Convert(x.Path) != detailNormals);
		tex ??= txs.FirstOrDefault(x => Convert(x.Path).EndsWithAny(suffixes) && File.Exists(Convert(x.Path) + ".dds") && Convert(x.Path) != detailNormals);

		string path = "";
		if (tex is not null)
			path = Convert(tex.Path);
		else
		if (firstLoad)
		{
			// If nothing, just get the first texture with needed suffix. If it is only one in folder, it will be correct	
			var textures = Directory.GetFiles(".", "*.dds").Where(x => Convert(x).EndsWithAny(suffixes) && Convert(x) != detailNormals).ToList();

			// Then we must find a slot to put this texture in
			if (textures.Count == 1)
			{
				path = Convert(textures[0]);
			}
		}

		if (path is null)
			return null;
	
		return LoadTexture(path + ".dds");
	}

	public static SRV LoadTexture(string path)
	{
		if (!File.Exists(path))
			return null;

		if (cache.ContainsKey(path))
			return cache[path];

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

		cache[path] = new SRV(d, new Texture2D(d, desc, mips));
		return cache[path];
	}

	public static Texture2DDescription ParseDescription(string path, out byte[] bytes, out int offset, out int blockSize)
	{
		bytes = File.ReadAllBytes(path);
		string fourCC = System.Text.Encoding.UTF8.GetString(bytes, 84, 4);
		offset = fourCC == "DX10" ? 148 : 128;

		Texture2DDescription desc = new()
		{
			Height = BC.ToInt32(bytes, 12),
			Width = BC.ToInt32(bytes, 16),
			MipLevels = BC.ToInt32(bytes, 28),
			ArraySize = BC.ToInt32(bytes, 136) == 4 ? 6 : 1,
			OptionFlags = BC.ToInt32(bytes, 136) == 4 ? ROF.TextureCube : ROF.None,
			BindFlags = BindFlags.ShaderResource,
			SampleDescription = new(1, 0)
		};

		desc.Format = fourCC switch
		{
			"\0\0\0\0" => Format.R8G8B8A8_UNorm,
			"DXT1" => Format.BC1_UNorm,
			"DXT5" => Format.BC3_UNorm,
			"ATI1" => Format.BC4_UNorm,
			"BC4U" => Format.BC4_UNorm,
			"BC5U" => Format.BC5_UNorm,
			"DX10" => (Format)bytes[128],
			_ => throw new($"Texture '{Path.GetFileName(path)}' uses DDS format '{fourCC}' which is not added yet.")
		};

		blockSize = desc.Format.AnyOf(Format.BC1_UNorm, Format.BC1_UNorm_SRgb, Format.BC4_UNorm) ? 8 : 16;
		return desc;
	}

	public static void Reload(Mesh m)
	{
		m.diffuse = FindTexture(m.material.Textures, ["Albedo", "g_Diffuse"], ["_a", "_d"]);
		m.normal = FindTexture(m.material.Textures, ["NormalMap", "g_Bumpmap"], ["_n"], "AAT100_Skin_01_n");
		m.metalness = FindTexture(m.material.Textures, ["MetallicMap"], ["_m"]);
		//	m.SSS = FindTexture(m.material.Textures, ["Mask"], ["_3m", "_1m"]);
		m.specular = FindTexture(m.material.Textures, ["g_Specular"], ["_s"]);
		m.reflectance = FindTexture(m.material.Textures, ["Reflectance"], ["_r"]);
	}

	public static void ReloadAll()
	{
		Meshes.ToList().ForEach(m => Reload(m));
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
	}
}