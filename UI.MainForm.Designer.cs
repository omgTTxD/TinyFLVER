partial class MainForm
{
	private System.ComponentModel.IContainer components = null;
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		Label label2;
		Label label4;
		DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
		buttonAddTexture = new Button();
		dataGridMeshes = new DataGridView();
		meshMaterialIndex = new DataGridViewTextBoxColumn();
		materialIndexDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		columnMaterialName = new DataGridViewTextBoxColumn();
		materialMTDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
		meshBindingSource = new BindingSource(components);
		dataGridTextures = new DataGridView();
		textureBindingSource = new BindingSource(components);
		buttonSave = new Button();
		flverVersions = new ComboBox();
		buttonImport = new Button();
		labelExposure = new Label();
		labelGamma = new Label();
		buttonOpen = new Button();
		refreshTimer = new System.Windows.Forms.Timer(components);
		Exposure = new TrackBar();
		Gamma = new TrackBar();
		comboBoxShaders = new ComboBox();
		LightAtCamera = new CheckBox();
		ColumnTextures = new DataGridViewComboBoxColumn();
		pathDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		typeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
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
		label2.Location = new Point(756, 534);
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
		label4.Location = new Point(762, 563);
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
		buttonAddTexture.Location = new Point(378, 50);
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
		dataGridMeshes.BorderStyle = BorderStyle.None;
		dataGridMeshes.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dataGridMeshes.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
		dataGridMeshes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dataGridMeshes.ColumnHeadersHeight = 30;
		dataGridMeshes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
		dataGridMeshes.Columns.AddRange(new DataGridViewColumn[] { meshMaterialIndex, materialIndexDataGridViewTextBoxColumn, columnMaterialName, materialMTDDataGridViewTextBoxColumn, dataGridViewCheckBoxColumn1 });
		dataGridMeshes.DataSource = meshBindingSource;
		dataGridMeshes.Location = new Point(12, 12);
		dataGridMeshes.Margin = new Padding(0);
		dataGridMeshes.Name = "dataGridMeshes";
		dataGridMeshes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dataGridMeshes.RowHeadersVisible = false;
		dataGridMeshes.RowHeadersWidth = 20;
		dataGridMeshes.RowTemplate.Height = 18;
		dataGridMeshes.ScrollBars = ScrollBars.None;
		dataGridMeshes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		dataGridMeshes.Size = new Size(232, 31);
		dataGridMeshes.TabIndex = 50;
		dataGridMeshes.DataBindingComplete += dataGridMeshes_DataBindingComplete;
		dataGridMeshes.SelectionChanged += dataGridMeshes_SelectionChanged;
		// 
		// meshMaterialIndex
		// 
		meshMaterialIndex.DataPropertyName = "meshMaterialIndex";
		dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
		meshMaterialIndex.DefaultCellStyle = dataGridViewCellStyle1;
		meshMaterialIndex.HeaderText = "Mesh MatIdx";
		meshMaterialIndex.Name = "meshMaterialIndex";
		meshMaterialIndex.ReadOnly = true;
		meshMaterialIndex.Width = 40;
		// 
		// materialIndexDataGridViewTextBoxColumn
		// 
		materialIndexDataGridViewTextBoxColumn.DataPropertyName = "materialIndex";
		materialIndexDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
		materialIndexDataGridViewTextBoxColumn.HeaderText = "Mat Idx";
		materialIndexDataGridViewTextBoxColumn.Name = "materialIndexDataGridViewTextBoxColumn";
		materialIndexDataGridViewTextBoxColumn.ReadOnly = true;
		materialIndexDataGridViewTextBoxColumn.Width = 40;
		// 
		// columnMaterialName
		// 
		columnMaterialName.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		columnMaterialName.DataPropertyName = "materialName";
		columnMaterialName.HeaderText = "Name";
		columnMaterialName.Name = "columnMaterialName";
		columnMaterialName.ReadOnly = true;
		columnMaterialName.Width = 62;
		// 
		// materialMTDDataGridViewTextBoxColumn
		// 
		materialMTDDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		materialMTDDataGridViewTextBoxColumn.DataPropertyName = "materialMTD";
		materialMTDDataGridViewTextBoxColumn.HeaderText = "Path";
		materialMTDDataGridViewTextBoxColumn.Name = "materialMTDDataGridViewTextBoxColumn";
		materialMTDDataGridViewTextBoxColumn.ReadOnly = true;
		materialMTDDataGridViewTextBoxColumn.Width = 54;
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
		dataGridTextures.Columns.AddRange(new DataGridViewColumn[] { ColumnTextures, pathDataGridViewTextBoxColumn, typeDataGridViewTextBoxColumn });
		dataGridTextures.DataSource = textureBindingSource;
		dataGridTextures.Location = new Point(12, 51);
		dataGridTextures.Margin = new Padding(4, 3, 4, 3);
		dataGridTextures.Name = "dataGridTextures";
		dataGridTextures.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dataGridTextures.RowHeadersVisible = false;
		dataGridTextures.RowTemplate.Height = 20;
		dataGridTextures.ScrollBars = ScrollBars.None;
		dataGridTextures.SelectionMode = DataGridViewSelectionMode.CellSelect;
		dataGridTextures.Size = new Size(261, 17);
		dataGridTextures.TabIndex = 51;
		dataGridTextures.Visible = false;
		dataGridTextures.DataSourceChanged += dataGridTextures_DataSourceChanged;
		// 
		// textureBindingSource
		// 
		textureBindingSource.DataSource = typeof(FLVER2.Texture);
		// 
		// buttonSave
		// 
		buttonSave.ForeColor = Color.White;
		buttonSave.Location = new Point(405, 75);
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
		buttonImport.Location = new Point(405, 45);
		buttonImport.Margin = new Padding(4, 3, 4, 3);
		buttonImport.Name = "buttonImport";
		buttonImport.Size = new Size(75, 23);
		buttonImport.TabIndex = 18;
		buttonImport.Text = "Import";
		buttonImport.UseVisualStyleBackColor = true;
		buttonImport.Visible = false;
		// 
		// labelExposure
		// 
		labelExposure.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		labelExposure.AutoSize = true;
		labelExposure.Location = new Point(1073, 535);
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
		labelGamma.Location = new Point(1073, 564);
		labelGamma.Margin = new Padding(4, 0, 4, 0);
		labelGamma.Name = "labelGamma";
		labelGamma.Size = new Size(34, 15);
		labelGamma.TabIndex = 39;
		labelGamma.Text = "1.000";
		// 
		// buttonOpen
		// 
		buttonOpen.Location = new Point(405, 12);
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
		Exposure.Location = new Point(822, 532);
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
		Gamma.Location = new Point(822, 561);
		Gamma.Margin = new Padding(4, 3, 4, 3);
		Gamma.Maximum = 1000;
		Gamma.Minimum = 200;
		Gamma.Name = "Gamma";
		Gamma.Size = new Size(244, 26);
		Gamma.TabIndex = 53;
		Gamma.TickStyle = TickStyle.None;
		Gamma.Value = 1000;
		// 
		// comboBoxShaders
		// 
		comboBoxShaders.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		comboBoxShaders.FormattingEnabled = true;
		comboBoxShaders.Location = new Point(994, 45);
		comboBoxShaders.Name = "comboBoxShaders";
		comboBoxShaders.Size = new Size(121, 23);
		comboBoxShaders.TabIndex = 54;
		// 
		// LightAtCamera
		// 
		LightAtCamera.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		LightAtCamera.AutoSize = true;
		LightAtCamera.Checked = true;
		LightAtCamera.CheckState = CheckState.Checked;
		LightAtCamera.Location = new Point(979, 79);
		LightAtCamera.Name = "LightAtCamera";
		LightAtCamera.Size = new Size(136, 19);
		LightAtCamera.TabIndex = 55;
		LightAtCamera.Text = "Light follows camera";
		LightAtCamera.UseVisualStyleBackColor = true;
		// 
		// ColumnTextures
		// 
		ColumnTextures.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
		ColumnTextures.DataPropertyName = "Path";
		ColumnTextures.HeaderText = "Browse...";
		ColumnTextures.Name = "ColumnTextures";
		ColumnTextures.Resizable = DataGridViewTriState.False;
		ColumnTextures.Width = 58;
		// 
		// pathDataGridViewTextBoxColumn
		// 
		pathDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		pathDataGridViewTextBoxColumn.DataPropertyName = "Path";
		pathDataGridViewTextBoxColumn.HeaderText = "Path";
		pathDataGridViewTextBoxColumn.Name = "pathDataGridViewTextBoxColumn";
		pathDataGridViewTextBoxColumn.Width = 54;
		// 
		// typeDataGridViewTextBoxColumn
		// 
		typeDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		typeDataGridViewTextBoxColumn.DataPropertyName = "Type";
		typeDataGridViewTextBoxColumn.HeaderText = "Type";
		typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
		typeDataGridViewTextBoxColumn.Resizable = DataGridViewTriState.True;
		typeDataGridViewTextBoxColumn.Width = 54;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlLight;
		ClientSize = new Size(1127, 602);
		Controls.Add(LightAtCamera);
		Controls.Add(comboBoxShaders);
		Controls.Add(Gamma);
		Controls.Add(Exposure);
		Controls.Add(dataGridTextures);
		Controls.Add(buttonOpen);
		Controls.Add(labelGamma);
		Controls.Add(label4);
		Controls.Add(labelExposure);
		Controls.Add(label2);
		Controls.Add(buttonAddTexture);
		Controls.Add(buttonSave);
		Controls.Add(buttonImport);
		Controls.Add(flverVersions);
		Controls.Add(dataGridMeshes);
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

	public ComboBox flverVersions;
	public Button buttonImport;
	public Button buttonOpen;
	public System.Windows.Forms.Timer refreshTimer;
	private BindingSource meshBindingSource;
	public Label labelExposure;
	public Label labelGamma;
	public TrackBar Exposure;
	public TrackBar Gamma;
	private Button buttonAddTexture;
	private DataGridViewTextBoxColumn texturePathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn textureTypeDataGridViewTextBoxColumn;
	public Button buttonSave;
	public DataGridView dataGridMeshes;
	public DataGridView dataGridTextures;
	private DataGridViewTextBoxColumn texTypeDataGridViewTextBoxColumn;
	public ComboBox comboBoxShaders;
	public CheckBox LightAtCamera;
	private BindingSource textureBindingSource;
	private DataGridViewTextBoxColumn meshMaterialIndex;
	private DataGridViewTextBoxColumn materialIndexDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn columnMaterialName;
	private DataGridViewTextBoxColumn materialMTDDataGridViewTextBoxColumn;
	private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
	private DataGridViewComboBoxColumn ColumnTextures;
	private DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
}