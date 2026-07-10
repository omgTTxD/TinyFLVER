#include "Util.Samplers.hlsl"

float Distribution(float NdotH, float r)
{
	float roughness = pow(r, 4);			
	float denom = NdotH * NdotH * (roughness - 1) + 1;
	return roughness / (pi * pow(denom, 2));
}

float3 Fresnel(float3 F0, float HdotV)
{
	return F0 + (1 - F0) * pow(1 - HdotV, 5);
}
float3 FresnelIBL(float3 F0, float NdotV, float roughness)
{
	float F = F0 + (1 - F0) * pow(1 - NdotV, 5);
	return lerp(0, F, 1 - roughness);
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
	
	float3 baseColor = GetAlbedo(i.uv);
	float alpha = GetAlbedo(i.uv).a;
	float metalness = GetMetalness(i.uv);
	float roughness = max(0.05, GetRoughness(i.uv));
	
	float3 F0 = GetF0(i.uv);
	float3 diffuseColor = baseColor * (1 - metalness);
	float3 specularColor = lerp(F0, baseColor, metalness);
	
	float G = Geometry(N, V, L, roughness);
	float D = Distribution(pdot(N, H), roughness);
	float3 F = Fresnel(specularColor, pdot(V, H));
	float3 F_IBL = FresnelIBL(specularColor, pdot(V, N), roughness);
    
	float3 diffuse	= (1 - F) * diffuseColor * pdot(N, L);
    float3 diffuseIBL = (1 - F) * diffuseColor *  GetIrradiance(N);
    float3 specular	= F * D * G / 4 / max(1e-9, pdot(N, V)); 
	float3 specularIBL = F * GetCubemap(reflect(-V, N), roughness);		
	
	float3 result = diffuse + diffuseIBL + specular + specularIBL;


	switch (debugID)
	{
		case 0: return sRGB(result, alpha);
		case 1: return sRGB(specular, alpha);
		case 2: return sRGB(specularIBL, alpha);							
		case 3: return sRGB(specular + specularIBL, alpha);					
		case 4: return sRGB(diffuse, alpha);								
		case 5: return sRGB(diffuseIBL, alpha);							
		case 6: return sRGB(diffuse + diffuseIBL, alpha);
		case 7: return Linear(D);							
		case 8: return Linear(G);							
		case 9: return Linear(F);					
		case 10: return Linear(F_IBL);								
	}
	
	return sRGB(result, alpha);
}