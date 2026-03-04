global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using static TinyFLVER.Globals;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using System.Numerics;
using SharpDX.Windows;
using SharpDX;

using Device = SharpDX.Direct3D11.Device;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace TinyFLVER;

public struct VSData
{
	public Matrix WorldViewProj;
	public Matrix WorldView;
	public Vector2 Offset;
	float pad1, pad2;
}

public struct PSData()
{
	public Vector3 LightPosition;
	public float Gamma = 80;
	public float Exposure = 10;
	public int DebugID;
	float pad1, pad2;
}

public class Globals
{
	public static Device dev;
	public static DeviceContext c;
	public static MainForm form;

	public static List<Mesh> Meshes = [];
	public static List<Mesh> Selected = [];
	public static List<Mesh> Hidden = [];

	public static VSData vsData;
	public static PSData psData;

//	public static string path;
	public static string path = "A:\\TinyFLVER\\bd_m_5070\\BD_M_5070.flver";
//	public static string path = "D:\\Models\\Gwynevere\\Orig\\c5310.flver";

	public static float toRadians(float x) { return Pi / 180 * x; }
	public static int toDegrees(float x) { return (int)(180 / Pi * x + 0.5); }
	public static float Cos(float x) { return (float)Math.Cos(x); }
	public static float Sin(float x) { return (float)Math.Sin(x); }
	public static float Pi = 3.14159265f;

	[STAThread]
	static void Main()
	{
		RenderLoop.Run(new MainForm(), SwapChainManager.Render);
	}
}
