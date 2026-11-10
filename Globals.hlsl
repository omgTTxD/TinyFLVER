#include "Normals.hlsl"

float3 Fresnel(float3 F0, float dot, float r)				
{ 
	return lerp(F0, max(1 - r, F0), pow(1 - dot, 5));
}

float Distribution(float NdotH, float r)
{
	float a = pow(r, 4);
	float denom = NdotH * NdotH * (a - 1) + 1;
	return a / (pi * pow(denom, 2));
}

float Geometry(float3 N, float3 V, float3 L, float r)
{
	//float k = (r + 1) * (r + 1) / 8;
	float k = pow(r, 4) / 2;
	float ggx1 = pdot(N, V) /  (pdot(N, V) * (1 - k) + k);
	float ggx2 = pdot(N, L) / (pdot(N, L) * (1 - k) + k);
	return ggx1 * ggx2;
}

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
float3 GetSubsurfaceColor()
{
	if (!IsSkin) return 0;
	float3 sssColor = { 0.4, 0.1, 0.1 };
	float diffuseWrap = saturate((pdot(N, L) + 0.25) / (1.0 + 0.25));
	float VdotH = pdot(V, normalize(L + N));
	float transmittance = VdotH;		
	float shadowMask = 1 - diffuseWrap;
	return sssColor * (transmittance * 0.012 + shadowMask * 0.1);
}
void InitGlobals(In input, bool facing)
{
	i = input; uv = i.uv; pos = i.pos; 
	alpha = GetAlpha(uv);
	
	N = GetNormal(i); if (!facing) N = -N;	
	V = normalize(CamPos - i.pos);
	L = GetPointLight(i.pos, V, N);
	H = normalize(L + V);
	
	r = GetRoughness(); 
	m = GetMetalness();
	lut = BRDF_LUT.Sample(clamped, float2(pdot(N, V), r)).xy;

	diffuseColor = GetDiffuseColor();
	specularColor = GetSpecularColor();
	
	G = Geometry(N, V, L,r);
	D = Distribution(pdot(N, H), r);	
	F = Fresnel(specularColor, pdot(V, H), r);
	F_IBL = Fresnel(specularColor, pdot(V, N), r);
}

float Shadow()
{	
 /*   float3 v = (pos - light);
	v = mul((float3x3)ShadowRot, v);
    float current = length(v) / 10 ;
    float stored  = PointShadowMap.Sample(s, v);  // нормализовать v не нужно
  
	
	return current - 0.001 > stored ? 0.0 : 1.0;
	if (IsNull(Depth) ) 
		return 1;*/
	
	
	float4 lp  = mul(i.pos, LightViewProjection);
	lp.xyz /= lp.w;
	float2 uv = lp.xy * float2(0.5, -0.5) + 0.5;
	return max(Depth.SampleCmpLevelZero(ShadowSmp, uv, lp.z - 0.005), lerp(0, 0.5, dot(V, L)));
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
	return ssColor * diffuseColor *pow(profile, 2) * saturate(0.3 + dot(L, -N));
  // return lerp(0, ssColor * diffuseColor, pow(profile, 1) * saturate(0.5 + dot(L, -N)));
}


