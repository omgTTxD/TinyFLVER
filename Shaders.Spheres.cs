using System.Runtime.InteropServices;

public class ShaderSpheres
{
	static int[] indices;
	static Buffer indexBuffer;
	static VertexBufferBinding vertexBuffer;

	public static PixelShader PS;
	public static VertexShader VS;

	struct SphereData
	{
		public float Roughness;
		public float Metallic;
		public Vector2 Offset;
	}

	static Buffer buffer;
	static SphereData sphereData;

	public static void Initialize()
	{
		int stacks = 32, slices = 32;
		List<Vertex> verts = [];
		for (int i = 0; i <= stacks; i++)
			for (int j = 0; j <= slices; j++)
			{
				float theta = i * 180 / stacks;
				float phi = j * 360 / slices;
				Vector3 n = new( 
					Sin(theta) * Cos(phi),
					Cos(theta),
					Sin(theta) * Sin(phi)
				);
				verts.Add(new Vertex
				{
					Position = new Vector3(-0.9f, 2.25f, 0) + n / 5,
					Normal = n,
					UV = new(j / (float)slices, i / (float)stacks, 0)
				});
			}

		List<int> idx = new List<int>();
		for (int i = 0; i < stacks; i++)
			for (int j = 0; j < slices; j++)
			{
				int tl = i * slices + j;
				int tr = tl + 1;
				int bl = tl + slices;
				int br = bl + 1;

				idx.Add(tl); idx.Add(bl); idx.Add(tr);
				idx.Add(tr); idx.Add(bl); idx.Add(br);
			}

		var vertices = verts.ToArray();
		indices = idx.ToArray();
		vertexBuffer = new(Buffer.Create(dev, BindFlags.VertexBuffer, vertices), Marshal.SizeOf<Vertex>(), 0);
		indexBuffer = Buffer.Create(dev, BindFlags.IndexBuffer, indices);

		Shaders.CompileMainShader();
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		PS = new(dev, ShaderBytecode.CompileFromFile("Spheres.hlsl", "PS", "ps_5_0"));
		VS = new(dev, ShaderBytecode.CompileFromFile("Spheres.hlsl", "VS", "vs_5_0"));

		buffer = new(dev, new(16, BindFlags.ConstantBuffer, ResourceUsage.Default));

		c.PixelShader.SetConstantBuffer(2, buffer);
		c.VertexShader.SetConstantBuffer(2, buffer);

		form.RenderSpheres.CheckedChanged += CheckedChanged;
	}

	public static void CheckedChanged(Object s, EventArgs e) {
		if (form.RenderSpheres.Checked)
		{
			Camera.cameraDistance = 5.5f;
			Camera.UpdateShaderData();
			form.comboBoxShaders.Enabled = false;
		}
		else
		{
			Camera.cameraDistance = 2.5f;
			Camera.UpdateShaderData();
			form.comboBoxShaders.Enabled = true;
		}
	}

	public static void Render()
	{
		c.PixelShader.Set(PS);
		c.VertexShader.Set(VS); 
		
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				sphereData.Offset = new Vector2(i * 0.5f, -j * 0.5f);
				sphereData.Roughness = i == 0 ? 0.04f : 0.25f * i;
				sphereData.Metallic = 0.25f * j;
				c.UpdateSubresource(ref sphereData, buffer);
				c.DrawIndexed(indices.Length, 0, 0);
			}
		}
	}
}