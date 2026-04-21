using SharpDX.Windows;
using ColorSlider;

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
			this.renderControl = new SharpDX.Windows.RenderControl();
			this.panel = new System.Windows.Forms.Panel();
			this.comboBoxShaders = new System.Windows.Forms.ComboBox();
			this.comboBoxFlverVersion = new System.Windows.Forms.ComboBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonImportFBX = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.openFlverDialog = new System.Windows.Forms.OpenFileDialog();
			this.checkBoxBackfaceCulling = new System.Windows.Forms.CheckBox();
			this.swapchainFormats = new System.Windows.Forms.ListBox();
			this.checkBoxDiffuseSRgb = new System.Windows.Forms.CheckBox();
			this.labelX = new System.Windows.Forms.Label();
			this.labelY = new System.Windows.Forms.Label();
			this.labelZ = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.sliderX = new ColorSlider.ColorSlider();
			this.sliderY = new ColorSlider.ColorSlider();
			this.sliderZ = new ColorSlider.ColorSlider();
			this.labelGamma = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.labelExposure = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.Exposure = new ColorSlider.ColorSlider();
			this.Gamma = new ColorSlider.ColorSlider();
			this.SuspendLayout();
			// 
			// renderControl
			// 
			this.renderControl.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.renderControl.BackColor = System.Drawing.SystemColors.ControlDark;
			this.renderControl.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.renderControl.ForeColor = System.Drawing.Color.White;
			this.renderControl.Location = new System.Drawing.Point(0, 0);
			this.renderControl.Name = "renderControl";
			this.renderControl.Size = new System.Drawing.Size(898, 563);
			this.renderControl.TabIndex = 1;
			this.renderControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.renderControl_MouseClick);
			// 
			// panel
			// 
			this.panel.AutoSize = true;
			this.panel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel.CausesValidation = false;
			this.panel.Location = new System.Drawing.Point(0, 1);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(263, 255);
			this.panel.TabIndex = 15;
			// 
			// comboBoxShaders
			// 
			this.comboBoxShaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxShaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxShaders.FormattingEnabled = true;
			this.comboBoxShaders.ItemHeight = 13;
			this.comboBoxShaders.Location = new System.Drawing.Point(728, 36);
			this.comboBoxShaders.Name = "comboBoxShaders";
			this.comboBoxShaders.Size = new System.Drawing.Size(160, 21);
			this.comboBoxShaders.TabIndex = 0;
			// 
			// comboBoxFlverVersion
			// 
			this.comboBoxFlverVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxFlverVersion.FormattingEnabled = true;
			this.comboBoxFlverVersion.Location = new System.Drawing.Point(728, 11);
			this.comboBoxFlverVersion.Name = "comboBoxFlverVersion";
			this.comboBoxFlverVersion.Size = new System.Drawing.Size(160, 21);
			this.comboBoxFlverVersion.TabIndex = 16;
			// 
			// buttonSave
			// 
			this.buttonSave.ForeColor = System.Drawing.Color.White;
			this.buttonSave.Location = new System.Drawing.Point(315, 36);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(64, 20);
			this.buttonSave.TabIndex = 19;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			// 
			// buttonImportFBX
			// 
			this.buttonImportFBX.ForeColor = System.Drawing.Color.White;
			this.buttonImportFBX.Location = new System.Drawing.Point(315, 10);
			this.buttonImportFBX.Name = "buttonImportFBX";
			this.buttonImportFBX.Size = new System.Drawing.Size(64, 20);
			this.buttonImportFBX.TabIndex = 18;
			this.buttonImportFBX.Text = "Import";
			this.buttonImportFBX.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(0, 491);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(190, 65);
			this.label3.TabIndex = 23;
			this.label3.Text = "- Alt+LMB - select all meshes, \r\n- Ctrl+LMB - add/remove to selection.\r\n- H - hid" +
    "e meshes.\r\n- Delete - delete meshes.\r\n- Space - edit selected mesh\'s material.";
			// 
			// openFlverDialog
			// 
			this.openFlverDialog.Filter = "FLVER|*.flver;*.bak";
			// 
			// checkBoxBackfaceCulling
			// 
			this.checkBoxBackfaceCulling.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxBackfaceCulling.AutoSize = true;
			this.checkBoxBackfaceCulling.ForeColor = System.Drawing.Color.White;
			this.checkBoxBackfaceCulling.Location = new System.Drawing.Point(783, 95);
			this.checkBoxBackfaceCulling.Name = "checkBoxBackfaceCulling";
			this.checkBoxBackfaceCulling.Size = new System.Drawing.Size(105, 17);
			this.checkBoxBackfaceCulling.TabIndex = 0;
			this.checkBoxBackfaceCulling.Text = "Backface culling";
			this.checkBoxBackfaceCulling.UseVisualStyleBackColor = true;
			// 
			// swapchainFormats
			// 
			this.swapchainFormats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.swapchainFormats.FormattingEnabled = true;
			this.swapchainFormats.Location = new System.Drawing.Point(754, 118);
			this.swapchainFormats.Name = "swapchainFormats";
			this.swapchainFormats.Size = new System.Drawing.Size(134, 56);
			this.swapchainFormats.TabIndex = 0;
			// 
			// checkBoxDiffuseSRgb
			// 
			this.checkBoxDiffuseSRgb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxDiffuseSRgb.AutoSize = true;
			this.checkBoxDiffuseSRgb.Checked = true;
			this.checkBoxDiffuseSRgb.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxDiffuseSRgb.Location = new System.Drawing.Point(796, 72);
			this.checkBoxDiffuseSRgb.Name = "checkBoxDiffuseSRgb";
			this.checkBoxDiffuseSRgb.Size = new System.Drawing.Size(92, 17);
			this.checkBoxDiffuseSRgb.TabIndex = 0;
			this.checkBoxDiffuseSRgb.Text = "Diffuse SRGB";
			this.checkBoxDiffuseSRgb.UseVisualStyleBackColor = true;
			// 
			// labelX
			// 
			this.labelX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelX.AutoSize = true;
			this.labelX.BackColor = System.Drawing.SystemColors.ControlDark;
			this.labelX.ForeColor = System.Drawing.Color.White;
			this.labelX.Location = new System.Drawing.Point(875, 479);
			this.labelX.Name = "labelX";
			this.labelX.Size = new System.Drawing.Size(19, 13);
			this.labelX.TabIndex = 0;
			this.labelX.Text = "20";
			// 
			// labelY
			// 
			this.labelY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelY.AutoSize = true;
			this.labelY.BackColor = System.Drawing.SystemColors.ControlDark;
			this.labelY.ForeColor = System.Drawing.Color.White;
			this.labelY.Location = new System.Drawing.Point(875, 505);
			this.labelY.Name = "labelY";
			this.labelY.Size = new System.Drawing.Size(19, 13);
			this.labelY.TabIndex = 1;
			this.labelY.Text = "20";
			// 
			// labelZ
			// 
			this.labelZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelZ.AutoSize = true;
			this.labelZ.BackColor = System.Drawing.SystemColors.ControlDark;
			this.labelZ.ForeColor = System.Drawing.Color.White;
			this.labelZ.Location = new System.Drawing.Point(872, 531);
			this.labelZ.Name = "labelZ";
			this.labelZ.Size = new System.Drawing.Size(19, 13);
			this.labelZ.TabIndex = 1;
			this.labelZ.Text = "60";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(534, 479);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Light X:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(534, 531);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Light Z:";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(534, 505);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(43, 13);
			this.label4.TabIndex = 2;
			this.label4.Text = "Light Y:";
			// 
			// sliderX
			// 
			this.sliderX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sliderX.BackColor = System.Drawing.SystemColors.ControlDark;
			this.sliderX.ColorSchema = ColorSlider.ColorSlider.ColorSchemas.RedColors;
			this.sliderX.ElapsedInnerColor = System.Drawing.Color.LightCoral;
			this.sliderX.ElapsedPenColorBottom = System.Drawing.Color.Salmon;
			this.sliderX.ElapsedPenColorTop = System.Drawing.Color.LightCoral;
			this.sliderX.Location = new System.Drawing.Point(583, 471);
			this.sliderX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.sliderX.Name = "sliderX";
			this.sliderX.Size = new System.Drawing.Size(286, 26);
			this.sliderX.TabIndex = 0;
			this.sliderX.Tag = "";
			this.sliderX.Text = "colorSlider1";
			this.sliderX.ThumbInnerColor = System.Drawing.Color.Red;
			this.sliderX.ThumbPenColor = System.Drawing.Color.Red;
			this.sliderX.ThumbRoundRectSize = new System.Drawing.Size(13, 13);
			this.sliderX.ThumbSize = new System.Drawing.Size(13, 13);
			this.sliderX.TickColor = System.Drawing.Color.Transparent;
			this.sliderX.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// sliderY
			// 
			this.sliderY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sliderY.BackColor = System.Drawing.SystemColors.ControlDark;
			this.sliderY.ColorSchema = ColorSlider.ColorSlider.ColorSchemas.GreenColors;
			this.sliderY.ElapsedInnerColor = System.Drawing.Color.Green;
			this.sliderY.ElapsedPenColorBottom = System.Drawing.Color.LightGreen;
			this.sliderY.ElapsedPenColorTop = System.Drawing.Color.SpringGreen;
			this.sliderY.Location = new System.Drawing.Point(583, 497);
			this.sliderY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.sliderY.Name = "sliderY";
			this.sliderY.Size = new System.Drawing.Size(286, 26);
			this.sliderY.TabIndex = 1;
			this.sliderY.Tag = "";
			this.sliderY.Text = "colorSlider1";
			this.sliderY.ThumbInnerColor = System.Drawing.Color.Green;
			this.sliderY.ThumbPenColor = System.Drawing.Color.Green;
			this.sliderY.ThumbRoundRectSize = new System.Drawing.Size(13, 13);
			this.sliderY.ThumbSize = new System.Drawing.Size(13, 13);
			this.sliderY.TickColor = System.Drawing.Color.Transparent;
			this.sliderY.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
			// 
			// sliderZ
			// 
			this.sliderZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.sliderZ.BackColor = System.Drawing.SystemColors.ControlDark;
			this.sliderZ.Location = new System.Drawing.Point(583, 523);
			this.sliderZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.sliderZ.Name = "sliderZ";
			this.sliderZ.Size = new System.Drawing.Size(286, 26);
			this.sliderZ.TabIndex = 1;
			this.sliderZ.Tag = "";
			this.sliderZ.Text = "colorSlider1";
			this.sliderZ.ThumbRoundRectSize = new System.Drawing.Size(13, 13);
			this.sliderZ.ThumbSize = new System.Drawing.Size(13, 13);
			this.sliderZ.TickColor = System.Drawing.Color.Transparent;
			this.sliderZ.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
			// 
			// labelGamma
			// 
			this.labelGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelGamma.AutoSize = true;
			this.labelGamma.Location = new System.Drawing.Point(850, 438);
			this.labelGamma.Name = "labelGamma";
			this.labelGamma.Size = new System.Drawing.Size(34, 13);
			this.labelGamma.TabIndex = 45;
			this.labelGamma.Text = "1.000";
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(583, 438);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(46, 13);
			this.label5.TabIndex = 44;
			this.label5.Text = "Gamma:";
			// 
			// labelExposure
			// 
			this.labelExposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExposure.AutoSize = true;
			this.labelExposure.Location = new System.Drawing.Point(850, 413);
			this.labelExposure.Name = "labelExposure";
			this.labelExposure.Size = new System.Drawing.Size(34, 13);
			this.labelExposure.TabIndex = 43;
			this.labelExposure.Text = "1.000";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(575, 413);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(54, 13);
			this.label6.TabIndex = 42;
			this.label6.Text = "Exposure:";
			// 
			// Exposure
			// 
			this.Exposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Exposure.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Exposure.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Exposure.Location = new System.Drawing.Point(635, 409);
			this.Exposure.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
			this.Exposure.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.Exposure.Name = "Exposure";
			this.Exposure.Size = new System.Drawing.Size(209, 20);
			this.Exposure.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.Exposure.TabIndex = 41;
			this.Exposure.Tag = "";
			this.Exposure.Text = "colorSlider1";
			this.Exposure.ThumbInnerColor = System.Drawing.Color.Gray;
			this.Exposure.ThumbPenColor = System.Drawing.Color.Gray;
			this.Exposure.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
			this.Exposure.ThumbSize = new System.Drawing.Size(6, 14);
			this.Exposure.TickColor = System.Drawing.Color.Transparent;
			this.Exposure.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// Gamma
			// 
			this.Gamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Gamma.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Gamma.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Gamma.Location = new System.Drawing.Point(635, 435);
			this.Gamma.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.Gamma.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
			this.Gamma.Name = "Gamma";
			this.Gamma.Size = new System.Drawing.Size(209, 20);
			this.Gamma.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            196608});
			this.Gamma.TabIndex = 40;
			this.Gamma.Tag = "";
			this.Gamma.Text = "colorSlider1";
			this.Gamma.ThumbInnerColor = System.Drawing.Color.Gray;
			this.Gamma.ThumbPenColor = System.Drawing.Color.Gray;
			this.Gamma.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
			this.Gamma.ThumbSize = new System.Drawing.Size(6, 14);
			this.Gamma.TickColor = System.Drawing.Color.Transparent;
			this.Gamma.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(898, 563);
			this.Controls.Add(this.labelGamma);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.labelExposure);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.Exposure);
			this.Controls.Add(this.Gamma);
			this.Controls.Add(this.sliderZ);
			this.Controls.Add(this.sliderY);
			this.Controls.Add(this.sliderX);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelZ);
			this.Controls.Add(this.labelY);
			this.Controls.Add(this.labelX);
			this.Controls.Add(this.checkBoxDiffuseSRgb);
			this.Controls.Add(this.swapchainFormats);
			this.Controls.Add(this.checkBoxBackfaceCulling);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonImportFBX);
			this.Controls.Add(this.comboBoxFlverVersion);
			this.Controls.Add(this.comboBoxShaders);
			this.Controls.Add(this.panel);
			this.Controls.Add(this.renderControl);
			this.DoubleBuffered = true;
			this.IsMdiContainer = true;
			this.KeyPreview = true;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TinyFLVER";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	public RenderControl renderControl;
	public Panel panel;
	public ComboBox comboBoxFlverVersion;
	public Button buttonSave;
	public Button buttonImportFBX;
	public ComboBox comboBoxShaders;
	private Label label3;
	public OpenFileDialog openFlverDialog;
	public CheckBox checkBoxBackfaceCulling;
	public ListBox swapchainFormats;
	public CheckBox checkBoxDiffuseSRgb;
	private Label labelX;
	private Label labelY;
	private Label labelZ;
	private Label label1;
	private Label label2;
	private Label label4;
	public ColorSlider.ColorSlider sliderX;
	public ColorSlider.ColorSlider sliderY;
	public ColorSlider.ColorSlider sliderZ;
	private Label labelGamma;
	private Label label5;
	private Label labelExposure;
	private Label label6;
	public ColorSlider.ColorSlider Exposure;
	public ColorSlider.ColorSlider Gamma;
}
