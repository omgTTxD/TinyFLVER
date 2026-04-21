public class ShaderDebug
{
	static string path = Shaders.folderPath + "Debug.hlsl";
	public static PixelShader PS = new(d, ShaderBytecode.CompileFromFile(path, "PS", "ps_5_0"));
	public static GeometryShader GS = new(d, ShaderBytecode.CompileFromFile(path, "GS", "gs_5_0"));

	public static void Render(Mesh m)
	{
		List<Vertex> vertices = [];
		int rate = 30;
		for (int i = 0; i < m.vertices.Count; i++)
			if (i % rate == 0 || i % rate == 1 || i % rate == 2)
				vertices.Add(m.vertices[i]);

		VertexBufferBinding vertexBufferBinding = new (Buffer.Create(d, BindFlags.VertexBuffer, vertices.ToArray()), Marshal.SizeOf<Vertex>(), 0);

	//	c.VertexShader.Set(Shaders.VS);
		c.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
		c.GeometryShader.Set(GS);
		c.PixelShader.Set(PS);
		c.Draw(vertices.Count, 0);
		c.GeometryShader.Set(null);
	}
}