public struct ShaderData()
{
	public vec3 LightPosition;
	public vec Gamma	{ get; set { field = value; form.Gamma.Value = (int)(value * 1000); form.labelGamma.Text = value; } } 
	public vec Exposure	{ get; set { field = value; form.Exposure.Value = (int)(value * 1000); form.labelExposure.Text = value; } }
	public float Time;
	public float Distance;
	public int flag;
}

public class Shaders
{
	static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref M);
	public static Buffer psBuffer = Buffer.Create<ShaderData>(d, BindFlags.ConstantBuffer, ref shaderData);

	public static PixelShader PS;
	public static VertexShader VS;

	public static void Initialize()
	{
		PS = new(d, ShaderBytecode.CompileFromFile(AppContext.BaseDirectory + "Shaders\\PS.hlsl", "PS", "ps_5_0"));
		var byteCode = ShaderBytecode.CompileFromFile(AppContext.BaseDirectory + "Shaders\\VS.hlsl", "VS", "vs_5_0");
		VS = new(d, byteCode); c.InputAssembler.InputLayout = new(d, byteCode, Vertex.layout);

		c.VertexShader.Set(VS);
		c.PixelShader.Set(PS);

		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);
		c.VertexShader.SetConstantBuffer(1, psBuffer);
		SetBackfaceCulling(true);
		SetAlphaBlending(true);
	}

	public static void UpdateBuffers()
	{
		shaderData.Time = Environment.TickCount / 1000f;
		c.UpdateSubresource(ref M, vsBuffer);
		c.UpdateSubresource(ref shaderData, psBuffer);
	}

	public static void SetAlphaBlending(bool enable)
	{
		c.OutputMerger.BlendState = new BlendState(d, BlendStateDescription.Default() with { 
			AlphaToCoverageEnable = enable, 
			IndependentBlendEnable = enable 
		});
	}

	public static void SetBackfaceCulling(bool enable)
	{
		c.Rasterizer.State = new(d, new() { 
			CullMode = enable ? CullMode.Back : CullMode.None, 
			FillMode = SharpDX.Direct3D11.FillMode.Solid, 
			IsFrontCounterClockwise = true, 
		});
	}
}

