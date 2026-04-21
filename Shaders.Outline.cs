public class ShaderOutline
{
	static string path = Shaders.folderPath + "Outline.hlsl";
	static VertexShader DrawOutline = new(d, ShaderBytecode.CompileFromFile(path, "VS1", "vs_5_0"));
	static VertexShader FillBackground = new(d, ShaderBytecode.CompileFromFile(path, "VS2", "vs_5_0"));
	static PixelShader PS = new(d, ShaderBytecode.CompileFromFile(path, "PS", "ps_5_0"));


	public static void Render(Mesh m)
	{
		shaderData.newFormat = m == nearestMesh ? 1 : 0;
		Shaders.UpdateBuffers();

		c.PixelShader.Set(PS);
		m.SetBuffers();

		c.VertexShader.Set(DrawOutline);
		c.DrawIndexed(m.indices.Length, 0, 0);

		SetDepthComparison(Comparison.Always);
		Shaders.SetAlphaBlending(false);

		c.VertexShader.Set(FillBackground);
		c.DrawIndexed(m.indices.Length, 0, 0);

		SetDepthComparison(Comparison.Less);
		Shaders.SetAlphaBlending(true);
	}

	static void SetDepthComparison(Comparison op)
	{
		c.OutputMerger.SetDepthStencilState(new(d, new() { DepthComparison = op, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All }));
	}
}