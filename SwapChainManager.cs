class SwapChainManager
{
	static Texture2D backBuffer;
	static DepthStencilView depth;
	static RenderTargetView output;
	static SwapChain3 swapChain;

	static int aaSamples = 2;
	static int scale = 2;

	public static void Render()
	{
		if (form != Form.ActiveForm) { Thread.Sleep(5); return; }
		c.ClearRenderTargetView(output, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
		
		ShadersData.UpdateBuffers();
		if (nearestMesh is not null) ShaderOutline.Render(nearestMesh);
		SelectedMeshes.ForEach(ShaderOutline.Render);
		Meshes.Where(m => !m.Hidden).ToList().ForEach(Mesh.Render);

		c.ResolveSubresource(output.Resource, 0, backBuffer, 0, backBuffer.Description.Format);
		swapChain.Present(0, PresentFlags.None);
	}

	public static void Initialize()
	{
		d = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = d.ImmediateContext;
		
		var swapChainDesc = new SwapChainDescription1() {
			BufferCount = 2, Width = 1, Height = 1,
			Format = Format.R16G16B16A16_Float,
			SampleDescription = new(1, 0),
			SwapEffect = SwapEffect.FlipDiscard,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
		};

		swapChain = new SwapChain1(new Factory4(), d, ref swapChainDesc).QueryInterface<SwapChain3>();
		form.Resize += (s, e) => ProcessResize();
		ProcessResize();
		
		var desc = backBuffer.Description with { Width = w * scale, Height = h * scale, SampleDescription = new(aaSamples, 0) };
		output = new(d, new Texture2D(d, desc));
		depth = new(d, new Texture2D(d, desc with { Format = Format.D32_Float, BindFlags = BindFlags.DepthStencil }));
		
		c.OutputMerger.SetTargets(depth, output);
		c.Rasterizer.SetViewport(0, 0, w * scale, h * scale);
		ApplyAcrylicBlur();
	}

	static void ProcessResize()
	{
		if (form.WindowState == FormWindowState.Minimized) return;
	
		w = form.ClientSize.Width; 
		h = form.ClientSize.Height;
		
		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, w, h, Format.Unknown, swapChain.Description1.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		Camera.UpdateShaderData();
	}

	static void ApplyAcrylicBlur()
	{		
		[System.Runtime.InteropServices.DllImport("AcrylicWindow.dll")]
		static extern void ApplyAcrylic(IntPtr hWnd, int radius = 60, float saturation = 1.3f);
		ApplyAcrylic(form.Handle);

		SharpDX.DirectComposition.Device dev = new(d.QueryInterface<SharpDX.DXGI.Device>());
		SharpDX.DirectComposition.Target.FromHwnd(dev, form.Handle, true).Root = new (dev) { Content = swapChain };
		dev.Commit();
	}
}