public struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public vec3 Normal = v.Normal;
	public vec2 UV = new(v.UVs.FirstOrDefault().X, v.UVs.FirstOrDefault().Y);
	public vec4 Tangent = v.Tangents.FirstOrDefault();

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

	public SRV diffuse, normal, SSS, metalness, specularDSR;

	public int meshMaterialIndex { get => mesh.MaterialIndex; set { mesh.MaterialIndex = value; } }
	public int materialIndex { get => material.Index; set { material.Index = value; } }
	public string materialName { get => material.Name; set { material.Name = value; } }
	public string materialMTD { get => material.MTD; set { material.MTD = value; } }

	public List<Texture> textureList;

	public bool Hidden { get; set; }
	public bool Selected;

	public List<Vertex> projectedVertices = [];

	public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];
		vertices = [.. m.Vertices.Select(v => new Vertex(v))];

		indices = [.. m.FaceSets[0].Indices];
		textureList = [.. material.Textures.Select(t => new Texture(t))];

		if (indices.Length == 0 || vertices.Count == 0) return;

		binding = new(Buffer.Create(d, BindFlags.VertexBuffer, vertices.ToArray()), SharpDX.Utilities.SizeOf<Vertex>(), 0);
		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, indices);
	}

	public void SetBuffers()
	{
		c.PixelShader.SetShaderResources(0, [diffuse, normal, SSS, metalness, specularDSR]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, binding);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
	}

	public static void Render(Mesh m)
	{
		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(Shaders.PS);
		m.SetBuffers();
		c.DrawIndexed(m.indices.Length, 0, 0);
	}
}