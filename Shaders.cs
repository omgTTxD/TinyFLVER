using static SoulsFormats.MATBIN;

class Shaders
{
	public static PixelShader PS;

	public static VertexShader VS;

	public static void Initialize()
	{
		SetBackfaceCulling(false);
		SetAlphaBlending(true);

		var shaders = Directory.GetFiles(AppContext.BaseDirectory + "Shaders", "*.hlsl").Select(x => Path.GetFileName(x)).
			Where(x => x.StartsWith(".") || x.StartsWith("PS")).ToArray();
		form.comboBoxShaders.Items.AddRange(shaders);
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => PS = new (dev, Compile(form.comboBoxShaders.Text, "PS"));
		form.comboBoxShaders.SelectedIndex = shaders.Length - 2;
		
		form.comboBoxCubemaps.Items.AddRange(Directory.GetFiles(AppContext.BaseDirectory + "Textures\\Cubemaps\\", "*.dds").Select(x => Path.GetFileName(x)).ToArray());
		form.comboBoxCubemaps.Items.Add("None");
		form.comboBoxCubemaps.SelectedIndexChanged += (_, _) =>
		{
			Textures.cubeMap = Textures.LoadTexture(AppContext.BaseDirectory + "Textures\\Cubemaps\\" + form.comboBoxCubemaps.Text);
			c.PixelShader.SetShaderResources(0, Textures.cubeMap);
		};
		form.comboBoxCubemaps.SelectedIndex = 0;


		var byteCode = Compile("VS.hlsl", "VS");
		c.InputAssembler.InputLayout = new(dev, byteCode, Vertex.layout);
		VS = new(dev, byteCode);
		c.VertexShader.Set(VS);

		form.comboBoxDebugID.SelectedIndexChanged += (_, _) => shaderData.DebugID = form.comboBoxDebugID.SelectedIndex; 
		form.comboBoxDebugID.SelectedIndex = 0;

		// We change sampler to set wrapping, since some textures have incorrect UVs. Game wraps UV too.
		/*	var sampler = new SamplerState(dev, new SamplerStateDescription
			{
				//	Filter = Filter.MinMagMipLinear,
				Filter = Filter.Anisotropic,
				AddressU = TextureAddressMode.Wrap,
				AddressV = TextureAddressMode.Wrap,
				AddressW = TextureAddressMode.Wrap,
				MaximumAnisotropy = 16,
			});

			c.PixelShader.SetSampler(0, sampler);*/
	}

	public static byte[] Compile(string name, string entry)
	{
		string profile = entry.Contains("PS") ? "ps_5_0" : "vs_5_0";
		return ShaderBytecode.CompileFromFile(AppContext.BaseDirectory + "Shaders\\" + name, entry, profile, include: new IncludeHandler());
	}

	public static void SetAlphaBlending(bool enable)
	{
		var desc = BlendStateDescription.Default() with { AlphaToCoverageEnable = enable, IndependentBlendEnable = enable };
		desc.RenderTarget[0].IsBlendEnabled = enable;
		desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
		desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
	//	desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
	//	desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
		c.OutputMerger.BlendState = new BlendState(dev, desc);
	}

	static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(dev, new()
		{
			CullMode = enable ? CullMode.Back : CullMode.None,
			FillMode = SharpDX.Direct3D11.FillMode.Solid,
			IsFrontCounterClockwise = true,
		});
	}

	class IncludeHandler : Include
	{
		public Stream Open(IncludeType type, string fileName, Stream parentStream) => File.OpenRead(AppContext.BaseDirectory + "Shaders\\" + fileName);
		public void Close(Stream stream) => stream.Close();
		public IDisposable Shadow { get; set; }
		public void Dispose() { }
	}
}