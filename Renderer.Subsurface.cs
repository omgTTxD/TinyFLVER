public class Subsurface
{
	static PixelShader psBlurH, psBlurV;
	public static SRV backBufferCopy;
	static RenderTargetView additionalRT;

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
		backBufferCopy = new SRV(d, new Texture2D(d, backBuffer.Description with
		{
			BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
		}));
		c.PixelShader.SetShaderResource(8, backBufferCopy);

		additionalRT = new RenderTargetView(d, backBuffer);
	}

	public static void PostProcessing()
	{
		c.PixelShader.SetShaderResource(7, null);
		c.PixelShader.SetShaderResources(12, Outline.idsSRV);
		c.VertexShader.Set(ShadowMap.RenderFullScreenQuadVS);
		
		c.ResolveSubresource(msaaRT.Resource, 0, backBufferCopy.Resource, 0, Format.R16G16B16A16_Float);	
		SetAndClearRT(additionalRT, null);
		c.PixelShader.Set(psBlurH);
		c.Draw(3, 0);

		c.CopyResource(backBuffer, backBufferCopy.Resource);
		SetMainRT();
		c.PixelShader.Set(psBlurV);
		c.Draw(3, 0);
	}
}
