using Microsoft.Win32;
using SharpDX.Direct3D;
using SharpDX.DirectComposition;
using Device = SharpDX.DXGI.Device;

public abstract class SwapChainManager
{
	static Texture2D backBuffer;
	static DepthStencilView depth;
	static RenderTargetView outputRTV;
	static RenderTargetView hdrRTV;
	static RenderTargetView sdrRTV;
	static SwapChain3 swapChain;

	public static Format format = Format.R16G16B16A16_Float;

	public static void Render()
	{
		if (form != Form.ActiveForm) { Thread.Sleep(250); return; }
		Shaders.UpdateBuffers();

		c.ClearRenderTargetView(outputRTV, new());
		c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);

		if (form.RenderSpheres.Checked)
			ShaderSpheres.Render();
		else
		{
			Selected.ForEach(ShaderOutline.Render);
			Meshes.Except(Hidden).Where(x => !x.Transparent).ToList().ForEach(Mesh.Render);
			Meshes.Except(Hidden).Where(x => x.Transparent).ToList().ForEach(Mesh.Render);	  // We render transparent meshes only after all the opaque
		}

		c.ResolveSubresource(outputRTV.Resource, 0, backBuffer, 0, format);
		swapChain.Present(1, PresentFlags.None);
	}

	public static void Initialize()
	{
		form.SizeChanged += (s, e) => ProcessResize();

		form.hdrEnabled.CheckedChanged += (s, e) => SetupHDR();

		// BgraSupport is required for Direct Composition
		dev = new(DriverType.Hardware, DeviceCreationFlags.BgraSupport);
		c = dev.ImmediateContext;

		// Swapchain, buffers and viewport should be configured exactly like this, to achieve best performance.
		// I don't know why, but it is what it is. Other ways are slower or won't paint on entire form
		var swapChainDescription1 = new SwapChainDescription1()
		{
			BufferCount = 2,
			Width = 1, Height = 1,
			Format = format,
			SampleDescription = new(1, 0),
			SwapEffect = SwapEffect.FlipDiscard,
			Usage = Usage.RenderTargetOutput,
			AlphaMode = AlphaMode.Premultiplied,
		};

		var swapChain1 = new SwapChain1(new Factory4(), dev, ref swapChainDescription1);
		swapChain = swapChain1.QueryInterface<SwapChain3>();

		ProcessResize();

		// Viewport should be always at screen maximum resolution regardless of form size
		c.Rasterizer.SetViewport(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
		
		CreateRenderTargets();
		DirectCompositionStuff();
		SetupHDR();
	}

	// Only SwapChain and its backbuffers are resized with form. Projection matrix should also be updated
	static void ProcessResize()
	{
		if (form.WindowState == FormWindowState.Minimized) return;

		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, form.ClientSize.Width, form.ClientSize.Height, format, SwapChainFlags.AllowModeSwitch);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		vsData.Proj = SharpDX.Matrix.PerspectiveFovLH(Pi / 6, form.ClientSize.Width / (float)form.ClientSize.Height, 0.01f, 100f);
	}

	static void SetupHDR()
	{
		RegistryKey parent = Registry.LocalMachine.OpenSubKey(@"SYSTEM\ControlSet001\Control\GraphicsDrivers\MonitorDataStore");
		RegistryKey key = parent?.OpenSubKey(parent?.GetSubKeyNames().First());

		// Microsoft calls it "HDR/SDR brightness balance" in settings and "SDR white level" in registry, but in reality it is Exposure
		float hdrExposure = (int)key?.GetValue("SDRWhiteLevel") / 1000f;
		bool hdrEnabled = (int)key?.GetValue("AdvancedColorEnabled") == 1;

		format = form.hdrEnabled.Checked ? Format.R16G16B16A16_Float : Format.R8G8B8A8_UNorm;

		backBuffer?.Dispose();
		swapChain.ResizeBuffers(0, form.ClientSize.Width, form.ClientSize.Height, format, SwapChainFlags.AllowModeSwitch);
		backBuffer = swapChain.GetBackBuffer<Texture2D>(0);

		if (form.hdrEnabled.Checked)
		{
			form.Gamma.Maximum = (decimal)1.2;
			form.Gamma.Value = 1;
			form.Exposure.Value = hdrEnabled ? (decimal)hdrExposure : 1;
			outputRTV = hdrRTV;
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG10NoneP709;
		}
		else
		{
			form.Gamma.Maximum = (decimal)2.5;
			form.Gamma.Value = (decimal)2.2f; // 2.025f;
			form.Exposure.Value = 1;
			outputRTV = sdrRTV;
			swapChain.ColorSpace1 = ColorSpaceType.RgbFullG22NoneP709;
		}

		c.OutputMerger.SetTargets(depth, outputRTV);
	}

	static void CreateRenderTargets()
	{
		// Just as with viewport, this should be at screen maximum resolution regardless of form size
		Texture2DDescription description = new Texture2DDescription
		{
			Width = Screen.PrimaryScreen.Bounds.Width,
			Height = Screen.PrimaryScreen.Bounds.Height,
			SampleDescription = new(8, 0),
			MipLevels = 1,
			ArraySize = 1,
			BindFlags = BindFlags.RenderTarget
		};

		// Create MSAA render target view
		sdrRTV = new(dev, new Texture2D(dev, description with { Format = Format.R8G8B8A8_UNorm }));
		hdrRTV = new(dev, new Texture2D(dev, description with { Format = Format.R16G16B16A16_Float }));

		// Create depth stencil view
		description.Format = Format.D32_Float;
		description.BindFlags = BindFlags.DepthStencil;
		depth = new(dev, new Texture2D(dev, description));
	}

	static void DirectCompositionStuff()
	{
		DesktopDevice directCompositionDevice = new(dev.QueryInterface<Device>());
		var visual = new Visual2(directCompositionDevice) { Content = swapChain };
		Target.FromHwnd(directCompositionDevice, form.Handle, true).Root = visual;
		directCompositionDevice.Commit();
	}
}