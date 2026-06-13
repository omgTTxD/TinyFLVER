class ShaderOutline
{
	static string path = Shaders.folder + "Outline.hlsl";
	static VertexShader DrawOutline = new(d, ShaderBytecode.CompileFromFile(path, "DrawOutline", "vs_5_0", include: new Shaders.FileIncludeHandler()));
	static VertexShader FillBackground = new(d, ShaderBytecode.CompileFromFile(path, "FillBackground", "vs_5_0", include: new Shaders.FileIncludeHandler()));
	static PixelShader PS = new(d, ShaderBytecode.CompileFromFile(path, "PS", "ps_5_0", include: new Shaders.FileIncludeHandler()));

	public static void Render(Mesh m)
	{
		if (m is null) return;

		shaderData.DynamicOutline = m == nearestMesh && !SelectedMeshes.Contains(m) ? 1 : 0;
		ShadersData.UpdateBuffers();
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
		c.OutputMerger.SetDepthStencilState(new(d, new() { 
			DepthComparison = op, 
			IsDepthEnabled = true, 
			DepthWriteMask = DepthWriteMask.All 
		}));
	}
}