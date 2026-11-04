#include "Normals.hlsl"

float3 Lighting(float3 L, float3 ssRadius, float roughness)
{
	if (!IsSkin)
		return 0;
	float3 ssColor = { 0.6, 0.255, 0.13 };
    float3 sss = 0.2 * exp(-3 * pdot(N, L) / (ssRadius + 0.001));

    float fre = saturate(1.0 - dot(N, V));
    float r   = 1.0 - roughness;
    float kr  = 0.04 + 0.96 * r * r * pow(fre, 5.0);

    return  //pdot(N, L) * (diffuseColor + kr * G) + 
	diffuseColor * ssColor * ssRadius * sss;
}

void InitGlobals(In input, bool facing)
{
	i = input; uv = i.uv; pos = i.pos; 
	alpha = GetAlpha(uv);
	
	N = GetNormal(i); if (!facing) N = -N;	
	V = normalize(CamPos - pos);
	L = GetPointLight(i.pos, V, N);
	H = normalize(L + V);
	
	r = GetRoughness(); 
	m = GetMetalness();
	lut = BRDF_LUT.Sample(s, float2(pdot(N, V) * 0.99, r)).xy;

	diffuseColor = GetDiffuseColor();
	specularColor = GetSpecularColor();
	
	G = Geometry(N, V, L,r);
	D = Distribution(pdot(N, H), r);	
	F = Fresnel(specularColor, pdot(V, H), r);
	F_IBL = Fresnel(specularColor, pdot(V, N), r);
}

float Shadow()
{	
	if (IsNull(Depth) ) 
		return 1;
	
	float4 lp  = mul(i.pos, LightViewProjection);
	lp.xyz /= lp.w;
	float2 uv = lp.xy * float2(0.5, -0.5) + 0.5;
	return max(Depth.SampleCmpLevelZero(ShadowSmp, uv, lp.z - 0.0001), 0.5);
	
	float s = 0;
	[unroll] for (int y = -2; y <= 2; y++)
	[unroll] for (int x = -2; x <= 2; x++)
		s += Depth.SampleCmpLevelZero(ShadowSmp, uv + float2(x, y) * 0.00005, lp.z - 0.0001);
	return max(s / 25.0, 0.5); 
}

float3 SSSTransmittance() {
	if (!IsSkin) return 0;
	float translucency = GetSSS(uv) == 0 ? GetSSS(uv) : 0.5;
	float scale = 8.25 * (1.0 - translucency) / 0.001;
    float4 shrinkedPos = float4(pos - 0.005 * N, 1.0);

	float4 shadowPosition = mul(shrinkedPos, LightViewProjection);
	float2 uv = shadowPosition.xy * float2(0.5, -0.5) + 0.5;
	float d1 = Depth.Sample(s, uv).r;
	float d2 = shadowPosition.z;
    float d = scale * pow(abs(d1 - d2), 1.4);
    float dd = -d * d;
    float3 profile = float3(0.233, 0.455, 0.649) * exp(dd / 0.0064) +
                     float3(0.1,   0.336, 0.344) * exp(dd / 0.0484) +
                     float3(0.118, 0.198, 0.0)   * exp(dd / 0.187)  +
                     float3(0.113, 0.007, 0.007) * exp(dd / 0.567)  +
                     float3(0.358, 0.004, 0.0)   * exp(dd / 1.99)   +
                     float3(0.078, 0.0,   0.0)   * exp(dd / 7.41);
	
	float3 ssColor = { 0.6, 0.255, 0.13 } ;
//	return ssColor * diffuseColor *pow(profile, 2) * saturate(0.3 + dot(L, -N));
   return lerp(0, ssColor * diffuseColor, pow(profile, 1) * saturate(0.5 + dot(L, -N)));
}