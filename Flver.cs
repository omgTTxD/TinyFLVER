using SharpAssimp;
using System.Xml.Serialization;

public class Flver
{
	public static string path;

	static OpenFileDialog openDialog = new() { Filter = "FLVER |*.flver" };
	static OpenFileDialog importDialog = new() { Filter = "FBX/gLTF |*.fbx;*.gltf;*.glb| All files |*.*" };

	public static void Initialize()
	{
		form.buttonImport.Click += (s, e) => buttonImport_Clicked();
		form.buttonOpen.Click += (s, e) => openDialog.ShowDialog();
		form.buttonSave.Click += buttonSave_Click;
		openDialog.FileOk += (s, e) => { path = openDialog.FileName; LoadFLVER(); };

	//	if (path is null) path = "R:\\Nightreign\\Wylder 2B\\parts\\Wylder Model\\BD_M_5000.flver";	
	//	if (path is null) path = "R:\\Elden Ring\\Dress\\parts\\lg_m_1280\\LG_M_1560.flver";
		if (path is null) openDialog.ShowDialog();
		else LoadFLVER();
	}

	public static void LoadFLVER()
	{
		try { flver = FLVER2.Read(path); }
		catch (Exception e) { MessageBox.Show(e.Message); return; }
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;
		ProcessAddedFlver();
	}

	static void buttonImport_Clicked()
	{
		if (importDialog.ShowDialog() == DialogResult.OK)
		try { Import();	}
		catch (Exception e) { MessageBox.Show("Error during import:" + e.Message); }
	}

	static void Import()
	{
		form.dataGridMeshes.SelectAll();
		Flver.DeleteMeshes();

		// Core postprocess
		PostProcessSteps postprocess = PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs | PostProcessSteps.MakeLeftHanded | PostProcessSteps.CalculateTangentSpace;
		postprocess |= PostProcessSteps.JoinIdenticalVertices | PostProcessSteps.RemoveComponent;
		// Normal maps
		postprocess |= PostProcessSteps.FixInFacingNormals | PostProcessSteps.FindInvalidData | PostProcessSteps.GenerateSmoothNormals;  
								
		Scene scene = new AssimpContext().ImportFile(importDialog.FileName, postprocess);
	
		// Create new buffer layout and also remove all the others
		FLVER2.BufferLayout layout = new() {
			new (FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position, 0),
			new (FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.Normal, 0),			
			new (FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.Tangent, 0),			
			new (FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.BoneIndices, 0),
			new (FLVER.LayoutType.UByte4Norm, FLVER.LayoutSemantic.BoneWeights, 0),
			new (FLVER.LayoutType.UByte4Norm, FLVER.LayoutSemantic.VertexColor, 1),
			new (FLVER.LayoutType.Short4, FLVER.LayoutSemantic.UV, 0)
		};
		flver.BufferLayouts = [layout];

		foreach (var fbxMat in scene.Materials)
		{
			var mat = new FLVER2.Material(fbxMat.Name, "C[AMSN]_e.matxml", 0) { Index = flver.Materials.Count, Textures = [] };
			flver.Materials.Add(mat);

			if (fbxMat.HasTextureDiffuse)
				mat.Textures.Add(new() { Path = fbxMat.TextureDiffuse.FilePath.Split(".")[0], Type = "C_AMSN__snp_Texture2D_2_AlbedoMap_0" });

			if (fbxMat.HasTextureNormal)
				mat.Textures.Add(new() { Path = fbxMat.TextureNormal.FilePath.Split(".")[0], Type = "C_AMSN__snp_Texture2D_7_NormalMap_4" });

			if (fbxMat.PBR.HasTextureMetalness)
				mat.Textures.Add(new() { Path = fbxMat.PBR.TextureMetalness.FilePath.Split(".")[0], Type = "C_AMSN__snp_Texture2D_0_MetallicMap_0" });

			foreach(var tex in mat.Textures)
			{
				if (tex.Path.StartsWith("*"))
					tex.Path = scene.Textures[int.Parse(tex.Path.Replace("*", ""))].Filename;
			}
		}

		foreach (var m in scene.Meshes)
		{
			FLVER2.Mesh flverMesh = new()
			{
				Dynamic = 1,
				VertexBuffers = [new(0)],
				MaterialIndex = m.MaterialIndex,
				BoundingBox = new FLVER2.Mesh.BoundingBoxes()
			};
			
			var boneData = new Dictionary<int, List<(int idx, float w)>>();
			foreach (Bone bone in m.Bones)
			 {
				int boneIndex = flver.Nodes.FindIndex(b => b.Name == bone.Name);
				foreach (VertexWeight w in bone.VertexWeights)
				{
					if (!boneData.TryGetValue(w.VertexID, out var list))
						boneData[w.VertexID] = list = new();
					
					list.Add((boneIndex, w.Weight));
				}
			}

			for (int i = 0; i < m.Vertices.Count; i++)
			{
				FLVER.Vertex v = new()
				{
					Position = m.Vertices[i],
					UVs = [m.TextureCoordinateChannels[0][i], new()],
					Normal = m.Normals[i],
					NormalW = 255,
					Tangents = [new(m.Tangents[i], -1)],
					Colors = [new(255, 255, 255, 255)]
				};

				if (boneData.TryGetValue(i, out var weights))
					for (int j = 0; j < weights.Count && j < 4; j++)
						(v.BoneIndices[j], v.BoneWeights[j]) = (weights[j].idx, weights[j].w);

				flverMesh.Vertices.Add(v);
			}

			flverMesh.FaceSets = [new() { Indices = [.. m.Faces.SelectMany(f => f.Indices)] }];
			flver.Meshes.Add(flverMesh);
		}

		ProcessAddedFlver();
	}

	static void ProcessAddedFlver()
	{
		Directory.SetCurrentDirectory(Path.GetDirectoryName(path));
		form.Text = Path.GetFileNameWithoutExtension(path) + " (" + Path.GetDirectoryName(path) + ")";
		Meshes.Clear();
		flver.Meshes.ForEach(m => Meshes.Add(new(m)));
		DisplayFlverVersion();
		Textures.CreateFileSystemWatcher();
	}

	public static void DeleteMeshes()
	{
		foreach (var mesh in SelectedMeshes)
		{
			flver.Meshes.Remove(mesh.mesh); 
			Meshes.Remove(mesh);
		}

		List<FLVER2.Material> newMaterials = [];

		foreach (var mesh in Meshes)
		{
			mesh.mesh.MaterialIndex = mesh.material.Index = newMaterials.Count;
			newMaterials.Add(mesh.material);
		}
		
		flver.Materials = newMaterials;
	}

	static void buttonSave_Click(object s, EventArgs e)
	{
		var backup = path.Replace(".flver", "_.flver");
		if (!File.Exists(backup))
			File.Copy(path, backup, false);
			
		flver.Write(path);
		if (File.Exists("_.yab"))
			Yabber.Repack(path.Replace(".flver", "_.yab"));
		MessageBox.Show("Saved");
	}

	// For Elden Ring it is neccessary to set correct FLVER version. Nightreign can read both NR and ER flvers. So I shouls always use ER-version I guess?
	static void DisplayFlverVersion()
	{
		Dictionary<int, string> versionsDict = new() {
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131092, "DS III \\ Sekiro \\ BloodBorne" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }};

		var comboBox = form.flverVersions;
		versionsDict.TryGetValue(flver.Header.Version, out var game);
		game = game ?? "Unknown";

		comboBox.Items.Clear(); comboBox.Items.Add(game);
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = versionsDict.FirstOrDefault(x => x.Value == comboBox.Text).Key;
		comboBox.SelectedIndex = 0;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy.
		if (flver.Header.Version == 131098) comboBox.Items.Add(versionsDict[131105]);
		if (flver.Header.Version == 131105) comboBox.Items.Add(versionsDict[131098]);

		// If we have "_s" texture, that means old DSR format, where _s contains roughness, metalness, F0 and emissive. Else it's new ER where "_n" contains glosiness.
		shaderData.OldFormat = game.StartsWith("Dark Souls") ? 1 : 0;
	}
}


// So I want say thanks to ForsakenSilver for your great program, I couldn't use anything else. If only it wasn't so slow because of bones and DataGrids etc...
//
// Also to Elden Ring Reforged' team. I never even played Elden Ring online before, because I couldn't play Vanilla after Reforged. Now I will be able to play online... 
//
// Big thanks to SchuhBaum for "Free Lock-On Camera". Easily the most underrated mod in entire Souls franchise in my opinion. Too bad it exists only for Elden Ring, and I can't play
// without this mod anympre... Just please don't turn it off when you fighting agile enemies. They are SUPPOSED to attack from begind, it is what makes them different and interesting.
//
// To JKAnderson for SoulsFormats. Without it I couln't see FLVER from debugger's perspective. ANd it would be imposibble to code anything.
//
// To Meowmaritus. Back in the days I was very impressed with DSAnimStudio, especially considering it had GPU-accelerated rendering and shader selection long before modern software.
// Probably your program inspired me to make my own... Because when I tried to adapt DSAnimStudio to open .flver files I failed miserably. T_T
//
// To fromsoftserve. Great work with your lighting engine mods. I watched your videos and I know you started to learn shaders recently. So am I, because those broken specular, normal maps,
// tonemapping, fog and water irritated me for several years already. And only now I have possibility to try to do something about it.