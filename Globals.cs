global using static Globals;
global using SharpDX.D3DCompiler;
global using SharpDX.Direct3D;
global using SharpDX.Direct3D11;
global using SharpDX.DXGI;
global using SoulsFormats;
global using System.Numerics;
global using System.Runtime.InteropServices;
global using System.Windows.Forms;
global using Buffer = SharpDX.Direct3D11.Buffer;
global using Matrix = SharpDX.Matrix;
global using SRV = SharpDX.Direct3D11.ShaderResourceView;

public class Globals
{
	public static SharpDX.Direct3D11.Device d;
	public static DeviceContext c;
	public static MainForm form;
	public static FLVER2 flver;

	public static List<Mesh> Meshes = [];
	public static Mesh nearestMesh;

	public static Matrices matrices;
	public static ShaderData shaderData;
	public static float ticks;


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