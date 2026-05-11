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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.shadersList = new System.Windows.Forms.ComboBox();
			this.flverVersions = new System.Windows.Forms.ComboBox();
			this.buttonSave = new System.Windows.Forms.Button();
			this.buttonImportFBX = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.Gamma = new ColorSlider.ColorSlider();
			this.Exposure = new ColorSlider.ColorSlider();
			this.label2 = new System.Windows.Forms.Label();
			this.labelExposure = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.labelGamma = new System.Windows.Forms.Label();
			this.buttonLoad = new System.Windows.Forms.Button();
			this.dataGridMeshes = new System.Windows.Forms.DataGridView();
			this.meshMaterialIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.materialIndexDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.columnMaterialName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.materialMTDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.meshBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dataGridTextures = new System.Windows.Forms.DataGridView();
			this.sRGBDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.texturePathDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.textureTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.textureBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.timer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.dataGridMeshes)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.meshBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridTextures)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textureBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// shadersList
			// 
			this.shadersList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.shadersList.BackColor = System.Drawing.SystemColors.ControlDark;
			this.shadersList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.shadersList.ForeColor = System.Drawing.SystemColors.WindowText;
			this.shadersList.FormattingEnabled = true;
			this.shadersList.ItemHeight = 13;
			this.shadersList.Location = new System.Drawing.Point(821, 47);
			this.shadersList.Name = "shadersList";
			this.shadersList.Size = new System.Drawing.Size(133, 21);
			this.shadersList.TabIndex = 0;
			// 
			// flverVersions
			// 
			this.flverVersions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flverVersions.BackColor = System.Drawing.SystemColors.Control;
			this.flverVersions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.flverVersions.FormattingEnabled = true;
			this.flverVersions.Location = new System.Drawing.Point(821, 22);
			this.flverVersions.Name = "flverVersions";
			this.flverVersions.Size = new System.Drawing.Size(133, 21);
			this.flverVersions.TabIndex = 16;
			// 
			// buttonSave
			// 
			this.buttonSave.Enabled = false;
			this.buttonSave.ForeColor = System.Drawing.Color.White;
			this.buttonSave.Location = new System.Drawing.Point(256, 65);
			this.buttonSave.Name = "buttonSave";
			this.buttonSave.Size = new System.Drawing.Size(64, 20);
			this.buttonSave.TabIndex = 19;
			this.buttonSave.Text = "Save";
			this.buttonSave.UseVisualStyleBackColor = true;
			// 
			// buttonImportFBX
			// 
			this.buttonImportFBX.Enabled = false;
			this.buttonImportFBX.ForeColor = System.Drawing.Color.White;
			this.buttonImportFBX.Location = new System.Drawing.Point(256, 39);
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
            1,
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
			// buttonLoad
			// 
			this.buttonLoad.Location = new System.Drawing.Point(256, 10);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(64, 23);
			this.buttonLoad.TabIndex = 47;
			this.buttonLoad.Text = "Load";
			this.buttonLoad.UseVisualStyleBackColor = true;
			// 
			// dataGridMeshes
			// 
			this.dataGridMeshes.AllowUserToAddRows = false;
			this.dataGridMeshes.AllowUserToResizeColumns = false;
			this.dataGridMeshes.AllowUserToResizeRows = false;
			this.dataGridMeshes.AutoGenerateColumns = false;
			this.dataGridMeshes.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.dataGridMeshes.BackgroundColor = System.Drawing.SystemColors.ControlDark;
			this.dataGridMeshes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridMeshes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridMeshes.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlDark;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridMeshes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridMeshes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridMeshes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.meshMaterialIndex,
            this.materialIndexDataGridViewTextBoxColumn,
            this.columnMaterialName,
            this.materialMTDDataGridViewTextBoxColumn,
            this.dataGridViewCheckBoxColumn1});
			this.dataGridMeshes.DataSource = this.meshBindingSource;
			dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlDark;
			dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridMeshes.DefaultCellStyle = dataGridViewCellStyle5;
			this.dataGridMeshes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dataGridMeshes.EnableHeadersVisualStyles = false;
			this.dataGridMeshes.Location = new System.Drawing.Point(10, 10);
			this.dataGridMeshes.Margin = new System.Windows.Forms.Padding(0);
			this.dataGridMeshes.Name = "dataGridMeshes";
			this.dataGridMeshes.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridMeshes.RowHeadersVisible = false;
			this.dataGridMeshes.RowHeadersWidth = 20;
			this.dataGridMeshes.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridMeshes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridMeshes.Size = new System.Drawing.Size(224, 31);
			this.dataGridMeshes.TabIndex = 50;
			this.dataGridMeshes.SelectionChanged += new System.EventHandler(this.dataGridMeshes_SelectionChanged);
			// 
			// meshMaterialIndex
			// 
			this.meshMaterialIndex.DataPropertyName = "meshMaterialIndex";
			this.meshMaterialIndex.HeaderText = "Mesh MatIdx";
			this.meshMaterialIndex.Name = "meshMaterialIndex";
			this.meshMaterialIndex.Width = 40;
			// 
			// materialIndexDataGridViewTextBoxColumn
			// 
			this.materialIndexDataGridViewTextBoxColumn.DataPropertyName = "materialIndex";
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.materialIndexDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.materialIndexDataGridViewTextBoxColumn.HeaderText = "Mat Idx";
			this.materialIndexDataGridViewTextBoxColumn.Name = "materialIndexDataGridViewTextBoxColumn";
			this.materialIndexDataGridViewTextBoxColumn.Width = 30;
			// 
			// columnMaterialName
			// 
			this.columnMaterialName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.columnMaterialName.DataPropertyName = "materialName";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.columnMaterialName.DefaultCellStyle = dataGridViewCellStyle3;
			this.columnMaterialName.HeaderText = "Name";
			this.columnMaterialName.Name = "columnMaterialName";
			this.columnMaterialName.Width = 58;
			// 
			// materialMTDDataGridViewTextBoxColumn
			// 
			this.materialMTDDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.materialMTDDataGridViewTextBoxColumn.DataPropertyName = "materialMTD";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			this.materialMTDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
			this.materialMTDDataGridViewTextBoxColumn.HeaderText = "Path";
			this.materialMTDDataGridViewTextBoxColumn.Name = "materialMTDDataGridViewTextBoxColumn";
			this.materialMTDDataGridViewTextBoxColumn.Width = 52;
			// 
			// dataGridViewCheckBoxColumn1
			// 
			this.dataGridViewCheckBoxColumn1.DataPropertyName = "Hidden";
			this.dataGridViewCheckBoxColumn1.HeaderText = "Hide";
			this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
			this.dataGridViewCheckBoxColumn1.Width = 33;
			// 
			// meshBindingSource
			// 
			this.meshBindingSource.DataSource = typeof(Mesh);
			// 
			// dataGridTextures
			// 
			this.dataGridTextures.AllowUserToAddRows = false;
			this.dataGridTextures.AllowUserToResizeRows = false;
			this.dataGridTextures.AutoGenerateColumns = false;
			this.dataGridTextures.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridTextures.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridTextures.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dataGridTextures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridTextures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sRGBDataGridViewCheckBoxColumn,
            this.texturePathDataGridViewTextBoxColumn,
            this.textureTypeDataGridViewTextBoxColumn});
			this.dataGridTextures.DataSource = this.textureBindingSource;
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridTextures.DefaultCellStyle = dataGridViewCellStyle6;
			this.dataGridTextures.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
			this.dataGridTextures.EnableHeadersVisualStyles = false;
			this.dataGridTextures.Location = new System.Drawing.Point(10, 44);
			this.dataGridTextures.Name = "dataGridTextures";
			this.dataGridTextures.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.dataGridTextures.RowHeadersVisible = false;
			this.dataGridTextures.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.dataGridTextures.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridTextures.Size = new System.Drawing.Size(224, 15);
			this.dataGridTextures.TabIndex = 51;
			// 
			// sRGBDataGridViewCheckBoxColumn
			// 
			this.sRGBDataGridViewCheckBoxColumn.DataPropertyName = "sRGB";
			this.sRGBDataGridViewCheckBoxColumn.HeaderText = "sRGB";
			this.sRGBDataGridViewCheckBoxColumn.Name = "sRGBDataGridViewCheckBoxColumn";
			this.sRGBDataGridViewCheckBoxColumn.ReadOnly = true;
			this.sRGBDataGridViewCheckBoxColumn.Width = 35;
			// 
			// texturePathDataGridViewTextBoxColumn
			// 
			this.texturePathDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.texturePathDataGridViewTextBoxColumn.DataPropertyName = "TexturePath";
			this.texturePathDataGridViewTextBoxColumn.HeaderText = "TexturePath";
			this.texturePathDataGridViewTextBoxColumn.Name = "texturePathDataGridViewTextBoxColumn";
			this.texturePathDataGridViewTextBoxColumn.Width = 88;
			// 
			// textureTypeDataGridViewTextBoxColumn
			// 
			this.textureTypeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.textureTypeDataGridViewTextBoxColumn.DataPropertyName = "TextureType";
			this.textureTypeDataGridViewTextBoxColumn.HeaderText = "TextureType";
			this.textureTypeDataGridViewTextBoxColumn.Name = "textureTypeDataGridViewTextBoxColumn";
			this.textureTypeDataGridViewTextBoxColumn.Width = 90;
			// 
			// textureBindingSource
			// 
			this.textureBindingSource.AllowNew = true;
			this.textureBindingSource.DataSource = typeof(Texture);
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 250;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.ClientSize = new System.Drawing.Size(966, 526);
			this.Controls.Add(this.dataGridTextures);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(this.labelGamma);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelExposure);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.Exposure);
			this.Controls.Add(this.Gamma);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.buttonSave);
			this.Controls.Add(this.buttonImportFBX);
			this.Controls.Add(this.flverVersions);
			this.Controls.Add(this.shadersList);
			this.Controls.Add(this.dataGridMeshes);
			this.DoubleBuffered = true;
			this.KeyPreview = true;
			this.Location = new System.Drawing.Point(250, 50);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "TinyFLVER";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseClick);
			((System.ComponentModel.ISupportInitialize)(this.dataGridMeshes)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.meshBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dataGridTextures)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textureBindingSource)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion
	public ComboBox flverVersions;
	public Button buttonSave;
	public Button buttonImportFBX;
	public ComboBox shadersList;
	private Label label3;
	public ColorSlider.ColorSlider Gamma;
	public ColorSlider.ColorSlider Exposure;
	private Label label2;
	private Label label4;
	public Button buttonLoad;
	public DataGridView dataGridMeshes;
	public DataGridView dataGridTextures;
	public System.Windows.Forms.Timer timer;
	private BindingSource meshBindingSource;
	private BindingSource textureBindingSource;
	public Label labelExposure;
	public Label labelGamma;
	private DataGridViewTextBoxColumn meshMaterialIndex;
	private DataGridViewTextBoxColumn materialIndexDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn columnMaterialName;
	private DataGridViewTextBoxColumn materialMTDDataGridViewTextBoxColumn;
	private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
	private DataGridViewCheckBoxColumn sRGBDataGridViewCheckBoxColumn;
	private DataGridViewTextBoxColumn texturePathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn textureTypeDataGridViewTextBoxColumn;
}
