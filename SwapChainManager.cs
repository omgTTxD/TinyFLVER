using SharpDX.Direct3D;
using SharpDX.DirectComposition;
using AlphaMode = SharpDX.DXGI.AlphaMode;
using Device = SharpDX.DirectComposition.Device;
using Factory2 = SharpDX.DXGI.Factory2;

namespace TinyFLVER;

public abstract class SwapChainManager
{
	static Texture2D backBuffer;
	static RenderTargetView msaa;
	static RenderTargetView msaaSDR;
	static RenderTargetView RTV;
	static DepthStencilView depth;
	static SwapChain3 swapChain;
	public static float bg;

	public static Format format = Format.R16G16B16A16_Float;
//	public static Format format = Format.R8G8B8A8_UNorm;

	public static void Render()
	{
		if (Form.ActiveForm != form) { Thread.Sleep(100); return; }
		Shaders.UpdateBuffers();
		
		c.ClearRenderTargetView(RTV, new(0, 0, 0, 0));
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);

		if (form.RenderSpheres.Checked)
			ShaderSpheres.Render();
		else
		{
			Selected.ForEach(ShaderOutline.Render);
			Meshes.Except(Hidden).ToList().FindAll(x => !Hidden.Contains(x) && !x.Transparent).ForEach(Mesh.Render);
			Meshes.FindAll(x => !Hidden.Contains(x) && x.Transparent).ForEach(Mesh.Render);
		}

		c.ResolveSubresource(RTV.Resource, 0, backBuffer, 0, format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		form.SizeChanged += (s, e) => ProcessResize();
		form.EnableHDR.CheckedChanged += (s, e) => ProcessResize(); 

		dev = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = dev.ImmediateContext;

		SwapChainForComposition();
		CreateRenderTargets();

		c.Rasterizer.SetViewport(0, 0, form.Width, form.Height);

		SetBackfaceCulling(CullMode.None);
		EnableAlphaBlending();
		ProcessResize();
	}

	static void CreateRenderTargets()
	{
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		var description = backBuffer.Description;
		description.SampleDescription = new(8, 0);
		description.Format = Format.R16G16B16A16_Float;

		msaa = new(dev, new Texture2D(dev, description));
		description.Format = Format.R8G8B8A8_UNorm;
		msaaSDR = new(dev, new Texture2D(dev, description));

		description.Format = Format.D32_Float;
		description.BindFlags = BindFlags.DepthStencil;
		depth = new(dev, new Texture2D(dev, description));
	}

	static void SwapChainForComposition()
	{
		var swapChainDescription1 = new SwapChainDescription1()
		{
			BufferCount = 2,
			Width = form.Width,
			Height = form.Height,
			Format = format,
			SampleDescription = new(1, 0),
			SwapEffect = SwapEffect.FlipSequential,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied
		};

		var swapChain1 = new SwapChain1(new Factory2(), dev, ref swapChainDescription1);
		swapChain = swapChain1.QueryInterface<SwapChain3>();

		var dxgiDevice = dev.QueryInterface<SharpDX.DXGI.Device>();
		var dcompDevice = new Device(dxgiDevice);
		var dcompVisual = new Visual(dcompDevice);

		dcompVisual.Content = swapChain;
		Target.FromHwnd(dcompDevice, form.Handle, true).Root = dcompVisual;
		dcompDevice.Commit();
	}

	public static void ProcessResize()
	{

		if (form.EnableHDR.Checked)
			format = Format.R16G16B16A16_Float;
		else
			format = Format.R8G8B8A8_UNorm;

		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, form.Width, form.Height, format, SwapChainFlags.None);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		if (form.EnableHDR.Checked)
		{
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG10NoneP709;
			form.Exposure.Value = 10;
			form.Gamma.Value = 80;
			bg = 0.0145f;
			RTV = msaa;
		}
		else
		{
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG22NoneP709;
			form.Exposure.Value = 10;
			form.Gamma.Value = 220;
			bg = 0.12f;
			RTV = msaaSDR;
		}
	
		c.OutputMerger.SetRenderTargets(depth, RTV);
	}

	public static void EnableAlphaBlending()
	{
		var desc = BlendStateDescription.Default();
		desc.RenderTarget[0].IsBlendEnabled = true;
		desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
		desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
		desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
		c.OutputMerger.BlendState = new BlendState(dev, desc);
	}

	public static void SetBackfaceCulling(CullMode mode)
	{
		c.Rasterizer.State = new(dev, new() { CullMode = mode, FillMode = SharpDX.Direct3D11.FillMode.Solid, IsFrontCounterClockwise = true });
	}
}