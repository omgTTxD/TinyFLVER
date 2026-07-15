#include "Samplers.hlsl"

float3 GetNormal(Out i)
{
    if (formatID == 3)
        return normalize(i.normal);
    
    float3 textureNormal;
    int mip = 0;
    
    Normal.GetDimensions(w, h);
   
    if (w >= 4096)
        mip = 2;
    else if (w >= 2048)
        mip = 1;
    
    if (w == 0)
        textureNormal = float3(0, 0, 1);
    else
    {
        float2 xy = Normal.SampleLevel(Sampler, i.uv, mip).xy;
        xy = xy * 2 - 1;
        
      if (FlipX) xy.x *= -1;
      if (FlipY) xy.y *= -1;
      if (SwapXY) xy.xy = xy.yx;
       
        float z = sqrt(saturate(1 - dot(xy, xy)));
        textureNormal = normalize(float3(xy, z));
    }
    
    float3 tangent = normalize(i.tangent);
    float3 bitangent = normalize(i.bitangent);
	
/*    if (FlipX)
		tangent *= -1;
    
    if (FlipY)
		bitangent *= -1;
    
    if (SwapXY)
	{
        float3 tmp = tangent;
        tangent = bitangent;
        bitangent = tmp;
	}*/
 /*
	float3 dp1 = ddx(i.pos.xyz);
    float3 dp2 = ddy(i.pos.xyz);
    float2 duv1 = ddx(i.uv);
    float2 duv2 = ddy(i.uv);
    float3x3 M = float3x3(dp1, dp2, cross(dp1, dp2));
    float2x3 inverseM = float2x3(cross(M[1], M[2]),cross(M[2], M[0]));
    float3 T = normalize(mul(float2(duv1.x, duv2.x), inverseM));
    float3 B = normalize(mul(float2(duv1.y, duv2.y), inverseM));
*/	
    float3 N = normalize(i.normal);
    float3 dp1 = ddx(i.pos.xyz); 
    float3 dp2 = ddy( i.pos.xyz); 
    float2 duv1 = ddx(i.uv ); 
    float2 duv2 = ddy(i.uv);   
    float3 dp2perp = cross( dp2, N ); float3 dp1perp = cross( N, dp1 ); 
    float3 T = dp2perp * duv1.x + dp1perp * duv2.x; 
    float3 B = dp2perp * duv1.y + dp1perp * duv2.y;   
    float invmax = 1 / sqrt( max( dot(T,T), dot(B,B) ) ); 
    T *= invmax;
    B *= invmax;
    
    float3x3 m = RecalcTan ? float3x3 (T, B, normalize(i.normal)) : float3x3(tangent, bitangent, normalize(i.normal))  ; 
    
    float3 result = normalize(mul(textureNormal, m));
    return result;
}

