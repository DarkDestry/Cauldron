
#pragma enable_d3d11_debug_symbols

float4x4 World;
float4x4 View;
float4x4 Projection;

Texture2D colorMapTexture;

float width;
float height;
float lineWidth;

float2 offsets[8];

SamplerState colorMap
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
};

struct VertexShaderOutput
{
	float2 UV : TEXCOORD0;
	float4 Position : POSITION0;
	float4 DepthPosition : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	output.UV = input.UV;
	
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
    return float4(1.0f,1.0f,0.0f,1.0f);
}


VertexShaderOutput OutlineVS(in VertexShaderInput input)
{
	VertexShaderOutput output;

	output.Position = input.Position;
	output.UV = input.UV;

	output.DepthPosition = output.Position;

	return output;
}

float4 OutlinePS(VertexShaderOutput input) : SV_TARGET
{
    float4 color = tex2D(colorMap, input.UV);

    for (int i = 0; i < 8; i++)
    {
		float2 localOffset = float2(lineWidth / width, lineWidth / height);
		localOffset = input.UV + offsets[i] * localOffset;
		localOffset.x = clamp(localOffset.x, 0.0001f, 0.9999f);
		localOffset.y = clamp(localOffset.y, 0.0001f, 0.9999f);
		//if (localOffset.x > 1 || localOffset.x < 0 || localOffset.y < 0 || localOffset.y > 1) continue;
        float4 samp = tex2D(colorMap, localOffset);
        color.rgb = max(samp.rgb, color.rgb);
    }

	color.rgb -= tex2D(colorMap, input.UV).rgb;
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
		VertexShader = compile vs_4_0 OutlineVS();
        PixelShader = compile ps_4_0 OutlinePS();
    }
}