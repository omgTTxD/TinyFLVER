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
	int formatID;
	uint MeshID;
	int debugID;
	int FlipX;
	int FlipY;
	int FlipZ;
	int SwapXY;	
	bool RecalcTB;
	int IsSkin;
};

static const float pi = 3.14159265f;

float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }
void swap(float3 a, float3 b) { float3 tmp = a; a = b; b = tmp; }

struct In {
	float3 pos				: POSITION;
	float3 N				: NORMAL;
	float4 T				: TANGENT;
	float3 uv				: TEXCOORD;
};

struct Out
{
	float4 screenCrd		: SV_POSITION;
	float4 pos				: POSITION;
	float3 N				: NORMAL;
	float4 T				: TANGENT;
	float2 uv				: TEXCOORD;
};

Out VS(In i)
{
	Out o;
	o.screenCrd = mul(float4(i.pos, 1), WVP);
	o.pos = mul(float4(i.pos, 1), W);
	o.N = mul(i.N, (float3x3) W);
	o.T = float4(mul(i.T.xyz, (float3x3) W), i.T.w);
	o.uv = i.uv;
	return o;
}


float3 Tonemap_Uchimura(float3 x, float P, float a, float m, float l, float c)
{
    // Math: https://www.desmos.com/calculator/gslcdxvipg
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
	const float P = 3; // max display brightness
	const float a = 1.1; // contrast
	const float m = 0.22; // linear section start
	const float l = 0.4; // linear section length
	const float c = 1.2; // black
	return Tonemap_Uchimura(x, P, a, m, l, c);
}

float4 sRGB(float3 value, float alpha)
{
	//return float4(pow(value * exposure, 1 / gamma), alpha);
	return float4(pow(Tonemap_Uchimura(value) * exposure, 1 / gamma), alpha);
}

float4 Linear(float value, float alpha)
{
	return float4((pow(value, 2.2 / gamma) * exposure).xxx, alpha);
}

float4 OutlineVS(In i) : SV_Position
{
	return mul(float4(i.pos + i.N * 0.0005 * sqrt(W[3][2]), 1), WVP); // M[3][2] is camera distance
}

float4 OutlinePS(float4 pos : SV_Position) : SV_Target
{
	return sRGB(1, 1);
}

float4 BackgroundVS(In i) : SV_Position
{
	return mul(float4(i.pos, 1), WVP);
}

float4 BackgroundPS(float4 pos : SV_Position) : SV_Target
{
	return 0;
}

uint PSMeshIDs() : SV_Target
{
	return MeshID;
}

