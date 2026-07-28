#include "Common.hlsl"

float4 res(float v) { v = v * 0.5 + 0.5; return Linear(v, 1); }

// After many hours of peeking into normals trying to figure out which one is better, and after implementing debug shaders several times, I FINALLY got the idea to output every 
// channel of every normal-related data. 

float4 PS(Out i) : SV_Target
{
	float3 N = normalize(i.normal);
	N.z *= FlipZ;	
	float3 textureN = GetNormalFromTexture(i.uv);
	
	float3 dp1 = ddx(i.pos); float3 dp2 = ddy(i.pos);
	float2 duv1 = ddx(i.uv); float2 duv2 = ddy(i.uv);
	
	float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
	float2x3 inversedM = float2x3(cross(M[1], M[2]), cross(M[2], M[0]));
	float3 T = normalize(mul(float2(duv1.x, duv2.x), inversedM)) * FlipX;
	float3 B = normalize(mul(float2(duv1.y, duv2.y), inversedM)) * FlipY;
	
	float TanW = dot(B, cross(N, T)) > 0 ? 1 : -1;
	
	float3 Tf = normalize(i.tangent) * FlipX;
	float3 Bf = normalize(cross(N, Tf)) * i.tangent.w * FlipY;

	if (SwapXY)
	{
		float3 tmp = T; T = B; B = tmp;
		tmp = Tf; Tf = Bf; Bf = tmp;	
	}
	
	float3 finalN = normalize(mul(textureN, float3x3(T, B, N)));
	float3 finalFlverN = normalize(mul(textureN, float3x3(Bf, Tf, N)));
	
	switch (debugID)
	{
		case 0:	 return res(Tf.x);
		case 1:  return res(T.x);
		case 2:  return res(Tf.y);
		case 3:  return res(T.y);
		case 4:  return res(Tf.z);
		case 5:  return res(T.z);
		case 6:  return res(Bf.x);
		case 7:  return res(B.x);
		case 8:  return res(Bf.y);
		case 9:  return res(B.y);
		case 10: return res(Bf.z); 	
		case 11: return res(B.z); 	
		case 12: return res(finalFlverN.x);
		case 13: return res(finalN.x);	
		case 14: return res(finalFlverN.y);	
		case 15: return res(finalN.y);	
		case 16: return res(finalFlverN.z);	
		case 17: return res(finalN.z);	
		case 18: return sRGB(i.uv.x, 1);
		case 19: return sRGB(i.uv.y, 1);
		case 20: return res(i.tangent.w);
		case 21: return res(TanW);
	}	
	
	return 0;
}
				 
				 