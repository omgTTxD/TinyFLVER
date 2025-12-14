using SoulsFormats;
using System.Numerics;

namespace TinyFLVER
{
	public partial class MainForm : Form
	{
		public static FLVER2 flver;
		public static string path = "";

		public MainForm()
		{
			InitializeComponent();

			string[] args = Environment.GetCommandLineArgs();

			if (args.Length > 1)
				path = args[1];
			else
				//path = "A:\\tinyFLVER\\bd_f_2250\\BD_F_2250.flver";
				if (openFlverDialog.ShowDialog() == DialogResult.OK)
				path = openFlverDialog.FileName;

			if (path == "")
				Environment.Exit(0);

			// Make the directory of flver current working directory. It is needed mostly for textures search.
			Directory.SetCurrentDirectory(Path.GetDirectoryName(path));
			flver = FLVER2.Read(path);
		}

		// Activate() should happen on Load, or form will be inactive.
		private void MainForm_Load(object sender, EventArgs e)
		{
			Activate();
			foreach (var x in MainForm.flver.Meshes)
				renderControl.meshes.Add(new Mesh(x, MainForm.flver.Materials[x.MaterialIndex]));
			FillMeshControls();
		}

		// Add TextBoxes and CheckBoxes to form, one for each mesh
		public void FillMeshControls()
		{
			panel.Controls.Clear();
			for (int i = 0; i < renderControl.meshes.Count; i++)
			{
				var mesh = renderControl.meshes[i];
				int y = 20 * i + 10;

				var mat = mesh.material;
				panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(10, y), Text = mat.Name });
				panel.Controls.Add(new TextBox { Size = new Size(100, 15), Location = new Point(120, y), Text = mat.MTD });

				CheckBox cb = new() { Size = new Size(15, 15), Location = new Point(280, y + 5) };
				mesh.checkBox = cb;

				cb.Click += (s, e) => { mesh.SetSelectionState(cb.Checked); Refresh(); };
				panel.Controls.Add(cb);

				var matIndex = mesh.flverMesh.MaterialIndex;
				Button buttonEditMaterial = new() { Text = "Edit", Size = new Size(40, 20), Location = new Point(230, y) };
				buttonEditMaterial.Click += (s, e) => { new MaterialEdit(flver, matIndex).ShowDialog(); renderControl.meshes.ForEach(x => x.TryToFindTextures()); };
				panel.Controls.Add(buttonEditMaterial);
			}
		}

		private void buttonSave_Click(object sender, EventArgs e)
		{
			if (!File.Exists(path + ".bak"))
				File.Copy(path, path + ".bak", false);

			flver.Write(path);
			MessageBox.Show("Saved");
		}

		void DeleteMeshes()
		{
			List<FLVER2.Material> newMaterials = [];
			List<Mesh> meshesToDelete = [];
			foreach (var mesh in renderControl.meshes)
			{
				if (mesh.checkBox.Checked == false)
				{
					mesh.flverMesh.MaterialIndex = newMaterials.Count;
					newMaterials.Add(mesh.material);
				}
				if (mesh.checkBox.Checked == true)
				{
					flver.Meshes.Remove(mesh.flverMesh);
					meshesToDelete.Add(mesh);
				}
			}

			foreach (var mesh in meshesToDelete)
				renderControl.meshes.Remove(mesh);

			flver.Materials = newMaterials;
			FillMeshControls();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
				DeleteMeshes();
			if (e.KeyCode == Keys.Escape)
				Environment.Exit(0);
			if (e.KeyCode == Keys.PageDown)
				RotateFlver(-float.Pi / 2);
			if (e.KeyCode == Keys.PageUp)
				RotateFlver(float.Pi / 2);
			if (e.KeyCode == Keys.Space)
			{
				var selectedMeshes = renderControl.GetSelectedMeshes();
				if (selectedMeshes.Length == 1)
				{
					var mesh = selectedMeshes[0];
					new MaterialEdit(flver, mesh.flverMesh.MaterialIndex).ShowDialog(); 

					// Проверяем текстуры для всех мешей т.к. многие из них указывают на одну и ту же текстуру
					renderControl.meshes.ForEach(x => x.TryToFindTextures());
				}
			}
		}


		private void buttonImport_Click(object sender, EventArgs e)
		{
			FbxImport.importFBX(this);

			renderControl.meshes.Clear();
			foreach (var x in flver.Meshes)
				renderControl.meshes.Add(new Mesh(x, flver.Materials[x.MaterialIndex]));
			FillMeshControls();
		}

		private void renderControl_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				renderControl.TryToSelectMesh(new Microsoft.Xna.Framework.Vector2(e.X, e.Y));
		}

		public void RotateFlver(float angle)
		{
			Matrix4x4 rotation = Matrix4x4.CreateRotationX(angle);

			foreach (FLVER.Dummy d in flver.Dummies)
				d.Position = Vector3.Transform(d.Position, rotation);

			foreach (var mesh in renderControl.meshes)
			{
				if (mesh.checkBox.Checked)
				{
					foreach (FLVER.Vertex v in mesh.flverMesh.Vertices)
					{
						v.Position = Vector3.Transform(v.Position, rotation);
						v.Normal = Vector3.Transform(v.Normal, rotation);
						v.Tangents[0] = Vector4.Transform(v.Tangents[0], rotation);
					}
					mesh.SetVerticeData();
				}
			}
		}
	}
}



