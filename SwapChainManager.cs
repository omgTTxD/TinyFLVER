using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

namespace TinyFLVER;

public abstract class SwapChainManager
{
	public static Device device;
	public static DeviceContext context;

	static MainForm form;
	static ListBox swapChainFormat;

	static Texture2D backBuffer;
	static RenderTargetView renderTargetView;

	static Texture2D depthBuffer;
	static DepthStencilView depthStencilView;

	static SwapChain3 swapChain;
	public static float bgColor = 0.0145f;

	public static void Render()
	{
		Shaders.UpdateBuffers();

		context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);
		context.ClearRenderTargetView(renderTargetView, new RawColor4(bgColor, bgColor, bgColor, 0));
		
		Flver.Selected.ForEach(x => OutlineEffect.Render(x));
		Flver.Meshes.Except(Flver.Hidden).ToList().ForEach(m => m.Render());

		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize(MainForm form)
	{
		SwapChainManager.form = form;
		SwapChainManager.swapChainFormat = form.swapChainFormat;
		form.checkBoxBackfaceCulling.CheckedChanged += (_, _) => SetBackfaceCulling();
		form.Resize += RecreateBuffers;

		//Format.R8G8B8A8_UNorm_SRgb requires Device.CreateWithSwapChain and basic SwapChain class
		form.swapChainFormat.DataSource = new[]
		{
			Format.R8G8B8A8_UNorm,
			Format.R8G8B8A8_UNorm_SRgb,
			Format.R10G10B10A2_UNorm,
			Format.R16G16B16A16_Float,
		};

		form.swapChainFormat.SelectedIndex = 3;
		
		swapChainFormat.SelectedIndexChanged += RecreateBuffers;
		
		var swapChainDescription1 = new SwapChainDescription1
		{
			BufferCount = 1,
			SampleDescription = new(1, 0),
			Usage = Usage.RenderTargetOutput,
			Format = (Format)swapChainFormat.SelectedItem,
			SwapEffect = SwapEffect.Sequential
		};

		device = new Device(DriverType.Hardware);
		var swapChain1 = new SwapChain1(new Factory2(), device, form.renderControl.Handle, ref swapChainDescription1);
		swapChain = swapChain1.QueryInterface<SwapChain3>();

		context = device.ImmediateContext;
		RecreateBuffers(null, null);
		SetBackfaceCulling();
	}

	public static void RecreateBuffers(object sender, EventArgs e)
	{
		var format = (Format)swapChainFormat.SelectedItem;

		// Resize viewport
		context.Rasterizer.SetViewport(new Viewport(0, 0, form.renderControl.Width, form.renderControl.Height));
		
		Textures.DiffuseSRgb = format == Format.R16G16B16A16_Float || format == Format.R8G8B8A8_UNorm_SRgb;
		
		if (Textures.DiffuseSRgb)
			bgColor = 0.0145f;
		else
			bgColor = 0.12f;

		backBuffer?.Dispose();
		renderTargetView?.Dispose();

		swapChain.ResizeBuffers(0, 0, 0, format, SwapChainFlags.None);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);
		renderTargetView = new RenderTargetView(device, backBuffer);

		if (format == Format.R16G16B16A16_Float)
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG10NoneP709;
		else
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG22NoneP709;
	
		// Depth Buffer
		var depthDescription = new Texture2DDescription()
		{
			Format = Format.D32_Float,
			ArraySize = 1,
			Width = form.renderControl.Width,
			Height = form.renderControl.Height,
			SampleDescription = new (1, 0),
			BindFlags = BindFlags.DepthStencil,
		};
		depthBuffer = new Texture2D(device, depthDescription);
		depthStencilView = new DepthStencilView(device, depthBuffer);
		context.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
	}

	static void SetBackfaceCulling()
	{
		context.Rasterizer.State = new RasterizerState(device, new RasterizerStateDescription()
		{
			CullMode = form.checkBoxBackfaceCulling.Checked ? CullMode.Back : CullMode.None,
			FillMode = FillMode.Solid,
			IsFrontCounterClockwise = true
		});
	}
}