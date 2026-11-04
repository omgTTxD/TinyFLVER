public class Subsurface
{
	static PixelShader psBlurH, psBlurV;
	static SRV srvA, srvB;
	static RenderTargetView rtvA, rtvB;
	static Texture2D texA, texB;

	public static void Initialize()
	{
		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();

		psBlurH = Shaders.CompilePS("BlurH");
		psBlurV = Shaders.CompilePS("BlurV");
	}

	public static (Texture2D tex, RenderTargetView rtv, SRV srv) CreateColorTarget()
	{
		var tex = new Texture2D(d, backBuffer.Description with {
			BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
		});

		return (tex, new RenderTargetView(d, tex), new ShaderResourceView(d, tex));
	}

	static void ProcessResize()
	{
		(texA, rtvA, srvA) = CreateColorTarget();
		(texB, rtvB, srvB) = CreateColorTarget();
	}

	public static void PostProcessing()
	{
		c.PixelShader.SetShaderResource(7, null);
		c.PixelShader.SetShaderResources(12, Outline.idsSRV);
		c.ResolveSubresource(msaaRT.Resource, 0, texA, 0, Format.R16G16B16A16_Float);
		c.VertexShader.Set(ShadowMap.RenderFullScreenQuadVS);
		SetAndClearRT(rtvB, null);
		c.PixelShader.SetShaderResource(8, srvA);
		c.PixelShader.Set(psBlurH);
		c.Draw(3, 0);

		SetAndClearRT(rtvA, null);
		c.PixelShader.SetShaderResource(8, srvB);
		c.PixelShader.Set(psBlurV);
		c.Draw(3, 0);

	}
}
