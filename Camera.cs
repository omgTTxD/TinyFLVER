public abstract class Camera
{
	public static Vector2 cameraOffset = new(0, 1.15f);
	public static float cameraDistance = 2.5f;
	static Vector2 oldPosition;

	public static Vector2 meshRotation = new(-20, -10);
	public static Vector2 lightRotation = new(-20, 30);

	public static void Initialize()
	{
		form.MouseMove += ProcessMouseInput;
		form.MouseWheel += ProcessMouseInput;
		UpdateShaderData();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		Vector2 d =	new(e.X - oldPosition.X, e.Y - oldPosition.Y);
		oldPosition = new(e.X, e.Y);

		if (e.Button == MouseButtons.Right)
			meshRotation -= d / 2.75f;

		if (e.Button == MouseButtons.Middle)
			cameraOffset += d * cameraDistance * 0.0004f;

		if (e.Button == MouseButtons.XButton1)
			lightRotation -= d / 5;

		if (e.Delta != 0)
			cameraDistance -= e.Delta / 1500f;

		UpdateShaderData();
	}

	// View matrix is identity, so we can just ignore it. Projection matrix is calculated in SwapChainManager.ProcessResize()
	public static void UpdateShaderData()
	{
		vsData.World = 
			Matrix.RotationY(toRadians(meshRotation.X)) *					// Rotation around Y axis is made by mouse movement on X axis, therefore it is "rotation.X"
			Matrix.Translation(cameraOffset.X, -cameraOffset.Y, 0) *		// Mesh should rotate around camera offset position
			Matrix.RotationX(toRadians(meshRotation.Y)) *				
			Matrix.Translation(0, 0, cameraDistance);					   

		psData.LightPosition = new (
			 Cos(lightRotation.Y) * Sin(lightRotation.X), 
			-Sin(lightRotation.Y), 
			 Cos(lightRotation.Y) * Cos(lightRotation.X)
		);
	}
}