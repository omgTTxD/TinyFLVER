using Newtonsoft.Json;

class Camera
{
	public static vec3 offset = new(0, -0.23, 2.1);
	public static vec2 meshRotation = new(-16.4, -12);
	public static vec2 lightRotation = new(-40, 10);
	static vec2 mouseCrd;

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;

		CameraSettings.Initialize();
		UpdateShaderData();
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = e.Location - mouseCrd;
		mouseCrd = e.Location;
		
		if (e.Button == MouseButtons.None && e.Delta == 0) 
			return;

		if (e.Button == MouseButtons.Right)
			meshRotation -= d / 4;
		
		if (e.Button == MouseButtons.Middle)
			offset.xy += new vec2(d.x, -d.y) * 0.0008 * offset.z;

		if (e.Button == MouseButtons.XButton1)
			lightRotation += d / 4;

		if (e.Delta != 0)
			offset.z -= e.Delta / 5000f * Math.Max(offset.z, 0.4f);

		UpdateShaderData(); 
		Renderer.Render();
	}

	public static void UpdateShaderData()
	{
		M.Model = Matrix4x4.CreateTranslation(0, -1, 0)
		  * Matrix4x4.CreateRotationY(toRadians(meshRotation.x))
		  * Matrix4x4.CreateRotationX(toRadians(meshRotation.y)) 
		  * Matrix4x4.CreateTranslation(offset);

		M.MVP = M.Model * Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(0.7f, (float)w / h, 0.01f, 100);

		cb.LightPosition =  new vec3(
			-cos(lightRotation.y) * sin(lightRotation.x),
			sin(lightRotation.y),
			cos(lightRotation.y) * cos(lightRotation.x));
		
		//cb.LightPosition = Vector3.TransformNormal(cb.LightPosition, M.Model);
	}
}

class CameraSettings()
{
	static string settingsPath = AppContext.BaseDirectory + "CameraSettings.txt";
	static Dictionary<string, CameraSettings> dictionary = [];

	public vec3 cameraOffset = Camera.offset;
	public vec2 meshRotation = Camera.meshRotation;
	public vec2 lightRotation = Camera.lightRotation;

	public static void Initialize()
	{
		if (!File.Exists(settingsPath))
			File.Create(settingsPath).Dispose();

		dictionary = JsonConvert.DeserializeObject<Dictionary<string, CameraSettings>>(File.ReadAllText(settingsPath)) ?? [];
	}

	public static void Save()
	{
		if (Flver.path is not null)
			dictionary[Flver.path] = new CameraSettings();

		File.WriteAllText(settingsPath, JsonConvert.SerializeObject(dictionary, Formatting.Indented));
	}

	public static void Load()
	{
		if (dictionary.TryGetValue(Flver.path, out var s))
		{
			Camera.offset = s.cameraOffset;
			Camera.meshRotation = s.meshRotation;
			Camera.lightRotation = s.lightRotation;
			Camera.UpdateShaderData();
		}
	}
}