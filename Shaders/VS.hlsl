
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
    int RecalcTan;
};

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }

float4 sRGB(float3 value, float alpha) { return float4(pow(value * exposure, 1 / gamma), alpha); }
float4 Linear(float3 value, float alpha) { return sRGB(pow(value, 2.2), alpha); }


struct In {
	float3 pos : POSITION;
	float3 n : NORMAL;
    float4 tangent : TANGENT;
	float2 uv : TEXCOORD;
};

struct Out
{
    float4 screenCrd : SV_POSITION;
    float4 pos : POSITION;
    float3 normal : NORMAL;
    float3 tangent : TANGENT;
    float3 bitangent : BINORMAL;
    float2 uv : TEXCOORD;
};

Out VS(In i)
{
    Out o;
    o.screenCrd = mul(float4(i.pos, 1), MVP);
    o.pos = mul(float4(i.pos, 1), M);
    o.normal = mul(i.n, (float3x3) M);
    o.tangent = mul(i.tangent.xyz, (float3x3) M);
    o.bitangent = cross(o.normal, o.tangent) * i.tangent.w;
    o.uv = i.uv;
    return o;
}





