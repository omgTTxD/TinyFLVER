using System.Runtime.InteropServices;

class SpheresRenderer
{
	static List<int> indices = [];
	static Buffer indexBuffer;
	static VertexBufferBinding vertexBuffer;
	static List<Vertex> originalVertices = [];

	public static void Initialize()
	{
		for (int i = 0; i <= 30; i++)
		{
			for (int j = 0; j < 30; j++)
			{
				float y = i * 6;
				float x = j * 12;
				
				vec3 n = new(sin(y) * cos(x), cos(y), sin(y) * sin(x));
				originalVertices.Add(new() { Position = new vec3(-0.45, 1.58, 0) + n / 10, Normal = n,  });

				int topLeft = i * 30 + j;
				int bottomLeft = topLeft + 30;

				indices.Add(topLeft); indices.Add(bottomLeft); indices.Add(topLeft + 1);
				indices.Add(topLeft + 1); indices.Add(bottomLeft); indices.Add(bottomLeft + 1);
			}
		}

		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [..indices]);

		form.RenderSpheres.CheckedChanged += (s, e) => {
			form.dgMeshes.ClearSelection();
			form.dgMeshes.Visible = !form.RenderSpheres.Checked;
			ShaderData.SetFormatId();
			form.comboBoxShaders.Focus();
		};
	}

	public static void Render()
	{
		
		Renderer.SetAndClearRT();
		Shaders.SetMain();
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				List<Vertex> vertices = [..originalVertices]; 
				foreach (ref var v in CollectionsMarshal.AsSpan(vertices))
				{
					v.Position.x += 0.24f * i;
					v.Position.y -= 0.23f * j;		  
					v.UV = new(0.25f * i, 0.25f * j, 0);
				}

				vertexBuffer = new(Buffer.Create(d, BindFlags.VertexBuffer, [..vertices]), Marshal.SizeOf<Vertex>(), 0);
				c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
				c.DrawIndexed(indices.Count, 0, 0);
			}
		}
	}
}