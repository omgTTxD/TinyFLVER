using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device1;

namespace TinyFLVER;

public abstract class SwapChainManager
{
	static Texture2D backBuffer;
	static RenderTargetView renderTargetView;
	static Texture2D msaaTexture;
	static RenderTargetView msaaRenderTargetView;

	static DepthStencilView depthStencilView;
	static SwapChain3 swapChain;

	public static float bgColor;
	public static Format format;

	public static void Render()
	{
		c.OutputMerger.SetRenderTargets(depthStencilView, msaaRenderTargetView);
		c.ClearRenderTargetView(msaaRenderTargetView, new (bgColor, bgColor, bgColor, 0));
		c.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1, 0);

		ShaderManager.UpdateBuffers();

		Selected.ForEach(x => ShaderManager.RenderOutline(x));
		Meshes.FindAll(m => !Hidden.Contains(m) && !m.Transparent).ForEach(x => ShaderManager.Render(x));
		Meshes.FindAll(m => !Hidden.Contains(m) && m.Transparent).ForEach(x => ShaderManager.Render(x));

		c.ResolveSubresource(msaaTexture, 0, backBuffer, 0, format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		form.checkBoxBackfaceCulling.CheckedChanged += (_, _) => SetBackfaceCulling();
		form.Resize += (_, _) => RecreateBuffers(); 

		form.swapchainFormats.DataSource = new[] {
			Format.R8G8B8A8_UNorm,
			Format.R8G8B8A8_UNorm_SRgb,
			Format.R10G10B10A2_UNorm,
			Format.R16G16B16A16_Float,
		};

		form.swapchainFormats.SelectedIndex = 1;
		format = (Format)form.swapchainFormats.SelectedItem;
		form.swapchainFormats.SelectedIndexChanged += (_, _) => { format = (Format)form.swapchainFormats.SelectedItem; RecreateBuffers(); };

		dev = new(DriverType.Hardware);
		c = dev.ImmediateContext;

		swapChain?.Dispose();
		var swapChainDescription = new SwapChainDescription()
		{
			BufferCount = 2,
			ModeDescription = new(format),
			SampleDescription = new(1, 0),
			OutputHandle = r.Handle,
			SwapEffect = SwapEffect.Discard,
			Usage = Usage.RenderTargetOutput
		};
		var swapChain0 = new SwapChain(new Factory2(), dev, swapChainDescription);
		swapChain = swapChain0.QueryInterface<SwapChain3>();

		EnableAlphaBlending();
		RecreateBuffers();
	}

	static void RecreateBuffers()
	{
		if (form.WindowState == FormWindowState.Minimized) return;

		// Resize viewport
		c.Rasterizer.SetViewport(new Viewport(0, 0, r.Width, r.Height));

		// SwapChain and Backbuffer
		Utilities.Dispose(ref backBuffer);
		try { swapChain.ResizeBuffers(0, 0, 0, format, SwapChainFlags.None); }
		catch { MessageBox.Show("Error while changing swapchains, probably because changed to SRgb with Flip model"); return; }
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		if (format == Format.R16G16B16A16_Float)
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG10NoneP709;
		else
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG22NoneP709;

		// MSAA
		msaaTexture = new Texture2D(dev, new Texture2DDescription() {
			Width = r.Width,
			Height = r.Height,
			Format = format,
			MipLevels = 1,
			ArraySize = 1,
			BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
			SampleDescription = new(8, 0),
		});
		msaaRenderTargetView = new (dev, msaaTexture);	

		// Depth Buffer
		depthStencilView = new(dev, new Texture2D(dev, new Texture2DDescription() {
			Width = r.Width,
			Height = r.Height,
			Format = Format.D32_Float,
			MipLevels = 1,
			ArraySize = 1,
			BindFlags = BindFlags.DepthStencil,
			// msaaTexture and depthTexture must have similar sample sount
			SampleDescription = msaaTexture.Description.SampleDescription
		}));

		if (format == Format.R16G16B16A16_Float || format == Format.R8G8B8A8_UNorm_SRgb)
		{
			bgColor = 0.0145f;
			form.Gamma.Value = 1;
		}
		else
		{
			bgColor = 0.12f;
			form.Gamma.Value = (decimal)2.2;
		}

		SetBackfaceCulling();
	}

	static void EnableAlphaBlending()
	{
		var desc = BlendStateDescription.Default();
		desc.RenderTarget[0].IsBlendEnabled = true;
		desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
		desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
		c.OutputMerger.BlendState = new BlendState(dev, desc);
	}


	static void SetBackfaceCulling()
	{
		c.Rasterizer.State = new (dev, new RasterizerStateDescription()
		{
			CullMode = form.checkBoxBackfaceCulling.Checked ? CullMode.Back : CullMode.None,
			FillMode = FillMode.Solid,
			IsFrontCounterClockwise = true
		});
	}
}