
using ColorSlider;
using ColorSlider;
using SharpDX.Direct3D11;
using SoulsFormats;
using System.Numerics;
using static TinyFLVER.ShaderManager;
using Buffer = SharpDX.Direct3D11.Buffer;
using Point = System.Drawing.Point;

namespace TinyFLVER;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
	
		Globals.form = this;
		Globals.r = renderControl;
		
		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);

		checkBoxDiffuseSRgb.CheckedChanged += (_, _) => { Meshes.ForEach(Textures.Load); };

		Exposure.ValueChanged += (_, _) => { psData.Exposure = (float)Exposure.Value; labelExposure.Text = psData.Exposure.ToString("0.000"); };
		Gamma.ValueChanged += (_, _) => { psData.Gamma = (float)Gamma.Value; labelGamma.Text = psData.Gamma.ToString("0.000"); };

		SwapChainManager.Initialize();
		ShaderManager.Initialize();
		Camera.Initialize();

		Load += (_, _) => Activate();

		Flver.LoadFLVER();

		sliderX.Tag = labelX;
		sliderY.Tag = labelY;
		sliderZ.Tag = labelZ;
		
		sliderX.ValueChanged += SliderLightCrd_ValueChanged;
		sliderY.ValueChanged += SliderLightCrd_ValueChanged;
		sliderZ.ValueChanged += SliderLightCrd_ValueChanged;

		SliderLightCrd_ValueChanged(sliderX, null);
		psData.Exposure = 1f;
	}

	static Buffer lightPositionBuffer;

	public void CreateMeshesUI()
	{
		panel.Controls.Clear();
		for (int i = 0; i < Meshes.Count; i++)
		{
			var mesh = Meshes[i];
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
			buttonEditMaterial.Click += (s, e) => { EditMeshMaterial(mesh); };

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
			Selected.ForEach(x => x.SetVisibility(!x.cbHidden.Checked));

		if (e.KeyCode == Keys.Space)
			if (Selected.Count == 1)
				EditMeshMaterial(Selected[0]);
	}

	void EditMeshMaterial(Mesh mesh)
	{
		new MaterialEdit(mesh.material.Index).ShowDialog();
		CreateMeshesUI();
	}

	private void SliderLightCrd_ValueChanged(object sender, EventArgs e)
	{
		var slider = sender as ColorSlider.ColorSlider;
		(slider.Tag as Label).Text = slider.Value.ToString();
		psData.L = Vector3.Normalize(new((float)sliderX.Value, (float)sliderY.Value, -(float)sliderZ.Value));
	}

	private void renderControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
			return;

		if (ModifierKeys == Keys.Alt)
		{
			Meshes.ForEach(m => m.SetSelection(true));
			return;
		}

		if (ModifierKeys == Keys.Control)
		{
			Mesh selected = Meshes.Where(x => x.CheckIntersection(e.Location) < 1).FirstOrDefault();
			selected?.SetSelection(!selected.cbSelected.Checked);
		}
		else
		{
			Mesh selected = Meshes.Where(x => x.CheckIntersection(e.Location) < 1).Except(Selected).FirstOrDefault();
			Meshes.ForEach(m => m.SetSelection(false));
			selected?.SetSelection(true);
		}
	}
}
