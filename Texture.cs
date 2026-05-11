using SharpDX;
using System.IO;
using System.Text;
using static System.BitConverter;

public class Texture (FLVER2.Texture texture)
{
	public static SRV Light = new (d, CreateColor([200, 200, 200, 255]));
	public static SRV Gray = new(d, CreateColor([127, 127, 127, 255]));
	public static SRV Black = new(d, CreateColor([0, 0, 0, 255]));
	
	public static Dictionary<string, SRV> cache = [];
	
	public FLVER2.Texture texture = texture;
	public string TexturePath { get => texture.Path?.Replace(".tif", ""); set { texture.Path = value; } }
	public string TextureType { get => texture.Type; set { texture.Type = value; } }
	public bool sRGB { get; set; }

	static Texture2D CreateColor(byte[] data) 
	{
		var desc = new Texture2DDescription {
			Width = 1, Height = 1,
			ArraySize = 1,
			SampleDescription = new(1, 0),
			Format = Format.R8G8B8A8_UNorm,
			BindFlags = BindFlags.ShaderResource
		};
		return new Texture2D(d, desc, new DataRectangle(DataStream.Create(data, false, false).DataPointer, 4));
	}

	static FileSystemWatcher watcher;

	public static void CreateFileSystemWatcher()
	{
		watcher = new FileSystemWatcher(".", "*.dds") { EnableRaisingEvents = true };
		watcher.Changed += (s, e) => { cache.Remove(Path.GetFileName(e.Name)); ReloadAll(); };
		watcher.Renamed += (s, e) => { cache.Remove(Path.GetFileName(e.Name)); ReloadAll(); };
	}

	public static SRV LoadDDS(Mesh m, params string[] patterns)
	{
		Texture tex = null;
		foreach (var pattern in patterns) 
			tex = m.textureList.Find(x => x.TexturePath is not null && x.TexturePath.Contains(pattern)) ?? tex;

		var path = Path.GetFileNameWithoutExtension(tex?.TexturePath) + ".dds";

		if (!File.Exists(path)) 
			return null;

		if (cache.ContainsKey(path))
			return cache[path];

		var bytes = File.ReadAllBytes(path);
		string fourCC = Encoding.UTF8.GetString(bytes, 84, 4);

		Texture2DDescription desc = new Texture2DDescription
		{
			Height = ToInt32(bytes, 12),
			Width = ToInt32(bytes, 16),
			SampleDescription = new(1, 0),
			ArraySize = 1,
			MipLevels = 1,
			BindFlags = BindFlags.ShaderResource,
			Format = fourCC == "DX10" ? (Format)bytes[128] : Format.BC3_UNorm  // doesn't really matter if BC3 or not
		};

		byte[] sRGB = [72, 75, 78, 99];
		tex.sRGB = sRGB.Contains(bytes[128]);

		int offset = fourCC == "DX10" ? 148 : 128;
		int pitch = (fourCC == "DXT1" || fourCC == "ATI1" || (fourCC == "DX10" && desc.Format == Format.BC1_UNorm_SRgb)) ? 2 : 4;

		var data = DataStream.Create(bytes, false, false, offset);
		cache[path] = new SRV(d, new Texture2D(d, desc, new DataRectangle(data.DataPointer, desc.Width * pitch)));
		return cache[path];
	}
	
	public static void Reload(Mesh m)
	{
		m.diffuse = LoadDDS(m, "_a", "_d") ?? Light;
		m.normal = LoadDDS(m, "_n") ?? Gray;
		m.metalness = LoadDDS(m, "_m") ?? Black;
		m.SSS = LoadDDS(m,"_3m", "_1m") ?? Black;
	}

	public static void ReloadAll()
	{
		Meshes.ForEach(m => Reload(m));
		GC.Collect();
	}
}