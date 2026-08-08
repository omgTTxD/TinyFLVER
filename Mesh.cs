struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public vec3 Normal = v.Normal;
	public vec4 Tangent = v.Tangents[0];
	public vec2 UV = v.UVs[0];

	public static implicit operator Vertex(FLVER.Vertex v) => new(v);

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32_Float, 0),
		new("NORMAL", 0, Format.R32G32B32_Float, 0),
		new("TANGENT", 0, Format.R32G32B32A32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32_Float, 0),
	];
}

public class Mesh
{
	// Getters and setters are needed for DataGrids to be editable
	public string materialName { get => material.Name; set => material.Name = value; }
	public string materialMTD { get => Path.GetFileName(material.MTD); set => material.MTD = value; }
	public bool Hidden { get; set; }
	
	public FLVER2.Mesh mesh;
	public FLVER2.Material material;

	VertexBufferBinding vertexBuffer;
	Buffer indexBuffer;
	int indexCount;

	public SRV diffuse, normal, SSS, metalness, reflectance, specularDSR;
	public static SRV cubeMap = Textures.LoadTexture(AppContext.BaseDirectory + "Textures\\CubeMaps\\2.dds");
	public SRV details = Textures.LoadTexture(AppContext.BaseDirectory + "Textures\\AAT100_Skin_01_n.dds");
	
	unsafe public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];
		indexCount = m.FaceSets[0].Indices.Count;

		if (indexCount== 0 || m.Vertices.Count == 0) 
			return;

		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [.. m.FaceSets[0].Indices]);
		vertexBuffer = new(Buffer.Create<Vertex>(d, BindFlags.VertexBuffer, [.. m.Vertices]), sizeof(Vertex), 0);
	
		Task.Run(() =>Textures.Reload(this));
	}

	public void Render()
	{
		c.PixelShader.SetShaderResources(0, [cubeMap, diffuse, normal, metalness, SSS, reflectance, specularDSR, details]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		shaderData.MeshID = Meshes.IndexOf(this) + 1;
		shaderData.IsSkin = material.Name.ContainsAny("Skin", "Body", "Face", "Head") ? 1 : 0;

		c.DrawIndexed(indexCount, 0, 0);
	}

	public static void Render(List<Mesh> meshes) => meshes.ForEach(m => { if (m is not null) m.Render(); });
	public static void Render(Mesh mesh) => mesh.Render();
}