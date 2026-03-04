using SoulsFormats;
using System.Linq;
using System.Runtime.InteropServices;

namespace TinyFLVER;

public partial class MainForm : Form
{
	List<Control> MeshUI = [];

	[DllImport("Acrylic.dll")]
	public static extern void SetAcrylicEffect(IntPtr hWnd);
	[DllImport("Acrylic.dll")]
	public static extern void SyncCoordinates(IntPtr hWnd);

	public MainForm()
	{
		InitializeComponent();
		form = this;

	//	SetAcrylicEffect(Handle);
		Move += (s, e) => SyncCoordinates(Handle);
		Load += (s, e) => Activate();

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Location = new Point(250, 50);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);

		SwapChainManager.Initialize();
		Shaders.Initialize();
		Camera.Initialize();
		Flver.Initialize();
		ShaderSpheres.Initialize();
		ShaderOutline.Initialize();

		Gamma.ValueChanged += (_, _) => { psData.Gamma = (float)Gamma.Value / 100; labelGamma.Text = psData.Gamma.ToString("0.00"); };
		Exposure.ValueChanged += (_, _) => { psData.Exposure = (float)Exposure.Value / 10; labelExposure.Text = psData.Exposure.ToString("0.0"); };

		Gamma.Value = 80;
		Exposure.Value = 10;
		debugList.SelectedIndex = 0;

		this.HelpButton = true;
		this.MaximizeBox = false;
		this.MinimizeBox = false;
		this.HelpButtonClicked += MainForm_HelpButtonClicked;
	}

	private void MainForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
	{
		MessageBox.Show("- Alt+LMB - select all meshes, \r\n- Ctrl+LMB - add/remove to selection.\r\n- H - hide meshes.\r\n- Delete - delete meshes.\r\n- Space - edit selected mesh's material.");
		e.Cancel = true;
	}

	public void CreateMeshesUI()
	{
		MeshUI.ForEach(x => form.Controls.Remove(x));
		MeshUI.Clear();
		for (int i = 0; i < Meshes.Count; i++)
		{
			var mesh = Meshes[i];
			int y = 20 * i + 10;

			var mat = mesh.material;
			MeshUI.Add(new TextBox { Size = new Size(100, 15), Location = new Point(10, y), Text = mat.Name });
			MeshUI.Add(new TextBox { Size = new Size(100, 15), Location = new Point(120, y), Text = mat.MTD, });

			mesh.cbSelected = new() { Size = new Size(15, 15), Location = new Point(280, y + 5) };
			mesh.cbSelected.Click += (s, e) => mesh.SetSelection(mesh.cbSelected.Checked);
			MeshUI.Add(mesh.cbSelected);

			mesh.cbHidden = new() { Size = new Size(15, 15), Location = new Point(300, y + 5) };
			mesh.cbHidden.Click += (s, e) => mesh.SetVisibility(mesh.cbHidden.Checked);
			MeshUI.Add(mesh.cbHidden);

			Button buttonEditMaterial = new() { Text = "Edit", Size = new Size(40, 20), Location = new Point(230, y) };
			buttonEditMaterial.Click += (s, e) => { EditMeshMaterial(mesh); };
			MeshUI.Add(buttonEditMaterial);
		}
		MeshUI.ForEach(x => form.Controls.Add(x));
	}

	void EditMeshMaterial(Mesh mesh)
	{
		new MaterialEdit(mesh.flverMesh.MaterialIndex).ShowDialog();
		Textures.ReloadAllAsync();
		CreateMeshesUI();
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
			Flver.DeleteMeshes();

		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (e.KeyCode == Keys.H || e.KeyCode == Keys.CapsLock)
			Selected.ForEach(x => x.SetVisibility(!x.cbHidden.Checked));

		if (e.KeyCode == Keys.Space)
			if (Selected.Count == 1)
				EditMeshMaterial(Selected[0]);
	}

	private void MainForm_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left) return;
		
		if (ModifierKeys == Keys.Alt)
		{
			Meshes.ForEach(x => x.SetSelection(true));
			return;
		}

		if (ModifierKeys == Keys.Control)
		{
			Mesh mesh = Meshes.Where(x => x.CheckIntersection(e.Location) < 1).OrderBy(x => x.CheckIntersection(e.Location)).FirstOrDefault();
			mesh?.SetSelection(!Selected.Contains(mesh));
		}
		else
		{
			Mesh mesh = Meshes.Except(Selected).Where(x => x.CheckIntersection(e.Location) < 1).OrderBy(x => x.CheckIntersection(e.Location)).FirstOrDefault();
			Meshes.ForEach(m => m.SetSelection(false));
			mesh?.SetSelection(true);
		}
	}
}
