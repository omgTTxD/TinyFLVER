cbuffer VertexShaderData : register(b0)
{
    row_major float4x4 WorldViewProj;
    row_major float4x4 WorldView;
    float CameraDistance;
};

struct Input
{
    float3 pos : POSITION;
    float3 normal : NORMAL;
};

struct Output
{
    float4 pos : SV_POSITION;
    float3 color : COLOR;
};

Output VS1(Input i)
{
    Output o;

   // i.normal = mul(i.normal, (float3x3) WorldView);
    i.pos = i.pos + i.normal * 0.0015 * CameraDistance;
    o.pos = mul(float4(i.pos, 1), WorldViewProj);
		
	// We don't want other meshes to rewrite outline, so we set Z = 0
    o.pos.z = 0;
    o.color = float3(0, 0.8, 1);
	
    return o;
}

Output VS2(Input i)
{
    Output o;
    o.pos = mul(float4(i.pos, 1), WorldViewProj);
	
	// Before making this pass we disabled depth checking, so this pass can rewrite depth -1 
	// After that we set depth to 100 so any mesh will rewrite background filling color
    o.pos.z = 100;
    o.color = float(0.0145);
	
    return o;
}

float4 PS(Output o) : SV_Target
{
    return float4(o.color, 1);
}
