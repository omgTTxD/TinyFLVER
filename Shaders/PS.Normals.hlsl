#include "Normals.hlsl"

float4 res(float v) { v = v * 0.5 + 0.5; return Linear(v, 1); }

// After many hours of peeking into normals trying to figure out which one is better, and after implementing debug shaders several times, I FINALLY got the idea to output every 
// channel of every normal-related data. 

float4 PS(Out i, bool facing : SV_IsFrontFace) : SV_Target
{	
	float3x3 TBN = GetTBN(i);
	float3 T = TBN[0];
	float3 B = TBN[1];
	float3 N = GetNormal(i);
	float Tw = RecalcTB ? sign(dot(T, cross(N, B))) : i.tangent.w;
	
	switch (debugID) {
		case 0: return res(T.x);
		case 1: return res(T.y);
		case 2: return res(T.z);
		case 3: return res(B.x);
		case 4: return res(B.y);
		case 5: return res(B.z); 	
		case 6: return res(N.x);	
		case 7: return res(N.y);	
		case 8: return res(N.z);	
		case 9: return res(Tw);
		case 10: return sRGB(i.uv.x, 1);
		case 11: return sRGB(i.uv.y, 1);	
	}
	
	return 0;
}
				 
				 