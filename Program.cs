class Program
{
	[STAThread]
	static void Main(string[] args)
	{
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
