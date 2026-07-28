using System.Runtime.InteropServices;

public struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = new(v.Position.X, v.Position.Y, v.Position.Z);
	public vec3 Normal = new(v.Normal.X, v.Normal.Y, v.Normal.Z);
	public vec4 Tangent = v.Tangents[0];
	public vec3 UV = v.UVs[0];

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
	public string materialMTD { get => material.MTD; set => material.MTD = value; }
	public bool Hidden { get; set; }
	
	public FLVER2.Mesh mesh;
	public FLVER2.Material material;

	VertexBufferBinding bufferBinding;
	Buffer indexBuffer;
	int indexCount;

	public SRV diffuse, normal, SSS, metalness, reflectance, specularDSR;
	public static SRV cubeMap = CreateSRV(AppContext.BaseDirectory + "Textures\\CubeMaps\\2.dds");

	public SRV details = CreateSRV(AppContext.BaseDirectory + "Textures\\AAT100_Skin_01_n.dds");
	public Vertex[] Vs;
	public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];
		
		Vertex[] vertices = [.. m.Vertices.ConvertAll<Vertex>(v => new(v))];
		indexCount = m.FaceSets[0].Indices.Count;

		/*var faces = mesh.GetFaces();
		vertices = new Vertex[faces.Count * 3];

		for (int i = 0; i < faces.Count; i++)
		{
			vertices[i * 3] = new(faces[i][1]); 
			vertices[i * 3 + 1] = new(faces[i][2]);
			vertices[i * 3 + 2] = new(faces[i][0]);
		}*/

		if (indexCount == 0 || m.Vertices.Count == 0) 
			return;

		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, [.. m.FaceSets[0].Indices]);
		var vertexBuffer = Buffer.Create(d, BindFlags.VertexBuffer, vertices);
		bufferBinding = new(vertexBuffer, Marshal.SizeOf<Vertex>(), 0);
		Textures.Reload(this);
	}

	public void Render()
	{
		c.PixelShader.SetShaderResources(0, [cubeMap, diffuse, normal, metalness, SSS, reflectance, specularDSR, details]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, bufferBinding);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		shaderData.MeshID = Meshes.IndexOf(this) + 1;
		shaderData.UseDetails = material.Name.ContainsAny("Skin", "Body", "Face") ? 1 : 0;

		c.DrawIndexed(indexCount, 0, 0);
	//	c.Draw(vertices.Length, 0);
	}

	public static void Render(List<Mesh> meshes) => meshes.ForEach(m => m.Render());
	public static void Render(Mesh mesh) => mesh.Render();
}