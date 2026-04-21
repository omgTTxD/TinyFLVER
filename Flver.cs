using SoulsFormats;
using static SoulsFormats.DRB.Shape;

namespace TinyFLVER;

public class Flver
{
	public static List<Mesh> Meshes = [];
	public static List<Mesh> Selected = [];
	public static List<Mesh> Hidden = [];

	public static string path;
	public static FLVER2 flver;

	public static MainForm form;

public async static void LoadTextures()
	{
		foreach (Mesh m in Meshes)
			Task.Run(m.LoadTextures);
	} 

	public static void LoadFLVER(MainForm form)
	{
		Flver.form = form;

		form.buttonImportFBX.Click += buttonImportFBX_Click;
		form.buttonSave.Click += buttonSave_Click;

		string[] args = Environment.GetCommandLineArgs();

		if (args.Length > 1)
			path = args[1];

		if (path is null && form.openFlverDialog.ShowDialog() == DialogResult.OK)
			path = form.openFlverDialog.FileName;

		if (path is null)
			Environment.Exit(0);

		flver = FLVER2.Read(path);

		foreach (FLVER2.Mesh mesh in flver.Meshes)
			Meshes.Add(new Mesh(mesh, flver.Materials[mesh.MaterialIndex]));

		DisplayFlverVersion(form.comboBoxFlverVersion);
		form.CreateMeshesUI();
	}

	private static void buttonImportFBX_Click(object sender, EventArgs e)
	{
		if (FbxImport.importFBX(form))
		{
			foreach (FLVER2.Mesh mesh in flver.Meshes)
				Meshes.Add(new Mesh(mesh, flver.Materials[mesh.MaterialIndex]));

			form.CreateMeshesUI();
		}
	}

	public static void ReloadTextures()
	{
		Meshes.ForEach(x => Task.Run(() => { x.LoadTextures(); }));
	}
	private static void buttonSave_Click(object sender, EventArgs e)
	{
		var backupPath = path.Replace(".flver", "_.flver");
		if (!File.Exists(backupPath))
			File.Copy(path, backupPath);

		flver.Write(path);
		MessageBox.Show("Saved");
	}

	public static void DeleteMeshes()
	{
		// First we remove all selected meshes
		foreach (var mesh in Selected)
		{
			Meshes.Remove(mesh);
			flver.Meshes.Remove(mesh.flverMesh);
		}

		// Materials are stored in mesh.Material, so we can safely clear flver.Materials
		flver.Materials.Clear();
		Selected.Clear();

		// Recreate materials and indices 
		foreach (var mesh in Meshes)
		{
			mesh.flverMesh.MaterialIndex = mesh.material.Index = flver.Materials.Count;
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

		comboBox.SelectedIndexChanged += (s, e) =>
		{
			flver.Header.Version = flverVersions.FirstOrDefault(x => x.Value == (s as ComboBox).Text).Key;
		};
	}

}
