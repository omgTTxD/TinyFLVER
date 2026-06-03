using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Forms.NET.Controls;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Color = Microsoft.Xna.Framework.Color;

namespace TinyFLVER;

public partial class MonoGame : MonoGameControl
{
	public static BasicEffect Effect;
	public List<Mesh> meshes = [];

	Vector3 lookAt = new(0, 1, 0);
	Vector3 offset = new(0, 0.1f, 0);
	float zoom = 2.5f;
	Vector2 rotation = new(MathHelper.ToRadians(-20), MathHelper.ToRadians(10));
	MouseState mousePrevState;
	protected override void Initialize()
	{
	//	Editor.RemoveAllComponents();

		Effect = new BasicEffect(Editor.GraphicsDevice)
		{
			VertexColorEnabled = true, 
 			PreferPerPixelLighting = true,
			SpecularColor = new Vector3(0.4f),
		};
		
		Effect.EnableDefaultLighting();
		SetMultiSampleCount(8);

		Editor.BackgroundColor = new Color(16, 16, 16, 0);
	}

	// Calculate View and Projection Matrices (World == Identity)
	void CalculateMatrices()
	{
		Editor.GraphicsDevice.DepthStencilState = new DepthStencilState();

		var rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, 0);
		var position = lookAt + Vector3.Transform(Vector3.Forward, rotationMatrix) * zoom;

		Effect.World = Matrix.CreateTranslation(offset);
		Effect.View = Matrix.CreateLookAt(position, lookAt, Vector3.Transform(Vector3.Up, rotationMatrix));
		Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(40), Editor.GraphicsDevice.Viewport.AspectRatio, 0.001f, 100f);
	}

	// Input handling and matrices calculation
	protected override void Update(GameTime gameTime)
	{
		MouseState mouseState = Mouse.GetState();
		int dx = mouseState.X - mousePrevState.X;
		int dy = mouseState.Y - mousePrevState.Y;

		if (mouseState.RightButton == ButtonState.Pressed)
			rotation += new Vector2(-dx, dy) * 0.005f;

		if (mouseState.MiddleButton == ButtonState.Pressed)
			offset -= new Vector3(dx, dy, 0) * 0.001f;

		if (mouseState.ScrollWheelValue != mousePrevState.ScrollWheelValue)
			zoom += (mousePrevState.ScrollWheelValue - mouseState.ScrollWheelValue) / 600f;

		mousePrevState = mouseState;

		CalculateMatrices();
	}

	protected override void Draw()
	{
		Editor.BeginAntialising();
		meshes.ForEach(mesh => mesh.Draw());
		Editor.EndAntialising();
	}

	public Mesh[] GetSelectedMeshes() {
		return meshes.Where(x => x.checkBox is { Checked: true }).ToArray() ;
	}
}