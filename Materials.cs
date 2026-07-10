using SharpAssimp;
using static SoulsFormats.PARAM;
using Semantic = SoulsFormats.FLVER.LayoutSemantic;
using Type = SoulsFormats.FLVER.LayoutType;

class Materials
{
	public static FLVER2.Material Common(Material m, out FLVER2.BufferLayout layout)
	{
		// Color is necessary for Elden Ring's C[AMSN] (and probably other materials), with index 1, otherwise it won't work.
		// NR works even without vertex colors. I wonder if order inside layout matters. It shouln't, since we have semantic, but I need to check it.
		layout = new() {
			new (Type.Float3, Semantic.Position),
			new (Type.UByte4, Semantic.Normal),
			new (Type.UByte4, Semantic.Tangent),
			new (Type.UByte4, Semantic.BoneIndices),
			new (Type.UByte4Norm, Semantic.BoneWeights),	
			new (Type.UByte4Norm, Semantic.VertexColor, 1),
			new (Type.Short4, Semantic.UV),
		};

		return CommonER(m);
	}

	public static FLVER2.Material Skin(Material m, out FLVER2.BufferLayout layout)
	{
		layout = new() {
			new (Type.Float3, Semantic.Position),
			new (Type.UByte4Norm, Semantic.BoneWeights),
			new (Type.UByte4, Semantic.BoneIndices),
			new (Type.UByte4, Semantic.Normal),
			new (Type.UByte4, Semantic.Tangent),
			new (Type.UByte4Norm, Semantic.VertexColor, 1),
			new (Type.Short4, Semantic.UV),
		};

		return SkinER(m);
	}

	public static FLVER2.Material CommonER(Material m)
	{
		return new FLVER2.Material(m.Name, "C[AMSN]_e.matxml", 0)
		{
			Index = flver.Materials.Count,
			Textures = {
			new() { Path = m.TextureDiffuse.FilePath ?? "",         Type = "C_AMSN__snp_Texture2D_2_AlbedoMap_0" },
			new() { Path = m.TextureNormal.FilePath ?? "",          Type = "C_AMSN__snp_Texture2D_7_NormalMap_4" },
			new() { Path = m.PBR.TextureMetalness.FilePath ?? "",   Type = "C_AMSN__snp_Texture2D_0_MetallicMap_0" } }
		};
	}

	public static FLVER2.Material SkinER(Material m)
	{
		return new FLVER2.Material(m.Name, "C[DetailBlend]_SSS.matxml", 0)
		{
			Index = flver.Materials.Count,
			Textures = {
			new() { Path = m.TextureDiffuse.FilePath ?? "", Type = "C_DetailBlend__SSS_snp_Texture2D_7_AlbedoMap" },
			new() { Path = m.TextureNormal.FilePath ?? "",  Type = "C_DetailBlend__SSS_snp_Texture2D_0_NormalMap" },
			new() { Path = "SYSTEX_White",      Type = "C_DetailBlend__SSS_snp_Texture2D_8_Mask1Map" },
			new() { Path = "SYSTEX_Dummy_m",    Type = "C_DetailBlend__SSS_snp_Texture2D_3_MetallicMap" },
			new() { Path = "AAT100_Skin_01_n",  Type = "C_DetailBlend__SSS_snp_Texture2D_6_NormalMap", Scale = new(20, 20) } }
		};
	}
}

