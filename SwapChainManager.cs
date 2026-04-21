using SharpDX.DirectComposition;
using Device = SharpDX.DXGI.Device;

public abstract class SwapChainManager
{
	static Texture2D backBuffer;
	static DepthStencilView depth;
	static RenderTargetView output;
	static SwapChain3 swapChain;

	static Format format = Format.R16G16B16A16_Float;

	public static void Render()
	{
		// Increasing sleep increases delay after alt-tab. Now it consumes 0.5% in background
		if (form != Form.ActiveForm) { Thread.Sleep(1); return; }
		Shaders.UpdateBuffers();

		c.ClearRenderTargetView(output, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);

		if (form.debugNormals.Checked) Meshes.ForEach(ShaderDebug.Render);
		if (nearestMesh is not null ) ShaderOutline.Render(nearestMesh);
		Meshes.ForEach(m => { if (m.Selected) ShaderOutline.Render(m); });
		Meshes.Where(m => !m.Hidden).ToList().ForEach(Mesh.Render);

		c.ResolveSubresource(output.Resource, 0, backBuffer, 0, format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		// BgraSupport is required for Direct Composition
		d = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = d.ImmediateContext;

		var desc = new SwapChainDescription1() 
		{ 
			BufferCount = 2, Width = 1, Height = 1,
			Format = format,
			SampleDescription = new(1, 0),
			SwapEffect = SwapEffect.FlipDiscard,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
		};

		swapChain = new SwapChain1(new Factory4(), d, ref desc).QueryInterface<SwapChain3>();
		
		ProcessResize();
		CreateRenderTargets();
		ApplyAcrylicBlur();

		form.SizeChanged += (s, e) => ProcessResize();
	}

	static void CreateRenderTargets()
	{
		// Render targets and viewport should be always at screen maximum resolution regardless of form size
		int w = Screen.PrimaryScreen.Bounds.Width;
		int h = Screen.PrimaryScreen.Bounds.Height;
		var desc = backBuffer.Description with { Width = w, Height = h, SampleDescription = new(8, 0) };

		output = new(d, new Texture2D(d, desc));
		depth = new(d, new Texture2D(d, desc with { Format = Format.D32_Float, BindFlags = BindFlags.DepthStencil }));

		c.OutputMerger.SetTargets(depth, output);
		c.Rasterizer.SetViewport(0, 0, w, h);
	}

	static void ProcessResize()
	{
		int w = form.ClientSize.Width;
		int h = form.ClientSize.Height;

		if (form.WindowState == FormWindowState.Minimized)
			return;

		matrices.Projection = SharpDX.Matrix.PerspectiveFovLH(Pi / 6, 1f * w / h, 0.01f, 100f);

		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, w, h, format, SwapChainFlags.None);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
	}

	static void ApplyAcrylicBlur()
	{
		[DllImport("AcrylicWindow.dll")]
		static extern void ApplyAcrylic(IntPtr hWnd, int radius = 75, float saturation = 1.5f);
		[DllImport("AcrylicWindow.dll")]
		static extern void SyncWindowPosition(IntPtr hWnd);

		SharpDX.DirectComposition.Device dev = new(d.QueryInterface<Device>());
		var visual = new Visual(dev) { Content = swapChain };
		Target.FromHwnd(dev, form.Handle, true).Root = visual;
		dev.Commit();

		ApplyAcrylic(form.Handle);
		form.Move += (s, e) => SyncWindowPosition(form.Handle);
	}
}