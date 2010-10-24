Texture SurfaceTexture;
samplerCUBE TextureSampler = sampler_state 
{ 
    texture = <SurfaceTexture> ; 
    magfilter = LINEAR; 
    minfilter = LINEAR; 
    mipfilter = LINEAR; 
    AddressU = Mirror;
    AddressV = Mirror;
};

float4x4 World : World;
float4x4 View : View;
float4x4 Projection : Projection;

float3 EyePosition : Camera;

struct VS_INPUT 
{
    float4 Position    : POSITION0;
    float3 Normal : NORMAL0;    
};

struct VS_OUTPUT 
{
    float4 Position    : POSITION0;
    float3 ViewDirection : TEXCOORD2;
        
};

float4 CubeMapLookup(float3 CubeTexcoord)
{    
    return texCUBE(TextureSampler, CubeTexcoord);
}

VS_OUTPUT Transform(VS_INPUT Input)
{
    float4x4 WorldViewProjection = mul(mul(World, View), Projection);
    float3 ObjectPosition = mul(Input.Position, World);
    
    VS_OUTPUT Output;
    Output.Position    = mul(Input.Position, WorldViewProjection);
    
    
    Output.ViewDirection = ObjectPosition - EyePosition;    
    
    return Output;
}

struct PS_INPUT 
{    
    float3 ViewDirection : TEXCOORD2;
};

float4 BasicShader(PS_INPUT Input) : COLOR0
{    
    float3 ViewDirection = normalize(Input.ViewDirection);        
    return CubeMapLookup(ViewDirection);
}

technique BasicShaderTech 
{
    pass P0
    {
        VertexShader = compile vs_2_0 Transform();
        PixelShader  = compile ps_2_0 BasicShader();
    }
}
