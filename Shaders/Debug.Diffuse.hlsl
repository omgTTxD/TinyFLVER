#include "Common.hlsl"

float4 PS(VS_OUT i) : SV_Target
{
    float4 albedo = GetAlbedo(i.uv);
    float3 L = normalize(-light);
	float3 N = CalculateNormal(i.posVS.xyz, normalize(i.normal), i.uv);
	float3 result = albedo.rgb * clamp(dot(N, L), 0, 1);
	return float4(pow(result * exposure, 1 / gamma), albedo.a);
}