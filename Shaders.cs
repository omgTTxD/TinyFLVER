using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using Format = SharpDX.DXGI.Format;
namespace TinyFLVER;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex(SoulsFormats.FLVER.Vertex v)
{
	public Vector3 Position = v.Position;
	public Vector3 Normal = v.Normal;
	public Vector3 UV = v.UVs[0];
	public Vector4 Tangent = v.Tangents[0];
}

struct Params(float exposure, float gamma)
{
	public float exposure = exposure;
	public float gamma = gamma;
}

public class Shaders
{
	static Device device = SwapChainManager.device;
	static DeviceContext context = SwapChainManager.context;

	static PixelShader PS;
	static VertexShader VS;

	static Buffer buffer;

	public static void Initialize(MainForm form)
	{
		Directory.SetCurrentDirectory(AppContext.BaseDirectory + "Shaders");
		var shaders = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.hlsl").Select(x => Path.GetFileName(x)).ToList();

		form.comboBoxShaders.Items.AddRange(shaders.FindAll(x => x != "Outline.hlsl" && x != "VertexShader.hlsl").ToArray());

		form.comboBoxShaders.SelectedIndexChanged += (_, _) => { CompileMainShader(form.comboBoxShaders.Text); };
		form.comboBoxShaders.SelectedIndex = 2;

		buffer = new Buffer(device, new BufferDescription(16, BindFlags.ConstantBuffer, ResourceUsage.Default));
		context.PixelShader.SetConstantBuffer(2, buffer);

	}

	public static void UpdateBuffers()
	{
		var par = new Params();
		context.UpdateSubresource(ref par, buffer);
	}

	public static void CompileMainShader(string shader)
	{
		ShaderBytecode byteCode = ShaderBytecode.CompileFromFile("VertexShader.hlsl", "VS", "vs_5_0");	
		VS = new VertexShader(device, byteCode);

		context.InputAssembler.InputLayout = new InputLayout(device, byteCode, [
			new ("POSITION", 0, Format.R32G32B32_Float, -1, 0),
			new ("NORMAL",   0, Format.R32G32B32_Float, -1, 0),
			new ("TEXCOORD", 0, Format.R32G32_Float, -1, 0),
			new ("TANGENT",  0, Format.R32G32B32A32_Float, -1, 0),
		]);

		PS = new PixelShader(device, ShaderBytecode.CompileFromFile(shader, "PS", "ps_5_0"));
	}

	public static void SetMainShaders()
	{
		context.VertexShader.Set(VS);
		context.PixelShader.Set(PS);
	}
}
