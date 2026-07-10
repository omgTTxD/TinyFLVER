struct Matrices
{
	public Matrix4x4 Model;
	public Matrix4x4 MVP;
}

struct ShaderData()
{
	public vec3 LightPosition;
	public vec Gamma	{ get; set { field = value; form.Gamma.Value = (int)(value * 1000); form.labelGamma.Text = value; } }
	public vec Exposure { get; set { field = value; form.Exposure.Value = (int)(value * 1000); form.labelExposure.Text = value; } }
	public float Time;
	public int Dynamic	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int FormatID { get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int MeshID	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int DebugID	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int FlipX	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int FlipY	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	public int SwapXY	{ get; set { field = value; ShadersData.UpdateBuffers(); } }
	int pad1, pad2, pad3;
}

class ShadersData
{
	static Buffer vsBuffer = Buffer.Create(dev, BindFlags.ConstantBuffer, ref M);
	static Buffer psBuffer = Buffer.Create<ShaderData>(dev, BindFlags.ConstantBuffer, ref shaderData);

	public static void Initialize()
	{
		c.VertexShader.SetConstantBuffer(0, vsBuffer);
		c.VertexShader.SetConstantBuffer(1, psBuffer);
		c.PixelShader.SetConstantBuffer(1, psBuffer);
	}

	public static void UpdateBuffers()
	{
		shaderData.Time = Environment.TickCount / 1500f;
		c.UpdateSubresource(ref M, vsBuffer);
		c.UpdateSubresource(ref shaderData, psBuffer);
	}
}