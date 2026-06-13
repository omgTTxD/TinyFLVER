#include "Common.hlsl"

float NormalDistribution(float NdotH, float r)
{
	float alpha = pow(r, 4);
	float denom = NdotH * NdotH * (alpha - 1) + 1;
	return alpha / (pi * pow(denom, 2));
}

float3 FresnelSchlick(float3 F0, float k) 
{
	return F0 + (1 - F0) * pow(1 - k, 5);
}

float GeometrySchlickGGX(float NdotV, float k) {
	float num   = NdotV;
	float denom = NdotV * (1.0 - k) + k;
	return num / denom;
}

float GeometrySmith(float3 N, float3 V, float3 L, float roughness) {
	float ggx2  = GeometrySchlickGGX(pdot(N, V), roughness);
	float ggx1  = GeometrySchlickGGX(pdot(N, L), roughness);
	return ggx1 * ggx2;
}

float4 PS(Out i) : SV_Target
{	
	float3 L = normalize(-light);

	float roughness, metalness;
	float3 F0;
    float4 albedo = GetAlbedo(i.uv);
	
	if (oldFormat) {
		roughness = PackedSpecular.Sample(Sampler, i.uv).x;
		metalness = PackedSpecular.Sample(Sampler, i.uv).y;
		F0 = PackedSpecular.Sample(Sampler, i.uv).z * 0.08; //  DiffuseF0, related to Index of Refraction (0 to 0.08 packed into the 0 to 1 range)
	} else {
		roughness = 1 - NormalMap.Sample(Sampler, i.uv).z;
	  roughness = 0.1;
		metalness = Metalness.Sample(Sampler, i.uv).r;
		F0 = lerp(0.00, albedo, metalness);
	}
	
	roughness = clamp(roughness, 0.1, 0.9);
	float thickness = SSS.Sample(Sampler, i.uv).x;
	
	float3 N = CalculateNormal(i.posVS.xyz, normalize(i.normal), i.uv);
	
	float3 V = normalize(-i.posVS.xyz);
	float3 H = normalize(L + V);  
	
	float D = NormalDistribution(pdot(N, H), roughness);
	//float3 F = FresnelSchlick(F0, pdot(V, H));
	float3 F = FresnelSchlick(F0, pdot(N, V));
	float G = GeometrySmith(N, V, L, roughness);
	
	float3 kd = (1 - F) * (1 - metalness);
	float3 diffuse = pdot(N, L) * albedo.rgb * kd;
	
	float3 specular = D * F * G / max(1e-3, 4 * pdot(N, V));
	float3 reflection = SkyColor(reflect(-V, N), roughness);
	float3 metallic = reflection* specular *  metalness;
	float3 ambient = albedo * 0.1;
	
	float3 sssColor = float3(1, 0.2, 0.2);
	float diffuseWrap =saturate((dot(N, L) + 0.5) / 1.5) ;
	float3 shadows =  max(0, (1 - diffuseWrap) * sssColor  * thickness * 0.05);
	float3 transmittance = pow(pdot(V, H), 2) * sssColor  * thickness * 0.01;
	float3 subsurface = shadows + transmittance;
	
	float3 result = ambient + diffuse +  metallic + subsurface;
    return float4(pow(result * exposure, 1 / gamma), albedo.a);
}