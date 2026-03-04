using System.Numerics;
using System.Runtime.InteropServices;

namespace TinyFLVER;

public class ShaderSpheres
{
	static int[] indices;
	static Buffer indexBuffer;
	static List<Vertex> verts = [];
	static Vertex[] vertices;
	static VertexBufferBinding vertexBuffer;
	static ShaderResourceView spheresSRV;

	struct SphereData
	{
		public float Roughness;
		public float Metallic;
	}

	static SphereData data;

	public static void Initialize()
	{
		form.RenderSpheres.CheckedChanged += CheckedChanged;

		int stacks = 32, slices = 32;

		for (int i = 0; i <= stacks; i++)
			for (int j = 0; j <= slices; j++)
			{
				float theta = i * Pi / stacks;
				float phi = j * 2 * Pi / slices;
				Vector3 n = new(
					Sin(theta) * Cos(phi),
					Cos(theta),
					Sin(theta) * Sin(phi)
				);
				verts.Add(new Vertex
				{
					Position = new Vector3(-1f, 2.1f, 0) + n / 5,
					Normal = n,
					UV = new(j / (float)slices, i / (float)stacks, 0)
				});
			}

		var idx = new List<int>();
		int w = slices + 1;
		for (int i = 0; i < stacks; i++)
			for (int j = 0; j < slices; j++)
			{
				int tl = i * w + j;
				int tr = tl + 1;
				int bl = tl + w;
				int br = bl + 1;

				idx.Add(tl); idx.Add(bl); idx.Add(tr);
				idx.Add(tr); idx.Add(bl); idx.Add(br);
			}

		vertices = verts.ToArray();
		indices = idx.ToArray();
		vertexBuffer = new(Buffer.Create(dev, BindFlags.VertexBuffer, vertices), Marshal.SizeOf<Vertex>(), 0);
		indexBuffer = Buffer.Create(dev, BindFlags.IndexBuffer, indices);
		Shaders.CompileMainShader();
		spheresSRV = new(dev, Textures.CreateDefaultTextures([255, 255, 255, 255]));
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		PS = new(dev, ShaderBytecode.CompileFromFile("Spheres.hlsl", "PS", "ps_5_0"));

		psBuffer = new(dev, new(16, BindFlags.ConstantBuffer, ResourceUsage.Default));
		c.PixelShader.SetConstantBuffer(1, psBuffer);
		data = new SphereData();
	}

	static Buffer psBuffer;
	public static PixelShader PS;

	public static void CheckedChanged(Object s, EventArgs e) {
		if (form.RenderSpheres.Checked)
		{
			Camera.distance = 3.5f;
			Camera.UpdateData();
			form.comboBoxShaders.Enabled = false;
		}
		else
		{
			Camera.distance = 1.75f;
			vsData.Offset = new(0);
			Camera.UpdateData();
			form.comboBoxShaders.Enabled = true;
		}
	}

	public static void Render()
	{
		c.PixelShader.Set(PS);
		c.VertexShader.Set(Shaders.VS); 
		
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);

		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				vsData.Offset = new Vector2(i * 0.5f, -j * 0.5f);
				Shaders.UpdateBuffers();

				data.Roughness = i == 0 ? 0.05f : 0.25f * i;
				data.Metallic = 0.25f * j;
				c.UpdateSubresource(ref data, psBuffer);

				c.DrawIndexed(indices.Length, 0, 0);
			}
		}
	}
}