using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SoulsFormats;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TinyFLVER;

public class Mesh
{
	Vertex[] vertices;
	public int[] indices;
	
	DeviceContext context = SwapChainManager.context;

	public ShaderResourceView diffuseShaderResource = Textures.defaultDiffuseShaderResource;
	public ShaderResourceView normalShaderResource = Textures.defaultDiffuseShaderResource;

	public FLVER2.Mesh flverMesh;
	public FLVER2.Material material;

	public CheckBox cbSelected;
	public CheckBox cbHidden;

	Boolean TexturesLoaded;

	public Buffer indexBuffer;
	public VertexBufferBinding vertexBufferBinding;
	public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
	{
		this.material = material;
		flverMesh = mesh;

		vertices = [.. flverMesh.Vertices.Select(v => new Vertex(v))];
		indices = [.. flverMesh.FaceSets[0].Indices];

		// Some mods have broken .flver which will crash during IndexBuffer creation, so we skip those meshes
		if (indices.Length == 0) return;

		var vertexBuffer = Buffer.Create(SwapChainManager.device, BindFlags.VertexBuffer, vertices);
		vertexBufferBinding = new (vertexBuffer, SharpDX.Utilities.SizeOf<Vertex>(), 0);

		context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
		indexBuffer = Buffer.Create(SwapChainManager.device, BindFlags.IndexBuffer, indices);

		LoadTextures();	
	}


	public void Render()
	{
		context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
		context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		context.PixelShader.SetShaderResources(0, diffuseShaderResource);
		context.PixelShader.SetShaderResource(1, normalShaderResource);

		Shaders.SetMainShaders();

		context.DrawIndexed(indices.Length, 0, 0);
	}


	public async void LoadTextures()
	{
		await Task.Run(() =>
		{
			var diffuse = Textures.LoadDDS(material.Textures.Find(x => x.Path.ToLower().Contains("_a")));
			if (diffuse != null)
				diffuseShaderResource = new(SwapChainManager.device, diffuse);

			var normal = Textures.LoadDDS(material.Textures.Find(x => x.Path.ToLower().Contains("_n")));

			if (normal != null)
			{
				ShaderResourceView tmp = new(SwapChainManager.device, normal);
				SwapChainManager.context.GenerateMips(tmp);
				normalShaderResource = tmp;
			}
		});
	}

	public float CheckIntersection(System.Drawing.Point mouseCrd)
	{
		float minZ = 1;

		foreach (Vertex v in vertices)
		{
			// Just project point to screen again, to get exact screen coordinates. Very easy. Like all things should be.
			SharpDX.Vector3 vertexCrd = Camera.ProjectVertexToScreen(v.Position);
		
			// Because we want to select faces, not vertices, distance should be big enough. Then we find closest mesh to camera by comparing Z.
			if (System.Numerics.Vector2.Distance(new(mouseCrd.X, mouseCrd.Y), new(vertexCrd.X, vertexCrd.Y)) < 30 && vertexCrd.Z < minZ)
				minZ = vertexCrd.Z;
		}

		return minZ;
	}

	public void SetSelection(bool selected)
	{
		cbSelected.Checked = selected;

		if (selected) Flver.Selected.Add(this);
		else Flver.Selected.Remove(this);
	}

	public void SetVisibility(bool hidden)
	{
		cbHidden.Checked = hidden;

		if (hidden) Flver.Hidden.Add(this);
		else Flver.Hidden.Remove(this);		
	}
}