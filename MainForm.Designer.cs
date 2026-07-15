partial class MainForm
{
	private System.ComponentModel.IContainer components = null;
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		Label label2;
		Label label4;
		BindingSource textureBindingSource;
		DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
		meshBindingSource = new BindingSource(components);
		dgMeshes = new DataGridView();
		hiddenDataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
		materialNameDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		materialMTDDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		dgTextures = new DataGridView();
		pathDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		typeDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
		Scale = new DataGridViewTextBoxColumn();
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
		buttonCopyMaterial = new Button();
		buttonPasteMaterial = new Button();
		AddTexture = new Label();
		GuessTexture = new CheckBox();
		comboBoxDebugID = new ComboBox();
		comboBoxCubemaps = new ComboBox();
		FlipY = new CheckBox();
		SwapXY = new CheckBox();
		FlipX = new CheckBox();
		RecalculateTangents = new CheckBox();
		RenderSpheres = new CheckBox();
		label2 = new Label();
		label4 = new Label();
		textureBindingSource = new BindingSource(components);
		((System.ComponentModel.ISupportInitialize)textureBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)meshBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)dgMeshes).BeginInit();
		((System.ComponentModel.ISupportInitialize)dgTextures).BeginInit();
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
		// textureBindingSource
		// 
		textureBindingSource.DataSource = typeof(FLVER2.Texture);
		// 
		// meshBindingSource
		// 
		meshBindingSource.DataSource = typeof(Mesh);
		// 
		// dgMeshes
		// 
		dgMeshes.AllowUserToAddRows = false;
		dgMeshes.AllowUserToResizeRows = false;
		dgMeshes.AutoGenerateColumns = false;
		dgMeshes.BackgroundColor = Color.Black;
		dgMeshes.BorderStyle = BorderStyle.None;
		dgMeshes.CausesValidation = false;
		dgMeshes.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dgMeshes.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
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
		dgMeshes.CellFormatting += dgMeshes_CellFormatting;
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
		dgTextures.AllowUserToAddRows = false;
		dgTextures.AllowUserToResizeRows = false;
		dgTextures.AutoGenerateColumns = false;
		dgTextures.BorderStyle = BorderStyle.None;
		dgTextures.CausesValidation = false;
		dgTextures.CellBorderStyle = DataGridViewCellBorderStyle.None;
		dgTextures.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
		dgTextures.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
		dgTextures.Columns.AddRange(new DataGridViewColumn[] { pathDataGridViewTextBoxColumn, typeDataGridViewTextBoxColumn, Scale });
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
		dgTextures.TabIndex = 51;
		dgTextures.Visible = false;
		dgTextures.CellFormatting += dataGridTextures_CellFormatting;
		dgTextures.CellParsing += dataGridTextures_CellParsing;
		dgTextures.EditingControlShowing += dataGridTextures_EditingControlShowing;
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
		// Scale
		// 
		Scale.DataPropertyName = "Scale";
		dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleCenter;
		Scale.DefaultCellStyle = dataGridViewCellStyle1;
		Scale.HeaderText = "Scale";
		Scale.Name = "Scale";
		Scale.Width = 40;
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
		buttonOpen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		buttonOpen.Location = new Point(845, 12);
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
		comboBoxShaders.Location = new Point(993, 42);
		comboBoxShaders.Name = "comboBoxShaders";
		comboBoxShaders.Size = new Size(121, 23);
		comboBoxShaders.TabIndex = 1;
		// 
		// LightAtCamera
		// 
		LightAtCamera.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		LightAtCamera.AutoSize = true;
		LightAtCamera.Checked = true;
		LightAtCamera.CheckState = CheckState.Checked;
		LightAtCamera.Location = new Point(975, 127);
		LightAtCamera.Name = "LightAtCamera";
		LightAtCamera.Size = new Size(136, 19);
		LightAtCamera.TabIndex = 55;
		LightAtCamera.Text = "Light follows camera";
		LightAtCamera.UseVisualStyleBackColor = true;
		// 
		// buttonCopyMaterial
		// 
		buttonCopyMaterial.Location = new Point(229, 11);
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
		// AddTexture
		// 
		AddTexture.AutoSize = true;
		AddTexture.BackColor = SystemColors.ControlDark;
		AddTexture.Location = new Point(204, 32);
		AddTexture.Name = "AddTexture";
		AddTexture.Size = new Size(15, 15);
		AddTexture.TabIndex = 61;
		AddTexture.Text = "+";
		AddTexture.Visible = false;
		// 
		// GuessTexture
		// 
		GuessTexture.AutoSize = true;
		GuessTexture.Location = new Point(16, 57);
		GuessTexture.Name = "GuessTexture";
		GuessTexture.Size = new Size(133, 19);
		GuessTexture.TabIndex = 62;
		GuessTexture.Text = "Try to guess textures";
		GuessTexture.UseVisualStyleBackColor = true;
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
		FlipY.Location = new Point(974, 223);
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
		SwapXY.Location = new Point(976, 248);
		SwapXY.Name = "SwapXY";
		SwapXY.Size = new Size(133, 19);
		SwapXY.TabIndex = 64;
		SwapXY.Text = "Swap n.XY after flips";
		SwapXY.UseVisualStyleBackColor = true;
		SwapXY.CheckedChanged += SwapXY_CheckedChanged;
		// 
		// FlipX
		// 
		FlipX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		FlipX.AutoSize = true;
		FlipX.Location = new Point(976, 198);
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
		RecalculateTangents.Location = new Point(976, 173);
		RecalculateTangents.Name = "RecalculateTangents";
		RecalculateTangents.Size = new Size(135, 19);
		RecalculateTangents.TabIndex = 71;
		RecalculateTangents.Text = "Recalculate tangents";
		RecalculateTangents.UseVisualStyleBackColor = true;
		// 
		// RenderSpheres
		// 
		RenderSpheres.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		RenderSpheres.AutoSize = true;
		RenderSpheres.Location = new Point(978, 150);
		RenderSpheres.Name = "RenderSpheres";
		RenderSpheres.Size = new Size(128, 19);
		RenderSpheres.TabIndex = 72;
		RenderSpheres.Text = "Render test spheres";
		RenderSpheres.UseVisualStyleBackColor = true;
		// 
		// MainForm
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		BackColor = SystemColors.ControlDark;
		ClientSize = new Size(1127, 602);
		Controls.Add(RenderSpheres);
		Controls.Add(RecalculateTangents);
		Controls.Add(FlipX);
		Controls.Add(SwapXY);
		Controls.Add(FlipY);
		Controls.Add(comboBoxCubemaps);
		Controls.Add(comboBoxDebugID);
		Controls.Add(GuessTexture);
		Controls.Add(AddTexture);
		Controls.Add(buttonPasteMaterial);
		Controls.Add(buttonCopyMaterial);
		Controls.Add(LightAtCamera);
		Controls.Add(comboBoxShaders);
		Controls.Add(Gamma);
		Controls.Add(Exposure);
		Controls.Add(buttonOpen);
		Controls.Add(labelGamma);
		Controls.Add(label4);
		Controls.Add(labelExposure);
		Controls.Add(label2);
		Controls.Add(buttonSave);
		Controls.Add(buttonImport);
		Controls.Add(flverVersions);
		Controls.Add(dgMeshes);
		Controls.Add(dgTextures);
		HelpButton = true;
		KeyPreview = true;
		Location = new Point(250, 50);
		Margin = new Padding(4, 3, 4, 3);
		MdiChildrenMinimizedAnchorBottom = false;
		Name = "MainForm";
		StartPosition = FormStartPosition.Manual;
		Text = "==";
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
	private BindingSource meshBindingSource;
	public Label labelExposure;
	public Label labelGamma;
	public TrackBar Exposure;
	public TrackBar Gamma;
	private DataGridViewTextBoxColumn texturePathColumn;
	private DataGridViewTextBoxColumn textureTypeDataGridViewTextBoxColumn;
	public Button buttonSave;
	public DataGridView dgMeshes;
	public DataGridView dgTextures;
	private DataGridViewTextBoxColumn texTypeDataGridViewTextBoxColumn;
	public ComboBox comboBoxShaders;
	public CheckBox LightAtCamera;
	private BindingSource textureBindingSource;
	private Label label2;
	private Label label4;
	private DataGridViewTextBoxColumn meshMaterialIndexDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn materialIndexDataGridViewTextBoxColumn;
	private Label AddTexture;
	private DataGridViewTextBoxColumn pathDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn typeDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn Scale;
	public CheckBox GuessTexture;
	private DataGridViewCheckBoxColumn hiddenDataGridViewCheckBoxColumn;
	private DataGridViewTextBoxColumn materialNameDataGridViewTextBoxColumn;
	private DataGridViewTextBoxColumn materialMTDDataGridViewTextBoxColumn;
	private ListBox listBox1;
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
}