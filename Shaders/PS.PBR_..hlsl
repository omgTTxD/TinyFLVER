#include "Common.hlsl"

float4 PS(Out i) : SV_Target
{
	//float3 baseColor = GetAlbedo(i.uv).rgb;
	float alpha = GetAlbedo(i.uv).a;
	float roughness = GetRoughness(i.uv);
	
	float3 N = GetNormal2(i);
	float3 V = normalize(-i.pos.xyz);
	float3 L = GetNonPointLight(V, N);
	float3 H = normalize(L + V);
	float dotNV = max(1e-9, pdot(N, V));
	
	float3 specularColor = GetSpecularColor(i.uv);
	float3 diffuseColor = GetDiffuseColor(i);
	
	float G = Geometry(N, V, L, roughness);
	float D = Distribution(pdot(N, H), roughness);
	float3 F = Fresnel(specularColor, pdot(V, H));
	float3 F_IBL = FresnelIBL(specularColor, pdot(V, N), roughness);
	
	float3 diffuse	= (1 - F) * diffuseColor * pdot(N, L);
	float3 diffuseIBL = (1 - F_IBL) * diffuseColor * GetIrradiance(N);
	
	float3 specular = F * D * G / 4 / dotNV;
	float3 specularIBL = F_IBL * GetCubemap(V, N, roughness);

	float3 result = diffuse + diffuseIBL + specular  + specularIBL;
	
	switch (debugID)
	{
		case 0:  return sRGB(result , alpha);
		case 1:  return sRGB(F, alpha);
		case 2:  return sRGB(F_IBL, alpha);
		case 3:  return sRGB(D, alpha);
		case 4:  return sRGB(G, alpha);
		case 5:  return sRGB(GetCubemap(V, N, roughness), alpha);
		case 6:  return sRGB(GetIrradiance(N), alpha);
		case 7:  return sRGB(specularColor, alpha);
		case 8:  return sRGB(specular, alpha);
		case 9:  return sRGB(specularIBL, alpha);
		case 10: return sRGB(specular + specularIBL, alpha);
		case 11: return sRGB(diffuseColor, alpha);
		case 12: return sRGB(diffuse, alpha);
		case 13: return sRGB(diffuseIBL, alpha);
		case 14: return sRGB(diffuse + diffuseIBL, alpha);
	}
	//return float4(pow(result * exposure, 1 / gamma), alpha);
	return sRGB(result, alpha);
}