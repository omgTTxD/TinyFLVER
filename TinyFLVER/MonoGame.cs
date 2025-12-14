using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.NET.Controls;
using System.Windows.Forms;
using SoulsFormats;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;

namespace TinyFLVER
{
	public partial class MonoGame : MonoGameControl
	{
		public static BasicEffect Effect;
		public List<Mesh> meshes = [];
		public List<Mesh> selectedMesed = [];
		public static Texture2D DefaultTexture;

		Vector3 lookAt = new(0, 0.4f, 0);
		Vector3 offset = new(0, -0.5f, 0);
		float zoom = 1.75f;
		Vector2 rotation = new(MathHelper.ToRadians(-20), MathHelper.ToRadians(10));
		MouseState mousePrevState;

		protected override void Initialize()
		{
			SetMultiSampleCount(8);
			Effect = new BasicEffect(Editor.GraphicsDevice)
			{
				VertexColorEnabled = true,
				PreferPerPixelLighting = true,
				TextureEnabled = true,
				SpecularColor = Vector3.Zero				
			};
			Effect.EnableDefaultLighting();

			//Mesh.effect = Effect;
			
			DefaultTexture = new Texture2D(Editor.GraphicsDevice, 1, 1);
			DefaultTexture.SetData([Color.White]);
			
			CalculateMatrices();
		}

		// Calculate View and Projection Matrices (World == Identity)
		void CalculateMatrices()
		{
			var rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, 0);
			var position = lookAt + Vector3.Transform(Vector3.Forward, rotationMatrix) * zoom;

			Effect.View = Matrix.CreateLookAt(position, lookAt, Vector3.Transform(Vector3.Up, rotationMatrix)) * Matrix.CreateTranslation(offset);
			Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60), Editor.GraphicsDevice.Viewport.AspectRatio, 0.001f, 100f);
		}

		// Input handling and matrices calculation
		protected override void Update(GameTime gameTime)
		{
			MouseState mouseState = Mouse.GetState();
			int dx = mouseState.X - mousePrevState.X;
			int dy = mouseState.Y - mousePrevState.Y;

			if (mouseState.RightButton == ButtonState.Pressed)
				rotation += new Vector2(-dx, dy) * 0.01f;

			if (mouseState.MiddleButton == ButtonState.Pressed)
				offset += new Vector3(dx, -dy, 0) * 0.005f;

			if (mouseState.ScrollWheelValue != mousePrevState.ScrollWheelValue)
				zoom += (mousePrevState.ScrollWheelValue - mouseState.ScrollWheelValue) / 600f;

			mousePrevState = mouseState;

			CalculateMatrices();
		}

		protected override void Draw()
		{
			Editor.BackgroundColor = new Color(16, 16, 16, 0);
			Editor.GraphicsDevice.DepthStencilState = new DepthStencilState();

			Editor.BeginAntialising();
			meshes.ForEach(mesh => mesh.Draw());
			Editor.EndAntialising();
		}

		public Mesh[] GetSelectedMeshes() {
			return meshes.Where(x => x.checkBox.Checked).ToArray() ;
		}

		public void TryToSelectMesh(Vector2 mouseCrd)
		{
			// Selecting all if Alt is held
			if (ModifierKeys == System.Windows.Forms.Keys.Alt)
			{
				meshes.ForEach(mesh => mesh.SetSelectionState(true));
				return;
			}

			// I am doing this so that when click in one point there will be always new mesh selected
			var previouslySelected = GetSelectedMeshes();

			// Clear all before selection something new if Control isn't held
			if (ModifierKeys != System.Windows.Forms.Keys.Control) {
				meshes.ForEach(mesh => mesh.SetSelectionState(false));

				Mesh selected = meshes.Where(m => m.CheckIntersection(mouseCrd) < 1 && !previouslySelected.Contains(m)).MinBy(m => m.CheckIntersection(mouseCrd));

				selected?.SetSelectionState(true);
			}
			else {
				Mesh selected = meshes.Where(m => m.CheckIntersection(mouseCrd) < 1).MinBy(m => m.CheckIntersection(mouseCrd));
				selected?.SetSelectionState(!selected.checkBox.Checked);
			}		
		}
	}
}
