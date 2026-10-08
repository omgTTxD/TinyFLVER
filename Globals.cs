global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using System.Windows.Forms;
global using static G;
global using static Renderer;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;
using System.ComponentModel;

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

	public static Matrices M;
	public static ShaderData cb;
	public static Texture2D backBuffer;

	public static RenderTargetView CreateRenderTarget(Texture2DDescription desc) => new(d, new Texture2D(d, desc));
	public static DepthStencilView CreateDepth(Texture2DDescription desc) => new(d, new Texture2D(d, desc));

	public static BindingList<Mesh> Meshes = [];
	public static Mesh nearestMesh;

	public static List<Mesh> selectedMeshes => form.dgMeshes.SelectedRows.Cast<DataGridViewRow>().Select(r => Meshes[r.Index]).ToList();
	public static List<Mesh> visibleMeshes => Meshes.Where(m => !m.Hidden).ToList();
	public static List<Mesh> hiddenMeshes => Meshes.Where(m => m.Hidden).ToList();

	public static float toRadians(double degrees) { return (float)(3.141592 / 180 * degrees); }
	public static float cos(float degrees) => (float)Math.Cos(toRadians(degrees));
	public static float sin(float degrees) => (float)Math.Sin(toRadians(degrees));

	public static bool ContainsAny(this string s, params string[] v) => v.Any(v =>  (s ?? "").Contains(v, StringComparison.OrdinalIgnoreCase));
	public static bool EndsWithAny(this string s, params string[] v) => v.Any(v => (s ?? "").EndsWith(v, StringComparison.OrdinalIgnoreCase));
	public static bool AnyOf(this object o, params object[] v) => v.Any(x => o.Equals(x));

	public static bool IsSkin(this string s) => (s ?? "").ContainsAny("Skin", "Body", "Face", "Head", "Arms", "Legs");
	public static bool IsMetal(this FLVER2.Material m) => m.MTD.ContainsAny("Metal");
}

public struct vec2(double x, double y) {
	public float x = (float)x, y = (float)y;
	public static implicit operator vec2(Point v) => new(v.X, v.Y);
	public static implicit operator vec2(Vector3 v) => new(v.X, v.Y);
	public static vec2 operator *(vec2 v, double f) => new(v.x * f, v.y * f);
	public static vec2 operator /(vec2 v, double f) => new(v.x / f, v.y / f);
	public static vec2 operator +(vec2 a, vec2 b) => new(a.x + b.x, a.y + b.y);
	public static vec2 operator -(vec2 a, vec2 b) => new(a.x - b.x, a.y - b.y);
}

public struct vec3(double x, double y, double z) {
	public float x = (float)x, y = (float)y, z = (float)z;
	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
	public static implicit operator Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector3 v) => new(v.X, v.Y, v.Z);
	public static vec3 operator *(vec3 v, double f) => new(v.x * f, v.y * f, v.z * f);
	public static vec3 operator /(vec3 v, double f) => new(v.x / f, v.y / f, v.z / f);
	public static vec3 operator +(vec3 a, vec3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
	public static vec3 operator -(vec3 a, vec3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
}

public struct vec4(double x, double y, double z, double w) {
	public float x = (float)x, y = (float)y, z = (float)z, w = (float)w;
	public static implicit operator vec4(Vector4 v) => new(v.X, v.Y, v.Z, v.W);
}