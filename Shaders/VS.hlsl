cbuffer VSData : register(b0)
{
	row_major float4x4 W;
	row_major float4x4 WVP;
};

cbuffer PSData : register(b1)
{
	float3 light;
	float gamma;
	float exposure;
	float time;
	int dynamic;
	int formatID;
	uint MeshID;
	int debugID;
	int FlipX;
	int FlipY;
	int FlipZ;
	int SwapXY;	
	bool RecalcTan;
	int UseDetails;
};

static const float pi = 3.14159265f;
float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }


struct In {
	float3 pos				: POSITION;
	float3 normal			: NORMAL;
	float4 tangent			: TANGENT;
	float3 uv				: TEXCOORD;
};

struct Out
{
	float4 screenCrd		: SV_POSITION;
	float4 pos				: POSITION;
	 float3 normal : NORMAL;
	 float4 tangent : TANGENT;
	float2 uv				: TEXCOORD;
};

Out VS(In i)
{
	Out o;
	o.screenCrd = mul(float4(i.pos, 1), WVP);
	o.pos = mul(float4(i.pos, 1), W);
	o.normal = (mul(i.normal, (float3x3) W));
	o.uv = i.uv;
	o.tangent = (float4(mul(i.tangent.xyz, (float3x3) W), i.tangent.w));
	
	return o;
}

float3 Tonemap_Uchimura(float3 x, float P, float a, float m, float l, float c)
{
    // Uchimura 2017, "HDR theory and practice"
    // Math: https://www.desmos.com/calculator/gslcdxvipg
    // Source: https://www.slideshare.net/nikuque/hdr-theory-and-practicce-jp
	float l0 = ((P - m) * l) / a;
	float L0 = m - m / a;
	float L1 = m + (1.0 - m) / a;
	float S0 = m + l0;
	float S1 = m + a * l0;
	float C2 = (a * P) / (P - S1);
	float CP = -C2 / P;

	float3 w0 = 1.0 - smoothstep(0.0, m, x);
	float3 w2 = step(m + l0, x);
	float3 w1 = 1.0 - w0 - w2;

	float3 T = m * pow(x / m, c);
	float3 S = P - (P - S1) * exp(CP * (x - S0));
	float3 L = m + a * (x - m);

	return T * w0 + L * w1 + S * w2;
}

float3 Tonemap_Uchimura(float3 x)
{
	const float P = 1.5; // max display brightness
	const float a = 1.1; // contrast
	const float m = 0.22; // linear section start
	const float l = 0.4; // linear section length
	const float c = 1.4; // black
	return Tonemap_Uchimura(x, P, a, m, l, c);
}

float4 sRGB(float3 value, float alpha)
{
	return float4(pow(Tonemap_Uchimura(value) * exposure, 1 / gamma), alpha);
	//return float4(pow(value * exposure, 1 / gamma), alpha);
}

float4 Linear(float value, float alpha)
{
	return float4((pow(value, 2.2 + 1 / gamma) * exposure).xxx, alpha);
}