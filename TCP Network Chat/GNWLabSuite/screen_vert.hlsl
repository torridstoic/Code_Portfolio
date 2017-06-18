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
	struct_out.base_uv = struct_in.base_uv;
	return struct_out;
}