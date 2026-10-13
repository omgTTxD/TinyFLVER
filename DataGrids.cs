using System;
using System.ComponentModel;
using System.Reflection;
using System.Security.Cryptography.Xml;

class DataGrids
{
	static DataGridView dgMeshes = form.dgMeshes;
	static DataGridView dgTextures = form.dgTextures;

	public static void Initialize()
	{
		PropertyInfo property = typeof(DataGridView).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		property.SetValue(dgMeshes, true);
		property.SetValue(dgTextures, true);

		dgTextures.DataError += (s, e) => { };
		dgMeshes.DataError += (s, e) => { };
		dgTextures.DataBindingComplete += ResizeAfterDataBinding;
		dgMeshes.DataBindingComplete += ResizeAfterDataBinding;

		dgTextures.CurrentCellDirtyStateChanged += ResizeWhenTyping;
		dgMeshes.CurrentCellDirtyStateChanged += ResizeWhenTyping;
		dgTextures.CellEndEdit += (s, e) => ResizeWhenTyping(s, null);

		dgTextures.CurrentCellDirtyStateChanged += (s, e) => { Textures.ReloadAll(); };
		
		dgMeshes.SizeChanged += (s, e) =>
		{
			form.buttonCopyMaterial.Left = form.buttonPasteMaterial.Left = dgMeshes.Width + 25;
			dgTextures.Top = Math.Max(75, dgMeshes.Height + 15);
		};

		form.KeyDown += ProcessKeyboardInput;

		dgTextures.CellFormatting += dataGridTextures_CellFormatting;
		dgTextures.CellParsing += dataGridTextures_CellParsing;
		dgTextures.EditingControlShowing += dataGridTextures_EditingControlShowing;
		dgMeshes.SelectionChanged += dataGridMeshes_SelectionChanged;
	}

	static void ResizeWhenTyping(object sender, EventArgs e)
	{
		var dg = sender as DataGridView;
		dg.CommitEdit(DataGridViewDataErrorContexts.Commit);
		dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dg.Refresh();
	}

	static void ResizeAfterDataBinding(object sender, EventArgs e)
	{
		var dg = sender as DataGridView;
		dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dg.Height = dg.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dg.ColumnHeadersHeight;
	}

	static void ProcessKeyboardInput(object sender, KeyEventArgs e)
	{
		if (dgMeshes.IsCurrentCellInEditMode || dgTextures.IsCurrentCellInEditMode)
			return;

		if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Q)
		{
			if (dgTextures.Focused)
			{
				int index = dgTextures.SelectedCells[0].RowIndex;
				Textures.cache.TryRemove(selectedMeshes[0].material.Textures.ElementAt(index).Path + ".dds", out var value);
				selectedMeshes[0].material.Textures.RemoveAt(index);
				Textures.Reload(selectedMeshes[0]);
				dgTextures.DataSource = null;
				dgTextures.DataSource = new BindingList<FLVER2.Texture>(selectedMeshes[0].material.Textures);
			}
			else
			{
				Flver.DeleteMeshes();
				dgMeshes.ClearSelection();
			}
		}

		if (dgTextures.Focused && e.KeyCode == Keys.Back)
		{
			dgTextures.CurrentCell.Value = "";
			dgTextures.Refresh();
			Textures.Reload(selectedMeshes[0]);
		}
	}

	static private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		form.buttonPasteMaterial.Visible = form.buttonCopyMaterial.Visible = dgTextures.Visible = selectedMeshes.Count == 1;

		if (selectedMeshes.Count == 1)
			dgTextures.DataSource = new BindingList<FLVER2.Texture>(selectedMeshes[0].material.Textures);

		dgTextures.ClearSelection();
	}

	static void dataGridTextures_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
	{
		if (e.DesiredType == typeof(Vector2) && int.TryParse((string)e.Value, out var scale))
			e.Value = new Vector2(scale, scale);

		e.ParsingApplied = true;
	}
	static Font bold;

	static void dataGridTextures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
	{
		if (e.Value is Vector2 v) e.Value = v.X;
		if (e.Value is string str) e.Value = Path.GetFileNameWithoutExtension(str);

		if (e.RowIndex < 0 || dgTextures.Rows[e.RowIndex].DataBoundItem is not FLVER2.Texture tex)
			return;

		var m = selectedMeshes[0];
		Textures.cache.TryGetValue(Path.GetFileNameWithoutExtension(tex.Path) + ".dds", out var srv);

		Color? color = tex.Path.ContainsAny("AAT100_Skin_01_n") ? Color.SkyBlue
			: srv is null ? null
			: srv == m.diffuse ? Color.Orange
			: srv == m.normal ? Color.DeepSkyBlue
			: srv == m.metalness ? Color.Gray
			: srv == m.reflectance ? Color.AntiqueWhite
			: srv == m.specular ? Color.Red
			: null;

		e.CellStyle.ForeColor = color ?? Color.FromArgb(80, 80, 80);
		if (color is not null)
			e.CellStyle.Font = bold ??= new Font(e.CellStyle.Font, FontStyle.Bold);

		ResizeAfterDataBinding(sender, null);
	}

	static void dataGridTextures_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
	{
		if (e.Control is ComboBox cb && dgTextures.CurrentCell.ColumnIndex == 0)
		{
			int rowIndex = dgTextures.CurrentCell.RowIndex;
			var type = dgTextures.Rows[rowIndex].Cells[2].Value.ToString();
			Vector2 scale = (Vector2)dgTextures.Rows[rowIndex].Cells[3].Value;

			List<string> textures = [];
			string[] suffixes = [".dds"];
			if (type.ContainsAny("Diffuse", "Albedo")) suffixes = ["_d", "_a"];
			if (type.ContainsAny("Normal", "Bump")) suffixes = ["_n"];

			textures.AddRange(Directory.GetFiles(".", "*.dds").Where(x => x.ContainsAny(suffixes)).Select(x => Path.GetFileNameWithoutExtension(x)));			
		
			if (scale.X != 1)
				textures = ["AAT100_Skin_01_n"];
			cb.DataSource = textures;
		}
	}
}

