#include "VS.hlsl"

TextureCube CubeMap : register(t0);
Texture2D Albedo : register(t1);
Texture2D Normal : register(t2);
Texture2D Metalness : register(t3);
Texture2D Subsurface : register(t4);
Texture2D Reflectance : register(t5);
Texture2D SpecularDSR : register(t6);
Texture2D SkinDetails : register(t7);
SamplerState Wrapped : register(s0);
static uint w, h;

float4 GetAlbedo(float2 uv)
{
	if (formatID == 3)
		return float4(0.5, 0.5, 0.5, 1);
	
	Albedo.GetDimensions(w, h);
	if (w == 0) 
		return float4(0.8, 0.8, 0.8, 1);
	
	return Albedo.Sample(Wrapped, uv);
}

float GetMetalness(float2 uv)
{
	if (formatID == 3)
		return uv.y;
	
	if (formatID == 1)
	{
		SpecularDSR.GetDimensions(w, h);
		return w == 0 ? 0 : SpecularDSR.Sample(Wrapped, uv).y;
	}
	
	Metalness.GetDimensions(w, h);
	return w == 0 ? 0 : Metalness.Sample(Wrapped, uv).x;
}


float GetRoughness(float2 uv)
{
	float roughness;
	
	if (formatID == 3)
		roughness = uv.x;
	else if (formatID == 1)
	{
		SpecularDSR.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : SpecularDSR.Sample(Wrapped, uv).x;
	}
	else
	{
		Normal.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : 1 - Normal.Sample(Wrapped, uv).z;
	}
	return max(0.02, roughness);
}

float3 GetF0(float2 uv)
{
	if (formatID == 1)
		return lerp(0, 0.08, SpecularDSR.Sample(Wrapped, uv).z);
	
	return 0.02f;
}

float3 GetDiffuseColor(Out i)
{
	if (formatID == 2)
		return GetAlbedo(i.uv).rgb;
	return GetAlbedo(i.uv).rgb * (1 - GetMetalness(i.uv));
}

float3 GetSpecularColor(float2 uv)
{
	if (formatID == 2)
	{
		Reflectance.GetDimensions(w, h);
		return w == 0 ? 0.25 : Reflectance.Sample(Wrapped, uv);
	}
	return lerp(GetF0(uv), GetAlbedo(uv).rgb, GetMetalness(uv));
}

float3 GetNonPointLight(float3 V, float3 N)
{
	float3 L = normalize(V - light * 10);
	float3 R = reflect(V, N);
	return normalize(L + 0.05 * normalize(dot(L, R) * R - L));
}

float3 GetCubemap(float3 V, float3 N, float roughness)
{
	CubeMap.GetDimensions(w, h);
	return w == 0 ? 0.1 : CubeMap.SampleLevel(Wrapped, reflect(-V, N), pow(roughness, 2) * 7).rgb / 4;
}

// I implemented BRDF LUT and multiscaterring, and didn't see much changes (since irradiance component is low enough, and even amient color approximation
// works fine, so last mip level of cubemap is more than enough for me.
float3 GetIrradiance(float3 direction)
{
	CubeMap.GetDimensions(w, h);
	return w == 0 ? 0.1 : CubeMap.SampleLevel(Wrapped, direction, 5).rgb / 4;
}

float3 GetSubsurface(float2 uv, float3 L, float3 N, float3 V)
{
	if (!IsSkin) return 0;
	float3 sssColor = float3(0.5, 0.2, 0.2);
	float diffuseWrap = saturate((pdot(N, L) + 0.5) / (1.0 + 0.5));
	float VdotH = saturate(dot(V, -normalize(L + N * 1.1)));
	float transmittance = VdotH;
	float shadowMask = 1 - diffuseWrap;
	return sssColor * (transmittance * 0.03 + shadowMask * 0.08);
}