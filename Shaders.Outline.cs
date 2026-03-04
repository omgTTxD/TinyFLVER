namespace TinyFLVER;

public struct OutlineData
{
	public float CameraDistance;
	public float Time;
	public float bgColor;
}

public class ShaderOutline
{
	public static VertexShader DrawOutline;
	public static VertexShader FillBackground;
	public static PixelShader PS;

	static OutlineData data;
	static Buffer dataBuffer;
	static BlendState opaque = new BlendState(dev, BlendStateDescription.Default());

	public static void Initialize()
	{
		DrawOutline = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "VS1", "vs_5_0"));
		FillBackground = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "VS2", "vs_5_0"));
		PS = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "PS", "ps_5_0"));

		dataBuffer = Buffer.Create(dev, BindFlags.ConstantBuffer, ref vsData);
		c.VertexShader.SetConstantBuffer(1, dataBuffer);
	}

	public static void Render(Mesh m)
	{
		data.CameraDistance = Camera.distance;
		data.Time = Environment.TickCount / 1000f;
		data.bgColor = SwapChainManager.bg;
		c.UpdateSubresource(ref data, dataBuffer);

		c.InputAssembler.SetVertexBuffers(0, m.vertexBuffer);
		c.PixelShader.Set(PS);

		c.VertexShader.Set(DrawOutline);
		SwapChainManager.SetBackfaceCulling(CullMode.Back);
		c.Draw(m.vertices.Length, 0);

		c.OutputMerger.SetBlendState(opaque);
		c.VertexShader.Set(FillBackground);
		SetDepthComparison(Comparison.Always);
		c.Draw(m.vertices.Length, 0);

		SetDepthComparison(Comparison.Less);
		SwapChainManager.EnableAlphaBlending();
		SwapChainManager.SetBackfaceCulling(CullMode.None);
	}

	public static void SetDepthComparison(Comparison op)
	{
		c.OutputMerger.SetDepthStencilState(new(dev, new() { DepthComparison = op, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All }));
	}
}
