using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using SoulsFormats.Other.MWC;

struct ShaderData()
{
	public Matrix4x4 ViewProjection { get; set { field = value; Sync(); } }
	public Matrix4x4 LightViewProj;
	public vec3 LightPos;
	public vec1 Gamma, Exposure, F0, detailsScale;
	public vec1 FormatID, MeshID, DebugID, FlipX, FlipY;
	public int SwapXY, RecalcTan, IsDSMetal, IsSkin, Cubes;
	public vec3 CameraPos;

	public static void SetFormatID()
	{
		cb.FormatID = form.RenderSpheres.Checked ? -1
			: Flver.game.StartsWith("DS III") ? 3
			: Flver.game.StartsWith("Dark Souls") 
			? (Meshes.Any(m => m.normal?.Description.Format == Format.BC5_UNorm) ? 2 : 1) : 0;

		Sync();
	}
}

class Shaders
{
	public static string dir = "R:\\~src\\TinyFLVER\\Shaders\\"; //AppContext.BaseDirectory + "Shaders\\";

	public static PixelShader PS;
	public static VertexShader VS;
	public static PixelShader WhitePS = Shaders.CompilePS("WhitePS");
	static Dictionary<string, PixelShader> PSs = [];

	public static Buffer psBuffer;
	public static string shaderName => form.comboBoxShaders.Text;
	static FileSystemWatcher watcher;

	public static void SetMain()
	{
		c.PixelShader.Set(PS);
		c.VertexShader.Set(VS);
	}

	public static void Initialize()
	{
		psBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref cb, 512);
		c.VertexShader.SetConstantBuffer(0, psBuffer);
		c.PixelShader.SetConstantBuffer(0, psBuffer);

		SetAlphaBlending(true);

		InitializeShadersListboxes();

		var vsByteCode = ShaderBytecode.CompileFromFile(dir + "VS.hlsl", "VS", "vs_5_0");
		c.InputAssembler.InputLayout = new(d, vsByteCode, Vertex.layout);
		VS = new(d, vsByteCode);
		c.VertexShader.Set(VS);

		SetTextureSampler();
		SetBackfaceCulling(false);

		// Doesn't work?
		watcher = new FileSystemWatcher(dir, "PS.*.hlsl") { SynchronizingObject = form, EnableRaisingEvents = true };
		watcher.Renamed += (s, e) => { if (e.Name.EndsWith(".hlsl")) PS = PSs[e.Name] = CompilePS("PS", e.Name); };
	}
	public static void SetAlphaBlending(bool enable)
	{
		var desc = BlendStateDescription.Default() with { AlphaToCoverageEnable = enable };
		desc.RenderTarget[0].IsBlendEnabled = enable;
		desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;
		c.OutputMerger.SetBlendState(new(d, desc));
	}
	public static void SetTextureSampler()
	{
		c.PixelShader.SetSampler(0, new(d, SamplerStateDescription.Default() with {
			Filter = Filter.Anisotropic,
			AddressU = TextureAddressMode.Wrap,
			AddressV = TextureAddressMode.Wrap, 
		})); 

		c.PixelShader.SetSampler(1, new(d, SamplerStateDescription.Default() with
		{
			Filter = Filter.ComparisonMinMagLinearMipPoint,
			ComparisonFunction = Comparison.LessEqual,
			AddressU = TextureAddressMode.Border,
			AddressV = TextureAddressMode.Border,
			AddressW = TextureAddressMode.Border,
			BorderColor = new RawColor4(1, 1, 1, 1)   // за пределами карты считаем освещённым
		}));
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, RasterizerStateDescription.Default() with
		{
			CullMode = enable ? CullMode.Back : CullMode.None,
			IsFrontCounterClockwise = true,
		});
	}

	static void InitializeShadersListboxes()
	{
		var shaders = Directory.GetFiles(dir, "*.hlsl").Select(x => Path.GetFileName(x)).Where(x => x.StartsWith("PS.")).ToArray();

		form.comboBoxShaders.Items.AddRange(shaders); 

		form.comboBoxShaders.SelectedIndexChanged += (_, _) =>
		{
			if (!PSs.ContainsKey(shaderName))
				PSs[shaderName] = CompilePS("PS", shaderName);

			BindDebugList();
			PS = PSs[shaderName];
		};

		form.comboBoxDebugID.SelectedIndexChanged += (_, _) => cb.DebugID = form.comboBoxDebugID.SelectedIndex;
		
		BindDebugList();	
	}

	static void BindDebugList()
	{
		int index = form.comboBoxDebugID.SelectedIndex;
		
		if (shaderName.StartsWith("PS.PBR"))
			form.comboBoxDebugID.DataSource = new string[] {
			"Result", "SpecularDirect", "SpecularIBL", "F", "F_IBL", "DiffuseIBL", "SSS" };

		if (shaderName.StartsWith("PS..Normals"))
			form.comboBoxDebugID.DataSource = new string[] { "T.x", "T.y", "T.z", "B.x", "B.y", "B.z", "N.x", "N.y", "N.z", "T.w", "UV.x", "UV.y" };

		form.comboBoxDebugID.SelectedIndex = index < form.comboBoxDebugID.Items.Count ? index : form.comboBoxDebugID.Items.Count - 1;
	}

	public static PixelShader CompilePS(string name, string file = "AdditionalShaders.hlsl") => new(d, ShaderBytecode.CompileFromFile(dir + file, name, "ps_5_0", include: new IncludeHandler()));
	public static VertexShader CompileVS(string name, string file = "AdditionalShaders.hlsl") => new(d, ShaderBytecode.CompileFromFile(dir + file, name, "vs_5_0", include: new IncludeHandler()));

	public class IncludeHandler : Include
	{
		public Stream Open(IncludeType type, string fileName, Stream parentStream) => File.OpenRead(dir + fileName);
		public void Close(Stream stream) => stream.Close();
		public IDisposable Shadow { get; set; }
		public void Dispose() { }
	}
}