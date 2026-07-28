using SharpDX;
using System.Drawing.Imaging;
using System.Threading;

class Renderer
{
	public static SwapChain1 swapChain;
	public static RenderTargetView msaaRT;
	public static DepthStencilView msaaDepth;

	public static int aaSamples = 4;
	public static int scale = 2;

	public static void Render()
	{
		if (form.RenderSpheres.Checked) 
			SpheresRenderer.Render();
		else
		{
			Outline.RenderMeshIDs();
			Outline.RenderOutline();
			Shaders.SetMain();
			Mesh.Render(visibleMeshes);
		}

		ResolveMSAA();
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		form.ResizeEnd += (s, e) => ProcessResize();
		form.refreshTimer.Tick += (s, e) => { if (form == Form.ActiveForm) Render(); };

		SwapChainDescription1 desc = new()
		{
			Width = 1, Height = 1,
			BufferCount = 2,
			SwapEffect = SwapEffect.FlipDiscard,
			Format = Format.R16G16B16A16_Float,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
			SampleDescription = new(1, 0)
		};

		swapChain = new SwapChain1(new SharpDX.DXGI.Factory4(), G.d, ref desc);

		// Direct composition is needed for acrylic blur
		ApplyAcrylic(form.Handle);
		SharpDX.DirectComposition.Device dCompDev = new(d.QueryInterface<SharpDX.DXGI.Device>());
		SharpDX.DirectComposition.Target.FromHwnd(dCompDev, form.Handle, true).Root = new(dCompDev) { Content = swapChain };
		dCompDev.Commit();

		ProcessResize();
		Outline.Initialize();
	}

	static void ProcessResize()
	{
		Utilities.Dispose(ref backBuffer);
		swapChain.ResizeBuffers(0, w, h, 0, swapChain.Description1.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		msaaRT = CreateRenderTarget(Desc.msaaRT);
		msaaDepth = CreateDepth(Desc.msaaDepth);

		Outline.idsRT = CreateRenderTarget(Desc.R32);
		Outline.idsDepth = CreateDepth(Desc.Depth);
	}

	public static void SetAndClearRT(RenderTargetView rt, DepthStencilView depth)
	{
		c.OutputMerger.SetTargets(depth, rt);
		c.ClearRenderTargetView(rt, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
		var desc = depth.ResourceAs<Texture2D>().Description;
		c.Rasterizer.SetViewport(0, 0, desc.Width, desc.Height);
	}

	public static void SetAndClearRT()
	{
		c.OutputMerger.SetTargets(msaaDepth, msaaRT);
		c.ClearRenderTargetView(msaaRT, new());
		c.ClearDepthStencilView(msaaDepth, DepthStencilClearFlags.Depth, 1, 0);
		c.Rasterizer.SetViewport(0, 0, w * scale, h * scale);
	}

	static void ResolveMSAA() => c.ResolveSubresource(msaaRT.Resource, 0, backBuffer, 0, backBuffer.Description.Format);
}