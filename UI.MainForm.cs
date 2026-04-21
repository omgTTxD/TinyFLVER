public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
		Globals.form = this;

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);
		Location = new Point(Screen.PrimaryScreen.Bounds.Width / 5, 50);

		Exposure.ValueChanged += (_, _) => { shaderData.Exposure = (float)Exposure.Value; labelExposure.Text = shaderData.Exposure.ToString("0.000"); };
		Gamma.ValueChanged += (_, _) => { shaderData.Gamma = (float)Gamma.Value; labelGamma.Text = shaderData.Gamma.ToString("0.000"); };

		Camera.Initialize();
		SwapChainManager.Initialize();
		Shaders.Initialize();
		
		dataGridMeshes.CurrentCellDirtyStateChanged += (s, e) => dataGridMeshes.CommitEdit(DataGridViewDataErrorContexts.Commit);
		dataGridTextures.CurrentCellDirtyStateChanged += (s, e) =>
		{
			if (dataGridTextures.CurrentCell.ColumnIndex == 1)
				dataGridTextures.CommitEdit(DataGridViewDataErrorContexts.Commit);
		};
		dataGridTextures.CellValueChanged += (s, e) => Texture.Reload();

		Load += (s, e) => { Activate(); Flver.Initialize(); };
	}

	public void BindMeshesDataGrid()
	{
		dataGridMeshes.DataSource = null;       
		dataGridMeshes.DataSource = Meshes;
		dataGridMeshes.Height = dataGridMeshes.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + form.dataGridMeshes.ColumnHeadersHeight;
		dataGridMeshes.Width = dataGridMeshes.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridMeshes.ClearSelection();
		buttonImportFBX.Left = buttonLoad.Left = buttonSave.Left = dataGridMeshes.Width + 25;
		dataGridTextures.Top = Math.Max(dataGridMeshes.Height + 20, 100);
		dataGridTextures.Width = dataGridMeshes.Width;
	}

	private void MainForm_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
			return;
		else if (ModifierKeys == Keys.Alt)
			dataGridMeshes.SelectAll();
		else if (ModifierKeys == Keys.Control)
			dataGridMeshes.Rows[Meshes.IndexOf(nearestMesh)]?.Selected ^= true;
		else
		{
			dataGridMeshes.ClearSelection();
			if (nearestMesh != null)
				dataGridMeshes.Rows[Meshes.IndexOf(nearestMesh)].Selected = true;
		}
			
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (dataGridMeshes.IsCurrentCellInEditMode || dataGridTextures.IsCurrentCellInEditMode)
			return;

		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (e.KeyCode == Keys.Delete)
			Flver.DeleteMeshes();

		if (e.KeyCode == Keys.H)
		{
			Meshes.ForEach(m => m.Hidden ^= m.Selected );
			dataGridMeshes.Refresh();
		}
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		// Set selected meshes based on data grid selected rows
		Meshes.ForEach(m => m.Selected = false);
		foreach (DataGridViewRow row in dataGridMeshes.SelectedRows)
			Meshes[row.Index].Selected = row.Selected;

		// If only one row is selected enable textures editing
		if (dataGridMeshes.SelectedRows.Count == 1)
		{
			dataGridTextures.Visible = true;
			dataGridTextures.DataSource = Meshes.First(m => m.Selected).textureList;
			dataGridTextures.Height = dataGridTextures.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridTextures.ColumnHeadersHeight;
			dataGridTextures.ClearSelection();
		}
		else
			dataGridTextures.Visible = false;
	}
}
