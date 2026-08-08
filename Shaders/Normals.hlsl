#include "Utility.hlsl"

float4 res(float v) { v = v * 0.5 + 0.5; return Linear(v, 1); }

// After many hours of peeking into normals trying to figure out which one is better, and after implementing debug shaders several times, I FINALLY got the idea to output every 
// channel of every normal-related data. 

float4 PS(Out i) : SV_Target
{	
	float3 N = normalize(i.N), T, B; N.z *= FlipZ;
	float3 textureN = GetNormalFromTexture(i.uv, Normal);
	
	RecalculateTangents(i.pos.xyz, i.uv, N, T, B);
	float TanW = sign(dot(B, cross(T, N))) > 0 ? 1 : -1;
	
	float3 Bf = i.T * FlipY;
	float3 Tf = normalize(cross(N, Bf)) * i.T.w * FlipX;
	
	if (formatID == 1)
	{
		Tf = (i.T);
		Bf = normalize(cross(N, Tf)) * i.T.w;
		float3 tmp = T.z; T.z = T.y; T.y = tmp;	
		T.z *= -1;
		B.x *= -1;
		TanW *= -1;
	}
		
	if (SwapXY)
	{ 
		float3 tmp = Tf; Tf = Bf; Bf = tmp;	
	}

	float3 finalN = normalize(mul(textureN, float3x3(T, B, N)));
	float3 finalFlverN = normalize(mul(textureN, float3x3(Tf, Bf, N)));
	
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
		case 18: return res(i.T.w);
		case 19: return res(TanW);
		case 20: return sRGB(i.uv.x, 1);
		case 21: return sRGB(i.uv.y, 1);	
		case 22: return res(N.x);
		case 23: return res(N.y);
		case 24: return res(N.z);
		case 25: return res(textureN.x);
		case 26: return res(textureN.y);
		case 27: return res(textureN.z);

	}
	
	return 0;
}
				 
				 