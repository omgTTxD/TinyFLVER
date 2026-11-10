using Newtonsoft.Json;

class Flver
{
	public static string path;
	public static string directory;
	public static string game = "";

	static OpenFileDialog openDialog = new() {
		InitialDirectory = Path.GetDirectoryName(Flver.path),
		Filter = "Files|*.flver;*.dcx;*.tpf;*.yab"
	};

	public static void Initialize()
	{
		form.buttonCopyMaterial.Click += (s, e) => CopyMaterial();
		form.buttonPasteMaterial.Click += (s, e) => PasteMaterial();
		
		form.buttonOpen.Click += (s, e) => OpenFileClicked();
		form.buttonSave.Click += (s, e) => Save();

		RecentLocation.Initialize();

		//Flver.path = "R:\\Dark Souls III\\#\\parts\\fc_f_0000\\FC_F_0000_.flver";
		LoadFLVER(Flver.path ?? form.RecentFiles.SelectedValue?.ToString());

		form.flverVersions.SelectedIndexChanged += (s, e) => 
			flver.Header.Version = versionsDict.FirstOrDefault(x => x.Value == form.flverVersions.Text).Key;

	}

	public static void LoadFLVER(string flverPath)
	{
		if (flverPath is null) 
			return;

		path = flverPath;
		RecentLocation.MoveToTop(flverPath);

		try { flver = FLVER2.Read(path); }
		catch (Exception e)
		{
			MessageBox.Show(e.Message);
			RecentLocation.dataSource.RemoveAt(form.RecentFiles.SelectedIndex);
			return;
		}

		if (Math.Max(flver.Header.BoundingBoxMax.X, flver.Header.BoundingBoxMax.Y) > 2)
		{
			Camera.light.z = Math.Max(flver.Header.BoundingBoxMax.X, flver.Header.BoundingBoxMax.Y);
			Camera.RecalculateMatrices();
		}
		
		directory = Path.GetDirectoryName(path);
		Directory.SetCurrentDirectory(directory);
		form.importDialog.InitialDirectory = directory;

		form.Text = $"{Path.GetFileName(path)} ({directory})";
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;

		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		
		Textures.PrepareForNewFlver();
		DisplayFlverVersion();
		CameraSettings.Load();

		form.dgMeshes.DataSource = Meshes;

		// To navigate saved locations using keyboard, otherwise shaders listbox will be focused.
		form.RecentFiles.Focus();
	}

	static void OpenFileClicked()
	{
		if (openDialog.ShowDialog() != DialogResult.OK)
			return;

		if (openDialog.FileName.ContainsAny(".dcx", ".tpf"))
		{
			Yabber.OpenWithYabber(openDialog.FileName);
			MessageBox.Show("Unpacked.");
		}

		else if (openDialog.FileName.EndsWith(".yab"))
		{
			Yabber.OpenWithYabber(openDialog.FileName);
			MessageBox.Show("Repacked.");
		}

		else
			LoadFLVER(openDialog.FileName);
	}

	public static void DeleteMeshes()
	{
		selectedMeshes.ForEach(m => { flver.Meshes.Remove(m.mesh); Meshes.Remove(m); });
		
		List<FLVER2.Material> newMaterials = [];
		foreach (var mesh in flver.Meshes)
		{
			var material = flver.Materials[mesh.MaterialIndex];
			mesh.MaterialIndex = material.Index = newMaterials.Count;
			newMaterials.Add(material);
		}
	
		flver.Materials = newMaterials;
	}

	static void Save()
	{
		Cursor.Current = Cursors.AppStarting;

		var backup = path.Replace(".flver", "_.flver");
		if (!File.Exists(backup))
			File.Copy(path, backup, false);

		flver.Write(path);

	//	if (File.Exists("_.yab"))
	//		Yabber.Repack(path.Replace(".flver", "_.yab"));

		Cursor.Current = Cursors.Default;
	}

	static void CopyMaterial()
	{
		if (selectedMeshes.Count == 1)
			Clipboard.SetText(JsonConvert.SerializeObject(selectedMeshes[0].mat));
	}

	static void PasteMaterial()
	{
		try	{
			var currentMaterial = selectedMeshes[0].mat;
			var newMaterial = JsonConvert.DeserializeObject<FLVER2.Material>(Clipboard.GetText());
			currentMaterial.Textures = newMaterial.Textures;
			currentMaterial.MTD = newMaterial.MTD;

			form.dgMeshes.Refresh();
			form.dgTextures.DataSource = new BindingList<FLVER2.Texture>(currentMaterial.Textures);
			selectedMeshes[0].ReloadTextures();
		}
		catch (Exception) { MessageBox.Show("Couldn't paste material."); }
	}

	static Dictionary<int, string> versionsDict = new() {
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131092, "DS III \\ BloodBorne \\ Sekiro" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }};

	// For Elden Ring it is neccessary to set correct FLVER version. Nightreign can read both NR and ER flvers.
	static void DisplayFlverVersion()
	{
		var comboBox = form.flverVersions;
		versionsDict.TryGetValue(flver.Header.Version, out game);
		game = game ?? "Unknown";

		comboBox.Items.Clear();	comboBox.Items.Add(game);
		form.flverVersions.SelectedIndex = 0;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy. There are different
		// checks for Flver integrity, but I didn't test it after first try. All those CHECKS can and will be eventually wiped out anyway. Let the game
		// decide what it can render and what it can't...
		// Just kidding, game won't decide anything too, of course all checks that I can reach will be deleted too.
		if (flver.Header.Version == 131098) comboBox.Items.Add(versionsDict[131105]);
		if (flver.Header.Version == 131105) comboBox.Items.Add(versionsDict[131098]);
		if (flver.Header.Version == 0) comboBox.Items.Add("Dark Souls \\ DS Remastered");
	}
}