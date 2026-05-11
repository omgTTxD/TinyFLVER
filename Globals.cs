global using static G;
global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using System.Windows.Forms;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using Matrix = SharpDX.Matrix;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;

public class G
{
	public static SharpDX.Direct3D11.Device d;
	public static DeviceContext c;
	public static MainForm form;
	public static FLVER2 flver;

	public static List<Mesh> Meshes = [];
	public static Mesh nearestMesh;

	public static Matrices M;
	public static ShaderData shaderData;
	public static float ticks;

	public static float toRadians(float degrees) { return 3.14159265f / 180 * degrees; }
	public static float cos(float degrees) => (float)Math.Cos(toRadians(degrees));
	public static float sin(float degrees) => (float)Math.Sin(toRadians(degrees));

	[STAThread]
	static void Main(string[] args)
	{
		string path = args.ElementAtOrDefault(0);

		if (path is null || path.EndsWith(".flver"))
		{
			Flver.path = path;
			Application.EnableVisualStyles();
			SharpDX.Windows.RenderLoop.Run(new MainForm(), SwapChainManager.Render);
		}
		else
		{
			if (path.EndsWith(".yab"))
				Yabber.Repack(path);
			else if (path.EndsWith("bnd.dcx"))
				Yabber.UnpackBnd(path);
			else if (path.EndsWith(".tpf.dcx") || path.EndsWith(".tpf"))
				Yabber.UnpackStandaloneTPF(path);
		}
	}

}

public struct vec(float x)
{
	public float x = x;
	public static implicit operator vec(int f) => new(f);
	public static implicit operator vec(double f) => new((float)f);
	public static implicit operator vec(decimal f) => new((float)f);
	public static implicit operator string(vec v) => v.x.ToString("0.000");
	public static implicit operator decimal(vec v) => (decimal)v.x;
}

public struct vec2(double x, double y)
{
	public float x = (float)x, y = (float)y;
	public static vec DistanceSquared(vec2 a, vec2 b) => Vector2.DistanceSquared(new(a.x, a.y), new(b.x, b.y));
	public static implicit operator vec2(Point v) => new(v.X, v.Y);
	public static vec2 operator *(vec2 v, double f) => new((v.x * f), (v.y * f));
	public static vec2 operator /(vec2 v, double f) => new((v.x / f), (v.y / f));
	public static vec2 operator +(vec2 a, vec2 b) => new(a.x + b.x, a.y + b.y);
	public static vec2 operator -(vec2 a, vec2 b) => new(a.x - b.x, a.y - b.y);
}

public struct vec3(double x, double y, double z)
{
	public float x = (float)x, y = (float)y, z = (float)z;
	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }

	public static implicit operator SharpDX.Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(SharpDX.Vector3 v) => new(v.X, v.Y, v.Z);

	public static implicit operator Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector3 v) => new(v.X, v.Y, v.Z);
}