using Newtonsoft.Json;
using SharpAssimp;
using System.Globalization;
using System.Text.Json;
using System.Windows.Forms;
using PPS = SharpAssimp.PostProcessSteps;

public class CameraSettings
{
	public Vector3 offset;
	public vec2 meshRotation;
	public vec2 lightRotation;
}

public class Flver
{
	public static string path;
	public static string directory;
	public static string game = "FLVER not loaded";

	static OpenFileDialog openDialog = new() {
		InitialDirectory = Path.GetDirectoryName(Flver.path),
		Filter = "FLVER|*.flver;*.bak"
	};

	static OpenFileDialog importDialog = new() {
		Filter = "gLTF, FBX|*.gltf;*.glb;*.fbx|All files|*.*"	
	};

	static string savedLocations = AppContext.BaseDirectory + "SavedLocations.txt";


	static void InitializeFileHistory()
	{
		if (!File.Exists(savedLocations))
			File.Create(savedLocations).Dispose();

		//var saved = File.ReadAllLines(savedLocations);
		
		var locations = File.ReadAllLines(savedLocations)
			.Select(x => x.Split('|'))
			.Select(x => new { Name = x[0], Path = x[1] })
			.ToList();

		form.SavedLocations.DataSource = locations;


		form.SavedLocations.SelectedIndexChanged += (s, e) =>
		{
			var index = form.SavedLocations.SelectedIndex;
			if (index == -1)
				return;

			var path = locations[index].Path;

			if (!File.Exists(path))
				MessageBox.Show($"Couldn't find file. Edit 'SavedLocation.txt' inside program's directory");
			else 
				LoadFLVER(path);
		};
	}

	public static void Initialize()
	{
		form.buttonCopyMaterial.Click += (s, e) => CopyMaterial();
		form.buttonPasteMaterial.Click += (s, e) => PasteMaterial();


		InitializeFileHistory();

		form.buttonOpen.Click += (s, e) => { if (openDialog.ShowDialog() == DialogResult.OK) LoadFLVER(openDialog.FileName); };

		form.buttonImport.Click += (s, e) => { 
			importDialog.InitialDirectory = directory;
			if (importDialog.ShowDialog() == DialogResult.OK)
			{
				Import(importDialog.FileName);
				Meshes = [.. flver.Meshes.Select(m => new Mesh(m))];
				form.dgMeshes.DataSource = Meshes;
			}
		};


		form.buttonSave.Click += (s, e) => Save();
		LoadFLVER();
	}

	public static void LoadFLVER(string flverPath = null)
	{	
		if (Flver.path is not null)
		Camera.SaveCamera();

		path = flverPath ?? path ??  form.SavedLocations.SelectedValue?.ToString();

		if (path is null)
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
		form.dgMeshes.DataSource = Meshes;

		Camera.LoadCamera();
	}


	public static void Import(string path)
	{
		var scene = new AssimpContext().ImportFile(path, PPS.Triangulate | PPS.FlipUVs | PPS.MakeLeftHanded | PPS.LimitBoneWeights | PPS.CalculateTangentSpace
			| PPS.JoinIdenticalVertices 
			| PPS.RemoveRedundantMaterials | PPS.FindInvalidData   |  PPS.GenerateSmoothNormals
			
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
				VertexBuffers = [new(0)],
				UseBoneWeights = true,
				MaterialIndex = mesh.MaterialIndex,
				BoundingBox = new(),
				FaceSets = [new() { Indices = [.. mesh.Faces.SelectMany(f => f.Indices)] }],
			};

			// Several years ago, when I tried to import BDO models to DS3, I noticed a strange bug with normals. To make them work, I needed to 
			// swap X and Y, which is very unusual. I thought it was because of ZY switch during import.
			// I was VERY surprised when I learned that even now, with hardly any changes to imported data, the normals are broken in the exact same way.
			// Even if I literally doing nothing during import, they are still broken. 
			// I thought bigger part of the problem is SoulsFormats incorrect reading/writing. But I looked at the code and I couldn't find obvious issues.

			// Another part of the problem is that NR and ER seems to recalculate tangents during rendering. I don't tested it much, but it should be
			// material dependent. 

			for (int i = 0; i < mesh.VertexCount; i++)
			{
				// Assimp doesn't seem to store W, so we must recalculate it
				int w = Vector3.Dot(Vector3.Cross(mesh.Normals[i], mesh.Tangents[i]), mesh.BiTangents[i]) > 0 ? 1 : -1;

				flverMesh.Vertices.Add(new FLVER.Vertex()
				{
					// Either way, it can be fixed by swapping X and Y channel in normal map, and then inverting new X in Photoshop. It is important to understand that
					// X is Tangent component of TBN matrix, which is required to calculate normals. And Y is Bitangent. So swapping XY is swapping tangents with bitangnets.
					// And swapping new X means saving bitangents with -1.
					Tangents = [new(-mesh.BiTangents[i], w)],
					Position = mesh.Vertices[i],
					Normal = mesh.Normals[i],
					UVs = [mesh.TextureCoordinateChannels[0][i], new()],
					Colors = [new(255, 255, 255, 255)],
				});
			}

			// We already normalized weights and limited bones affecting each vertice to 4 in preprocessing, so we can just use [w.VertexID]++ and be sure that index won't overflow
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