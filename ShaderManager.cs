using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SoulsFormats.Other.MWC;
using System.Numerics;
using static SoulsFormats.MCP;
using Buffer = SharpDX.Direct3D11.Buffer;
using Format = SharpDX.DXGI.Format;

namespace TinyFLVER;

public class ShaderManager
{
	static PixelShader PS;
	static VertexShader VS;

	public static VertexShader outlineVS1;
	public static VertexShader outlineVS2;
	public static PixelShader outlinePS;

	public static Buffer bufL;
	public static Buffer bufGamma;

	public struct PSData
	{
		public Vector3 L;
		public float Gamma;
		public float Exposure;
	}

	public static PSData psData;
	static Buffer psBuf;

	public static void UpdateBuffers()
	{
		c.UpdateSubresource(ref psData, psBuf);
	}

	public static void Initialize()
	{
		PrepareShaders();

		psBuf = new(dev, new(32, BindFlags.ConstantBuffer, ResourceUsage.Default));
		c.PixelShader.SetConstantBuffer(0, psBuf);
	}

	public static void PrepareShaders()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		var shaders = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.fx").Select(x => Path.GetFileName(x));
		form.comboBoxShaders.Items.AddRange(shaders.Where(fileName => fileName != "VS.fx" && fileName != "Outline.fx").ToArray());
		form.comboBoxShaders.SelectedIndexChanged += (_, _) => { CompileMainShader(); };
		form.comboBoxShaders.SelectedIndex = 0;
		CompileMainShader();

		outlineVS1 = new(dev, ShaderBytecode.CompileFromFile("Outline.fx", "VS1", "vs_5_0"));
		outlineVS2 = new(dev, ShaderBytecode.CompileFromFile("Outline.fx", "VS2", "vs_5_0"));
		outlinePS = new(dev, ShaderBytecode.CompileFromFile("Outline.fx", "PS", "ps_5_0"));
	}


	public static void CompileMainShader()
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		ShaderBytecode byteCode = ShaderBytecode.CompileFromFile("VS.fx", "VS", "vs_5_0");
		c.InputAssembler.InputLayout = new(dev, byteCode, [
			new ("POSITION", 0, Format.R32G32B32_Float, -1, 0),
			new ("NORMAL",   0, Format.R32G32B32_Float, -1, 0),
			new ("TEXCOORD", 0, Format.R32G32_Float, -1, 0),
		]);
		VS = new(dev, byteCode);
		PS = new(dev, ShaderBytecode.CompileFromFile(form.comboBoxShaders.Text, "PS", "ps_5_0"));
	}


	static void SetDepthComparison(Comparison op)
	{
		c.OutputMerger.SetDepthStencilState(new(dev, new() { DepthComparison = op, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All }));
	}


	public static void Render(Mesh m)
	{
		c.VertexShader.Set(VS);
		c.PixelShader.Set(PS);
		c.InputAssembler.SetVertexBuffers(0, m.bufferBinding);
		c.PixelShader.SetShaderResources(0, m.diffuseBinding ?? Textures.defaultDiffuseSRV);
		c.PixelShader.SetShaderResource(1, m.normalBinding ?? Textures.defaultNormalSRV);
		c.PixelShader.SetShaderResource(2, m.sssBinding);
		c.Draw(m.Vs.Length, 0);
	}


	public static void RenderOutline(Mesh m)
	{
		c.PixelShader.Set(outlinePS);
		c.InputAssembler.SetVertexBuffers(0, m.bufferBinding);

		c.VertexShader.Set(outlineVS1);
		c.Draw(m.Vs.Length, 0);

		c.VertexShader.Set(outlineVS2);
		SetDepthComparison(Comparison.Always);
		c.Draw(m.Vs.Length, 0);
		SetDepthComparison(Comparison.Less);
	}
}
