namespace TinyFLVER;

public class Flver
{
	public static FLVER2 flver;

	public static void Initialize()
	{
		form.buttonImportFBX.Click += buttonImportFBX_Click;
		form.buttonSave.Click += buttonSave_Click;

		string[] args = Environment.GetCommandLineArgs();

		if (args.Length > 1) path = args[1];
		if (path is null && form.openFlverDialog.ShowDialog() == DialogResult.OK)
			path = form.openFlverDialog.FileName;
		if (path is null) Environment.Exit(0);

		LoadFLVER(path);
	}

	public static void LoadFLVER(string path)
	{
		try
		{
			flver = FLVER2.Read(path);

			foreach (FLVER2.Mesh mesh in flver.Meshes)
				Meshes.Add(new(mesh, flver.Materials[mesh.MaterialIndex]));

			Textures.ReloadAllAsync();
			form.CreateMeshesUI();

			DisplayFlverVersion(form.comboBoxFlverVersion);
		}
		catch (Exception ex) { MessageBox.Show(ex.Message); }
	}

	public static void ProcessFLVER()
	{
		Meshes.Clear();
		Selected.Clear();
		Hidden.Clear();	
		foreach (FLVER2.Mesh mesh in flver.Meshes)
			Meshes.Add(new(mesh, flver.Materials[mesh.MaterialIndex]));

		Textures.ReloadAllAsync();
		form.CreateMeshesUI();
	}

	private static void buttonImportFBX_Click(object sender, EventArgs e)
	{

		FbxImport.importFBX(form);
		ProcessFLVER();
	}

	private static void buttonSave_Click(object sender, EventArgs e)
	{
		if (!File.Exists(path + ".bak"))
			File.Copy(path, path + ".bak");
		flver.Write(path);
		MessageBox.Show("Saved");
	}

	public static void DeleteMeshes()
	{
		// First we remove all selected meshes
		foreach (var mesh in Selected)
			Meshes.Remove(mesh);
		Selected.Clear();

		// Recreate materials and indices 
		flver.Materials.Clear();
		flver.Meshes.Clear();
		foreach (var mesh in Meshes)
		{
			// Theoretically I need to set GXIndex too, but ER and NR seems to work fine, so I won't do it yet
			flver.Meshes.Add(mesh.flverMesh);
			mesh.flverMesh.MaterialIndex = flver.Materials.Count;
			flver.Materials.Add(mesh.material);
		}

		form.CreateMeshesUI();
	}

	public static void DisplayFlverVersion(ComboBox comboBox)
	{
		Dictionary<int, string> flverVersions = new() {
			{ 131092, "DS III \\ Sekiro \\ BloodBorne"},
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }
		};

		comboBox.Items.Add(flverVersions[flver.Header.Version]);

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy.
		if (flver.Header.Version == 131098)
			comboBox.Items.Add(flverVersions[131105]);
		if (flver.Header.Version == 131105)
			comboBox.Items.Add(flverVersions[131098]);

		comboBox.SelectedIndex = 0;
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = flverVersions.FirstOrDefault(x => x.Value == comboBox.Text).Key;
	}
}
