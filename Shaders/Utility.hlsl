#include "Samplers.hlsl"

void RecalculateTangents(float3 pos, float2 uv, float3 N, out float3 T, out float3 B)
{
	float3 dp2perp = cross(ddy(pos), cross(ddx(pos), ddy(pos)));
	float3 dp1perp = cross(cross(ddx(pos), ddy(pos)), ddx(pos));
	T = normalize(dp2perp * ddx(uv).x + dp1perp * ddy(uv).x);
	B = normalize(dp2perp * ddx(uv).y + dp1perp * ddy(uv).y);
}

float3 GetNormalFromTexture(float2 uv, Texture2D normal)
{
	normal.GetDimensions(w, h);
	if (w == 0) return float3(0, 0, 1);
	float2 xy = normal.Sample(Wrapped, uv).xy * 2 - 1;
	
	xy *= int2(FlipX, FlipY);
	if (SwapXY)	xy.xy = xy.yx;
	
	float z = sqrt(saturate(1 - dot(xy, xy))) * FlipZ;
	return normalize(float3(xy, z));
}

float3 BlendDetails(float3 n1, float2 uv)
{
	float3 n2 = GetNormalFromTexture(uv * 20, SkinDetails);
	float3 t = n1 + float3(0, 0, 1);
	float3 u = n2 * float3(-1, -1, 1);
	return normalize(t * dot(t, u) / t.z - u);
}

float3 GetNormal(Out i)
{
	if (formatID == 3)
		return normalize(i.N);

	float3 VertexN = normalize(i.N), T, B,
	TextureN = GetNormalFromTexture(i.uv, Normal);
	
	if (IsSkin)
		TextureN = BlendDetails(TextureN, i.uv);	
	
	float3 FlverB = i.T;
	float3 FlverT = normalize(cross(VertexN, FlverB)) * i.T.w;
	
	RecalculateTangents(i.pos.xyz, i.uv, VertexN, T, B);
		
	if (formatID == 1 && RecalcTB) 
		swap(T, B);	
	
	float3x3 TBN = RecalcTB ? float3x3(T, B, VertexN) : float3x3(FlverT, FlverB, VertexN);

	return normalize(mul(TextureN, TBN));
}


float3 Fresnel(float3 F0, float VdotH)				
{ 
	return lerp(F0, 1, pow(1 - VdotH, 5)); 
}

float3 FresnelIBL(float3 F0, float VdotN, float r)	
{ 
//	return lerp(F0, max(1 - r, F0), pow(1 - VdotN, 5)); 
	return F0 + max(1 - r, F0) * pow(1 - VdotN, 5); 
}

float Distribution(float NdotH, float roughness)
{
	float a = pow(roughness, 4);
	float denom = NdotH * NdotH * (a - 1) + 1;
	return a / max(1e-9, pi * pow(denom, 2));
}

float Geometry(float3 N, float3 V, float3 L, float roughness)
{
	float k = pow((roughness + 1), 2) / 8;
	//float k = pow(roughness, 2) / 2;
	float ggx1 = pdot(N, L) / (pdot(N, L) * (1 - k) + k);
	float ggx2 = pdot(N, V) / (pdot(N, V) * (1 - k) + k);
	return ggx1 * ggx2;
}


