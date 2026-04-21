cbuffer PerObject : register(b0)
{
	row_major float4x4 WorldViewProj;
	row_major float4x4 WorldView;
};

struct VS_IN
{
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

// If w = 1 then vector is position. If w == 0, then vector is direction. Translation doesn't work at all if w == 0. Projection breaks if w == 0
// We can use only two spaces - World space, and Camera Space, and Camera space is easier. When calculating angle it is important that vectors are in the same space
// In camera space positive Z looks toward camera.
VS_OUT VS(VS_IN i)
{
	VS_OUT o;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
	o.posVS = mul(float4(i.pos, 1), WorldView);
	o.normal = mul(i.normal, (float3x3) WorldView);
	o.uv = i.uv;
	return o;
}

float4 PS(VS_OUT i) : SV_Target
{		
	float3 Normal = normalize(i.normal);	
	float3 LightDir = normalize(float3(1, 1, -1));	
	float NdotL = dot(Normal, LightDir);
	float3 diffuse = clamp(NdotL, 0, 1);
	return float4(diffuse, 1);
}