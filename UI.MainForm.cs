using Microsoft.Win32;
using SharpDX.Windows;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
		form = this;

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);
		Location = new Point(Screen.PrimaryScreen.Bounds.Width / 5, 100);
		
		SwapChainManager.Initialize();
		Shaders.Initialize();
		Camera.Initialize();
		SetupHDR();

		Exposure.ValueChanged += (s, e) => shaderData.Exposure = Exposure.Value / 1000f;
		Gamma.ValueChanged += (s, e) => shaderData.Gamma = Gamma.Value / 1000f;

		dataGridMeshes.CurrentCellDirtyStateChanged += (s, e) => dataGridMeshes.CommitEdit(DataGridViewDataErrorContexts.Commit);
		dataGridMeshes.DataError += (s, e) => { };
		dataGridTextures.CurrentCellDirtyStateChanged += (s, e) => { dataGridTextures.CommitEdit(DataGridViewDataErrorContexts.Commit); Texture.Reload(selected); };

		Load += (s, e) => { Show(); Flver.Initialize();  Overlay.Initialize(); };
	}

	static void SetupHDR()
	{
		var monitors = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		var currentMonitor = monitors?.OpenSubKey(monitors?.GetSubKeyNames().Last());
		bool hdrEnabled = currentMonitor?.GetValue("AdvancedColorEnabled")?.ToString() == "1";

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
		shaderData.Exposure = hdrEnabled ? float.Parse(exposure ?? "1000") / 1000f : 1;

		var devices = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
		var currentDevice = devices?.OpenSubKey(devices?.GetSubKeyNames().First());
		var nVidiaContrast = (int)(currentDevice?.OpenSubKey("Color").GetValue("3538950") ?? 100);
		shaderData.Gamma = hdrEnabled ? 1 - (120 - nVidiaContrast) * 0.015f : 1;
	}

	public void BindMeshesDataGrid()
	{
		form.SuspendLayout();
		dataGridMeshes.DataSource = null;
		dataGridMeshes.DataSource = Meshes;
		dataGridMeshes.ClearSelection();
		dataGridMeshes.Width = dataGridMeshes.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridMeshes.Height = dataGridMeshes.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridMeshes.ColumnHeadersHeight;
		buttonImport.Left = buttonOpen.Left = buttonSave.Left = buttonDelete.Left = dataGridMeshes.Width + 25;
		dataGridTextures.Top = Math.Max(dataGridMeshes.Height + 20, 100);
		form.ResumeLayout();
	}

	void MainForm_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
			return;

		if (ModifierKeys == Keys.Alt)
			dataGridMeshes.SelectAll();
		else
		if (ModifierKeys == Keys.Control)
			dataGridMeshes.Rows[Meshes.IndexOf(nearestMesh)]?.Selected ^= true;
		else
		{
			dataGridMeshes.EndEdit();
			dataGridTextures.EndEdit();
			dataGridMeshes.ClearSelection();
			if (nearestMesh != null)
				dataGridMeshes.Rows[Meshes.IndexOf(nearestMesh)].Selected = true;
		}
	}

	void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (dataGridMeshes.IsCurrentCellInEditMode || dataGridTextures.IsCurrentCellInEditMode)
			return;

		if (e.KeyCode == Keys.Delete)
			if (dataGridTextures.Focused) RemoveTexture();
			else Flver.DeleteMeshes();

		if (e.KeyCode == Keys.H)
			Meshes.ForEach(m => m.Hidden ^= m.Selected);

		dataGridMeshes.Refresh();
	}

	public Mesh selected;

	void dataGridTextures_DataSourceChanged(object sender, EventArgs e)
	{
		if (dataGridTextures.DataSource == null) return;
		dataGridTextures.Width = dataGridTextures.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridTextures.Height = dataGridTextures.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridTextures.ColumnHeadersHeight;
		buttonAddTexture.Top = dataGridTextures.Top + 1;
		buttonAddTexture.Left = dataGridTextures.Width + 15;
		dataGridTextures.ClearSelection();
	}

	void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		if (dataGridMeshes.Rows.Count != Meshes.Count) return;
		Meshes.ForEach(m => m.Selected = dataGridMeshes.Rows[Meshes.IndexOf(m)].Selected);
		buttonAddTexture.Visible = dataGridTextures.Visible = dataGridMeshes.SelectedRows.Count == 1;
		selected = Meshes.FirstOrDefault(m => m.Selected);
		dataGridTextures.DataSource = selected?.textureList;
	}

	void buttonAddTexture_Click(object sender, EventArgs e)
	{
		selected.material.Textures.Add(new());
		selected.textureList = [.. selected.material.Textures.Select(t => new Texture(t))];
		dataGridTextures.DataSource = selected.textureList;
	}

	void RemoveTexture()
	{
		selected.material.Textures.RemoveAt(dataGridTextures.SelectedCells[0].RowIndex);
		selected.textureList = [.. selected.material.Textures.Select(t => new Texture(t))];
		dataGridTextures.DataSource = selected.textureList;
		Texture.Reload(selected);
	}

	private void buttonDelete_Click(object sender, EventArgs e)
	{
		dataGridMeshes.SelectAll();
		Flver.DeleteMeshes();
	}
}
