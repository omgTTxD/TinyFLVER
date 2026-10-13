struct Matrices
{
	public Matrix4x4 MVP;
	public Matrix4x4 Model;
}

struct ShaderData()
{
	public vec3 LightPosition;
	public float Gamma;
	public float Exposure;
	public int FormatID;
	public int MeshID;
	public int DebugID;
	public int FlipX, FlipY, FlipZ;
	public int SwapXY;
	public int RecalcTan;
	public int IsSkin; 
	public int IsMetal;

	public static void SetFormatID()
	{
		cb.FormatID = form.RenderSpheres.Checked ? -1 
			: Flver.game.StartsWith("DS III") ? 3
			: Flver.game.StartsWith("Dark Souls") ? (Meshes.Any(m => m.normal?.Description.Format == Format.BC5_UNorm) ? 2 : 1) : 0;
		Sync();
	}

	public static void Sync()
	{
		c.UpdateSubresource(ref M, Shaders.vsBuffer);
		c.UpdateSubresource(ref cb, Shaders.psBuffer);
	}
}

class Shaders
{
	public static string dir = AppContext.BaseDirectory + "Shaders\\";
	public static PixelShader PS;
	public static VertexShader VS;
	public static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref M);
	public static Buffer psBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref cb, 64);
	static Dictionary<string, PixelShader> PSs = [];

	public static string shaderName => form.comboBoxShaders.Text;
	
	public static void Initialize()
	{
		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);

		form.FlipX.CheckedChanged += (s, e) => { cb.FlipX = form.FlipX.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.FlipY.CheckedChanged += (s, e) => { cb.FlipY = form.FlipY.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.FlipZ.CheckedChanged += (s, e) => { cb.FlipZ = form.FlipZ.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.SwapXY.CheckedChanged += (s, e) => { cb.SwapXY = form.SwapXY.Checked ? 1 : 0; form.comboBoxShaders.Focus(); };
		form.RecalculateTangents.CheckedChanged += (s, e) => { 
			cb.RecalcTan = form.RecalculateTangents.Checked ? 1 : 0; 
			form.comboBoxShaders.Focus();
		};

		cb.FlipX = 1;
		cb.FlipY = 1;
		cb.FlipZ = 1;
		cb.Exposure = 1;
		cb.Gamma = 1;

		InitializeShadersListboxes();

		var vsByteCode = ShaderBytecode.CompileFromFile(dir + "VS.hlsl", "VS", "vs_5_0", ShaderFlags.None);
		c.InputAssembler.InputLayout = new(d, vsByteCode, Vertex.layout);
		VS = new(d, vsByteCode);

		SetTextureSampler();
		SetBackfaceCulling(true);
		SetAlphaBlending(true);
	}

	public static void SetTextureSampler()
	{
		c.PixelShader.SetSampler(0, new(d, SamplerStateDescription.Default() with
		{
			Filter = Filter.Anisotropic,
			AddressU = TextureAddressMode.Wrap,
			AddressV = TextureAddressMode.Wrap,
		})); 
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new()
		{
			CullMode = enable ? CullMode.Back : CullMode.None,
			FillMode = FillMode.Solid,
			IsDepthClipEnabled = false,
			IsFrontCounterClockwise = true
		});
	}

	public static void SetAlphaBlending(bool enable)
	{
		c.OutputMerger.BlendState = new BlendState(d, BlendStateDescription.Default() with { AlphaToCoverageEnable = enable });
	}

	static void InitializeShadersListboxes()
	{
		var shaders = Directory.GetFiles(dir, "*.hlsl").Select(x => Path.GetFileName(x)).Where(x => x.StartsWith("PS.")).ToArray();

		form.comboBoxShaders.Items.AddRange(shaders); 

		form.comboBoxShaders.SelectedIndexChanged += (_, _) =>
		{
			try
			{
				if (!PSs.ContainsKey(shaderName))
					PSs[shaderName] = new(d, ShaderBytecode.CompileFromFile(dir + shaderName, "PS", "ps_5_0", include: new IncludeHandler()));
			
			PS = PSs[shaderName];
			}
			catch (Exception) { }
			;
			BindDebugList();
			Shaders.SetMain();
		};

		form.comboBoxDebugID.SelectedIndexChanged += (_, _) => cb.DebugID = form.comboBoxDebugID.SelectedIndex;
		
		BindDebugList();
		form.comboBoxDebugID.SelectedIndex = 0;
	}

	public static void SetMain()
	{
		c.VertexShader.Set(VS);
		c.PixelShader.Set(PS);
	}

	static void BindDebugList()
	{
		int index = form.comboBoxDebugID.SelectedIndex;
		
		if (shaderName.StartsWith("PS.PBR"))
			form.comboBoxDebugID.DataSource = new string[] {
			"Result", "Fresnel", "Direct", "IBL", "SpecularColor", "Distribution", "Geometry", "Cubemap", "SpecularFull", "DiffuseColor", "NdotL", "Irradiance", "DiffuseFull",
			"Metalness", "Roughness", "F0", "Subsurface"};

		if (shaderName.StartsWith("PS.Normals"))
			form.comboBoxDebugID.DataSource = new string[] { "T.x", "T.y", "T.z", "B.x", "B.y", "B.z", "N.x", "N.y", "N.z", "T.w", "UV.x", "UV.y" };

		form.comboBoxDebugID.SelectedIndex = index < form.comboBoxDebugID.Items.Count ? index : form.comboBoxDebugID.Items.Count - 1;
	}

	public class IncludeHandler : Include
	{
		public Stream Open(IncludeType type, string fileName, Stream parentStream) => File.OpenRead(dir + fileName);
		public void Close(Stream stream) => stream.Close();
		public IDisposable Shadow { get; set; }
		public void Dispose() { }
	}
}