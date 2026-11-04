#include "Samplers.hlsl"
#pragma warning( disable : 0048 )

void RecalculateTangents(float3 pos, float2 uv, inout float3 T, inout float3 B) {
	float3 dp1perp = cross(cross(ddx(pos), ddy(pos)), ddx(pos));
	float3 dp2perp = cross(ddy(pos), cross(ddx(pos), ddy(pos)));
	T = dp2perp * ddx(uv).x + dp1perp * ddy(uv).x;
	B = dp2perp * ddx(uv).y + dp1perp * ddy(uv).y;
	B = normalize(B - T * dot(T, B));
	T = normalize(T - B * dot(T, B));
}

float3 GetTextureNormal(float2 uv, Texture2D normal)
{
	normal.GetDimensions(w, h);
	if (w == 0) return float3(0, 0, 1);
	
	float2 xy = normal.Sample(s, uv).xy * 2 - 1;
	float z = sqrt(1 - pdot(xy, xy));
	
	return float3(xy, z);
}

float3 GetBlendedDetails(float2 uv)
{
	float3 n1 = GetTextureNormal(uv, Normal);
	float3 n2 = GetTextureNormal(uv * DetailsScale, SkinDetails);
	float3 t = n1 + float3(0, 0, 1);
	float3 u = n2 * float3(-1, -1, 1);
	return normalize(t * dot(t, u) / t.z - u);
}

float3x3 GetTBN(In i)
{
	// Tangents should be normalized after interpolation, but it makes them differ in contrast from recalculated tangents MUCH more
	float3 N = normalize(i.normal);
	float3 B = i.tangent;
	float3 T = normalize(cross(N, B)) * i.tangent.w;
	if (RecalcTB) RecalculateTangents(i.pos, i.uv, T, B);
	
	T *= FlipX; B *= FlipY;
	if (SwapXY) swap(T, B);
	return float3x3(T, B, N);
}

float3 GetNormal(In i)
{	
	if (formatID == Spheres) return normalize(i.normal);
	SkinDetails.GetDimensions(w, h);
	float3 textureN = w == 0 ? GetTextureNormal(i.uv, Normal) : GetBlendedDetails(i.uv);
	return normalize(mul(textureN, GetTBN(i)));
}
