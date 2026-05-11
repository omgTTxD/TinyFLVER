using SharpAssimp;
//using Assimp;
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
		
	//	if (path is null) path = "R:\\FLVER Models\\2B\\BD_M_5070.flver";	

		if (path is null) openDialog.ShowDialog();
		else LoadFLVER();
	}

	public static void LoadFLVER()
	{
		try { flver = FLVER2.Read(path); }
		catch (Exception e) { MessageBox.Show(e.Message); return; }
		form.dataGridMeshes.Visible = true;
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;
		form.buttonDelete.Visible = true;
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
		Scene scene = new AssimpContext().ImportFile(importDialog.FileName,
			  PostProcessSteps.Triangulate
			| PostProcessSteps.FlipUVs
			| PostProcessSteps.JoinIdenticalVertices
			| PostProcessSteps.MakeLeftHanded
			| PostProcessSteps.CalculateTangentSpace
			| PostProcessSteps.FixInFacingNormals
			| PostProcessSteps.GenerateSmoothNormals
			| PostProcessSteps.FindInvalidData
		);

		int initialMaterialsCount = flver.Materials.Count;

		foreach (var fbxMaterial in scene.Materials)
		{
			var mat = new FLVER2.Material(fbxMaterial.Name, "C[AMSN]_e.matxml", 0) { Index = flver.Materials.Count };
			flver.Materials.Add(mat);

			string albedo = Path.GetFileNameWithoutExtension(fbxMaterial.TextureDiffuse.FilePath);
			if (albedo is null) 
				continue;

			string normal = albedo.Replace("_d", "_n").Replace("_a", "_n");
			string metalness = normal.Replace("_n", "_m");

			mat.Textures = new() {
				new() { Path = albedo,	  Type = "C_AMSN__snp_Texture2D_2_AlbedoMap_0" },	
				new() { Path = metalness, Type = "C_AMSN__snp_Texture2D_0_MetallicMap_0"},
				new() { Path = normal,    Type = "C_AMSN__snp_Texture2D_7_NormalMap_4" },
			};
		}

		foreach (var m in scene.Meshes)
		{
			FLVER2.Mesh flverMesh = new()
			{
				Dynamic = 1,
				VertexBuffers = [new(flver.BufferLayouts.Count - 1)],
				MaterialIndex = initialMaterialsCount + m.MaterialIndex
			};

			var boneIndices = m.Vertices.Select(_ => new List<int>()).ToList();
			var boneWeights = m.Vertices.Select(_ => new List<float>()).ToList();
			foreach (var bone in m.Bones)
			{
				foreach (var w in bone.VertexWeights)
				{
					boneIndices[w.VertexID].Add(flver.Nodes.FindIndex(b => b.Name == bone.Name));
					boneWeights[w.VertexID].Add(w.Weight);
				}
			}

			if (m.Normals.Count == 0) 
				MessageBox.Show("Vertices doesn't have normals!");

			for (int i = 0; i < m.Vertices.Count; i++)
			{
				var v = new FLVER.Vertex() { Position = m.Vertices[i]};
		
				v.UVs = [m.TextureCoordinateChannels[0][i], new()];
				
				if (m.Normals.Count == m.Vertices.Count) 
					v.Normal = m.Normals[i];

				if (m.Tangents.Count == m.Vertices.Count)
					v.Tangents = [new(m.Tangents[i], 1)];

				for (int j = 0; j < boneIndices[i].Count && j < 4; j++)
					(v.BoneIndices[j], v.BoneWeights[j]) = (boneIndices[i][j], boneWeights[i][j]);

				flverMesh.Vertices.Add(v);
			}

			flverMesh.FaceSets = [new()];
			flverMesh.FaceSets[0].Indices = [];
			m.Faces.ForEach(f => flverMesh.FaceSets[0].Indices.AddRange([f.Indices[0], f.Indices[1], f.Indices[2]]));

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
		form.BindMeshesDataGrid();
		Texture.CreateFileSystemWatcher();
	}

	public static void DeleteMeshes()
	{
		foreach (var mesh in Meshes.Where(m => m.Selected).ToList())
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
		form.BindMeshesDataGrid();
	}


	static void buttonSave_Click(object s, EventArgs e)
	{
		var backup = path.Replace(".flver", "_.flver");
		if (!File.Exists(backup))
			File.Copy(path, backup, false);

		flver.Write(path);
		MessageBox.Show("Saved");
	}


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
		shaderData.flag = game.StartsWith("Dark Souls") ? 1 : 0;
	}
}