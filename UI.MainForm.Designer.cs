using SharpDX.Windows;

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
			this.comboBoxShaders = new System.Windows.Forms.ComboBox();
			this.comboBoxFlverVersion = new System.Windows.Forms.ComboBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonImportFBX = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.openFlverDialog = new System.Windows.Forms.OpenFileDialog();
			this.Gamma = new ColorSlider.ColorSlider();
			this.Exposure = new ColorSlider.ColorSlider();
			this.labelLightRotation = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.RenderSpheres = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelExposure = new System.Windows.Forms.Label();
			this.EnableHDR = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.labelGamma = new System.Windows.Forms.Label();
			this.debugList = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// comboBoxShaders
			// 
			this.comboBoxShaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxShaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxShaders.FormattingEnabled = true;
			this.comboBoxShaders.ItemHeight = 13;
			this.comboBoxShaders.Location = new System.Drawing.Point(796, 36);
			this.comboBoxShaders.Name = "comboBoxShaders";
			this.comboBoxShaders.Size = new System.Drawing.Size(160, 21);
			this.comboBoxShaders.TabIndex = 0;
			// 
			// comboBoxFlverVersion
			// 
			this.comboBoxFlverVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxFlverVersion.FormattingEnabled = true;
			this.comboBoxFlverVersion.Location = new System.Drawing.Point(796, 11);
			this.comboBoxFlverVersion.Name = "comboBoxFlverVersion";
			this.comboBoxFlverVersion.Size = new System.Drawing.Size(160, 21);
			this.comboBoxFlverVersion.TabIndex = 16;
			// 
			// buttonSave
			// 
			this.buttonSave.ForeColor = System.Drawing.Color.White;
			this.buttonSave.Location = new System.Drawing.Point(359, 37);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(64, 20);
			this.buttonSave.TabIndex = 19;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			// 
			// buttonImportFBX
			// 
			this.buttonImportFBX.ForeColor = System.Drawing.Color.White;
			this.buttonImportFBX.Location = new System.Drawing.Point(359, 10);
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
			this.label3.Location = new System.Drawing.Point(0, 454);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(0, 13);
			this.label3.TabIndex = 23;
			// 
			// openFlverDialog
			// 
			this.openFlverDialog.Filter = "FLVER|*.flver;*.bak";
			// 
			// Gamma
			// 
			this.Gamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Gamma.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Gamma.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Gamma.Location = new System.Drawing.Point(719, 469);
			this.Gamma.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
			this.Gamma.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.Gamma.Name = "Gamma";
			this.Gamma.Size = new System.Drawing.Size(209, 20);
			this.Gamma.TabIndex = 28;
			this.Gamma.Tag = "";
			this.Gamma.Text = "colorSlider1";
			this.Gamma.ThumbInnerColor = System.Drawing.Color.Gray;
			this.Gamma.ThumbPenColor = System.Drawing.Color.Gray;
			this.Gamma.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
			this.Gamma.ThumbSize = new System.Drawing.Size(6, 14);
			this.Gamma.TickColor = System.Drawing.Color.Transparent;
			this.Gamma.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// Exposure
			// 
			this.Exposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Exposure.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Exposure.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Exposure.Location = new System.Drawing.Point(719, 443);
			this.Exposure.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
			this.Exposure.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.Exposure.Name = "Exposure";
			this.Exposure.Size = new System.Drawing.Size(209, 20);
			this.Exposure.TabIndex = 30;
			this.Exposure.Tag = "";
			this.Exposure.Text = "colorSlider1";
			this.Exposure.ThumbInnerColor = System.Drawing.Color.Gray;
			this.Exposure.ThumbPenColor = System.Drawing.Color.Gray;
			this.Exposure.ThumbRoundRectSize = new System.Drawing.Size(2, 2);
			this.Exposure.ThumbSize = new System.Drawing.Size(6, 14);
			this.Exposure.TickColor = System.Drawing.Color.Transparent;
			this.Exposure.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
			// 
			// labelLightRotation
			// 
			this.labelLightRotation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelLightRotation.AutoSize = true;
			this.labelLightRotation.Location = new System.Drawing.Point(917, 504);
			this.labelLightRotation.Name = "labelLightRotation";
			this.labelLightRotation.Size = new System.Drawing.Size(25, 13);
			this.labelLightRotation.TabIndex = 32;
			this.labelLightRotation.Text = "0, 0";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(832, 504);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 33;
			this.label1.Text = "Light Rotation: ";
			// 
			// RenderSpheres
			// 
			this.RenderSpheres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RenderSpheres.AutoSize = true;
			this.RenderSpheres.Location = new System.Drawing.Point(828, 63);
			this.RenderSpheres.Name = "RenderSpheres";
			this.RenderSpheres.Size = new System.Drawing.Size(127, 17);
			this.RenderSpheres.TabIndex = 30;
			this.RenderSpheres.Text = "Render Test Spheres";
			this.RenderSpheres.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(659, 447);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 35;
			this.label2.Text = "Exposure:";
			// 
			// labelExposure
			// 
			this.labelExposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExposure.AutoSize = true;
			this.labelExposure.Location = new System.Drawing.Point(934, 447);
			this.labelExposure.Name = "labelExposure";
			this.labelExposure.Size = new System.Drawing.Size(13, 13);
			this.labelExposure.TabIndex = 36;
			this.labelExposure.Text = "1";
			// 
			// EnableHDR
			// 
			this.EnableHDR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.EnableHDR.AutoSize = true;
			this.EnableHDR.Checked = true;
			this.EnableHDR.CheckState = System.Windows.Forms.CheckState.Checked;
			this.EnableHDR.Location = new System.Drawing.Point(869, 86);
			this.EnableHDR.Name = "EnableHDR";
			this.EnableHDR.Size = new System.Drawing.Size(86, 17);
			this.EnableHDR.TabIndex = 37;
			this.EnableHDR.Text = "Enable HDR";
			this.EnableHDR.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(667, 472);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 13);
			this.label4.TabIndex = 38;
			this.label4.Text = "Gamma:";
			// 
			// labelGamma
			// 
			this.labelGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelGamma.AutoSize = true;
			this.labelGamma.Location = new System.Drawing.Point(934, 472);
			this.labelGamma.Name = "labelGamma";
			this.labelGamma.Size = new System.Drawing.Size(13, 13);
			this.labelGamma.TabIndex = 39;
			this.labelGamma.Text = "1";
			// 
			// debugList
			// 
			this.debugList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.debugList.Items.AddRange(new object[] {
            "Result",
            "Roughness",
            "Diffuse",
            "SpecD",
            "SpecF",
            "SpecG",
            "Specular",
            "SssShadows",
            "SssTransmittance",
            "Sss",
            "Ambient"});
			this.debugList.Location = new System.Drawing.Point(887, 109);
			this.debugList.Name = "debugList";
			this.debugList.Size = new System.Drawing.Size(69, 147);
			this.debugList.TabIndex = 46;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(966, 526);
			this.Controls.Add(this.debugList);
			this.Controls.Add(this.labelGamma);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.EnableHDR);
			this.Controls.Add(this.labelExposure);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.RenderSpheres);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelLightRotation);
			this.Controls.Add(this.Exposure);
			this.Controls.Add(this.Gamma);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonImportFBX);
			this.Controls.Add(this.comboBoxFlverVersion);
			this.Controls.Add(this.comboBoxShaders);
			this.KeyPreview = true;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "TinyFLVER";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseClick);
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion
	public ComboBox comboBoxFlverVersion;
	public Button buttonSave;
	public Button buttonImportFBX;
	public ComboBox comboBoxShaders;
	private Label label3;
	public OpenFileDialog openFlverDialog;
	public ColorSlider.ColorSlider Gamma;
	public ColorSlider.ColorSlider Exposure;
	private Label label1;
	public Label labelLightRotation;
	public CheckBox RenderSpheres;
	private Label label2;
	private Label labelExposure;
	public CheckBox EnableHDR;
	private Label label4;
	private Label labelGamma;
	public ListBox debugList;
}
