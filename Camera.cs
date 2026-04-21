public struct Matrices()
{
	public Matrix World;
	public Matrix Projection;
}

public abstract class Camera
{
	public static Vector2 offset = new(0, 1.05f);
	public static float distance = 3;
	public static vec2 mousePos;

	public static Vector2 meshRot = new(-10, -10);
	public static Vector2 lightRot = new(-20, 30);

	public static void Initialize()
	{
		form.MouseMove += ProcessMouseInput;
		form.MouseWheel += ProcessMouseInput;
		form.MouseUp += (s, e) => ProjectVertices();
		
		SetupHDR();
		UpdateShaderData();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mousePos; mousePos = e.Location;

		if (e.Button == MouseButtons.None && e.Delta == 0)
			FindNearestMesh();

		if (e.Button == MouseButtons.Right)
			meshRot -= d / 2.75;

		if (e.Button == MouseButtons.Middle)
			offset += d * distance * 0.0004f;

		if (e.Button == MouseButtons.XButton1)
			lightRot -= d / 5;

		if (e.Delta != 0)
			distance -= e.Delta / 1500f;

		UpdateShaderData();
	}

	static void SetupHDR()
	{
		var monitors = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		var currentMonitor = monitors?.OpenSubKey(monitors?.GetSubKeyNames().First());

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
		shaderData.Exposure = exposure is not null ? float.Parse(exposure) / 1000f : 1;

		var devices = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
		var currentDevice = devices?.OpenSubKey(devices?.GetSubKeyNames().First()).OpenSubKey("Color");
		var nVidiaContrast = currentDevice?.GetValue("3538950")?.ToString();

		if (nVidiaContrast is null || int.Parse(nVidiaContrast) <= 100)
			shaderData.Gamma = 0.7f;
		else if (int.Parse(nVidiaContrast) <= 110)
			shaderData.Gamma = 0.85f;
		else
			shaderData.Gamma = 1;

		form.Exposure.Value = (decimal)shaderData.Exposure;
		form.Gamma.Value = (decimal)shaderData.Gamma;
	}

	// View matrix is identity, so we can just ignore it. Projection matrix is calculated in SwapChainManager.ProcessResize()
	public static void UpdateShaderData()
	{
		matrices.World = 
			Matrix.RotationY(toRadians(meshRot.X)) *		// Rotation around Y axis is made by mouse movement on X axis, therefore it is "rotation.X"
			Matrix.Translation(offset.X, -offset.Y, 0) *	// Mesh should rotate around camera offset position
			Matrix.RotationX(toRadians(meshRot.Y)) * 
			Matrix.Translation(0, 0, distance);

		shaderData.LightPosition = new (cos(lightRot.Y) * sin(lightRot.X), -sin(lightRot.Y), cos(lightRot.Y) * cos(lightRot.X));
	}

	public static void ProjectVertices()
	{
		int w = form.ClientSize.Width, h = form.ClientSize.Height;
		Meshes.ForEach(m =>	m.projected = [..m.vertices.Select(v => SharpDX.Vector3.Project(v.Position, 0, 0, w, h, 0, 1, matrices.World * matrices.Projection))]);
	}

	public static void FindNearestMesh()
	{
		float minZ = 1; nearestMesh = null;
		Meshes.ForEach(m => m.projected.ForEach(v => {
			if (Vector2.DistanceSquared(Camera.mousePos, v.xy) < 100 && v.z < minZ) 
			{
				minZ = v.z; 
				nearestMesh = m;
			}
		}));
	}
}