float4x4 World;
float4x4 View;
float4x4 Projection;

texture colorMapTexture;

float2 offsets[] =
{
    float2(1, 1),
    float2(0, 1),
    float2(-1, 1),
    float2(-1, 0),
    float2(-1, -1),
    float2(0, -1),
    float2(1, -1),
    float2(1, 0),
};

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
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 DepthPosition : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;
	
    // Change the position vector to be 4 units for proper matrix calculations.
    input.Position.w = 1.0f;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);

    output.DepthPosition = output.Position;

	return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : SV_TARGET
{
    return float4(1.0f,0.0f,0.0f,1.0f);
}

float4 OutlinePS(float2 texCoord : TEXCOORD0) : SV_TARGET
{
    float4 color = float4(0.0f, 0.0f, 0.0f, 0.0f);
    color.a = 1-tex2D(colorMap, texCoord).a;

    for (int i = 0; i < 8; i++)
    {
        float4 sample = tex2D(colorMap, texCoord + offsets[i]);
        color.rgb = max(sample.rgb, color.rgb);
    }

    return color;
}

technique Outline
{
    pass Pass1
    {
        VertexShader = compile vs_4_0 VertexShaderFunction();
        PixelShader = compile ps_4_0 PixelShaderFunction();
    }

    pass Pass2
    {
        PixelShader = compile ps_4_0 OutlinePS();
    }
}