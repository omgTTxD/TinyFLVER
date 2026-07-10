using System.Runtime.InteropServices;

public class ShaderSpheres
{
	static int[] indices;
	static Buffer indexBuffer;
	static VertexBufferBinding vertexBuffer;
	static List<Vertex> vertices = [];
	public static PixelShader PS;
	public static VertexShader VS;

	static List<Vertex> originalVertices = [];

	static Buffer buffer;

	public static void Initialize()
	{
		int stacks = 64, slices = 64;

		for (int i = 0; i <= stacks; i++)
			for (int j = 0; j < slices; j++)
			{
				float y = i * 180 / stacks;
				float x = j * 360 / slices;
				Vector3 n = new(sin(y) * cos(x), cos(y), sin(y) * sin(x));
				vertices.Add(new() { Position = new vec3(-0.45, 1.58, 0) + n / 10, Normal = n });
			}
		originalVertices = [..vertices];

		List<int> idx = [];
		for (int i = 0; i < stacks ; i++)
			for (int j = 0; j < slices; j++)
			{
				int topLeft = i * slices + j;
				int bottomLeft = topLeft + slices;

				idx.Add(topLeft); idx.Add(bottomLeft); idx.Add(topLeft + 1);
				idx.Add(topLeft + 1); idx.Add(bottomLeft); idx.Add(bottomLeft + 1);
			}

		indices = [..idx];
		indexBuffer = Buffer.Create(dev, BindFlags.IndexBuffer, indices);
		
		form.RenderSpheres.CheckedChanged += (s, e) => {
			form.dgMeshes.ClearSelection();
			form.dgMeshes.Visible = !form.RenderSpheres.Checked;
			if (!form.RenderSpheres.Checked)
				shaderData.FormatID = Flver.game.StartsWith("Dark Souls") ? 1 : Flver.game.StartsWith("BloodBorne") ? 2 : 0;
			else
				shaderData.FormatID = 3;
		};

		//form.RenderSpheres.Checked = true;
	}

	static Buffer vertexBufferRes;


	public static void Render()
	{
		Renderer.SetRenderTargets(Renderer.msaaRT, Renderer.depth);
		c.PixelShader.Set(Shaders.PS);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				vertices = [..originalVertices]; 
				foreach (ref var v in CollectionsMarshal.AsSpan(vertices))
				{
					v.Position.x += 0.24f * i;
					v.Position.y -= 0.23f * j;
					v.UV.x = 0.25f * i;
					v.UV.y = 0.25f * j;
				}

				vertexBuffer = new(Buffer.Create(dev, BindFlags.VertexBuffer, [..vertices]), Marshal.SizeOf<Vertex>(), 0);
				c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
				c.DrawIndexed(indices.Length, 0, 0);
			}
		}
	}
}