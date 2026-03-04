cbuffer PSData : register(b0)
{
	float3 lightPosition;
	float gamma;
	float exposure;
	int showId;
};

cbuffer SphereData : register(b1)
{
	float roughness;
	float metallness;
}

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b) { return max(0, dot(a, b)); }


float3 SkyColor(float3 dir)
{
	float3 zenith = float3(0.15, 0.3, 0.8);
	float3 horizon = float3(0.5, 0.65, 0.85);
	float3 ground = float3(0.15, 0.12, 0.1);
	float3 sky = lerp(horizon, zenith, saturate(pow(max(dir.y, 0), 0.5)));
	float3 color = lerp(ground, sky, smoothstep(-0.02, 0.02, dir.y));
	return color;
}

float NormalDistribution(float NdotH, float r)
{
	float alpha = r * r;
	float alpha2 = alpha * alpha;
	float tmp = (NdotH * NdotH * (alpha2 - 1)) + 1;
	return alpha2 / max(1e-6, pi * tmp * tmp);
}

float3 Fresnel(float3 F0, float x) 
{
	return F0 + (1 - F0) * pow(1 - x, 5);
}

float GeometrySchlickGGX(float NdotV, float k)
{   
	float nom = NdotV;
	float denom = NdotV * (1.0 - k) + k;
	return nom / denom;
}
  
float GeometrySmith(float3 N, float3 V, float3 L, float k)
{
	float ggx1 = GeometrySchlickGGX(pdot(N, V), k);
	float ggx2 = GeometrySchlickGGX(pdot(N, L), k);
	return ggx2 * ggx1;
}


float4 PS(VS_OUT i) : SV_Target
{
	float3 albedo = float(0.5);
	float3 N = normalize(i.normal);
	float3 V = normalize(-i.posVS.xyz);
	float3 L = normalize(lightPosition );
	float3 H = normalize(L + V);
	float3 R = reflect(-V, N);
	
	float3 F0 = lerp(0.02, albedo, metallness);
	float D = NormalDistribution(pdot(N, H), roughness);
	float3 F = Fresnel(F0, pdot(N, V));
	
	float G = GeometrySmith(N, V, L, roughness);
	float3 reflection = SkyColor(N);
	float3 kd = (1 - F) * (1 - metallness);
	float3 km = (1 - F) * metallness;
	float3 diffuse = pdot(N, L) * albedo * kd;
	float3 specular = D * F * G / 4 / pdot(N, V) * float3(0, 1, 1);
//	specular = lerp(lerp(albedo, 0, metallness), specular, roughness);
	float3 ambient = albedo * 0.01;
		
	float3 result;
	switch (showId) {
		case 0: result = ambient + diffuse +  specular ; break;
		case 1: result = diffuse; break;
		case 2: result = roughness; break;
		case 3: result = D;	break;
		case 4: result = F;	break;	
		case 5: result = G;	break;
		case 6: result = specular; break;
		case 7: result = 0; break;
		case 8: result = 0;	break;
		case 9: result = 0; break;	
		case 10: result = ambient; break;	
	};
		
	result = result * exposure;
	result = pow(result, 1 / gamma);
	return float4(result, 1);
}

