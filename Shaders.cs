public struct ShaderData()
{
	public Vector3 LightPosition;
	public float Gamma = 1;
	public float Exposure = 1;
	public float Time;
	public float Distance;
	public int newFormat;
}

public class Shaders
{

	static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref matrices);
	public static Buffer psBuffer = Buffer.Create<ShaderData>(d, BindFlags.ConstantBuffer, ref shaderData);

	public static PixelShader PS;
	public static VertexShader VS;

	public static string folderPath = AppContext.BaseDirectory + "Shaders\\";

	public static void Initialize()
	{
		foreach (var shader in Directory.GetFiles(folderPath, "PS.*.hlsl"))
			form.shadersList.Items.Add(Path.GetFileNameWithoutExtension(shader));
		form.shadersList.SelectedIndexChanged += (_, _) => { CompileMainShader(); };
		form.shadersList.SelectedIndex = 1;

		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);
		c.VertexShader.SetConstantBuffer(1, psBuffer);
		SetBackfaceCulling(false);
		SetAlphaBlending(true);
	}

	public static void UpdateBuffers()
	{
		shaderData.Time = Environment.TickCount / 1000f;
		shaderData.Distance = Camera.distance;
		c.UpdateSubresource(ref matrices, vsBuffer);
		c.UpdateSubresource(ref shaderData, psBuffer);
	}

	public static void CompileMainShader()
	{
		PS = new(d, ShaderBytecode.CompileFromFile(folderPath + form.shadersList.Text + ".hlsl", "PS", "ps_5_0"));
		var byteCode = ShaderBytecode.CompileFromFile(folderPath + "VS.hlsl", "VS", "vs_5_0"); 
		VS = new(d, byteCode); c.InputAssembler.InputLayout = new(d, byteCode, Vertex.layout);
		
		c.VertexShader.Set(VS);
		c.PixelShader.Set(PS);
	}

	public static void SetAlphaBlending(bool enable)
	{
		var desc = BlendStateDescription.Default() with { AlphaToCoverageEnable = enable, IndependentBlendEnable = enable };

		if (enable)
		{
			desc.RenderTarget[0].IsBlendEnabled = true;
			desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
			desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
			desc.RenderTarget[0].SourceAlphaBlend = BlendOption.SourceAlpha;
			desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
		}

		c.OutputMerger.BlendState = new BlendState(d, desc);
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new() { CullMode = enable ? CullMode.Back : CullMode.None, FillMode = SharpDX.Direct3D11.FillMode.Solid, IsFrontCounterClockwise = true, });
	}
}

