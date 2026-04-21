using SharpDX;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using Matrix = SharpDX.Matrix;
using Point = System.Drawing.Point;

namespace TinyFLVER;

public abstract class Camera
{
	static Point oldLocation;

	static Vector3 offset = new(0, -0.1f, 0) ;
	static Vector2 rotation = new (MathUtil.DegreesToRadians(-14), MathUtil.DegreesToRadians(-6));
	public static float zoom = 1.75f;

	public static Matrices matrices;
	static Buffer matrixBuffer;

	public struct Matrices(Matrix WorldViewProj, Matrix WorldView, float CameraDistance, float BgColor)
	{
		public Matrix WorldViewProj = WorldViewProj;
		public Matrix WorldView = WorldView;
		public float CameraDistance = CameraDistance;
		public float BgColor = BgColor;
		double padding;
	}

	public static void Initialize()
	{
		form.renderControl.MouseMove += ProcessMouseInput;
		form.renderControl.MouseWheel += ProcessMouseInput;

		matrixBuffer = Buffer.Create(dev, BindFlags.ConstantBuffer, ref matrices);
		c.VertexShader.SetConstantBuffer(0, matrixBuffer);

		CalculateMatrices();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		(int x, int y) d = (e.X - oldLocation.X, e.Y - oldLocation.Y);
		oldLocation = e.Location;

		// Moving mouse left and right (dx in mouse coordinates) should rotate model around Y axis, and dy should rotate around X.
		if (e.Button == MouseButtons.Right)
		{
			rotation.X -= d.y * 0.005f;
			rotation.Y -= d.x * 0.005f;
		}

		if (e.Button == MouseButtons.Middle)
			offset += new Vector3(d.x, -d.y, 0) * 0.001f;

		if (e.Delta != 0)
			zoom -= e.Delta * zoom / 2000;

		CalculateMatrices();
	}


	public static void CalculateMatrices()
	{
		// Translation by (0, -1, 0) is needed so that model would rotate around its center, and not around its feet. Translation by offset is needed
		// so when we zoom in to head, model would rotate around its head and not around its center. And after rotation we are zooming in
		var world = Matrix.Translation(new Vector3(0, -1, 0)) *
			Matrix.RotationY(rotation.Y) *
			Matrix.Translation(offset) * 
			Matrix.RotationX(rotation.X) * 
			Matrix.Translation(0, 0, zoom);

		// I've seen some code which creates matrix like this: LookAt(Vector3.Forward, Vector3.Zero, Vector3.UnitY). This is incorrect, since camera 
		// should be in (0, 0, 0) of camera space, and some shaders calculations laters depends on it.
		var view = Matrix.LookAtLH(Vector3.Zero, Vector3.ForwardLH, Vector3.UnitY);
		var proj = Matrix.PerspectiveFovLH(3.14f / 4, form.Width / (float)form.Height, 0.01f, 100f);

		matrices.WorldView = world * view;
		matrices.WorldViewProj = world * view * proj;
		matrices.CameraDistance = zoom;
		matrices.BgColor = SwapChainManager.bgColor;
		
		c.UpdateSubresource(ref matrices, matrixBuffer);
	}


	public static Vector3 ProjectVertexToScreen(System.Numerics.Vector3 v)
	{
		var vertex = Vector3.TransformCoordinate(new Vector3(v.X, v.Y, v.Z), matrices.WorldViewProj);

		// Matrices transform vertices to [-1; 1] coordinates, so we need to multiply by Height and Width
		vertex.X = (vertex.X + 1) / 2 * form.Width;
		vertex.Y = (-vertex.Y + 1) / 2 * form.Height;
		return vertex;
	}
}
