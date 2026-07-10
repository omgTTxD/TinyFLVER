using Microsoft.Win32;

class Camera
{
	public static vec3 offset = new(0, 1.13, 2.5);
	public static vec2 mouse;

	static vec2 meshRotation = new(-16.4, -12);
	static vec2 lightRotation = new(-20, 20);

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;

		form.LightAtCamera.CheckedChanged += (s, e) => UpdateShaderData();
		form.ResizeEnd += (s, e) => UpdateShaderData();
		
		UpdateShaderData();
		SetupHDR();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mouse;
		mouse = e.Location;
		
		if (e.Button == MouseButtons.None && e.Delta == 0) 
			return;

		if (e.Button == MouseButtons.Right)				
			meshRotation -= d / 2.75;

		if (e.Button == MouseButtons.Middle)			
			offset.xy += d * offset.z * 0.0006;

		if (e.Button == MouseButtons.XButton1)			
			lightRotation -= d / 5;

		if (e.Delta != 0)								
			offset.z -= e.Delta / 1500f;
		
		UpdateShaderData();
		Renderer.Render();
	}

	public static void UpdateShaderData() 
	{
		M.Model = Matrix4x4.CreateRotationY(toRadians(meshRotation.x))		// Rotation around Y axis is made by mouse movement on X axis, so it is "mesh.x"
			  * Matrix4x4.CreateTranslation(offset.x, -offset.y, 0)			// Mesh should rotate around camera offset position instead of (0, 0, 0)
			  * Matrix4x4.CreateRotationX(toRadians(meshRotation.y))
			  * Matrix4x4.CreateTranslation(0, 0, offset.z);
		
		var proj = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(0.5f, (float)w / h, 0.01f, 100);

		// View matrix is identity, since camera is at (0, 0, 0) of camera space, looking at +Z
		M.MVP = M.Model * proj;

		shaderData.LightPosition = new(
			cos(lightRotation.y) * sin(lightRotation.x), 
		   -sin(lightRotation.y), 
			cos(lightRotation.y) * cos(lightRotation.x)
		);

		if (!form.LightAtCamera.Checked) {
			Matrix4x4.Invert(M.Model, out var inversed);
			shaderData.LightPosition = Vector3.TransformNormal(shaderData.LightPosition, Matrix4x4.Transpose(inversed));
		}
	}

	static void SetupHDR()
	{
		// HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore
		var monitors = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		var currentMonitor = monitors?.OpenSubKey(monitors?.GetSubKeyNames().First());
		bool hdrEnabled = currentMonitor?.GetValue("AdvancedColorEnabled")?.ToString() == "1";

		if (!hdrEnabled)
		{
			shaderData.Exposure = shaderData.Gamma = 1;
			return;
		}

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
		shaderData.Exposure = float.Parse(exposure ?? "1000") / 1000f;

		// HKEY_CURRENT_USER\SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices
		var devices = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
		var currentDevice = devices?.OpenSubKey(devices?.GetSubKeyNames().Last());
		
		// nVidia contrast is gamma. I have it set to 75% most of the time. 50% is for true HDR sources (mostly, films). 
		// 100% is for SDR video/games. In terms of registry values, 100% is 120, 75% is 110, and 50% is 100. 
		var nVidiaContrast = (int)(currentDevice?.OpenSubKey("Color").GetValue("3538950") ?? 100);
		shaderData.Gamma = 1 - (120 - nVidiaContrast) * 0.025;
	}
}