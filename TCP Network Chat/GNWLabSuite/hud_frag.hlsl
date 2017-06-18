cbuffer VRAM : register( b0 )
{
	float2 kOffsets;
	float kSides;
};

texture2D base : register(t0);
SamplerState texture_settings[4] : register(s0);

float mod(float x, float y)
{
  return x - y * floor(x/y);
}

float4 main(float2 base_uv : TEXCOORD0) : SV_TARGET
{
	return base.Sample(texture_settings[0], base_uv);
}