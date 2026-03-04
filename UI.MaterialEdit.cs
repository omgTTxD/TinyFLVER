using Newtonsoft.Json;
using SoulsFormats;
using System.Runtime.InteropServices;

namespace TinyFLVER;

public partial class MaterialEdit : Form
{
	List<TextBox> textureTypeList = [];
	List<TextBox> texturePathList = [];

	int materialIndex;

	public MaterialEdit(int materialIndex)
	{
		InitializeComponent();
		this.materialIndex = materialIndex;
		var material = Flver.flver.Materials[materialIndex];

		textBoxMaterialName.Text = material.Name;
		textBoxMaterialPath.Text = material.MTD;
		textBoxJson.Text = JsonConvert.SerializeObject(material);

		// Add controls for each texture
		int y = 0;
		for (int i = 0; i < material.Textures.Count; i++)
		{
			y += 35;
			groupBoxTextures.Controls.Add(new Label { Size = new Size(40, 15), Location = new Point(10, y), Text = "Type:" });
			TextBox textBoxType = new TextBox { Size = new Size(585, 15), Location = new Point(50, y - 3), Text = material.Textures[i].Type };
			groupBoxTextures.Controls.Add(textBoxType);
			textureTypeList.Add(textBoxType);

			y += 23;
			groupBoxTextures.Controls.Add(new Label { Size = new Size(40, 15), Location = new Point(10, y), Text = "Path:" });
			TextBox textBoxPath = new TextBox { Size = new Size(585, 15), Location = new Point(50, y - 3), Text = material.Textures[i].Path };
			groupBoxTextures.Controls.Add(textBoxPath);
			texturePathList.Add(textBoxPath);		
		}
	}


	private void buttonSaveJson_Click(object sender, EventArgs e)
	{
		Flver.flver.Materials[materialIndex] = JsonConvert.DeserializeObject<FLVER2.Material>(textBoxJson.Text);

		Close();
	}


	private void buttonOK_Click(object sender, EventArgs e)
	{
		var material = Flver.flver.Materials[materialIndex];
		material.MTD = textBoxMaterialPath.Text;
		material.Name = textBoxMaterialName.Text;

		for (int i = 0; i < material.Textures.Count; i++)
		{
			material.Textures[i].Path = texturePathList[i].Text;
			material.Textures[i].Type = textureTypeList[i].Text;
		}

		Close();
	}

	private void buttonCancel_Click(object sender, EventArgs e)
	{
		Close();
	}
}