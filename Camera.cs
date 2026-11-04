class Camera
{
	public static vec3 offset = new(0, -1.2, 2);
	public static vec2 camera = new(-0.5, -0.5);
	public static vec3 light = new(0.1, 0.6, 1);
	static vec2 mouseCrd;

	public static void Initialize()
	{
		form.MouseWheel += ProcessMouseInput;
		form.MouseMove += ProcessMouseInput;
	}

	static void ProcessMouseInput(object sender, MouseEventArgs e)
	{
		vec2 d = (e.Location - mouseCrd) / 150;
		mouseCrd = e.Location;

		if (e.Button == MouseButtons.None && e.Delta == 0)
			return;

		switch (e.Button) {
			case MouseButtons.Right:	camera += d; break;
			case MouseButtons.Middle:	offset.xy += d * new vec2(1, -1) * 0.2 * offset.z;  break;
			case MouseButtons.XButton1: light.xy += d; break;
			case MouseButtons.XButton2: light.z += d.x; break;
			case MouseButtons.None:		offset.z -= e.Delta / 5000f * Math.Max(offset.z, 0.4f); break;
		}
	
		RecalculateMatrices();
		Renderer.Render();
	}

	public static void RecalculateMatrices()
	{
		var View = RotationYX(camera) * Matrix4x4.CreateTranslation(offset);
		cb.ViewProjection = View * PerspectiveLH(0.7f, 0.01f, 100);
		cb.CameraPos = Vector3.Transform(new(), Invert(View));

		cb.LightPos = Vector4.Transform(new Vector4(0, 0, -light.z, 1), 
			Matrix4x4.CreateFromYawPitchRoll(-light.x, light.y, 0) *
			Matrix4x4.CreateTranslation(0, 1, 0));

		cb.LightViewProj = LookAtLH(cb.LightPos) * Matrix4x4.CreateOrthographicLeftHanded(2, 2, 0.1f, 5);
	}
}