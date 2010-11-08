//-----------------------------------------------------------------------------
// DrawModel.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

float4x4 World;
float4x4 GameViewProj;
float4x4 LightViewProj;

float3 LightDirection;
float4 AmbientColor = float4(0.05, 0.05, 0.05, 0);
float4 PointLight1;
float DepthBias = 0.01f;

texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = (Texture);
};

texture ShadowMap;
sampler ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
	 magfilter = LINEAR;
	 minfilter = LINEAR;
	 mipfilter=LINEAR;
	 AddressU = clamp;
	 AddressV = clamp;
};

struct DrawWithShadowMap_VSIn
{
    float4 Position : POSITION;
    float3 Normal   : NORMAL0;
    float2 TexCoord : TEXCOORD0;
};

struct DrawWithShadowMap_VSOut
{
    float4 Position : POSITION;
    float3 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
};

struct DrawWithShadowMap_PSIn
{
    float4 Position : POSITION;
    float4 Normal   : TEXCOORD0;
    float2 TexCoord : TEXCOORD1;
    float4 WorldPos : TEXCOORD2;
};

struct CreateShadowMap_VSOut
{
    float4 Position : POSITION;
    float Depth     : TEXCOORD0;
};

// Transforms the model into light space an renders out the depth of the object
CreateShadowMap_VSOut CreateShadowMap_VertexShader(float4 Position: POSITION)
{
    CreateShadowMap_VSOut Out;
    Out.Position = mul(Position, mul(World, LightViewProj)); 
    Out.Depth = Out.Position.z / Out.Position.w;    
    return Out;
}

// Saves the depth value out to the 32bit floating point texture
float4 CreateShadowMap_PixelShader(CreateShadowMap_VSOut input) : COLOR
{ 
    return float4(input.Depth, 0, 0, 0);
}

// Draws the model with shadows
DrawWithShadowMap_VSOut DrawWithShadowMap_VertexShader(DrawWithShadowMap_VSIn input)
{
    DrawWithShadowMap_VSOut Output;
	  
    // Transform the models verticies and normal
    Output.Position = mul(input.Position, mul(World,GameViewProj));
    Output.Normal =  normalize(mul(input.Normal, World));
    Output.TexCoord = input.TexCoord;
    
    // Save the vertices postion in world space
    Output.WorldPos = mul(input.Position, World);
    
    return Output;
}

// Determines the depth of the pixel for the model and checks to see 
// if it is in shadow or not
float4 DrawWithShadowMap_PixelShader(DrawWithShadowMap_PSIn input) : COLOR
{ 
    // Color of the model
	 float4 diffuseColor;
	 float diffuseIntensity;
	float4 diffuse;

	// Find the position of this pixel in light space
    float4 lightingPosition = mul(input.WorldPos, LightViewProj);
    
    // Find the position in the shadow map for this pixel
    float2 ShadowTexCoord = 0.5 * lightingPosition.xy / 
                            lightingPosition.w + float2( 0.5, 0.5 );
    ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

    // Get the current depth stored in the shadow map
    float shadowdepth = tex2D(ShadowMapSampler, ShadowTexCoord).r; 
	 float ourdepth = (lightingPosition.z / lightingPosition.w) - DepthBias;

	 //if (shadowdepth <= ourdepth){
		//diffuse = AmbientColor;
	 //} else {
      diffuseColor = float4( 0.75 , 0.75 , 0.75 , 1 );//tex2D(TextureSampler, input.TexCoord);
		diffuseIntensity = saturate(dot(LightDirection, input.Normal));
		diffuse = diffuseIntensity * diffuseColor + AmbientColor;
		//diffuse = diffuseColor + AmbientColor;
    //};

    return diffuse;
}

// Technique for creating the shadow map
technique CreateShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 CreateShadowMap_VertexShader();
        PixelShader = compile ps_2_0 CreateShadowMap_PixelShader();
    }
}

// Technique for drawing with the shadow map
technique DrawWithShadowMap
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 DrawWithShadowMap_VertexShader();
        PixelShader = compile ps_2_0 DrawWithShadowMap_PixelShader();
    }
}
