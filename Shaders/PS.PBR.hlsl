#include "Normals.hlsl"

// Fresnel which becomes 0 when roughness becomes 1. And VdotN instead of VdotH since we want rim lights (which VdotH doesn't give).
float3 Fresnel(float3 F0, float dot, float r)				
{ 
	return lerp(F0, max(1 - r, F0) / 2, pow(1 - dot, 5));
}

float Distribution(float NdotH, float r)
{
	float a = pow(r, 4);
	float denom = NdotH * NdotH * (a - 1) + 1;
	return a / (pi * pow(denom, 2));
}

float Geometry(float3 N, float3 V, float3 L, float r)
{
	//float k = (r + 1) * (r + 1) / 8;
	float k = pow(r, 2) / 2;
	float ggx1 = pdot(N, V) /  (pdot(N, V) * (1 - k) + k);
	float ggx2 = pdot(N, L) / (pdot(N, L) * (1 - k) + k);
	return ggx1 * ggx2;
}

float4 PS(Out i, bool facing : SV_IsFrontFace) : SV_Target
{
	float3 N = GetNormal(i); if (!facing) N = -N;	
	float3 V = normalize(-i.pos.xyz);
	float3 L = GetNonPointLight(V, N);
	float3 H = normalize(L + V);
	float r = GetRoughness(i.uv);
	
	
	float3 diffuseColor = GetDiffuseColor(i);
	float3 specularColor = GetSpecularColor(i.uv) ;
	
	if (exposure == 1)
	{
		diffuseColor /= 2;
		specularColor /= 2;
	}
	
	float G = Geometry(N, V, L, r);
	float D = Distribution(dot(N, H), r);	
	float3 F0 = GetF0(i.uv);
	float3 F = Fresnel(specularColor, pdot(V, H), r);
	float3 F2 = Fresnel(specularColor, pdot(V, N), r);

	float3 diffuseDirect = diffuseColor * (1 - F) * pdot(N, L);
	float3 specularDirect = F * D * G / 4;
	
	float3 diffuseIBL = diffuseColor  * GetCubemapDiffuse(N) / pi;
	float3 specularIBL = F2 * GetCubemapSpecular(V, N, r) / 4;
	
	float3 result = diffuseDirect + specularDirect + diffuseIBL + specularIBL + GetSubsurface(i.uv, L, N, V);
	
	switch (debugID) {
		case 1:  return sRGB(diffuseIBL);
		case 2:  return sRGB(specularColor);
		case 3:  return sRGB(G);
		case 4:  return sRGB( G * D / max(1e-12, pdot(N, V)));
		case 5:  return sRGB(F);
		case 6:  return sRGB(F);
		case 7:  return sRGB(GetCubemapSpecular(V, N, r)/ pi) ;
		case 8:  return sRGB(specularIBL);
		case 9:  return sRGB(diffuseColor);
		case 10: return sRGB(GetWrappedNdotL(N, L));
		case 11: return sRGB(GetCubemapDiffuse(N));
		case 12: return sRGB(diffuseDirect + diffuseIBL);
		case 13: return sRGB(GetMetalness(i.uv));
		case 14: return sRGB(GetRoughness(i.uv));
		case 15: return sRGB(GetF0(i.uv));
		case 16: return sRGB(GetSubsurface(i.uv, L, N, V));		
	}
	
	return sRGB(result, GetAlpha(i.uv));
}