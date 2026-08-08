class SpheresRenderer
{
	static List<int> indices = [];
	static Buffer indexBuffer;

	static List<Vertex> originalVertices = [];

	public static void Initialize()
	{
		int k = 30;
		for (int i = 0; i <= k; i++)
		{
			for (int j = 0; j < k; j++)
			{
				int x = j * 12, y = i * 6;
				vec3 n = new(sin(y) * cos(x), cos(y), sin(y) * sin(x));
				originalVertices.Add(new() { Position = new vec3(-0.45, 1.58, 0) + n / 10, Normal = n });
				indices.AddRange([i * k + j, (i + 1) * k + j, i * k + (j + 1),  (i + 1) * k + (j + 1)]);
			}
		}

		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [..indices]);

		form.RenderSpheres.CheckedChanged += (s, e) => {
			form.dgMeshes.ClearSelection();
			form.dgMeshes.Visible = !form.RenderSpheres.Checked;
	
			form.RecalculateTangents.Enabled = !form.RenderSpheres.Checked;
			form.comboBoxShaders.Focus();
		};
	}

	unsafe public static void Render()
	{
		shaderData.IsSkin = 0;
		
		Shaders.SetMain();
		c.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		for (float i = 0; i <= 1; i += 0.25f)
		{
			for (float j = 0; j <= 1; j += 0.25f)
			{
				var vertices = originalVertices.ConvertAll<Vertex>(v => new () 
				{ 
					Position = v.Position + new vec3(i, -j, 0), 
					Normal = v.Normal, 
					UV = new(i, j) 
				}) ;
				c.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(Buffer.Create(d, BindFlags.VertexBuffer, [.. vertices]), sizeof(Vertex), 0));
				c.DrawIndexed(indices.Count, 0, 0);
			}
		}
	}
}