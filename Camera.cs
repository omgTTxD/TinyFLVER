using Microsoft.Win32;
using Newtonsoft.Json;

struct Matrices
{
	public Matrix4x4 World;
	public Matrix4x4 WVP;
}

class Camera
{
	public static vec3 offset = new(0, -0.23, 2.1);
	static vec2 mouseCrd;

	public static vec2 meshRotation = new(-16.4, -12);
	public static vec2 lightRotation = new(-40, 10);

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;
		form.ResizeEnd += (s, e) => UpdateShaderData();
		UpdateShaderData();
		SetupHDR();

		if (!File.Exists(cameraSettings))
			File.Create(cameraSettings).Dispose();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mouseCrd;
		mouseCrd = e.Location;
		
		if (e.Button == MouseButtons.None && e.Delta == 0) 
			return;

		if (e.Button == MouseButtons.Right)
			meshRotation -= d / 3.5;
		
		if (e.Button == MouseButtons.Middle)
			offset.xy += new vec2(d.x, -d.y) * offset.z * 0.0008; 

		if (e.Button == MouseButtons.XButton1)
			lightRotation -= d / 3.5;

		if (e.Delta != 0)								
			offset.z -= e.Delta / 3000f * offset.z;
		
		UpdateShaderData(); 
		Renderer.Render();
	}

	public static void UpdateShaderData()
	{
		M.World = Matrix4x4.CreateTranslation(0, -1, 0) *
			Matrix4x4.CreateRotationY(toRadians(meshRotation.x)) *
			Matrix4x4.CreateRotationX(toRadians(meshRotation.y)) *
			Matrix4x4.CreateTranslation(offset);

		M.WVP = M.World * Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(0.5f, (float)w / h, 0.01f, 100);

		shaderData.LightPosition = new vec3(
			cos(lightRotation.y) * sin(lightRotation.x),
			-sin(lightRotation.y),
			cos(lightRotation.y) * cos(lightRotation.x));

		ShaderData.Sync();
	}

	public static void SetupHDR()
	{
		shaderData.Exposure = shaderData.Gamma = 1;

		try {
			// HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore
			var monitors = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
			var currentMonitor = monitors.OpenSubKey(monitors.GetSubKeyNames().First());
			bool hdrEnabled = currentMonitor.GetValue("AdvancedColorEnabled")?.ToString() == "1";

			if (!hdrEnabled)
				return;

			// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
			var exposure = currentMonitor?.GetValue("SDRWhiteLevel")?.ToString();
			shaderData.Exposure = float.Parse(exposure ?? "1000") / 1000f * 1.25;

			// HKEY_CURRENT_USER\SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices
			var devices = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\NVIDIA Corporation\Global\NVTweak\Devices");
			var currentDevice = devices.OpenSubKey(devices.GetSubKeyNames().First());

			// nVidia contrast is similair to gamma. I have it set to 75% most of the time. 50% is for true HDR sources (mostly, films). 
			// 100% is for Youtube/SDR videos. In terms of registry values, 100% is 120, 75% is 110, and 50% is 100.
			var nVidiaContrast = (int)(currentDevice?.OpenSubKey("Color").GetValue("3538950") ?? 100);
			shaderData.Gamma = 1 - (120 - nVidiaContrast) * 0.015;
		}
		catch (Exception) {  }
	}

	class CameraSettings
	{
		public Vector3 offset;
		public vec2 meshRotation;
		public vec2 lightRotation;
	}

	static string cameraSettings = AppContext.BaseDirectory + "CameraSettings.txt";
	static Dictionary<string, CameraSettings> locations = [];

	public static void SaveCamera()
	{
		if (Flver.path is null)
			return;

		locations[Flver.path] = new CameraSettings() { offset = offset, meshRotation = meshRotation, lightRotation = lightRotation };
		File.WriteAllText(cameraSettings, JsonConvert.SerializeObject(locations));
	}

	public static void LoadCamera()
	{
		locations = JsonConvert.DeserializeObject<Dictionary<string, CameraSettings>>(File.ReadAllText(cameraSettings)) ?? [];
		if (locations.TryGetValue(Flver.path, out var settings))
		{
			offset = settings.offset;
			meshRotation = settings.meshRotation;
			lightRotation = settings.lightRotation;
			UpdateShaderData();
		}
	}
}