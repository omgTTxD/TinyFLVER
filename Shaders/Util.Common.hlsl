cbuffer VSData : register(b0)
{
    row_major float4x4 M;
    row_major float4x4 MVP;
};

cbuffer PSData : register(b1)
{
    float3 light;
    float gamma;
    float exposure;
    float time;
    int dynamic;
    int formatID;
    uint MeshID;
    int debugID;
    int FlipX;
    int FlipY;
    int SwapXY;
};

struct Out
{
    float4 screenCrd : SV_POSITION;
    float4 pos : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD;
};

static const float pi = 3.14159265f;

float pdot(float3 a, float3 b)
{
    return saturate(dot(a, b));
}
float4 sRGB(float3 value, float alpha)
{
    return float4(pow(value * exposure, 1 / gamma), alpha);
}
float4 sRGB(float3 value)
{
    return float4(pow(value * exposure, 1 / gamma), 1);
}
float4 Linear(float value)
{
    return sRGB(pow(float3(value, value, value), 2.2), 1);
}
float4 Linear(float3 value)
{
    return sRGB(pow(value, 2.2), 1);
}