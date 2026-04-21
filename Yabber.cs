using System.Xml;

public class Yabber
{
	public static void UnpackBnd(string filePath)
	{
		string outputDir = Path.GetDirectoryName(filePath) + "\\" + Path.GetFileName(filePath).Split('.')[0];// Replace(".partsbnd.dcx", "");
		Directory.CreateDirectory(outputDir);
		Directory.SetCurrentDirectory(outputDir);

		if (DCX.Is(filePath))
		{
			byte[] bytes = DCX.Decompress(filePath, out DCX.Type compression);

			if (BND4.Is(filePath))
				Unpack(new BND4Reader(bytes) { Compression = compression } , filePath, outputDir);

			if (BND3.Is(filePath))
				Unpack(new BND3Reader(bytes) { Compression = compression }, filePath, outputDir);
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

			var bytes = bnd.ReadFile(file);

			if (Path.GetExtension(file.Name) == ".tpf")
			{
				var tpf = TPF.Read(bytes);
				xw.WriteElementString("compression", tpf.Compression.ToString());
				xw.WriteElementString("encoding", tpf.Encoding.ToString());
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
			byte[] bytes;
			if (innerPath.EndsWith(".tpf"))
			{
				var compression = fileNode.SelectSingleNode("compression").InnerText;
				var encoding = byte.Parse(fileNode.SelectSingleNode("encoding").InnerText);
				bytes = RepackTPF(compression, encoding);
			}
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

	public static void Unpack(BND3Reader bnd, string sourceName, string targetDir)
	{
		var xws = new XmlWriterSettings();
		xws.Indent = true;
		var xw = XmlWriter.Create($"{targetDir}\\_.yab", xws);
		xw.WriteStartElement("bnd3");
		xw.WriteElementString("filename", Path.GetFileName(sourceName));
		xw.WriteElementString("compression", bnd.Compression.ToString());
		WriteBinderFiles(bnd, xw, targetDir);
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
		if (xml.SelectSingleNode("bnd3") is not null)
		{
			var bnd = new BND3();
			filename = xml.SelectSingleNode("bnd3/filename").InnerText;
			Enum.TryParse(xml.SelectSingleNode("bnd3/compression")?.InnerText ?? "None", out compression);
			bnd.Compression = compression;
			ReadBinderFiles(bnd, xml.SelectSingleNode("bnd3/files"), sourceDir);
			bytes = bnd.Write(compression);
		}

		if (xml.SelectSingleNode("tpf") is not null)
		{
			string compession = xml.SelectSingleNode("tpf/compression")?.InnerText ?? "None";
			byte encoding = byte.Parse(xml.SelectSingleNode("tpf/encoding").InnerText);
			bytes = RepackTPF(compession, encoding);

			filename = xml.SelectSingleNode("tpf/filename").InnerText;
		}

		string outPath = $"{targetDir}\\{filename}";
		string backupPath = outPath.Replace(".dcx", "_.dcx");
		if (outPath.EndsWith(".tpf")) backupPath = outPath.Replace(".tpf", "_.tpf");

		if (!File.Exists(backupPath))
			File.Copy(outPath, backupPath, false);
		File.WriteAllBytes(outPath, bytes);
	}


	public static void UnpackStandaloneTPF(string filePath)
	{
		var tpf = TPF.Read(filePath);

		string outputDir = filePath.Replace(".tpf.dcx", "").Replace(".tpf", "");
		Directory.CreateDirectory(outputDir);
		Directory.SetCurrentDirectory(outputDir);

		var xw = XmlWriter.Create("_.yab", new XmlWriterSettings() { Indent = true });
		xw.WriteStartElement("tpf");
		xw.WriteElementString("filename", Path.GetFileName(filePath));
		xw.WriteElementString("compression", tpf.Compression.ToString());
		xw.WriteElementString("encoding", tpf.Encoding.ToString());
		xw.WriteEndElement();
		xw.Close();

		UnpackTPF(tpf);
	}

	public static void UnpackTPF(TPF tpf)
	{
		foreach (var dds in tpf.Textures)
			File.WriteAllBytes(Path.GetFileName(dds.Name) + ".dds", dds.Bytes);
	}


	public static byte[] RepackTPF(string compressionString, byte encoding = 1)
	{
		Enum.TryParse(compressionString, out DCX.Type compression);
		TPF tpf = new TPF() { Compression = compression, Encoding = encoding };

		foreach (string path in Directory.GetFiles(".", "*.dds"))
		{
			string fileName = Path.GetFileNameWithoutExtension(path);
			tpf.Textures.Add(new TPF.Texture(fileName, 0, 0, File.ReadAllBytes(fileName + ".dds"), TPF.TPFPlatform.PC));
		}

		return tpf.Write();
	}
}