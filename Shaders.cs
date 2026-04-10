public struct VSData()
{
	public Matrix Proj;
	public Matrix World;
}

public struct PSData() 
{
	public Vector3 LightPosition;
	public float Gamma { get; set; }
	public float Exposure { get; set; }
	public float Time;
	public int DebugID;
	public int newFormat;
}

public struct Vertex(FLVER.Vertex v)
{
	public Vector3 Position = v.Position;
	public Vector3 Normal = v.Normal;
	public Vector3 UV = v.UVs[0];
	public Vector4 Tangent = v.Tangents[0];
}

public class Shaders
{
	public static Buffer psBuffer;
	public static Buffer vsBuffer;

	public static PixelShader PS;
	public static VertexShader VS;

	public static void Initialize()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		var shaders = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.hlsl").Select(x => Path.GetFileName(x));
		form.comboBoxShaders.Items.AddRange(shaders.Where(fileName => fileName.StartsWith("PS.")).ToArray());
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => { CompileMainShader(); };
		form.comboBoxShaders.SelectedIndex = 1;

		SetAlphaBlending(true);
		SetBackfaceCulling(CullMode.None);

		vsBuffer = Buffer.Create<VSData>(dev, BindFlags.ConstantBuffer, ref vsData);
		c.VertexShader.SetConstantBuffer(0, vsBuffer);

		psBuffer = Buffer.Create(dev, BindFlags.ConstantBuffer, ref psData);
		c.PixelShader.SetConstantBuffer(1, psBuffer);
	}

	public static void UpdateBuffers()
	{
		psData.Time = Environment.TickCount / 1000f;
		c.UpdateSubresource(ref vsData, vsBuffer);
		c.UpdateSubresource(ref psData, psBuffer);
	}

	public static void CompileMainShader()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		ShaderBytecode byteCode = ShaderBytecode.CompileFromFile("VS.hlsl", "VS", "vs_5_0");

		c.InputAssembler.InputLayout = new(dev, byteCode, [
			new ("POSITION", 0, Format.R32G32B32_Float, -1, 0),
			new ("NORMAL",   0, Format.R32G32B32_Float, -1, 0),
			new ("TEXCOORD", 0, Format.R32G32_Float, -1, 0),
			new ("TANGENT", 0, Format.R32G32B32A32_Float, -1, 0),
		]);

		VS = new(dev, byteCode);
		PS = new(dev, ShaderBytecode.CompileFromFile(form.comboBoxShaders.Text, "PS", "ps_5_0"));
	}

	public static void SetAlphaBlending(bool enable)
	{
		var desc = BlendStateDescription.Default();
		desc.AlphaToCoverageEnable = enable;
		desc.IndependentBlendEnable = enable;

		if (enable)
		{
			desc.RenderTarget[0].IsBlendEnabled = true;
			desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
			desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
			desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
		}

		c.OutputMerger.BlendState = new BlendState(dev, desc);
	}

	public static void SetBackfaceCulling(CullMode mode)
	{
		c.Rasterizer.State = new(dev, new()
		{
			CullMode = mode,
			FillMode = SharpDX.Direct3D11.FillMode.Solid,
			IsFrontCounterClockwise = true,
		}
		);
	}
}
