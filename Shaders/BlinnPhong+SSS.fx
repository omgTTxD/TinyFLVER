Texture2D DiffuseTex : register(t0);
Texture2D NormalMap : register(t1);
Texture2D SssMap : register(t2);
SamplerState Sampler : register(s0);

cbuffer Settings : register(b0)
{
    float3 L;
    float gamma;
	float exposure;
}

struct VS_OUT
{
    float4 pos : SV_POSITION;
    float4 posVS : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD;
};

float3 RecalculateZ(float2 n)
{
    float2 xy = n * 2 - 1;
    float z = sqrt(saturate(1 - dot(xy, xy)));
    return normalize(float3(xy.x, xy.y, z));
}

// Recalculate tangents and bitangents from UV and N
float3x3 CalculateTBN(float3 pos, float3 N, float2 uv)
{
    float3 dp1 = ddx(pos);
    float3 dp2 = ddy(pos);
    float2 duv1 = ddx(uv);
    float2 duv2 = ddy(uv);

    float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
    float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
    float3 T = normalize(mul(float2(duv1.x, duv2.x), inverseM));
    float3 B = normalize(mul(float2(duv1.y, duv2.y), inverseM));
    return float3x3(T, B, N);
}

float3 GetViewSpaceNormal(float3 normalTangentSpace, float3 position, float3 normal, float2 uv)
{
    const float3x3 TBN = CalculateTBN(position, normal, uv);
    return normalize(mul(normalTangentSpace, TBN));
}

float4 PS(VS_OUT i) : SV_Target
{
    // Texture sampling
    float2 NormalXY = NormalMap.Sample(Sampler, i.uv).xy;
    float glossiness = NormalMap.Sample(Sampler, i.uv).z;
    float3 diffuseColor = DiffuseTex.Sample(Sampler, i.uv).xyz;  
    float sss = SssMap.Sample(Sampler, i.uv).x / 2;

    // Real Normal
    float3 NormalTangentSpace = RecalculateZ(NormalXY);
    float3 N = GetViewSpaceNormal(NormalTangentSpace, i.posVS.xyz, normalize(i.normal), i.uv);

	// Diffuse
    float NdotL = clamp(dot(L, N), 0.3, 1);
    float3 diffuse = NdotL * diffuseColor;
	
	// Specular
    float3 V = normalize(-i.posVS.xyz);
    float3 R = reflect(L, N);
    float EdotR = saturate(dot(V, R));
    if (dot(N, V) < 0) EdotR = 0;
    float3 specular = pow(EdotR, 6) * glossiness / 10;

    // Subsurface (very basic and incorrect)
    float3 sssColor = float3(0.6, 0.2, 0.2);
    float wrap = 0.5;
    float diffuseWrap = saturate((NdotL + wrap) / (1.0 + wrap));

    float thickness = 0.01;
    float3 H = normalize(L + N * 1.1);
    float VdotH = saturate(dot(V, -H));
    float transmittance = pow(VdotH, 2) * thickness;
    float shadowMask = 1 - diffuseWrap;
    float3 subsurface = transmittance * 0.5 + shadowMask * 0.5;
    
    float3 result = diffuse + specular + diffuseColor * sssColor * sss * subsurface;
  
    float alpha = DiffuseTex.Sample(Sampler, i.uv).w;
    if (alpha < 0.1) discard;

    result = result * exposure;
	result = pow(result, 1 / gamma);
    
    return float4(result, alpha);
}