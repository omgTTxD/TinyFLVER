#include "VS.hlsl"

TextureCube CubeMap : register(t0);
Texture2D Albedo : register(t1); 
Texture2D Normal : register(t2);
Texture2D Metalness : register(t3);
Texture2D Subsurface : register(t4);
Texture2D Reflectance : register(t5);
Texture2D SpecularDSR : register(t6);
Texture2D SkinDetails : register(t7);

SamplerState Sampler : register(s0);
SamplerState Wrapped : register(s1);
static uint w, h;

float4 GetAlbedo(float2 uv)
{
	if (formatID == 3)
		return float4(0.3, 0.3, 0.3, 1);
	
	Albedo.GetDimensions(w, h);
	if (w == 0) 
		return float4(1, 1, 1, 1);
	
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
	return max(0.04, roughness);
}

float3 GetF0(float2 uv)
{
	//  DiffuseF0, related to Index of Refraction (0 to 0.08 packed into the 0 to 1 range)
	if (formatID == 1)
		return lerp(0, 0.08, SpecularDSR.Sample(Wrapped, uv).z);
	
	return 0.04f;
}

float3 GetDiffuseColor(Out i)
{
	if (formatID == 2)
		return GetAlbedo(i.uv).rgb;
	
	float3 baseColor = GetAlbedo(i.uv).rgb;
	float metalness = GetMetalness(i.uv);
	return baseColor * (1 - metalness);
}

float3 GetSpecularColor(float2 uv)
{
	if (formatID == 2)
	{
		Reflectance.GetDimensions(w, h);
		if (w == 0) 
			return 0.25;
		
		return Reflectance.Sample(Wrapped, uv);
	}
	
	float3 F0 = GetF0(uv);
	float3 baseColor = GetAlbedo(uv).rgb;
	float metalness = GetMetalness(uv);
	return lerp(F0, baseColor, metalness);
}

float3 GetNonPointLight(float3 V, float3 N)
{
	float3 L = normalize(-light);
	float3 R = reflect(V, N);
	float3 centerToRay = dot(L, R) * R - L;
	float3 closestPoint = L + centerToRay * saturate(0.05 * rsqrt(dot(centerToRay, centerToRay)));
	return normalize(closestPoint);
}

float3 GetCubemap(float3 V, float3 N, float roughness)
{
	CubeMap.GetDimensions(w, h);
	if (w == 0) 
		return 0.25;
	
	return CubeMap.SampleLevel(Sampler, reflect(-V, N), pow(roughness, 4) * 7).rgb / 4;
}

// I implemented BRDF LUT and multiscaterring, and didn't see much changes (since irradiance component is low enough, and even amient color approximation
// works fine, so last mip level of cubemap is more than enough for me.
float3 GetIrradiance(float3 direction)
{
	CubeMap.GetDimensions(w, h);
	if (w == 0) 
		return 0.2;
	
	return CubeMap.SampleLevel(Sampler, direction, 3).rgb / 4;
}

float3 GetSubsurface(float2 uv) 
{ 
	Subsurface.GetDimensions(w, h); 
	return w == 0 ? 0 : Subsurface.Sample(Wrapped, uv);
}