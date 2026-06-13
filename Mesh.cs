using System.ComponentModel;

public class Mesh
{
	public FLVER2.Mesh mesh;
	public FLVER2.Material material;
	public List<Vertex> vertices;
	public int[] indices;
	public VertexBufferBinding binding;
	Buffer indexBuffer;

	public SRV diffuse, normal, SSS, metalness, specularDSR;

	public int meshMaterialIndex => mesh.MaterialIndex;
	public int materialIndex => material.Index;
	public string materialName => material.Name;
	public string materialMTD => material.MTD;
	public bool Hidden { get; set; }

	public List<vec3> projectedVertices = [];

	public Mesh(FLVER2.Mesh m)
	{
		mesh = m;
		material = flver.Materials[m.MaterialIndex];
		vertices = [.. m.Vertices.Select(v => new Vertex(v))];
		indices = [.. m.FaceSets[0].Indices];

		if (indices.Length == 0 || vertices.Count == 0) return;
		binding = new(Buffer.Create(d, BindFlags.VertexBuffer, vertices.ToArray()), SharpDX.Utilities.SizeOf<Vertex>(), 0);
		indexBuffer = Buffer.Create(d, BindFlags.IndexBuffer, indices);
	
		ReloadTextures();
	}

	public void SetBuffers()
	{
		c.PixelShader.SetShaderResources(0, [diffuse, normal, metalness, SSS, specularDSR]);
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

	public void ReloadTextures()
	{
		diffuse = Textures.FindTexture(material.Textures, "AlbedoMap", "_a", "_d");
		normal = Textures.FindTexture(material.Textures, "NormalMap", "_n");
		metalness = Textures.FindTexture(material.Textures, "MetallicMap", "_m") ;
		SSS = Textures.FindTexture(material.Textures, "_3m");
		specularDSR = Textures.FindTexture(material.Textures, "_s");
		GC.Collect();
	}

	public static void ReloadAllTextures() { 
		Meshes.ToList().ForEach(m => m.ReloadTextures());
		form.dataGridTextures.Invalidate();
	}

	public static void ProjectVertices()
	{
		Parallel.ForEach(Meshes, m => m.projectedVertices = [.. m.vertices.Select(v => v.Position.project() )]);
	}

	public static Mesh PickNearest() => Meshes.SelectMany(m => m.projectedVertices
		.Where(v => Vector2.DistanceSquared(Camera.mouse, v.xy) < 1500 / Camera.offset.z)
		.Select(v => (Mesh: m, v.z))).OrderBy(x => x.Mesh.Hidden).ThenBy(x => x.z).FirstOrDefault().Mesh;
	
}