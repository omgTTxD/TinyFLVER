using System.Collections.Generic;
using D3D11 = SharpDX.Direct3D11.Device;
using DComp = SharpDX.DirectComposition.Device;
using DXGI = SharpDX.DXGI.Device;
using Target = SharpDX.DirectComposition.Target;

class Renderer
{
	static SwapChain1 swapChain;
	static RenderTargetView msaaRT;
	static DepthStencilView msaaDepth;

	static int scale = 2;
	static int aaSamples = 4;

	public static D3D11 d;
	public static DeviceContext c;

	static DComp dCompDev;
	static Target dCompTarget;


	public static void Render()
	{
		ShaderData.SetFormatID();
		PrepareRT(msaaRT, msaaDepth);

		if (form.RenderSpheres.Checked)
			SpheresRenderer.Render();
		else
		{
			Outline.RenderMeshIDs();
			Shaders.SetMain();
			Mesh.Render(visibleMeshes);
			Outline.RenderOutline();
		}

		c.ResolveSubresource(msaaRT.Resource, 0, backBuffer, 0, backBuffer.Description.Format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		d = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = d.ImmediateContext;

		SwapChainDescription1 desc = new() {
			Width = 1, Height = 1,
			BufferCount = 2,
			SwapEffect = SwapEffect.FlipDiscard,
			Format = Format.R16G16B16A16_Float,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
			SampleDescription = new(1, 0),
		};
		swapChain = new SwapChain1(new Factory2(), d, ref desc);

		// Direct composition is needed for acrylic blur
		dCompDev = new(d.QueryInterface<DXGI>());
		dCompTarget = Target.FromHwnd(dCompDev, form.Handle, true);
		dCompTarget.Root = new(dCompDev) { Content = swapChain };
		dCompDev.Commit();
		ApplyAcrylic(form.Handle);

		form.refreshTimer.Tick += (s, e) => { if (form == Form.ActiveForm) Render(); };
		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();
	}

	static void ProcessResize()
	{
		backBuffer?.Dispose(); 
		swapChain.ResizeBuffers(0, w , h , 0, swapChain.Description.Flags);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		var msaaDesc = backBuffer.Description with
		{
			Width = w * scale,
			Height = h * scale,
			SampleDescription = new(Renderer.aaSamples, 0)
		};
		msaaRT = CreateRenderTarget(msaaDesc);

		msaaDepth = CreateDepth(msaaDesc with 
		{
			Format = Format.D24_UNorm_S8_UInt,
			BindFlags = BindFlags.DepthStencil
		});

		Outline.idsRT = CreateRenderTarget(backBuffer.Description with { Format = Format.R32_Float });
		Outline.idsDepth = CreateDepth(backBuffer.Description with { Format = Format.D32_Float, BindFlags = BindFlags.DepthStencil } );

		Camera.UpdateShaderData();
	}

	public static void PrepareRT(RenderTargetView rt = null, DepthStencilView depth = null)
	{
		rt ??= msaaRT; 	depth ??= msaaDepth;

		c.OutputMerger.SetTargets(depth, rt);
		c.ClearRenderTargetView(rt, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);

		var desc = depth.ResourceAs<Texture2D>().Description;
		c.Rasterizer.SetViewport(0, 0, desc.Width, desc.Height);
	}
}