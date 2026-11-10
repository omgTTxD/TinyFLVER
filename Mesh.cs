using SharpDX.Direct2D1.Effects;
using System.IO;

struct Vertex(FLVER.Vertex v)
{
	public vec4 Position = v.Position;
	public vec4 Normal	 = v.Normal;
	public vec4 Tangent  = v.Tangents.FirstOrDefault();
	public vec2 UV		 = v.UVs[0];

	public static InputElement[] layout = [
		new("POSITION", 0, Format.R32G32B32A32_Float, 0),
		new("NORMAL",	0, Format.R32G32B32A32_Float, 0),
		new("TANGENT",	0, Format.R32G32B32A32_Float, 0),
		new("TEXCOORD", 0, Format.R32G32_Float, 0),
	];
}

public class Mesh()
{
	public string materialName { get => mat.Name; set => mat.Name = value; }
	public string materialMTD { get => Path.GetFileName(mat.MTD); set => mat.MTD = value; }
	public bool Hidden { get; set; }

	public FLVER2.Mesh mesh;
	public FLVER2.Material mat;

	VertexBufferBinding vertexBuffer;
	Buffer indexBuffer;
	int indexCount;

	public SRV diffuse, normal, Subsurface, metalness, reflectance, specular, details;
	float detailsScale;
	public bool transparent;


	public Mesh(FLVER2.Mesh m) : this()
	{
		this.mesh = m;
		mat = flver.Materials[m.MaterialIndex];
		vertexBuffer = new(Buffer.Create(d, BindFlags.VertexBuffer, [.. m.Vertices.Select(v => new Vertex(v))]), Utilities.SizeOf<Vertex>(), 0);
		indexBuffer = m.FaceSets[0].Indices.Count > 0 ? Buffer.Create(d, BindFlags.IndexBuffer, [.. m.FaceSets[0].Indices]) : null;
		indexCount = m.FaceSets[0].Indices.Count;
		transparent = (mat.Name.ContainsAny("Hair", "brow", "Lash", "Glass") || mat.Name.EndsWith("e")) && !mat.Name.ContainsAny("Face") ;
	}
	

	public void Render()
	{
		c.PixelShader.SetShaderResources(0, [diffuse, normal, metalness, reflectance, specular, details, Subsurface]);
		c.InputAssembler.PrimitiveTopology = mesh.FaceSets[0].TriangleStrip ? PrimitiveTopology.TriangleStrip : PrimitiveTopology.TriangleList;
		c.InputAssembler.SetVertexBuffers(0, vertexBuffer);
		c.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

		cb.MeshID = IsSkin(mat.Name) ? Meshes.IndexOf(this) + 1 : -(Meshes.IndexOf(this) + 1);
		cb.IsDSMetal = mat.IsMetal() ? 1 : 0;
		cb.IsSkin = IsSkin(mat.Name) ? 1 : 0;
		cb.detailsScale = detailsScale;
		cb.F0 =  mat.IsMetal() ? 1f : IsSkin(mat.Name) ? 0.02f : mat.Name.ContainsAny(["Eye"]) ? 0.05f : 0.03f;
		Sync();

		c.DrawIndexed(indexCount, 0, 0);
		c.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
	}

	public static void Render(List<Mesh> meshes, PixelShader PS = null, VertexShader VS = null)
	{
		if (VS is not null) c.VertexShader.Set(VS);
		if (PS is not null) c.PixelShader.Set(PS);

		meshes.FindAll(x => x is not null && !x.transparent).ForEach(m => m.Render());
		meshes.FindAll(x => x is not null && x.transparent).ForEach(m => m.Render());
	}

	static string Convert(string path) => path is null ? null : Path.GetFileNameWithoutExtension(path).Split('.')[0];


	public void ReloadTextures()
	{
		diffuse			= FindTexture(["Albedo", "g_Diffuse"],	["_a", "_d"]);
		metalness		= FindTexture(["MetallicMap"],			["_m"]);
		specular		= FindTexture(["g_Specular"],			["_s"]);
		reflectance		= FindTexture(["Reflectance"],			["_r"]);
		Subsurface		= FindTexture(["Mask"],					["_3m", "_1m"]);

		FindNormals();
		FindDetails();
	}

	public string TryFindingSingleTexture(string[] suffixes)
	{
		List<string> textures = [.. Directory.GetFiles(".", "*.dds").Where(x => Convert(x).EndsWithAny(suffixes))];
		return Convert(textures.FirstOrDefault());
	}

	public SRV FindTexture(string[] types, string[] suffixes, string path = "")
	{
		// First we try to find texture by type. If nothing, try to find by suffix.
		var tex = mat.Textures.FirstOrDefault(x => x.Type.ContainsAny(types));
		tex ??= mat.Textures.FirstOrDefault(x => Convert(x.Path).EndsWithAny(suffixes));
		
		// If still nothing, and there is only one texture with given suffix, just assume it is needed texture
		if(tex is null)
			path = TryFindingSingleTexture(suffixes);
		
		if (tex is not null)
			path = Convert(tex.Path);

		return path is null ? null : Textures.cache.ContainsKey(path) ? Textures.cache[path] : Textures.ReadTexture(path);
	}

	void FindNormals()
	{
		var tex = mat.Textures.FirstOrDefault(x => x.Type.ContainsAny(["NormalMap", "g_Bumpmap"]) && x.Type != "_6_NormalMap");
		tex ??= mat.Textures.FirstOrDefault(x => Convert(x.Path).EndsWithAny("_n") && x.Path != "AAT100_Skin_01_n");

		normal = tex is not null 
			? Textures.ReadTexture(Convert(tex.Path)) 
			: Textures.ReadTexture(TryFindingSingleTexture(["_n"]));
	}

	void FindDetails()
	{
		var tex = mat.Textures.FirstOrDefault(x => x.Type.ContainsAny(["Texture2D_6_NormalMap", "g_DetailBumpmapTexture", "g_DetailBumpmap"]));
		tex ??= mat.Textures.FirstOrDefault(x => x.Path.ContainsAny(["Details"]));
		tex ??= mat.Textures.FirstOrDefault(x => x.Path.ContainsAny(["AAT100_Skin_01_n"]));

		details = tex is null ? null
			: tex.Path == "AAT100_Skin_01_n" ? Textures.ReadTexture(Textures.dir + "AAT100_Skin_01_n")
			: Textures.ReadTexture(Convert(tex.Path));

		if (details is not null)
			detailsScale = tex.Scale.X;
	}
}

