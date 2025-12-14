using Newtonsoft.Json;
using SoulsFormats;

namespace TinyFLVER
{
	public partial class MaterialEdit : Form
	{
		readonly List<TextBox> textureTypeList = new List<TextBox>();
		readonly List<TextBox> texturePathList = new List<TextBox>();

		readonly FLVER2 flver;
		readonly int materialIndex;

		public MaterialEdit(FLVER2 _flver, int _materialIndex)
		{
			InitializeComponent();

			this.flver = _flver;
			this.materialIndex = _materialIndex;

			var material = flver.Materials[materialIndex];
			textBoxMaterialName.Text = material.Name;
			textBoxMaterialPath.Text = material.MTD;
			textBoxJson.Text = JsonConvert.SerializeObject(material);

			// Add controls for each texture
			int y = 20;
			for (int i = 0; i < material.Textures.Count; i++)
			{
				groupBoxTextures.Controls.Add(new Label { Size = new Size(40, 15), Location = new Point(10, y), Text = "Type:" });
				TextBox textBoxType = new TextBox { Size = new Size(585, 15), Location = new Point(50, y - 3), Text = material.Textures[i].Type };
				groupBoxTextures.Controls.Add(textBoxType);
				textureTypeList.Add(textBoxType);
				y += 20;

				groupBoxTextures.Controls.Add(new Label { Size = new Size(40, 15), Location = new Point(10, y), Text = "Path:" });
				TextBox textBoxPath = new TextBox { Size = new Size(585, 15), Location = new Point(50, y - 3), Text = material.Textures[i].Path };
				groupBoxTextures.Controls.Add(textBoxPath);
				texturePathList.Add(textBoxPath);
				y += 20;
			}

			// Change bottom controls location and form size because of added texture controls
			int addedHeight = 40 * material.Textures.Count;
			groupBoxTextures.Height += addedHeight;
			groupBoxJson.Location = new Point(groupBoxJson.Location.X, groupBoxJson.Location.Y + addedHeight);
			buttonSaveJson.Location = new Point(buttonSaveJson.Location.X, buttonSaveJson.Location.Y + addedHeight);
			buttonOK.Location = new Point(buttonOK.Location.X, buttonOK.Location.Y + addedHeight);
			buttonCancel.Location = new Point(buttonCancel.Location.X, buttonCancel.Location.Y + addedHeight);
			this.Height += addedHeight;
		}


		private void buttonOK_Click(object sender, EventArgs e)
		{
			var material = flver.Materials[materialIndex];

			material.MTD = textBoxMaterialPath.Text;
			material.Name = textBoxMaterialName.Text;

			for (int i = 0; i < material.Textures.Count; i++)
			{
				material.Textures[i].Path = texturePathList[i].Text;
				material.Textures[i].Type = textureTypeList[i].Text;
			}

			Close();
		}

		private void buttonSaveJson_Click(object sender, EventArgs e)
		{
			flver.Materials[materialIndex] = JsonConvert.DeserializeObject<FLVER2.Material>(textBoxJson.Text);
			Close();

		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MaterialEdit_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				Close();
			if (e.KeyCode == Keys.Enter)
				buttonOK_Click(sender, null);
		}
	}
}
