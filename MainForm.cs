using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Reflection;

public partial class MainForm : Form
{



	// This is needed to prevent DataGrids from flickering
	protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }
	[DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
	const int HOTKEY_ID = 9001;
	const uint VK_SNAPSHOT = 0x2C;
	[DllImport("user32.dll", SetLastError = true)]
	private static extern bool UnregisterHotKey(
		IntPtr hWnd,
		int id);

	public bool screenshotRequested;
	protected override void WndProc(ref System.Windows.Forms.Message m)
	{
		const int WM_HOTKEY = 0x0312;
		if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
		{
			shaderData.Exposure = 1;
			Renderer.Render();
			screenshotRequested = true;
			//UnregisterHotKey(Handle, HOTKEY_ID);

		}
		base.WndProc(ref m);
	}

	FormWindowState previousState;
	protected override void OnResizeEnd(EventArgs e)
	{
		//if (WindowState == FormWindowState.Minimized) return;
		previousState = WindowState;
		base.OnResizeEnd(e);
	}
	protected override void OnResize(EventArgs e)
	{
		// If form is maximized, call process resize manually
		if (previousState != WindowState)
			OnResizeEnd(null);

		base.OnResize(e);
	}

	public MainForm()
	{
		InitializeComponent();
		form = this;

		Type dgType = dgMeshes.GetType();
		PropertyInfo property = dgType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
		property.SetValue(dgMeshes, true);

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 70), (int)(Screen.PrimaryScreen.Bounds.Height / 100f * 92));
		Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 16), 40);
		//	RegisterHotKey(Handle, HOTKEY_ID, 0, VK_SNAPSHOT);

		Renderer.Initialize();
		Shaders.Initialize();
		Camera.Initialize();
		SpheresRenderer.Initialize();
		Flver.Initialize();


		ToolTip toolTip = new();
		toolTip.AutoPopDelay = 2000;
		toolTip.InitialDelay = 500;
		toolTip.ReshowDelay = 500;
		toolTip.SetToolTip(RecalculateTangents, "Otherwise, read tangents from FLVER.");
		toolTip.SetToolTip(SwapXY, "Swap perfromed after flipping is done.");



		Load += (s, e) => Activate();
		//	form.RenderSpheres.Checked = true;
		form.RecalculateTangents.Checked = true;

		AddTexture.Click += (s, e) => ((BindingList<FLVER2.Texture>)dgTextures.DataSource).Add(new());
		form.dgMeshes.DataSource = Meshes;
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

		dgTextures.SizeChanged += (s, e) =>
		{
			AddTexture.Left = dgTextures.Width + dgTextures.Left;
		};

		GuessTexture.CheckedChanged += (s, e) => Textures.ReloadAll();
	}

	private void dataGridMeshes_SelectionChanged(object sender, EventArgs e)
	{
		buttonPasteMaterial.Visible = buttonCopyMaterial.Visible =
			AddTexture.Visible = dgTextures.Visible = selectedMeshes.Count == 1;

		GuessTexture.Visible = selectedMeshes.Count != 1;

		if (selectedMeshes.Count == 1)
			dgTextures.DataSource = new BindingList<FLVER2.Texture>(selectedMeshes[0].material.Textures);
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

		if (ModifierKeys == Keys.Control && nearestMesh is not null)
			dgMeshes.Rows[Meshes.IndexOf(nearestMesh)]?.Selected ^= true;


		if (ModifierKeys == Keys.None)
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
			selectedMeshes.ForEach(m => m.Hidden ^= true);
		dgMeshes.Refresh();
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

	private void SwapXY_CheckedChanged(object sender, EventArgs e)
	{

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