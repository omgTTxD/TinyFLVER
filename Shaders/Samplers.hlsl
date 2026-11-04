#include "VS.hlsl"

Texture2D Albedo : register(t0);
Texture2D Normal : register(t1);
Texture2D Metalness : register(t2);
Texture2D Reflectance : register(t3);
Texture2D Specular : register(t4);
Texture2D Subsurface : register(t6);
Texture2D SkinDetails : register(t5);

TextureCube CubeMap : register(t10);
Texture2D RectMap : register(t11);

Texture2D BRDF_LUT : register(t9);

Texture2D Depth : register(t7);
Texture2D BackBuffer : register(t8);
Texture2D MeshIDTexture : register(t12); 

SamplerComparisonState ShadowSmp : register(s1);
SamplerState s : register(s0);
static float w, h;
bool IsNull(Texture2D tex)
{
	tex.GetDimensions(w, h);
	return w == 0;
}

static int Spheres = -1, ER = 0, DS1 = 1, DSR = 2, DS3 = 3;

static In i;
static float2 uv;
static float3 pos;
static float alpha;

static float3 N, V, L, H;
static float r, m;
static float2 lut;
static float3 diffuseColor, specularColor;
static float G, D, F, F_IBL;

float GetAlpha(float2 uv) {
	if (formatID == Spheres)
		return 1;
	
	return IsNull(Albedo) ? 0 : Albedo.Sample(s, uv).a; 
}

float GetMetalness()
{
	if (formatID == Spheres)	return 1 - uv.y;
	if (formatID == DS1)		return F0;	
	if (formatID == DSR)		return Specular.Sample(s, uv).y;
	if (formatID == DS3)
	{
		Reflectance.GetDimensions(w, h);
		float3 color = w == 0 ? 0 : Reflectance.Sample(s, uv);
		return IsNull(Reflectance) ? 0 :  max(color.r, max(color.g, color.b));
	}
	
	return IsNull(Metalness) ? 0 : Metalness.Sample(s, uv).x;
}

float3 GetBaseColor()
{
	if (formatID == Spheres) 
		return pow(1, 2.2);
	
	if (formatID == DS1 || formatID == DSR)	
		return pow(Albedo.Sample(s, uv), 2.2);	
	
	return IsNull(Albedo) ? 0.6 : Albedo.Sample(s, uv);
}

float3 GetDiffuseColor() 
{
	if (formatID == DS3) 
		return GetBaseColor();
	
	return GetBaseColor() * (1 - m);
}

float3 GetSpecularColor()
{	
	if (formatID == DS1)
		return IsNull(Specular) ? 1 : pow(Specular.Sample(s, uv), 2.2);
	
	if (formatID == DS3)
		return IsNull(Reflectance) ? 1 : Reflectance.Sample(s, uv);
	
	return lerp(F0, GetBaseColor(), GetMetalness()) ;
}


bool IsNull(TextureCube tex)
{
	tex.GetDimensions(w, h);
	return w == 0;
}

float3 SphereProject(float3 P, float3 R, float3 center)
{
	float3 d = P - center;
	float b = dot(d, R);
	float c = dot(d, d);
	float t = -b + sqrt(b * b - c);   // P внутри сферы => c < 0, корень всегда есть
	return d + R * t;                 // направление из центра в точку пересечения
}


float3 GetRadiance()
{	
	CubeMap.GetDimensions(w, h);
	if (w == 0) RectMap.GetDimensions(w, h);
	
	float mip = r * log2(w);
	float3 d = reflect(-V, N);
	
	if (!IsNull(RectMap))
	{  
		float u = atan2(d.x, d.z) / (2 * pi) + 0.5; 
	//	u = atan(d.x/ -d.z) / ( 2 *pi) + 0.5; 
		float v =acos(clamp(d.y, -1, 1)) / pi; 
		return pow(RectMap.SampleLevel(s, float2(u, v), mip).rgb, 1);
	}

	if (!IsNull(CubeMap))
		return pow(CubeMap.SampleLevel(s, d, mip).rgb, 1.5);

	return 1;
}

float3 GetIrradiance(float3 N)
{	
	CubeMap.GetDimensions(w, h);
	if (w == 0) RectMap.GetDimensions(w, h);
	
	if (!IsNull(RectMap))
	{  
		float u = atan2(N.x, -N.z) / (2 * pi) + 0.5; 
		float v = acos(clamp(N.y, -1, 1)) / pi; 
		return pow(RectMap.SampleLevel(s, float2(u, v), log2(w)).rgb, 1);
	}
	
	if (!IsNull(CubeMap))
		return pow(CubeMap.SampleLevel(s, N, log2(w) - 0.5).rgb, 1.5);

	return 1;
}

// Тут мы просто превращаем точечный источник света в чуть более толстенький, чтобы красиво отражался в глазах и больше ни для чего.
float3 GetPointLight(float3 pos, float3 V, float3 N)
{
	float radius = formatID == Spheres ? 0.01 : 0.05 ;
	float3 R = reflect(-V, N);
	float3 L = normalize(light - pos);				   
	float3 c = dot(L, R) * R - L;		   
	L += c * saturate(radius / length(c));
	return normalize(L);
}

float GetF0(float2 uv)
{
	if (formatID == DSR) return Specular.Sample(s, uv).z * 0.08;
	
	// Иногда F0 можно получить через текстуру, но надежнее вбивать вручную для двух единственных важных для меня материалов - кожи и глаз. 
	return lerp(0.04, 1, GetMetalness());
}

float GetSSS(float2 uv) 
{
	return Subsurface.Sample(s, uv).x;
}

float GetRoughness()
{
	float roughness;
	
	if (formatID == Spheres)
		roughness = uv.x;

	else if (formatID == DS1)
		roughness = IsDSMetal ? 0.5 : 0.8;
	
	else if (formatID == DSR)
	{
		Specular.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : pow(Specular.Sample(s, uv).x, 1.);
	}
	else
	{
		Normal.GetDimensions(w, h);
		roughness = w == 0 ? 0.5 : 1 - Normal.Sample(s, uv).z;
	}
	
	return max(0.04, roughness);	// without clamping roughness == 0 makes speculars black
}


float Attenuation(float radius)
{
	float dist = length(light - pos);
	return pow(saturate(1 - pow(dist / radius, 4)), 2);              
}

// VdotH является максимальным, когда RdotL близок к еденице, а NdotL - к минусу 1.
float3 GetSubsurfaceColor(float2 uv, float3 N, float3 L, float3 V)
{
	if (!IsSkin) return 0;
	float3 sssColor = { 0.4, 0.1, 0.1 };
	float diffuseWrap = saturate((pdot(N, L) + 0.25) / (1.0 + 0.25));
	float VdotH = pdot(V, normalize(L + N));
	float transmittance = VdotH;		
	float shadowMask = 1 - diffuseWrap;
	return sssColor * (transmittance * 0.012 + shadowMask * 0.1);
}

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


float4 sRGB(float3 v, float a = 1) {
	return float4(pow(Tonemap(v) * exposure, 1 / gamma), a);
}

float4 Linear(float3 v, float a = 1) {
	return float4((pow(v * exposure, 2.2 / gamma)), a);
}


static float4 blurKernel[] = {
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

