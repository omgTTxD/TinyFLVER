cbuffer PerObject : register(b0)
{
	row_major float4x4 WorldViewProj;
	row_major float4x4 World;
};

struct Input {
	float3 pos			: POSITION;
	float3 normal		: NORMAL;
};

struct Output {
	float4 pos			: SV_POSITION;
	float4 color		: COLOR;
};

cbuffer Settings : register(b1)
{
	float CameraDistance;
	float BackgroundColor;
}

// The idea is to render full outline of mesh, then "erase" parts by rendering this pixels again without
// depth checking, and rewrite depth so later we can render meshes again
Output FirstPass(Input i) {
	Output o;

	// Extend mesh a little
	i.pos = i.pos + i.normal * 0.0015 * CameraDistance;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
		
	// We don't want other meshes to rewrite outline, so we set negative Z (0 would also suffice)
	o.pos.z = -1;
	// Fill entire mesh with color
	o.color = float4(0, 0.8f, 1, 1);

	return o;
}

Output SecondPass(Input i) {
	Output o;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
		
	// Before making this pass we disabled depth checking, so this pass can rewrite depth -1, other
	// meshes cant. We also set z to 1.1f so any mesh can rewrite it
	o.pos.z = 10;
	o.color = BackgroundColor;
	
	return o;
}

float4 OutlinePS(Output o) : SV_Target {
	// We either paint in selection color or background color
	return o.color;	
}
