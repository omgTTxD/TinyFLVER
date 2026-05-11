Texture2D AlbedoMap : register(t0);
Texture2D NormalMap : register(t1);
Texture2D SSS : register(t2);
Texture2D Metalness : register(t3);
Texture2D PackedSpecular : register(t4);
SamplerState Sampler : register(s0);

cbuffer PSData : register(b1)
{
	float3 Light;
	float gamma;
	float exposure;
	int newFormat;
	float time;
	float distance;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

float3 CalculateNormal(float3 pos, float3 N, float2 uv)
{
    int texSize = 2048;
    float2 xy = NormalMap.Sample(Sampler, uv).xy;
    xy += NormalMap.Sample(Sampler, uv + float2(1, 0) / texSize).xy;
    xy += NormalMap.Sample(Sampler, uv + float2(0, 1) / texSize).xy;
    xy += NormalMap.Sample(Sampler, uv + float2(1, 1) / texSize).xy;
	
    xy = xy / 4;
	xy = xy * 2 - 1;

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

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }

float3 SkyColor(float3 dir, float roughness)
{
	float3 sky	= float3(0.4, 0.5, 0.9);
	float3 ground = float3(0.6, 0.4, 0.3);

	float t = saturate(dir.y + 0.5);

	float3 color = lerp(ground, sky, t);
    return color;
   // return lerp(color, 0.5, 1 - roughness);
}

float NormalDistribution(float NdotH, float r)
{
	float alpha = r; // * r
	float alpha2 = alpha * alpha;
	float tmp = (NdotH * NdotH * (alpha2 - 1)) + 1;
	return alpha2 / max(1e-6, pi * tmp * tmp);
}

float3 Fresnel(float3 F0, float3 F90, float VdotH)
{
	float x = pow(1 - VdotH, 5);
	return F0 + (F90 - F0) * x;
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
	float3 L = normalize(-Light);

	float roughness, metalness; // metarupa
	float3 F0;
	float3 albedo = AlbedoMap.Sample(Sampler, i.uv).rgb;
	
	if (newFormat) {
		roughness = 1 - NormalMap.Sample(Sampler, i.uv).z;
		metalness = Metalness.Sample(Sampler, i.uv).x;
		F0 = lerp(0.03, albedo, metalness);
	} else {
		roughness = PackedSpecular.Sample(Sampler, i.uv).x;
		metalness = PackedSpecular.Sample(Sampler, i.uv).y;
		F0 = PackedSpecular.Sample(Sampler, i.uv).z * 0.08;		//  DiffuseF0, related to Index of Refraction (0 to 0.08 packed into the 0 to 1 range)
	}
	
	roughness = clamp(roughness, 0.05, 0.8);
	float thickness = SSS.Sample(Sampler, i.uv).x;
	
	float3 N = CalculateNormal(i.posVS.xyz, normalize(i.normal), i.uv);
	
	float3 V = normalize(-i.posVS.xyz);
	float3 H = normalize(L + V);  
	
 // float D = NormalDistribution(pdot(N, H), roughness);
	float D = pow(NormalDistribution(pdot(N, H), roughness), 1.3);
	float3 F = Fresnel(F0, 1, pdot(V, H));
	float G = GeometrySmith(N, V, L, roughness);
	
	float3 kd = (1 - F) * (1 - metalness);
	float3 diffuse = pdot(N, L) * albedo * kd;
	
	float3 specular = D * F * G / max(1e-3, 4 * pdot(N, V));
	float3 reflection = SkyColor(reflect(-V, N), roughness);
    float3 metallic =  specular * F * metalness;
	float3 ambient = albedo * 0.1;
	
	float3 sssColor = float3(1, 0.2, 0.2);
	float diffuseWrap =saturate((dot(N, L) + 0.5) / 1.5) ;
	float3 shadows =  max(0, (1 - diffuseWrap) * sssColor  * thickness * 0.05);
	float3 transmittance = pow(pdot(V, H), 2) * sssColor  * thickness * 0.01;
	float3 subsurface = shadows + transmittance;
	
	float3 result = ambient + diffuse + specular + metallic + subsurface;
    
	result *= exposure;
	result = pow(result, 1 / gamma);
	
    return float4(result, AlbedoMap.Sample(Sampler, i.uv).a);
}