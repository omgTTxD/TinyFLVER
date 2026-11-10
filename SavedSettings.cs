using Newtonsoft.Json;

class CameraSettings()
{
	static string settingsPath = AppContext.BaseDirectory + "CameraSettings.txt";
	static Dictionary<string, CameraSettings> dictionary = [];

	public vec3 cameraOffset = Camera.offset;
	public vec2 meshRotation = Camera.camera;
	public vec3 lightRotation = Camera.light;

	public static void Initialize()
	{
		if (!File.Exists(settingsPath))
			File.Create(settingsPath).Dispose();

		dictionary = JsonConvert.DeserializeObject<Dictionary<string, CameraSettings>>(File.ReadAllText(settingsPath)) ?? [];
	}

	public static void Save()
	{
		if (Flver.path is null) return;
		dictionary[Flver.path] = new();
		File.WriteAllText(settingsPath, JsonConvert.SerializeObject(dictionary, Formatting.Indented));
	}

	public static void Load()
	{
		if (dictionary.TryGetValue(Flver.path, out var s)) {
			Camera.offset = s.cameraOffset;
			Camera.camera = s.meshRotation;
			Camera.light = s.lightRotation;
		}
	}
}

public class RecentLocation
{
	public static string recentFiles = AppContext.BaseDirectory + "SavedLocations.txt";
	public static BindingList<string> dataSource;

	static bool reordering;

	public static void Initialize()
	{
		if (!File.Exists(recentFiles))
			File.Create(recentFiles).Dispose();

		form.RecentFiles.FormattingEnabled = true;
		form.RecentFiles.Format += (s, e) => e.Value = $"{Directory.GetParent(e.Value.ToString()).Name} ({Path.GetFileNameWithoutExtension(e.Value.ToString().ToLower())})";

		dataSource = new BindingList<string>(File.ReadAllLines(RecentLocation.recentFiles).Where(x => x != "" && File.Exists(x)).ToList()) ?? [];
		form.RecentFiles.DataSource = dataSource;
		form.RecentFiles.DisplayMember = "Directory.GetParent(Name).Name";

		form.RecentFiles.SelectedValueChanged += SelectedMeshChanged;

		form.KeyDown += (s, e) =>
		{
			if (e.KeyCode == Keys.Delete && form.RecentFiles.Focused)
			{
				dataSource.RemoveAt(0);
				Flver.LoadFLVER(form.RecentFiles.SelectedValue?.ToString());
			}
		};
	}

	public static void MoveToTop(string item)
	{

		reordering = true;
		dataSource.Remove(item);
		dataSource.Insert(0, item);
		form.RecentFiles.SelectedIndex = 0;
		form.RecentFiles.Size = new Size(form.RecentFiles.Width, (form.RecentFiles.Items.Count + 1) * form.RecentFiles.ItemHeight);
		reordering = false;
		Save();
	}

	public static void Save()
	{
		File.WriteAllLines(recentFiles, dataSource.ToList());
	}

	public static void SelectedMeshChanged(object sender, EventArgs e)
	{
		if (reordering || form.RecentFiles.SelectedIndex == -1)
			return;

		CameraSettings.Save();
		Flver.LoadFLVER(form.RecentFiles.SelectedValue?.ToString());
	}
}