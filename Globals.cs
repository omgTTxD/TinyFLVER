global using SharpDX.D3DCompiler;
global using SharpDX;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.ComponentModel;
global using System.Numerics;
global using System.Windows.Forms;
global using static G;
global using static Renderer;
global using Vector2 = System.Numerics.Vector2;
global using Vector3 = System.Numerics.Vector3;
global using Vector4 = System.Numerics.Vector4;

global using Buffer = SharpDX.Direct3D11.Buffer;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;
global using Point = System.Drawing.Point;
global using Color = System.Drawing.Color;

class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		SharpDX.Configuration.EnableReleaseOnFinalizer = true;
		Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
		Application.ThreadException += (s, e) => MessageBox.Show(e.Exception.Message, "Error");
		AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show((e.ExceptionObject as Exception).Message, "Error");
		Application.EnableVisualStyles();

		if (args.Length == 0 || args[0].EndsWith(".flver") || args[0].EndsWith(".bak"))
		{
			Flver.path = args.FirstOrDefault();
			Application.Run(new MainForm());
		}
		else
			Yabber.OpenWithYabber(args[0]);
	}
}

static class G
{
	[System.Runtime.InteropServices.DllImport("AcrylicWindow.dll")]
	public static extern void ApplyAcrylic(IntPtr hWnd, int radius = 76, float saturation = 1.5f);

	public static int w => form.ClientSize.Width;
	public static int h => form.ClientSize.Height;

	public static MainForm form;
	public static FLVER2 flver;

	public static ShaderData cb;
	public static Texture2D backBuffer;

//	public static RenderTargetView CreateRenderTarget(Texture2DDescription desc) => new(d, new Texture2D(d, desc));
	public static DepthStencilView CreateDepth(Texture2DDescription desc) => new(d, new Texture2D(d, desc with { BindFlags = BindFlags.DepthStencil }));

	public static BindingList<Mesh> Meshes = [];
	public static Mesh nearestMesh => Meshes.ElementAtOrDefault(nearestMeshIndex);
	public static int nearestMeshIndex;// => (int)(Utilities.Read<float>(Outline.dataPointer)) - 1;


	public static List<Mesh> selectedMeshes => form.dgMeshes.SelectedRows.Cast<DataGridViewRow>().Select(r => Meshes[r.Index]).ToList();
	public static List<Mesh> visibleMeshes => Meshes.Where(m => !m.Hidden).ToList();
	public static List<Mesh> hiddenMeshes => Meshes.Where(m => m.Hidden).ToList();

	public static vec2 toRadians(vec2 rotation) { return rotation * 3.141592 / 180; }

	public static bool ContainsAny(this string s, params string[] v) => v.Any(v => (s ?? "").Contains(v, StringComparison.OrdinalIgnoreCase));
	public static bool EndsWithAny(this string s, params string[] v) => v.Any(v => (s ?? "").EndsWith(v, StringComparison.OrdinalIgnoreCase));
	public static bool AnyOf(this object o, params object[] v) => v.Any(x => o.Equals(x));
	public static bool IsSkin(string s) => (s ?? "").ContainsAny("Skin", "Body", "Face", "Head", "Arms", "Legs");
	//public static bool IsSkin(this string s) => (s ?? "").ContainsAny("Skin", "Body", "Face", "Head", "Arms", "Legs");
	public static bool IsMetal(this FLVER2.Material m) => m.MTD.ContainsAny("Metal");

	public static Matrix4x4 Invert(Matrix4x4 m) { Matrix4x4.Invert(m, out var inv); return inv; }
	public static Matrix4x4 RotationYX(vec2 rotation) => Matrix4x4.CreateRotationY(-rotation.x) * Matrix4x4.CreateRotationX(-rotation.y, new(0, 1, 0));
	public static Matrix4x4 LookAtLH(vec3 offset) => Matrix4x4.CreateLookAtLeftHanded(offset, Vector3.UnitY, Vector3.UnitY);
	public static Matrix4x4 PerspectiveLH(float fov = 0.7f, float near = 0.01f, float far = 100f) => Matrix4x4.CreatePerspectiveFieldOfViewLeftHanded(fov, (float)w / h, near, far);

	public static void Sync() => c.UpdateSubresource(ref cb, Shaders.psBuffer); 


}
public struct vec1(double x)
{
	public float x = (float)x;
	public static implicit operator vec1(double a) => new(a);
	public static implicit operator vec1(float a) => new(a);
	public static implicit operator string(vec1 v) => v.x.ToString("0.000");
	public static vec1 operator *(vec1 v, float f) => new(v.x * f);
	public static vec1 operator *(float f, vec1 v) => new(v.x * f);
	public static vec1 operator /(vec1 v, double f) => new(v.x / f);
	

	// Возможно спорно и неправильно, но нужно было для матрицы скейла.
	public static implicit operator System.Numerics.Vector3(vec1 v) => new(v.x, v.x, v.x);
}

public struct vec2(double x, double y)
{
	public float x = (float)x, y = (float)y;
	public static implicit operator vec2(Point v) => new(v.X, v.Y);
	public static implicit operator vec2(Vector3 v) => new(v.X, v.Y);
	public static implicit operator vec2(Vector2 v) => new(v.X, v.Y);
	public static implicit operator Vector2(vec2 v) => new(v.x, v.y);
	public static vec2 operator *(vec2 v, double f) => new(v.x * f, v.y * f);
	public static vec2 operator *(vec2 a, vec2 b) => new(a.x * b.x, a.y * b.y);
	public static vec2 operator /(vec2 v, double f) => new(v.x / f, v.y / f);
	public static vec2 operator +(vec2 a, vec2 b) => new(a.x + b.x, a.y + b.y);
	public static vec2 operator -(vec2 a, vec2 b) => new(a.x - b.x, a.y - b.y);
}


public struct vec3(double x, double y, double z)
{
	public float x = (float)x, y = (float)y, z = (float)z;

	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
	public vec2 xz { get => new(x, z); set { x = value.x; z = value.y; } }
	public static vec3 up = new vec3(0, 1, 0);
	public static implicit operator Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector3 v) => new(v.X, v.Y, v.Z);



	//	public static implicit operator Vector4(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector4 v) => new(v.X / v.W, v.Y / v.W, v.Z / v.W);
	public static implicit operator vec3(vec4 v) => new(v.x / v.w, v.y / v.w, v.z / v.w);
	public static vec3 operator *(vec3 v, double f) => new(v.x * f, v.y * f, v.z * f);
	public static vec3 operator /(vec3 v, double f) => new(v.x / f, v.y / f, v.z / f);

	public static vec3 operator +(vec3 a, vec3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
	public static vec3 operator -(vec3 a, vec3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
	public static vec3 operator -(vec3 a) => new(a.x, a.y, a.z);
	public static float length(vec3 a) => ((Vector3)a).Length();

}


public struct vec4(double x, double y, double z, double w)
{
	public float x = (float)x, y = (float)y, z = (float)z, w = (float)w;
	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
	public static implicit operator vec4(vec2 v) => new(v.x, v.y, 0, 1);
	public static implicit operator vec4(vec3 v) => new(v.x, v.y, v.z, 1);
	public static implicit operator vec4(Vector4 v) => new(v.X, v.Y, v.Z, v.W);
	public static implicit operator vec4(Vector3 v) => new(v.X, v.Y, v.Z, 1);
	public static implicit operator System.Numerics.Vector4(vec4 v) => new(v.x, v.y, v.z, v.w);
	public vec1 length() => ((Vector4)this).Length();
}