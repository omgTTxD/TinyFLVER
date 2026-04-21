cbuffer VSData : register(b0)
{
    row_major float4x4 World;
	row_major float4x4 Proj;
};
cbuffer PSData : register(b1)
{
    float3 Light;
    float gamma;
    float exposure;
    float time;
    float distance;
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
    float k  = cos(atan2(i.uv.y - 0.5, i.uv.x - 0.5) * 2 + time);
    if (showId == 1)
    {
        o.color.xyz = float3(0, 1, 1) * lerp(0, 1, k);
        i.pos = i.pos + i.normal * 0.00075 * distance * k;
    }
    else
    {
        o.color.xyz = float3(0, 1, 1);
        i.pos = i.pos + i.normal * 0.0005 * distance;
    }
    o.color.a = 1;
	o.pos = mul(float4(i.pos, 1), mul(World, Proj));
  
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