using SharpDX.Windows;
using System.Linq;

public class Overlay : RenderForm
{
	static Graphics g;

	public static void Initialize()
	{
		overlay = new Overlay();
		overlay.Location = form.PointToScreen(Point.Empty);

		g = overlay.CreateGraphics();
		g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

		// Make that vertices are projected only once after last MouseWheel event
		System.Windows.Forms.Timer wheelTimer = new() { Interval = 50 };
		form.MouseWheel += (s, e) => {
			wheelTimer.Stop(); 
			wheelTimer.Start();
			overlay.Refresh();
		};

		wheelTimer.Tick += (s, e) => { 
			wheelTimer.Stop(); 
			ProjectVertices(); 
		};

		form.MouseMove += (s, e) => {
			if (e.Button != MouseButtons.None)
				overlay.Refresh();
		}; 
		
		form.refreshTimer.Tick += (s, e) => {
			Overlay.FindNearestMesh(); 
		};

		form.MouseUp += (s, e) => {
			ProjectVertices();

			if (e.Button == MouseButtons.Left)
			{
				FindNearestVertex();
				overlay.Refresh();
			}
		};
	}

	static void CheckMesh(Mesh m, ref float minZ)
	{
		foreach (var v in m.projectedVertices)
		{
			if (vec2.DistanceSquared(Camera.mousePos, v.Position.xy) < 250 && v.Position.z < minZ)
			{
				minZ = v.Position.z;
				nearestMesh = m;
			}
		}
	}
	public static void ProjectVertices()
	{
		Parallel.ForEach(Meshes, m => m.projectedVertices = [.. m.vertices.Select(v => new Vertex() { Position = v.Position.project() })]);
	}

	public static void FindNearestMesh()
	{
		nearestMesh = null;
		float minZ = 1;

		Meshes.AsParallel().Where(m => !m.Hidden).ForAll(m => CheckMesh(m, ref minZ));

		if (nearestMesh == null)
			Meshes.AsParallel().Where(m => m.Hidden).ForAll(m => CheckMesh(m, ref minZ));
	}

	public static void FindNearestVertex()
	{
		if (nearestMesh is null) 
			return;

		nearestVertex = new Vertex();
		float minZ = 1;

		Parallel.ForEach(nearestMesh.vertices, v => 
		{
			if (vec2.DistanceSquared(Camera.mousePos, v.Position.project().xy) < 250 && v.Position.z < minZ)
			{
				minZ = v.Position.z;
				nearestVertex = v;
			}
		});
	}


	static void PaintVertexNormal(object sender, PaintEventArgs e)
	{
		if (nearestVertex.Position == new vec3(0, 0, 0))
			return;

		g.Clear(overlay.BackColor);

		var P = nearestVertex.Position.project().xy;
		vec3 N = nearestVertex.Normal;
		vec3 T = nearestVertex.Tangent.xyz;

		DrawVector(N, P, Color.Blue);
		DrawVector(T, P, Color.Red);
	}

	static void DrawVector(vec3 vec, vec2 startPos, Color color)
	{
		Pen pen = new Pen(color, 1.5f) {  EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
		g.DrawLine(pen, startPos, startPos + (vec * 200 / Camera.offset.z).projectNormal().xy);
	}

	Overlay()
	{
		TransparencyKey = BackColor;
		FormBorderStyle = FormBorderStyle.None;
		ShowInTaskbar = false;
		Size = form.ClientSize;
		Show(form);

		form.Move += (s, e) => Location = form.PointToScreen(Point.Empty);
		form.Resize += (s, e) => Size = form.ClientSize;
		Paint += PaintVertexNormal;
	}

	// Transparency for mouse clicks
	protected override CreateParams CreateParams { get { var cp = base.CreateParams; cp.ExStyle |= 0x20; return cp; } }
}