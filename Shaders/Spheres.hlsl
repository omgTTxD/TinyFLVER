cbuffer VSData : register(b0)
{
	row_major float4x4 Proj;
	row_major float4x4 World;
};

cbuffer PSData : register(b1)
{
	float3 lightPosition;
	float gamma;
	float exposure;
	float time;
	int showId;
};

cbuffer SpherePSData : register(b2)
{
	float roughness;
	float metalness;
	float2 offset;
}

struct VS_IN
{
	float3 pos : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

struct VS_OUT
{
	float4 pos : SV_POSITION;
	float4 posVS : POSITION;
	float3 normal : NORMAL;
	float2 uv : TEXCOORD;
};

VS_OUT VS(VS_IN i)
{
	VS_OUT o;
	i.pos.xy += offset.xy;
	o.pos = mul(float4(i.pos, 1), mul(World, Proj));
	o.posVS = mul(float4(i.pos, 1), World);
	o.normal = mul(i.normal, (float3x3) World);
	o.uv = i.uv;
	return o;
}

static const float pi = 3.14159265f; 
float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }

float3 SkyColor(float3 dir)
{
	float3 sky	= float3(0.4, 0.5, 0.9);
	float3 ground = float3(0.6, 0.4, 0.3);

	float t = saturate(dir.y + 0.5);

	float3 color = lerp(ground, sky, t);
	return lerp(color, 0.5, roughness);
}

float NormalDistribution(float NdotH, float r)
{
	float alpha = r * r;
	float alpha2 = alpha * alpha;
	float tmp = (NdotH * NdotH * (alpha2 - 1)) + 1.0;
	return alpha2 / max(1e-6, pi * tmp * tmp);
}

float3 FresnelSchlick(float3 F0, float3 F90, float VdotH)
{
	float x = pow(1 - VdotH, 5);
	return F0 + (F90 - F0) * x;
}

float GeometrySchlickGGX(float NdotV, float k)
{
	float denom = NdotV * (1.0 - k) + k;
	return NdotV / max(denom, 1e-6);
}

float GeometrySmith(float3 N, float3 V, float3 L, float k)
{
	float ggxV = GeometrySchlickGGX(pdot(N, V), k);
	float ggxL = GeometrySchlickGGX(pdot(N, L), k);
	return ggxV * ggxL;
}

float4 PS(VS_OUT i) : SV_Target
{
	float3 albedo = 0.8;
	float3 N = normalize(i.normal);
	float3 V = normalize(-i.posVS.xyz);
	float3 L = normalize(-lightPosition);
	float3 H = normalize(L + V);
	float3 F0 = lerp(0.02, albedo, metalness);

	float D = NormalDistribution(pdot(N, H), roughness);
	float3 F = FresnelSchlick(F0, 1, pdot(V, H));
	float G = GeometrySmith(N, V, L, roughness);

	float3 kd = (1 - F) * (1 - metalness);
	float3 diffuse = pdot(N, L) * albedo * kd;
	float3 specular = D * F * G * float3(0, 1, 1) / max(4.0 * pdot(N, V), 1e-6);
	float3 reflection = SkyColor(reflect(-V, N));
	float3 metallic = reflection * F * metalness;
	float3 ambient = albedo * 0.03;

	float3 result;

	switch (showId)
	{
		case 0:  result = ambient + diffuse + specular + metallic; break;
		case 1:  result = diffuse; break;
		case 2:  result = roughness; break;
		case 3:  result = D; break;
		case 4:  result = F; break;
		case 5:  result = G; break;
		case 6:  result = specular; break;
		case 7:  result = 0.0; break;
		case 8:  result = 0.0; break;
		case 9:  result = 0.0; break;
		case 10: result = metallic; break;
		default: result = ambient + diffuse + specular + metallic; break;
	}

	result *= exposure;
	result = pow(result, 1.0 / gamma);

	return float4(result, 1.0);
}