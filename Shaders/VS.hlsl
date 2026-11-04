// Thanks Google AI to telling me about this. I don't think I would ever thought about it myseft, since C# outlaws macros, and
// now they are almost don't exists in my worldview. But at least I should have made
//    struct vec2 { float x, y; };  
// a loooong time ago, since "vec" is so much better than fucking obnoxious "float" (which not only sounds like absolute shit would sound, but
// he is fucking LONG and breaks all identation. Not only that, whenever I encounter them in C#, they try to force me add useless "f" to them.
// I fucking hate floats...
#define vec1 vector<float, 1>
#define vec2 vector<float, 2>
#define vec3 vector<float, 3>
#define vec4 vector<float, 4>
#define m4x4 row_major float4x4
#define pdot(x, y) (saturate(dot(x, y)))


cbuffer Data : register(b0) 
{	
	m4x4 ViewProjection;
	m4x4 LightViewProjection;
	vec3 light;
	vec1 gamma, exposure, F0, DetailsScale;
	vec1 formatID, MeshID, debugID, FlipX, FlipY;
	bool SwapXY, RecalcTB, IsDSMetal, IsSkin, Cubes;
	vec3 CamPos;
};

static const float pi = 3.14159265f;
void swap(in out float3 a, in out float3 b) { float3 tmp = a; a = b; b = tmp; }

struct In {
	vec4 pos : POSITION;
	vec3 normal : NORMAL;
	vec4 tangent : TANGENT;
	vec2 uv : TEXCOORD;
};

void VS(in out In i, uint id : SV_InstanceID, out float4 svPos : SV_Position) {	
	if (formatID == -1)
	{
		i.uv = float2(id / 5, id % 5) / 4;
		i.pos.xy += i.uv + float2(-0.5, 0.5);
	}
	svPos = mul(i.pos, ViewProjection);
}

float3 Tonemap(float3 x, float a = 1.5, float m = 0.22, float l = 0.4, float c = 1.4) {
//	return x;
	float P = exposure;
	float l0 = ((P - m) * l) / a;
	float L0 = m - m / a;
	float L1 = m + (1 - m) / a;
	float S0 = m + l0;
	float S1 = m + a * l0;
	float C2 = (a * P) / (P - S1);
	float CP = -C2 / P;

	float3 w0 = 1 - smoothstep(0, m, x);
	float3 w2 = step(m + l0, x);
	float3 w1 = 1 - w0 - w2;
	float3 T = m * pow(x / m, c);
	float3 S = P - (P - S1) * exp(CP * (x - S0));
	float3 L = m + a * (x - m);

	return T * w0 + L * w1 + S * w2;
}


