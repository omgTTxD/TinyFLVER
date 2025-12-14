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
			label1 = new Label();
			textBoxMaterialName = new TextBox();
			label2 = new Label();
			textBoxMaterialPath = new TextBox();
			buttonOK = new Button();
			buttonCancel = new Button();
			textBoxJson = new TextBox();
			buttonSaveJson = new Button();
			groupBoxMaterial = new GroupBox();
			groupBoxTextures = new GroupBox();
			groupBoxJson = new GroupBox();
			groupBoxMaterial.SuspendLayout();
			groupBoxJson.SuspendLayout();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(7, 29);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(42, 15);
			label1.TabIndex = 0;
			label1.Text = "Name:";
			// 
			// textBoxMaterialName
			// 
			textBoxMaterialName.Location = new Point(52, 25);
			textBoxMaterialName.Margin = new Padding(4, 3, 4, 3);
			textBoxMaterialName.Name = "textBoxMaterialName";
			textBoxMaterialName.Size = new Size(196, 23);
			textBoxMaterialName.TabIndex = 1;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(8, 57);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(34, 15);
			label2.TabIndex = 2;
			label2.Text = "Path:";
			// 
			// textBoxMaterialPath
			// 
			textBoxMaterialPath.Location = new Point(52, 53);
			textBoxMaterialPath.Margin = new Padding(4, 3, 4, 3);
			textBoxMaterialPath.Name = "textBoxMaterialPath";
			textBoxMaterialPath.Size = new Size(682, 23);
			textBoxMaterialPath.TabIndex = 3;
			// 
			// buttonOK
			// 
			buttonOK.Location = new Point(575, 522);
			buttonOK.Margin = new Padding(4, 3, 4, 3);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(88, 27);
			buttonOK.TabIndex = 4;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			buttonOK.Click += buttonOK_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Location = new Point(670, 522);
			buttonCancel.Margin = new Padding(4, 3, 4, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(88, 27);
			buttonCancel.TabIndex = 5;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += buttonCancel_Click;
			// 
			// textBoxJson
			// 
			textBoxJson.Location = new Point(7, 22);
			textBoxJson.Margin = new Padding(4, 3, 4, 3);
			textBoxJson.Multiline = true;
			textBoxJson.Name = "textBoxJson";
			textBoxJson.ScrollBars = ScrollBars.Vertical;
			textBoxJson.Size = new Size(727, 319);
			textBoxJson.TabIndex = 6;
			// 
			// buttonSaveJson
			// 
			buttonSaveJson.Location = new Point(14, 522);
			buttonSaveJson.Margin = new Padding(4, 3, 4, 3);
			buttonSaveJson.Name = "buttonSaveJson";
			buttonSaveJson.Size = new Size(88, 27);
			buttonSaveJson.TabIndex = 7;
			buttonSaveJson.Text = "Save JSON";
			buttonSaveJson.UseVisualStyleBackColor = true;
			buttonSaveJson.Click += buttonSaveJson_Click;
			// 
			// groupBoxMaterial
			// 
			groupBoxMaterial.Controls.Add(label1);
			groupBoxMaterial.Controls.Add(textBoxMaterialName);
			groupBoxMaterial.Controls.Add(label2);
			groupBoxMaterial.Controls.Add(textBoxMaterialPath);
			groupBoxMaterial.Location = new Point(14, 14);
			groupBoxMaterial.Margin = new Padding(4, 3, 4, 3);
			groupBoxMaterial.Name = "groupBoxMaterial";
			groupBoxMaterial.Padding = new Padding(4, 3, 4, 3);
			groupBoxMaterial.Size = new Size(742, 92);
			groupBoxMaterial.TabIndex = 9;
			groupBoxMaterial.TabStop = false;
			groupBoxMaterial.Text = "Material";
			// 
			// groupBoxTextures
			// 
			groupBoxTextures.Location = new Point(14, 113);
			groupBoxTextures.Margin = new Padding(4, 3, 4, 3);
			groupBoxTextures.Name = "groupBoxTextures";
			groupBoxTextures.Padding = new Padding(4, 3, 4, 3);
			groupBoxTextures.Size = new Size(742, 39);
			groupBoxTextures.TabIndex = 10;
			groupBoxTextures.TabStop = false;
			groupBoxTextures.Text = "Textures";
			// 
			// groupBoxJson
			// 
			groupBoxJson.Controls.Add(textBoxJson);
			groupBoxJson.Location = new Point(14, 159);
			groupBoxJson.Margin = new Padding(4, 3, 4, 3);
			groupBoxJson.Name = "groupBoxJson";
			groupBoxJson.Padding = new Padding(4, 3, 4, 3);
			groupBoxJson.Size = new Size(742, 355);
			groupBoxJson.TabIndex = 11;
			groupBoxJson.TabStop = false;
			groupBoxJson.Text = "JSON";
			// 
			// MaterialEdit
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(768, 567);
			Controls.Add(groupBoxJson);
			Controls.Add(groupBoxTextures);
			Controls.Add(groupBoxMaterial);
			Controls.Add(buttonSaveJson);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOK);
			FormBorderStyle = FormBorderStyle.Fixed3D;
			KeyPreview = true;
			Margin = new Padding(4, 3, 4, 3);
			Name = "MaterialEdit";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "MaterialEdit";
			KeyDown += MaterialEdit_KeyDown;
			groupBoxMaterial.ResumeLayout(false);
			groupBoxMaterial.PerformLayout();
			groupBoxJson.ResumeLayout(false);
			groupBoxJson.PerformLayout();
			ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMaterialName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMaterialPath;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxJson;
        private System.Windows.Forms.Button buttonSaveJson;
        private System.Windows.Forms.GroupBox groupBoxMaterial;
        private System.Windows.Forms.GroupBox groupBoxTextures;
        private System.Windows.Forms.GroupBox groupBoxJson;
    }
}