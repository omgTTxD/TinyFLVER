#include "VS.hlsl"

struct Outline {
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

Outline DrawOutline(In i) {
    Outline o;
	
	float width = dynamic ? 0.00025 * (0.5 + 0.5 * cos(time)) : 0.0005;
	o.pos = mul(float4(i.pos + i.n * width * sqrt(M[3][2]), 1), MVP);		// M[3][2] is camera distance
	o.col =  float4(1, 1, 1, 1);
	o.pos.z += 0.0005 * o.pos.w;
	//o.col *= dynamic ? cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 8 + time ) : 1;
	return o;
}

Outline FillBackground(In i)
{
    Outline o;
	o.pos = mul(float4(i.pos, 1), MVP);
	o.col = 0;
	return o;
}

float4 PS(Outline o) : SV_Target
{
	return sRGB(o.col.rgb, o.col.a);
}

uint PSMeshIDs() : SV_Target
{
    return MeshID;
}