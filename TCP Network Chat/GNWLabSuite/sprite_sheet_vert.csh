#if 0
//
// Generated by Microsoft (R) HLSL Shader Compiler 6.3.9600.16384
//
//
// Buffer Definitions: 
//
// cbuffer VRAM
// {
//
//   float2 sprite_position;            // Offset:    0 Size:     8
//   float sprite_scale;                // Offset:    8 Size:     4
//   float sprite_rotation;             // Offset:   12 Size:     4
//   float2 sprite_clip_ratio;          // Offset:   16 Size:     8
//   float2 sprite_index;               // Offset:   24 Size:     8
//
// }
//
//
// Resource Bindings:
//
// Name                                 Type  Format         Dim Slot Elements
// ------------------------------ ---------- ------- ----------- ---- --------
// VRAM                              cbuffer      NA          NA    0        1
//
//
//
// Input signature:
//
// Name                 Index   Mask Register SysValue  Format   Used
// -------------------- ----- ------ -------- -------- ------- ------
// POSITION                 0   xyz         0     NONE   float   xyz 
// TEXCOORD                 0   xy          1     NONE   float   xy  
//
//
// Output signature:
//
// Name                 Index   Mask Register SysValue  Format   Used
// -------------------- ----- ------ -------- -------- ------- ------
// TEXCOORD                 0   xy          0     NONE   float   xy  
// SV_POSITION              0   xyzw        1      POS   float   xyzw
//
vs_4_0
dcl_constantbuffer cb0[2], immediateIndexed
dcl_input v0.xyz
dcl_input v1.xy
dcl_output o0.xy
dcl_output_siv o1.xyzw, position
dcl_temps 3
mul r0.xy, cb0[1].zwzz, l(0.032258, 0.047619, 0.000000, 0.000000)
mad o0.xy, v1.xyxx, l(0.032258, 0.047619, 0.000000, 0.000000), r0.xyxx
sincos r0.x, r1.x, cb0[0].w
mov r2.z, r0.x
mov r2.y, r1.x
mov r2.x, -r0.x
dp2 r0.x, v0.xyxx, r2.xyxx
dp2 r0.y, v0.xyxx, r2.yzyy
mul r0.zw, cb0[0].zzzz, cb0[1].xxxy
mul r1.xy, r0.zwzz, r0.yxyy
add o1.xy, r1.xyxx, cb0[0].xyxx
mov o1.z, v0.z
mov o1.w, l(1.000000)
ret 
// Approximately 14 instruction slots used
#endif

const BYTE sprite_sheet_vert[] =
{
     68,  88,  66,  67, 147,  32, 
     80, 213, 225,  18, 200, 194, 
    107,  54,  22,  88, 219, 226, 
    173,  71,   1,   0,   0,   0, 
    184,   4,   0,   0,   5,   0, 
      0,   0,  52,   0,   0,   0, 
    180,   1,   0,   0,   8,   2, 
      0,   0,  96,   2,   0,   0, 
     60,   4,   0,   0,  82,  68, 
     69,  70, 120,   1,   0,   0, 
      1,   0,   0,   0,  68,   0, 
      0,   0,   1,   0,   0,   0, 
     28,   0,   0,   0,   0,   4, 
    254, 255,   0,   1,   0,   0, 
     67,   1,   0,   0,  60,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,   1,   0,   0,   0, 
     86,  82,  65,  77,   0, 171, 
    171, 171,  60,   0,   0,   0, 
      5,   0,   0,   0,  92,   0, 
      0,   0,  32,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0, 212,   0,   0,   0, 
      0,   0,   0,   0,   8,   0, 
      0,   0,   2,   0,   0,   0, 
    228,   0,   0,   0,   0,   0, 
      0,   0, 244,   0,   0,   0, 
      8,   0,   0,   0,   4,   0, 
      0,   0,   2,   0,   0,   0, 
      4,   1,   0,   0,   0,   0, 
      0,   0,  20,   1,   0,   0, 
     12,   0,   0,   0,   4,   0, 
      0,   0,   2,   0,   0,   0, 
      4,   1,   0,   0,   0,   0, 
      0,   0,  36,   1,   0,   0, 
     16,   0,   0,   0,   8,   0, 
      0,   0,   2,   0,   0,   0, 
    228,   0,   0,   0,   0,   0, 
      0,   0,  54,   1,   0,   0, 
     24,   0,   0,   0,   8,   0, 
      0,   0,   2,   0,   0,   0, 
    228,   0,   0,   0,   0,   0, 
      0,   0, 115, 112, 114, 105, 
    116, 101,  95, 112, 111, 115, 
    105, 116, 105, 111, 110,   0, 
      1,   0,   3,   0,   1,   0, 
      2,   0,   0,   0,   0,   0, 
      0,   0,   0,   0, 115, 112, 
    114, 105, 116, 101,  95, 115, 
     99,  97, 108, 101,   0, 171, 
    171, 171,   0,   0,   3,   0, 
      1,   0,   1,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
    115, 112, 114, 105, 116, 101, 
     95, 114, 111, 116,  97, 116, 
    105, 111, 110,   0, 115, 112, 
    114, 105, 116, 101,  95,  99, 
    108, 105, 112,  95, 114,  97, 
    116, 105, 111,   0, 115, 112, 
    114, 105, 116, 101,  95, 105, 
    110, 100, 101, 120,   0,  77, 
    105,  99, 114, 111, 115, 111, 
    102, 116,  32,  40,  82,  41, 
     32,  72,  76,  83,  76,  32, 
     83, 104,  97, 100, 101, 114, 
     32,  67, 111, 109, 112, 105, 
    108, 101, 114,  32,  54,  46, 
     51,  46,  57,  54,  48,  48, 
     46,  49,  54,  51,  56,  52, 
      0, 171, 171, 171,  73,  83, 
     71,  78,  76,   0,   0,   0, 
      2,   0,   0,   0,   8,   0, 
      0,   0,  56,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   3,   0,   0,   0, 
      0,   0,   0,   0,   7,   7, 
      0,   0,  65,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   3,   0,   0,   0, 
      1,   0,   0,   0,   3,   3, 
      0,   0,  80,  79,  83,  73, 
     84,  73,  79,  78,   0,  84, 
     69,  88,  67,  79,  79,  82, 
     68,   0, 171, 171,  79,  83, 
     71,  78,  80,   0,   0,   0, 
      2,   0,   0,   0,   8,   0, 
      0,   0,  56,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   3,   0,   0,   0, 
      0,   0,   0,   0,   3,  12, 
      0,   0,  65,   0,   0,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,   3,   0,   0,   0, 
      1,   0,   0,   0,  15,   0, 
      0,   0,  84,  69,  88,  67, 
     79,  79,  82,  68,   0,  83, 
     86,  95,  80,  79,  83,  73, 
     84,  73,  79,  78,   0, 171, 
    171, 171,  83,  72,  68,  82, 
    212,   1,   0,   0,  64,   0, 
      1,   0, 117,   0,   0,   0, 
     89,   0,   0,   4,  70, 142, 
     32,   0,   0,   0,   0,   0, 
      2,   0,   0,   0,  95,   0, 
      0,   3, 114,  16,  16,   0, 
      0,   0,   0,   0,  95,   0, 
      0,   3,  50,  16,  16,   0, 
      1,   0,   0,   0, 101,   0, 
      0,   3,  50,  32,  16,   0, 
      0,   0,   0,   0, 103,   0, 
      0,   4, 242,  32,  16,   0, 
      1,   0,   0,   0,   1,   0, 
      0,   0, 104,   0,   0,   2, 
      3,   0,   0,   0,  56,   0, 
      0,  11,  50,   0,  16,   0, 
      0,   0,   0,   0, 230, 138, 
     32,   0,   0,   0,   0,   0, 
      1,   0,   0,   0,   2,  64, 
      0,   0,   8,  33,   4,  61, 
     49,  12,  67,  61,   0,   0, 
      0,   0,   0,   0,   0,   0, 
     50,   0,   0,  12,  50,  32, 
     16,   0,   0,   0,   0,   0, 
     70,  16,  16,   0,   1,   0, 
      0,   0,   2,  64,   0,   0, 
      8,  33,   4,  61,  49,  12, 
     67,  61,   0,   0,   0,   0, 
      0,   0,   0,   0,  70,   0, 
     16,   0,   0,   0,   0,   0, 
     77,   0,   0,   8,  18,   0, 
     16,   0,   0,   0,   0,   0, 
     18,   0,  16,   0,   1,   0, 
      0,   0,  58, 128,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  54,   0,   0,   5, 
     66,   0,  16,   0,   2,   0, 
      0,   0,  10,   0,  16,   0, 
      0,   0,   0,   0,  54,   0, 
      0,   5,  34,   0,  16,   0, 
      2,   0,   0,   0,  10,   0, 
     16,   0,   1,   0,   0,   0, 
     54,   0,   0,   6,  18,   0, 
     16,   0,   2,   0,   0,   0, 
     10,   0,  16, 128,  65,   0, 
      0,   0,   0,   0,   0,   0, 
     15,   0,   0,   7,  18,   0, 
     16,   0,   0,   0,   0,   0, 
     70,  16,  16,   0,   0,   0, 
      0,   0,  70,   0,  16,   0, 
      2,   0,   0,   0,  15,   0, 
      0,   7,  34,   0,  16,   0, 
      0,   0,   0,   0,  70,  16, 
     16,   0,   0,   0,   0,   0, 
    150,   5,  16,   0,   2,   0, 
      0,   0,  56,   0,   0,   9, 
    194,   0,  16,   0,   0,   0, 
      0,   0, 166, 138,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   6, 132,  32,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,  56,   0,   0,   7, 
     50,   0,  16,   0,   1,   0, 
      0,   0, 230,  10,  16,   0, 
      0,   0,   0,   0,  22,   5, 
     16,   0,   0,   0,   0,   0, 
      0,   0,   0,   8,  50,  32, 
     16,   0,   1,   0,   0,   0, 
     70,   0,  16,   0,   1,   0, 
      0,   0,  70, 128,  32,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,  54,   0,   0,   5, 
     66,  32,  16,   0,   1,   0, 
      0,   0,  42,  16,  16,   0, 
      0,   0,   0,   0,  54,   0, 
      0,   5, 130,  32,  16,   0, 
      1,   0,   0,   0,   1,  64, 
      0,   0,   0,   0, 128,  63, 
     62,   0,   0,   1,  83,  84, 
     65,  84, 116,   0,   0,   0, 
     14,   0,   0,   0,   3,   0, 
      0,   0,   0,   0,   0,   0, 
      4,   0,   0,   0,   8,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   1,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   4,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0,   0,   0,   0,   0, 
      0,   0
};
