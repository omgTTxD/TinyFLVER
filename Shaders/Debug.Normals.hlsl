#include "Common.hlsl"

float4 PS(VS_OUT i) : SV_Target
{
	float3 N = CalculateNormal(i.posVS.xyz, normalize(i.normal), i.uv);
	N = (N + 1) * 0.5;
	return float4(pow(N * exposure, 1 / gamma), 1);
}