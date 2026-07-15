using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

class Shadows
{
	public static ShaderResourceView shadows;
	public static RenderTargetView shadowsRT;
	public static Texture2D shadowsTex;
	public static DepthStencilView shadowsDepth;

	public static void Initialize()
	{
		form.ResizeEnd += (s, e) => ProcessResize();
		ProcessResize();
	}

	static void ProcessResize()
	{
		shadowsTex = CreateTex2D(backBuffer.Description with
		{
			Format = Format.R32_Typeless,
			BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
		});

		shadowsDepth = CreateDepth(Desc.Depth);
//		shadows = new SRV(d, shadowsTex);
	}

	public static void Render()
	{
		SetAndClearRT(shadowsRT, shadowsDepth);

	}
}

