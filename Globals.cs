global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using static Globals;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using Matrix = SharpDX.Matrix;
using System.IO;

public class Globals
{
	public static SharpDX.Direct3D11.Device dev;
	public static DeviceContext c;
	public static MainForm form;

	public static List<Mesh> Meshes = [];
	public static List<Mesh> Selected = [];
	public static List<Mesh> Hidden = [];

	public static VSData vsData;
	public static PSData psData;

	public static float toRadians(float x) { return Pi / 180 * x; }
	public static int toDegrees(float x) { return (int)(180 / Pi * x + 0.5 * x / Math.Abs(x)); }
	public static float Cos(float x) { return (float)Math.Cos(toRadians(x)); }
	public static float Sin(float x) { return (float)Math.Sin(toRadians(x)); }
	public static float Pi = 3.14159265f;

	[STAThread]
	static void Main(string[] args)
	{
		string path = args.ElementAtOrDefault(0);

		if (path is null || path.EndsWith(".flver"))
		{
			Application.EnableVisualStyles();
			SharpDX.Windows.RenderLoop.Run(new MainForm(), SwapChainManager.Render);
		}
		else
		{
			if (path.EndsWith(".yab"))
				Yabber.Repack(path);
			else if (path.EndsWith(".partsbnd.dcx"))
				Yabber.UnpackBnd(path);
			else if (path.EndsWith(".tpf.dcx"))
				Yabber.UnpackStandaloneTPF(path);
		}
	}
}