global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using System.Windows.Forms;
global using static G;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;
using Microsoft.VisualBasic.Devices;
using System.ComponentModel;
using Device = SharpDX.Direct3D11.Device;


class G {
	public static Device d;
	public static DeviceContext c;
	public static MainForm form;
	public static FLVER2 flver;

	public static Matrices M;
	public static ShaderData shaderData;
	public static int w, h;

	public static BindingList<Mesh> Meshes = [];
	public static List<Mesh> SelectedMeshes => form.dataGridMeshes.SelectedRows.Cast<DataGridViewRow>().Select(r => Meshes[r.Index]).ToList();
	public static Mesh nearestMesh; 



	public static int ticks;

	public static float toRadians(float degrees) { return 3.14159265f / 180 * degrees; }
	public static float cos(float degrees) => (float)Math.Cos(toRadians(degrees));
	public static float sin(float degrees) => (float)Math.Sin(toRadians(degrees));

	[STAThread]
	static void Main(string[] args)
	{
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

public struct vec(double x) 
{
	public float x = (float)x;
	public static implicit operator vec(double f) => new(f);
	public static implicit operator float(vec v) => v.x;
	public static implicit operator string(vec v) => v.x.ToString("0.000");
}

public struct vec2(double x, double y)
{
	public float x = (float)x, y = (float)y;
	public static implicit operator vec2(Point v) => new(v.X, v.Y);
	public static implicit operator Vector2(vec2 v) => new(v.x, v.y);
	public static vec2 operator *(vec2 v, double f) => new((v.x * f), (v.y * f));
	public static vec2 operator /(vec2 v, double f) => new((v.x / f), (v.y / f));
	public static vec2 operator +(vec2 a, vec2 b) => new(a.x + b.x, a.y + b.y);
	public static vec2 operator -(vec2 a, vec2 b) => new(a.x - b.x, a.y - b.y);
}

public struct vec3(double x, double y, double z)
{
	public float x = (float)x, y = (float)y, z = (float)z;
	public vec2 xy { get => new(x, y); set { x = value.x; y = value.y; } }
	public static implicit operator Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(Vector3 v) => new(v.X , v.Y, v.Z);
	public static implicit operator vec3(Vector4 v) => new(v.X / v.W, v.Y / v.W, v.Z / v.W);
	public vec3 project()
	{
		vec3 ndc = Vector4.Transform(new Vector4(this, 1), M.MVP);
		return new vec3((ndc.x + 1) * 0.5f * w, (1 - ndc.y) * 0.5f * h, ndc.z);
	}
}