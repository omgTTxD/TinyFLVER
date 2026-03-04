using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector3 = SharpDX.Vector3;

namespace TinyFLVER;

public abstract class Camera
{
	public static Vector2 offset = new(0, -1.1f);
	public static float distance = 1.75f;
	static Vector2 oldPosition;

	public static Vector2 meshRotation = new(toRadians(-20), toRadians(-10));
	public static Vector2 lightRotation = new(toRadians(20), toRadians(30));

	public static void Initialize()
	{
		form.MouseMove += ProcessMouseInput;
		form.MouseWheel += ProcessMouseInput;
		UpdateData();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		Vector2 d = new(e.X - oldPosition.X, e.Y - oldPosition.Y);
		oldPosition = new(e.X, e.Y);

		if (e.Button == MouseButtons.Middle)
			offset -= new Vector2(d.X, d.Y) * distance * 0.0005f;

		if (e.Button == MouseButtons.Right)
			meshRotation -= d * 0.005f;

		if (e.Button == MouseButtons.XButton1)
			lightRotation += new Vector2(-d.X, d.Y) * 0.002f;

		if (e.Delta != 0)
			distance -= e.Delta / 1500f;

		UpdateData();
	}

	public static void UpdateData()
	{
		var World = 
			Matrix.RotationY(meshRotation.X) *					// Rotation around Y axis is made by mouse movement on X axis, therefore it is "rotation.X"
			Matrix.Translation(-offset.X, offset.Y, 0) *		// Mesh should rotate around camera offset position
			Matrix.RotationX(meshRotation.Y) *
			Matrix.Translation(0, 0, distance);

		// Camera should be in (0, 0, 0) in Camera Space, so creating it like this: "LookAt(Vector3.Forward, Vector3.Zero, ...)" is incorrect.
		var View = Matrix.LookAtLH(Vector3.Zero, Vector3.ForwardLH, Vector3.UnitY);
		var Proj = Matrix.PerspectiveFovLH(Pi/4, form.Width / (float)form.Height , 0.01f, 100f);

		vsData.WorldView = World * View;
		vsData.WorldViewProj = World * View * Proj;
		psData.LightPosition = new(Cos(lightRotation.Y) * Sin(lightRotation.X), Sin(lightRotation.Y), -Cos(lightRotation.Y) * Cos(lightRotation.X));
		form.labelLightRotation.Text = $"{toDegrees(lightRotation.X)}, {toDegrees(lightRotation.Y)}";
	}
}