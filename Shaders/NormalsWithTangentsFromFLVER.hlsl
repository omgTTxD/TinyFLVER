Texture2D DiffuseTex : register(t0);
Texture2D NormalMap : register(t1);
SamplerState Sampler : register(s0);

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
    float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

float3 CalculateNormalZ(float2 n)
{
	float2 xy = n * 2 - 1;
	float z = sqrt(saturate(1 - dot(xy, xy)));
	return normalize(float3(xy.x, xy.y, z));
}

float4 PS(VS_OUT i) : SV_Target
{
	float2 NormalXY = NormalMap.Sample(Sampler, i.uv).xy;
	float3 normalTangentSpace = CalculateNormalZ(NormalXY);

	float3 diffuseColor = DiffuseTex.Sample(Sampler, i.uv).xyz;
	float3 LightDir = normalize(float3(1, 1, -3));
	
	float3 T = normalize(i.tangent.xyz);
	float3 N = normalize(i.normal);
	float3 B = cross(N, T);

	float3 normalViewSpace = normalize(normalTangentSpace.x * T + normalTangentSpace.y * B + normalTangentSpace.z * N);

	// Specular
	float glossiness = NormalMap.Sample(Sampler, i.uv).z;
	float3 EyeDir = normalize(-i.posVS.xyz);
	float3 R = reflect(LightDir, normalViewSpace);
	float EdotR = saturate(dot(EyeDir, R));
	float3 specular = pow(EdotR, 10) * glossiness / 4;
	
	// Diffuse
	float NdotL = clamp(dot(LightDir, normalViewSpace), 0.03, 1);
	float3 diffuse = NdotL * diffuseColor;

	float3 result = diffuse + specular;
	
	// Simple and dirty transparency 
	float alpha = DiffuseTex.Sample(Sampler, i.uv).w;
	if (alpha < 0.1)
		discard;
		
	return float4(result, alpha);
}

