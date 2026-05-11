using System.Windows.Forms;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
		form = this;

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);
		Location = new Point(Screen.PrimaryScreen.Bounds.Width / 5, 60);

		Camera.Initialize();
		SwapChainManager.Initialize();
		Shaders.Initialize(); 

		dataGridMeshes.CurrentCellDirtyStateChanged += (s, e) => dataGridMeshes.CommitEdit(DataGridViewDataErrorContexts.Commit);
		dataGridMeshes.DataError += (s, e) => { };
		dataGridTextures.CurrentCellDirtyStateChanged += (s, e) => { dataGridTextures.CommitEdit(DataGridViewDataErrorContexts.Commit); Texture.Reload(Meshes.FirstOrDefault(m => m.Selected)); };
		
		Exposure.Scroll += (s, e) => shaderData.Exposure = Exposure.Value;
		Gamma.Scroll += (s, e) => shaderData.Gamma = Gamma.Value;

		Load += (s, e) => { Activate(); Flver.Initialize(); };
	}

	public void BindMeshesDataGrid()
	{
		dataGridMeshes.DataSource = null;       
		dataGridMeshes.DataSource = Meshes;
		dataGridMeshes.Width = dataGridMeshes.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridMeshes.Height = dataGridMeshes.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridMeshes.ColumnHeadersHeight;
		buttonImportFBX.Left = buttonLoad.Left = buttonSave.Left = dataGridMeshes.Width + 25;
		dataGridTextures.Top = Math.Max(dataGridMeshes.Height + 20, 100);
		dataGridMeshes.ClearSelection();
	}

	private void MainForm_MouseClick(object sender, MouseEventArgs e)
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
			dataGridMeshes.ClearSelection();
			if (nearestMesh != null)
				dataGridMeshes.Rows[Meshes.IndexOf(nearestMesh)].Selected = true;
		}
			
	}

	private void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (dataGridMeshes.IsCurrentCellInEditMode || dataGridTextures.IsCurrentCellInEditMode)
			return;

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
		if (dataGridMeshes.RowCount > Meshes.Count)
			return;

		foreach (DataGridViewRow row in dataGridMeshes.Rows)
			Meshes[row.Index].Selected = row.Selected;

		// Show textures data grid if only one mesh selected
		dataGridTextures.Visible = dataGridMeshes.SelectedRows.Count == 1;
		dataGridTextures.DataSource = Meshes.FirstOrDefault(m => m.Selected)?.textureList;
		dataGridTextures.Width = dataGridTextures.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridTextures.Height = dataGridTextures.Rows.Count * 22 + dataGridTextures.ColumnHeadersHeight;
		dataGridTextures.ClearSelection();
	}
}
