#include <Util.Common.hlsl>

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
	
	float width = dynamic ? 0.00025 * (0.5 + 0.5 * cos(time)) : 0.0005;
	o.pos = mul(float4(i.pos + i.n * width * sqrt(M[3][2]), 1), MVP);		// M[3][2] is camera distance
	o.col = 1;
	
	//o.col *= dynamic ? cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 8 + time ) : 1;
	return o;
}

Outline FillBackground(In i)
{
    Outline o = { mul(float4(i.pos, 1), MVP), float4(0, 0, 0, 0) };
	return o;
}

float4 PS(Outline o) : SV_Target
{
	o.col.rgb = pow(o.col.rgb * exposure, 1 / gamma);
	return o.col;
}

uint PSMeshIDs() : SV_Target
{
    return MeshID;
}