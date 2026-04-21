namespace TinyFLVER;

static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		Application.EnableVisualStyles();
		Application.Run(new MainForm());
	}
}