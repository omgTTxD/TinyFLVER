namespace TinyFLVER;

partial class MainForm
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
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
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		renderControl = new SharpDX.Windows.RenderControl();
		panel = new Panel();
		comboBoxShaders = new ComboBox();
		label2 = new Label();
		comboBoxFlverVersion = new ComboBox();
		buttonSave = new Button();
		buttonImportFBX = new Button();
		label1 = new Label();
		label3 = new Label();
		openFlverDialog = new OpenFileDialog();
		checkBoxBackfaceCulling = new CheckBox();
		swapChainFormat = new ListBox();
		checkBoxDiffuseSRgb = new CheckBox();
		SuspendLayout();
		// 
		// renderControl
		// 
		renderControl.AutoValidate = AutoValidate.EnableAllowFocusChange;
		renderControl.BackColor = SystemColors.ControlDark;
		renderControl.BackgroundImageLayout = ImageLayout.None;
		renderControl.Dock = DockStyle.Fill;
		renderControl.Location = new Point(0, 0);
		renderControl.Name = "renderControl";
		renderControl.Size = new Size(1048, 709);
		renderControl.TabIndex = 1;
		renderControl.MouseClick += renderControl_MouseClick;
		// 
		// panel
		// 
		panel.AutoSize = true;
		panel.BackColor = SystemColors.ControlDark;
		panel.CausesValidation = false;
		panel.Location = new Point(0, 1);
		panel.Name = "panel";
		panel.Size = new Size(331, 114);
		panel.TabIndex = 15;
		// 
		// comboBoxShaders
		// 
		comboBoxShaders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		comboBoxShaders.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxShaders.FormattingEnabled = true;
		comboBoxShaders.ItemHeight = 15;
		comboBoxShaders.Location = new Point(880, 42);
		comboBoxShaders.Name = "comboBoxShaders";
		comboBoxShaders.Size = new Size(156, 23);
		comboBoxShaders.TabIndex = 0;
		// 
		// label2
		// 
		label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		label2.AutoSize = true;
		label2.Location = new Point(792, 16);
		label2.Name = "label2";
		label2.Size = new Size(82, 15);
		label2.TabIndex = 17;
		label2.Text = "FLVER version:";
		// 
		// comboBoxFlverVersion
		// 
		comboBoxFlverVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		comboBoxFlverVersion.FormattingEnabled = true;
		comboBoxFlverVersion.Location = new Point(880, 13);
		comboBoxFlverVersion.Name = "comboBoxFlverVersion";
		comboBoxFlverVersion.Size = new Size(156, 23);
		comboBoxFlverVersion.TabIndex = 16;
		// 
		// buttonSave
		// 
		buttonSave.Location = new Point(368, 41);
		buttonSave.Name = "buttonSave";
		buttonSave.Size = new Size(75, 23);
		buttonSave.TabIndex = 19;
		buttonSave.Text = "Save";
		buttonSave.UseVisualStyleBackColor = true;
		// 
		// buttonImportFBX
		// 
		buttonImportFBX.Location = new Point(368, 12);
		buttonImportFBX.Name = "buttonImportFBX";
		buttonImportFBX.Size = new Size(75, 23);
		buttonImportFBX.TabIndex = 18;
		buttonImportFBX.Text = "Import";
		buttonImportFBX.UseVisualStyleBackColor = true;
		// 
		// label1
		// 
		label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		label1.AutoSize = true;
		label1.Location = new Point(828, 45);
		label1.Name = "label1";
		label1.Size = new Size(46, 15);
		label1.TabIndex = 22;
		label1.Text = "Shader:";
		// 
		// label3
		// 
		label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		label3.AutoSize = true;
		label3.Location = new Point(0, 625);
		label3.Name = "label3";
		label3.Size = new Size(212, 75);
		label3.TabIndex = 23;
		label3.Text = "- Alt+LMB - select all meshes, \r\n- Ctrl+LMB - add/remove to selection.\r\n- H - hide meshes.\r\n- Delete - delete meshes.\r\n- Space - edit selected mesh's material.";
		// 
		// openFlverDialog
		// 
		openFlverDialog.Filter = "FLVER|*.flver;*.bak";
		// 
		// checkBoxBackfaceCulling
		// 
		checkBoxBackfaceCulling.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		checkBoxBackfaceCulling.AutoSize = true;
		checkBoxBackfaceCulling.Location = new Point(924, 71);
		checkBoxBackfaceCulling.Name = "checkBoxBackfaceCulling";
		checkBoxBackfaceCulling.Size = new Size(112, 19);
		checkBoxBackfaceCulling.TabIndex = 0;
		checkBoxBackfaceCulling.Text = "Backface culling";
		checkBoxBackfaceCulling.UseVisualStyleBackColor = true;
		// 
		// swapChainFormat
		// 
		swapChainFormat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		swapChainFormat.FormattingEnabled = true;
		swapChainFormat.Location = new Point(880, 123);
		swapChainFormat.Name = "swapChainFormat";
		swapChainFormat.Size = new Size(156, 64);
		swapChainFormat.TabIndex = 0;
		// 
		// checkBoxDiffuseSRgb
		// 
		checkBoxDiffuseSRgb.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		checkBoxDiffuseSRgb.AutoSize = true;
		checkBoxDiffuseSRgb.Checked = true;
		checkBoxDiffuseSRgb.CheckState = CheckState.Checked;
		checkBoxDiffuseSRgb.Location = new Point(824, 71);
		checkBoxDiffuseSRgb.Name = "checkBoxDiffuseSRgb";
		checkBoxDiffuseSRgb.Size = new Size(94, 19);
		checkBoxDiffuseSRgb.TabIndex = 0;
		checkBoxDiffuseSRgb.Text = "Diffuse SRGB";
		checkBoxDiffuseSRgb.UseVisualStyleBackColor = true;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlDark;
		CausesValidation = false;
		ClientSize = new Size(1048, 709);
		Controls.Add(checkBoxDiffuseSRgb);
		Controls.Add(swapChainFormat);
		Controls.Add(checkBoxBackfaceCulling);
		Controls.Add(label3);
		Controls.Add(label1);
		Controls.Add(buttonSave);
		Controls.Add(buttonImportFBX);
		Controls.Add(label2);
		Controls.Add(comboBoxFlverVersion);
		Controls.Add(comboBoxShaders);
		Controls.Add(panel);
		Controls.Add(renderControl);
		DoubleBuffered = true;
		IsFullscreen = true;
		IsMdiContainer = true;
		KeyPreview = true;
		Name = "MainForm";
		StartPosition = FormStartPosition.CenterScreen;
		Text = "TinyFLVER";
		KeyDown += MainForm_KeyDown;
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	public SharpDX.Windows.RenderControl renderControl;
	public Panel panel;
	private Label label2;
	public ComboBox comboBoxFlverVersion;
	public Button buttonSave;
	public Button buttonImportFBX;
	public ComboBox comboBoxShaders;
	private Label label1;
	private Label label3;
	public OpenFileDialog openFlverDialog;
	public CheckBox checkBoxBackfaceCulling;
	public ListBox swapChainFormat;
	public CheckBox checkBoxDiffuseSRgb;
}
