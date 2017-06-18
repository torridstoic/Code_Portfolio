cbuffer VRAM : register( b0 )
{
	float2 kOffsets;
	float2 backbuffer_dimension;
};

#define SAMPLES 8
Texture2DMS<float4,SAMPLES> base : register(t0);
SamplerState texture_settings[4] : register(s0);
int2 samp_off = int2(0,0);

float4 main(float2 base_uv : TEXCOORD0, float4 pos: SV_POSITION) : SV_TARGET
{
	float4 color = 0;
	float x = kOffsets.x;
	float y = kOffsets.y;
	int2 coord = int2(x*floor(pos.x/x),y*floor(pos.y/y));

	[unroll]
	for(int samp = 0; samp < SAMPLES; ++samp)
	{
		color += base.Load(coord,samp);
	}

	color *= (1.0f/SAMPLES);
	return color;
}