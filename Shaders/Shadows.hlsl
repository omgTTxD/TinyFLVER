/*cbuffer LightVP : register(b0) { float4x4 lightViewProj; };

float4 VSMain(float3 pos : POSITION) : SV_POSITION
{
    return mul(float4(pos, 1.0), lightViewProj);
}*/