using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectComposition;
using System.Security.Policy;
using static SoulsFormats.MATBIN;


struct Matrices
{
	public Matrix4x4 Model;
	public Matrix4x4 MVP;
}

struct ShaderData()
{
	public vec3 LightPosition;
	public vec Gamma { get; set { field = value; form.Gamma.Value = (int)(value * 1000); form.labelGamma.Text = value; Sync(); } }
	public vec Exposure { get; set { field = value; form.Exposure.Value = (int)(value * 1000); form.labelExposure.Text = value; Sync(); } }
	public float Time;
	public int Dynamic { get; set { field = value; Sync(); } }
	public int FormatID { get; set { field = value; Sync(); } }
	public int MeshID { get; set { field = value; Sync(); } }
	public int DebugID { get; set { field = value; Sync(); } }
	public int FlipX { get; set { field = value; Sync(); } }
	public int FlipY { get; set { field = value; Sync(); } }
	public int SwapXY { get; set { field = value; Sync(); } }
	public int RecalcTan { get; set { field = value; Sync(); } }
	int pad1, pad2;

	public static void SetFormatId()
	{
		if (!form.RenderSpheres.Checked)
			shaderData.FormatID = Flver.game.StartsWith("Dark Souls") ? 1 : Flver.game.StartsWith("BloodBorne") ? 2 : 0;
		else
			shaderData.FormatID = 3;
	}

	public static void Sync()
	{
		shaderData.Time = Environment.TickCount / 1500f;
		c.UpdateSubresource(ref M, Shaders.vsBuffer);
		c.UpdateSubresource(ref shaderData, Shaders.psBuffer);
	} 
}

class Shaders
{
	public static PixelShader PS;
	public static VertexShader VS;
	public static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref M);
	public static Buffer psBuffer = Buffer.Create<ShaderData>(d, BindFlags.ConstantBuffer, ref shaderData);

	public static void Initialize()
	{
		SetBackfaceCulling(false);
		SetAlphaBlending(true);

		form.Exposure.ValueChanged += (s, e) => shaderData.Exposure = form.Exposure.Value / 1000f;
		form.Gamma.ValueChanged += (s, e) => shaderData.Gamma = form.Gamma.Value / 1000f;

		form.FlipX.CheckedChanged += (s, e) => shaderData.FlipX = form.FlipX.Checked ? 1 : 0;
		form.FlipY.CheckedChanged += (s, e) => shaderData.FlipY = form.FlipY.Checked ? 1 : 0;
		form.SwapXY.CheckedChanged += (s, e) => shaderData.SwapXY = form.SwapXY.Checked ? 1 : 0;

		form.RecalculateTangents.CheckedChanged += (s, e) =>
		{
			shaderData.RecalcTan = form.RecalculateTangents.Checked ? 1 : 0;
		};


		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.VertexShader.SetConstantBuffer(1, psBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);

		var shaders = Directory.GetFiles(AppContext.BaseDirectory + "Shaders", "*.hlsl").Select(x => Path.GetFileName(x)).
			Where(x => x.StartsWith(".") || x.StartsWith("PS")).ToArray();
		form.comboBoxShaders.Items.AddRange(shaders);
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => PS = new (d, Compile(form.comboBoxShaders.Text, "PS"));
		form.comboBoxShaders.SelectedIndex = shaders.Length - 2;
		
		form.comboBoxCubemaps.Items.Add("None");
		form.comboBoxCubemaps.Items.AddRange(Directory.GetFiles(AppContext.BaseDirectory + "Textures\\Cubemaps\\", "*.dds").Select(x => Path.GetFileName(x)).ToArray());
		form.comboBoxCubemaps.SelectedIndexChanged += (_, _) =>
		{
			Mesh.cubeMap = CreateSRV(AppContext.BaseDirectory + "Textures\\Cubemaps\\" + form.comboBoxCubemaps.Text);
			c.PixelShader.SetShaderResources(0, Mesh.cubeMap);
		};
		form.comboBoxCubemaps.SelectedIndex = 1;

		var byteCode = Compile("VS.hlsl", "VS");
		c.InputAssembler.InputLayout = new(d, byteCode, Vertex.layout);
		VS = new(d, byteCode);
		c.VertexShader.Set(VS);

		form.comboBoxDebugID.SelectedIndexChanged += (_, _) => shaderData.DebugID = form.comboBoxDebugID.SelectedIndex; 
		form.comboBoxDebugID.SelectedIndex = 7;

		// Some imported textures have incorrect UVs. Blender and Elden Ring wraps them, so I should too. 
		c.PixelShader.SetSampler(1, new(d, new SamplerStateDescription
		{
			Filter = SharpDX.Direct3D11.Filter.Anisotropic,
			AddressU = TextureAddressMode.Wrap,
			AddressV = TextureAddressMode.Wrap,
			AddressW = TextureAddressMode.Wrap,
		}));
	}

	public static void SetMain()
	{
		c.VertexShader.Set(VS);
		c.PixelShader.Set(PS);
	}

	public static byte[] Compile(string name, string entry)
	{
		string profile = entry.Contains("PS") ? "ps_5_0" : "vs_5_0";
		return ShaderBytecode.CompileFromFile(AppContext.BaseDirectory + "Shaders\\" + name, entry, profile, include: new IncludeHandler());
	}

	public static void SetAlphaBlending(bool enable)
	{
		c.OutputMerger.BlendState = new BlendState(d, BlendStateDescription.Default() with {
			AlphaToCoverageEnable = enable, IndependentBlendEnable = enable });
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new()
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