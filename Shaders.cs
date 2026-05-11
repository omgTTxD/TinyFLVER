public struct ShaderData()
{
	public Vector3 LightPosition;
	public vec Gamma	{ get; set { field = value; form.Gamma.Value = value; form.labelGamma.Text = value; } } 
	public vec Exposure	{ get; set { field = value; form.Exposure.Value = value; form.labelExposure.Text = value; } }
	public float Time;
	public float Distance;
	public int newFormat;
}

public class Shaders
{
	static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref M);
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
		c.UpdateSubresource(ref M, vsBuffer);
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

