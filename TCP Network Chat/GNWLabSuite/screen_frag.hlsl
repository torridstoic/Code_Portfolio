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
	float2 p = base_uv - 0.5f;
	float r = length(p);
	float a = atan(p.y/p.x);
	a = mod(a, 6.28/kSides);
	a = abs(a - 6.28/kSides/2.0f);
	float2 pos = r * float2(cos(a),sin(a)) + 0.5f + kOffsets;

	return base.Sample(texture_settings[0], pos);
}