class Camera 
{
	public static void RecalculateMatrices()
	{
		cb.ViewProjection = RotationYX(camera) * Matrix4x4.CreateTranslation(offset) * PerspectiveLH(0.5f, 0.01f, 100);
		cb.CameraPos = Vector3.Transform(-offset, Invert(RotationYX(camera)));
		cb.LightPos = Vector3.Transform(new(0, 0, -light.z), Matrix4x4.CreateFromYawPitchRoll(light.x, light.y, 0)) + new Vector3(0, 1.4f, 0);
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = (e.Location - mouseCrd) / 100;
		mouseCrd = e.Location;

		if (e.Button == MouseButtons.None && e.Delta == 0)
			return;

		switch (e.Button) {
			case MouseButtons.Right:	camera -= d; break;
			case MouseButtons.Middle:	offset.xy += d * new vec2(1, -1) * 0.1 * offset.z;  break;
			case MouseButtons.XButton1: light.xy -= d; break;
			case MouseButtons.XButton2: light.z += d.x; light.z = Math.Clamp(light.z, 0.01f, 1.25f); break;
			case MouseButtons.None:		offset.z -= e.Delta / 5000f * Math.Max(offset.z, 0.4f); break;
		}
	
		RecalculateMatrices();
		Renderer.Render();
	}

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;
	}

	public static vec3 offset = new(0, -1.2, 2);
	public static vec2 camera = new(-0.5, -0.5);
	public static vec3 light = new(0.1, 0.6, 1);
	static vec2 mouseCrd;
}