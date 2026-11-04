using D3D11 = SharpDX.Direct3D11.Device;
using DCompDev = SharpDX.DirectComposition.Device;
using DXGI = SharpDX.DXGI.Device;
using Target = SharpDX.DirectComposition.Target;

class Renderer
{
	public static D3D11 d;
	public static DeviceContext c;

	static DCompDev dCompDev;
	static Target dCompTarget;

	static SwapChain1 swapChain;
	public static RenderTargetView msaaRT;
	public static DepthStencilView msaaDepth;

	static int scale = 1;
	static int aaSamples = 8;

	public static void Render()
	{
		SetAndClearRT(msaaRT, msaaDepth);
		Spheres.RenderLightSource();
		ShaderData.SetFormatID();

		if (form.RenderSpheres.Checked)
			Spheres.Render();
		else
		{
			Outline.RenderMeshIDs();
			ShadowMap.RenderShadowMap();
			Mesh.Render(visibleMeshes, Shaders.PS, Shaders.VS);
			Outline.RenderOutline();
			Subsurface.PostProcessing();
		}

		ResolveToBackbuffer();
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		d = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = d.ImmediateContext;

		SwapChainDescription1 desc = new()
		{
			Width = 1, Height = 1,
			BufferCount = 2,
			SwapEffect = SwapEffect.FlipSequential,
			Format = Format.R16G16B16A16_Float,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
			SampleDescription = new(1, 0),
		};
		swapChain = new SwapChain1(new Factory4(), d, ref desc);

		// Direct composition is needed for acrylic blur
		dCompDev = new(d.QueryInterface<DXGI>());
		dCompTarget = Target.FromHwnd(dCompDev, form.Handle, true);
		dCompTarget.Root = new(dCompDev) { Content = swapChain };
		dCompDev.Commit();
		ApplyAcrylic(form.Handle);

		form.refreshTimer.Interval = 100;
		form.refreshTimer.Tick += (s, e) => { if (form == Form.ActiveForm) Render(); };

		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();
	}

	static void ProcessResize()
	{
		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, w, h, 0, swapChain.Description.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		var msaaDesc = backBuffer.Description with
		{
			Width = w * scale,
			Height = h * scale,
			SampleDescription = new(aaSamples, 0)
		};

		msaaRT = new(d, new Texture2D(d, msaaDesc));
		msaaDepth = CreateDepth(msaaDesc with { Format = Format.D24_UNorm_S8_UInt });
	}

	public static void SetAndClearRT(RenderTargetView rt, DepthStencilView depth)
	{
		c.ClearRenderTargetView(rt, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
		c.OutputMerger.SetTargets(depth, rt);

		Texture2DDescription desc = depth is not null ? depth.ResourceAs<Texture2D>().Description : rt.ResourceAs<Texture2D>().Description;
		c.Rasterizer.SetViewport(0, 0, desc.Width, desc.Height);
	}

	public static void SetMainRT() {
		c.OutputMerger.SetTargets(msaaDepth, msaaRT);
		c.Rasterizer.SetViewport(0, 0, w * scale, h * scale);
	}

	// Final step before presenting frame, resolve current render target (post-processing RT in most cases), or just MSAA RT in earlier versions
	static void ResolveToBackbuffer() => c.ResolveSubresource(c.OutputMerger.GetRenderTargets(1)[0].Resource, 0, backBuffer, 0, Format.R16G16B16A16_Float);
}