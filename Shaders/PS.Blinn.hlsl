#include "Normals.hlsl"
#include "VS.hlsl"

float4 PS(Out i) : SV_Target
{
    float4 albedo = GetAlbedo(i.uv);
    float3 L = normalize(-light);
    float3 N = GetNormal(i);
    float3 diffuse = pdot(N, L) * albedo.rgb;
    float3 ambient = 0.2 * albedo.rgb;
	
    float shininess = 1 - GetRoughness(i.uv);
    float3 V = normalize(-i.pos.xyz);
    float3 R = reflect(L, N);                                  
    float3 specular = pow(pdot(V, R), 6) * shininess / 10;
	
    float3 result = diffuse + ambient + specular;
   	return sRGB(result, albedo.a);
}