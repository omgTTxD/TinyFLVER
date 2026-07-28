#include "VS.hlsl"

float4 DrawOutline(In i) : SV_Position
{
	float width = dynamic ? 0.00025 * (0.5 + 0.5 * cos(time)) : 0.0005;
	return mul(float4(i.pos + i.normal * width * sqrt(W[3][2]), 1), WVP); // M[3][2] is camera distance
	//o.col *= dynamic ? cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 8 + time ) : 1;
}

float4 FillBackground(In i) : SV_Position
{
	return mul(float4(i.pos, 1), WVP);
}

float4 BackgroundPS(float4 pos : SV_Position) : SV_Target
{
	//outDepth = pos.z / pos.w;
	return 0;
}

float4 OutlinePS(float4 pos : SV_Position) : SV_Target
{
	return sRGB(float3(1, 1, 1), 1);
}

uint PSMeshIDs() : SV_Target
{
	return MeshID;
}