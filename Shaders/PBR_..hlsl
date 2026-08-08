#include "Utility.hlsl"

float4 PS(Out i, bool facing : SV_IsFrontFace) : SV_Target
{
	float alpha = GetAlbedo(i.uv).a;
	float roughness = GetRoughness(i.uv);
	
	float3 N = GetNormal(i);
	if (!facing) N = -N;
	
	float3 V = normalize(-i.pos.xyz);
	float3 L = GetNonPointLight(V, N);
	float3 H = normalize(L + V);
	
	float3 specularColor = GetSpecularColor(i.uv);
	float3 diffuseColor = GetDiffuseColor(i);
	
	float G = Geometry(N, V, L, roughness);
	float D = Distribution(pdot(N, H), roughness);
	float3 F = Fresnel(specularColor, pdot(V, H));
	float3 F_IBL = FresnelIBL(specularColor, pdot(V, N), roughness);
	
	float3 diffuse = (1 - F) * diffuseColor * pdot(N, L) / pi;
	float3 diffuseIBL = (1 - F_IBL) * diffuseColor * GetIrradiance(N) / pi;
	
	float3 specular = F * D * G / 4 / max(1e-6, pdot(N, V));
	float3 specularIBL = F_IBL * GetCubemap(V, N, roughness );
	
	float3 result = diffuse + diffuseIBL + specular + specularIBL + GetSubsurface(i.uv, L, N, V);
	
	switch (debugID)
	{
		case 0:  return sRGB(result , alpha);
		case 1:  return Linear(roughness, alpha);
		case 2:  return Linear(GetMetalness(i.uv), alpha);
		case 3:  return sRGB(F, alpha);
		case 4:  return sRGB(F_IBL.x, alpha);
		case 5:  return sRGB(D, alpha);
		case 6:  return Linear(G, alpha);
		case 7:  return sRGB(GetCubemap(V, N, roughness), alpha);
		case 8:  return sRGB(GetIrradiance(N), alpha);
		case 9:  return sRGB(specularColor, alpha);
		case 10: return sRGB(specular, alpha);
		case 11: return sRGB(specularIBL, alpha);
		case 12: return sRGB(specular + specularIBL, alpha);
		case 13: return sRGB(diffuseColor, alpha);
		case 14: return sRGB(diffuse, alpha);
		case 15: return sRGB(diffuseIBL, alpha);
		case 16: return sRGB(diffuse + diffuseIBL, alpha);
		case 17: return sRGB(GetSubsurface(i.uv, L, N, V), alpha);
		case 18: return sRGB(GetF0(i.uv), alpha);			
	}

	return sRGB(result, alpha);
}