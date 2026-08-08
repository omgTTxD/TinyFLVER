using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

public partial class MainForm : Form
{
	// This is needed to prevent DataGrids from flickering
	protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ExStyle |= 0x02000000; return cp; } }

	public MainForm()
	{
		InitializeComponent(); ticks = Environment.TickCount64;
		form = this;

		Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
		Size = new Size((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 70), (int)(Screen.PrimaryScreen.Bounds.Height / 100f * 92));
		Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 100f * 16), 40);

		RegisterHotKey(Handle, 1, 0, 0x2C);
		RegisterHotKey(Handle, 0, 1, 0x2C);

		Renderer.Initialize();
		Shaders.Initialize();
		Camera.Initialize();
		SpheresRenderer.Initialize();
		Flver.Initialize();

		SavedLocations.Size = new Size(SavedLocations.Width, SavedLocations.PreferredSize.Height);
		form.RecalculateTangents.Checked = true;

		form.comboBoxShaders.SelectedIndex = comboBoxShaders.Items.Count - 2;
		//form.SavedLocations.SelectedIndex = 1;

		SetupDataGrids();

	}

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
			comboBoxShaders.Focus();
			dgMeshes.ClearSelection();
			if (nearestMesh is not null)
				dgMeshes.Rows[Meshes.IndexOf(nearestMesh)].Selected = true;
		}
	}

	void MainForm_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
		{
			Camera.SaveCamera();
			Environment.Exit(0);
		}

		//if (e.KeyCode == Keys.Insert) 


		if (e.KeyCode == Keys.Home)
			RenderSpheres.Checked = !RenderSpheres.Checked;

		if (e.KeyCode == Keys.End)
			RecalculateTangents.Checked = !RecalculateTangents.Checked;

		if (dgMeshes.IsCurrentCellInEditMode || dgTextures.IsCurrentCellInEditMode)
			return;

		if (e.KeyCode == Keys.H)
		{
			selectedMeshes.ForEach(m => m.Hidden ^= true);
			dgMeshes.Refresh();
		}

		if (e.KeyCode == Keys.PageUp && comboBoxCubemaps.SelectedIndex > 0)
			comboBoxCubemaps.SelectedIndex--;

		if (e.KeyCode == Keys.PageDown && comboBoxCubemaps.SelectedIndex < comboBoxCubemaps.Items.Count - 1)
			comboBoxCubemaps.SelectedIndex++;

		if (!dgMeshes.Focused && !dgTextures.Focused && !comboBoxCubemaps.Focused)
		{
			if (e.KeyCode == Keys.Left && comboBoxShaders.SelectedIndex > 0)
				comboBoxShaders.SelectedIndex--;

			if (e.KeyCode == Keys.Right && comboBoxShaders.SelectedIndex < comboBoxShaders.Items.Count - 1)
				comboBoxShaders.SelectedIndex++;

			if (e.KeyCode == Keys.Up && comboBoxDebugID.SelectedIndex > 0)
				comboBoxDebugID.SelectedIndex--;

			if (e.KeyCode == Keys.Down && comboBoxDebugID.SelectedIndex < comboBoxDebugID.Items.Count - 1)
				comboBoxDebugID.SelectedIndex++;

			e.SuppressKeyPress = true;
		}
	}

	[DllImport("user32.dll")] static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
	protected override void WndProc(ref System.Windows.Forms.Message m)
	{
		if (m.Msg == 0x0312)
		{
			shaderData.Exposure = 1;
			Renderer.Render();
			Bitmap bitmap = new(Width, Height);
			Graphics.FromImage(bitmap).CopyFromScreen(Location, new(), Size);
			Clipboard.SetDataObject(bitmap, true);
			Thread.Sleep(30);
			Camera.SetupHDR();
		}

		if (m.Msg == 0x00A3 && m.WParam.ToInt32() == 2 && ModifierKeys == (Keys.Control))
		{
			TaskDialog.ShowDialog(this, new TaskDialogPage
			{
				Caption = "Much respect to:",
				Text = credits,
			});
			Link.Visible = true;
			Link.LinkClicked += (s, e) => Process.Start(new ProcessStartInfo("https://github.com/omgTTxD/TinyFLVER/blob/main/X.png") { UseShellExecute = true });
			return;
		}

		base.WndProc(ref m);
	}


	FormWindowState previousState;
	protected override void OnResize(EventArgs e)
	{
		if (previousState != WindowState && WindowState != FormWindowState.Minimized)
		{
			previousState = WindowState;
			OnResizeEnd(null);
		}

		base.OnResize(e);
	}
	
	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start(new ProcessStartInfo("https://github.com/omgTTxD/TinyFLVER/blob/main/X.png") { UseShellExecute = true });
		Link.Visible = false;	
	}

	string credits = @"
- ForsakenSilver. For your great program, and without it I don't think I would write mine. If only it wasn't so slow because of all DataGrids...
- FTRichter. For Dark Souls: Prepare to Die Again. I replayed it not once already, and still it continues to be very calculated experience. Vanilla basically didn't have nor equipment breaking mechanic, nor good soul economy (by comparison with PtDA). It was such a pleasure to actually care about souls and therefore, about gameplay.
- Halvard, Gabri, Angeluso, Aether, Dashima and Rodan. For Dark Souls III - Champion's Ashes. Like before, I'll never play vanilla instead of this mod. It's a pity that I think latest version of the mod is not always the best. And version I played and liked was old, witch changes to map and enemy locations, right before they removed all changes to add them again later.
- Ionian and Mikiri. For Sekiro Resurrection. Again, big improvement compared to vanilla. But I don't know which version I'll play next time. Because I want to play without Kuro Charm and Probably Demon's Bell, and mod will be too hard for me. And after vanilla I can try Resurrection again.
- Kirnifr, ivi, poeconcept, hanalove, Cavou, Rayan, Pina Colada, cultist, remu and Tiarok. For Elden Ring Reforged. I never ever played Elden Ring online before, because I can't play vanilla Miyazaki games. Therefore, I almost never experienced PvP, but now I can. Not to mention single gameplay is much-much better than vanilla.
- SchuhBaum. for ""Free Lock-On Camera"". Easily most underrated mod in the entire Souls franchise in my opinion. Too bad it exists only for Elden Ring, so I can't play other games until someone ports this mod to them. Also, don't use any type of auto camera when fighting agile enemies. It is sooo much better when enemies can actually sneak on you with their attacks. Vanilla camera lock is literally destroys attacks like Nameless King's sneak attack and others...
- JKAnderson. Without SoulsFormats I couln't see FLVER's contents from debugger's perspective. And without seeing that, I would never be able to code anything FLVER-related.
- Meowmaritus. Back in the days I was very impressed with DSAnimStudio, especially considering it had GPU rendering and shader selection, long before any software, not to mention incredible replication of in-game behaviour, so you can see and undertand hitboxes, interpolation bugs, etc.
I liked it so much I wanted to open every .flver with it. But I couldn't do it by default (which is already very strange). I tried to implemented it myself, but I failed miserably T_T I was not even remotely close. So I had no choice but to make my own program. 
- fromsoftserve. Great work with your lighting engine and texture mods. I watched your videos and I know you are trying to fix specularity issues.  So am I, because this utterly destroyed specular shaders are driving me insane. Mettallic blood makes my eyes bleed. Too much specularity on terrain is the reason I can't play Nightreigh or even Elden Ring. But finally, I can try and fix it now. The road will be long and it is already insanely hard. But I don't think someone else will do it, so again, I have no choise...
- DropOff. For when I searched something in discord, it was his messages that I saw most. Given how hard modding can be, his efforts alone made a big difference to how much modes we have now.";
}


