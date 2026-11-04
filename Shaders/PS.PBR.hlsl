#include "Globals.hlsl"

float4 PS(In i, bool facing : SV_IsFrontFace) : SV_Target
{ 
	InitGlobals(i, facing);
		clip(alpha - 0.1);
	float3 diffuseDirect = diffuseColor * (1 - F)  * pdot(N, L) ;
	float3 diffuseIBL = diffuseColor * (1 - F_IBL) * GetIrradiance(N) / 4 / 4;
	
	F_IBL = (F_IBL * lut.x + lut.y) ;
	float3 specularDirect = F * D * G / 4 /max(1e-9, pdot(N, V));		
	float3 specularIBL = F_IBL * GetRadiance() / 4;
		
	//float3 subsurface = (Lighting(L, 1, 0.3)) / 4 ;
	
	float3 result = 0;
	result += diffuseIBL;
	result += specularIBL;
	result += (diffuseDirect + specularDirect) * Shadow();
	
	result += SSSTransmittance();
	
	switch (debugID) {
		case 1:  return sRGB(GetSSS(uv));
		case 2:  return sRGB(SSSTransmittance());
		case 3:  return sRGB(F);
		case 4:  return sRGB(F_IBL);
		case 5:  return sRGB(diffuseIBL);
	}

	return sRGB(result);
}