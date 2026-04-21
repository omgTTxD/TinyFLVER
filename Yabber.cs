using System.Xml;

public class Yabber
{
	public static void UnpackBnd(string filePath)
	{
		string outputDir = filePath.Replace(".partsbnd.dcx", "");
		Directory.CreateDirectory(outputDir);
		Directory.SetCurrentDirectory(outputDir);

		if (DCX.Is(filePath))
		{
			byte[] bytes = DCX.Decompress(filePath, out DCX.Type compression);

			if (BND4.Is(bytes))
			{
				BND4Reader bnd = new BND4Reader(bytes) { Compression = compression };
				Unpack(bnd, filePath, outputDir);
			}
		}
	}


	public static void WriteBinderFiles(BinderReader bnd, XmlWriter xw, string targetDir)
	{
		xw.WriteStartElement("files");
		foreach (var file in bnd.Files)
		{
			xw.WriteStartElement("file");
			xw.WriteElementString("id", file.ID.ToString());
			xw.WriteElementString("path", file.Name);

			var bytes = (bnd.ReadFile(file));

			if (Path.GetExtension(file.Name) == ".tpf")
			{
				var tpf = TPF.Read(bytes);
				xw.WriteElementString("compression", tpf.Compression.ToString());
				UnpackTPF(tpf);
			}
			else
				File.WriteAllBytes(Path.GetFileName(file.Name), bytes);

			xw.WriteEndElement();
		}
		xw.WriteEndElement();
	}

	public static void ReadBinderFiles(IBinder bnd, XmlNode filesNode, string sourceDir)
	{
		foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
		{
			string id = fileNode.SelectSingleNode("id")?.InnerText ?? "-1";
			string innerPath = fileNode.SelectSingleNode("path").InnerText;

			Byte[] bytes;
			if (innerPath.EndsWith(".tpf"))
				bytes = RepackTPF(fileNode.SelectSingleNode("compression").InnerText);
			else
				bytes = File.ReadAllBytes(Path.GetFileName(innerPath));

			bnd.Files.Add(new BinderFile(Binder.FileFlags.Flag1, int.Parse(id), innerPath, bytes));
		}

	}

	public static void Unpack(BND4Reader bnd, string filePath, string outputDir)
	{
		var xws = new XmlWriterSettings();
		xws.Indent = true;
		var xw = XmlWriter.Create($"{outputDir}\\_.yab", xws);
		xw.WriteStartElement("bnd4");
		xw.WriteElementString("filename", Path.GetFileName(filePath));
		xw.WriteElementString("compression", bnd.Compression.ToString());
		WriteBinderFiles(bnd, xw, outputDir);
		xw.WriteEndElement();
		xw.Close();
	}

	public static void Repack(string filePath)
	{
		var sourceDir = Path.GetDirectoryName(filePath);
		Directory.SetCurrentDirectory(sourceDir);
		var targetDir = Directory.GetParent(sourceDir);

		var xml = new XmlDocument();
		xml.Load($"{sourceDir}\\_.yab");
		DCX.Type compression;

		byte[] bytes = [];
		string filename = "";

		if (xml.SelectSingleNode("bnd4") is not null)
		{
			var bnd = new BND4();
			filename = xml.SelectSingleNode("bnd4/filename").InnerText;
			Enum.TryParse(xml.SelectSingleNode("bnd4/compression")?.InnerText ?? "None", out compression);
			bnd.Compression = compression;
			ReadBinderFiles(bnd, xml.SelectSingleNode("bnd4/files"), sourceDir);
			bytes = bnd.Write(compression);
		}

		if (xml.SelectSingleNode("tpf") is not null)
		{
			bytes = RepackTPF(xml.SelectSingleNode("tpf/compression")?.InnerText ?? "None");
			filename = xml.SelectSingleNode("tpf/filename").InnerText;
		}

		string outPath = $"{targetDir}\\{filename}";
		string backupPath = outPath.Replace(".dcx", "_.dcx");

		if (!File.Exists(backupPath))
			File.Copy(outPath, backupPath, false);
		File.WriteAllBytes(outPath, bytes);
	}


	public static void UnpackStandaloneTPF(string filePath)
	{
		var tpf = TPF.Read(filePath);

		string outputDir = filePath.Replace(".tpf.dcx", "");
		Directory.CreateDirectory(outputDir);
		Directory.SetCurrentDirectory(outputDir);

		var xw = XmlWriter.Create("_.yab", new XmlWriterSettings() { Indent = true });
		xw.WriteStartElement("tpf");
		xw.WriteElementString("filename", Path.GetFileName(filePath));
		xw.WriteElementString("compression", tpf.Compression.ToString());
		xw.WriteEndElement();
		xw.Close();

		UnpackTPF(tpf);
	}

	public static void UnpackTPF(TPF tpf)
	{
		foreach (var dds in tpf.Textures)
			File.WriteAllBytes(Path.GetFileName(dds.Name) + ".dds", dds.Bytes);
	}


	public static byte[] RepackTPF(string compressionString)
	{
		Enum.TryParse(compressionString, out DCX.Type compression);
		TPF tpf = new TPF() { Compression = compression };

		foreach (string path in Directory.GetFiles(".", "*.dds"))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			tpf.Textures.Add(new TPF.Texture(fileName, 0, 0, File.ReadAllBytes(fileName + ".dds"), TPF.TPFPlatform.PC));
		}

		return tpf.Write();
	}
}
