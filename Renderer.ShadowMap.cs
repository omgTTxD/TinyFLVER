public class ShadowMap
{
	public static PixelShader RenderDebugTexturePS, ShadowMapPS;
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
			Width = 2096, Height = 2096,
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
			Dimension = DepthStencilViewDimension.Texture2D,
			Texture2D = new() { MipSlice = 0 },
		});
	}

	public static void RenderShadowMap()
	{
		float dist = (cb.LightPos).Length();
		float r = 2;
		float fov = 2f * MathF.Atan(1 / dist) ;        // запас, чтоб не обрезать
		float near = MathF.Max(0.05f, dist - r);
		float far = dist + r;

		var view = Matrix4x4.CreateLookAtLeftHanded(cb.LightPos, new(), Vector3.UnitY);
		var proj = Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(fov , 1, near, far);
		//cb.LightViewProj = view * proj;
		SetAndClearRT(null, shadowMapDSV);

		cb.LightViewProj = LookAtLH(cb.LightPos) * Matrix4x4.CreateOrthographicLeftHanded(2, 2, 0.1f, 5);

		cb.ViewProjection = cb.LightViewProj;
		c.PixelShader.Set(ShadowMapPS);
		Mesh.Render(visibleMeshes);

		Camera.RecalculateMatrices();
		SetMainRT();
		c.PixelShader.SetShaderResource(17, shadowMapSRV);
	}

	public static void ViewShadowMap()
	{
		c.PixelShader.SetShaderResource(7, Shadow.pointShadowSRV);
		c.PixelShader.SetShaderResource(8, Subsurface.backBufferCopy);
		c.ResolveSubresource(msaaRT.Resource, 0, Subsurface.backBufferCopy.Resource, 0, Format.R16G16B16A16_Float);

		c.VertexShader.Set(RenderFullScreenQuadVS);
		c.PixelShader.Set(RenderDebugTexturePS);
		c.Draw(3, 0);
	}
}
