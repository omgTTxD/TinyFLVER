using System.Threading;

class Renderer
{
	static SwapChain1 swapChain;
	public static Texture2D backBuffer;

	public static RenderTargetView msaaRT;
	public static DepthStencilView depth;

	static int antialiasing = 2;
	static int scale = 2;

	public static void Render()
	{
		if (form != Form.ActiveForm) { Thread.Sleep(50); return; }
		
		ShadersData.UpdateBuffers();

		if (form.RenderSpheres.Checked)
			ShaderSpheres.Render();
		else
		{
			Outline.Render();
			VisibleMeshes.ForEach(Mesh.Render);
		}

		c.ResolveSubresource(msaaRT.Resource, 0, backBuffer, 0, backBuffer.Description.Format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		form.ResizeEnd += (s, e) => ProcessResize();
		form.refreshTimer.Tick += (s, e) => Renderer.Render();

		dev = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = dev.ImmediateContext;

		CreateSwapChain();
		ProcessResize();

		Outline.Initialize();
	}

	static void CreateSwapChain()
	{
		var swapChainDesc = new SwapChainDescription1()
		{
			Width = 1, 
			Height = 1,
			BufferCount = 2,
			SwapEffect = SwapEffect.FlipDiscard,
			Format = Format.R16G16B16A16_Float,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
			SampleDescription = new(1, 0)
		};

		swapChain = new SwapChain1(new Factory4(), dev, ref swapChainDesc);

		// I used direct composition to be able to apply acrylic blur
		SharpDX.DirectComposition.Device d = new(dev.QueryInterface<SharpDX.DXGI.Device>());
		SharpDX.DirectComposition.Target.FromHwnd(d, form.Handle, true).Root = new(d) { Content = swapChain };
		d.Commit();
	}

	static void ProcessResize()
	{
		if (form.WindowState == FormWindowState.Minimized)
			return;
		
		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, w, h , Format.Unknown, swapChain.Description1.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
		
		RecreateRenderTargets();
	}

	static void RecreateRenderTargets()
	{
		var d = backBuffer.Description with
		{
			Width = w * scale,
			Height = h * scale,
			SampleDescription = new(antialiasing, 0)
		};

		msaaRT = new(dev, new Texture2D(dev, d));
		depth = new(dev, new Texture2D(dev, d with { 
			Format = Format.D32_Float, BindFlags = BindFlags.DepthStencil }));
	}

	public static void SetRenderTargets(RenderTargetView renderTarget, DepthStencilView depthStencil)
	{
		c.OutputMerger.SetTargets(depthStencil, renderTarget);
		c.ClearRenderTargetView(renderTarget, new());
		c.ClearDepthStencilView(depthStencil, DepthStencilClearFlags.Depth, 1, 0);

		// Viewport size must be equal to render target
		using var tex = renderTarget.Resource.QueryInterface<Texture2D>();
		c.Rasterizer.SetViewport(0, 0, tex.Description.Width, tex.Description.Height);
	}

	public static void ClearDepth()
	{
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
	}
}