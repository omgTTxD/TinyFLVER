cbuffer VSData : register(b0)
{
	row_major float4x4 Proj;
	row_major float4x4 World;
};

cbuffer PSData : register(b1)
{
	float3 Light;
	float gamma;
	float exposure;
	float time;
	int showId;
};

struct Input {
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

struct Output {
	float4 pos : SV_POSITION;
	float4 color : COLOR;
};

Output VS1(Input i) {
	Output o;
	float width = abs(cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 4 + time * 1));
//	float width = 1;
	o.color = float4(1, 0.2, 0, 1) * lerp(0, 1, width);
	i.pos = i.pos + i.normal * 0.004 * width;
	o.pos = mul(float4(i.pos, 1), mul(World, Proj));
	o.pos.z = 0;

	return o;
}

Output VS2(Input i) {
	Output o;
	o.pos = mul(float4(i.pos, 1), mul(World, Proj));
	o.pos.z = 100;
	o.color = 0;
	return o;
}

float4 PS(Output o) : SV_Target {
	float3 result = o.color.rgb;
	result = result * exposure;
	result = pow(result, 1 / gamma);
	return float4(result, o.color.a);
}