namespace TinyFLVER
{
	partial class MaterialEdit
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxMaterialName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxMaterialPath = new System.Windows.Forms.TextBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSaveJson = new System.Windows.Forms.Button();
			this.groupBoxMaterial = new System.Windows.Forms.GroupBox();
			this.groupBoxTextures = new System.Windows.Forms.GroupBox();
			this.groupBoxJson = new System.Windows.Forms.GroupBox();
			this.textBoxJson = new System.Windows.Forms.TextBox();
			this.groupBoxMaterial.SuspendLayout();
			this.groupBoxJson.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(38, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// textBoxMaterialName
			// 
			this.textBoxMaterialName.Location = new System.Drawing.Point(45, 22);
			this.textBoxMaterialName.Name = "textBoxMaterialName";
			this.textBoxMaterialName.Size = new System.Drawing.Size(169, 20);
			this.textBoxMaterialName.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(32, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Path:";
			// 
			// textBoxMaterialPath
			// 
			this.textBoxMaterialPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxMaterialPath.Location = new System.Drawing.Point(45, 46);
			this.textBoxMaterialPath.Name = "textBoxMaterialPath";
			this.textBoxMaterialPath.Size = new System.Drawing.Size(422, 20);
			this.textBoxMaterialPath.TabIndex = 3;
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(492, 12);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(492, 41);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonSaveJson
			// 
			this.buttonSaveJson.Location = new System.Drawing.Point(492, 69);
			this.buttonSaveJson.Name = "buttonSaveJson";
			this.buttonSaveJson.Size = new System.Drawing.Size(75, 23);
			this.buttonSaveJson.TabIndex = 7;
			this.buttonSaveJson.Text = "Save JSON";
			this.buttonSaveJson.UseVisualStyleBackColor = true;
			this.buttonSaveJson.Click += new System.EventHandler(this.buttonSaveJson_Click);
			// 
			// groupBoxMaterial
			// 
			this.groupBoxMaterial.Controls.Add(this.label1);
			this.groupBoxMaterial.Controls.Add(this.textBoxMaterialName);
			this.groupBoxMaterial.Controls.Add(this.label2);
			this.groupBoxMaterial.Controls.Add(this.textBoxMaterialPath);
			this.groupBoxMaterial.Location = new System.Drawing.Point(12, 12);
			this.groupBoxMaterial.Name = "groupBoxMaterial";
			this.groupBoxMaterial.Size = new System.Drawing.Size(473, 80);
			this.groupBoxMaterial.TabIndex = 9;
			this.groupBoxMaterial.TabStop = false;
			this.groupBoxMaterial.Text = "Material";
			// 
			// groupBoxTextures
			// 
			this.groupBoxTextures.AutoSize = true;
			this.groupBoxTextures.Location = new System.Drawing.Point(11, 255);
			this.groupBoxTextures.Name = "groupBoxTextures";
			this.groupBoxTextures.Size = new System.Drawing.Size(560, 49);
			this.groupBoxTextures.TabIndex = 10;
			this.groupBoxTextures.TabStop = false;
			this.groupBoxTextures.Text = "Textures";
			// 
			// groupBoxJson
			// 
			this.groupBoxJson.AutoSize = true;
			this.groupBoxJson.Controls.Add(this.textBoxJson);
			this.groupBoxJson.Location = new System.Drawing.Point(10, 98);
			this.groupBoxJson.Name = "groupBoxJson";
			this.groupBoxJson.Size = new System.Drawing.Size(561, 152);
			this.groupBoxJson.TabIndex = 11;
			this.groupBoxJson.TabStop = false;
			this.groupBoxJson.Text = "JSON";
			// 
			// textBoxJson
			// 
			this.textBoxJson.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBoxJson.Location = new System.Drawing.Point(3, 16);
			this.textBoxJson.Multiline = true;
			this.textBoxJson.Name = "textBoxJson";
			this.textBoxJson.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxJson.Size = new System.Drawing.Size(555, 133);
			this.textBoxJson.TabIndex = 6;
			// 
			// MaterialEdit
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(579, 312);
			this.Controls.Add(this.groupBoxJson);
			this.Controls.Add(this.groupBoxTextures);
			this.Controls.Add(this.groupBoxMaterial);
			this.Controls.Add(this.buttonSaveJson);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MaterialEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MaterialEdit";
			this.groupBoxMaterial.ResumeLayout(false);
			this.groupBoxMaterial.PerformLayout();
			this.groupBoxJson.ResumeLayout(false);
			this.groupBoxJson.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxMaterialName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxMaterialPath;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonSaveJson;
		private System.Windows.Forms.GroupBox groupBoxMaterial;
		private System.Windows.Forms.GroupBox groupBoxTextures;
		private System.Windows.Forms.GroupBox groupBoxJson;
		private TextBox textBoxJson;
	}
}