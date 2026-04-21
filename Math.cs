global using static Functions;

public abstract class Functions
{
	public static float Pi = 3.14159265f;
	public static float toRadians(float degrees) { return Pi / 180 * degrees; }
	public static float cos(float degrees) => (float)Math.Cos(toRadians(degrees));
	public static float sin(float degrees) => (float)Math.Sin(toRadians(degrees));
}

public struct vec2(float x, float y)
{
	public float X = x, Y = y;

	public static vec2 operator *(vec2 v, double f) => new((float)(v.X * f), (float)(v.Y * f));
	public static vec2 operator /(vec2 v, double f) => new((float)(v.X / f), (float)(v.Y / f));

	public static implicit operator Point(vec2 v) => new((int)v.X, (int)v.Y);
	public static implicit operator vec2(Point v) => new(v.X, v.Y);

	public static implicit operator System.Numerics.Vector2(vec2 v) => new(v.X, v.Y);
	public static implicit operator vec2(System.Numerics.Vector2 v) => new(v.X, v.Y);

	public static vec2 operator +(vec2 a, Point b) => new(a.X + b.X, a.Y + b.Y);
	public static vec2 operator -(vec2 a, Point b) => new(a.X - b.X, a.Y - b.Y);
}

public struct vec3(float x, float y, float z)
{
	public float x = x, y = y, z = z;
	public Vector2 xy => new(x, y);

	public static implicit operator SharpDX.Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(SharpDX.Vector3 v) => new(v.X, v.Y, v.Z);

	public static implicit operator System.Numerics.Vector3(vec3 v) => new(v.x, v.y, v.z);
	public static implicit operator vec3(System.Numerics.Vector3 v) => new(v.X, v.Y, v.Z);
}

public struct mat
{
	public float[] arr;
	public mat(float[][] m)
	{
		arr = new float[16];
		for (int i = 0; i < 4; i++)
			for (int j = 0; j < 4; j++)
				arr[i * 4 + j] = m[i][j];
	}

	public static implicit operator SharpDX.Matrix(mat m) => new(m.arr);

}