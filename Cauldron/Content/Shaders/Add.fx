#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif
#pragma enable_d3d11_debug_symbols

matrix WorldViewProjection;

texture tex1;

sampler2D tex1Map = sampler_state
{
	Texture = <tex1>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
};
texture tex2;

sampler2D tex2Map = sampler_state
{
	Texture = <tex2>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 UV : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

    output.Position = input.Position;
	output.UV = input.UV;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	return tex2D(tex1Map, input.UV).rgba + tex2D(tex2Map, input.UV).rgba;
}

technique Blit
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};