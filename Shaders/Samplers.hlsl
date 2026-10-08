#include "VS.hlsl"

Texture2D Albedo : register(t0);
Texture2D Normal : register(t1);
Texture2D Metalness : register(t2);
Texture2D Reflectance : register(t3);
Texture2D Specular : register(t4);
TextureCube CubeMap : register(t5);
Texture2D SkinDetails : register(t6);
Texture2D BRDF_LUT : register(t7);
SamplerState s : register(s0);
static int w, h;

static int Spheres = -1, ER = 0, DS1 = 1, DSR = 2, DS3 = 3;


float3 GetNonPointLight(float3 V, float3 N)
{
	const float radius = 0.08;

	float3 L = normalize(-light * 10 + V);
	float3 R = reflect(-V, N);
	float3 perp = R - dot(R, L) * L;

	float s = length(perp);
	if (s < 1e-5)
		return L;

	float a = min(radius, asin(min(s, 1.0)));
	return L * cos(a) + (perp / s) * sin(a);
}


float GetAlpha(float2 uv)
{
	if (formatID == Spheres)
		return 1;
	
	Albedo.GetDimensions(w, h);
	return w == 0 ? 1 : Albedo.Sample(s, uv).a;
}

float3 GetBaseColor(float2 uv)
{
	if (formatID == Spheres)
		return 0.5;
	
	Albedo.GetDimensions(w, h);
	if (w == 0) return 0.6;
	
	// DS1 and DS3 textures are saved as non-sRGB
	if (formatID == DS1 || formatID == DSR)
		return pow(Albedo.Sample(s, uv), 2.2);
	
	return Albedo.Sample(s, uv);
}

float GetMetalness(float2 uv)
{
	if (formatID == Spheres)
		return uv.y;
	
	if (formatID == DS1)
		return IsDSMetal ? 1 : 0;	
	
	if (formatID == DSR)
		return IsDSMetal ? Specular.Sample(s, uv).y : 0;
	
	if (formatID == DS3)
	{
		Reflectance.GetDimensions(w, h);
		float3 color = w == 0 ? 1 : Reflectance.Sample(s, uv);
		return max(color.r, max(color.g, color.b));
	}
	
	return Metalness.Sample(s, uv).x;
}
// F0 for dielectrics is 0.04. F0 for metals is 1. 
float GetF0(float2 uv)
{
	float DielectricF0; 
	if (formatID == DSR)
		DielectricF0 = Specular.Sample(s, uv).z * 0.08;
	
	DielectricF0 = 0.04;
	return lerp(DielectricF0, 1, GetMetalness(uv));
	
	return lerp(0.02, 1, GetMetalness(uv)); 
}
float3 GetDiffuseColor(Out i)
{
	if (formatID == DS3)
		return GetBaseColor(i.uv);
	
	return GetBaseColor(i.uv) * (1 - GetMetalness(i.uv));
}

// Since I wanted main shader to be similair for both metallic and specular workflows, I added metalness inside (which is better to be outside
// Therefore it's not exactly soecularColar, but more like color/intensity...
float3 GetSpecularColor(float2 uv)
{	
	if (formatID == DS1)
	{
		Specular.GetDimensions(w, h);
		return w == 0 ? 1 : pow(Specular.Sample(s, uv), 2.2);
	}
	
	if (formatID == DS3)
	{
		Reflectance.GetDimensions(w, h);
		return w == 0 ? 1 : Reflectance.Sample(s, uv);
	}
	
	// 1 can become "lightColor" in the future...
	return lerp(1, GetBaseColor(uv), GetMetalness(uv)) * GetF0(uv);
}

float GetRoughness(float2 uv)
{
	float roughness;
	
	if (formatID == Spheres)
		roughness = uv.x;
	else if (formatID == DS1)
		roughness = IsDSMetal ? 0.5 : 0.8;
	else if (formatID == DSR)
	{
		Specular.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : pow(Specular.Sample(s, uv).x, 1.1);
	}
	else
	{
		Normal.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : 1 - Normal.Sample(s, uv).z;
	}
	
	return max(0.02, roughness);	// without clamping roughness == 0 makes speculars black
}

float3 GetCubemapSpecular(float3 V, float3 N, float roughness)
{
	// I have sRGB and non-sRGB cubemap (and I hasn't been able to edit them), so apply gamma correction depending on size
	CubeMap.GetDimensions(w, h);
	float3 color = w == 0 ? 0.25 : CubeMap.SampleLevel(s, reflect(-V, N), pow(roughness, 0.7) * log2(w)).rgb / 4;
	return w >= 256 ? pow(color, 1.25) * 3 : pow(color, 1.2) ;
}

float3 GetCubemapDiffuse(float3 direction)
{
	CubeMap.GetDimensions(w, h);
	return w == 0 ? 0.5 : pow(CubeMap.SampleLevel(s, direction, log2(h)).rgb, 1);
}

// SSS looks MUCH better when wrapped NdotL is used instead of NdotL in diffuse light
float3 GetWrappedNdotL(float3 N, float3 L)
{
	return IsSkin ? saturate((pdot(N, L) + 0.1) / (1 + 0.1)) : pdot(N, L);
}

float3 GetSubsurface(float2 uv, float3 L, float3 N, float3 V)
{
	if (!IsSkin) return 0;
	float3 sssColor = float3(0.4, 0.1, 0.1);
	float diffuseWrap = saturate((pdot(N, L) + 0.25) / (1.0 + 0.25));
	float VdotH = pdot(V, normalize(L + N));
	float transmittance = VdotH;
	float shadowMask = 1 - diffuseWrap;
	return sssColor * (transmittance * 0.02 + shadowMask * 0.1);
}
