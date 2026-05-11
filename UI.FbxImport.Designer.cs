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
			this.chkBlenderTan = new System.Windows.Forms.CheckBox();
			this.txtImportPath = new System.Windows.Forms.TextBox();
			this.btnChooseImportFile = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.chkMirrorTertiary = new System.Windows.Forms.CheckBox();
			this.chkInverseTan = new System.Windows.Forms.CheckBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// chkBlenderTan
			// 
			this.chkBlenderTan.AutoSize = true;
			this.chkBlenderTan.Location = new System.Drawing.Point(12, 97);
			this.chkBlenderTan.Name = "chkBlenderTan";
			this.chkBlenderTan.Size = new System.Drawing.Size(393, 17);
			this.chkBlenderTan.TabIndex = 0;
			this.chkBlenderTan.Text = "Blender Tangents (Experimental, may fix blender exported FBX tangents issue)";
			// 
			// txtImportPath
			// 
			this.txtImportPath.Location = new System.Drawing.Point(12, 25);
			this.txtImportPath.Name = "txtImportPath";
			this.txtImportPath.Size = new System.Drawing.Size(355, 20);
			this.txtImportPath.TabIndex = 1;
			// 
			// btnChooseImportFile
			// 
			this.btnChooseImportFile.Location = new System.Drawing.Point(373, 22);
			this.btnChooseImportFile.Name = "btnChooseImportFile";
			this.btnChooseImportFile.Size = new System.Drawing.Size(75, 23);
			this.btnChooseImportFile.TabIndex = 2;
			this.btnChooseImportFile.Text = "Choose...";
			this.btnChooseImportFile.UseVisualStyleBackColor = true;
			this.btnChooseImportFile.Click += new System.EventHandler(this.btnChooseImportFile_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(79, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Import file path:";
			// 
			// chkMirrorTertiary
			// 
			this.chkMirrorTertiary.AutoSize = true;
			this.chkMirrorTertiary.Location = new System.Drawing.Point(12, 50);
			this.chkMirrorTertiary.Name = "chkMirrorTertiary";
			this.chkMirrorTertiary.Size = new System.Drawing.Size(128, 17);
			this.chkMirrorTertiary.TabIndex = 5;
			this.chkMirrorTertiary.Text = "Mirror Tertiary (Z) Axis";
			this.chkMirrorTertiary.UseVisualStyleBackColor = true;
			// 
			// chkInverseTan
			// 
			this.chkInverseTan.AutoSize = true;
			this.chkInverseTan.Location = new System.Drawing.Point(12, 74);
			this.chkInverseTan.Name = "chkInverseTan";
			this.chkInverseTan.Size = new System.Drawing.Size(114, 17);
			this.chkInverseTan.TabIndex = 6;
			this.chkInverseTan.Text = "Inverse tangent W";
			this.chkInverseTan.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Location = new System.Drawing.Point(289, 148);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 8;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(369, 148);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// FbxImport
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(460, 182);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.chkInverseTan);
			this.Controls.Add(this.chkMirrorTertiary);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnChooseImportFile);
			this.Controls.Add(this.txtImportPath);
			this.Controls.Add(this.chkBlenderTan);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "FbxImport";
			this.Text = "FbxImport";
			this.ResumeLayout(false);
			this.PerformLayout();

	}

	#endregion

	private System.Windows.Forms.CheckBox chkBlenderTan;
	private System.Windows.Forms.TextBox txtImportPath;
	private System.Windows.Forms.Button btnChooseImportFile;
	private System.Windows.Forms.Label label1;
	private System.Windows.Forms.CheckBox chkMirrorTertiary;
	private System.Windows.Forms.CheckBox chkInverseTan;
	private System.Windows.Forms.Button buttonOk;
	private System.Windows.Forms.Button buttonCancel;
}