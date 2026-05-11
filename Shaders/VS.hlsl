cbuffer VSData : register(b0)
{
	row_major float4x4 World;
	row_major float4x4 Proj;
};

struct In
{
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

struct Out
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
	float4 tangent : TANGENT;
};

Out VS(In i)
{
	Out o;
	o.pos = mul(float4(i.pos, 1), mul(World, Proj));
	o.posVS = mul(float4(i.pos, 1), World);
	o.normal = mul(i.normal, (float3x3) World);
	o.tangent.xyz = mul(i.tangent.xyz, (float3x3) World);
	o.tangent.w = i.tangent.w;
	o.uv = i.uv;
	return o;
}