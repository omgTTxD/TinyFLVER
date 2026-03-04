using System.Runtime.InteropServices;

namespace TinyFLVER;

public class Shaders
{
	public static Buffer psBuffer;
	public static Buffer vsBuffer;

	public static PixelShader PS;
	public static VertexShader VS;

	public static void Initialize()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		var shaders = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.hlsl").Select(x => Path.GetFileName(x));
		form.comboBoxShaders.Items.AddRange(shaders.Where(fileName => fileName.StartsWith("PS.")).ToArray());
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => { CompileMainShader(); };
		form.comboBoxShaders.SelectedIndex = 1;

		psData = new();
		
		psBuffer = new(dev, new(Marshal.SizeOf<PSData>(), BindFlags.ConstantBuffer, ResourceUsage.Default));
		c.PixelShader.SetConstantBuffer(0, psBuffer);

		vsBuffer = Buffer.Create(dev, BindFlags.ConstantBuffer, ref vsData);
		c.VertexShader.SetConstantBuffer(0, vsBuffer);
	}

	public static void UpdateBuffers()
	{
		psData.DebugID = form.debugList.SelectedIndex;
		c.UpdateSubresource(ref vsData, vsBuffer);
		c.UpdateSubresource(ref psData, psBuffer);
	}

	public static void CompileMainShader()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		ShaderBytecode byteCode = ShaderBytecode.CompileFromFile("VS.hlsl", "VS", "vs_5_0");
		c.InputAssembler.InputLayout = new(dev, byteCode, [
			new ("POSITION", 0, Format.R32G32B32_Float, -1, 0),
			new ("NORMAL",   0, Format.R32G32B32_Float, -1, 0),
			new ("TEXCOORD", 0, Format.R32G32_Float, -1, 0)]);

		VS = new(dev, byteCode);
		PS = new(dev, ShaderBytecode.CompileFromFile(form.comboBoxShaders.Text, "PS", "ps_5_0"));
	}
}
