using System.Runtime.InteropServices;

public partial class MainForm : Form
{
	[DllImport("AcrylicWindow.dll")]
	public static extern void ApplyAcrylic(IntPtr hWnd, int radius = 75, float saturation = 1.5f);
	[DllImport("AcrylicWindow.dll")]
	public static extern void SyncWindowPosition(IntPtr hWnd);

	List<Control> MeshUI = [];

	public MainForm()
	{
		InitializeComponent();
		Globals.form = this;

		ApplyAcrylic(Handle);
		Move += (s, e) => SyncWindowPosition(Handle);
		Load += (s, e) => Activate();

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);

		Exposure.ValueChanged += (_, _) => { psData.Exposure = (float)Exposure.Value; labelExposure.Text = psData.Exposure.ToString("0.000"); };
		Gamma.ValueChanged += (_, _) => { psData.Gamma = (float)Gamma.Value ; labelGamma.Text = psData.Gamma.ToString("0.000"); };

		SwapChainManager.Initialize();
		Shaders.Initialize();
		Camera.Initialize();
		Flver.Initialize();
		ShaderSpheres.Initialize();
		ShaderOutline.Initialize();
		
		debugList.SelectedIndexChanged += (_, _) => { psData.DebugID = debugList.SelectedIndex; };
		debugList.SelectedIndex = 0;

	//	RenderSpheres.Checked = true;
	}

	public void RecreateMeshesUI()
	{
		SuspendLayout();
		form.Controls.Find("UI", false).ToList().ForEach(c => Controls.Remove(c)); 

		int y = 10;
		foreach (Mesh mesh in Meshes) 
		{
			Controls.Add(new TextBox { Name = "UI", Location = new Point(10, y), Size = new(100, 20), Text = mesh.material.Name, });
			Controls.Add(new TextBox { Name = "UI", Location = new Point(120, y), Size = new(140, 20), Text = mesh.material.MTD, });
			Controls.Add(new Button  { Name = "UI", Location = new Point(270, y), Size = new Size(40, 20), Text = "Edit" , Tag = mesh.flverMesh.MaterialIndex });

			var cbSelected = new CheckBox { Name = "UI", Location = new Point(320, y + 5), AutoSize = true, Tag = mesh };
			Controls.Add(cbSelected);
			mesh.cbSelected = cbSelected;

			var cbHidden = new CheckBox {Name = "UI",  Location = new Point(340, y + 5), AutoSize = true, Tag = mesh };
			Controls.Add(cbHidden);
			mesh.cbHidden = cbHidden;

			y += 20;
		}

		Controls.Find("UI", false).ToList().ForEach(c => c.Click += (s, e) => MeshUIClick(c));
		ResumeLayout(true);
	}

	void MeshUIClick(Control sender)
	{
		if (sender.Text == "Edit")
			EditMeshMaterial((int)(sender as Button).Tag);

		if (sender.Location.X == 320)
			SetSelection((sender.Tag as Mesh), (sender as CheckBox).Checked);

		if (sender.Location.X == 340)
			SetVisibility((sender.Tag as Mesh), (sender as CheckBox).Checked);
	}

	void EditMeshMaterial(int materialIndex)
	{
		new MaterialEdit(materialIndex).ShowDialog();
		Textures.ReloadAll();
		RecreateMeshesUI();
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
			Flver.DeleteMeshes();

		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (e.KeyCode == Keys.H || e.KeyCode == Keys.CapsLock)
			Selected.ForEach(m => SetVisibility(m, !m.cbHidden.Checked));

		if (e.KeyCode == Keys.Enter)
			if (Selected.Count == 1)
				EditMeshMaterial(Selected[0].flverMesh.MaterialIndex);
	}

	private void MainForm_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left) return;
		
		if (ModifierKeys == Keys.Alt)
		{
			Meshes.ForEach(m => SetSelection(m, true));
			return;
		}

		if (ModifierKeys == Keys.Control)
		{
			Mesh mesh = Meshes.Where(x => x.CheckIntersection(e.Location) < 1).OrderBy(x => x.CheckIntersection(e.Location)).FirstOrDefault();
			SetSelection(mesh, !Selected.Contains(mesh));
		}
		else
		{
			Mesh mesh = Meshes.Except(Selected).Where(x => x.CheckIntersection(e.Location) < 1).OrderBy(x => x.CheckIntersection(e.Location)).FirstOrDefault();
			Meshes.ForEach(m => SetSelection(m, false));
			SetSelection(mesh, true);
		}
	}

	public void SetSelection(Mesh mesh, bool selected)
	{
		if (mesh is null) return;
		mesh.cbSelected.Checked = selected;
		if (selected) Selected.Add(mesh);
		else Selected.Remove(mesh);
	}

	public void SetVisibility(Mesh mesh, bool hidden)
	{
		if (mesh is null) return;
		mesh.cbHidden.Checked = hidden;
		if (hidden) Hidden.Add(mesh);
		else Hidden.Remove(mesh);
	}
}
