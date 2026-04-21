using System.Drawing;
using System.Reflection;

namespace TinyFLVER;

partial class MainForm
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null))
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		openFlverDialog = new OpenFileDialog();
		buttonSave = new Button();
		panel = new Panel();
		buttonImport = new Button();
		monoGame = new MonoGame();
		label1 = new Label();
		SuspendLayout();
		// 
		// openFlverDialog
		// 
		openFlverDialog.Filter = "FLVER|*.flver;*.bak";
		openFlverDialog.OkRequiresInteraction = true;
		// 
		// buttonSave
		// 
		buttonSave.Location = new Point(333, 41);
		buttonSave.Name = "buttonSave";
		buttonSave.Size = new Size(75, 23);
		buttonSave.TabIndex = 1;
		buttonSave.Text = "Save";
		buttonSave.UseVisualStyleBackColor = true;
		buttonSave.Click += buttonSave_Click;
		// 
		// panel
		// 
		panel.AutoSize = true;
		panel.BackColor = SystemColors.ControlDark;
		panel.BackgroundImageLayout = ImageLayout.None;
		panel.CausesValidation = false;
		panel.Location = new Point(0, 0);
		panel.Name = "panel";
		panel.Size = new Size(209, 50);
		panel.TabIndex = 3;
		// 
		// buttonImport
		// 
		buttonImport.Location = new Point(333, 12);
		buttonImport.Name = "buttonImport";
		buttonImport.Size = new Size(75, 23);
		buttonImport.TabIndex = 4;
		buttonImport.Text = "Import";
		buttonImport.UseVisualStyleBackColor = true;
		buttonImport.Click += buttonImport_Click;
		// 
		// monoGame
		// 
		monoGame.BackColor = SystemColors.ControlDark;
		monoGame.Dock = DockStyle.Fill;
		monoGame.ForeColor = Color.White;
		monoGame.GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.HiDef;
		monoGame.Location = new Point(0, 0);
		monoGame.MouseHoverUpdatesOnly = false;
		monoGame.Name = "monoGame";
		monoGame.Size = new Size(1161, 1184);
		monoGame.TabIndex = 12;
		monoGame.MouseClick += monoControl_MouseClick;
		// 
		// label1
		// 
		label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		label1.AutoSize = true;
		label1.BackColor = SystemColors.ControlDark;
		label1.Location = new Point(0, 1115);
		label1.Name = "label1";
		label1.Size = new Size(209, 60);
		label1.TabIndex = 13;
		label1.Text = "- Alt+LMB - select all meshes, \r\n- Ctrl+LMB - add/remove to selection.\r\n- Delete - delete meshes.\r\n- Space - edit selected mesh's material";
		// 
		// MainForm
		// 
		BackColor = SystemColors.ControlDark;
		ClientSize = new Size(1161, 1184);
		Controls.Add(buttonSave);
		Controls.Add(buttonImport);
		Controls.Add(panel);
		Controls.Add(label1);
		Controls.Add(monoGame);
		KeyPreview = true;
		Name = "MainForm";
		StartPosition = FormStartPosition.Manual;
		Text = "TinyFLVER";
		Load += MainForm_Load;
		KeyDown += MainForm_KeyDown;
		ResumeLayout(false);
		PerformLayout();
	}
	#endregion

	private System.Windows.Forms.OpenFileDialog openFlverDialog;
	private System.Windows.Forms.Button buttonSave;
	private System.Windows.Forms.Panel panel;
	private System.Windows.Forms.Button buttonImport;
	private MonoGame monoGame;
	private Label label1;
}
