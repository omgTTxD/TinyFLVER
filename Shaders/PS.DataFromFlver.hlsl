Texture2D AlbedoMap : register(t0);
Texture2D NormalMap : register(t1);
Texture2D SSS : register(t2);
Texture2D Metalness : register(t3);
Texture2D PackedSpecular : register(t4);
SamplerState Sampler : register(s0);

cbuffer PSData : register(b1)
{
	float3 Light;
	float gamma;
	float exposure;
	float time;
	int showId;
	int newFormat;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

float3 RecalculateZ(float2 n)
{
	float2 xy = n * 2 - 1;
	float z = sqrt(saturate(1 - dot(xy, xy)));
	return normalize(float3(xy.x, xy.y, z));
}

float4 PS(VS_OUT i) : SV_Target
{
	float2 NormalXY = NormalMap.Sample(Sampler, i.uv).xy;
	float3 normalTangentSpace = RecalculateZ(NormalXY);
	
	float3 LightDir = normalize(-Light);
	float3 diffuseColor = AlbedoMap.Sample(Sampler, i.uv).xyz;
	
	float3 T = normalize(i.tangent.xyz);
	float3 N = normalize(i.normal);
	float3 B = cross(N, T) * i.tangent.w;

	float3 normalViewSpace = normalize(normalTangentSpace.x * T + normalTangentSpace.y * B + normalTangentSpace.z * N);
	
	// Specular
	float glossiness = NormalMap.Sample(Sampler, i.uv).z;
	float3 EyeDir = normalize(-i.posVS.xyz);
	float3 R = reflect(-LightDir, normalViewSpace);
	float EdotR = saturate(dot(EyeDir, R));
	float3 specular = pow(EdotR, 10) * glossiness / 4;
	
	// Diffuse
	float NdotL = clamp(dot(LightDir, i.normal), 0.03, 1);
	float3 diffuse = NdotL * diffuseColor;

	float3 result = diffuse;
	
	float alpha = AlbedoMap.Sample(Sampler, i.uv).a;
	
	result = result * exposure;
	result = pow(result, 1 / gamma);

	return float4(result, alpha);
}

