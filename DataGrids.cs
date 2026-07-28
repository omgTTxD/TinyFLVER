using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class MainForm : Form
{
	void SetupDataGrids()
	{
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

		AddTexture.Click += (s, e) => ((BindingList<FLVER2.Texture>)dgTextures.DataSource).Add(new());
		dgMeshes.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgTextures.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgTextures.DataError += (s, e) => { };

		dgTextures.DataBindingComplete += ResizeAfterDataBinding;
		dgMeshes.DataBindingComplete += ResizeAfterDataBinding;

		dgTextures.CurrentCellDirtyStateChanged += ResizeWhenTyping;
		dgMeshes.CurrentCellDirtyStateChanged += ResizeWhenTyping;

		dgTextures.CurrentCellDirtyStateChanged += (s, e) => Textures.Reload(selectedMeshes[0]);

		dgMeshes.SizeChanged += (s, e) =>
		{
			buttonCopyMaterial.Left = buttonPasteMaterial.Left = dgMeshes.Width + 25;
			AddTexture.Top = dgTextures.Top = GuessTexture.Top = Math.Max(75, dgMeshes.Height + 15);
		};

		dgTextures.SizeChanged += (s, e) => AddTexture.Left = dgTextures.Width + dgTextures.Left;
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		buttonPasteMaterial.Visible = buttonCopyMaterial.Visible =
			AddTexture.Visible = dgTextures.Visible = selectedMeshes.Count == 1;

		GuessTexture.Visible = selectedMeshes.Count != 1;

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

		if (e.Value is string)
			e.Value = Path.GetFileName(e.Value as string);
	}

	private void dgMeshes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		if (e.Value is string)
			e.Value = Path.GetFileName(e.Value as string);
	}

	void dataGridTextures_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (e.Control is ComboBox cb && dgTextures.CurrentCell.OwningColumn == ColumnBrowsePath)
		{
			int rowIndex = dgTextures.CurrentCell.RowIndex;
			var type = dgTextures.Rows[rowIndex].Cells[2].Value.ToString();
			Vector2 scale = (Vector2)dgTextures.Rows[rowIndex].Cells[3].Value;
			string pattern = "*.dds";
			string path = dgTextures.Rows[rowIndex].Cells[1].Value.ToString();
			
			List<string> textures = [path];

			if (type.ContainsAny("Albedo", "Diffuse")) pattern = "*_a*.dds";
			if (type.ContainsAny("Bumpmap", "Normal")) pattern = "*_n*.dds";
			if (type.ContainsAny("Bumpmap", "Metallic")) pattern = "*_m*.dds";
			if (type.Contains("Mask1")) pattern = "*_1m*.dds";

			textures.AddRange(Directory.GetFiles(".", pattern).Select(x => Path.GetFileNameWithoutExtension(x)).Where(x => x != path));
			if (scale.X != 1)
				textures = ["AAT100_Skin_01_n"];

			cb.DataSource = textures;
		}
	}
}

