using System.Runtime.InteropServices;

public class Mesh
{
	public List<Vertex> vertices = [];
	public VertexBufferBinding vertexBufferBinding;

	public ShaderResourceView diffuse;
	public ShaderResourceView normal;
	public ShaderResourceView SSS;
	public ShaderResourceView metalness;
	public ShaderResourceView specularDSR;

	public FLVER2.Mesh flverMesh;
	public FLVER2.Material material;

	public CheckBox cbSelected;
	public CheckBox cbHidden;

	public bool Transparent;

	public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
	{
		this.material = material;
		flverMesh = mesh;

		// Dark Souls Remastered have strange indices, which I don't know how to use. And since I don't want to 
		// have two versions of code, I stopped used indices in ER also.
		foreach (var face in flverMesh.GetFaces())
		{
			vertices.Add(new (face[0]));
			vertices.Add(new (face[1]));
			vertices.Add(new (face[2]));
		}
		
		if (vertices.Count != 0) 
			vertexBufferBinding = new (Buffer.Create(dev, BindFlags.VertexBuffer, vertices.ToArray()), Marshal.SizeOf<Vertex>(), 0);
	}

	public static void Render(Mesh m)
	{
		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(Shaders.PS);
		c.InputAssembler.SetVertexBuffers(0, m.vertexBufferBinding);

		c.PixelShader.SetShaderResources(0, m.diffuse ?? Textures.Light);
		c.PixelShader.SetShaderResource(1, m.normal ?? Textures.Gray);
		c.PixelShader.SetShaderResource(2, m.SSS ?? Textures.Black);
		c.PixelShader.SetShaderResource(3, m.metalness ?? Textures.Black);
		c.PixelShader.SetShaderResource(4, m.specularDSR ?? Textures.Gray);
		c.Draw(m.vertices.Count, 0);
	}

	public float CheckIntersection(Point mouse)
	{
		float minZ = 1;
		foreach (Vertex vertex in vertices)
		{
			// Just project point to screen again, to get exact screen coordinates. Very easy. Like all things should be.
			var v = SharpDX.Vector3.TransformCoordinate(new SharpDX.Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z), vsData.World * vsData.Proj);

			// Matrices transform vertices to [-1; 1] coordinates, so we need to multiply by Height and Width
			v.X = (v.X + 1) / 2 * form.ClientSize.Width;
			v.Y = (-v.Y + 1) / 2 * form.ClientSize.Height;

			// Because we want to select faces, not vertices, distance should be big enough. Then we find closest mesh to camera by comparing Z.
			if (System.Numerics.Vector2.Distance(new(mouse.X, mouse.Y), new(v.X, v.Y)) < 30 && v.Z < minZ)
				minZ = v.Z;
		}

		return minZ;
	}
}