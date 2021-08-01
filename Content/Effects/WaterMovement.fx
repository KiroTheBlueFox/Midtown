#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
Texture2D TerrainTexture;
float2 TerrainTextureSize;
int Height;
float Time;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

sampler2D TerrainTextureSampler = sampler_state
{
    Texture = <TerrainTexture>;
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 coords = input.TextureCoordinates;

	coords.x += /* cos(coords.x * 200.0 + Time * 2.0) */ sin(coords.y * 200.0 + Time * 2.0) / 400.0;
    float4 colorToDraw = tex2D(SpriteTextureSampler, coords);
    float4 currentPixel = tex2D(TerrainTextureSampler, input.TextureCoordinates);
    if (currentPixel.a == 0)
    {
        float maxHeight = clamp(4, 16, Height);
        float offset = 2;
        for (int i = 0; i < maxHeight; i++)
        {
            float yPosition = input.TextureCoordinates.y - (1 + i) / TerrainTextureSize.y;
            if (yPosition >= 0)
            {
                float4 pixelAbove = tex2Dlod(TerrainTextureSampler, float4(coords.x, yPosition, TerrainTextureSize.x, TerrainTextureSize.y));
                if (pixelAbove.a != 0)
                {
                    if (i == 0)
                    {
                        colorToDraw = float4(1, 1, 1, 1);
                    }
                    else
                    {
                        float pixelToTake = input.TextureCoordinates.y - (1 + i * 2) / TerrainTextureSize.y;
                        if (pixelToTake >= 0)
                        {
                            if (colorToDraw.a == 0)
                            {
                                colorToDraw = float4(0, 0, 1, (1 - (i - 1) / maxHeight) / 2 /*(i + 1 == maxHeight) ? 0.25 : 0.5*/);
                                //colorToDraw = tex2Dlod(TerrainTextureSampler, float4(coords.x, pixelToTake, TerrainTextureSize.x, TerrainTextureSize.y));
                            }
                        }
                    }
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }
	
    return colorToDraw * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};