#include "Samplers.hlsl"

float4 OutlineVS(In i) : SV_Position { 
	float cameraDistance = ViewProjection[3][2] * 0.00075;
	return mul(i.pos + float4(i.normal * cameraDistance, 0), ViewProjection); 
}

float4 WhitePS() : SV_Target { 	
	return sRGB(2, 1); 
} 

float MeshIDsPS() : SV_Target {	
	return MeshID; 
}
float4 RenderFullScreenQuadVS(uint id : SV_VertexID, out float2 uv : TEXCOORD) : SV_Position {
	uv = float2((id << 1) & 2, id & 2);
	return float4(uv * float2(2, -2) + float2(-1, 1), 0, 1);
}

float ShadowMapPS(In i, float4 pos : SV_Position) : SV_Target{ 
	clip(GetAlpha(i.uv) - 0.5); 
	return pos.z;
}

float LinearizeDepth(float depth, float n, float f) {
	return n * f / (f - depth * (f - n));
}

float GetDepth(float2 uv)
{
	float z = Depth.Sample(s, uv);
	if (z == 0)		discard;
	return z;
//	return LinearizeDepth(z, 0.1, 5);
}
float PointShadowPS(In i) : SV_Target
{	
	clip(GetAlpha(i.uv) - 0.5); 
    return length(light - i.pos) / 10;
}
//TextureCube<float> PointShadowMap : register(t7);

float4 RenderDebugTexturePS(float4 pos : SV_Position, float2 uv : TEXCOORD) : SV_TARGET
{   
	if (textureID == 1)
		return BackBuffer.Sample(s, uv).xxxw;

	if (textureID == 2)
		return float4(GetDepth(uv).xxx, 1);

	if (textureID == 3) {
		int id = MeshIDTexture.Sample(s, uv);
		if (id == 0) discard;
		return float4(id.xxx, 1);
	}
	
	if (textureID == 4) {  
float2 p = uv * 2 - 1;
float z = PointShadowMap.Sample(s, float3(p.x, -p.y, 1)).x;
	return float4(z, z, z, 1);
	}
	
	return 0;
}


static float4 kernel[] = {
    float4(0.560479, 0.669086, 0.784728, 0),
    float4(0.00471691, 0.000184771, 5.07566e-005, -2),
    float4(0.0192831, 0.00282018, 0.00084214, -1.28),
    float4(0.03639, 0.0130999, 0.00643685, -0.72),
    float4(0.0821904, 0.0358608, 0.0209261, -0.32),
    float4(0.0771802, 0.113491, 0.0793803, -0.08),
    float4(0.0771802, 0.113491, 0.0793803, 0.08),
    float4(0.0821904, 0.0358608, 0.0209261, 0.32),
    float4(0.03639, 0.0130999, 0.00643685, 0.72),
    float4(0.0192831, 0.00282018, 0.00084214, 1.28),
    float4(0.00471691, 0.000184771, 5.07565e-005, 2),
};


static const float NEAR = 0.1, FAR = 10;  

float4 PSBlurSubsurface(float4 svPos , float2 uv, float2 dir) : SV_TARGET
{
	float4 baseColor = BackBuffer.Sample(s, uv);
	float meshID = MeshIDTexture.Sample(s, uv);
	if (meshID < 0)// || baseColor.x == baseColor.y == baseColor.z == 0)
		return baseColor;
	
    float depth = GetDepth(uv);
	
	// Calculate sssWidth scale (1.0 for a unit plane sitting on the projection window): 
    float widthScale = 1 / tan(0.5 * radians(0.5)) / depth;
	float sssWidth = 0.3 / max(1, ViewProjection[2][3]);
	float stepSize = sssWidth * widthScale * dir / 3;
	
	float2 step = sssWidth * sssWidth * 0.1 * dir / 3;
	
	float3 result = baseColor.rgb * kernel[0].rgb;
	
	for (int i = 1; i < 11; i++)
	{
		float2 offset = uv + kernel[i].a * step;
		float4 color = BackBuffer.Sample(s, offset);
		result += kernel[i].rgb * color.rgb;
	}
	return float4(result, baseColor.a);
}

float4 BlurH(float4 pos : SV_POSITION, float2 uv : TEXCOORD) : SV_TARGET
{
	return PSBlurSubsurface(pos, uv, float2(1, 0));
}

float4 BlurV(float4 pos : SV_POSITION, float2 uv : TEXCOORD) : SV_TARGET
{	
	return PSBlurSubsurface(pos, uv, float2(0, 1));
}

