public struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public vec3 Normal = v.Normal;
	public vec2 UV = ((vec3)v.UVs[0]).xy;

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32_Float, 0),
		new("NORMAL", 0, Format.R32G32B32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32_Float, 0)
	];
}

public class Mesh
{
	public FLVER2.Mesh mesh;
	public FLVER2.Material material;
	VertexBufferBinding vertexBuffer;
	Buffer indexBuffer;
	public int indexCount;

	public SRV diffuse, normal, SSS, metalness, specularDSR;

	// Getters and setters are needed for DataGrids to be editable
	public string materialName		{ get => material.Name;			set => material.Name = value; } 
	public string materialMTD		{ get => material.MTD;			set => material.MTD = value; } 
	public bool Hidden				{ get;							set; }
	
	public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];

		Vertex[] vertices = [.. m.Vertices.ConvertAll<Vertex>(v => new(v))];
		int[] indices = [.. m.FaceSets[0].Indices];
		indexCount = m.FaceSets[0].Indices.Count;
		
		if (indexCount == 0 || m.Vertices.Count == 0) 
			return;

		indexBuffer = Buffer.Create(dev, BindFlags.IndexBuffer, indices);
		vertexBuffer = new(Buffer.Create(dev, BindFlags.VertexBuffer, vertices), Utilities.SizeOf<Vertex>(), 0);
		
		Textures.Reload(this);
	}

	public void SetBuffers()
	{
		c.PixelShader.SetShaderResources(0, [Textures.cubeMap, diffuse, normal, metalness, SSS, specularDSR]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
	}

	public static void Render(Mesh m)
	{
		m.SetBuffers();
		c.DrawIndexed(m.indexCount, 0, 0);
	}
}