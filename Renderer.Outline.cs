using Microsoft.VisualBasic.Devices;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using SharpDX.DXGI;
using SoulsFormats.Attributes;
using SoulsFormats.Other.MWC;
using SharpDX;

class Outline 
{
	static VertexShader DrawOutline = new(d, Shaders.Compile("Outline.hlsl", "DrawOutline"));
	static VertexShader FillBackground = new(d, Shaders.Compile("Outline.hlsl", "FillBackground"));
	static PixelShader OutlinePS = new(d, Shaders.Compile("Outline.hlsl", "OutlinePS"));
	static PixelShader BackgroundPS = new(d, Shaders.Compile("Outline.hlsl", "BackgroundPS"));
	public static PixelShader PickingPS = new(d, Shaders.Compile("Outline.hlsl", "PSMeshIDs"));

	public static RenderTargetView idsRT;
	public static DepthStencilView idsDepth;
	public static Texture2D idStaging;
	public static DataStream idData;

	public static void Initialize()
	{
		form.MouseMove += SelectMeshUnderCursor;
		idStaging = CreateTex2D(Desc.StagingMeshIDs); 
		c.MapSubresource(idStaging, 0, MapMode.Read, 0, out idData);
	}

	static void SelectMeshUnderCursor(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
			c.CopySubresourceRegion(idsRT.Resource, 0, new(e.X, e.Y, 0, e.X + 1, e.Y + 1, 1), idStaging, 0);
	}

	public static void RenderMeshIDs()
	{
		Renderer.SetAndClearRT(idsRT, idsDepth);
		c.PixelShader.Set(PickingPS);

		Mesh.Render(hiddenMeshes);
		ClearDepth(idsDepth);
		Mesh.Render(visibleMeshes);
	}

	public static void RenderOutline()
	{
		List<Mesh> outlines = [.. selectedMeshes];
		
		if (nearestMesh != null)
			outlines.Add(nearestMesh);

		Renderer.SetAndClearRT();

		c.PixelShader.Set(OutlinePS);
		c.VertexShader.Set(DrawOutline);
		Mesh.Render(outlines);

		ClearDepth(Renderer.msaaDepth);

		c.PixelShader.Set(BackgroundPS);
		c.VertexShader.Set(FillBackground);
		Shaders.SetAlphaBlending(false);
		Mesh.Render(outlines);
		Shaders.SetAlphaBlending(true);

		ClearDepth(Renderer.msaaDepth);
	}

	static void ClearDepth(DepthStencilView depth) => c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
}