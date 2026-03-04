Texture2D AlbedoMap : register(t0);
Texture2D NormalMap : register(t1);
Texture2D SSS : register(t2);
Texture2D Metalness : register(t3);
SamplerState Sampler : register(s0);

cbuffer PSData : register(b0)
{
	float3 L;
	float gamma;
	float exposure;
	int showId;
};

struct VS_OUT
{
	float4 sv_pos : SV_POSITION;
	float4 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b) { return max(0, dot(a, b)); }

float3 CalculateNormal(float3 pos, float3 vertexNormal, float2 uv)
{
	float2 xy = NormalMap.Sample(Sampler, uv).xy * 2 - 1;
	float z = sqrt(saturate(1 - dot(xy, xy)));
	float3 N = float3(xy, z);
	
	float3x3 M = float3x3(ddx(pos), ddy(pos), cross(ddx(pos), ddy(pos)));
	float2x3 inverseM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
	float3 T = normalize(mul(float2(ddx(uv).x, ddy(uv).x), inverseM));
	float3 B = normalize(mul(float2(ddx(uv).y, ddy(uv).y), inverseM));
	
	float3x3 TBN = float3x3(T, B, vertexNormal);
	return normalize(mul(N, TBN));
}

float NormalDistribution(float NdotH, float r)
{
	float alpha = r * r;
	float alpha2 = alpha * alpha;
	float tmp = (NdotH * NdotH * (alpha2 - 1)) + 1;
	return alpha2 / max(1e-6, pi * tmp * tmp);
}

float3 Fresnel(float3 F0, float x)
{
	return F0 + (1 - F0) * pow(1 - x, 5);
}


float GeometrySchlickGGX(float NdotV, float k)
{
    float num   = NdotV;
    float denom = NdotV * (1.0 - k) + k;
	
    return num / denom;
}
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float ggx2  = GeometrySchlickGGX(pdot(N, V), roughness);
    float ggx1  = GeometrySchlickGGX(pdot(N, L), roughness);
    return ggx1 * ggx2;
}
float4 PS (VS_OUT i) : SV_Target
{
	float3 albedo = AlbedoMap.Sample(Sampler, i.uv).rgb;
	float roughness = 1 - NormalMap.Sample(Sampler, i.uv).z;
	roughness = clamp(roughness, 0.2, 0.8);
	float thickness = SSS.Sample(Sampler, i.uv).x;
	float metallness = Metalness.Sample(Sampler, i.uv).x;
	float3 N = CalculateNormal(i.pos.xyz, normalize(i.normal), i.uv);
	float3 V = normalize(-i.pos.xyz);
	float3 H = normalize(L+ V);
	
	float3 F0 = lerp(0.02, albedo, metallness);
	float D = NormalDistribution(pdot(N, H), roughness);
	float3 F = Fresnel(F0, pdot(N, V));
	float G = GeometrySmith(N, V, L, roughness);
	
	float3 kd = (1 - F) * (1 - metallness);
	float3 diffuse = pdot(N, L) * albedo * kd;
	
	float3 specular = D * F * G / max(1e-6, 2 * pdot(N, V));
	float3 ambient = albedo * 0.1;
	
    float3 sssColor = float3(1, 0.2, 0.2);
    float diffuseWrap = saturate(pdot(N, L) + 0.5 / 1.5);
    float3 shadows =  max(0, (1 - diffuseWrap) * sssColor  * thickness * 0.05);
    float3 transmittance = pow(pdot(V, H), 2) * sssColor  * thickness * 0.01;
	float3 subsurface = shadows + transmittance;
	
	float3 result;
	switch (showId) {
		case 0: result = ambient + diffuse + specular + subsurface; break;
		case 1: result = roughness; break;
		case 2: result = diffuse; break;
		case 3: result = D;	break;
		case 4: result = F;	break;	
		case 5: result = G;	break;
		case 6: result = specular; break;
		case 7: result = shadows; break;
		case 8: result = transmittance;	break;
		case 9: result = subsurface; break;	
		case 10: result = ambient; break;
	};
	
	result = result * exposure;
	result = pow(result, 1 / gamma);

	return float4(result , AlbedoMap.Sample(Sampler, i.uv).a);
}