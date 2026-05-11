global using static G;
global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using System.Windows.Forms;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;
using SharpDX.Windows;

public class G
{
	public static SharpDX.Direct3D11.Device d;
	public static DeviceContext c;
	public static MainForm form;
	public static Overlay overlay;
	public static FLVER2 flver;

	public static Matrices M;
	public static ShaderData shaderData;
	public static int w, h;

	public static List<Mesh> Meshes = [];
	public static Mesh nearestMesh;
	public static Vertex nearestVertex;

	public static float toRadians(float degrees) { return 3.14159265f / 180 * degrees; }
	public static float cos(float degrees) => (float)Math.Cos(toRadians(degrees));
	public static float sin(float degrees) => (float)Math.Sin(toRadians(degrees));

	public static Matrix4x4 PerspectiveFovLH(float fov, float aspect, float znear, float zfar)
	{
		float num = 1 / (float)Math.Tan(fov * 0.5);
		float num2 = zfar / (zfar - znear);
		return default(Matrix4x4) with { M11 = num / aspect, M22 = num, M33 = num2, M34 = 1, M43 = (0 - num2) * znear };
	}

	[STAThread]
	static void Main(string[] args)
	{
		Application.EnableVisualStyles();

		if (args.Length == 0 || args[0].EndsWith(".flver"))
		{
			Flver.path = args.FirstOrDefault();
			form = new MainForm();
			RenderLoop.Run(form, SwapChainManager.Render);
		}
		else
			Yabber.OpenWithYabber(args[0]);
	}
}

public struct vec(double x)
{
	public float x = (float)x;
	public static implicit operator vec(float f) => new(f);
	public static implicit operator double(vec v) => v.x;
	public static implicit operator string(vec v) => v.x.ToString("0.000");
}

public struct vec2(double x, double y)
{
	public float x = (float)x, y = (float)y;
	public static vec DistanceSquared(vec2 a, vec2 b) => Vector2.DistanceSquared(a, b);

	public static implicit operator vec2(Point v) => new(v.X, v.Y);
	public static implicit operator PointF(vec2 v) => new(v.x, v.y);

	public static implicit operator Vector2(vec2 v) => new(v.x, v.y);
	public static implicit operator vec2(Vector2 v) => new(v.X, v.Y);

	public static vec2 operator *(vec2 v, float f) => new((v.x * f), (v.y * f));
	public static vec2 operator /(vec2 v, float f) => new((v.x / f), (v.y / f));
	public static vec2 operator +(vec2 a, vec2 b) => new(a.x + b.x, a.y + b.y);
	public static vec2 operator -(vec2 a, vec2 b) => new(a.x - b.x, a.y - b.y);
}

public struct vec3(double x, double y, double z)
{
	public float x = (float)x, y = (float)y, z = (float)z;
	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
	public static vec3 operator *(vec3 v, float f) => new(v.x * f, v.y * f, v.z * f);
	public static vec3 operator /(vec3 v, float f) => new(v.x / f, v.y / f, v.z / f);
	public static bool operator ==(vec3 a, vec3 b) => a.x == b.x && a.y == b.y && a.z == b.z;
	public static bool operator !=(vec3 a, vec3 b) => a.x != b.x || a.y != b.y || a.z == b.z;
	public static implicit operator Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector3 v) => new(v.X, v.Y, v.Z);
	public static implicit operator vec3(Vector4 v) => new(v.X / v.W, v.Y / v.W, v.Z / v.W);
	public static vec3 cross(vec3 a, vec3 b) => Vector3.Cross(a, b);
	public vec3 normalize() => Vector3.Normalize(this);

	public vec3	project() 
	{
		vec3 ndc = Vector4.Transform(new Vector4(this, 1), M.World * M.Proj);
		return new vec3((ndc.x + 1) * 0.5f * w, (1 - ndc.y) * 0.5f * h, ndc.z);
	}

	public vec3 projectNormal() 
	{
 		vec3 ndc = Vector3.TransformNormal(this, M.World);
		return new((ndc.x + 1) * 0.5f, (1 - ndc.y) * 0.5f, ndc.z);
	}
}

public struct vec4(float x, float y, float z, float w)
{
	public float x = x, y = y, z = z, w = w;
	public vec3 xyz => new(x, y, z); 
	public vec4 normalize() => Vector4.Normalize(this);
	public static implicit operator Vector4(vec4 v) => new(v.x / v.w, v.y / v.w, v.z / v.w, 1);
	public static implicit operator vec4(vec3 v) => new(v.x, v.y, v.z, 1);
	public static implicit operator vec4(Vector3 v) => new(v.X, v.Y, v.Z, 1);
	public static implicit operator vec4(Vector4 v) => new(v.X / v.W, v.Y / v.W, v.Z / v.W, 1);
}