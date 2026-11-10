class Shadow
{
	const int CubeSize = 2096;
	public static Texture2D pointShadowCube;
	public static RTV[] pointShadowRTV = new RTV[6];
	public static DepthStencilView pointShadowDSV;
	public static SRV pointShadowSRV;
	static PixelShader PointShadowPS = Shaders.CompilePS("PointShadowPS");

	public static void Initialize()
	{
		pointShadowCube = new Texture2D(d, backBuffer.Description with
		{
			Width = CubeSize,
			Height = CubeSize,
			ArraySize = 6,
			Format = Format.R32_Float,
			BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
			OptionFlags = ResourceOptionFlags.TextureCube,
		});

		pointShadowSRV = new ShaderResourceView(d, pointShadowCube);

		for (int i = 0; i < 6; i++)
			pointShadowRTV[i] = new RenderTargetView(d, pointShadowCube,
				new RenderTargetViewDescription
				{
					Format = Format.R32_Float,
					Dimension = RenderTargetViewDimension.Texture2DArray,
					Texture2DArray = { MipSlice = 0, FirstArraySlice = i, ArraySize = 1 },
				});

		pointShadowDSV = CreateDepth(pointShadowCube.Description with
		{
			ArraySize = 1,
			Format = Format.D32_Float,
			OptionFlags = ResourceOptionFlags.None,
		});
	}

	static vec3[] faceDir = { Vector3.UnitX, -Vector3.UnitX, Vector3.UnitY, -Vector3.UnitY, Vector3.UnitZ, -Vector3.UnitZ };
	static vec3[] faceUp = { Vector3.UnitY, Vector3.UnitY, -Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitY, Vector3.UnitY };

	public static void RenderPointShadow()
	{
		c.PixelShader.Set(PointShadowPS);
		cb.ShadowRot = Invert(Matrix4x4.CreateLookAtLeftHanded(new(), new vec3(0, 1f, 0) - cb.LightPos, Vector3.UnitY));
		
		for (int i = 0; i < 6; i++)
		{
			SetAndClearRT(pointShadowRTV[i], pointShadowDSV);
			cb.ViewProjection = Matrix4x4.CreateLookAtLeftHanded(cb.LightPos, cb.LightPos + faceDir[i] * cb.ShadowRot, faceUp[i] * cb.ShadowRot)
				* Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(MathUtil.PiOverTwo, 1, 0.1f, 10);
			Mesh.Render(visibleMeshes);
		}
		
		Camera.RecalculateMatrices();
		SetMainRT();     
		c.PixelShader.SetShaderResource(7, pointShadowSRV);
	}
}