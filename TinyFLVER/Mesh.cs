using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pfim;
using SoulsFormats;
using System.Linq;
using Color = Microsoft.Xna.Framework.Color;

namespace TinyFLVER
{
	public class Mesh
	{
		public CheckBox? checkBox;
		public FLVER2.Mesh flverMesh;
		public FLVER2.Material material;

		VertexPositionColorNormalTexture[] vertices = [];
		int[] indices = [];

		BasicEffect effect = MonoGame.Effect;
		Texture2D texture;
		static Dictionary<string, Texture2D> textureCache = [];

		public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
		{
			this.flverMesh = mesh;
			this.material = material;
			SetVerticeData();
			Task.Run(TryToFindTextures);
		}

		public void SetVerticeData()
		{
			// Position, normals and UVs don't change ever because we transform them with matrices before every Draw.
			// Only color changes when user selects meshes with mouse or check boxes. 
			vertices = [.. flverMesh.Vertices.Select( v => new VertexPositionColorNormalTexture(
				  new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
				  checkBox?.Checked == true ? Color.Red : Color.White,
				  new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z),
				  new Vector2(v.UVs[0].X, v.UVs[0].Y)
			))];
			indices = [.. flverMesh.FaceSets[0].Indices];
		}


		public void SetSelectionState(bool selected)
		{
			checkBox.Checked = selected;

			for (int i = 0; i < vertices.Length; i++)
				vertices[i].Color = selected ? Color.Red : Color.White;
		}

		// Checks if mouse clicked on mesh
		public float CheckIntersection(Vector2 mouseCrd)
		{
			float minZ = 1;
			foreach (var v in vertices)
			{
				// All other code I saw uses Unproject(). But it didn't work for me. Rays, distance between point and line segment - too hard. My way is simpler.
				// Just project point to screen again, to get exact screen coordinates. Very easy. Like all should be.
				Vector3 screenCrd = effect.GraphicsDevice.Viewport.Project(v.Position, effect.Projection, effect.View, effect.World);


				// Because we want to select faces on meshes and not only vertices themselves, distance should be big enoug. Then we find closest mesh to camera by
				// comparing Z.
				if (Vector2.Distance(mouseCrd, new(screenCrd.X, screenCrd.Y)) < 30 && screenCrd.Z < minZ)
					minZ = screenCrd.Z;
			}

			return minZ;
		}


		public void Draw()
		{
			if (indices.Length == 0) return;

			if (texture is not null)
				effect.Texture = texture;

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				pass.Apply();

			effect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
		}


		public void TryToFindTextures()
		{
			// First round: we find "_a" or "_d" in texture's path
			var textureInfo = material.Textures.FirstOrDefault(x => x.Path.Contains("_a") || x.Path.Contains("_d"), null);
			if (textureInfo is null)
				textureInfo = material.Textures.FirstOrDefault(x => x.Type.Contains("albedo") || x.Type.Contains("diffuse"), null);
			if (textureInfo is not null)
			{
				string textureName = Path.GetFileNameWithoutExtension(textureInfo.Path);

				// Several meshes might be using same texture so wee need to check cache and load if not cached yet
				if (textureCache.ContainsKey(textureName) == false)
				{
					try
					{
						var image = Pfimage.FromFile(textureName + ".dds");
						texture = new Texture2D(effect.GraphicsDevice, image.Width, image.Height);
						if (image.Format == ImageFormat.Rgba32)
						{
							for (int i = 0; i < image.Data.Length; i += 4)
							{
								var temp = image.Data[i + 2];
								image.Data[i + 2] = image.Data[i];
								image.Data[i] = temp;
							}

							texture.SetData(image.Data, 0, image.DataLen);
							textureCache[textureName] = texture;
						}
					}
					catch { texture = MonoGame.DefaultTexture; }
				}
				else
					texture = textureCache[textureName];
			}
			// if we didn't found albedo names or types, set temporary texture
			else
				texture = MonoGame.DefaultTexture;

		}
	}
}
