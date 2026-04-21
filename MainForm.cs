using SoulsFormats;

namespace TinyFLVER;

public partial class MainForm : Form
{
	public static FLVER2 flver;
	public static string path = "";

	public MainForm()
	{
		InitializeComponent();

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Location = new Point(250, 50);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);

		string[] args = Environment.GetCommandLineArgs();

		if (args.Length > 1)
			path = args[1];
		else
			if (openFlverDialog.ShowDialog() == DialogResult.OK)
			path = openFlverDialog.FileName;

		if (path == "")
			Environment.Exit(0);

		// Make the directory of flver current working directory. It is needed mostly for textures search.
		Directory.SetCurrentDirectory(Path.GetDirectoryName(path));
	}

	// Activate() should happen on Load, or form will be inactive.
	private void MainForm_Load(object sender, EventArgs e)
	{
		Activate();
		flver = FLVER2.Read(path);
		ProcessFlver();
	}

	// Add TextBoxes and CheckBoxes to form, one for each mesh
	public void ProcessFlver()
	{
		monoGame.meshes.Clear();
		foreach (var x in flver.Meshes)
			monoGame.meshes.Add(new Mesh(x, flver.Materials[x.MaterialIndex]));

		panel.Controls.Clear();
		for (int i = 0; i < monoGame.meshes.Count; i++)
		{
			var mesh = monoGame.meshes[i];
			int y = 20 * i + 10;

			var mat = mesh.material;
			panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(10, y), Text = mat.Name });
			panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(120, y), Text = mat.MTD, Visible = true });

			CheckBox cb = new() { Size = new Size(15, 15), Location = new Point(280, y + 5) };
			mesh.checkBox = cb;
			cb.Click += (s, e) => { mesh.SetSelectionState(cb.Checked); };
			panel.Controls.Add(cb);

			Button buttonEditMaterial = new() { Text = "Edit", Size = new Size(40, 20), Location = new Point(230, y) };
			buttonEditMaterial.Click += (s, e) => { new MaterialEdit(flver, mesh.flverMesh.MaterialIndex).ShowDialog(); monoGame.meshes.ForEach(x => Task.Run(x.TryToFindTextures)); };
			panel.Controls.Add(buttonEditMaterial);
		}
	}

	private void buttonSave_Click(object sender, EventArgs e)
	{
		var backupPath = path.Replace(".flver", "_.flver");
		if (!File.Exists(backupPath))
			File.Copy(path, backupPath, false);

		flver.Write(path);
		MessageBox.Show("Saved");
	}

	void DeleteMeshes()
	{
		List<FLVER2.Material> newMaterials = [];

		foreach (var mesh in monoGame.GetSelectedMeshes())
		{
			monoGame.meshes.Remove(mesh);
			flver.Meshes.Remove(mesh.flverMesh);
		}

		foreach (var mesh in monoGame.meshes)
		{
			mesh.flverMesh.MaterialIndex = mesh.material.Index = newMaterials.Count;
			newMaterials.Add(mesh.material);
		}

		flver.Materials = newMaterials;
		ProcessFlver();
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
			DeleteMeshes();

		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (e.KeyCode == Keys.Space && monoGame.GetSelectedMeshes().Length == 1) {
			new MaterialEdit(flver, monoGame.GetSelectedMeshes()[0].material.Index).ShowDialog();

			// We change only one mesh, but other meshes could have same texture, so we must refresh all
			monoGame.meshes.ForEach(x => Task.Run(x.TryToFindTextures));
		}  
	}

	private void monoControl_MouseClick(object sender, MouseEventArgs e) 
	{
		if (e.Button != MouseButtons.Left)
			return;

		// Select all meshes if Alt is held
		if (ModifierKeys == System.Windows.Forms.Keys.Alt)
		{
			monoGame.meshes.ForEach(mesh => mesh.SetSelectionState(true));
			return;
		}

		// Clear all before selection something new if Control isn't held
		if (ModifierKeys == System.Windows.Forms.Keys.Control) {
			Mesh selected = monoGame.meshes.Where(m => m.CheckIntersection(e.Location) < 1).MinBy(m => m.CheckIntersection(e.Location));
			selected?.SetSelectionState(!selected.checkBox.Checked);
		}
		else {
			// If several meshes located in clicked point, select one of the new meshes
			Mesh selected = monoGame.meshes.Where(m => m.CheckIntersection(e.Location) < 1 && !monoGame.GetSelectedMeshes().Contains(m)).MinBy(m => m.CheckIntersection(e.Location));
			monoGame.meshes.ForEach(mesh => mesh.SetSelectionState(false));
			selected?.SetSelectionState(true);
		}
	}

	private void buttonImport_Click(object sender, EventArgs e)
	{
		if (FbxImport.importFBX(this))
			ProcessFlver();
	}
}