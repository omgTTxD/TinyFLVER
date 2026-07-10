#include "Util.Samplers.hlsl"

float4 PS(Out i) : SV_Target
{
    float3 N = GetNormal(i);
    float4 albedo = GetAlbedo(i.uv);
    float3 L = normalize(-light);
    
    // Diffuse with added ambient
    float NdotL = clamp(dot(L, N), 0.1, 1);
    float3 diffuse = NdotL * albedo.rgb;
	
	// Specular
    float shininess = 1 - GetRoughness(i.uv);
    float3 V = normalize(-i.pos.xyz);
    float3 R = reflect(-L, N);                           // Interesting backlight effect if using L instead of -L
    float EdotR = saturate(dot(V, R));
    float3 specular = pow(EdotR, 6) * shininess / 10;
	
    float3 result = diffuse + specular;
   	return float4(pow(result * exposure, 1 / gamma), albedo.a);
}