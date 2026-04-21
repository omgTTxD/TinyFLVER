using SharpDX.Windows;

namespace TinyFLVER;

static class Program
{
	[STAThread]
	static void Main()
	{
		Application.EnableVisualStyles();
		RenderLoop.Run(new MainForm(), SwapChainManager.Render);
	}
}