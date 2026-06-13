Texture2D AlbedoMap : register(t0);
Texture2D NormalMap : register(t1);
Texture2D Metalness : register(t2);
Texture2D SSS : register(t3);
Texture2D PackedSpecular : register(t4);
SamplerState Sampler : register(s0);

cbuffer VSData : register(b0)
{
    row_major float4x4 M;
    row_major float4x4 MVP;
};


cbuffer PSData : register(b1)
{
	float3 light;
	float gamma;
	float exposure;
	float time;
	int dynamic;
	int oldFormat;
};

struct Out
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b)
{
    return saturate(dot(a, b));
}

static uint w, h;

float4 GetAlbedo(float2 uv)
{
	AlbedoMap.GetDimensions(w, h);
    return (w == 0) ? float4(0.8, 0.8, 0.8, 1) : AlbedoMap.Sample(Sampler, uv);
}

float4 GetNormal(float2 uv)
{
    NormalMap.GetDimensions(w, h);
    return (w == 0) ? float4(0.5, 0.5, 0.5, 1) : NormalMap.Sample(Sampler, uv);
}

float4 GetMetalness(float2 uv)
{
    Metalness.GetDimensions(w, h);
    return (w == 0) ? float4(0, 0, 0, 1) : Metalness.Sample(Sampler, uv);
}

float4 GetSSS(float2 uv)
{
    SSS.GetDimensions(w, h);
    return (w == 0) ? float4(1, 1, 1, 1) : SSS.Sample(Sampler, uv);
}



float3 CalculateNormal(float3 pos, float3 N, float2 uv)
{
    float2 xy = GetNormal(uv).xy * 2 - 1;
	float z = sqrt(saturate(1 - dot(xy, xy)));
	float3 textureNormal = normalize(float3(xy, z));
	
	float3 dp1 = ddx_fine(pos);
	float3 dp2 = ddy_fine(pos);
	float2 duv1 = ddx_fine(uv);
	float2 duv2 = ddy_fine(uv);
	
	float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
	float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
	float3 T = normalize(mul(float2(duv1.x, duv2.x), inverseM));
	float3 B = normalize(mul(float2(duv1.y, duv2.y), inverseM));
	
	return normalize(mul(textureNormal, float3x3(T, B, N)));
}

float3 SkyColor(float3 dir, float roughness)
{
	float a = roughness * roughness;
	float k = 1.0 / (0.125 + 2.5 * a);

	float3 zenith = float3(0.20, 0.32, 0.60); // глубока€ синева
	float3 horizon = float3(0.90, 0.80, 0.62); // тЄпла€ дымка, €рче всего
	float3 ground = float3(0.16, 0.12, 0.09); // тЄмное альбедо земли

	float y = dir.y * k;
	float3 c = lerp(horizon, zenith, saturate(y));
	c = lerp(c, ground, saturate(-y * 2.0)); // вниз переход короче
	return c;
}