struct Matrices
{
	public Matrix4x4 Model;
	public Matrix4x4 MVP;
}

struct ShaderData()
{
	public vec3 LightPosition;
	public vec Gamma { get; set { field = value; form.Gamma.Value = (int)(value * 1000); form.labelGamma.Text = value; } }
	public vec Exposure { get; set { field = value; form.Exposure.Value = (int)(value * 1000); form.labelExposure.Text = value; } }
	public float Time;
	public int DynamicOutline;
	public int OldFormat;
}

public struct Vertex(FLVER.Vertex v)
{
	public vec3 Position = v.Position;
	public vec3 Normal = v.Normal;
	public vec2 UV = ((vec3)v.UVs[0]).xy;

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32_Float, 0),
		new("NORMAL", 0, Format.R32G32B32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32_Float, 0)
	];
}

class ShadersData
{
	static Buffer vsBuffer = Buffer.Create(d, BindFlags.ConstantBuffer, ref M);
	static Buffer psBuffer = Buffer.Create<ShaderData>(d, BindFlags.ConstantBuffer, ref shaderData);

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