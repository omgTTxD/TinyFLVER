cbuffer VSData : register(b0)
{
	row_major float4x4 WorldViewProj;
	row_major float4x4 WorldView;
};

cbuffer VSData : register(b1)
{
	float cameraDistance;
	float time;
	float bgColor;
};

struct Input {
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

struct Output {
	float4 pos : SV_POSITION;
	float4 color : COLOR;
	float2 uv : TEXCOORD;
};

Output VS1(Input i) {
	Output o;
	o.uv = i.uv;
	float wave = cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 4 + time  );
//	float wave = sin(atan2(i.uv.x, i.uv.x) * 4 + time );
	//float thickness = 1 + 1.5 * pow(cos(time), 2);
	float thickness = 1 + 1 *wave;
	i.pos = i.pos + i.normal * 0.001 * cameraDistance * thickness;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
	// We don't want other meshes to rewrite outline, so we set Z = 0
	o.pos.z = 0;
	o.color.rgb = float3(0, 0.8, 0.8) * lerp(0, 1, wave);
	//o.color.a = 0;

	o.color = float4(0, 1, 1, 1) * lerp(0.6, 1, wave);
	return o;
}

Output VS2(Input i) {
	Output o;
	o.pos = mul(float4(i.pos, 1), WorldViewProj);
	// Before making this pass we disabled depth checking, so this pass can rewrite depth -1 
	// After that we set depth to 100 so any mesh will rewrite background filling color
	o.pos.z = 100;
	o.color = float4(0, 0,0,0);
	return o;
}

float4 PS(Output o) : SV_Target {
	return o.color;
}
