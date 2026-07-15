using Microsoft.VisualBasic.Devices;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.DXGI;

class Outline 
{
	static VertexShader DrawOutline = new(d, Shaders.Compile("Outline.hlsl", "DrawOutline"));
	static VertexShader FillBackground = new(d, Shaders.Compile("Outline.hlsl", "FillBackground"));
	static PixelShader OutlinePS = new(d, Shaders.Compile("Outline.hlsl", "PS"));
	public static PixelShader PickingPS = new(d, Shaders.Compile("Outline.hlsl", "PSMeshIDs"));

	public static RenderTargetView idRT;
	public static DepthStencilView idDepth;
	public static Texture2D idStaging;
	public static DataStream idData;

	public static void Initialize()
	{
		form.MouseMove += SelectMeshUnderCursor;

		idStaging = CreateTex2D(Desc.StagingR32); 
		c.MapSubresource(idStaging, 0, MapMode.Read, 0, out idData);
	}

	static void SelectMeshUnderCursor(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			nearestMesh = null;
			return;
		}

		c.CopySubresourceRegion(idRT.Resource, 0, new(e.X, e.Y, 0, e.X + 1, e.Y + 1, 1), idStaging, 0);
		nearestMesh = Meshes.ElementAtOrDefault(Utilities.Read<int>(idData.DataPointer) - 1);
	}

	public static void RenderMeshIDs()
	{
		SetAndClearRT(idRT, idDepth);
		c.PixelShader.Set(PickingPS);

		Mesh.Render(hiddenMeshes);
		ClearDepth(idDepth);
		Mesh.Render(visibleMeshes);
	}

	public static void RenderOutline()
	{
		c.PixelShader.Set(OutlinePS);
		c.VertexShader.Set(DrawOutline);

		if (nearestMesh is not null)
			Mesh.Render(nearestMesh);

		Mesh.Render(selectedMeshes);

		c.VertexShader.Set(FillBackground);
		Shaders.SetAlphaBlending(false);
		Mesh.Render(selectedMeshes);
		Shaders.SetAlphaBlending(true);

		ClearDepth(Renderer.msaaDepth);
	}


	static void ClearDepth(DepthStencilView depth) => c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
}