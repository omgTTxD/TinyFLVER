struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public vec3 Normal	 = v.Normal;
	public vec4 Tangent  = v.Tangents.FirstOrDefault();
	public vec3 UV = v.UVs[0];//.FirstOrDefault();

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32_Float, 0),
		new("NORMAL",	0, Format.R32G32B32_Float, 0),
		new("TANGENT",	0, Format.R32G32B32A32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32B32_Float, 0),
	];
}

public class Mesh(FLVER2.Mesh m)
{
	// Getters and setters are needed for DataGrids to be editable
	public string materialName { get => material.Name; set => material.Name = value; }
	public string materialMTD { get => Path.GetFileName(material.MTD); set => material.MTD = value; }
	public bool Hidden { get; set; }

	public FLVER2.Mesh mesh = m;
	public FLVER2.Material material = flver.Materials[m.MaterialIndex];

	VertexBufferBinding vertexBuffer = new(Buffer.Create(d, BindFlags.VertexBuffer, [.. m.Vertices.Select(v => new Vertex(v))]), SharpDX.Utilities.SizeOf<Vertex>(), 0);
	Buffer indexBuffer = m.FaceSets[0].Indices.Count > 0 ? Buffer.Create(d, BindFlags.IndexBuffer, [.. m.FaceSets[0].Indices]) : null;
	int indexCount = m.FaceSets[0].Indices.Count;

	public SRV diffuse, normal, SSS, metalness, reflectance, specular;

	public void Render()
	{
		c.PixelShader.SetShaderResources(0, [diffuse, normal, metalness, reflectance, specular]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		cb.MeshID = Meshes.IndexOf(this) + 1;
		cb.IsSkin = material.Name.IsSkin() ? 1 : 0;
		cb.IsMetal = material.IsMetal() ? 1 : 0;
		ShaderData.Sync();

		c.DrawIndexed(indexCount, 0, 0);
	}

	public static void Render(List<Mesh> meshes) => meshes.ForEach(m => m?.Render());
}