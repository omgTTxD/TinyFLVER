global using static TinyFLVER.Globals;
using SharpDX.Direct3D11;
using SharpDX.Windows;

namespace TinyFLVER;

public class Globals
{
	public static Device dev;
	public static DeviceContext c;
	public static MainForm form;
	public static RenderControl r;

	public static List<Mesh> Meshes = [];
	public static List<Mesh> Selected = [];
	public static List<Mesh> Hidden = [];

	public static string path;

	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		RenderLoop.Run(new MainForm(), SwapChainManager.Render);
	}
}
