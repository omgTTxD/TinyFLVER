#include "Globals.hlsl"

float4 PS(In i, bool facing : SV_IsFrontFace) : SV_Target
{ 
	InitGlobals(i, facing);
	float sh = 2 / pow(r, 4);	
	
	float3 diffuse = diffuseColor * pdot(N, L);
	float3 diffuseIBL = diffuseColor * 0.05;
	
	float3 specular = specularColor * pow(pdot(H, N), sh) / (4 * pow(r, 4));   
	float3 specularIBL = specularColor * GetRadiance() / 4;
	
	float3 result = (diffuse + specular) * Shadow();
	result *= Attenuation(2);
	result += diffuseIBL + specularIBL;
	result += SSSTransmittance();
	result += Lighting(L, 1, 0.3) / 4;
	switch (debugID) {
		case 1:  return sRGB(SSSTransmittance());
		case 2:  return sRGB(specular);
		case 3:  return sRGB(specularIBL);
		case 4:  return sRGB(diffuseIBL);
		case 5:  return sRGB(specular);
		case 6:  return sRGB(diffuse);	
	}

	return sRGB(result, GetAlpha(i.uv));
}
