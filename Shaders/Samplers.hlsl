#include "VS.hlsl"


TextureCube CubeMap : register(t0);
Texture2D Albedo : register(t1); 
Texture2D Normal : register(t2);
Texture2D Metalness : register(t3);
Texture2D Subsurface : register(t4);
Texture2D Reflectance : register(t5);
Texture2D SpecularDSR : register(t6);
Texture2D LUT : register(t7);

SamplerState Sampler : register(s0);
SamplerState Wrapped : register(s1);
static uint w, h;

float4 GetAlbedo(float2 uv)
{
    if (formatID == 3)
     //   return pow(float4(0.562, 0.252, 0.224, 1), 1.5);
        return float4(0.8, 0.8, 0.8, 1);
    
    Albedo.GetDimensions(w, h);
    if (w == 0) 
        return float4(1, 1, 1, 1);
    
    return Albedo.Sample(Wrapped, uv);
}

float3 GetCubemap(float3 direction, float roughness)
{
    CubeMap.GetDimensions(w, h);
    if (w == 0) 
        return 0.25;
    
    return CubeMap.SampleLevel(Sampler, direction, roughness * 5).rgb;
}

float3 GetIrradiance(float3 direction)
{
    CubeMap.GetDimensions(w, h);
    if (w == 0) 
        return 0.2;
    if (w == 1024)
        return CubeMap.SampleLevel(Sampler, direction, 11).rgb / 2; 
    else
        return CubeMap.SampleLevel(Sampler, direction, 5).rgb / 2;
}

float GetMetalness(float2 uv)
{
    if (formatID == 3)
        return uv.y;
    
    if (formatID == 1)
    {
        SpecularDSR.GetDimensions(w, h);
        if (w == 0) 
            return 0;     
        return SpecularDSR.Sample(Wrapped, uv).y;
    }    
    
    Metalness.GetDimensions(w, h);
    if (w == 0) 
        return 0;
    return Metalness.Sample(Wrapped, uv).r;
}

float GetRoughness(float2 uv)
{
    if (formatID == 3)
        return uv.x;
    
    if (formatID == 1)
    {
        SpecularDSR.GetDimensions(w, h);
        if (w == 0) 
            return 0.5;
        return SpecularDSR.Sample(Wrapped, uv).x;
    }
    
    Normal.GetDimensions(w, h);
    if (w == 0) 
        return 0.5;
    return 1 - Normal.Sample(Wrapped, uv).z;
}

float4 GetSubsurface(float2 uv) 
{ 
    Subsurface.GetDimensions(w, h); 
    if (w == 0)
        return float4(0, 0, 0, 1);
    
    return Subsurface.Sample(Wrapped, uv);
}

float GetF0(float2 uv)
{
    //  DiffuseF0, related to Index of Refraction (0 to 0.08 packed into the 0 to 1 range)
    if (formatID == 1)
        return SpecularDSR.Sample(Wrapped, uv).z * 0.08;
    
    return 0.02f;
}

float3 GetDiffuseColor(Out i)
{	
    if (formatID == 2)
	{
        return GetAlbedo(i.uv).rgb;
	}
    
	float3 baseColor = GetAlbedo(i.uv).rgb;
	float metalness = GetMetalness(i.uv);
    return baseColor * (1 - metalness);
}

float3 GetSpecularColor(Out i)
{  
	if (formatID == 2)
	{
		Reflectance.GetDimensions(w, h);
		if (w == 0) 
			return float3(1, 1, 1);
        
        return Reflectance.Sample(Wrapped, i.uv).rgb;
	}
    
    float3 F0 = GetF0(i.uv);
    float3 baseColor = GetAlbedo(i.uv).rgb;
	float metalness = GetMetalness(i.uv);
   // F0 = lerp(1, F0, metalness);
    return lerp(F0, baseColor, metalness);
}