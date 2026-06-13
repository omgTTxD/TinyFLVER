#include <Common.hlsl>

struct In {
	float4 pos : POSITION;
	float3 n	 : NORMAL;
	float3 uv : TEXCOORD;
};


Out VS(In i)
{
    Out o;
	o.pos   = mul(i.pos, MVP);
	o.posVS = mul(i.pos, M);
	o.normal= mul(i.n, (float3x3) M);
	o.uv	= i.uv;
	return o;
}