partial class MainForm
{
	private System.ComponentModel.IContainer components = null;
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
		DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
		GammaText = new Label();
		textureBindingSource = new BindingSource(components);
		ExposureText = new Label();
		meshBindingSource = new BindingSource(components);
		dgMeshes = new DataGridView();
		hiddenDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
		materialNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		materialMTDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		dgTextures = new DataGridView();
		ColumnBrowsePath = new DataGridViewComboBoxColumn();
		pathDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		typeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		ColumnTexScale = new DataGridViewTextBoxColumn();
		buttonSave = new Button();
		flverVersions = new ComboBox();
		buttonImport = new Button();
		ExposureValue = new Label();
		GammaValue = new Label();
		buttonOpen = new Button();
		refreshTimer = new System.Windows.Forms.Timer(components);
		Exposure = new TrackBar();
		Gamma = new TrackBar();
		comboBoxShaders = new ComboBox();
		buttonCopyMaterial = new Button();
		buttonPasteMaterial = new Button();
		comboBoxDebugID = new ComboBox();
		comboBoxCubemaps = new ComboBox();
		FlipY = new CheckBox();
		SwapXY = new CheckBox();
		FlipX = new CheckBox();
		RecalculateTangents = new CheckBox();
		RenderSpheres = new CheckBox();
		FlipZ = new CheckBox();
		SavedLocations = new ListBox();
		Link = new LinkLabel();
		((System.ComponentModel.ISupportInitialize)textureBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)meshBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)dgMeshes).BeginInit();
		((System.ComponentModel.ISupportInitialize)dgTextures).BeginInit();
		((System.ComponentModel.ISupportInitialize)Exposure).BeginInit();
		((System.ComponentModel.ISupportInitialize)Gamma).BeginInit();
		SuspendLayout();
		// 
		// GammaText
		// 
		GammaText.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		GammaText.AutoSize = true;
		GammaText.Location = new Point(762, 563);
		GammaText.Margin = new Padding(4, 0, 4, 0);
		GammaText.Name = "GammaText";
		GammaText.Size = new Size(52, 15);
		GammaText.TabIndex = 38;
		GammaText.Text = "Gamma:";
		// 
		// textureBindingSource
		// 
		textureBindingSource.DataSource = typeof(FLVER2.Texture);
		// 
		// ExposureText
		// 
		ExposureText.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		ExposureText.AutoSize = true;
		ExposureText.Location = new Point(756, 534);
		ExposureText.Margin = new Padding(4, 0, 4, 0);
		ExposureText.Name = "ExposureText";
		ExposureText.Size = new Size(58, 15);
		ExposureText.TabIndex = 35;
		ExposureText.Text = "Exposure:";
		// 
		// meshBindingSource
		// 
		meshBindingSource.DataSource = typeof(Mesh);
		// 
		// dgMeshes
		// 
		dgMeshes.AllowUserToResizeRows = false;
		dgMeshes.AutoGenerateColumns = false;
		dgMeshes.BorderStyle = BorderStyle.None;
		dgMeshes.CausesValidation = false;
		dgMeshes.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dgMeshes.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgMeshes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dgMeshes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dgMeshes.Columns.AddRange(new DataGridViewColumn[] { hiddenDataGridViewCheckBoxColumn, materialNameDataGridViewTextBoxColumn, materialMTDDataGridViewTextBoxColumn });
		dgMeshes.DataSource = meshBindingSource;
		dgMeshes.EditMode = DataGridViewEditMode.EditOnF2;
		dgMeshes.Location = new Point(9, 9);
		dgMeshes.Margin = new Padding(0);
		dgMeshes.Name = "dgMeshes";
		dgMeshes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dgMeshes.RowHeadersVisible = false;
		dgMeshes.RowHeadersWidth = 5;
		dgMeshes.RowTemplate.Height = 18;
		dgMeshes.ScrollBars = ScrollBars.None;
		dgMeshes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
		dgMeshes.ShowCellErrors = false;
		dgMeshes.ShowCellToolTips = false;
		dgMeshes.ShowEditingIcon = false;
		dgMeshes.Size = new Size(188, 20);
		dgMeshes.TabIndex = 0;
		dgMeshes.SelectionChanged += dataGridMeshes_SelectionChanged;
		// 
		// hiddenDataGridViewCheckBoxColumn
		// 
		hiddenDataGridViewCheckBoxColumn.DataPropertyName = "Hidden";
		hiddenDataGridViewCheckBoxColumn.HeaderText = "Hidden";
		hiddenDataGridViewCheckBoxColumn.Name = "hiddenDataGridViewCheckBoxColumn";
		hiddenDataGridViewCheckBoxColumn.Width = 50;
		// 
		// materialNameDataGridViewTextBoxColumn
		// 
		materialNameDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		materialNameDataGridViewTextBoxColumn.DataPropertyName = "materialName";
		materialNameDataGridViewTextBoxColumn.HeaderText = "Name";
		materialNameDataGridViewTextBoxColumn.Name = "materialNameDataGridViewTextBoxColumn";
		materialNameDataGridViewTextBoxColumn.Width = 62;
		// 
		// materialMTDDataGridViewTextBoxColumn
		// 
		materialMTDDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
		materialMTDDataGridViewTextBoxColumn.DataPropertyName = "materialMTD";
		materialMTDDataGridViewTextBoxColumn.HeaderText = "Material";
		materialMTDDataGridViewTextBoxColumn.Name = "materialMTDDataGridViewTextBoxColumn";
		materialMTDDataGridViewTextBoxColumn.Width = 73;
		// 
		// dgTextures
		// 
		dgTextures.AllowUserToResizeRows = false;
		dgTextures.AutoGenerateColumns = false;
		dgTextures.BorderStyle = BorderStyle.None;
		dgTextures.CausesValidation = false;
		dgTextures.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dgTextures.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
		dgTextures.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dgTextures.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dgTextures.Columns.AddRange(new DataGridViewColumn[] { ColumnBrowsePath, pathDataGridViewTextBoxColumn, typeDataGridViewTextBoxColumn, ColumnTexScale });
		dgTextures.DataSource = textureBindingSource;
		dgTextures.EditMode = DataGridViewEditMode.EditOnF2;
		dgTextures.Location = new Point(9, 32);
		dgTextures.Margin = new Padding(4, 3, 4, 3);
		dgTextures.Name = "dgTextures";
		dgTextures.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
		dgTextures.RowHeadersVisible = false;
		dgTextures.RowTemplate.Height = 20;
		dgTextures.ScrollBars = ScrollBars.None;
		dgTextures.SelectionMode = DataGridViewSelectionMode.CellSelect;
		dgTextures.ShowCellErrors = false;
		dgTextures.ShowCellToolTips = false;
		dgTextures.ShowEditingIcon = false;
		dgTextures.ShowRowErrors = false;
		dgTextures.Size = new Size(188, 17);
		dgTextures.TabIndex = 88;
		dgTextures.Visible = false;
		dgTextures.CellFormatting += dataGridTextures_CellFormatting;
		dgTextures.CellParsing += dataGridTextures_CellParsing;
		dgTextures.EditingControlShowing += dataGridTextures_EditingControlShowing;
		// 
		// ColumnBrowsePath
		// 
		ColumnBrowsePath.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
		ColumnBrowsePath.DataPropertyName = "Path";
		dataGridViewCellStyle1.NullValue = "Browse...";
		ColumnBrowsePath.DefaultCellStyle = dataGridViewCellStyle1;
		ColumnBrowsePath.DropDownWidth = 100;
		ColumnBrowsePath.FlatStyle = FlatStyle.Popup;
		ColumnBrowsePath.HeaderText = "Browse...";
		ColumnBrowsePath.MinimumWidth = 60;
		ColumnBrowsePath.Name = "ColumnBrowsePath";
		ColumnBrowsePath.SortMode = DataGridViewColumnSortMode.Programmatic;
		ColumnBrowsePath.Width = 60;
		// 
		// pathDataGridViewTextBoxColumn
		// 
		pathDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
		pathDataGridViewTextBoxColumn.DataPropertyName = "Path";
		pathDataGridViewTextBoxColumn.HeaderText = "Path";
		pathDataGridViewTextBoxColumn.MinimumWidth = 60;
		pathDataGridViewTextBoxColumn.Name = "pathDataGridViewTextBoxColumn";
		pathDataGridViewTextBoxColumn.Width = 60;
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
		// ColumnTexScale
		// 
		ColumnTexScale.DataPropertyName = "Scale";
		dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
		ColumnTexScale.DefaultCellStyle = dataGridViewCellStyle2;
		ColumnTexScale.HeaderText = "Scale";
		ColumnTexScale.Name = "ColumnTexScale";
		ColumnTexScale.Width = 40;
		// 
		// buttonSave
		// 
		buttonSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		buttonSave.ForeColor = Color.White;
		buttonSave.Location = new Point(846, 70);
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
		flverVersions.BackColor = SystemColors.Window;
		flverVersions.DropDownStyle = ComboBoxStyle.DropDownList;
		flverVersions.FormattingEnabled = true;
		flverVersions.Location = new Point(937, 12);
		flverVersions.Margin = new Padding(4, 3, 4, 3);
		flverVersions.Name = "flverVersions";
		flverVersions.Size = new Size(178, 23);
		flverVersions.TabIndex = 16;
		// 
		// buttonImport
		// 
		buttonImport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		buttonImport.ForeColor = Color.White;
		buttonImport.Location = new Point(846, 41);
		buttonImport.Margin = new Padding(4, 3, 4, 3);
		buttonImport.Name = "buttonImport";
		buttonImport.Size = new Size(75, 23);
		buttonImport.TabIndex = 18;
		buttonImport.Text = "Import";
		buttonImport.UseVisualStyleBackColor = true;
		buttonImport.Visible = false;
		// 
		// ExposureValue
		// 
		ExposureValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		ExposureValue.AutoSize = true;
		ExposureValue.Location = new Point(1073, 535);
		ExposureValue.Margin = new Padding(4, 0, 4, 0);
		ExposureValue.Name = "ExposureValue";
		ExposureValue.Size = new Size(34, 15);
		ExposureValue.TabIndex = 36;
		ExposureValue.Text = "1.000";
		// 
		// GammaValue
		// 
		GammaValue.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		GammaValue.AutoSize = true;
		GammaValue.Location = new Point(1073, 564);
		GammaValue.Margin = new Padding(4, 0, 4, 0);
		GammaValue.Name = "GammaValue";
		GammaValue.Size = new Size(34, 15);
		GammaValue.TabIndex = 39;
		GammaValue.Text = "1.000";
		// 
		// buttonOpen
		// 
		buttonOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		buttonOpen.Location = new Point(846, 13);
		buttonOpen.Margin = new Padding(4, 3, 4, 3);
		buttonOpen.Name = "buttonOpen";
		buttonOpen.Size = new Size(75, 23);
		buttonOpen.TabIndex = 47;
		buttonOpen.Text = "Open";
		buttonOpen.UseVisualStyleBackColor = true;
		// 
		// refreshTimer
		// 
		refreshTimer.Enabled = true;
		refreshTimer.Interval = 5;
		// 
		// Exposure
		// 
		Exposure.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		Exposure.Location = new Point(821, 529);
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
		comboBoxShaders.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxShaders.FormattingEnabled = true;
		comboBoxShaders.Location = new Point(993, 42);
		comboBoxShaders.Name = "comboBoxShaders";
		comboBoxShaders.Size = new Size(121, 23);
		comboBoxShaders.TabIndex = 0;
		// 
		// buttonCopyMaterial
		// 
		buttonCopyMaterial.Location = new Point(229, 6);
		buttonCopyMaterial.Name = "buttonCopyMaterial";
		buttonCopyMaterial.Size = new Size(75, 23);
		buttonCopyMaterial.TabIndex = 56;
		buttonCopyMaterial.Text = "Copy Mat.";
		buttonCopyMaterial.UseVisualStyleBackColor = true;
		buttonCopyMaterial.Visible = false;
		// 
		// buttonPasteMaterial
		// 
		buttonPasteMaterial.Location = new Point(229, 40);
		buttonPasteMaterial.Name = "buttonPasteMaterial";
		buttonPasteMaterial.Size = new Size(75, 23);
		buttonPasteMaterial.TabIndex = 57;
		buttonPasteMaterial.Text = "Paste Mat.";
		buttonPasteMaterial.UseVisualStyleBackColor = true;
		buttonPasteMaterial.Visible = false;
		// 
		// comboBoxDebugID
		// 
		comboBoxDebugID.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		comboBoxDebugID.DropDownStyle = ComboBoxStyle.DropDownList;
		comboBoxDebugID.FormattingEnabled = true;
		comboBoxDebugID.Items.AddRange(new object[] { "DiffuseColor", "Roughness", "Metalness", "Cubemap", "Irradiance", "DiffuseColor", "SpecularTint", "Result", "Specular", "SpecularIBL", "SpecularFull", "Diffuse", "DiffuseIBL", "DiffuseFull", "Fresnel", "Distribution", "Geometry" });
		comboBoxDebugID.Location = new Point(993, 71);
		comboBoxDebugID.Name = "comboBoxDebugID";
		comboBoxDebugID.Size = new Size(121, 23);
		comboBoxDebugID.TabIndex = 1;
		// 
		// comboBoxCubemaps
		// 
		comboBoxCubemaps.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		comboBoxCubemaps.FormattingEnabled = true;
		comboBoxCubemaps.Location = new Point(994, 98);
		comboBoxCubemaps.Name = "comboBoxCubemaps";
		comboBoxCubemaps.Size = new Size(121, 23);
		comboBoxCubemaps.TabIndex = 2;
		// 
		// FlipY
		// 
		FlipY.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		FlipY.AutoSize = true;
		FlipY.Location = new Point(982, 208);
		FlipY.Name = "FlipY";
		FlipY.Size = new Size(132, 19);
		FlipY.TabIndex = 63;
		FlipY.Text = "Flip n.Y (Bitangents)";
		FlipY.UseVisualStyleBackColor = true;
		// 
		// SwapXY
		// 
		SwapXY.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		SwapXY.AutoSize = true;
		SwapXY.Location = new Point(984, 232);
		SwapXY.Name = "SwapXY";
		SwapXY.Size = new Size(131, 19);
		SwapXY.TabIndex = 64;
		SwapXY.Text = "Swap XY (after flips)";
		SwapXY.UseVisualStyleBackColor = true;
		// 
		// FlipX
		// 
		FlipX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		FlipX.AutoSize = true;
		FlipX.Location = new Point(984, 184);
		FlipX.Name = "FlipX";
		FlipX.Size = new Size(123, 19);
		FlipX.TabIndex = 65;
		FlipX.Text = "Flip n.X (Tangents)";
		FlipX.UseVisualStyleBackColor = true;
		// 
		// RecalculateTangents
		// 
		RecalculateTangents.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		RecalculateTangents.AutoSize = true;
		RecalculateTangents.Location = new Point(984, 160);
		RecalculateTangents.Name = "RecalculateTangents";
		RecalculateTangents.Size = new Size(135, 19);
		RecalculateTangents.TabIndex = 71;
		RecalculateTangents.Text = "Recalculate tangents";
		RecalculateTangents.UseVisualStyleBackColor = false;
		// 
		// RenderSpheres
		// 
		RenderSpheres.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		RenderSpheres.AutoSize = true;
		RenderSpheres.Location = new Point(986, 137);
		RenderSpheres.Name = "RenderSpheres";
		RenderSpheres.Size = new Size(128, 19);
		RenderSpheres.TabIndex = 72;
		RenderSpheres.TabStop = false;
		RenderSpheres.Text = "Render test spheres";
		RenderSpheres.UseVisualStyleBackColor = true;
		// 
		// FlipZ
		// 
		FlipZ.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		FlipZ.AutoSize = true;
		FlipZ.Location = new Point(1052, 256);
		FlipZ.Name = "FlipZ";
		FlipZ.Size = new Size(65, 19);
		FlipZ.TabIndex = 89;
		FlipZ.Text = "Flip n.Z";
		FlipZ.UseVisualStyleBackColor = true;
		// 
		// SavedLocations
		// 
		SavedLocations.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		SavedLocations.DisplayMember = "Name";
		SavedLocations.FormattingEnabled = true;
		SavedLocations.Location = new Point(1019, 281);
		SavedLocations.Name = "SavedLocations";
		SavedLocations.Size = new Size(94, 139);
		SavedLocations.TabIndex = 90;
		SavedLocations.ValueMember = "Path";
		// 
		// Link
		// 
		Link.AutoSize = true;
		Link.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 204);
		Link.LinkColor = Color.DodgerBlue;
		Link.Location = new Point(846, 9);
		Link.Name = "Link";
		Link.Size = new Size(63, 25);
		Link.TabIndex = 91;
		Link.TabStop = true;
		Link.Text = "Link...";
		Link.Visible = false;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(96F, 96F);
		AutoScaleMode = AutoScaleMode.Dpi;
		AutoValidate = AutoValidate.Disable;
		BackColor = SystemColors.ControlDark;
		ClientSize = new Size(1127, 602);
		Controls.Add(SavedLocations);
		Controls.Add(FlipZ);
		Controls.Add(RenderSpheres);
		Controls.Add(RecalculateTangents);
		Controls.Add(FlipX);
		Controls.Add(SwapXY);
		Controls.Add(FlipY);
		Controls.Add(comboBoxCubemaps);
		Controls.Add(comboBoxDebugID);
		Controls.Add(buttonPasteMaterial);
		Controls.Add(buttonCopyMaterial);
		Controls.Add(comboBoxShaders);
		Controls.Add(Gamma);
		Controls.Add(Exposure);
		Controls.Add(buttonOpen);
		Controls.Add(GammaValue);
		Controls.Add(GammaText);
		Controls.Add(ExposureValue);
		Controls.Add(ExposureText);
		Controls.Add(buttonSave);
		Controls.Add(buttonImport);
		Controls.Add(flverVersions);
		Controls.Add(dgMeshes);
		Controls.Add(dgTextures);
		Controls.Add(Link);
		DoubleBuffered = true;
		KeyPreview = true;
		Location = new Point(499, 509);
		Margin = new Padding(4, 3, 4, 3);
		Name = "MainForm";
		StartPosition = FormStartPosition.Manual;
		Text = "Come...";
		KeyDown += MainForm_KeyDown;
		MouseClick += MainForm_MouseClick;
		((System.ComponentModel.ISupportInitialize)textureBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)meshBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)dgMeshes).EndInit();
		((System.ComponentModel.ISupportInitialize)dgTextures).EndInit();
		((System.ComponentModel.ISupportInitialize)Exposure).EndInit();
		((System.ComponentModel.ISupportInitialize)Gamma).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	public ComboBox flverVersions;
	public Button buttonImport;
	public Button buttonOpen;
	public System.Windows.Forms.Timer refreshTimer;
	//private BindingSource meshBindingSource;
	public Label ExposureValue;
	public Label GammaValue;
	public TrackBar Exposure;
	public TrackBar Gamma;
	public Button buttonSave;
	public DataGridView dgMeshes;
	public DataGridView dgTextures;
	public ComboBox comboBoxShaders;
	private DataGridViewCheckBoxColumn hiddenDataGridViewCheckBoxColumn;
	private DataGridViewTextBoxColumn materialNameDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn materialMTDDataGridViewTextBoxColumn;
	public ComboBox comboBoxDebugID;
	public ComboBox comboBoxCubemaps;
	public CheckBox checkBoxFlipNormalY;
	public CheckBox FlipY;
	public CheckBox SwapXY;
	public CheckBox FlipX;
	public CheckBox RecalculateTangents;
	public CheckBox RenderSpheres;
	public Button buttonCopyMaterial;
	public Button buttonPasteMaterial;
	public CheckBox FlipZ;
	public ListBox SavedLocations;
	private DataGridViewComboBoxColumn ColumnBrowsePath;
	private DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn ColumnTexScale;
	private LinkLabel Link;
	private Label GammaText;
	private BindingSource textureBindingSource;
	private Label ExposureText;
	private BindingSource meshBindingSource;
}