using System.Windows.Forms;

public class Flver
{
	public static FLVER2 flver;
	public static string path;

	static OpenFileDialog openFLVER = new() { Filter = "FLVER | *.flver; *.bak" };

	public static void Initialize()
	{
		form.buttonImportFBX.Click += (s, e) => { FbxImport.importFBX(form); ProcessNewFlver(); };
		form.buttonSave.Click += buttonSave_Click;
		form.buttonLoad.Click += (s, e) => { path = null;  OpenFlver(); };

		string arg = Environment.GetCommandLineArgs().ElementAtOrDefault(1);

		if (arg is not null) 
			path = arg;

		OpenFlver();
	}

	public static void OpenFlver()
	{
		if (path is null && openFLVER.ShowDialog() == DialogResult.OK)
			path = openFLVER.FileName;

		try
		{
			if (path is null) return;
			flver = FLVER2.Read(path);

			Meshes.Clear();
			Selected.Clear();
			Hidden.Clear();

			ProcessNewFlver();
		}
		catch (Exception ex) { MessageBox.Show(ex.Message); }
	}

	public static void ProcessNewFlver()
	{
		flver.Meshes.ForEach(m => Meshes.Add(new(m, flver.Materials[m.MaterialIndex])));
		Textures.ReloadAll();
		form.RecreateMeshesUI();
		DisplayFlverVersion(form.comboBoxFlverVersion);
	}

	public static void DeleteMeshes()
	{
		// First we remove all selected meshes
		foreach (var mesh in Selected)
		{
			Meshes.Remove(mesh);
			Hidden.Remove(mesh);
		}
		Selected.Clear();

		flver.Materials.Clear();
		flver.Meshes.Clear();

		// Recreate materials and their indices 
		foreach (var mesh in Meshes)
		{
			flver.Meshes.Add(mesh.flverMesh);
			mesh.flverMesh.MaterialIndex = mesh.material.Index = flver.Materials.Count;
			flver.Materials.Add(mesh.material);
		}

		form.RecreateMeshesUI();
	}

	private static void buttonSave_Click(object sender, EventArgs e)
	{
		if (!File.Exists(path + ".bak"))
			File.Copy(path, path + ".bak", false);
		try
		{
			flver.Write(path);
			MessageBox.Show("Saved");
		}
		catch (Exception ex) { MessageBox.Show(ex.Message); }
	}

	public static void DisplayFlverVersion(ComboBox comboBox)
	{
		Dictionary<int, string> flverVersions = new() {
			{ 131092, "DS III \\ Sekiro \\ BloodBorne"},
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }
		};

		comboBox.Items.Clear();
		var	game = flverVersions[flver.Header.Version];
		comboBox.Items.Add(game);

		// If we have "_s" texture, that means old DSR format, where _s contains roughness, metalness, F0 and emissive. Else it's new ER where "_n" contains glosiness.
		if (game.StartsWith("Dark Souls"))
			psData.newFormat = 0;
		else
			psData.newFormat = 1;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy.
		if (flver.Header.Version == 131098)
			comboBox.Items.Add(flverVersions[131105]);
		if (flver.Header.Version == 131105)
			comboBox.Items.Add(flverVersions[131098]);

		comboBox.SelectedIndex = 0;
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = flverVersions.FirstOrDefault(x => x.Value == comboBox.Text).Key;
	}
}