using SharpDX.Direct2D1;
using System.Drawing.Imaging;
using System.IO;
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
		if (form != Form.ActiveForm) { 
			Thread.Sleep(50); 
			return; 
		}
		
		Outline.RenderMeshIDs();
		SetAndClearRT(msaaRT, msaaDepth);
		//SetAndClearRT(Shadows.shadowsRT, Shadows.shadowsDepth);

		if (form.RenderSpheres.Checked)
			SpheresRenderer.Render();
		else
		{
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
		form.refreshTimer.Tick += (s, e) => Render();

		CreateSwapChain();
		ProcessResize();

		Outline.Initialize();
		Shadows.Initialize();
	}

	static void CreateSwapChain()
	{
		swapChain = new SwapChain1(new SharpDX.DXGI.Factory4(), G.d, ref Desc.swapChain);
		
		// Direct composition is needed for acrylic blur
		SharpDX.DirectComposition.Device dev = new(d.QueryInterface<SharpDX.DXGI.Device>());
		SharpDX.DirectComposition.Target.FromHwnd(dev, form.Handle, true).Root = new(dev) { Content = swapChain };
		dev.Commit();
		ApplyAcrylic(form.Handle);
	}

	static void ProcessResize()
	{
		backBuffer?.Dispose(); 
		swapChain.ResizeBuffers(0, w, h, 0, swapChain.Description1.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		msaaRT = CreateRenderTarget(Desc.msaaRT);
		msaaDepth = CreateDepth(Desc.msaaDepth);

		Outline.idRT = CreateRenderTarget(Desc.R32);
		Outline.idDepth = CreateDepth(Desc.Depth);

		Screenshot.renderTarget = CreateRenderTarget(backBuffer.Description);
		c.Rasterizer.SetViewport(0, 0, w * scale, h * scale);
	}

	static void ResolveMSAA() => c.ResolveSubresource(msaaRT.Resource, 0, backBuffer, 0, backBuffer.Description.Format);
}