#include "Normals.hlsl"

float Distribution(float NdotH, float r)
{
	float roughness = pow(r, 4);			
	float denom = NdotH * NdotH * (roughness - 1) + 1;
	return roughness / (pi * pow(denom, 2));
}
float3 Fresnel(float3 F0, float VdotH)
{
	return F0 + (1 - F0) * pow(1 - VdotH, 5);
}
float3 FresnelIBL(float3 F0, float VdotN, float r)
{
	return F0 + max(pow(1 - r, 2), F0) * pow(1 - VdotN, 5);
}

float Geometry(float3 N, float3 V, float3 L, float roughness)
{
	float k = (roughness + 1) * (roughness + 1) / 8;
	k = roughness * roughness;
	float ggx1 = pdot(N, L) / (pdot(N, L) * (1 - k) + k);
	float ggx2 = pdot(N, V) / (pdot(N, V) * (1 - k) + k);
	return ggx1 * ggx2;
}

float4 PS(Out i) : SV_Target
{
	float3 N = GetNormal(i);
	float3 V = normalize(-i.pos.xyz);
	float3 L = normalize(-light);
	float3 H = normalize(L + V);

	float alpha = GetAlbedo(i.uv).a;
	float roughness = max(0.05, GetRoughness(i.uv));
	
	float3 specularTint = GetSpecularColor(i);
	float3 diffuseColor = GetDiffuseColor(i);
	
	float G = Geometry(N, V, L, roughness);
	float D = Distribution(pdot(N, H), roughness);
	float3 F = Fresnel(specularTint, pdot(V, H));
	float3 F_IBL = FresnelIBL(specularTint, pdot(V, N), roughness);
	float3 diffuse	= (1 - F) * diffuseColor * pdot(N, L);
    float3 diffuseIBL = (1 - F_IBL) * diffuseColor * GetIrradiance(N);
    float3 specular	= F * D * G / 4 / max(1e-9, pdot(N, V));
	float3 specularIBL = F_IBL * pdot(N, V) * GetCubemap(reflect(-V, N), roughness);

	float3 result = diffuse + diffuseIBL + specular + specularIBL;

	switch (debugID)
	{
		case 0:  return sRGB(GetAlbedo(i.uv).rgb, alpha);
		case 1:  return Linear(roughness, alpha);
		case 2:  return Linear(GetMetalness(i.uv), alpha);
		case 3:  return sRGB(GetCubemap(reflect(-V, N), roughness), alpha);
		case 4:  return sRGB(GetIrradiance(N), alpha);
		case 5:  return sRGB(diffuseColor, alpha);
		case 6:  return sRGB(specularTint, alpha);
		case 7:  return sRGB(result, alpha);
		case 8:  return sRGB(specular, alpha);
		case 9:  return sRGB(specularIBL, alpha);
		case 10: return sRGB(specular + specularIBL, alpha);
		case 11: return sRGB(diffuse, alpha);
		case 12: return sRGB(diffuseIBL, alpha);
		case 13: return sRGB(diffuse + diffuseIBL, alpha);
		case 14: return sRGB(F, alpha);
		case 15: return sRGB(D, alpha);
		case 16: return sRGB(G, alpha);
	}
	
	return sRGB(result, alpha);
}