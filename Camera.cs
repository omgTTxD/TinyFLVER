using Microsoft.Win32;
using SharpDX;

public struct Matrices
{
	public Matrix4x4 World;
	public Matrix4x4 Proj;
}

public abstract class Camera
{
	public static vec3 offset = new(0, 1.15, 2.5);
	public static vec2 meshRotation = new(-10, -10);
	public static vec2 lightRotation = new(-20, 30);
	public static vec2 mousePos;

	public static void Initialize()
	{
		form.MouseWheel += (s, e) => { if (e.Delta != 0) { offset.z -= e.Delta / 1500f; UpdateShaderData(); } };
		form.MouseMove += ProcessMouseInput;
		UpdateShaderData();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mousePos; 
		mousePos = e.Location;

		if (e.Button == MouseButtons.None)
			return;

		if (e.Button == MouseButtons.Right)
			meshRotation -= d / 2.75F;

		if (e.Button == MouseButtons.Middle)
			offset.xy += d * offset.z * 0.0004F;

		if (e.Button == MouseButtons.XButton1)
			lightRotation -= d / 5;

		UpdateShaderData();
	}

	// View matrix is identity, so we can just ignore it. Projection matrix is calculated in SwapChainManager.ProcessResize()
	static void UpdateShaderData()
	{
			M.World = Matrix4x4.CreateRotationY(toRadians(meshRotation.x))			// Rotation around Y axis is made by mouse movement on X axis, therefore it is "rotation.X"
					* Matrix4x4.CreateTranslation(offset.x, -offset.y, 0)			// Mesh should rotate around camera offset position
					* Matrix4x4.CreateRotationX(toRadians(meshRotation.y))
					* Matrix4x4.CreateTranslation(0, 0, offset.z);

		shaderData.LightPosition = new (
			cos(lightRotation.y) * sin(lightRotation.x), 
			-sin(lightRotation.y), 
			cos(lightRotation.y) * cos(lightRotation.x));

		shaderData.Distance = offset.z;
	}
}