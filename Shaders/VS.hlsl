#include <Util.Samplers.hlsl>

struct In {
	float3 pos : POSITION;
	float3 n : NORMAL;
	float2 uv : TEXCOORD;
};

Out VS(In i)
{
    Out o;
    o.screenCrd = mul(float4(i.pos, 1), MVP);
    o.pos = mul(float4(i.pos, 1), M);
    o.normal = mul(i.n, (float3x3) M); // Multiplication order matters
    o.uv = i.uv;
    return o;
}