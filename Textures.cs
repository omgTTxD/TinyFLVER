using Pfim;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SoulsFormats;
using System.Collections.Concurrent;

namespace TinyFLVER;

public abstract class Textures
{
	public static ShaderResourceView defaultDiffuseShaderResource = new(SwapChainManager.device, CreateTextureFromBytes([200, 200, 200, 255]));
	public static ShaderResourceView defaultNormalShaderResource = new(SwapChainManager.device, CreateTextureFromBytes([127, 127, 127, 255]));

	public static bool DiffuseSRgb = true;

	static ConcurrentDictionary<string, Texture2D> textures = [];

	public static Texture1D CreateTextureFromBytes(byte[] data) {
		var desc = new Texture1DDescription {
			Width = 1,
			ArraySize = 1,
			MipLevels = 1,
			Format = Format.R8G8B8A8_UNorm,
			BindFlags = BindFlags.ShaderResource
		};
		
		return new (SwapChainManager.device, desc, DataStream.Create(data, true, true));
	}

	public static Texture2D LoadDDS(FLVER2.Texture texture) {

		var textureName = Path.GetFileNameWithoutExtension(texture?.Path) + ".dds";
		string fullPath = Path.GetDirectoryName(Flver.path) + "\\" + textureName;

		if (!File.Exists(fullPath))
			return null;

		Format format = (DiffuseSRgb && textureName.Contains("_a")) ? Format.B8G8R8A8_UNorm_SRgb : Format.B8G8R8A8_UNorm;	
		string key = textureName + "|" + format;

		if (textures.ContainsKey(key))
			return textures[key];

		var image = Dds.Create(File.ReadAllBytes(fullPath), new PfimConfig());
		var desc = new Texture2DDescription
		{
			Width = image.Width,
			Height = image.Height,
			SampleDescription = new(1, 0),
			MipLevels = 1,
			ArraySize = 1,
			BindFlags = BindFlags.ShaderResource,
			Format = format,
		};

		if (textureName.Contains("_n"))
		{
			desc.MipLevels = 3;
			desc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
			desc.OptionFlags = ResourceOptionFlags.GenerateMipMaps;
		}
		
		var tex = new Texture2D(SwapChainManager.device, desc);
		SwapChainManager.context.UpdateSubresource(image.Data, tex, 0, image.Width * 4);	
		textures[key] = tex;

		return tex;
	}
}