using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace TinyFLVER;

struct Settings(float CameraDistance = 0, float BackgroundColor = 0)
{
	public float CameraDistance = CameraDistance;
	public float BackgroundColor = BackgroundColor;
}

internal class OutlineEffect
{
	static VertexShader firstPass;
	static VertexShader secondPass;
	static PixelShader outlinePS;

	static Buffer settingsBuffer;
	static Device device = SwapChainManager.device;
	static DeviceContext context = SwapChainManager.context;

	static DepthStencilState stateNormal;
	static DepthStencilState stateAlwaysRewrite;

	static Settings settings;

	public static void Initialize()
	{
		firstPass = new VertexShader(device, ShaderBytecode.CompileFromFile("Outline.hlsl", "FirstPass", "vs_5_0"));
		secondPass = new VertexShader(device, ShaderBytecode.CompileFromFile("Outline.hlsl", "SecondPass", "vs_5_0"));
		outlinePS = new PixelShader(device, ShaderBytecode.CompileFromFile("Outline.hlsl", "OutlinePS", "ps_5_0"));

		settingsBuffer = new Buffer(device, new BufferDescription(16, BindFlags.ConstantBuffer, ResourceUsage.Default));
		context.VertexShader.SetConstantBuffer(1, settingsBuffer);

		settings = new Settings(BackgroundColor: SwapChainManager.bgColor);

		stateNormal = new DepthStencilState(device, new() { DepthComparison = Comparison.LessEqual, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All });
		stateAlwaysRewrite = new DepthStencilState(device, new() {	DepthComparison = Comparison.Always, IsDepthEnabled = true, DepthWriteMask = DepthWriteMask.All	});
	}

	public static void Render(Mesh mesh)
	{
		context.InputAssembler.SetVertexBuffers(0, mesh.vertexBufferBinding);
		context.InputAssembler.SetIndexBuffer(mesh.indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);

		context.PixelShader.Set(outlinePS);

		settings.CameraDistance = Camera.zoom;
		context.UpdateSubresource(ref settings, settingsBuffer);

		context.VertexShader.Set(firstPass);
		context.DrawIndexed(mesh.indices.Length, 0, 0);

		context.VertexShader.Set(secondPass);
		context.OutputMerger.SetDepthStencilState(stateAlwaysRewrite);
		context.DrawIndexed(mesh.indices.Length, 0, 0);
		context.OutputMerger.SetDepthStencilState(stateNormal);
	}
}
