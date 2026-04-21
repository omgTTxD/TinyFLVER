using SharpDX.Windows;
using Point = System.Drawing.Point;

namespace TinyFLVER;

public partial class MainForm : RenderForm
{
	public MainForm()
	{
		InitializeComponent();

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);
		SizeChanged += (_, _) => { if (WindowState == FormWindowState.Maximized) SwapChainManager.RecreateBuffers(null, null); };
		checkBoxDiffuseSRgb.CheckedChanged += (_, _) => { Textures.DiffuseSRgb = checkBoxDiffuseSRgb.Checked; Flver.LoadTextures(); };

		SwapChainManager.Initialize(this);
		Camera.Initialize(renderControl);
		Shaders.Initialize(this);
		OutlineEffect.Initialize();

		Load += (_, _) => Activate();
		Flver.LoadFLVER(this);
	}

	public void CreateMeshesUI()
	{
		panel.Controls.Clear();
		for (int i = 0; i < Flver.Meshes.Count; i++)
		{
			var mesh = Flver.Meshes[i];
			int y = 20 * i + 10;

			var mat = mesh.material;
			panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(10, y), Text = mat.Name });
			panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(120, y), Text = mat.MTD });

			mesh.cbSelected = new() { Size = new Size(15, 15), Location = new Point(280, y + 5) };
			mesh.cbSelected.Click += (s, e) => mesh.SetSelection(mesh.cbSelected.Checked);
			panel.Controls.Add(mesh.cbSelected);
			
			mesh.cbHidden = new() { Size = new Size(15, 15), Location = new Point(300, y + 5) };
			mesh.cbHidden.Click += (s, e) => mesh.SetVisibility(mesh.cbHidden.Checked);
			panel.Controls.Add(mesh.cbHidden);

			Button buttonEditMaterial = new() { Text = "Edit", Size = new Size(40, 20), Location = new Point(230, y) };
			buttonEditMaterial.Click += (s, e) => EditMeshMaterial(mesh);

			panel.Controls.Add(buttonEditMaterial);
		}
	}


	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete)
			Flver.DeleteMeshes();

		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (e.KeyCode == Keys.H)
			Flver.Selected.ForEach(x => x.SetVisibility(!x.cbHidden.Checked));

		if (e.KeyCode == Keys.Space)
			if (Flver.Selected.Count == 1)
				EditMeshMaterial(Flver.Selected[0]);
	}



	private void renderControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
			return;

		if (ModifierKeys == Keys.Alt)
		{
			Flver.Meshes.ForEach(m => m.SetSelection(true));
			return;
		}

		if (ModifierKeys == Keys.Control)
		{
			Mesh selected = Flver.Meshes.Where(x => x.CheckIntersection(e.Location) < 1).FirstOrDefault();
			selected?.SetSelection(!selected.cbSelected.Checked);
		}
		else
		{
			Mesh selected = Flver.Meshes.Where(x => x.CheckIntersection(e.Location) < 1).Except(Flver.Selected).FirstOrDefault();
			Flver.Meshes.ForEach(m => m.SetSelection(false));
			selected?.SetSelection(true);
		}
	}

	void EditMeshMaterial(Mesh mesh)
	{
		new MaterialEdit(mesh.material).ShowDialog();
		Flver.LoadTextures();
	}
}
