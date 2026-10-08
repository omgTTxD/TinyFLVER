class SpheresRenderer
{
	static List<int> indices = [];
	static Buffer indexBuffer;
	static vec3 Offset = new vec3(-0.45, 1.58, 0);

	public static void Initialize()
	{
		List<Vertex> vertices = [];
		int k = 30;
		for (int i = 0; i <= k; i++)
		{
			for (int j = 0; j < k; j++)
			{
				int x = j * 12, y = i * 6;
				vec3 N = new(sin(y) * cos(x), cos(y), sin(y) * sin(x));
				vertices.Add(new() { Position = Offset + N / 10, Normal = N, Tangent = new(1, 0, 0, 1)});
				int idx = i * k + j;
				indices.AddRange([idx, idx + k, idx + 1, idx + 1 + k]);
			}
		}

		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [..indices]);

		form.RenderSpheres.CheckedChanged += (s, e) =>
		{
			form.dgMeshes.ClearSelection();
			form.dgMeshes.Visible = !form.RenderSpheres.Checked;
			form.RecalculateTangents.Enabled = !form.RenderSpheres.Checked;
			form.RecalculateTangents.Checked = !form.RenderSpheres.Checked;

			if (form.RenderSpheres.Checked)
			{
				form.comboBoxShaders.Items.Remove("PS.Normals.hlsl");
				form.comboBoxShaders.SelectedIndex = 0;
			}
			else
				form.comboBoxShaders.Items.Insert(0, "PS.Normals.hlsl");

			form.comboBoxShaders.Focus();

			if (form.RenderSpheres.Checked)
			{
				Shaders.SetMain();
				cb.IsSkin = 0;
				cb.IsMetal = 0;
				c.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
				c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
				c.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(Buffer.Create(d, BindFlags.VertexBuffer, [.. vertices]), SharpDX.Utilities.SizeOf<Vertex>(), 0));
			}
		};
	}

	public static void Render()
	{
		c.DrawIndexedInstanced(indices.Count, 25, 0, 0, 0);
	}
}