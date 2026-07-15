using Newtonsoft.Json;
using SharpAssimp;
using System.Security.Policy;
using System.Windows.Forms;
using PPS = SharpAssimp.PostProcessSteps;


public class Flver
{
	public static string path;
	public static string directory;
	public static string game = "FLVER not loaded";

	static OpenFileDialog openDialog = new() {
		InitialDirectory = Path.GetDirectoryName(Flver.path),
		Filter = "FLVER|*.flver"
	};

	static OpenFileDialog importDialog = new() {
		Filter = "FBX/gLTF|*.fbx;*.gltf;*.glb|All files|*.*"
	};

	static string recentFilesPath = AppContext.BaseDirectory + "RecentLocations.txt";

	public static void Initialize()
	{
		form.buttonCopyMaterial.Click += (s, e) => CopyMaterial();
		form.buttonPasteMaterial.Click += (s, e) => PasteMaterial();

		if (!File.Exists(recentFilesPath))
			File.Create(recentFilesPath).Dispose();

		var recents = File.ReadAllLines(AppContext.BaseDirectory + "RecentLocations.txt").ToList();
		recents.ForEach(x => openDialog.CustomPlaces.Add(x));

		form.buttonOpen.Click += (s, e) => openDialog.ShowDialog(); 
		openDialog.FileOk += (s, e) =>
		{
			path = openDialog.FileName;
			directory = Path.GetDirectoryName(path);

			if (!recents.Contains(directory))
			{
				openDialog.CustomPlaces.Add(directory);
				var sw = new StreamWriter(File.Open(recentFilesPath, FileMode.Append));
				sw.WriteLine(directory);
				sw.Close();
			}

			LoadFLVER();
		};

		form.buttonImport.Click += (s, e) => { 
			importDialog.InitialDirectory = directory; 
			if (importDialog.ShowDialog() == DialogResult.OK) 
				Import(importDialog.FileName);
			Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		};

		form.buttonSave.Click += (s, e) => Save();
		
		LoadFLVER();
	}

	public static void LoadFLVER()
	{
		if (path is null && openDialog.ShowDialog() != DialogResult.OK || path == "")
			return;	

		try { flver = FLVER2.Read(path); }
		catch (Exception e) { MessageBox.Show(e.Message); return; }

		directory = Path.GetDirectoryName(path);
		Directory.SetCurrentDirectory(directory);

		form.Text = $"{Path.GetFileName(path)} ({directory})";
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;

		DisplayFlverVersion();
		Textures.CreateFileSystemWatcher();

		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
	}


	public static void Import(string path)
	{
		var scene = new AssimpContext().ImportFile(path, PPS.Triangulate | PPS.FlipUVs | PPS.MakeLeftHanded | PPS.LimitBoneWeights
			| PPS.CalculateTangentSpace 
		);

		flver.Meshes.Clear();
		flver.Materials.Clear();
		flver.BufferLayouts = [Desc.bufferLayout];

		foreach (var assimpMat in scene.Materials)
		{
			var mat = assimpMat.Name.ContainsAny("body", "skin", "head", "face") ? Desc.SkinER(assimpMat) : Desc.CommonER(assimpMat);

			// *.glb imports texture names as "*1", "*2", etc... Also, normal's path appends to metalness' path via "-"
			foreach (var t in mat.Textures.Where(t => t.Path.StartsWith("*")))
				t.Path = scene.Textures[int.Parse(t.Path.Substring(1))].Filename.Split("-")[0];

			flver.Materials.Add(mat);
		}

		foreach (var mesh in scene.Meshes)
		{
			FLVER2.Mesh flverMesh = new()
			{
				MaterialIndex = mesh.MaterialIndex,
				UseBoneWeights = true,
				BoundingBox = new(),
				FaceSets = [new() { Indices = [.. mesh.Faces.SelectMany(f => f.Indices)] }],
				VertexBuffers = [new(0)]
			};

			// I think one of the problem with tangents is that SoulsFormats doesn't seem to read W correctly. W should be equal to 1 for half of the
			// mesh, and -1 for other half of the mesh. SoulsFormats reads -1 for entire mesh most of the times, which is wrong. For some meshes
			// tangent.X equals 0, and tangent.w equals 1 (for entire mesh again), which is also doesn't look like correct tangents.
			// I looked at the code and I couldn't find obvious issues. 

			// Another part of the problem is that NR and ER seems to recalculate tangents during rendering. I don't tested it much, but it should be
			// material dependent. 

			// Either way, meshes that I tried to import to DS3 several years ago with FLVER Editor with his swapping/mirroring, and meshes that I try
			// to import in NR now, using .glb have the same problem, which can be fixed by swapping X and Y channel in normal map, and then inverting new X.
			// It's not necessary to edit normal maps, we can do this during import. X is tangent, Y is bitangent. Swap XY means swap tangents with bitangnets.
			// Swap new X means swap bitangents, so we must save -bitangent as tangent.
			for (int i = 0; i < mesh.VertexCount; i++)
			{
				// Assimp doesn't seem to store W, so we must recalculate it
				var bitangent = Vector3.Cross(mesh.Normals[i], mesh.Tangents[i]);
				int w = Vector3.Dot(bitangent, mesh.BiTangents[i]) > 0 ? 1 : -1;

				flverMesh.Vertices.Add(new FLVER.Vertex()
				{
					Position = mesh.Vertices[i],
					Normal = mesh.Normals[i],
					Tangents = [new(-mesh.BiTangents[i], w)],
					//Tangents = [new(0, 0, 0, 0)],
					UVs = [mesh.TextureCoordinateChannels[0][i], new()],
					Colors = [new(255, 255, 255, 255)],
				});
			}

			// We already normalized weights and limited bones affecting one vertice to 4 (via PPS.LimitBoneWeights)
			int[] allocatedWeightsCount = new int[mesh.VertexCount];
			mesh.Bones.ForEach(b => b.VertexWeights.ForEach(w =>
			{
				FLVER.Vertex v = flverMesh.Vertices[w.VertexID];
				v.BoneWeights[allocatedWeightsCount[w.VertexID]] = w.Weight;
				v.BoneIndices[allocatedWeightsCount[w.VertexID]] = flver.Nodes.FindIndex(n => n.Name == b.Name); ;
				allocatedWeightsCount[w.VertexID]++;
			}));

			flver.Meshes.Add(flverMesh);
		}
	}

	public static void DeleteMeshes()
	{
		selectedMeshes.ForEach(m => flver.Meshes.Remove(m.mesh));

		List<FLVER2.Material> newMaterials = [];
		foreach (var mesh in flver.Meshes)
		{
			var material = flver.Materials[mesh.MaterialIndex];
			mesh.MaterialIndex = material.Index = newMaterials.Count;
			newMaterials.Add(material);
		}

		flver.Materials = newMaterials;
	}

	static void SyncMeshesAndDataGrid()
	{
		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		
	}

	static void Save()
	{
		Cursor.Current = Cursors.AppStarting;

		var backup = path.Replace(".flver", "_.flver");
		if (!File.Exists(backup))
			File.Copy(path, backup, false);

			flver.Write(path);

		if (File.Exists("_.yab"))
			Yabber.Repack(path.Replace(".flver", "_.yab"));

		Cursor.Current = Cursors.Default;
	}


	static void CopyMaterial()
	{
		if (selectedMeshes.Count == 1)
			Clipboard.SetText(JsonConvert.SerializeObject(selectedMeshes[0].material));
	}

	static void PasteMaterial()
	{
		try
		{
			var currentMatrial = selectedMeshes[0].material;
			var newMaterial = JsonConvert.DeserializeObject<FLVER2.Material>(Clipboard.GetText());
			currentMatrial.Textures = newMaterial.Textures;

			form.dgTextures.DataSource = selectedMeshes[0].material.Textures;
			Textures.Reload(selectedMeshes[0]);
		}
		catch (Exception e) { MessageBox.Show(e.Message); }
	}

	// For Elden Ring it is neccessary to set correct FLVER version. Nightreign can read both NR and ER flvers.
	static void DisplayFlverVersion()
	{
		Dictionary<int, string> versionsDict = new() {
			{ 131084, "Dark Souls \\ DS Remastered" },
			{ 131092, "BloodBorne \\ DS III \\ Sekiro" },
			{ 131098, "Elden Ring" },
			{ 131105, "Elden Ring Nightreign" }};

		var comboBox = form.flverVersions;
		versionsDict.TryGetValue(flver.Header.Version, out game);
		game = game ?? "Unknown";

		comboBox.Items.Clear(); comboBox.Items.Add(game);
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = versionsDict.FirstOrDefault(x => x.Value == comboBox.Text).Key;
		comboBox.SelectedIndex = 0;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy.
		// Or can they?.. I didn't test it much yet.
		if (flver.Header.Version == 131098) comboBox.Items.Add(versionsDict[131105]);
		if (flver.Header.Version == 131105) comboBox.Items.Add(versionsDict[131098]);

		// 0: Metallic PBR workflow, ER+		2: Specular PBR workflow, BB+
		// 1: DSR strange workflow. Specular texture contains roughness, metalness, F0 and emissive.
		shaderData.FormatID = game.StartsWith("Dark Souls") ? 1 : game.StartsWith("BloodBorne") ? 2 : 0;
	}
}