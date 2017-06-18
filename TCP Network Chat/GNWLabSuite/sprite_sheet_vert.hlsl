cbuffer VRAM : register( b0 )
{
	float2 sprite_position;
	float sprite_scale;
	float sprite_rotation;
	float2 sprite_clip_ratio;
	float2 sprite_index;
};

struct INPUT_VERTEX
{
	float3 in_pos : POSITION;
	float2 base_uv : TEXCOORD0;
};

struct OUTPUT_VERTEX
{
	float2 base_uv : TEXCOORD0;
	float4 out_pos : SV_POSITION;
};

OUTPUT_VERTEX main(INPUT_VERTEX struct_in)
{
	OUTPUT_VERTEX struct_out = (OUTPUT_VERTEX)0;
	struct_out.out_pos = float4(struct_in.in_pos,1.0f);

	float s = sin(sprite_rotation);
	float c = cos(sprite_rotation);
	float2x2 rot;
	rot[0][0] = c;
	rot[0][1] = -s;
	rot[1][0] = s;
	rot[1][1] = c;
	struct_out.out_pos.xy = mul(struct_out.out_pos.xy,rot);

	struct_out.out_pos.x *= sprite_clip_ratio.x * sprite_scale;
	struct_out.out_pos.y *= sprite_clip_ratio.y * sprite_scale;

	struct_out.out_pos.xy+=sprite_position;

	struct_out.base_uv = struct_in.base_uv;
	struct_out.base_uv.x /= 31.0;
	struct_out.base_uv.y /= 21.0;
	struct_out.base_uv.x += sprite_index.x/31.0;
	struct_out.base_uv.y += sprite_index.y/21.0;
	return struct_out;
}