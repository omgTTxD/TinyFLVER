using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pfim;
using SharpDX.MediaFoundation;
using SoulsFormats;
using Color = Microsoft.Xna.Framework.Color;

namespace TinyFLVER;

public class Mesh
{
	public CheckBox checkBox;
	public FLVER2.Mesh flverMesh;
	public FLVER2.Material material;

	VertexPositionColorNormalTexture[] vertices = [];
	int[] indices = [];

	BasicEffect effect = MonoGame.Effect;
	Texture2D texture;

	//	static ConcurrentDictionary<string, Texture2D> textureCache = [];
	public Mesh(FLVER2.Mesh mesh, FLVER2.Material material)
	{
		this.flverMesh = mesh;
		this.material = material;
		SetVerticeData();
		Task.Run(TryToFindTextures);
	}

	public void SetVerticeData()
	{
		// Position, normals and UVs don't change ever because we transform them with matrices before every Draw.
		// Only color changes when user selects meshes with mouse or check boxes. 
		vertices = [.. flverMesh.Vertices.Select(
			v => new VertexPositionColorNormalTexture(
				new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
				checkBox?.Checked == true ? Color.Red : Color.White,
				new Vector3(v.Normal.X, v.Normal.Y, v.Normal.Z),
				new Vector2(v.UVs[0].X, v.UVs[0].Y)
		))];

		indices = [.. flverMesh.FaceSets[0].Indices];
	}

	public void SetSelectionState(bool selected)
	{
		checkBox.Checked = selected;

		for (int i = 0; i < vertices.Length; i++)
			vertices[i].Color = selected ? Color.Red : Color.White;
	}

	// Checks if mouse clicked on mesh
	public float CheckIntersection(System.Drawing.Point mouseCrd)
	{
		float minZ = 1;
		foreach (var v in vertices)
		{
			// All other code I saw uses Unproject(). But it didn't work for me. Rays, distance between point and line segment - too hard. I like a simpler way.
			// Just project point to screen again, to get exact screen coordinates. 
			Vector3 screenCrd = MonoGame.Effect.GraphicsDevice.Viewport.Project(v.Position, effect.Projection, effect.View, effect.World);

			// Because we want to select faces on meshes and not only vertices themselves, distance should be big enough. Then we find closest mesh to camera by
			// comparing Z.
			if (Vector2.Distance(new(mouseCrd.X, mouseCrd.Y), new(screenCrd.X, screenCrd.Y)) < 30 && screenCrd.Z < minZ)
				minZ = screenCrd.Z;
		}

		return minZ;
	}

	public void Draw()
	{
		if (indices.Length == 0) return;

		effect.TextureEnabled = texture is not null;
		effect.Texture = texture;

		// We need this to change texture
		foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			pass.Apply();

		MonoGame.Effect.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
	}

	public void TryToFindTextures()
	{
		var textureInfo = material.Textures.Find(x => x.Path.Contains("_a") || x.Path.Contains("_d"));
		string textureName = Path.GetFileNameWithoutExtension(textureInfo?.Path);
		try
		{
			var image = Pfimage.FromFile(textureName + ".dds"); 
			texture = new Texture2D(effect.GraphicsDevice, image.Width, image.Height, false, SurfaceFormat.Bgra32);
			texture.SetData(image.Data, 0, image.DataLen);
		}
		catch { }
	}
}