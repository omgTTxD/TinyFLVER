#include "Samplers.hlsl"

float Distribution(float NdotH, float r)
{
	float roughness = pow(r, 4);
	float denom = NdotH * NdotH * (roughness - 1) + 1;
	return roughness / (pi * pow(denom, 2));
}

float3 Fresnel(float3 F0, float VdotH)
{
	return F0 + (1 - F0) * pow(1 - VdotH, 5);
}

float3 FresnelIBL(float3 F0, float VdotN, float r)
{
	return F0 + max(pow(1 - r, 1), F0) * pow(1 - VdotN, 5);
}

float Geometry(float3 N, float3 V, float3 L, float roughness)
{
	float k = (roughness + 1) * (roughness + 1) / 8;
	k = roughness * roughness;
	float ggx1 = pdot(N, L) / (pdot(N, L) * (1 - k) + k);
	float ggx2 = pdot(N, V) / (pdot(N, V) * (1 - k) + k);
	return ggx1 * ggx2;
}


float3 UnpackNormal(float2 xy)
{
	xy = xy * 2 - 1;
	float z = sqrt(saturate(1 - dot(xy, xy))) * FlipZ;
	return normalize(float3(xy, z));
}

float3 BlendRNM(float3 n1, float3 n2)
{
	float3 t = n1 + float3(0, 0, 1);
	float3 u = n2 * float3(-1, -1, 1);
	return normalize(t * dot(t, u) / t.z - u);
}
float3 GetNormalFromTexture(float2 uv, bool isDetails = false)
{
	Normal.GetDimensions(w, h);
	if (w == 0)
		return float3(0, 0, 1);
	

	float2 xy = isDetails ? SkinDetails.Sample(Wrapped, uv * 100 / ddx(uv)).xy : Normal.Sample(Wrapped, uv).xy;
	xy = xy * 2 - 1;

		if (SwapXY)
			xy.xy = xy.yx;
	
		float z = sqrt(saturate(1 - dot(xy, xy))) * FlipZ;
		return normalize(float3(xy, z));
	}


void GetTB1(float3 pos, float2 uv, float3 N, out float3 T, out float3 B)
{
	float3x3 M = float3x3(ddx(pos), ddy(pos), cross(ddx(pos), ddy(pos)));
	float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
	B = normalize(mul(float2(ddx(uv).x, ddy(uv).x), inverseM)) * FlipX;
	T = normalize(mul(float2(ddx(uv).y, ddy(uv).y), inverseM)) * FlipY;
}

void GetTB2(float3 pos, float2 uv, float3 N, out float3 T, out float3 B)
{
	B = cross(ddy(pos), N) * ddx(uv).x + cross(N, ddx(pos)) * ddy(uv).x;
	T = cross(ddy(pos), N) * ddx(uv).y + cross(N, ddx(pos)) * ddy(uv).y;
	float invmax = rsqrt(max(dot(T, T), dot(B, B)));
	T = (T * invmax) * FlipX;
	B = (B * invmax) * FlipY;
}

float3 GetNormal(Out i)
{
	if (formatID == 3)
		return normalize(i.normal);

	float3 T, B, N = normalize(i.normal),
	textureNormal = GetNormalFromTexture(i.uv, false);
	
	if (UseDetails != 0)
	{
		float detailScale = 20 / length(ddx(i.uv)) * length(ddx(i.pos));
		float3 detail = UnpackNormal(SkinDetails.Sample(Wrapped, i.uv * detailScale).rg);
		textureNormal = BlendRNM(textureNormal, detail);
	}	
	
		GetTB1(i.pos.xyz, i.uv, N, T, B);
		float3 Tf = normalize(i.tangent) * FlipY;
		float3 Bf = normalize(cross(N, Tf)) * FlipX * i.tangent.w;
	
		float3x3 TBN = RecalcTan ? float3x3(B, T, N) : float3x3(Bf, Tf, N);
		return normalize(mul(textureNormal, TBN));
	}


float3 GetNormal2(Out i)
{
	if (formatID == 3)
		return normalize(i.normal);

	float3 T, B, N = normalize(i.normal),
	textureNormal = GetNormalFromTexture(i.uv, false);
	
	if (UseDetails != 0)
	{
		float detailScale = 20 / length(ddx(i.uv)) * length(ddx(i.pos));
		float3 detail = UnpackNormal(SkinDetails.Sample(Wrapped, i.uv * detailScale).rg);
		textureNormal = BlendRNM(textureNormal, detail);
	}
	GetTB2(i.pos.xyz, i.uv, N, T, B);
	float3 Tf = normalize(i.tangent) * FlipY;
	float3 Bf = normalize(cross(N, Tf)) * i.tangent.w * FlipX;
	
	float3x3 TBN = RecalcTan ? float3x3(B, T, N) : float3x3(Bf, Tf, N);
	return normalize(mul(textureNormal, TBN));
}

