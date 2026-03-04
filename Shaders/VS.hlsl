cbuffer VertexShaderData : register(b0)
{
	row_major float4x4 WorldViewProj;
	row_major float4x4 WorldView;
	float2 Offset;
};

struct VS_IN
{
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

VS_OUT VS(VS_IN i)
{
	VS_OUT o;
	i.pos.xy += Offset;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
	o.posVS = mul(float4(i.pos, 1), WorldView);
	o.normal = mul(i.normal, (float3x3) WorldView);
	o.uv = i.uv;
	return o;
}