namespace TinyFLVER
{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			openFlverDialog = new OpenFileDialog();
			buttonSave = new Button();
			panel = new Panel();
			buttonImport = new Button();
			renderControl = new MonoGame();
			SuspendLayout();
			// 
			// openFlverDialog
			// 
			openFlverDialog.FileName = "openFileDialog1";
			openFlverDialog.Filter = "FLVER|*.flver;*.bak";
			// 
			// buttonSave
			// 
			buttonSave.Location = new Point(329, 41);
			buttonSave.Name = "buttonSave";
			buttonSave.Size = new Size(75, 23);
			buttonSave.TabIndex = 1;
			buttonSave.Text = "Save";
			buttonSave.UseVisualStyleBackColor = true;
			buttonSave.Click += buttonSave_Click;
			// 
			// panel
			// 
			panel.AutoScroll = true;
			panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			panel.BackgroundImageLayout = ImageLayout.None;
			panel.Location = new Point(0, 12);
			panel.Name = "panel";
			panel.Size = new Size(313, 452);
			panel.TabIndex = 3;
			// 
			// buttonImport
			// 
			buttonImport.Location = new Point(329, 12);
			buttonImport.Name = "buttonImport";
			buttonImport.Size = new Size(75, 23);
			buttonImport.TabIndex = 4;
			buttonImport.Text = "Import";
			buttonImport.UseVisualStyleBackColor = true;
			buttonImport.Click += buttonImport_Click;
			// 
			// renderControl
			// 
			renderControl.BackColor = SystemColors.ControlDark;
			renderControl.Dock = DockStyle.Right;
			renderControl.ForeColor = Color.Transparent;
			renderControl.GraphicsProfile = Microsoft.Xna.Framework.Graphics.GraphicsProfile.HiDef;
			renderControl.Location = new Point(447, 0);
			renderControl.MouseHoverUpdatesOnly = false;
			renderControl.Name = "renderControl";
			renderControl.Size = new Size(1162, 1048);
			renderControl.TabIndex = 12;
			renderControl.Text = "monoGame1";
			renderControl.MouseClick += renderControl_MouseClick;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			BackColor = SystemColors.ControlDark;
			ClientSize = new Size(1609, 1048);
			Controls.Add(renderControl);
			Controls.Add(buttonImport);
			Controls.Add(panel);
			Controls.Add(buttonSave);
			DoubleBuffered = true;
			Icon = (Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			Name = "MainForm";
			Text = "TinyFLVER";
			Load += MainForm_Load;
			KeyDown += MainForm_KeyDown;
			ResumeLayout(false);
		}




		#endregion

		private System.Windows.Forms.OpenFileDialog openFlverDialog;
		private System.Windows.Forms.Button buttonSave;
		private System.Windows.Forms.Panel panel;
		private System.Windows.Forms.Button buttonImport;
		private MonoGame renderControl;
	}
}