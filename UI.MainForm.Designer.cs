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
			this.Gamma = new ColorSlider.ColorSlider();
			this.Exposure = new ColorSlider.ColorSlider();
			this.RenderSpheres = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelExposure = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelGamma = new System.Windows.Forms.Label();
			this.debugList = new System.Windows.Forms.ListBox();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.hdrEnabled = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// comboBoxShaders
			// 
			this.comboBoxShaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxShaders.BackColor = System.Drawing.SystemColors.Window;
			this.comboBoxShaders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxShaders.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.comboBoxShaders.ForeColor = System.Drawing.SystemColors.WindowText;
			this.comboBoxShaders.FormattingEnabled = true;
			this.comboBoxShaders.ItemHeight = 13;
			this.comboBoxShaders.Location = new System.Drawing.Point(794, 37);
			this.comboBoxShaders.Name = "comboBoxShaders";
			this.comboBoxShaders.Size = new System.Drawing.Size(160, 21);
			this.comboBoxShaders.TabIndex = 0;
			// 
			// comboBoxFlverVersion
			// 
			this.comboBoxFlverVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxFlverVersion.FormattingEnabled = true;
			this.comboBoxFlverVersion.Location = new System.Drawing.Point(794, 12);
			this.comboBoxFlverVersion.Name = "comboBoxFlverVersion";
			this.comboBoxFlverVersion.Size = new System.Drawing.Size(160, 21);
			this.comboBoxFlverVersion.TabIndex = 16;
			// 
			// buttonSave
			// 
			this.buttonSave.ForeColor = System.Drawing.Color.White;
			this.buttonSave.Location = new System.Drawing.Point(378, 38);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(64, 20);
			this.buttonSave.TabIndex = 19;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			// 
			// buttonImportFBX
			// 
			this.buttonImportFBX.ForeColor = System.Drawing.Color.White;
			this.buttonImportFBX.Location = new System.Drawing.Point(378, 11);
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
			// Gamma
			// 
			this.Gamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Gamma.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Gamma.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Gamma.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Gamma.Location = new System.Drawing.Point(705, 489);
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
			this.Gamma.TabIndex = 28;
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
			// Exposure
			// 
			this.Exposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Exposure.BackColor = System.Drawing.SystemColors.ControlDark;
			this.Exposure.ElapsedInnerColor = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorBottom = System.Drawing.Color.Gray;
			this.Exposure.ElapsedPenColorTop = System.Drawing.Color.Gray;
			this.Exposure.Location = new System.Drawing.Point(705, 463);
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
			this.Exposure.TabIndex = 30;
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
			// RenderSpheres
			// 
			this.RenderSpheres.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.RenderSpheres.Location = new System.Drawing.Point(818, 64);
			this.RenderSpheres.Name = "RenderSpheres";
			this.RenderSpheres.Size = new System.Drawing.Size(136, 17);
			this.RenderSpheres.TabIndex = 30;
			this.RenderSpheres.Text = "Render Test Spheres";
			this.RenderSpheres.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(645, 467);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 13);
			this.label2.TabIndex = 35;
			this.label2.Text = "Exposure:";
			// 
			// labelExposure
			// 
			this.labelExposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExposure.AutoSize = true;
			this.labelExposure.Location = new System.Drawing.Point(920, 467);
			this.labelExposure.Name = "labelExposure";
			this.labelExposure.Size = new System.Drawing.Size(34, 13);
			this.labelExposure.TabIndex = 36;
			this.labelExposure.Text = "1.000";
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(653, 492);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(46, 13);
			this.label4.TabIndex = 38;
			this.label4.Text = "Gamma:";
			// 
			// labelGamma
			// 
			this.labelGamma.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelGamma.AutoSize = true;
			this.labelGamma.Location = new System.Drawing.Point(920, 492);
			this.labelGamma.Name = "labelGamma";
			this.labelGamma.Size = new System.Drawing.Size(34, 13);
			this.labelGamma.TabIndex = 39;
			this.labelGamma.Text = "1.000";
			// 
			// debugList
			// 
			this.debugList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.debugList.Items.AddRange(new object[] {
            "Result",
            "Diffuse",
            "Roughness",
            "SpecD",
            "SpecF",
            "SpecG",
            "Specular",
            "SssShadows",
            "SssTransmittance",
            "Sss",
            "Metalness"});
			this.debugList.Location = new System.Drawing.Point(885, 110);
			this.debugList.Name = "debugList";
			this.debugList.Size = new System.Drawing.Size(69, 147);
			this.debugList.TabIndex = 46;
			// 
			// buttonLoad
			// 
			this.buttonLoad.Location = new System.Drawing.Point(378, 65);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(64, 23);
			this.buttonLoad.TabIndex = 47;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.UseVisualStyleBackColor = true;
			// 
			// hdrEnabled
			// 
			this.hdrEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.hdrEnabled.AutoSize = true;
			this.hdrEnabled.Checked = true;
			this.hdrEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.hdrEnabled.Location = new System.Drawing.Point(818, 87);
			this.hdrEnabled.Name = "hdrEnabled";
			this.hdrEnabled.Size = new System.Drawing.Size(86, 17);
			this.hdrEnabled.TabIndex = 49;
			this.hdrEnabled.Text = "Enable HDR";
			this.hdrEnabled.UseVisualStyleBackColor = true;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlDark;
			this.ClientSize = new System.Drawing.Size(966, 526);
			this.Controls.Add(this.hdrEnabled);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.debugList);
			this.Controls.Add(this.labelGamma);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelExposure);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.RenderSpheres);
			this.Controls.Add(this.Exposure);
			this.Controls.Add(this.Gamma);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonImportFBX);
			this.Controls.Add(this.comboBoxFlverVersion);
			this.Controls.Add(this.comboBoxShaders);
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(250, 50);
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
	public ColorSlider.ColorSlider Gamma;
	public ColorSlider.ColorSlider Exposure;
	private Label label2;
	private Label labelExposure;
	private Label label4;
	private Label labelGamma;
	public ListBox debugList;
	public Button buttonLoad;
	public CheckBox RenderSpheres;
	public CheckBox hdrEnabled;
}
