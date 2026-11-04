using SharpDX.Direct3D11;

class Outline
{
	static VertexShader OutlineVS;
	public static PixelShader MeshIDsPS;

	public static RenderTargetView idsRT;
	public static DepthStencilView idsDepth;
	public static Texture2D idsTexture;
	public static SRV idsSRV;
	public static Texture2D idStaging;

	public static DepthStencilState stencilEnabled = CreateEnabledStencil();
	public static DepthStencilState stencilDisabled = new(d, DepthStencilStateDescription.Default());

	public static void Initialize()
	{
		
		OutlineVS = Shaders.CompileVS("OutlineVS");
		MeshIDsPS = Shaders.CompilePS("MeshIDsPS");

		idStaging = new(d, backBuffer.Description with
		{
			Width = 1, Height = 1,
			Format = Format.R32_Float,
			Usage = ResourceUsage.Staging,
			CpuAccessFlags = CpuAccessFlags.Read,
			BindFlags = BindFlags.None
		});

		c.MapSubresource(idStaging, 0, MapMode.Read, 0, out var idData);

		form.MouseMove += (s, e) => {
			if (e.Button.AnyOf(MouseButtons.None))
			{
				c.CopySubresourceRegion(idsTexture, 0, new(e.X, e.Y, 0, e.X + 1, e.Y + 1, 1), idStaging, 0);		
				nearestMeshIndex = (int)(Utilities.Read<float>(idData.DataPointer));

				if (nearestMeshIndex < 0)
					nearestMeshIndex *= -1;

				nearestMeshIndex -= 1;
			}
		};
		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();

	}

	static void ProcessResize()
	{
		// We are gonna read from this texture when masking subsurface for skin meshes only
		idsTexture = new Texture2D(d, backBuffer.Description with 
		{ 	
			Format = Format.R32_Float, 			
			BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget
		});

		idsSRV = new ShaderResourceView(d, idsTexture);
		idsRT = new RenderTargetView(d, idsTexture);
		idsDepth = CreateDepth(backBuffer.Description with { Format = Format.D32_Float });
	}

	public static DepthStencilState CreateEnabledStencil()
	{
		var op = new DepthStencilOperationDescription();
		op.Comparison = Comparison.NotEqual;
		op.FailOperation = StencilOperation.Keep;
		op.PassOperation = op.DepthFailOperation = StencilOperation.Replace;

		return new(d, DepthStencilStateDescription.Default() with {
			IsStencilEnabled = true,
			FrontFace = op,	BackFace = op,
		});
	}

	public static void RenderMeshIDs()
	{
		SetAndClearRT(idsRT, idsDepth);
		Mesh.Render([.. hiddenMeshes], MeshIDsPS);
		c.ClearDepthStencilView(idsDepth, DepthStencilClearFlags.Depth, 1, 0);
		Mesh.Render([.. visibleMeshes], MeshIDsPS);
	}

	public static void RenderOutline()
	{
		SetMainRT();
		c.ClearDepthStencilView(msaaDepth, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 1);
		c.OutputMerger.SetDepthStencilState(stencilEnabled);
		
		c.PixelShader.Set(null);
		Mesh.Render([.. selectedMeshes, nearestMesh]);
		Mesh.Render([.. selectedMeshes, nearestMesh], Shaders.WhitePS, OutlineVS);
		
		c.OutputMerger.SetDepthStencilState(stencilDisabled);
	}
}