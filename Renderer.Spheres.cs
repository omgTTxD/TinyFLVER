class Spheres
{
	static List<int> indices = [];
	public static Buffer indexBuffer;
	public static VertexBufferBinding vertexBufferBinding;

	public static void Initialize()
	{
		List<Vertex> vertices = [];

		//if (Random.Shared.NextInt64(0, 100) < 20)
		{
			int k = 60;
			for (int i = 0; i <= k; i++)
			{
				for (int j = 0; j < k; j++)
				{
					double x = 2 * Math.PI / k * j, y = Math.PI / k * i;
					vec3 N = new(Math.Sin(y) * Math.Cos(x), Math.Cos(y), Math.Sin(y) * Math.Sin(x));
					vertices.Add(new() { Position = (N / 10), Normal = N });
					int idx = i * k + j;
					indices.AddRange([idx, idx + k, idx + 1, idx + 1 + k]);
				}
			}
		}
		
		/*(vec3 N, vec3 T, vec3 B)[] faces =
		[
			(new(1, 0, 0),  new(0, 0, 1), new(0, -1, 0)),
			(new(0, 0, 1),  new(1, 0, 0),  new(0, 1, 0)),
			(new(0, 1, 0),  new(1, 0, 0),  new(0, 0, -1)),
			(new(-1, 0, 0), new(0, 0, 1),  new(0, 1, 0)),
			(new(0, 0, -1), new(1, 0, 0), new(0, 1, 0)),
			(new(0, -1, 0), new(1, 0, 0),  new(0, 0, 1)),
		];

		foreach (var (N, T, B) in faces)
		{
			int idx = vertices.Count;
			vertices.Add(new() { Position = (N + T + B) / 10, Normal = N });
			vertices.Add(new() { Position = (N + T - B) / 10, Normal = N });
			vertices.Add(new() { Position = (N - T + B) / 10, Normal = N });
			vertices.Add(new() { Position = (N - T - B) / 10, Normal = N });
			indices.AddRange([idx, idx + 1, idx + 2, idx + 3]);
		}*/


		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [..indices]);
		vertexBufferBinding = new VertexBufferBinding(Buffer.Create(d, BindFlags.VertexBuffer, [.. vertices]), SharpDX.Utilities.SizeOf<Vertex>(), 0);

		form.RenderSpheres.CheckedChanged += (s, e) =>
		{
			form.dgMeshes.ClearSelection();
			form.dgMeshes.Visible = !form.RenderSpheres.Checked;

			form.RecalculateTangents.Enabled = !form.RenderSpheres.Checked;

			if (form.RenderSpheres.Checked)
			{
				if (form.comboBoxShaders.Text == "PS.Normals.hlsl")
					form.comboBoxShaders.SelectedIndex++;

				form.comboBoxShaders.Items.Remove("PS.Normals.hlsl");
			}
			else
				form.comboBoxShaders.Items.Insert(0, "PS.Normals.hlsl");

			form.comboBoxShaders.Focus();

			if (form.RenderSpheres.Checked)
			{
				c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
				c.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
			}
		};
	}

	public static void Render()
	{
		c.PixelShader.SetShaderResource(7, null);
		c.DrawIndexedInstanced(indices.Count, 25, 0, 0, 0);
	}
	
	public static void RenderLightSource() 
	{
		cb.FormatID = 0;
		//c.PixelShader.SetShaderResource(7, null); 
		c.PixelShader.Set(Shaders.WhitePS);
		c.VertexShader.Set(Shaders.VS);

		cb.ViewProjection = Matrix4x4.CreateScale(0.1f) * Matrix4x4.CreateTranslation(cb.LightPos) * cb.ViewProjection;
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
		c.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
		c.DrawIndexed(indices.Count, 0, 0);
		
		Camera.RecalculateMatrices();
		Shaders.SetMain();
	}
}