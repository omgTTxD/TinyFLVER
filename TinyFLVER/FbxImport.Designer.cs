using System.Drawing;
using System.Drawing.Printing;

namespace TinyFLVER
{
    partial class FbxImport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			chkBlenderTan = new CheckBox();
			txtImportPath = new TextBox();
			btnChooseImportFile = new Button();
			label1 = new Label();
			chkMirrorTertiary = new CheckBox();
			chkInverseTan = new CheckBox();
			chkSetTexture = new CheckBox();
			buttonOk = new Button();
			buttonCancel = new Button();
			SuspendLayout();
			// 
			// chkBlenderTan
			// 
			chkBlenderTan.AutoSize = true;
			chkBlenderTan.Location = new Point(14, 112);
			chkBlenderTan.Margin = new Padding(4, 3, 4, 3);
			chkBlenderTan.Name = "chkBlenderTan";
			chkBlenderTan.Size = new Size(435, 19);
			chkBlenderTan.TabIndex = 0;
			chkBlenderTan.Text = "Blender Tangents (Experimental, may fix blender exported FBX tangents issue)";
			// 
			// txtImportPath
			// 
			txtImportPath.Location = new Point(14, 29);
			txtImportPath.Margin = new Padding(4, 3, 4, 3);
			txtImportPath.Name = "txtImportPath";
			txtImportPath.Size = new Size(414, 23);
			txtImportPath.TabIndex = 1;
			// 
			// btnChooseImportFile
			// 
			btnChooseImportFile.Location = new Point(435, 25);
			btnChooseImportFile.Margin = new Padding(4, 3, 4, 3);
			btnChooseImportFile.Name = "btnChooseImportFile";
			btnChooseImportFile.Size = new Size(88, 27);
			btnChooseImportFile.TabIndex = 2;
			btnChooseImportFile.Text = "Choose...";
			btnChooseImportFile.UseVisualStyleBackColor = true;
			btnChooseImportFile.Click += btnChooseImportFile_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(14, 10);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(92, 15);
			label1.TabIndex = 3;
			label1.Text = "Import file path:";
			// 
			// chkMirrorTertiary
			// 
			chkMirrorTertiary.AutoSize = true;
			chkMirrorTertiary.Location = new Point(14, 58);
			chkMirrorTertiary.Margin = new Padding(4, 3, 4, 3);
			chkMirrorTertiary.Name = "chkMirrorTertiary";
			chkMirrorTertiary.Size = new Size(143, 19);
			chkMirrorTertiary.TabIndex = 5;
			chkMirrorTertiary.Text = "Mirror Tertiary (Z) Axis";
			chkMirrorTertiary.UseVisualStyleBackColor = true;
			// 
			// chkInverseTan
			// 
			chkInverseTan.AutoSize = true;
			chkInverseTan.Location = new Point(14, 85);
			chkInverseTan.Margin = new Padding(4, 3, 4, 3);
			chkInverseTan.Name = "chkInverseTan";
			chkInverseTan.Size = new Size(121, 19);
			chkInverseTan.TabIndex = 6;
			chkInverseTan.Text = "Inverse tangent W";
			chkInverseTan.UseVisualStyleBackColor = true;
			// 
			// chkSetTexture
			// 
			chkSetTexture.AutoSize = true;
			chkSetTexture.Location = new Point(14, 139);
			chkSetTexture.Margin = new Padding(4, 3, 4, 3);
			chkSetTexture.Name = "chkSetTexture";
			chkSetTexture.Size = new Size(142, 19);
			chkSetTexture.TabIndex = 7;
			chkSetTexture.Text = "Auto set texture paths";
			chkSetTexture.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			buttonOk.Location = new Point(337, 171);
			buttonOk.Margin = new Padding(4, 3, 4, 3);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(88, 27);
			buttonOk.TabIndex = 8;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			buttonOk.Click += buttonOk_Click;
			// 
			// buttonCancel
			// 
			buttonCancel.Location = new Point(431, 171);
			buttonCancel.Margin = new Padding(4, 3, 4, 3);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(88, 27);
			buttonCancel.TabIndex = 9;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += buttonCancel_Click;
			// 
			// FbxImport
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(537, 210);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOk);
			Controls.Add(chkSetTexture);
			Controls.Add(chkInverseTan);
			Controls.Add(chkMirrorTertiary);
			Controls.Add(label1);
			Controls.Add(btnChooseImportFile);
			Controls.Add(txtImportPath);
			Controls.Add(chkBlenderTan);
			Margin = new Padding(4, 3, 4, 3);
			Name = "FbxImport";
			Text = "FbxImport";
			ResumeLayout(false);
			PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkBlenderTan;
        private System.Windows.Forms.TextBox txtImportPath;
        private System.Windows.Forms.Button btnChooseImportFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkMirrorTertiary;
        private System.Windows.Forms.CheckBox chkInverseTan;
        private System.Windows.Forms.CheckBox chkSetTexture;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
    }
}