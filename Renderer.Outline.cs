class Outline
{
	static VertexShader DrawOutline = new(dev, Shaders.Compile("Outline.hlsl", "DrawOutline"));
	static VertexShader FillBackground = new(dev, Shaders.Compile("Outline.hlsl", "FillBackground"));
	static PixelShader OutlinePS = new(dev, Shaders.Compile("Outline.hlsl", "PS"));
	public static PixelShader PickingPS = new(dev, Shaders.Compile("Outline.hlsl", "PSMeshIDs"));

	public static RenderTargetView idsRT;
	static DepthStencilView idsDepth;
	public static Texture2D idsStaging;

	public static void Initialize()
	{
		idsStaging = new Texture2D(dev, Renderer.backBuffer.Description with
		{
			Width = 1,
			Height = 1,
			Format = Format.R32_UInt,
			Usage = ResourceUsage.Staging,
			CpuAccessFlags = CpuAccessFlags.Read,
			BindFlags = BindFlags.None
		});

		form.ResizeEnd += (s, e) => RecreateRenderTargets();
		RecreateRenderTargets();
	}

	

	public static void Render()
	{
		GetNearestMesh();
		if (nearestMesh is not null)
			RenderOutline(nearestMesh);

		SelectedMeshes.ForEach(Outline.RenderOutline);
	}

	static void GetNearestMesh()
	{
		RenderMeshIDs();

		// Clamp mouse coordinates to form bounds
		int px = (int)Math.Clamp(Camera.mouse.x, 0, w - 1);
		int py = (int)Math.Clamp(Camera.mouse.y, 0, h - 1);

		// Get mesh ID under mouse pointer
		c.CopySubresourceRegion(idsRT.Resource, 0, new(px, py, 0, px + 1, py + 1, 1), idsStaging, 0);
		c.MapSubresource(idsStaging, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None, out var data);
		nearestMesh = Meshes.ElementAtOrDefault(Utilities.Read<int>(data.DataPointer) - 1);
	}

	static void RecreateRenderTargets()
	{
		if (form.WindowState == FormWindowState.Minimized)
			return;

		idsRT = new(dev, new Texture2D(dev, Renderer.backBuffer.Description with { Format = Format.R32_UInt }));
		idsDepth = new(dev, new Texture2D(dev, Renderer.backBuffer.Description with
		{
			Format = Format.D32_Float,
			BindFlags = BindFlags.DepthStencil
		}));
	}

	static void RenderMeshIDs()
	{
		void RenderMeshID(Mesh m)
		{
			shaderData.MeshID = Meshes.IndexOf(m) + 1;
			Mesh.Render(m);
		}

		Renderer.SetRenderTargets(idsRT, idsDepth);
		c.PixelShader.Set(PickingPS);

		// Render mesh IDs
		HiddenMeshes.ForEach(RenderMeshID);
		c.ClearDepthStencilView(idsDepth, DepthStencilClearFlags.Depth, 1, 0);
		VisibleMeshes.ForEach(RenderMeshID);

		c.PixelShader.Set(Shaders.PS);
		Renderer.SetRenderTargets(Renderer.msaaRT, Renderer.depth);
	}

	static void RenderOutline(Mesh m)
	{
		c.PixelShader.Set(OutlinePS);

		c.VertexShader.Set(DrawOutline);
		Mesh.Render(m);
		Renderer.ClearDepth();
		Shaders.SetAlphaBlending(false);

		c.VertexShader.Set(FillBackground);
		Mesh.Render(m);
		Renderer.ClearDepth();
		Shaders.SetAlphaBlending(true);

		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(Shaders.PS);
	}
}