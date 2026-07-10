using SharpAssimp;
using System.Security.Policy;
using System.Windows.Forms;
using Semantic = SoulsFormats.FLVER.LayoutSemantic;
using PPS = SharpAssimp.PostProcessSteps;
using Type = SoulsFormats.FLVER.LayoutType;

public class Flver
{
	public static string path;
	public static string directory;
	public static string game;

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

		form.buttonImport.Click += (s, e) => { importDialog.InitialDirectory = directory; if (importDialog.ShowDialog() == DialogResult.OK) Import(importDialog.FileName); SyncMeshesAndDataGrid(); };
		form.buttonSave.Click += (s, e) => Save();
		
		//if (path is null) path = "R:\\Elden Ring\\Dress\\parts\\fc_f_0000\\fc_f_0000.flver";
		//	if (path is null) path = "R:\\Nightreign\\Wylder 2B\\parts\\Wylder Model\\BD_M_5000.flver";	
		//	if (path is null) path = "R:\\Dark Souls Remastered\\@\\parts\\AM_A_9550_M\\AM_A_9550_M.flver.bak";
		
		if (path is null)
			openDialog.ShowDialog();

		if (path is not null)
			LoadFLVER();
	}

	public static void LoadFLVER()
	{

		try { flver = FLVER2.Read(path); }
		catch (Exception e) { MessageBox.Show(e.Message); return; }

		directory = Path.GetDirectoryName(path);
		Directory.SetCurrentDirectory(directory);

		form.Text = $"{Path.GetFileName(path)} ({directory})";
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;

		DisplayFlverVersion();
		Textures.CreateFileSystemWatcher();

		SyncMeshesAndDataGrid();
	}

	public static void Import(string path)
	{
		Scene scene = new AssimpContext().ImportFile(path, PPS.Triangulate | PPS.MakeLeftHanded | PPS.FlipUVs | PPS.LimitBoneWeights
			| PPS.CalculateTangentSpace			
		);

		flver.Meshes.Clear();
		flver.Materials.Clear();

		FLVER2.BufferLayout layout = null;
		
		// Materials
		foreach (var assimpMat in scene.Materials)
		{
			FLVER2.Material mat = assimpMat.Name.ContainsAny("body", "skin", "head", "face") 
				? Materials.Skin(assimpMat, out layout) 
				: Materials.Common(assimpMat, out layout);

			// *.glb imports texture names as "*1", "*2", etc... Also, normal's path appends to metalness' path via "-"
			foreach (var t in mat.Textures.Where(t => t.Path.StartsWith("*")))
				t.Path = scene.Textures[int.Parse(t.Path.Substring(1))].Filename.Split("-")[0];

			flver.Materials.Add(mat);
		}

		// Also removing all other layouts so they don't interfere in testing
		flver.BufferLayouts = [layout];

		// Meshes
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

			// Vertice data
			for (int i = 0; i < mesh.VertexCount; i++)
			{
				// Most meshes (especially those imported with FLVER Editor with axis swapping) need to swap XY and flip new Y. Bitangent direction should be stored in
				// Tangent.W, however, there is also NormalW which is usually 0, and could also store W. I need to watch how shader loads them later.

				// FBX is probably breakibg normals too. Also should check gltf export and assimp import and postprocessing. Since assimp doesn't store W, we can
				// calculate it ourselves. First we calculate bitangent, then we check angle with stored bitangend like that:
			//	var bitangent = Vector3.Cross(mesh.Normals[i], mesh.Tangents[i]);
			//	var w = Vector3.Dot(mesh.BiTangents[i]), bitangent) > 0 ? 1 : -1;

				// At least I am very glad I am already know that SoulsFormats doesn't read W correctly (always -1, instead of +/-1). And therefore, writing is wrong too. So
				// this is the first thing that should be fixed before all others. I also have a feeling that this is the only fix needed.
				
				// And I also will need to return shader which uses stored tangents, and not recalculates them...

				flverMesh.Vertices.Add(new FLVER.Vertex()
				{
					Position = mesh.Vertices[i],
					Normal = mesh.Normals[i],
					Tangents = [new(mesh.Tangents[i], -1)],
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
		SelectedMeshes.ForEach(m => flver.Meshes.Remove(m.mesh));

		List<FLVER2.Material> newMaterials = [];
		foreach (var mesh in flver.Meshes)
		{
			var material = flver.Materials[mesh.MaterialIndex];
			mesh.MaterialIndex = material.Index = newMaterials.Count;
			newMaterials.Add(material);
		}

		flver.Materials = newMaterials;
	}

	public static void SyncMeshesAndDataGrid()
	{
		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		form.dgMeshes.DataSource = Meshes;
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