cbuffer VSData : register(b0)
{
	row_major float4x4 ModelViewProjection;
	row_major float4x4 Model;
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
	int RecalcTB;
	int IsSkin;
	int IsDSMetal;
};

static const float pi = 3.14159265f;

float pdot(float3 a, float3 b) { return saturate(dot(a, b)); }
float pdot(float2 a, float2 b) { return saturate(dot(a, b)); }

void swap(inout float3 a, inout float3 b) { float3 tmp = a; a = b; b = tmp; }
float sqr(float a) { return a * a; }
float inv(float a) { return 1 - a; }

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
	float3 normal			: NORMAL;
	float4 tangent			: TANGENT;
	float2 uv				: TEXCOORD;
};

Out VS(In i, uint instanceID : SV_InstanceID)
{	
	Out o;		
	float2 xy = float2(instanceID / 5, instanceID % 5) / 4;
	i.pos.x += xy.x;
	i.pos.y -= xy.y;

	o.screenCrd = mul(float4(i.pos, 1), ModelViewProjection);
	o.pos		= mul(float4(i.pos, 1), Model);
	o.normal	= mul(i.normal, (float3x3)Model);
	o.tangent	= float4(mul(i.tangent.xyz, (float3x3)Model), i.tangent.w);
	
	o.uv = instanceID == 0 ? i.uv : xy;
	return o;
}
//								  Max Bright   Contrast		   Linear section Start / End	  Black
float3 TonemapUchimura(float3 x, float a = 1.3, float m = 0.22, float l = 0.4, float c = 1.3)
{
	//return x;
	float P = exposure;
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

float4 sRGB(float3 value, float alpha)
{
	return float4(pow(TonemapUchimura(value) , 1 / gamma)* exposure, alpha);
}

float4 sRGB(float3 value)
{
	return float4(pow(TonemapUchimura(value), 1 / gamma) * exposure, 1);
}

float4 Linear(float3 value, float alpha)
{
	return float4((pow(value* exposure, 2.2 / gamma)), alpha);
}

float4 Linear(float3 value)
{
	return float4((pow(value* exposure, 2.2 / gamma)), 1);
}