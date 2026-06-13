using Microsoft.Win32;

class Camera
{
	public static vec3 offset = new(0, 1.15, 2.5);
	public static vec2 meshRot	= new(-10, -10);
	public static vec2 lightRot = new(-20, 30);
	public static vec2 mouse;

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;
		form.MouseUp += (s, e) => Mesh.ProjectVertices();
		form.LightAtCamera.CheckedChanged += (s, e) => UpdateShaderData();
		Mesh.ProjectVertices();
		SetupHDR();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mouse;
		mouse = e.Location;

		if (e.Button == MouseButtons.None && e.Delta == 0) 
			return;
		if (e.Button == MouseButtons.Right)				
			meshRot -= d / 2.75;
		if (e.Button == MouseButtons.Middle)			
			offset.xy += d * offset.z * 0.0006;
		if (e.Button == MouseButtons.XButton1)			
			lightRot -= d / 5;
		if (e.Delta != 0)								
			offset.z -= e.Delta / 1500f;
		
		UpdateShaderData();
		SwapChainManager.Render();
	}

	public static void UpdateShaderData() 
	{
		M.Model = Matrix4x4.CreateRotationY(toRadians(meshRot.x))				// Rotation around Y axis is made by mouse movement on X axis, so it is "mesh.x"
			  * Matrix4x4.CreateTranslation(offset.x, -offset.y, 0)				// Mesh should rotate around camera offset position instead of (0, 0, 0)
			  * Matrix4x4.CreateRotationX(toRadians(meshRot.y))
			  * Matrix4x4.CreateTranslation(0, 0, offset.z);

		// View matrix is identity
		M.MVP = M.Model * Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(0.5f, (float)w / h, 0.01f, 100);
		shaderData.LightPosition = new(cos(lightRot.y) * sin(lightRot.x), -sin(lightRot.y), cos(lightRot.y) * cos(lightRot.x));

		if (!form.LightAtCamera.Checked) {
			Matrix4x4.Invert(M.Model, out var inversed);
			shaderData.LightPosition = Vector3.TransformNormal(shaderData.LightPosition, Matrix4x4.Transpose(inversed));
		}
	}

	static void SetupHDR()
	{
		var monitors = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		var currentMonitor = monitors?.OpenSubKey(monitors?.GetSubKeyNames().Last());
		bool hdrEnabled = currentMonitor?.GetValue("AdvancedColorEnabled")?.ToString() == "1";

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
		shaderData.Exposure = hdrEnabled ? float.Parse(exposure ?? "1000") / 1000f : 1;

		// nVidia contrast is gamma. I have it at 85%-100% at all times. 50% is for HDR movies only.
		var devices = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
		var currentDevice = devices?.OpenSubKey(devices?.GetSubKeyNames().First());
		var nVidiaContrast = (int)(currentDevice?.OpenSubKey("Color").GetValue("3538950") ?? 100);
		shaderData.Gamma = hdrEnabled ? 1 - (120 - nVidiaContrast) * 0.015f : 1;
	}
}