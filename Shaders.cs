
struct ShaderData()
{
	public vec3 LightPosition;
	public vec1 Gamma { get; set { field = value; form.Gamma.Value = (int)(value * 1000); form.GammaValue.Text = value; Sync(); } }
	public vec1 Exposure { get; set { field = value; form.Exposure.Value = (int)(value * 1000); form.ExposureValue.Text = value; Sync(); } }
	public int FormatID { get; set { field = value; Sync(); } }
	public int MeshID { get; set { field = value; Sync(); } }
	public int DebugID { get; set { field = value; Sync(); } }
	public int FlipX, FlipY, FlipZ;
	public int SwapXY{ get; set { field = value; Sync(); } }
	public bool RecalcTan{ get; set { field = value; Sync(); } }
	public int IsSkin { get; set { field = value; Sync(); } }
	public int p1, p2;

	public static void Sync()
	{
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
	static Dictionary<string, PixelShader> PSs = [];
	
	public static void Initialize()
	{
		SetBackfaceCulling(false);

		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.VertexShader.SetConstantBuffer(1, psBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);

		form.Exposure.ValueChanged += (s, e) => shaderData.Exposure = form.Exposure.Value / 1000f;
		form.Gamma.ValueChanged += (s, e) => shaderData.Gamma = form.Gamma.Value / 1000f;

		form.FlipX.CheckedChanged += (s, e) => { shaderData.FlipX = form.FlipX.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.FlipY.CheckedChanged += (s, e) => { shaderData.FlipY = form.FlipY.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.FlipZ.CheckedChanged += (s, e) => { shaderData.FlipZ = form.FlipZ.Checked ? -1 : 1; form.comboBoxShaders.Focus(); };
		form.SwapXY.CheckedChanged += (s, e) => { shaderData.SwapXY = form.SwapXY.Checked ? 1 : 0; form.comboBoxShaders.Focus(); };
		form.RecalculateTangents.CheckedChanged += (s, e) => { 
			shaderData.RecalcTan = form.RecalculateTangents.Checked ? true : false; 
			form.comboBoxShaders.Focus();
		};

		shaderData.FlipX = 1;
		shaderData.FlipY = 1;
		shaderData.FlipZ = 1;

		InitializeShadersListboxes();
		InitializeCubemapListbox();

		c.InputAssembler.InputLayout = new(d, Compile("VS.hlsl", "VS"), Vertex.layout);
		VS = new(d, Compile("VS.hlsl", "VS"));
		c.VertexShader.Set(VS);

		c.PixelShader.SetSampler(0, new(d, SamplerStateDescription.Default() with
		{
			Filter = Filter.Anisotropic,
			AddressU = TextureAddressMode.Wrap,
			AddressV = TextureAddressMode.Wrap,
			//MipLodBias = 0.5f,
		}));
	}

	static void InitializeCubemapListbox()
	{
		form.comboBoxCubemaps.Items.Add("None");
		form.comboBoxCubemaps.Items.AddRange(Directory.GetFiles(AppContext.BaseDirectory + "Textures\\Cubemaps\\", "*.dds").Select(x => Path.GetFileName(x)).ToArray());
		form.comboBoxCubemaps.SelectedIndexChanged += (_, _) =>
		{
			Mesh.cubeMap = Textures.LoadTexture(AppContext.BaseDirectory + "Textures\\Cubemaps\\" + form.comboBoxCubemaps.Text);
			c.PixelShader.SetShaderResources(0, Mesh.cubeMap);
		};
		form.comboBoxCubemaps.SelectedIndex = 1;
	}

	static void InitializeShadersListboxes()
	{
		var shaders = Directory.GetFiles(AppContext.BaseDirectory + "Shaders", "*.hlsl").Select(x => Path.GetFileName(x)).
		Where(x => x.StartsWith("Normals") || x.StartsWith("PBR")).ToArray();

		form.comboBoxShaders.Items.AddRange(shaders); 
		form.comboBoxShaders.SelectedIndex = 0;

		form.comboBoxShaders.SelectedIndexChanged += (_, _) =>
		{
			var shaderName = form.comboBoxShaders.Text;

			if (!PSs.ContainsKey(shaderName))
				PSs[shaderName] = new(d, Compile(shaderName, "PS"));

			PS = PSs[shaderName];

			form.RecalculateTangents.Enabled = !shaderName.StartsWith("Normals");
			BindDebugList();
		};

		form.comboBoxDebugID.SelectedIndexChanged += (_, _) => shaderData.DebugID = form.comboBoxDebugID.SelectedIndex;
		form.comboBoxDebugID.SelectedIndex = 0;
		BindDebugList();
	}

	static void BindDebugList()
	{
		int index = form.comboBoxDebugID.SelectedIndex;
		var shaderName = form.comboBoxShaders.SelectedItem?.ToString();

		if (shaderName.StartsWith("PBR"))
			form.comboBoxDebugID.DataSource = Desc.debugPBR;

		if (shaderName.StartsWith("Normals"))
			form.comboBoxDebugID.DataSource = Desc.debugNormals;

		form.comboBoxDebugID.SelectedIndex = index < form.comboBoxDebugID.Items.Count ? index : form.comboBoxDebugID.Items.Count - 1;
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
		c.OutputMerger.BlendState = new BlendState(d, BlendStateDescription.Default() with
		{
			AlphaToCoverageEnable = enable,
			IndependentBlendEnable = enable
		});
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new()
		{
			CullMode = enable ? CullMode.Back : CullMode.None,
			FillMode = FillMode.Solid,
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