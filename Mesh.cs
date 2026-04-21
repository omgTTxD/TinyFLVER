using SharpDX.Direct3D11;
using SharpDX.Mathematics;
using SoulsFormats;
using SoulsFormats.Other.MWC;
using System.Numerics;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TinyFLVER;

public class Mesh
{
	public Vertex[] Vs;
	public VertexBufferBinding bufferBinding;

	public ShaderResourceView diffuseBinding;
	public ShaderResourceView normalBinding;
	public ShaderResourceView sssBinding;

	public FLVER2.Mesh flverMesh;
	public FLVER2.Material material;

	public CheckBox cbSelected;
	public CheckBox cbHidden;

	public bool Transparent;

	public struct Vertex(SoulsFormats.FLVER.Vertex v)
	{
		public Vector3 Position = v.Position;
		public Vector3 Normal = v.Normal;
		public Vector2 UV = new(v.UVs[0].X, v.UVs[0].Y);
	}

	public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
	{
		this.material = material;
		flverMesh = mesh;

		var faces = flverMesh.GetFaces();
		Vs = new Vertex[faces.Count * 3];
		for (int i = 0; i < faces.Count; i++)
		{
			Vs[i * 3] = new(faces[i][0]);
			Vs[i * 3 + 1] = new(faces[i][1]);
			Vs[i * 3 + 2] = new(faces[i][2]);
		}

		if (Vs.Length == 0) return;

		var vertexBuffer = Buffer.Create(dev, BindFlags.VertexBuffer, Vs);
		bufferBinding = new(vertexBuffer, Marshal.SizeOf<Vertex>(), 0);
		Textures.Load(this);
	}


	public float CheckIntersection(System.Drawing.Point mouseCrd)
	{
		float minZ = 1;
		foreach (Vertex v in Vs)
		{
			// Just project point to screen again, to get exact screen coordinates. Very easy. Like all things should be.
			//	 new SharpDX.Vector(v.Position.X, v.Position.Y, v.Position.Z) * Camera.vsData.WorldViewProj;
			var vertex = SharpDX.Vector3.TransformCoordinate(new SharpDX.Vector3(v.Position.X, v.Position.Y, v.Position.Z), Camera.matrices.WorldViewProj);

			// Matrices transform vertices to [-1; 1] coordinates, so we need to multiply by Height and Width
			vertex.X = (vertex.X + 1) / 2 * form.Width;
			vertex.Y = (-vertex.Y + 1) / 2 * form.Height;
			// Because we want to select faces, not vertices, distance should be big enough. Then we find closest mesh to camera by comparing Z.
			if (System.Numerics.Vector2.Distance(new(mouseCrd.X, mouseCrd.Y), new(vertex.X, vertex.Y)) < 30 && vertex.Z < minZ)
				minZ = vertex.Z;
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