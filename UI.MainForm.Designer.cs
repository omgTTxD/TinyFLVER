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
		components = new System.ComponentModel.Container();
		Label label2;
		Label label4;
		DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
		buttonAddTexture = new Button();
		dataGridMeshes = new DataGridView();
		meshMaterialIndex = new DataGridViewTextBoxColumn();
		materialIndexDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		columnMaterialName = new DataGridViewTextBoxColumn();
		materialMTDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
		meshBindingSource = new BindingSource(components);
		dataGridTextures = new DataGridView();
		sRGBDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
		texturePathDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		textureTypeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		textureBindingSource = new BindingSource(components);
		buttonSave = new Button();
		flverVersions = new ComboBox();
		buttonImport = new Button();
		label3 = new Label();
		labelExposure = new Label();
		labelGamma = new Label();
		buttonOpen = new Button();
		refreshTimer = new System.Windows.Forms.Timer(components);
		Exposure = new TrackBar();
		Gamma = new TrackBar();
		buttonDelete = new Button();
		label2 = new Label();
		label4 = new Label();
		((System.ComponentModel.ISupportInitialize)dataGridMeshes).BeginInit();
		((System.ComponentModel.ISupportInitialize)meshBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)dataGridTextures).BeginInit();
		((System.ComponentModel.ISupportInitialize)textureBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)Exposure).BeginInit();
		((System.ComponentModel.ISupportInitialize)Gamma).BeginInit();
		SuspendLayout();
		// 
		// label2
		// 
		label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		label2.AutoSize = true;
		label2.Location = new Point(756, 539);
		label2.Margin = new Padding(4, 0, 4, 0);
		label2.Name = "label2";
		label2.Size = new Size(58, 15);
		label2.TabIndex = 35;
		label2.Text = "Exposure:";
		// 
		// label4
		// 
		label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		label4.AutoSize = true;
		label4.Location = new Point(762, 568);
		label4.Margin = new Padding(4, 0, 4, 0);
		label4.Name = "label4";
		label4.Size = new Size(52, 15);
		label4.TabIndex = 38;
		label4.Text = "Gamma:";
		// 
		// buttonAddTexture
		// 
		buttonAddTexture.BackColor = SystemColors.ControlDark;
		buttonAddTexture.FlatAppearance.BorderSize = 0;
		buttonAddTexture.FlatStyle = FlatStyle.Flat;
		buttonAddTexture.Font = new Font("Segoe UI", 9F);
		buttonAddTexture.Location = new Point(279, 50);
		buttonAddTexture.Margin = new Padding(4, 3, 4, 3);
		buttonAddTexture.Name = "buttonAddTexture";
		buttonAddTexture.Size = new Size(20, 21);
		buttonAddTexture.TabIndex = 53;
		buttonAddTexture.TabStop = false;
		buttonAddTexture.Text = "+";
		buttonAddTexture.UseVisualStyleBackColor = true;
		buttonAddTexture.Visible = false;
		buttonAddTexture.Click += buttonAddTexture_Click;
		// 
		// dataGridMeshes
		// 
		dataGridMeshes.AllowUserToAddRows = false;
		dataGridMeshes.AllowUserToResizeColumns = false;
		dataGridMeshes.AllowUserToResizeRows = false;
		dataGridMeshes.AutoGenerateColumns = false;
		dataGridMeshes.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
		dataGridMeshes.BorderStyle = BorderStyle.None;
		dataGridMeshes.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dataGridMeshes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle1.BackColor = SystemColors.ControlDark;
		dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
		dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
		dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
		dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
		dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
		dataGridMeshes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
		dataGridMeshes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridMeshes.Columns.AddRange(new DataGridViewColumn[] { meshMaterialIndex, materialIndexDataGridViewTextBoxColumn, columnMaterialName, materialMTDDataGridViewTextBoxColumn, dataGridViewCheckBoxColumn1 });
		dataGridMeshes.DataSource = meshBindingSource;
		dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleCenter;
		dataGridViewCellStyle5.BackColor = SystemColors.ControlDark;
		dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
		dataGridViewCellStyle5.ForeColor = SystemColors.ControlText;
		dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
		dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
		dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
		dataGridMeshes.DefaultCellStyle = dataGridViewCellStyle5;
		dataGridMeshes.EditMode = DataGridViewEditMode.EditOnEnter;
		dataGridMeshes.EnableHeadersVisualStyles = false;
		dataGridMeshes.Location = new Point(12, 12);
		dataGridMeshes.Margin = new Padding(0);
		dataGridMeshes.Name = "dataGridMeshes";
		dataGridMeshes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dataGridMeshes.RowHeadersVisible = false;
		dataGridMeshes.RowHeadersWidth = 20;
		dataGridMeshes.ScrollBars = ScrollBars.None;
		dataGridMeshes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		dataGridMeshes.Size = new Size(261, 36);
		dataGridMeshes.TabIndex = 50;
		dataGridMeshes.Visible = false;
		dataGridMeshes.SelectionChanged += dataGridMeshes_SelectionChanged;
		// 
		// meshMaterialIndex
		// 
		meshMaterialIndex.DataPropertyName = "meshMaterialIndex";
		meshMaterialIndex.HeaderText = "Mesh MatIdx";
		meshMaterialIndex.Name = "meshMaterialIndex";
		meshMaterialIndex.Width = 40;
		// 
		// materialIndexDataGridViewTextBoxColumn
		// 
		materialIndexDataGridViewTextBoxColumn.DataPropertyName = "materialIndex";
		dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
		materialIndexDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
		materialIndexDataGridViewTextBoxColumn.HeaderText = "Mat Idx";
		materialIndexDataGridViewTextBoxColumn.Name = "materialIndexDataGridViewTextBoxColumn";
		materialIndexDataGridViewTextBoxColumn.Width = 30;
		// 
		// columnMaterialName
		// 
		columnMaterialName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		columnMaterialName.DataPropertyName = "materialName";
		dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
		columnMaterialName.DefaultCellStyle = dataGridViewCellStyle3;
		columnMaterialName.HeaderText = "Name";
		columnMaterialName.Name = "columnMaterialName";
		columnMaterialName.Width = 58;
		// 
		// materialMTDDataGridViewTextBoxColumn
		// 
		materialMTDDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		materialMTDDataGridViewTextBoxColumn.DataPropertyName = "materialMTD";
		dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
		materialMTDDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
		materialMTDDataGridViewTextBoxColumn.HeaderText = "Path";
		materialMTDDataGridViewTextBoxColumn.Name = "materialMTDDataGridViewTextBoxColumn";
		materialMTDDataGridViewTextBoxColumn.Width = 52;
		// 
		// dataGridViewCheckBoxColumn1
		// 
		dataGridViewCheckBoxColumn1.DataPropertyName = "Hidden";
		dataGridViewCheckBoxColumn1.HeaderText = "Hide";
		dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
		dataGridViewCheckBoxColumn1.Width = 33;
		// 
		// meshBindingSource
		// 
		meshBindingSource.DataSource = typeof(Mesh);
		// 
		// dataGridTextures
		// 
		dataGridTextures.AllowUserToAddRows = false;
		dataGridTextures.AllowUserToResizeRows = false;
		dataGridTextures.AutoGenerateColumns = false;
		dataGridTextures.BorderStyle = BorderStyle.None;
		dataGridTextures.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dataGridTextures.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dataGridTextures.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dataGridTextures.Columns.AddRange(new DataGridViewColumn[] { sRGBDataGridViewCheckBoxColumn, texturePathDataGridViewTextBoxColumn, textureTypeDataGridViewTextBoxColumn });
		dataGridTextures.DataSource = textureBindingSource;
		dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
		dataGridViewCellStyle6.BackColor = SystemColors.Window;
		dataGridViewCellStyle6.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
		dataGridViewCellStyle6.ForeColor = SystemColors.ControlText;
		dataGridViewCellStyle6.SelectionBackColor = SystemColors.Highlight;
		dataGridViewCellStyle6.SelectionForeColor = SystemColors.HighlightText;
		dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
		dataGridTextures.DefaultCellStyle = dataGridViewCellStyle6;
		dataGridTextures.EditMode = DataGridViewEditMode.EditOnKeystroke;
		dataGridTextures.EnableHeadersVisualStyles = false;
		dataGridTextures.Location = new Point(12, 51);
		dataGridTextures.Margin = new Padding(4, 3, 4, 3);
		dataGridTextures.Name = "dataGridTextures";
		dataGridTextures.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dataGridTextures.RowHeadersVisible = false;
		dataGridTextures.ScrollBars = ScrollBars.None;
		dataGridTextures.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		dataGridTextures.Size = new Size(261, 17);
		dataGridTextures.TabIndex = 51;
		dataGridTextures.Visible = false;
		dataGridTextures.DataSourceChanged += dataGridTextures_DataSourceChanged;
		// 
		// sRGBDataGridViewCheckBoxColumn
		// 
		sRGBDataGridViewCheckBoxColumn.DataPropertyName = "sRGB";
		sRGBDataGridViewCheckBoxColumn.HeaderText = "sRGB";
		sRGBDataGridViewCheckBoxColumn.Name = "sRGBDataGridViewCheckBoxColumn";
		sRGBDataGridViewCheckBoxColumn.ReadOnly = true;
		sRGBDataGridViewCheckBoxColumn.Width = 35;
		// 
		// texturePathDataGridViewTextBoxColumn
		// 
		texturePathDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		texturePathDataGridViewTextBoxColumn.DataPropertyName = "TexturePath";
		texturePathDataGridViewTextBoxColumn.HeaderText = "Path";
		texturePathDataGridViewTextBoxColumn.Name = "texturePathDataGridViewTextBoxColumn";
		texturePathDataGridViewTextBoxColumn.Width = 54;
		// 
		// textureTypeDataGridViewTextBoxColumn
		// 
		textureTypeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		textureTypeDataGridViewTextBoxColumn.DataPropertyName = "TextureType";
		textureTypeDataGridViewTextBoxColumn.HeaderText = "Type";
		textureTypeDataGridViewTextBoxColumn.Name = "textureTypeDataGridViewTextBoxColumn";
		textureTypeDataGridViewTextBoxColumn.Width = 54;
		// 
		// textureBindingSource
		// 
		textureBindingSource.AllowNew = true;
		textureBindingSource.DataSource = typeof(Texture);
		// 
		// buttonSave
		// 
		buttonSave.ForeColor = Color.White;
		buttonSave.Location = new Point(306, 75);
		buttonSave.Margin = new Padding(4, 3, 4, 3);
		buttonSave.Name = "buttonSave";
		buttonSave.Size = new Size(75, 23);
		buttonSave.TabIndex = 19;
		buttonSave.Text = "Save";
		buttonSave.UseVisualStyleBackColor = true;
		buttonSave.Visible = false;
		// 
		// flverVersions
		// 
		flverVersions.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		flverVersions.BackColor = SystemColors.Control;
		flverVersions.DropDownStyle = ComboBoxStyle.DropDownList;
		flverVersions.FormattingEnabled = true;
		flverVersions.Location = new Point(960, 12);
		flverVersions.Margin = new Padding(4, 3, 4, 3);
		flverVersions.Name = "flverVersions";
		flverVersions.Size = new Size(154, 23);
		flverVersions.TabIndex = 16;
		// 
		// buttonImport
		// 
		buttonImport.ForeColor = Color.White;
		buttonImport.Location = new Point(306, 45);
		buttonImport.Margin = new Padding(4, 3, 4, 3);
		buttonImport.Name = "buttonImport";
		buttonImport.Size = new Size(75, 23);
		buttonImport.TabIndex = 18;
		buttonImport.Text = "Import";
		buttonImport.UseVisualStyleBackColor = true;
		buttonImport.Visible = false;
		// 
		// label3
		// 
		label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		label3.AutoSize = true;
		label3.Location = new Point(0, 524);
		label3.Margin = new Padding(4, 0, 4, 0);
		label3.Name = "label3";
		label3.Size = new Size(0, 15);
		label3.TabIndex = 23;
		// 
		// labelExposure
		// 
		labelExposure.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		labelExposure.AutoSize = true;
		labelExposure.Location = new Point(1073, 540);
		labelExposure.Margin = new Padding(4, 0, 4, 0);
		labelExposure.Name = "labelExposure";
		labelExposure.Size = new Size(34, 15);
		labelExposure.TabIndex = 36;
		labelExposure.Text = "1.000";
		// 
		// labelGamma
		// 
		labelGamma.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		labelGamma.AutoSize = true;
		labelGamma.Location = new Point(1073, 569);
		labelGamma.Margin = new Padding(4, 0, 4, 0);
		labelGamma.Name = "labelGamma";
		labelGamma.Size = new Size(34, 15);
		labelGamma.TabIndex = 39;
		labelGamma.Text = "1.000";
		// 
		// buttonOpen
		// 
		buttonOpen.Location = new Point(13, 12);
		buttonOpen.Margin = new Padding(4, 3, 4, 3);
		buttonOpen.Name = "buttonOpen";
		buttonOpen.Size = new Size(75, 27);
		buttonOpen.TabIndex = 47;
		buttonOpen.Text = "Open";
		buttonOpen.UseVisualStyleBackColor = true;
		// 
		// refreshTimer
		// 
		refreshTimer.Enabled = true;
		refreshTimer.Interval = 16;
		// 
		// Exposure
		// 
		Exposure.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		Exposure.Location = new Point(822, 537);
		Exposure.Margin = new Padding(4, 3, 4, 3);
		Exposure.Maximum = 15000;
		Exposure.Minimum = 1000;
		Exposure.Name = "Exposure";
		Exposure.Size = new Size(244, 26);
		Exposure.TabIndex = 52;
		Exposure.TickStyle = TickStyle.None;
		Exposure.Value = 1000;
		// 
		// Gamma
		// 
		Gamma.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		Gamma.Location = new Point(822, 566);
		Gamma.Margin = new Padding(4, 3, 4, 3);
		Gamma.Maximum = 1000;
		Gamma.Minimum = 200;
		Gamma.Name = "Gamma";
		Gamma.Size = new Size(244, 26);
		Gamma.TabIndex = 53;
		Gamma.TickStyle = TickStyle.None;
		Gamma.Value = 1000;
		// 
		// buttonDelete
		// 
		buttonDelete.Location = new Point(306, 104);
		buttonDelete.Name = "buttonDelete";
		buttonDelete.Size = new Size(75, 23);
		buttonDelete.TabIndex = 54;
		buttonDelete.Text = "Delete All";
		buttonDelete.UseVisualStyleBackColor = true;
		buttonDelete.Visible = false;
		buttonDelete.Click += buttonDelete_Click;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlLight;
		ClientSize = new Size(1127, 607);
		Controls.Add(buttonDelete);
		Controls.Add(Gamma);
		Controls.Add(Exposure);
		Controls.Add(dataGridTextures);
		Controls.Add(buttonOpen);
		Controls.Add(labelGamma);
		Controls.Add(label4);
		Controls.Add(labelExposure);
		Controls.Add(label2);
		Controls.Add(label3);
		Controls.Add(buttonAddTexture);
		Controls.Add(buttonSave);
		Controls.Add(buttonImport);
		Controls.Add(flverVersions);
		Controls.Add(dataGridMeshes);
		DoubleBuffered = true;
		KeyPreview = true;
		Location = new Point(250, 50);
		Margin = new Padding(4, 3, 4, 3);
		Name = "MainForm";
		StartPosition = FormStartPosition.Manual;
		Text = "TinyFLVER";
		KeyDown += MainForm_KeyDown;
		MouseClick += MainForm_MouseClick;
		((System.ComponentModel.ISupportInitialize)dataGridMeshes).EndInit();
		((System.ComponentModel.ISupportInitialize)meshBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)dataGridTextures).EndInit();
		((System.ComponentModel.ISupportInitialize)textureBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)Exposure).EndInit();
		((System.ComponentModel.ISupportInitialize)Gamma).EndInit();
		ResumeLayout(false);
		PerformLayout();

	}

	#endregion
	public ComboBox flverVersions;
	public Button buttonImport;
	private Label label3;
	private Label label2;
	private Label label4;
	public Button buttonOpen;
	public System.Windows.Forms.Timer refreshTimer;
	private BindingSource meshBindingSource;
	private BindingSource textureBindingSource;
	public Label labelExposure;
	public Label labelGamma;
	private DataGridViewTextBoxColumn meshMaterialIndex;
	private DataGridViewTextBoxColumn materialIndexDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn columnMaterialName;
	private DataGridViewTextBoxColumn materialMTDDataGridViewTextBoxColumn;
	private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
	public TrackBar Exposure;
	public TrackBar Gamma;
	private Button buttonAddTexture;
	private DataGridViewCheckBoxColumn sRGBDataGridViewCheckBoxColumn;
	private DataGridViewTextBoxColumn texturePathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn textureTypeDataGridViewTextBoxColumn;
	public Button buttonSave;
	private DataGridView dataGridTextures;
	public DataGridView dataGridMeshes;
	public Button buttonDelete;
}
