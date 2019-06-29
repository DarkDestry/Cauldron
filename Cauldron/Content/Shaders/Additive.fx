#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

texture tex1;
texture tex2;

sampler2D colorMap = sampler_state
{
    Texture = <colorMapTexture>;
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
	float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
	float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = input.Position;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(
}

technique AdditiveBlend
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};