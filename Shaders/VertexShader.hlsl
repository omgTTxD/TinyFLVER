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
	float4 tangent : TANGENT;
};

VS_OUT VS(VS_IN i)
{
	VS_OUT o;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
	o.posVS = mul(float4(i.pos, 1), WorldView);
	o.normal = mul(i.normal, (float3x3) WorldView);
	o.tangent.xyz = mul(i.tangent.xyz, (float3x3) WorldView);
	o.tangent.w = i.tangent.w;
	o.uv = i.uv;
	return o;
}