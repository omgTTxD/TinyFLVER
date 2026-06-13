using Microsoft.VisualBasic.Devices;
using Microsoft.Win32;

public partial class MainForm : Form
{
	public MainForm()
	{
		InitializeComponent();
		form = this;

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size(Screen.PrimaryScreen.Bounds.Width / 3 * 2, Screen.PrimaryScreen.Bounds.Height / 8 * 7);
		Location = new Point(Screen.PrimaryScreen.Bounds.Width / 8, 50);

		Exposure.ValueChanged += (s, e) => shaderData.Exposure = Exposure.Value / 1000f;
		Gamma.ValueChanged += (s, e) => shaderData.Gamma = Gamma.Value / 1000f;

		form.refreshTimer.Tick += (s, e) => { nearestMesh = Mesh.PickNearest(); SwapChainManager.Render(); };

		dataGridMeshes.CurrentCellDirtyStateChanged += (s, e) =>
		{
			dataGridMeshes.CommitEdit(DataGridViewDataErrorContexts.Commit);
			dataGridTextures.Width = dataGridTextures.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		};

		dataGridTextures.CurrentCellDirtyStateChanged += (s, e) =>
		{
			dataGridTextures.CommitEdit(DataGridViewDataErrorContexts.Commit); 
			SelectedMeshes[0].ReloadTextures(); 
			dataGridTextures.Width = dataGridTextures.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
			dataGridTextures.Refresh();
		};

		// This is needed to disable errors when typing characters in number fields
		dataGridMeshes.DataError += (s, e) => { };
		dataGridTextures.DataError += (s, e) => { };
		dataGridMeshes.DataSource = Meshes;
	
		SwapChainManager.Initialize();
		Shaders.Initialize();
		ShadersData.Initialize();
		Flver.Initialize();
		Camera.Initialize();
		
		ColumnTextures.DataSource = Directory.GetFiles(".", "*.dds").Select(x => Path.GetFileNameWithoutExtension(x)).ToList();

		Load += (s, e) => Activate();
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
			SelectedMeshes.ForEach(m => m.Hidden ^= true);


		dataGridMeshes.Refresh();
	}

	private void dataGridMeshes_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
	{
		dataGridMeshes.ClearSelection();
		dataGridMeshes.Width = dataGridMeshes.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridMeshes.Height = dataGridMeshes.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridMeshes.ColumnHeadersHeight;
		buttonImport.Left = buttonOpen.Left = buttonSave.Left = dataGridMeshes.Width + 25;
		dataGridTextures.Top = Math.Max(dataGridMeshes.Height + 20, 100);
	}

	void dataGridTextures_DataSourceChanged(object sender, EventArgs e)
	{
		dataGridTextures.Width = dataGridTextures.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dataGridTextures.Height = dataGridTextures.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dataGridTextures.ColumnHeadersHeight;
		buttonAddTexture.Top = dataGridTextures.Top + 1;
		buttonAddTexture.Left = dataGridTextures.Width + 15;
		dataGridTextures.ClearSelection();
	}

	void RemoveTexture()
	{
		SelectedMeshes[0].material.Textures.RemoveAt(dataGridTextures.SelectedCells[0].RowIndex);
		dataGridTextures.DataSource = null; dataGridTextures.DataSource = SelectedMeshes[0].material.Textures;
	}

	void buttonAddTexture_Click(object sender, EventArgs e)
	{
		SelectedMeshes[0].material.Textures.Add(new());
		dataGridTextures.DataSource = null; dataGridTextures.DataSource = SelectedMeshes[0].material.Textures;
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		buttonAddTexture.Visible = dataGridTextures.Visible = SelectedMeshes.Count == 1;
		if (SelectedMeshes.Count == 1)
			dataGridTextures.DataSource = SelectedMeshes[0].material.Textures;
		dataGridTextures.Invalidate();
	}
}