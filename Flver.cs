using Newtonsoft.Json;
using SharpAssimp;
using System.ComponentModel;
using PPS = SharpAssimp.PostProcessSteps;

class Flver
{
	public static string path;
	public static string directory;
	public static string game = "FLVER not loaded";

	static OpenFileDialog openDialog = new() {
		InitialDirectory = Path.GetDirectoryName(Flver.path),
		Filter = "FLVER|*.flver;*.dcx;*.tpf;*.yab"
	};

	static OpenFileDialog importDialog = new() {
		Filter = "gLTF, FBX|*.gltf;*.glb;*.fbx|All files|*.*"	
	};

	static string savedLocations = AppContext.BaseDirectory + "SavedLocations.txt";

	static void InitializeFileHistory()
	{
		if (!File.Exists(savedLocations))
			File.Create(savedLocations).Dispose();

		form.SavedLocations.DataSource = File.ReadAllLines(savedLocations).Select(x => x.Split('|'))
			.Select(x => new { Name = x[0], Path = x[1] }).ToList();

		form.SavedLocations.SelectedIndexChanged += (s, e) =>
		{
			if (!File.Exists(form.SavedLocations.SelectedValue.ToString()))
				MessageBox.Show($"Couldn't find file. Edit 'SavedLocation.txt' inside program's directory");
			else
			{
				path = form.SavedLocations.SelectedValue.ToString();
				LoadFLVER();
			}
		};
	}

	public static void Initialize()
	{
		form.buttonCopyMaterial.Click += (s, e) => CopyMaterial();
		form.buttonPasteMaterial.Click += (s, e) => PasteMaterial();

		InitializeFileHistory();

		form.buttonOpen.Click += (s, e) => {
			if (openDialog.ShowDialog() != DialogResult.OK)
				return;
			if (openDialog.FileName.ContainsAny(".dcx", ".tpf"))
			{
				Yabber.OpenWithYabber(openDialog.FileName);
				MessageBox.Show("Unpacked.");
			}
			else if (openDialog.FileName.Contains(".yab")) 
			{
				Yabber.OpenWithYabber(openDialog.FileName);
				MessageBox.Show("Repacked.");
			}
			else
				LoadFLVER(openDialog.FileName); 	
		};

		form.buttonImport.Click += (s, e) => { 
			importDialog.InitialDirectory = directory;
			if (importDialog.ShowDialog() == DialogResult.OK)
				Import(importDialog.FileName);
		};

		form.buttonSave.Click += (s, e) => Save();
		LoadFLVER();
	}

	public static void LoadFLVER(string flverPath = null)
	{	
		Camera.SaveCamera();
		path = flverPath ?? path ?? form.SavedLocations.SelectedValue?.ToString();

		try { flver = FLVER2.Read(path); }
		catch (Exception e) { MessageBox.Show(e.Message); return; }

		directory = Path.GetDirectoryName(path);
		Directory.SetCurrentDirectory(directory);

		DisplayFlverVersion();
		form.Text = $"{Path.GetFileName(path)} ({directory})";
		form.buttonImport.Visible = true;
		form.buttonSave.Visible = true;
	
		Textures.CreateFileSystemWatcher();
		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
		Camera.LoadCamera();

		Renderer.Render();
	}


	public static void Import(string path)
	{
		var scene = new AssimpContext().ImportFile(path, PPS.Triangulate | PPS.FlipUVs | PPS.MakeLeftHanded | PPS.LimitBoneWeights
			| PPS.JoinIdenticalVertices
		);

		flver.Meshes.Clear();
		flver.Materials.Clear();
		flver.BufferLayouts = game.StartsWith("Dark Souls \\") ? [Desc.BufferLayoutDSR] : [Desc.BufferLayoutER];

		foreach (var assimpMat in scene.Materials)
		{
			var mat = assimpMat.Name.ContainsAny("body", "skin", "head", "face") ? Desc.Skin(assimpMat) : Desc.Metallic(assimpMat);

			// *.glb imports texture names as "*1", "*2", etc... Also, normal's path appends to metalness' path via "-"
			foreach (var t in mat.Textures.Where(t => t.Path.StartsWith("*")))
				t.Path = scene.Textures[int.Parse(t.Path.Substring(1))].Filename.Split("-")[0];

			flver.Materials.Add(mat);
		}

		foreach (var mesh in scene.Meshes)
		{
			FLVER2.Mesh flverMesh = new()
			{
				VertexBuffers = [new(0)],
				UseBoneWeights = true,
				MaterialIndex = mesh.MaterialIndex,
				BoundingBox = new(),
				FaceSets = [new() { Indices = [.. mesh.GetIndices()] }],
			};

			//   Several years ago, when I tried to import BDO models to DS3, I noticed a strange bug with normals. To make them work, I needed to 
			// swap X with -Y, which is very unusual (in 99% you just need to flip Y).
			//   At first I thought it was because of ZY axis switch during import in FLVER Editor. So I was very surprised when I encountered this exact
			// same bug again, even if I don't touch imported data.
			//   Then I thought that SoulsFormats reads T/B incorrectly. But this bug occurs because FLVER treats tangetns as bitangents. And to fix this
			// bug, we can either swap X/Y and flip new X in Photoshop, or swap them during import. We start recalculations from Bitangent and send them
			// to FLVER as input.tangent. Then, in shader:  
			//
			// FlverB = input.T;
			// Tf = cross(N, Bf) * input.tangent.w;
			// FinalN = TextureN * [Tf, Bf, VertexN];
			// 
			// I am saving it as Bitangent, and will be recalculating missing Tangent. And to invert Tangent, we need to save -W.

			for (int i = 0; i < mesh.VertexCount; i++)
			{
				int w = Vector3.Dot(Vector3.Cross(mesh.Normals[i], mesh.Tangents[i]), mesh.BiTangents[i]) > 0 ? 1 : -1;

				if (game.StartsWith("Dark Souls \\"))
					w *= -1;

				flverMesh.Vertices.Add(new FLVER.Vertex()
				{
					Tangents = game.StartsWith("Dark Souls \\") ? [new(mesh.Tangents[i], w)] : [new(mesh.BiTangents[i], -w)],
				//	Tangents = [new(mesh.BiTangents[i], -w)],
					Position = mesh.Vertices[i],
					Normal = mesh.Normals[i],
					UVs = [mesh.TextureCoordinateChannels[0][i], new()],
					Colors = [new(255, 255, 255, 255)],
				});
			}

			// We already normalized weights and limited bones affecting each vertice to 4 in preprocessing, so we can be sure that index won't overflow.
			int[] weightsCount = new int[mesh.VertexCount];
			mesh.Bones.ForEach(b => b.VertexWeights.ForEach(w =>
			{
				FLVER.Vertex v = flverMesh.Vertices[w.VertexID];
				v.BoneWeights[weightsCount[w.VertexID]] = w.Weight;
				v.BoneIndices[weightsCount[w.VertexID]] = flver.Nodes.FindIndex(n => n.Name == b.Name);
				weightsCount[w.VertexID]++;
			}));

			flver.Meshes.Add(flverMesh);
		}

		Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
	}

	public static void DeleteMeshes()
	{
		List<Mesh> toDelete = [..selectedMeshes];
		toDelete.ForEach(m => { flver.Meshes.Remove(m.mesh); Meshes.Remove(m); });
		
		List<FLVER2.Material> newMaterials = [];
		foreach (var mesh in flver.Meshes)
		{
			var material = flver.Materials[mesh.MaterialIndex];
			mesh.MaterialIndex = material.Index = newMaterials.Count;
			newMaterials.Add(material);
		}
	
		flver.Materials = newMaterials;
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
		try {
			var currentMaterial = selectedMeshes[0].material;
			var newMaterial = JsonConvert.DeserializeObject<FLVER2.Material>(Clipboard.GetText());
			currentMaterial.Textures = newMaterial.Textures;
			currentMaterial.MTD = newMaterial.MTD;

			form.dgMeshes.Refresh();
			form.dgTextures.DataSource = new BindingList<FLVER2.Texture>(currentMaterial.Textures);
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

		comboBox.Items.Clear();	comboBox.Items.Add(game);
		comboBox.SelectedIndexChanged += (s, e) => flver.Header.Version = versionsDict.FirstOrDefault(x => x.Value == comboBox.Text).Key;
		comboBox.SelectedIndex = 0;

		// Elden Ring and Nightreign FLVERs can be freely changed to one another. Other FLVERs can't be changed that easy. There are different
		// checks for Flver integrity, but I didn't test it after first try. All those CHECKS can and will be eventually wiped out anyway. Let the game
		// decide what it can render and what it can't. Just kidding, of course all checks that I can reach will be wiped out from the game also.
		if (flver.Header.Version == 131098) comboBox.Items.Add(versionsDict[131105]);
		if (flver.Header.Version == 131105) comboBox.Items.Add(versionsDict[131098]);
		comboBox.Items.Add(versionsDict[131092]);
		comboBox.Items.Add(versionsDict[131084]);

		// 0: Metallic PBR workflow, ER+		2: Specular PBR workflow, BB+		3: Test Spheres
		// 1: DSR strange workflow. Specular texture contains roughness, metalness, F0 and emissive.
		shaderData.FormatID = game.StartsWith("Dark Souls") ? 1 : game.StartsWith("BloodBorne") ? 2 : 0;
	}
}