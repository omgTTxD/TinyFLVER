#include "Globals.hlsl"

float4 PS(In i, bool facing : SV_IsFrontFace) : SV_Target
{ 
	InitGlobals(i, facing);

	float alpha2 = pow(GetRoughness(), 4);
	float shininess = 2 / alpha2;

	float3 diffuseDirect  =diffuseColor * pdot(N, L);
	float3 specularDirect = specularColor * pow(pdot(H, N), shininess) / (4 * pi * alpha2);   
	float3 diffuseIndirect = diffuseColor * 0.04;

	float3 result = (diffuseDirect + specularDirect) * Attenuation(2) + diffuseIndirect;
	
	switch (debugID) {
		case 1:  return sRGB(GetIrradiance(N));
		case 2:  return sRGB(diffuseDirect);
		case 3:  return sRGB(F);
		case 4:  return sRGB(diffuseDirect);
	}

	return sRGB(result, GetAlpha(i.uv));
}