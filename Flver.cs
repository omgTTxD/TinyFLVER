public class Flver
{
	public static string path;

	static OpenFileDialog openFLVER = new() { Filter = "FLVER | *.flver" };

	public static void Initialize()
	{
		form.buttonImportFBX.Click += (s, e) => { if (FbxImport.ImportFBX()) ProcessNewFlver();  };
		form.buttonSave.Click += buttonSave_Click;
		form.buttonLoad.Click += (s, e) => { path = null;  OpenFlver(); };

		//if (path is null) path = "R:\\FLVER Models\\2B\\BD_M_5070.flver";
		//if (path is null) path = "R:\\Nightreign\\Wylder 2B\\parts\\bd_m_5000\\BD_M_5000.flver";

		OpenFlver();
	}

	public static void OpenFlver()
	{
		if (path is null && openFLVER.ShowDialog() == DialogResult.OK)
			path = openFLVER.FileName;

		if (path is null) 
			return;
			
		flver = FLVER2.Read(path);
		var directory = Path.GetDirectoryName(path);
		form.Text = Path.GetFileNameWithoutExtension(path) + " (" + directory + ")";
		Directory.SetCurrentDirectory(directory);
		Texture.CreateFileSystemWatcher();

		form.buttonImportFBX.Enabled = true;
		form.buttonSave.Enabled = true;

		ProcessNewFlver();
	}

	public static void ProcessNewFlver()
	{
		Meshes.Clear();
		flver.Meshes.ForEach(m => Meshes.Add(new(m)));
		Camera.ProjectVertices();
		DisplayFlverVersion();
		Texture.ReloadAll();
		form.BindMeshesDataGrid();
	}

	public static void DeleteMeshes()
	{
		foreach (var mesh in Meshes.Where(m => m.Selected).ToList())
		{
			flver.Meshes.Remove(mesh.mesh); 
			Meshes.Remove(mesh);
		}

		List<FLVER2.Material> newMaterials = [];

		foreach (var mesh in Meshes)
		{
			mesh.mesh.MaterialIndex = mesh.material.Index = newMaterials.Count;
			newMaterials.Add(mesh.material);
		}
		
		flver.Materials = newMaterials;
		form.BindMeshesDataGrid();
	}


	private static void buttonSave_Click(object sender, EventArgs e)
	{
		var backupPath = path.Replace(".flver", "_.flver");
		if (!File.Exists(backupPath))
			File.Copy(path, backupPath, false);
		try
		{
			flver.Write(path);
			MessageBox.Show("Saved");
		}
		catch (Exception ex) { MessageBox.Show(ex.Message); }
	}

	public static void DisplayFlverVersion()
	{
		Dictionary<int, string> versionsDict = new() {
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131091, "DS III \\ Sekiro \\ BB Face" },
			{ 131092, "DS III \\ Sekiro \\ BloodBorne" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }
		};

		var comboBox = form.flverVersions;

		versionsDict.TryGetValue(flver.Header.Version, out var game);
		game = game ?? "Unknown";

		comboBox.Items.Clear(); comboBox.Items.Add(game);
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = versionsDict.FirstOrDefault(x => x.Value == comboBox.Text).Key;
		comboBox.SelectedIndex = 0;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy.
		if (flver.Header.Version == 131098) comboBox.Items.Add(versionsDict[131105]);
		if (flver.Header.Version == 131105) comboBox.Items.Add(versionsDict[131098]);

		// If we have "_s" texture, that means old DSR format, where _s contains roughness, metalness, F0 and emissive. Else it's new ER where "_n" contains glosiness.
	/*	if (game.StartsWith("Dark Souls"))
			psData.newFormat = 0;
		else
			psData.newFormat = 1;*/
	}
}