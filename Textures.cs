using SharpDX;

class Textures
{
	static FileSystemWatcher watcher;
	static Texture2DDescription defaultDesc;

	public static void CreateFileSystemWatcher()
	{
		watcher = new FileSystemWatcher(".", "*.dds") { EnableRaisingEvents = true };
		watcher.Changed += (s, e) => Mesh.ReloadAllTextures();
		watcher.Renamed += (s, e) => Mesh.ReloadAllTextures();
	}

	public static SRV FindTexture(List<FLVER2.Texture> txs, string type, params string[] patterns)
	{
		var tex = txs.FirstOrDefault(x => patterns.Any(p => (x.Path ?? "").Contains(p)) || (x.Type ?? "").Contains(type));
		var path = Path.ChangeExtension(tex?.Path, ".dds");
		
		if (path is null || !File.Exists(path)) 
			return null;
		
		byte[] bytes = [];
		while (bytes.Length == 0 ) try { bytes = File.ReadAllBytes(path); } catch (IOException) { Thread.Sleep(10); }
		return CreateFromBytes(bytes);
	}

	public static SRV CreateFromBytes(byte[] bytes)
	{
		Texture2DDescription desc = defaultDesc with { 
			Height = BitConverter.ToInt32(bytes, 12),
			Width = BitConverter.ToInt32(bytes, 16),
			ArraySize = 1, MipLevels = 1,
			BindFlags = BindFlags.ShaderResource,
			SampleDescription = new(1, 0)
		};

		int pitch = 4; int offset = 128;
		switch (System.Text.Encoding.UTF8.GetString(bytes, 84, 4))
		{
			case "DXT1": desc.Format = Format.BC1_UNorm; pitch = 2; break;
			case "DXT5": desc.Format = Format.BC3_UNorm; break;
			case "ATI1": desc.Format = Format.BC4_UNorm; pitch = 2; break;
			case "DX10": desc.Format = (Format)bytes[128]; offset = 148;
				if (desc.Format == Format.BC1_UNorm_SRgb) pitch = 2; break;
		}

		var data = DataStream.Create(bytes, false, false, offset);
		return new SRV(d, new Texture2D(d, desc, new DataRectangle(data.DataPointer, desc.Width * pitch)));
	}
}