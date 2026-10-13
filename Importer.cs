using SharpAssimp;
using PPS = SharpAssimp.PostProcessSteps;
using Semantic = SoulsFormats.FLVER.LayoutSemantic;
using Type = SoulsFormats.FLVER.LayoutType;

class Importer
{
	public static void Initialize()
	{
		form.buttonImport.Click += (s, e) => form.importDialog.ShowDialog();
		form.importDialog.FileOk += (s, e) => Import(form.importDialog.FileName);
	}

	public static void Import(string path)
	{
		var scene = new AssimpContext().ImportFile(path, 
			PPS.Triangulate | PPS.FlipUVs | PPS.MakeLeftHanded | PPS.LimitBoneWeights | PPS.GenerateNormals | PPS.CalculateTangentSpace | PPS.JoinIdenticalVertices
		);

		flver.Meshes.Clear();
		flver.Materials.Clear();
		flver.BufferLayouts = Flver.game.StartsWith("Dark Souls") ? [BufferLayoutDSR] : Flver.game.StartsWith("DS III") ? [BufferLayoutDS3] : [BufferLayoutER];

		foreach (var mat in scene.Materials)
		{
			FLVER2.Material flverMat = new();

			if (Flver.game.StartsWith("Dark Souls"))
				flverMat = mat.Name.IsSkin() ? SkinDSR(mat) : CommonDSR(mat);

			else if (Flver.game.StartsWith("DS III"))
				flverMat = mat.Name.IsSkin() ? SkinDS3(mat) : CommonDS3(mat);

			else if (Flver.game.StartsWith("Elden Ring"))
				flverMat = mat.Name.IsSkin() ? SkinER(mat) : CommonER(mat);

			flverMat.Name = mat.Name;
			flverMat.Index = flver.Materials.Count;

			// *.glb imports texture names as "*1", "*2", etc... Also, sometimes normal'l path is "tex_n.dds.001" or "tex_n.dds-tex_m.dds" so we split that.
			foreach (var t in flverMat.Textures.Where(t => t is not null && t.Path.StartsWith("*")))
				t.Path = scene.Textures[int.Parse(t.Path.Substring(1))].Filename.Split(".")[0];

			flver.Materials.Add(flverMat);
		}

		foreach (var m in scene.Meshes)
		{
			FLVER2.Mesh flverMesh = new()
			{
				VertexBuffers = [new(0)],
				UseBoneWeights = true,
				MaterialIndex = m.MaterialIndex,
				BoundingBox = new(),
				FaceSets = [new() { Indices = [.. m.GetIndices()] }],
			};

			//   Several years ago, when I tried to import BDO models to DS3, I noticed a strange bug with normals. To make them work, I needed to 
			// swap X with -Y, which is very unusual (in 99% you just need to flip Y).
			//   At first I thought it was because of ZY axis switch during import in FLVER Editor. So I was very surprised when I encountered this exact
			// same bug again, even if I don't touch imported data.
			//   Then I thought that SoulsFormats reads T/B incorrectly. But this bug occurs because FLVER treats tangetns as bitangents. And to fix this
			// bug, we can either swap X/Y and flip new X in Photoshop, or swap them during import. We start with Bitangent and send it to FLVER as
			// Input.Tangent which will be saved as Bitangent in pixel shader. And to invert Tangent, we need to inverted Tw.			

			for (int i = 0; i < m.VertexCount; i++)
			{
				int Tw = Math.Sign(Vector3.Dot(Vector3.Cross(m.Normals[i], m.Tangents[i]), m.BiTangents[i]));

				flverMesh.Vertices.Add(new FLVER.Vertex()
				{
					Tangents = [new(m.BiTangents[i], -Tw)],
					Position = m.Vertices[i],
					Normal = m.Normals[i],
					UVs = [m.TextureCoordinateChannels[0][i], new()],
					Colors = [new(255, 255, 255, 255)],
				});
			}

			// We already normalized weights and limited bones affecting each vertice to 4 in preprocessing, so we can be sure that index won't overflow.
			int[] weightsCount = new int[m.VertexCount];
			m.Bones.ForEach(b => b.VertexWeights.ForEach(w =>
			{
				FLVER.Vertex v = flverMesh.Vertices[w.VertexID];
				v.BoneWeights[weightsCount[w.VertexID]] = w.Weight;
				v.BoneIndices[weightsCount[w.VertexID]] = flver.Nodes.FindIndex(n => n.Name == b.Name);
				weightsCount[w.VertexID]++;
			}));

			if (Flver.game.StartsWith("Dark Souls"))
			{
				m.Bones.ForEach(b => flverMesh.BoneIndices.Add(flverMesh.BoneIndices.Count));
				flverMesh.NodeIndex = m.Bones.Count;
			}
			
			flver.Meshes.Add(flverMesh);
		}

		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		Textures.ReloadAll();
	}

	public static FLVER2.BufferLayout BufferLayoutDSR => new() {
		new (Type.Float3, Semantic.Position),
		new (Type.UByte4, Semantic.BoneIndices),
		new (Type.Short4Norm, Semantic.BoneWeights),
		new (Type.UByte4Norm, Semantic.Normal),
		new (Type.UByte4Norm, Semantic.Tangent),
		new (Type.Short2, Semantic.UV)
	};

	public static FLVER2.BufferLayout BufferLayoutDS3 => new() {
		new (Type.Float3, Semantic.Position),
		new (Type.UByte4, Semantic.BoneIndices),
		new (Type.UByte4Norm, Semantic.BoneWeights),
		new (Type.UByte4, Semantic.Normal),
		new (Type.UByte4, Semantic.Tangent),
		new (Type.UByte4, Semantic.Tangent, 1),
		new (Type.Short4, Semantic.UV),
		new (Type.UByte4Norm, Semantic.UV, 1),
	};


	public static FLVER2.BufferLayout BufferLayoutER => new() {
		new (Type.Float3, Semantic.Position),
		new (Type.UByte4, Semantic.Normal),
		new (Type.UByte4, Semantic.Tangent),
		new (Type.Short4, Semantic.UV),
		new (Type.UByte4, Semantic.BoneIndices),
		new (Type.UByte4Norm, Semantic.BoneWeights),
		new (Type.UByte4Norm, Semantic.VertexColor, 1)		// Color is necessary for Elden Ring's C[AMSN], index must be 1
	};

	public static FLVER2.Material CommonDSR(Material m)
	{
		return new() { MTD = m.Name.Contains("Metal") ? "P_Metal[DSB]_Edge.mtd" : "P_Leather[DSB]_Edge.mtd", Textures = {
		new("g_Diffuse",			m.TextureDiffuse.FilePath),
		new("g_Specular",			m.TextureNormal.FilePath),
		new("g_Bumpmap",			m.PBR.TextureMetalness.FilePath),
		new("g_DetailBumpmap" ) } };
	}

	public static FLVER2.Material SkinDSR(Material m) 
	{
		return new() {	MTD = "Ps_Body[DSBT].mtd", Textures = {
		new("g_Diffuse",			m.TextureDiffuse.FilePath),
		new("g_Specular",			m.TextureNormal.FilePath),
		new("g_Bumpmap",			m.PBR.TextureMetalness.FilePath),
		new("g_DetailBumpmap" ) } };
	}

	public static FLVER2.Material CommonDS3(Material m)
	{
		return new() { MTD = m.Name.Contains("Metal") ? "P_BD[ARSN]_e.mtd" : "P_BD[ARSN]_e_Cloth.mtd", Textures = {
		new("SAT_Equip_snp_Texture2D_5_AlbedoMap_0",        m.TextureDiffuse.FilePath),
		new("SAT_Equip_snp_Texture2D_1_BlendMask",          "SYSTEX_DummyBurn_m"),
		new("SAT_Equip_snp_Texture2D_0_EmissiveMap_0",      "SYSTEX_DummyBurn_em"),
		new("SAT_Equip_snp_Texture2D_2_DamageNormal",       "SYSTEX_DummyDamagedNormal"),
		new("SAT_Equip_snp_Texture2D_4_NormalMap_0",        m.TextureNormal.FilePath),
		new("SAT_Equip_snp_Texture2D_3_ReflectanceMap_0",   m.TextureReflection.FilePath ) } };
	}

	public static FLVER2.Material SkinDS3(Material m)
	{
		return new() { MTD = "P[ChrCustom_Skin]_SSS.mtd", Textures = {
		new("g_DiffuseTexture",            m.TextureDiffuse.FilePath),
		new("g_SpecularTexture",           m.TextureSpecular.FilePath),
		new("g_ShininessTexture",          "texture_s"),
		new("g_BumpmapTexture",            m.TextureNormal.FilePath ),
		new("g_DetailBumpmapTexture",	   "SYSTEX_DummyNormal"),
		new("g_BloodMaskTexture",          "dummy128"),
		new("g_ScatteringMaskTexture",     "texture_m") } };
	}

	public static FLVER2.Material CommonER(Material m) 
	{
		return new() { Name = m.Name, MTD = "C[AMSN]_e.matxml", Textures = {
		new("C_AMSN__snp_Texture2D_2_AlbedoMap_0",				m.TextureDiffuse.FilePath),
		new("C_AMSN__snp_Texture2D_7_NormalMap_4",				m.TextureNormal.FilePath),
		new("C_AMSN__snp_Texture2D_0_MetallicMap_0",			m.PBR.TextureMetalness.FilePath) } };		
	}

	public static FLVER2.Material SkinER(Material m) 
	{
		return new() { MTD = "C[DetailBlend]_SSS.matxml", Textures = {
		new("C_DetailBlend__SSS_snp_Texture2D_7_AlbedoMap",		m.TextureDiffuse.FilePath),
		new("C_DetailBlend__SSS_snp_Texture2D_0_NormalMap",		m.TextureNormal.FilePath),
		new("C_DetailBlend__SSS_snp_Texture2D_8_Mask1Map",		"SYSTEX_White"),
		new("C_DetailBlend__SSS_snp_Texture2D_3_MetallicMap",	"SYSTEX_Dummy_m"),
		new("C_DetailBlend__SSS_snp_Texture2D_6_NormalMap",		"AAT100_Skin_01_n", 20) } };
	}
}	