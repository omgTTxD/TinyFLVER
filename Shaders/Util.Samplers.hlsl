#include "Util.Common.hlsl"

TextureCube CubeMap : register(t0);
Texture2D Albedo : register(t1); 
Texture2D Normal : register(t2);
Texture2D Metalness : register(t3);
Texture2D Subsurface : register(t4);
Texture2D SpecularDSR : register(t5);

SamplerState Sampler : register(s0); 
static uint w, h;


float4 GetAlbedo(float2 uv)
{
    if (formatID == 3)
    //    return pow(float4(0.562, 0.252, 0.224, 1), 1.5);
         return float4(0.8, 0.8, 0.8, 1);
    
    Albedo.GetDimensions(w, h);
    if (w == 0) 
        return float4(1, 1, 1, 1);
    
    return Albedo.Sample(Sampler, uv);
}


float3 GetNormal(Out i)
{
    if (formatID == 3)
        return normalize(i.normal);
    
    float3 textureNormal;
    int mip = 0;
    
    Normal.GetDimensions(w, h);
   
    if (w >= 4096)
        mip = 1;
    else if (w >= 2048)
        mip = 0;
    
    if (w == 0)
        textureNormal = float3(0, 0, 1);
    else
    {
        float2 xy = Normal.SampleLevel(Sampler, i.uv, mip).xy;
        xy = xy * 2 - 1;
        
        if (FlipX) xy.x *= -1;
        if (FlipY) xy.y *= -1;
        if (SwapXY) xy.xy = xy.yx;
     
        float z = sqrt(saturate(1 - dot(xy, xy)));
        textureNormal = normalize(float3(xy, z));
    }
    
	float3 dp1 = ddx_fine(i.pos.xyz);
    float3 dp2 = ddy_fine(i.pos.xyz);
    float2 duv1 = ddx_fine(i.uv);
    float2 duv2 = ddy_fine(i.uv);
    float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
    float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
    float3 T = normalize(mul(float2(duv1.x, duv2.x), inverseM));
    float3 B = normalize(mul(float2(duv1.y, duv2.y), inverseM));
	
    return normalize(mul(textureNormal, float3x3(T, B, normalize(i.normal))));
}

float3 GetCubemap(float3 direction, float roughness)
{
    CubeMap.GetDimensions(w, h);
    if (w == 0) 
        return 0.25;
    
    if (w == 1024)
        return pow(CubeMap.SampleLevel(Sampler, direction, pow(roughness, 0.5) * 11).rgb, 0.7); 
    else
        return CubeMap.SampleLevel(Sampler, direction, roughness * 5).rgb;
}

float3 GetIrradiance(float3 direction)
{
    CubeMap.GetDimensions(w, h);
    if (w == 0) 
        return 0.1;
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
        return SpecularDSR.Sample(Sampler, uv).y;
    }    
    
    Metalness.GetDimensions(w, h);
    if (w == 0) 
        return 0;
    return Metalness.Sample(Sampler, uv).r;
}

float GetRoughness(float2 uv)
{
    if (formatID == 3)
        return uv.x;
    
    if (formatID == 1)
    {
        SpecularDSR.GetDimensions(w, h);
        if (w == 0) 
            return 0;
        return SpecularDSR.Sample(Sampler, uv).x;
    }
    
    Normal.GetDimensions(w, h);
    if (w == 0) 
        return 0.5;
    return 1 - Normal.Sample(Sampler, uv).z;
}

float4 GetSubsurface(float2 uv) 
{ 
    Subsurface.GetDimensions(w, h); 
    if (w == 0)
        return float4(0, 0, 0, 1);
    
    return Subsurface.Sample(Sampler, uv);
}

float GetF0(float2 uv)
{
    //  DiffuseF0, related to Index of Refraction (0 to 0.08 packed into the 0 to 1 range)
    if (formatID == 1)
        return SpecularDSR.Sample(Sampler, uv).z * 0.08;
    
    return 0.04f;
}

