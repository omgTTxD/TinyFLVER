struct VSOut
{
    float4 pos : SV_POSITION;
    float2 uv  : TEXCOORD0;
};

VSOut VSFullscreen(uint id : SV_VertexID)
{
    VSOut o;
    o.uv = float2((id << 1) & 2, id & 2);
    o.pos = float4(o.uv * 2 - 1, 0, 1);
    o.pos.y = -o.pos.y;
    return o;
}

Texture2D depthTex : register(t0);
SamplerState pointSampler : register(s0);

float4 PSDepthView(VSOut input) : SV_TARGET
{
    float d = depthTex.Sample(pointSampler, input.uv).r;
    d = pow(d, 8);
  //  d = (nearZ * farZ) / (farZ - d * (farZ - nearZ)) / farZ;
    return float4(d, d, d, 1);
}