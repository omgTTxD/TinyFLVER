public class ShaderOutline
{
	public static VertexShader DrawOutline;
	public static VertexShader FillBackground;
	public static PixelShader PS;

	public static void Initialize()
	{
		DrawOutline = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "VS1", "vs_5_0"));
		FillBackground = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "VS2", "vs_5_0"));
		PS = new(dev, ShaderBytecode.CompileFromFile("Outline.hlsl", "PS", "ps_5_0"));
		c.VertexShader.SetConstantBuffer(1, Shaders.psBuffer);
	}

	public static void Render(Mesh m)
	{
		c.PixelShader.Set(PS);
		c.InputAssembler.SetVertexBuffers(0, m.vertexBufferBinding);
		
		c.VertexShader.Set(DrawOutline);
		c.Draw(m.vertices.Count, 0);

		SetDepthComparison(Comparison.Always);
		Shaders.SetAlphaBlending(false);

		c.VertexShader.Set(FillBackground);
		c.Draw(m.vertices.Count, 0);

		SetDepthComparison(Comparison.Less);
		Shaders.SetAlphaBlending(true);
	}

	public static void SetDepthComparison(Comparison op)
	{
		c.OutputMerger.SetDepthStencilState(new(dev, new() { DepthComparison = op, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All }));
	}
}