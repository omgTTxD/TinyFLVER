#include <Common.hlsl>

struct In {
	float3 pos : POSITION;
	float3 n   : NORMAL;
	float2 uv  : TEXCOORD;
};

struct Outline {
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

Outline DrawOutline(In i) {
    Outline o;
	o.pos = mul(float4(i.pos + i.n * 0.00075 * sqrt(M[3][2]), 1), MVP);		// M[3][2] is camera distance
	o.col = float4(0, 0.6, 0.8, 1);
	o.col *= dynamic ? cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 4 + time ) : 1;
	return o;
}

Outline FillBackground(In i)
{
    Outline o = { mul(float4(i.pos, 1), MVP), float4(0, 0, 0, 0) };
	o.pos.z = 100;
	return o;
}

float4 PS(Outline o) : SV_Target
{
	o.col.rgb = pow(o.col.rgb * exposure, 1 / gamma);
	return o.col;
}