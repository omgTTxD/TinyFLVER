public class ShadowMap
{
	static PixelShader RenderDebugTexturePS, ShadowMapPS;
	public static VertexShader RenderFullScreenQuadVS;
	public static Texture2D shadowMapTexture;
	public static SRV shadowMapSRV;
	public static DepthStencilView shadowMapDSV;

	public static void Initialize()
	{
		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();

		ShadowMapPS = Shaders.CompilePS("ShadowMapPS");
		RenderDebugTexturePS = Shaders.CompilePS("RenderDebugTexturePS");
		RenderFullScreenQuadVS = Shaders.CompileVS("RenderFullScreenQuadVS");
	}

	public static void ProcessResize()
	{
		shadowMapTexture = new Texture2D(d, backBuffer.Description with
		{
			Width = w * 6, Height = h * 6,
			Format = Format.R32_Typeless,
			BindFlags = BindFlags.ShaderResource | BindFlags.DepthStencil,
		});

		shadowMapSRV = new ShaderResourceView(d, shadowMapTexture, new()
		{
			Format = Format.R32_Float,
			Dimension = ShaderResourceViewDimension.Texture2D,
			Texture2D = new() { MipLevels = 1 }
		});

		shadowMapDSV = new DepthStencilView(d, shadowMapTexture, new()
		{
			Format = Format.D32_Float,
			Dimension = DepthStencilViewDimension.Texture2D
		});
	}

	public static void RenderShadowMap()
	{
		SetAndClearRT(null, shadowMapDSV);
		cb.ViewProjection = cb.LightViewProj;
	
		c.PixelShader.Set(ShadowMapPS);
		c.VertexShader.Set(Shaders.VS);
		Mesh.Render(visibleMeshes);
		
		Camera.RecalculateMatrices();
		SetMainRT();
		Shaders.SetMain();
		c.PixelShader.SetShaderResource(7, shadowMapSRV);
	}

	public static void ViewShadowMap()
	{
		c.VertexShader.Set(RenderFullScreenQuadVS);
		c.PixelShader.Set(RenderDebugTexturePS);
		c.Draw(3, 0);
	}
}
