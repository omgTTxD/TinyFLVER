using SharpDX;

class Outline 
{
	static VertexShader DrawOutline = new(d, Shaders.Compile("VS.hlsl", "OutlineVS"));
	static VertexShader FillBackground = new(d, Shaders.Compile("VS.hlsl", "BackgroundVS"));
	static PixelShader OutlinePS = new(d, Shaders.Compile("VS.hlsl", "OutlinePS"));
	static PixelShader BackgroundPS = new(d, Shaders.Compile("VS.hlsl", "BackgroundPS"));
	public static PixelShader PickingPS = new(d, Shaders.Compile("VS.hlsl", "PSMeshIDs"));

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
		int px = e.X * Renderer.scale;
		int py = e.Y * Renderer.scale;
		if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
			c.CopySubresourceRegion(idsRT.Resource, 0, new(px, py, 0, px + 1, py + 1, 1), idStaging, 0);
	}

	public static void RenderMeshIDs()
	{
		Renderer.SetAndClearRT(idsRT, idsDepth);
		c.PixelShader.Set(PickingPS);

		Mesh.Render(hiddenMeshes);
		ClearDepth(idsDepth);
		Mesh.Render(visibleMeshes);

		Renderer.SetAndClearMainRT();
	}

	public static void RenderOutline()
	{
		c.PixelShader.Set(OutlinePS);
		c.VertexShader.Set(DrawOutline);
		Mesh.Render([.. selectedMeshes, nearestMesh]);
		ClearDepth(Renderer.msaaDepth);

		c.PixelShader.Set(BackgroundPS);
		c.VertexShader.Set(FillBackground);
		Shaders.SetAlphaBlending(false);
		Mesh.Render([.. selectedMeshes, nearestMesh]);
		Shaders.SetAlphaBlending(true);
		ClearDepth(Renderer.msaaDepth);
	}

	static void ClearDepth(DepthStencilView depth) => c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
}