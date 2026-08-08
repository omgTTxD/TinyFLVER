using System.ComponentModel;
using System.Reflection;

public partial class MainForm : Form
{
	void SetupDataGrids()
	{
		PropertyInfo property = typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		property.SetValue(dgMeshes, true);
		property.SetValue(dgTextures, true);

		//form.dgMeshes.DataSource = Meshes;

		void ResizeWhenTyping(object sender, EventArgs e)
		{
			var dg = sender as DataGridView;
			dg.CommitEdit(DataGridViewDataErrorContexts.Commit);
			dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
			dg.Refresh();
		}

		void ResizeAfterDataBinding(object sender, EventArgs e)
		{
			var dg = sender as DataGridView;
			dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
			dg.Height = dg.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dg.ColumnHeadersHeight;
			dg.ClearSelection();
		}

		dgTextures.DataError += (s, e) => { };

		dgTextures.DataBindingComplete += ResizeAfterDataBinding;
		dgMeshes.DataBindingComplete += ResizeAfterDataBinding;

		dgTextures.CurrentCellDirtyStateChanged += ResizeWhenTyping;
		dgMeshes.CurrentCellDirtyStateChanged += ResizeWhenTyping;

		dgTextures.CurrentCellDirtyStateChanged += (s, e) => Textures.Reload(selectedMeshes[0]);

		dgMeshes.SizeChanged += (s, e) =>
		{
			buttonCopyMaterial.Left = buttonPasteMaterial.Left = dgMeshes.Width + 25;
			dgTextures.Top = Math.Max(75, dgMeshes.Height + 15);
		};

		KeyDown += ProcessKeyboardInput;
	}

	private void ProcessKeyboardInput(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Q)
		{
			if (dgTextures.Focused)
			{
				if (dgTextures.SelectedCells.Count == 0)
					return;

				selectedMeshes[0].material.Textures.RemoveAt(dgTextures.SelectedCells[0].RowIndex);
				Textures.ReloadAll();
				dgTextures.Invalidate();
				//	((BindingList<FLVER2.Texture>)dgTextures.DataSource).RemoveAt(dgTextures.SelectedCells[0].RowIndex);

				//dgTextures.DataSource = new BindingList<FLVER2.Texture>(selectedMeshes[0].material.Textures);

			}
			else
				Flver.DeleteMeshes();
		}

		if (dgTextures.Focused && e.KeyCode == Keys.Back)
		{
			dgTextures.CurrentCell.Value = "";
			dgTextures.Refresh();
			Textures.Reload(selectedMeshes[0]);
		}
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		buttonPasteMaterial.Visible = buttonCopyMaterial.Visible = dgTextures.Visible = selectedMeshes.Count == 1;

		if (selectedMeshes.Count == 1)
			dgTextures.DataSource = new BindingList<FLVER2.Texture>(selectedMeshes[0].material.Textures);
	}

	void dataGridTextures_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
	{
		if (e.DesiredType == typeof(Vector2) && int.TryParse((string)e.Value, out var scale))
			e.Value = new Vector2(scale, scale);
		e.ParsingApplied = true;
	}

	void dataGridTextures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		if (e.Value is Vector2 v)
			e.Value = v.X;
	}

	void dataGridTextures_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (e.Control is ComboBox cb && dgTextures.CurrentCell.OwningColumn == ColumnBrowsePath)
		{
			int rowIndex = dgTextures.CurrentCell.RowIndex;
			var type = dgTextures.Rows[rowIndex].Cells[2].Value.ToString();
			Vector2 scale = (Vector2)dgTextures.Rows[rowIndex].Cells[3].Value;
			string currentTexture = dgTextures.Rows[rowIndex].Cells[1].Value.ToString();
			
			List<string> textures = [currentTexture];
			textures.AddRange(Directory.GetFiles(".", "*.dds").Select(x => Path.GetFileNameWithoutExtension(x)));
			if (scale.X != 1)
				textures = ["AAT100_Skin_01_n"];

			cb.DataSource = textures;
		}
	}
}

