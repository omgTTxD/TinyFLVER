using System.Runtime.InteropServices;

namespace TinyFLVER;

public struct Vertex(FLVER.Vertex v)
{
	public Vector3 Position = v.Position;
	public Vector3 Normal = v.Normal;
	public Vector3 UV = v.UVs[0];
}

public class Mesh
{
	public Vertex[] vertices;
	public VertexBufferBinding vertexBuffer;

	public ShaderResourceView diffuseBinding;
	public ShaderResourceView normalBinding;
	public ShaderResourceView sssBinding;
	public ShaderResourceView metalnessBinding;
	public FLVER2.Mesh flverMesh;
	public FLVER2.Material material;

	public CheckBox cbSelected;
	public CheckBox cbHidden;

	public bool Transparent;

	public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
	{
		this.material = material;
		flverMesh = mesh;

		var faces = flverMesh.GetFaces();
		vertices = new Vertex[faces.Count * 3];
		for (int i = 0; i < faces.Count; i++)
		{
			vertices[i * 3] = new (faces[i][0]);
			vertices[i * 3 + 1] = new (faces[i][1]);
			vertices[i * 3 + 2] = new (faces[i][2]);
		}
		
		if (vertices.Length != 0) 
			vertexBuffer = new (Buffer.Create(dev, BindFlags.VertexBuffer, vertices), Marshal.SizeOf<Vertex>(), 0);
	}

	public static void Render(Mesh m)
	{
		c.InputAssembler.SetVertexBuffers(0, m.vertexBuffer);
		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(Shaders.PS);
		c.PixelShader.SetShaderResources(0, m.diffuseBinding ?? Textures.defaultDiffuseSRV);
		c.PixelShader.SetShaderResource(1, m.normalBinding ?? Textures.defaultNormalSRV);
		c.PixelShader.SetShaderResource(2, m.sssBinding ?? Textures.defaultBlackSRV);
		c.PixelShader.SetShaderResource(3, m.metalnessBinding ?? Textures.defaultBlackSRV);
		c.Draw(m.vertices.Length, 0);
	}

	public float CheckIntersection(Point mouse)
	{
		float minZ = 1;
		foreach (Vertex vertex in vertices)
		{
			// Just project point to screen again, to get exact screen coordinates. Very easy. Like all things should be.
			var v = SharpDX.Vector3.TransformCoordinate(new SharpDX.Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z), vsData.WorldViewProj);

			// Matrices transform vertices to [-1; 1] coordinates, so we need to multiply by Height and Width
			v.X = (v.X + 1) / 2 * form.Width;
			v.Y = (-v.Y + 1) / 2 * form.Height;

			// Because we want to select faces, not vertices, distance should be big enough. Then we find closest mesh to camera by comparing Z.
			if (System.Numerics.Vector2.Distance(new(mouse.X, mouse.Y), new(v.X, v.Y)) < 30 && v.Z < minZ)
				minZ = v.Z;
		}

		return minZ;
	}

	public void SetSelection(bool selected)
	{
		cbSelected.Checked = selected;
		if (selected) Selected.Add(this);
		else Selected.Remove(this);
	}

	public void SetVisibility(bool hidden)
	{
		cbHidden.Checked = hidden;
		if (hidden) Hidden.Add(this);
		else Hidden.Remove(this);		
	}
}