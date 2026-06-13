class Shaders
{
	public static PixelShader PS;
	public static VertexShader VS;
	public static string folder = AppContext.BaseDirectory + "\\Shaders\\";

	public static void Initialize()
	{
		SetBackfaceCulling(false);
		SetAlphaBlending(true);

		var shaders = Directory.GetFiles(folder, "*.hlsl").Select(x => Path.GetFileName(x)).Except(new string[] { "VS.hlsl", "Common.hlsl", "Outline.hlsl" }).ToArray();
		form.comboBoxShaders.Items.AddRange(shaders);
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => CompilePixelShader();
		form.comboBoxShaders.SelectedIndex = shaders.Length - 1;

		var byteCode = ShaderBytecode.CompileFromFile(folder + "VS.hlsl", "VS", "vs_5_0", include: new FileIncludeHandler());
		VS = new(d, byteCode); 
		c.InputAssembler.InputLayout = new(d, byteCode, Vertex.layout);
		c.VertexShader.Set(new(d, byteCode));
	}

	public static void CompilePixelShader()
	{
		string shaderPath = folder + form.comboBoxShaders.SelectedItem.ToString();
		PS = new(d, ShaderBytecode.CompileFromFile(shaderPath, "PS", "ps_5_0", include: new FileIncludeHandler()));
		c.PixelShader.Set(PS);
	}

	public static void SetAlphaBlending(bool enable)
	{
		var desc = BlendStateDescription.Default() with { AlphaToCoverageEnable = enable, IndependentBlendEnable = enable };
		desc.RenderTarget[0].IsBlendEnabled = enable;
		desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
		desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
		desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
		desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
		c.OutputMerger.BlendState = new BlendState(d, desc);
	}

	static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new()
		{
			CullMode = enable ? CullMode.Back : CullMode.None,
			FillMode = SharpDX.Direct3D11.FillMode.Solid,
			IsFrontCounterClockwise = true,
		});
	}

	public class FileIncludeHandler : Include
	{
		public Stream Open(IncludeType type, string fileName, Stream parentStream) => File.OpenRead(folder + fileName);
		public void Close(Stream stream) => stream.Close();
		public IDisposable Shadow { get; set; }
		public void Dispose() { }
	}
}