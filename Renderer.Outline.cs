using SharpDX;

class Outline 
{
	static string src = @"#include <VS.hlsl>
	float4 OutlineVS(In i) : SV_Position { return mul(float4(i.pos + i.normal * 0.001 * Model[3][2], 1), ModelViewProjection); }
	float4 OutlinePS() : SV_Target		 { return sRGB(1, 1); } 
	float PickingPS() : SV_Target		 { return MeshID; }";

	static VertexShader OutlineVS = new(d, ShaderBytecode.Compile(src, "OutlineVS", "vs_5_0", ShaderFlags.None, EffectFlags.None, null, include: new Shaders.IncludeHandler()));
	static PixelShader OutlinePS = new(d, ShaderBytecode.Compile(src, "OutlinePS", "ps_5_0", ShaderFlags.None, EffectFlags.None, null, include: new Shaders.IncludeHandler()));
	static PixelShader PickingPS = new(d, ShaderBytecode.Compile(src, "PickingPS", "ps_5_0", ShaderFlags.None, EffectFlags.None, null, include: new Shaders.IncludeHandler()));

	public static RenderTargetView idsRT;
	public static DepthStencilView idsDepth;
	public static Texture2D idStaging;
	public static DataStream idData;

	public static DepthStencilState stencilOutline;

	public static void Initialize()
	{
		idStaging = new(d, backBuffer.Description with {
			Width = 1, Height = 1,
			Format = Format.R32_Float,
			Usage = ResourceUsage.Staging,
			CpuAccessFlags = CpuAccessFlags.Read,
			BindFlags = BindFlags.None
		}); 

		c.MapSubresource(idStaging, 0, MapMode.Read, 0, out idData);
		stencilOutline = MakeOutlineState();

		form.MouseMove += SelectMeshUnderCursor;
	}

	public static DepthStencilState MakeOutlineState()
	{
		DepthStencilOperationDescription op = new DepthStencilOperationDescription();
		op.DepthFailOperation = op.PassOperation = StencilOperation.Replace;
		op.FailOperation = StencilOperation.Keep;
		op.Comparison = Comparison.GreaterEqual;

		return new(d, DepthStencilStateDescription.Default() with
		{
			IsStencilEnabled = true,
			FrontFace = op,
			BackFace = op,
		});
	}

	static void SelectMeshUnderCursor(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left || e.Button == MouseButtons.None)
		{
			c.CopySubresourceRegion(idsRT.Resource, 0, new(e.X, e.Y, 0, e.X + 1, e.Y + 1, 1), idStaging, 0);
			nearestMesh = Meshes.ElementAtOrDefault((int)(SharpDX.Utilities.Read<float>(idData.DataPointer)) - 1);
		}
	}

	public static void RenderMeshIDs()
	{
		Renderer.PrepareRT(idsRT, idsDepth);
		c.PixelShader.Set(PickingPS);

		Mesh.Render(hiddenMeshes);
		ClearDepth(idsDepth);
		Mesh.Render(visibleMeshes);

		Renderer.PrepareRT();
	}

	public static void RenderOutline()
	{
		DrawOutlinePass(selectedMeshes);
		if (nearestMesh is not null)
			DrawOutlinePass([nearestMesh]);
	}

	static void DrawOutlinePass(List<Mesh> meshes)
	{
		c.OutputMerger.SetDepthStencilState(stencilOutline, 2);
		c.VertexShader.Set(Shaders.VS);
		c.PixelShader.Set(null);
		Mesh.Render(meshes);

		c.OutputMerger.SetDepthStencilState(stencilOutline, 1);
		c.VertexShader.Set(OutlineVS);
		c.PixelShader.Set(OutlinePS);
		Mesh.Render(meshes);

		c.OutputMerger.SetDepthStencilState(stencilOutline, 0);
	}

	static void ClearDepth(DepthStencilView depth) => c.ClearDepthStencilView(depth, DepthStencilClearFlags.Depth, 1, 0);
}