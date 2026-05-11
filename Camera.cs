using Microsoft.Win32;

public struct Matrices()
{
	public Matrix World;
	public Matrix Projection;
}

public abstract class Camera
{
	public static vec3 offset = new(0, 1.3, 2);
	public static vec2 meshRot = new(-10, -10);
	public static vec2 lightRot = new(-20, 30);
	public static vec2 mousePos;

	public static void Initialize()
	{
		form.MouseMove += ProcessMouseInput;
		form.MouseWheel += ProcessMouseInput;
		form.MouseUp += (s, e) => ProjectVertices();
		SetupHDR();
		UpdateShaderData();
	}

	static void SetupHDR()
	{
		var monitors = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		var currentMonitor = monitors?.OpenSubKey(monitors?.GetSubKeyNames().Last());
		bool hdrEnabled = currentMonitor?.GetValue("AdvancedColorEnabled")?.ToString() == "1";

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
		shaderData.Exposure = hdrEnabled ? float.Parse(exposure ?? "1000") / 1000f : 1;

		var devices = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
		var currentDevice = devices?.OpenSubKey(devices?.GetSubKeyNames().First());
		var nVidiaContrast = (int)(currentDevice?.OpenSubKey("Color").GetValue("3538950") ?? 100);
		shaderData.Gamma = hdrEnabled ? 1 - (120 - nVidiaContrast) * 0.015 : 1;
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mousePos; 
		mousePos = e.Location;

		if (e.Button == MouseButtons.None && e.Delta == 0)
			FindNearestMesh();

		if (e.Button == MouseButtons.Right)
			meshRot -= d / 2.75;

		if (e.Button == MouseButtons.Middle)
			offset.xy += d * offset.z * 0.0004;

		if (e.Button == MouseButtons.XButton1)
			lightRot -= d / 5;

		if (e.Delta != 0)
			offset.z -= e.Delta / 1500f;

		UpdateShaderData();
	}

	// View matrix is identity, so we can just ignore it. Projection matrix is calculated in SwapChainManager.ProcessResize()
	public static void UpdateShaderData()
	{
		M.World = 
			Matrix.RotationY(toRadians(meshRot.x)) *		// Rotation around Y axis is made by mouse movement on X axis, therefore it is "rotation.X"
			Matrix.Translation(offset.x, -offset.y, 0) *	// Mesh should rotate around camera offset position
			Matrix.RotationX(toRadians(meshRot.y)) * 
			Matrix.Translation(0, 0, offset.z);

		shaderData.LightPosition = new (cos(lightRot.y) * sin(lightRot.x), -sin(lightRot.y), cos(lightRot.y) * cos(lightRot.x));
		shaderData.Distance = Camera.offset.z;
	}

	public static void ProjectVertices()
	{
		int w = form.ClientSize.Width, h = form.ClientSize.Height;
		Meshes.ForEach(m =>	m.projected = [..m.vertices.Select(v => SharpDX.Vector3.Project(v.Position, 0, 0, w, h, 0, 1, M.World * M.Projection))]);
	}

	public static void FindNearestMesh()
	{
		float minZ = 1; nearestMesh = null;
		Meshes.ForEach(m => m.projected.ForEach(v => {
			if (vec2.DistanceSquared(Camera.mousePos, v.xy) < 100 && v.z < minZ) 
			{
				minZ = v.z; 
				nearestMesh = m;
			}
		}));
	}
}