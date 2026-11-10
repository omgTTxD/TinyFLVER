using SharpAssimp.Configs;
using System.Collections.Concurrent;
using System.IO;
using BC = System.BitConverter;
using ROF = SharpDX.Direct3D11.ResourceOptionFlags;

public class Textures
{	
	public static ConcurrentDictionary<string, SRV> cache = [];
	public static string dir = AppContext.BaseDirectory + "Textures\\";
	static SRV CubeMap, RectMap, BRDF_LUT = ReadTexture(dir + "BRDF_LUT.dds");
	public static SRV forDeletion = new SRV(-1);

	public static void Initialize()
	{
		form.comboBoxCubemaps.Items.AddRange(["None", ..Directory.GetFiles(dir, "*Map*").Select(x => Path.GetFileName(x))]);
		form.comboBoxCubemaps.SelectedIndexChanged += (_, _) => ReadCubemaps();
	}

	// ==== Texture Loading ===============================================================================================================

	public static void ReadCubemaps()
	{
		var name = form.comboBoxCubemaps.Text;
		var srv = ReadTexture(dir + name);
		if (name == "None" || name is null || srv is null) 
			CubeMap = RectMap = null;
		else {
			bool cubeMap = srv.Description.Dimension != ShaderResourceViewDimension.Texture2D;
			CubeMap = cubeMap ? srv : null;
			RectMap = !cubeMap ? srv : null;
		}

		c.PixelShader.SetShaderResources(9, BRDF_LUT,CubeMap, RectMap);
	}

	public static SRV ReadTexture(string path)
	{
		if (path is null)
			return null;

		if (!path.EndsWith(".dds"))
			path = Path.ChangeExtension(path, ".dds");// GetFileNameWithoutExtension(path) + ".dds";

		if (cache.ContainsKey(path))
			return cache[path];

		var texture = LoadFromDisk(path);

		if (texture is null)
			return null;

		cache[path] = texture;

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
			"\0\0\0\0" => BC.ToInt32(bytes, 92) == 0xFF0000 ? Format.B8G8R8A8_UNorm : Format.R8G8B8A8_UNorm,
			"DXT1" => Format.BC1_UNorm,
			"DXT5" => Format.BC3_UNorm,
			"ATI1" => Format.BC4_UNorm,
			"BC4U" => Format.BC4_UNorm,
			"BC5U" => Format.BC5_UNorm,
			"DX10" => (Format)bytes[128],
			_ => Format.Unknown
		};

		blockSize = desc.Format.AnyOf(Format.BC1_UNorm, Format.BC1_UNorm_SRgb, Format.BC4_UNorm) ? 8 : 16;

		if (desc.Format == 0)
			form.Text = ($"Error. Texture '{Path.GetFileName(path)}' has format '{fourCC}' which is not added yet.");

		return desc;
	}

	public static SRV LoadFromDisk(string path)
	{

		if (!File.Exists(path))
			return null;

		var desc = ParseDescription(path, out var bytes, out var offset, out var blockSize);
		
		if (desc.Format == Format.Unknown)
			return null;

		using var data = DataStream.Create(bytes, false, false);
		var mips = new DataBox[desc.ArraySize * desc.MipLevels];

		for (int face = 0; face < desc.ArraySize; face++)
		{
			for (int mip = 0; mip < desc.MipLevels; mip++)
			{
				int width = Math.Max(1, ((desc.Width >> mip) + 3) / 4);
				int height = Math.Max(1, ((desc.Height >> mip) + 3) / 4);
				mips[face * desc.MipLevels + mip] = new(data.DataPointer + offset, width * blockSize, 0);
				offset += width * height * blockSize;
			}
		}

		return new SRV(d, new Texture2D(d, desc, mips));
	}

	// ==== Texture Names Guessing ========================================================================================================
	
	static FileSystemWatcher watcher;

	// When we first loading FLVER if we found only one texture, then automatically apply it. But when we delete it we shouldn't guess it again.

	public static void PrepareForNewFlver()
	{
		watcher?.Dispose();
		watcher = new FileSystemWatcher(".", "*.dds") { SynchronizingObject = form, EnableRaisingEvents = true };
		watcher.Renamed += TextureCHanged;
		watcher.Deleted += TextureCHanged;
		watcher.Created += TextureCHanged;
		watcher.Changed += TextureCHanged;


		cache.Clear();
		Textures.ReloadAll();
	}

	static void TextureCHanged(object s, FileSystemEventArgs e)
	{
		cache.TryRemove(Path.GetFileName(e.Name), out var v); 
		ReloadAll();
	}

	public static void ReloadAll()
	{
		Meshes.ToList().ForEach(m => m.ReloadTextures());
		GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive);
	}
}