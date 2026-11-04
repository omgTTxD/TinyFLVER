#include "Samplers.hlsl"

float4 OutlineVS(In i) : SV_Position { 
	float cameraDistance = ViewProjection[3][2] * 0.001;
	return mul(i.pos + i.normal * cameraDistance, ViewProjection); 
}

float4 WhitePS() : SV_Target { 	
	return sRGB(1, 1); 
} 

float MeshIDsPS() : SV_Target {	
	return MeshID; 
}

void ShadowMapPS(In i) { 
	clip(GetAlpha(i.uv) - 0.5); 
}

float LinearizeDepth(float depth, float n, float f) {
	return n * f / (f - depth * (f - n));
}

float4 RenderFullScreenQuadVS(uint id : SV_VertexID, out float2 uv : TEXCOORD) : SV_Position {
	uv = float2((id << 1) & 2, id & 2);
	return float4(uv * float2(2, -2) + float2(-1, 1), 0, 1);
}

float GetDepth(float2 uv) { 
    float z = Depth.Sample(s, uv);
	if (z == 1) discard;
	return LinearizeDepth(z, 0.1, 10);
}

float4 RenderDebugTexturePS(float4 pos : SV_Position, float2 uv : TEXCOORD) : SV_TARGET
{   
	//return float4(uv.xyy, 1);
	//	return BackBuffer.Sample(s, uv).xxxw;
	//	return float4(BackBuffer.Sample(s, uv).xxx, 1);
	return float4(GetDepth(uv).xxx, 1);
	int id = MeshIDTexture.Load(int3(pos.xy, 0));
	if (id == 0) discard;
	return float4(id.xxx, 1);
}

static const float NEAR = 0.1, FAR = 5;  

float4 PSBlurSubsurface(float4 svPos, float2 uvIn, uniform float2 dir) : SV_TARGET
{
	int2 size;
	BackBuffer.GetDimensions(size.x, size.y);
	
	float meshID = MeshIDTexture.Load(int3(svPos.xy, 0));
	float4 colorM = BackBuffer.Load(int3(svPos.xy, 0));
	float depthM = LinearizeDepth(Depth.Load(int3(svPos.xy, 0)), NEAR, FAR);
	
		if (meshID < 0)
		return colorM;
	
	float sssWidth = 0.2;
	float pixelsPerMeter = sssWidth * size.y * 0.05 / depthM;
	float2 finalStep = sssWidth * pixelsPerMeter * dir * (1.0 / 3.0);
	float3 blurred = colorM.rgb * blurKernel[0].rgb;

	[unroll]
	for (int i = 1; i < 11; i++)
	{
		float2 uv = saturate((svPos.xy + blurKernel[i].a * finalStep) / size);
		float3 color = BackBuffer.SampleLevel(s, uv, 0).rgb;
		float depth = LinearizeDepth(Depth.SampleLevel(s, uv, 0), NEAR, FAR);
		float k = saturate(300.0 * 0.5 * sssWidth * abs(depthM - depth)); 
		blurred += blurKernel[i].rgb * lerp(color, colorM.rgb, k);
	}
	
	return float4(pow(blurred, 1), colorM.a);
	
}

float4 BlurH(float4 pos : SV_POSITION, float2 uv : TEXCOORD) : SV_TARGET
{
	return PSBlurSubsurface(pos, uv, float2(1, 0));
}

float4 BlurV(float4 pos : SV_POSITION, float2 uv : TEXCOORD) : SV_TARGET
{	
	return PSBlurSubsurface(pos, uv, float2(0, 1));
}

