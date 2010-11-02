float4x4 gWorld : World < string UIWidget="None"; >;
float4x4 gWVP : WorldViewProjection < string UIWidget="None"; >;

float4 Color : COLOR;
float3 BlendPoint : POSITION;
float  Near;
float  Far;

struct VertexShaderInput{
    float4 Position : POSITION;
};

struct VertexShaderOutput{
    float4 Position : POSITION;
	float4 WorldPos : TEXCOORD1;
};

struct PixelShaderInput{
	float4 WorldPos : TEXCOORD1;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input){
    VertexShaderOutput output;

    output.WorldPos = mul(input.Position, gWorld);
    output.Position = mul(input.Position, gWVP);

    return output;
}

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0{
	float4 color = Color;
	float dist = distance(BlendPoint, input.WorldPos);
	color.a = 1 - (dist-Near) / (Far-Near)  ;
    return color;
}

technique Fog
{
    pass Foging
    {
		AlphaBlendEnable = true;
		ZWriteEnable = false;
		  
		SrcBlend = SrcAlpha;
		DestBlend = InvSrcAlpha;

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
