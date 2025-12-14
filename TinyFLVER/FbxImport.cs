using Assimp;
using Newtonsoft.Json;
using SoulsFormats;
using System.Numerics;

// Mostly untouched verison of code from FlverEditor. I just deleted all I could delete without breaking anything too much.
namespace TinyFLVER
{
	public partial class FbxImport : Form
	{
		public class FbxImportSettings
		{
			private static string SettingsFilePath => Path.Combine(
				Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
				"fbx_import_settings.json");

			public enum Axis { X, Y, Z, NegX, NegY, NegZ }

			// File Paths
			public string ImportFilePath { get; set; } = "";

			public Axis PrimaryAxis { get; set; } = Axis.X;
			public Axis SecondaryAxis { get; set; } = Axis.Y;
			public bool MirrorTertiaryAxis { get; set; } = true;

			public bool BlenderTan { get; set; } = false;

			public bool InverseTangentW { get; set; } = false;

			// Other Options
			public bool SetTexture { get; set; } = true;
			public bool SetLOD { get; set; } = false;


			public FbxImportSettings()
			{
				// Set default bone conversion file path relative to the executable
			}


			public void Save()
			{
				try
				{
					string json = JsonConvert.SerializeObject(this);
					File.WriteAllText(SettingsFilePath, json);
				}
				catch (Exception ex)
				{
					// Log or handle the error appropriately, for now we just ignore it
					Console.WriteLine($"Failed to save FBX import settings: {ex.Message}");
				}
			}

			public static FbxImportSettings Load()
			{
				try
				{
					if (File.Exists(SettingsFilePath))
					{
						string json = File.ReadAllText(SettingsFilePath);
						return JsonConvert.DeserializeObject<FbxImportSettings>(json);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Failed to load FBX import settings, using defaults: {ex.Message}");
				}
				return new FbxImportSettings();
			}
		}


		public FbxImportSettings Settings { get; private set; }

		static MainForm form;
		public FbxImport(FbxImportSettings initialSettings, MainForm form)
		{
			this.Settings = initialSettings;
			InitializeComponent();
			PopulateControlsFromSettings();
			FbxImport.form = form;
		}

		private void PopulateControlsFromSettings()
		{
			txtImportPath.Text = Settings.ImportFilePath;
			chkMirrorTertiary.Checked = Settings.MirrorTertiaryAxis; // Populate new checkbox
			chkBlenderTan.Checked = Settings.BlenderTan;
			chkInverseTan.Checked = Settings.InverseTangentW;
			chkSetTexture.Checked = Settings.SetTexture;
		}

		private void buttonOk_Click(object sender, EventArgs e)
		{
			Settings.ImportFilePath = txtImportPath.Text;
			Settings.MirrorTertiaryAxis = chkMirrorTertiary.Checked;
			Settings.BlenderTan = chkBlenderTan.Checked;
			Settings.InverseTangentW = chkInverseTan.Checked;
			Settings.SetTexture = chkSetTexture.Checked;

			DialogResult = DialogResult.OK;
		}

		private void btnChooseImportFile_Click(object sender, EventArgs e)
		{
			using (var ofd = new OpenFileDialog { Filter = "FBX Files|*.fbx|All Files|*.*" })
			{
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					txtImportPath.Text = ofd.FileName;
				}
			}
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}


		public static int boneFindParentTimes = 15;//i
		public static Vector3 RemapVector(Vector3 input, FbxImportSettings.Axis primary, FbxImportSettings.Axis secondary, bool mirrorTertiary)
		{
			// Determine the remapped basis vectors. This defines the transformation.
			Vector3 newX = new Vector3(GetAxisValue(new Vector3(1, 0, 0), primary), GetAxisValue(new Vector3(0, 1, 0), primary), GetAxisValue(new Vector3(0, 0, 1), primary));
			Vector3 newY = new Vector3(GetAxisValue(new Vector3(1, 0, 0), secondary), GetAxisValue(new Vector3(0, 1, 0), secondary), GetAxisValue(new Vector3(0, 0, 1), secondary));
			Vector3 newZ = Vector3.Cross(newX, newY);

			if (mirrorTertiary)
			{
				newZ *= -1;
			}

			// The remapped vector is a linear combination of the new basis vectors,
			// scaled by the original vector's components.
			return (input.X * newX) + (input.Y * newY) + (input.Z * newZ);
		}


		public static void importFBX(MainForm mainForm)
		{
			// Load settings and show the import form
			FbxImportSettings settings = FbxImportSettings.Load();
			using (var form = new FbxImport(settings, mainForm))
			{
				if (form.ShowDialog() != DialogResult.OK)
				{
					return; // User cancelled
				}
				settings = form.Settings; // Get updated settings
				settings.Save(); // Save for next time
			}

			if (string.IsNullOrEmpty(settings.ImportFilePath) || !File.Exists(settings.ImportFilePath))
			{
				MessageBox.Show("Import file path is not valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			AssimpContext importer = new AssimpContext();
			Directory.SetCurrentDirectory(Path.GetDirectoryName(settings.ImportFilePath));
			// importer.SetConfig(new NormalSmoothingAngleConfig(66.0f));


			//Table prepartion finished

			Scene md = importer.ImportFile(settings.ImportFilePath, PostProcessSteps.CalculateTangentSpace | PostProcessSteps.Triangulate); // PostProcessPreset.TargetRealTimeMaximumQuality



			var boneParentList = new Dictionary<String, String>();
			//Build the parent list of bones.
			//printNodeStruct(md.RootNode);

			//First, added a custom default layout.
			int layoutCount = MainForm.flver.BufferLayouts.Count;
			FLVER2.BufferLayout newBL = new FLVER2.BufferLayout
			{
				new FLVER.LayoutMember(FLVER.LayoutType.Float3, FLVER.LayoutSemantic.Position, 0),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.Normal, 0),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.Tangent, 0),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.Tangent, 1),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4, FLVER.LayoutSemantic.BoneIndices, 0),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4Norm, FLVER.LayoutSemantic.BoneWeights, 0),
				new FLVER.LayoutMember(FLVER.LayoutType.UByte4Norm, FLVER.LayoutSemantic.VertexColor, 1),
				new FLVER.LayoutMember(FLVER.LayoutType.Short4, FLVER.LayoutSemantic.UV, 0)
			};

			MainForm.flver.BufferLayouts.Add(newBL);

			int materialCount = MainForm.flver.Materials.Count;

			// --- NEW: Robustly determine if winding order needs to be flipped. ---
			// A change in coordinate system handedness requires flipping face winding.
			// This is determined by the determinant of the basis vector transformation matrix.
			// If the determinant is negative, handedness has changed.
			//    Vector3 basisX = new Vector3(1, 0, 0);
			//   Vector3 basisY = new Vector3(0, 1, 0);
			//   Vector3 basisZ = new Vector3(0, 0, 1);

			Vector3 remappedBasisX = RemapVector(new Vector3(1, 0, 0), settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);
			Vector3 remappedBasisY = RemapVector(new Vector3(0, 1, 0), settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);
			Vector3 remappedBasisZ = RemapVector(new Vector3(0, 0, 1), settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);

			System.Numerics.Matrix4x4 basisMatrix = new System.Numerics.Matrix4x4(
				remappedBasisX.X, remappedBasisY.X, remappedBasisZ.X, 0,
				remappedBasisX.Y, remappedBasisY.Y, remappedBasisZ.Y, 0,
				remappedBasisX.Z, remappedBasisY.Z, remappedBasisZ.Z, 0,
				0, 0, 0, 1
			);

			bool flipWinding = basisMatrix.GetDeterminant() > 0;

			foreach (var mat in md.Materials)
			{
				FLVER2.Material matnew = new FLVER2.Material();
				matnew.Name = Path.GetFileNameWithoutExtension(settings.ImportFilePath) + "_" + mat.Name;

				if (settings.SetTexture)
				{
					if (mat.HasTextureDiffuse) //g_DiffuseTexture
					{
						SetFlverMatPath(matnew, "g_DiffuseTexture", FindFileName(mat.TextureDiffuse.FilePath) + ".tif");
					}
					if (mat.HasTextureNormal)//g_BumpmapTexture
					{
						SetFlverMatPath(matnew, "g_BumpmapTexture", FindFileName(mat.TextureNormal.FilePath) + ".tif");
					}
					if (mat.HasTextureSpecular)//g_SpecularTexture
					{
						SetFlverMatPath(matnew, "g_SpecularTexture", FindFileName(mat.TextureSpecular.FilePath) + ".tif");
					}
				}
				MainForm.flver.Materials.Add(matnew);
			}


			//mn.MaterialIndex = materialCount;

			foreach (var m in md.Meshes)
			{
				FLVER2.Mesh mn = new FLVER2.Mesh();
				mn.MaterialIndex = 0;
				mn.BoneIndices = new List<int>();
				mn.BoneIndices.Add(0);
				mn.BoneIndices.Add(1);
				mn.BoundingBox = new FLVER2.Mesh.BoundingBoxes();
				mn.BoundingBox.Max = new Vector3(1, 1, 1);
				mn.BoundingBox.Min = new Vector3(-1, -1, -1);
				mn.BoundingBox.Unk = new Vector3();
				mn.NodeIndex = 0;
				mn.Dynamic = 1;
				mn.VertexBuffers = new List<FLVER2.VertexBuffer>();
				mn.VertexBuffers.Add(new FLVER2.VertexBuffer(layoutCount));
				mn.Vertices = new List<FLVER.Vertex>();

				List<List<int>> verticesBoneIndices = new List<List<int>>();
				List<List<float>> verticesBoneWeights = new List<List<float>>();


				//If it has bones, then record the bone weight info
				if (m.HasBones)
				{
					for (int i2 = 0; i2 < m.VertexCount; i2++)
					{
						verticesBoneIndices.Add(new List<int>());
						verticesBoneWeights.Add(new List<float>());
					}

					for (int i2 = 0; i2 < m.BoneCount; i2++)
					{
						string boneName = m.Bones[i2].Name;
						int boneIndex;


						Console.WriteLine("Cannot find ->" + boneName);
						boneIndex = findFLVER_Bone(MainForm.flver, boneName);

						for (int bp = 0; bp < boneFindParentTimes; bp++)
						{
							if (boneIndex == -1)
							{
								if (boneParentList.ContainsValue(boneName))
								{
									if (boneParentList[boneName] != null)
									{
										boneName = boneParentList[boneName];

										boneIndex = findFLVER_Bone(MainForm.flver, boneName);
									}
								}
							}
						}


						if (boneIndex == -1) { boneIndex = 0; }
						for (int i3 = 0; i3 < m.Bones[i2].VertexWeightCount; i3++)
						{
							var vw = m.Bones[i2].VertexWeights[i3];
							verticesBoneIndices[vw.VertexID].Add(boneIndex);
							verticesBoneWeights[vw.VertexID].Add(vw.Weight);
						}
					}
				}


				// m.Bones[0].VertexWeights[0].
				for (int i = 0; i < m.Vertices.Count; i++)
				{
					var vit = m.Vertices[i];
					var channels = m.TextureCoordinateChannels[0];

					List<Vector3> tempUvs = new List<Vector3>();

					var uv1 = new Vector3(); // Basic fall back
					var uv2 = new Vector3();

					if (channels != null && m.TextureCoordinateChannelCount > 0)
					{
						uv1 = ToVec3(channels[i]);
						uv1.Y = 1 - uv1.Y;
						uv2 = ToVec3(channels[i]);
						uv2.Y = 1 - uv2.Y;
						for (int j = 0; j < m.TextureCoordinateChannelCount; j++)
						{
							var tempChan = m.TextureCoordinateChannels[j];
							tempUvs.Add(new Vector3(tempChan[i].X, 1f - tempChan[i].Y, 0));
						}
						while (tempUvs.Count < 2) { tempUvs.Add(new Vector3(uv1.X, uv1.Y, uv1.Z)); }
					}


					var normal = new Vector3D(0, 1, 0);
					if (m.HasNormals && m.Normals.Count > i)
					{
						normal = m.Normals[i];
					}

					var tangent = new Vector3(1, 0, 0);

					//TODO: Two experimental function, tangent calculation suited for blender
					var swapTanBitan = false; // Blender export only
					var inverseTan = false; // Blender export only
					if (settings.BlenderTan)
					{
						swapTanBitan = true;
						inverseTan = true;
					}
					if (m.HasTangentBasis && m.Tangents.Count > i)
					{
						m.Tangents[i].Normalize();
						m.BiTangents[i].Normalize();
						tangent = ToVec3(m.Tangents[i]);
						if (swapTanBitan) { tangent = ToVec3(m.BiTangents[i]); }

					}
					else if (m.HasNormals && m.Normals.Count > i)
					{
						// Actually speaking totally wrong 
						m.Normals[i].Normalize();
						normal.Normalize();
						tangent = ToVec3(Vector3D.Cross(m.Normals[i], normal));
					}

					bool hasBitangent = false;
					Vector3 bitangent;
					if (m.HasTangentBasis && m.BiTangents.Count > i)
					{
						m.Tangents[i].Normalize();
						m.BiTangents[i].Normalize();
						hasBitangent = true;
						bitangent = ToVec3(m.BiTangents[i]);
						if (swapTanBitan) { bitangent = ToVec3(m.Tangents[i]); }
					}
					else
					{
						bitangent = tangent;
					}

					// --- NEW: Remap vectors based on settings, including the mirror option ---
					Vector3 remappedPosition = RemapVector(ToVec3(vit), settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);
					Vector3 remappedNormal = RemapVector(ToVec3(normal), settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);
					Vector3 remappedTangent = RemapVector(tangent, settings.PrimaryAxis, settings.SecondaryAxis, settings.MirrorTertiaryAxis);
					//     Vector3 remappedBitangent;

					if (inverseTan) { remappedTangent = new Vector3(-remappedTangent.X, -remappedTangent.Y, -remappedTangent.Z); }

					float tangentW = 1;
					if (settings.InverseTangentW)
					{
						tangentW *= -1;
					}
					if (hasBitangent)
					{
						//判断CrossProduct(normal, tangent) 是不是和 Bitangent的夹角小于90°，如果小于则表示w不用翻面，否则W要翻面
						// 不能在remapped中比较，要用fbx导出的时候的坐标系
						Vector3 n = ToVec3(normal);
						Vector3 t = tangent;
						Vector3 positiveBitangent = Vector3.Cross(n, t);
						if (Vector3.Dot(positiveBitangent, bitangent) < 0)
						{
							tangentW *= -1;
						}
					}

					FLVER.Vertex v = generateVertex(remappedPosition, uv1, uv2, remappedNormal, remappedTangent, tangentW);
					if (tempUvs.Count > 1) { v.UVs = tempUvs; } // V2.6 Multi-UV Support
					if (m.HasBones)
					{
						for (int j = 0; j < verticesBoneIndices[i].Count && j < 4; j++)
						{
							v.BoneIndices[j] = (verticesBoneIndices[i])[j];
							v.BoneWeights[j] = (verticesBoneWeights[i])[j];
						}
					}
					mn.Vertices.Add(v);
				}

				List<int> faceIndexs = new List<int>();
				foreach (var face in m.Faces)
				{
					if (face.IndexCount == 3)
					{
						// --- NEW: Use robust flipWinding calculated earlier ---
						if (flipWinding)
						{
							faceIndexs.Add(face.Indices[0]);
							faceIndexs.Add(face.Indices[2]);
							faceIndexs.Add(face.Indices[1]);
						}
						else
						{
							faceIndexs.Add(face.Indices[0]);
							faceIndexs.Add(face.Indices[1]);
							faceIndexs.Add(face.Indices[2]);
						}

					}
					else if (face.IndexCount == 4)
					{ // NOT Yet Tested on quads yet...
						if (flipWinding)
						{
							faceIndexs.Add(face.Indices[0]);
							faceIndexs.Add(face.Indices[2]);
							faceIndexs.Add(face.Indices[1]);

							faceIndexs.Add(face.Indices[2]);
							faceIndexs.Add(face.Indices[0]);
							faceIndexs.Add(face.Indices[3]);
						}
						else
						{
							faceIndexs.Add(face.Indices[0]);
							faceIndexs.Add(face.Indices[1]);
							faceIndexs.Add(face.Indices[2]);

							faceIndexs.Add(face.Indices[2]);
							faceIndexs.Add(face.Indices[3]);
							faceIndexs.Add(face.Indices[0]);
						}


					}
					else
					{
						//Probrably something WRONG
					}


				}

				mn.FaceSets = new List<FLVER2.FaceSet>();
				mn.FaceSets.Add(generateBasicFaceSet());
				mn.FaceSets[0].Indices = faceIndexs;


				mn.MaterialIndex = materialCount + m.MaterialIndex;
				MainForm.flver.Meshes.Add(mn);
			}

			//UpdateVertices();
			form.FillMeshControls();
		}
		static Vector3 ToVec3(Vector3D v)
		{
			var vec = new Vector3(v.X, v.Y, v.Z);
			return vec;
		}

		static int findFLVER_Bone(FLVER2 f, string name)
		{
			for (int flveri = 0; flveri < f.Nodes.Count; flveri++)
			{
				if (f.Nodes[flveri].Name == name)
				{

					return flveri;

				}

			}
			return -1;
		}
		static FLVER2.FaceSet generateBasicFaceSet()
		{
			FLVER2.FaceSet ans = new FLVER2.FaceSet
			{
				CullBackfaces = true,
				TriangleStrip = false,
				Unk06 = 1
			};

			return ans;
		}

		static FLVER.Vertex generateVertex(Vector3 pos, Vector3 uv1, Vector3 uv2, Vector3 normal, Vector3 tangets, float tangentW = -1)
		{
			FLVER.Vertex ans = new FLVER.Vertex
			{
				Position = pos
			};
			ans.BoneIndices[0] = 0; ans.BoneIndices[1] = 0; ans.BoneIndices[2] = 0; ans.BoneIndices[3] = 0;
			ans.BoneWeights[0] = 1; ans.BoneWeights[1] = 0; ans.BoneWeights[2] = 0; ans.BoneWeights[3] = 0;
			ans.UVs = new List<Vector3>
			{
				uv1,
				uv2
			};
			ans.Normal = normal;
			ans.NormalW = -1;
			ans.Tangents = new List<Vector4>
			{
				new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW),
				new Vector4(tangets.X, tangets.Y, tangets.Z, tangentW)
			};
			ans.Colors = new List<FLVER.VertexColor>
			{
				new FLVER.VertexColor(255, 255, 255, 255)
			};
			return ans;
		}

		//	private static Vector3 toNumV3(Vector3 normal) {
		//		return new Vector3(normal.X, normal.Y, normal.Z);
		//	}
		private static float GetAxisValue(Vector3 v, FbxImportSettings.Axis axis)
		{
			switch (axis)
			{
				case FbxImportSettings.Axis.X: return v.X;
				case FbxImportSettings.Axis.Y: return v.Y;
				case FbxImportSettings.Axis.Z: return v.Z;
				case FbxImportSettings.Axis.NegX: return -v.X;
				case FbxImportSettings.Axis.NegY: return -v.Y;
				case FbxImportSettings.Axis.NegZ: return -v.Z;
				default: throw new ArgumentOutOfRangeException(nameof(axis));
			}
		}

		public static void SetFlverMatPath(FLVER2.Material m, string typeName, string newPath)
		{
			for (int i = 0; i < m.Textures.Count; i++)
			{
				if (m.Textures[i].Type == typeName)
				{
					m.Textures[i].Path = newPath;
					return;
				}
			}

			FLVER2.Texture tn = new FLVER2.Texture
			{
				Type = typeName,
				Path = newPath,
				Scale = new Vector2(1, 1),
				Unk10 = 1,
				Unk11 = true
			};
			m.Textures.Add(tn);
		}

		public static string FindFileName(string arg)
		{
			int startIndex = arg.LastIndexOf('/');

			int altStartIndex = arg.LastIndexOf('\\');

			if (altStartIndex > startIndex)
			{
				startIndex = altStartIndex;
			}

			int endIndex = arg.LastIndexOf('.');
			if (startIndex < 0) { startIndex = 0; }
			if (endIndex >= 0)
			{
				//maye "..\\aquatools" endindex = 1 startIndex = 2
				if (startIndex >= endIndex) { endIndex = arg.Length; }

				string res = arg.Substring(startIndex, endIndex - startIndex);
				if ((res.ToCharArray())[0] == '\\' || (res.ToCharArray())[0] == '/')
				{
					res = res.Substring(1);
				}
				return res;
			}

			return arg;
		}
	}
}


// Speaking of which, thanks Forsakensilver for this great program, I couldn't imagine modding Souls games without it.

// Also respect goes to Elden Ring Reforged' team. Online multiplayer? I never even played Elden Ring online before. Mostly because I never play vanilla Souls games.
// I sometimes like to watch ChaseTheBro's videos on YouTube, and I see that PvP in Elden Ring can be fun. 

// Big thanks to SchuhBaum for "Free Lock-On Camera". Very, very, veeeeeery inderrated mod in my opinion. So much so that I can't even play any other Souls game without it xD
// Too bad no one is ported it to other games yet, but I will do it eventually (I hope). Just please don't be pussies who toggles it off for agile enemies ><
// Poor nippon sararymen give the last of their remaining soul to make enemies disappear and attack from behind. Only to see their work multiplied by zero with target locking.

// JKAnderson. For SoulsFormats. I couln't write any program without this DLL.

// Meowmaritus. I was quite impressed with DSAnimStudio, especially considering it had GPU-accelerated rendering and shader selection long before modern software like 
// SmithBox. Probably your program inspired me to make my own... But not before I tried to adapt DSAnimStudio to open .flver files and failed miserably. T_T

// fromsoftserve. Respect for managing to make DS2 Lighting Engine. I watched your videos and know that you are trying to learn shaders. I'm gonna learn them too, and maybe we
// will be able to fix specularity and other shading errors...