struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

float4 PS(VS_OUT i) : SV_Target
{
	float3 Normal = normalize(i.normal);
	float3 LightDir = normalize(float3(1, 1, -1));
	float3 EyeDir = normalize(-i.posVS.xyz);
	
	// Half Vector 
	//float3 H = normalize(LightDir + EyeDir);
	//float HdotN = saturate(dot(H, Normal));
	//float specular = pow(HdotN, 8) / 4;
	
	// Reflect
	float3 R = reflect(-LightDir, Normal);
	float EdotR = saturate(dot(EyeDir, R));
	float3 specular = pow(EdotR, 10) / 4;
	
	return float4(specular, 1);
}