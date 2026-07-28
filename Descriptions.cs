using Semantic = SoulsFormats.FLVER.LayoutSemantic;
using Type = SoulsFormats.FLVER.LayoutType;

class Desc
{
	public static string[] debugPBR =   { "Result", "Fresnel", "FresnelIBL", "Distribution", "Geometry","Cubemap", "Irradiance", "SpecularColor", "Specular", "SpecularIBL",  "SpecularFull","DiffuseColor", "Diffuse", "DiffuseIBL",  "DiffuseFull" };

	public static string[] debugNormals = {
		"T.x.Flver", "T.x.Recalc", "T.y.Flver", "T.y.Recalc", "T.z.Flver", "T.z.Recalc", 
		"B.x.Flver", "B.x.Recalc", "B.y.Flver", "B.y.Recalc", "B.z.Flver", "B.z.Recalc",
			"finalN.x.Flver", "finalN.x.Recalc", "finalN.y.Flver", "finalN.y.Recalc", "finalN.z.Flver", "finalN.z.Recalc","uv.x", "uv.y","T.w.Flver", "T.w.Recalc"};

	public static FLVER2.BufferLayout bufferLayout => new () {
			new (Type.Float3, Semantic.Position),
			new (Type.UByte4, Semantic.Normal),
			new (Type.UByte4, Semantic.Tangent),
			new (Type.Short4, Semantic.UV),
			new (Type.UByte4, Semantic.BoneIndices),
			new (Type.UByte4Norm, Semantic.BoneWeights),
			// Color is necessary for Elden Ring's C[AMSN] (and probably other materials), with index 1, otherwise it won't work.
			new (Type.UByte4Norm, Semantic.VertexColor, 1),
	};

	public static FLVER2.Material CommonER(SharpAssimp.Material m) {
		string met = m.PBR.TextureMetalness.FilePath ?? "";
		return new(m.Name, "C[AMSN]_e.matxml", 0) {


			Index = flver.Materials.Count,
			Textures = {
				new() { Path = m.TextureDiffuse.FilePath ?? "", Type = "C_AMSN__snp_Texture2D_2_AlbedoMap_0" },
				new() { Path = m.TextureNormal.FilePath ?? "", Type = "C_AMSN__snp_Texture2D_7_NormalMap_4" },
				new() { Path = m.PBR.TextureMetalness.FilePath ?? "", Type = "C_AMSN__snp_Texture2D_0_MetallicMap_0" } }
		}; }

	public static FLVER2.Material SkinER(SharpAssimp.Material m) => new (m.Name, "C[DetailBlend]_SSS.matxml", 0) 
	{
		Index = flver.Materials.Count,
		Textures = {
			new() { Path = m.TextureDiffuse.FilePath ?? "", Type = "C_DetailBlend__SSS_snp_Texture2D_7_AlbedoMap" },
			new() { Path = m.TextureNormal.FilePath ?? "",  Type = "C_DetailBlend__SSS_snp_Texture2D_0_NormalMap" },
			new() { Path = "SYSTEX_White",	  Type = "C_DetailBlend__SSS_snp_Texture2D_8_Mask1Map" },
			new() { Path = "SYSTEX_Dummy_m",	Type = "C_DetailBlend__SSS_snp_Texture2D_3_MetallicMap" },
			new() { Path = "AAT100_Skin_01_n",  Type = "C_DetailBlend__SSS_snp_Texture2D_6_NormalMap", Scale = new(20, 20) } }
	};
	


	public static Texture2DDescription Depth => backBuffer.Description with
	{
		Format = Format.D32_Float,
		BindFlags = BindFlags.DepthStencil
	};

	public static Texture2DDescription msaaRT => backBuffer.Description with
	{
		Width = w * Renderer.scale,
		Height = h * Renderer.scale,
		SampleDescription = new(Renderer.aaSamples, 0)
	};

	public static Texture2DDescription msaaDepth => msaaRT with
	{
		Format = Format.D32_Float,
		BindFlags = BindFlags.DepthStencil
	};

	public static Texture2DDescription R32 => backBuffer.Description with 
	{ 
		Format = Format.R32_UInt 
	};

	public static Texture2DDescription StagingMeshIDs => backBuffer.Description with
	{
		Width = 1,
		Height = 1,
		Format = Format.R32_UInt,
		Usage = ResourceUsage.Staging,
		CpuAccessFlags = CpuAccessFlags.Read,
		BindFlags = BindFlags.None
	};

	public static Texture2DDescription DDS(byte[] bytes, Format format) => new ()
	{
		Format = format,
		Height = BitConverter.ToInt32(bytes, 12),
		Width = BitConverter.ToInt32(bytes, 16),
		MipLevels = BitConverter.ToInt32(bytes, 28),
		BindFlags = BindFlags.ShaderResource,
		SampleDescription = new(1, 0),
		ArraySize = BitConverter.ToInt32(bytes, 136) == 4 ? 6 : 1,
		OptionFlags = BitConverter.ToInt32(bytes, 136) == 4 ? ResourceOptionFlags.TextureCube : ResourceOptionFlags.None
	};
}

