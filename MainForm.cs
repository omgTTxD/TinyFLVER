using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

public partial class MainForm : Form
{
	// This is needed to prevent DataGrids from flickering
	protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

	[System.Runtime.InteropServices.DllImport("AcrylicWindow.dll")]
	static extern void ApplyAcrylic(IntPtr hWnd, int radius = 75, float saturation = 1.5f);


	public MainForm()
	{
		InitializeComponent();
		form = this;

		ApplyAcrylic(form.Handle);

		// This prevents rare cases of flickering, for example during saving when ER is lauched
		SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 70), (int)(Screen.PrimaryScreen.Bounds.Height / 100f * 92));
		Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 16), 40);

		Renderer.Initialize();
		Shaders.Initialize();
		ShadersData.Initialize();
		Camera.Initialize();

		// Crashing now due to mesh picking
		MaximizeBox = false;

		Load += (s, e) => { Activate(); Flver.Initialize(); ShaderSpheres.Initialize(); };

		Exposure.ValueChanged += (s, e) => shaderData.Exposure = Exposure.Value / 1000f;
		Gamma.ValueChanged += (s, e) => shaderData.Gamma = Gamma.Value / 1000f;
		form.checkBoxFlipX.CheckedChanged += (s, e) => shaderData.FlipX = form.checkBoxFlipX.Checked ? 1 : 0;
		form.checkBoxFlipY.CheckedChanged += (s, e) => shaderData.FlipY = form.checkBoxFlipY.Checked ? 1 : 0;
		form.checkBoxSwapXY.CheckedChanged += (s, e) => shaderData.SwapXY = form.checkBoxSwapXY.Checked ? 1 : 0;

		buttonCopyMaterial.Click += (s, e) => CopyMaterial();
		buttonPasteMaterial.Click += (s, e) => PasteMaterial();

		AddTexture.Click += (s, e) => ((BindingList<FLVER2.Texture>)dgTextures.DataSource).Add(new());

		dgMeshes.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgTextures.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgTextures.DataError += (s, e) => { };

		dgTextures.DataBindingComplete += ResizeAfterDataBinding;
		dgMeshes.DataBindingComplete += ResizeAfterDataBinding;

		dgTextures.CurrentCellDirtyStateChanged += ResizeWhenTyping;
		dgMeshes.CurrentCellDirtyStateChanged += ResizeWhenTyping;

		dgTextures.CurrentCellDirtyStateChanged += (s, e) => Textures.Reload(SelectedMeshes[0]);

		dgMeshes.SizeChanged += (s, e) =>
		{
			buttonCopyMaterial.Left = buttonPasteMaterial.Left = dgMeshes.Width + 25;
			AddTexture.Top = dgTextures.Top = Math.Max(dgMeshes.Height + 20, 100);
		};

		dgTextures.SizeChanged += (s, e) => AddTexture.Left = dgTextures.Width + dgTextures.Left;

		GuessTexture.CheckedChanged += (s, e) => Textures.ReloadAll();
	}

	void ResizeWhenTyping(object sender, EventArgs e)
	{
		DataGridView dg = sender as DataGridView;
		dg.CommitEdit(DataGridViewDataErrorContexts.Commit);
		dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.None);
		dg.Refresh();
	}

	void ResizeAfterDataBinding(object sender, EventArgs e)
	{
		DataGridView dg = sender as DataGridView;
		dg.Width = dg.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
		dg.Height = dg.Rows.GetRowsHeight(DataGridViewElementStates.Visible) + dg.ColumnHeadersHeight;
		dg.ClearSelection();
	}


	// Input Handling ====================================================================================
	void MainForm_MouseClick(object sender, MouseEventArgs e)
	{
		dgMeshes.EndEdit();
		dgTextures.EndEdit();

		if (e.Button != MouseButtons.Left)
			return;

		if (ModifierKeys == Keys.Control)
		{
			if (nearestMesh is not null)
				dgMeshes.Rows[Meshes.IndexOf(nearestMesh)]?.Selected ^= true;
		}
		else
		{
			dgMeshes.ClearSelection();
			if (nearestMesh is not null)
				dgMeshes.Rows[Meshes.IndexOf(nearestMesh)].Selected = true;
		}
	}

	void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
			Environment.Exit(0);

		if (dgMeshes.IsCurrentCellInEditMode || dgTextures.IsCurrentCellInEditMode)
			return;

		if (e.KeyCode == Keys.Delete)
			if (dgTextures.Focused)
				((BindingList<FLVER2.Texture>)dgTextures.DataSource).RemoveAt(dgTextures.SelectedCells[0].RowIndex);
			else
				Flver.DeleteMeshes();

		if (e.KeyCode == Keys.H)
			SelectedMeshes.ForEach(m => m.Hidden ^= true);
		dgMeshes.Refresh();
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		buttonPasteMaterial.Visible = buttonCopyMaterial.Visible = AddTexture.Visible = dgTextures.Visible = SelectedMeshes.Count == 1;
		if (SelectedMeshes.Count == 1)
			dgTextures.DataSource = new BindingList<FLVER2.Texture>(SelectedMeshes[0].material.Textures);
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
		if (e.Control is TextBox tb && dgTextures.CurrentCell.OwningColumn == pathDataGridViewTextBoxColumn)
		{
			tb.AutoCompleteMode = AutoCompleteMode.Suggest;
			tb.AutoCompleteSource = AutoCompleteSource.CustomSource;

			AutoCompleteStringCollection texSource = new();
			texSource.AddRange(Directory.GetFiles(".", "*.dds").Select(x => Path.GetFileNameWithoutExtension(x)).ToArray());
			tb.AutoCompleteCustomSource = texSource;
		}
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		if (dgMeshes.IsCurrentCellInEditMode || dgTextures.IsCurrentCellInEditMode)
			return base.ProcessCmdKey(ref msg, keyData);

		if (keyData == (Keys.Control | Keys.C))
			CopyMaterial();

		if (keyData == (Keys.Control | Keys.V))
			PasteMaterial();

		return base.ProcessCmdKey(ref msg, keyData);
	}

	void CopyMaterial()
	{
		if (SelectedMeshes.Count == 1)
			Clipboard.SetText(JsonConvert.SerializeObject(SelectedMeshes[0].material));
	}

	void PasteMaterial()
	{
		try
		{
			var currentMatrial = SelectedMeshes[0].material;
			var newMaterial = JsonConvert.DeserializeObject<FLVER2.Material>(Clipboard.GetText());
			currentMatrial.Textures = newMaterial.Textures;
			currentMatrial.MTD = newMaterial.MTD;
			currentMatrial.Name = newMaterial.Name;

			dgMeshes.Refresh();
			dgTextures.DataSource = SelectedMeshes[0].material.Textures;
			Textures.Reload(SelectedMeshes[0]);
		}
		catch (Exception e) { MessageBox.Show(e.Message); }
	}


}

// So I want say thanks to ForsakenSilver for your great program, I couldn't use anything else. If only it wasn't so slow because of bones and DataGrids etc...
//
// Also to Elden Ring Reforged' team. I never even played Elden Ring online before, because I couldn't play Vanilla after Reforged. Now I will be able to play online... 
//
// Big thanks to SchuhBaum for "Free Lock-On Camera". Easily the most underrated mod in entire Souls franchise in my opinion. Too bad it exists only for Elden Ring, and I can't play
// without this mod anympre... Just please don't turn it off when you fighting agile enemies. They are SUPPOSED to attack from begind, it is what makes them different and interesting.
//
// To JKAnderson for SoulsFormats. Without it I couln't see FLVER from debugger's perspective. ANd it would be imposibble to code anything.
//
// To Meowmaritus. Back in the days I was very impressed with DSAnimStudio, especially considering it had GPU-accelerated rendering and shader selection long before modern software.
// Probably your program inspired me to make my own... Because when I tried to adapt DSAnimStudio to open .flver files I failed miserably. T_T
//
// To fromsoftserve. Great work with your lighting engine mods. I watched your videos and I know you started to learn shaders recently. So am I, because those broken specular, normal maps,
// tonemapping, fog and water irritated me for several years already. And only now I have possibility to try to do something about it.