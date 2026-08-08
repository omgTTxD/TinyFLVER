using System.Xml;

public class Yabber
{
	public static void OpenWithYabber(string path)
	{
		if (path.EndsWith(".yab"))
			Yabber.Repack(path);
		else if (path.EndsWith("bnd.dcx"))
			Yabber.Unpack(path);
		else if (path.EndsWith(".tpf.dcx") || path.EndsWith(".tpf"))
			UnpackStandaloneTPF(path);
	}

	static void Unpack(string path)
	{
		string outputDir = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path).Replace(".partsbnd", "");
		Directory.CreateDirectory(outputDir); Directory.SetCurrentDirectory(outputDir);
		byte[] bytes = DCX.Decompress(path, out DCX.Type compression);
		BinderReader bnd = BND4.Is(bytes) ? new BND4Reader(bytes) : new BND3Reader(bytes);

		XmlWriter xw = XmlWriter.Create("_.yab", new() { Indent = true, ConformanceLevel = ConformanceLevel.Auto });
		xw.WriteStartElement("container");
		xw.WriteAttributeString("type", BND4.Is(bytes) ? "bnd4" : "bnd3");
		xw.WriteAttributeString("filename", Path.GetFileName(path));
		xw.WriteAttributeString("compression", compression.ToString());
		foreach (var file in bnd.Files)
		{
			xw.WriteStartElement("file");
			xw.WriteAttributeString("id", file.ID.ToString());
			xw.WriteAttributeString("path", file.Name);
			if (Path.GetExtension(file.Name) == ".tpf")
				UnpackTPF(TPF.Read(bnd.ReadFile(file)), xw);
			else
				File.WriteAllBytes(Path.GetFileName(file.Name), bnd.ReadFile(file));
			xw.WriteEndElement();
		}
		xw.WriteEndElement();
		xw.Close();
	}

	public static void Repack(string path)
	{
		Directory.SetCurrentDirectory(Path.GetDirectoryName(path));
		var xml = new XmlDocument(); xml.Load("_.yab");
		XmlNode node = xml.SelectSingleNode("container");
		Enum.TryParse(node.Attributes["compression"]?.InnerText ?? "None", out DCX.Type compression);
		string type = node.Attributes["type"].InnerText;
		byte[] bytes = (type == "bnd4" || type == "bnd3") ? RepackFilesToBND(type, compression, node) : RepackTPF(node).Write();
		File.WriteAllBytes($"{Directory.GetParent(Path.GetDirectoryName(path))}\\{node.Attributes["filename"].InnerText}", bytes);
	}

	static byte[] RepackFilesToBND(string type, DCX.Type compression, XmlNode xml)
	{
		IBinder bnd = type == "bnd4" ? new BND4() { Compression = compression } : new BND3() { Compression = compression };
		foreach (XmlNode node in xml.SelectNodes("file"))
		{
			string innerPath = node.Attributes["path"].InnerText; byte[] bytes = [];
			bytes = innerPath.EndsWith(".tpf") ? RepackTPF(node).Write() : File.ReadAllBytes(Path.GetFileName(innerPath));
			bnd.Files.Add(new BinderFile(Binder.FileFlags.Flag1, int.Parse(node.Attributes["id"].InnerText), innerPath, bytes));
		}
		return type == "bnd4" ? (bnd as BND4).Write(compression) : (bnd as BND3).Write(compression);
	}

	static void UnpackStandaloneTPF(string path)
	{
		var tpf = TPF.Read(File.ReadAllBytes(path));
		string outputDir = path.Replace(".tpf.dcx", "").Replace(".tpf", "");
		Directory.CreateDirectory(outputDir); Directory.SetCurrentDirectory(outputDir);
		var xw = XmlWriter.Create("_.yab", new XmlWriterSettings() { Indent = true, ConformanceLevel = ConformanceLevel.Auto });
		xw.WriteStartElement("container");
		xw.WriteAttributeString("type", "tpf");
		xw.WriteAttributeString("filename", Path.GetFileName(path));
		UnpackTPF(tpf, xw);
		xw.Close();
	}

	static void UnpackTPF(TPF tpf, XmlWriter xw)
	{
		if (tpf.Compression != DCX.Type.None) xw.WriteAttributeString("compression", tpf.Compression.ToString());
		xw.WriteAttributeString("encoding", tpf.Encoding.ToString());
		foreach (var dds in tpf.Textures)
			File.WriteAllBytes(Path.GetFileName(dds.Name) + ".dds", dds.Bytes);
	}

	static TPF RepackTPF(XmlNode node)
	{
		Enum.TryParse(node.Attributes["compression"]?.InnerText ?? "None", out DCX.Type compression);
		TPF tpf = new TPF() { Compression = compression, Encoding = byte.Parse(node.Attributes["encoding"].InnerText) };
		foreach (string path in Directory.GetFiles(".", "*.dds"))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			tpf.Textures.Add(new TPF.Texture(fileName, 0, 0, File.ReadAllBytes(fileName + ".dds"), TPF.TPFPlatform.PC));
		}
		return tpf;
	}
}