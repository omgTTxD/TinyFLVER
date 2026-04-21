public struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public Vector3 Normal = v.Normal;
	public Vector2 UV = new(v.UVs.FirstOrDefault().X, v.UVs.FirstOrDefault().Y);
	public Vector4 Tangent = v.Tangents.FirstOrDefault();

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32_Float, 0),
		new("NORMAL", 0, Format.R32G32B32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32_Float, 0),
		new("TANGENT", 0, Format.R32G32B32A32_Float, 0)
	];
}

public class Mesh
{
	public FLVER2.Mesh mesh;
	public FLVER2.Material material;
	public List<Vertex> vertices;
	public int[] indices;
	VertexBufferBinding binding;
	Buffer indexBuffer;
	PrimitiveTopology topology;

	public ShaderResourceView diffuse;
	public ShaderResourceView normal;
	public ShaderResourceView SSS;
	public ShaderResourceView metalness;
	public ShaderResourceView specularDSR;

	public int meshMaterialIndex { get => mesh.MaterialIndex; set { mesh.MaterialIndex = value; } }
	public int materialIndex { get => material.Index; set { material.Index = value; } }
	public string materialName { get => material.Name; set { material.Name = value; } }
	public string materialMTD { get => material.MTD; set { material.MTD = value; } }

	public List<Texture> textureList;

	public bool Hidden { get; set; }
	public bool Selected;

	public List<vec3> projected = [];

	public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];

		vertices = [.. m.Vertices.Select(v => new Vertex(v))];
		indices = [.. m.FaceSets[0].Indices];
		textureList = [.. material.Textures.Select(t => new Texture(t))];
		topology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		
		if (indices.Length == 0) return;
		
		binding = new(Buffer.Create(d, BindFlags.VertexBuffer, vertices.ToArray()), Marshal.SizeOf<Vertex>(), 0);
		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, indices);
	}

	public void SetBuffers()
	{
		c.InputAssembler.PrimitiveTopology = topology;
		c.InputAssembler.SetVertexBuffers(0, binding);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
	}

	public static void Render(Mesh m)
	{
		if (m.Hidden) return;
		m.SetBuffers();
		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(Shaders.PS);
		c.PixelShader.SetShaderResources(0, [m.diffuse, m.normal, m.SSS, m.metalness, m.specularDSR]);
		c.DrawIndexed(m.indices.Length, 0, 0);
	}
}