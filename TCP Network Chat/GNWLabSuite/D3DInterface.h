#pragma once
#pragma warning(disable : 4005)
#pragma warning(disable : 4244)
/*D3D11*/
#include <d3d11.h>
#pragma comment(lib, "d3d11.lib")
/*TOOL KIT*/
#include "DirectXTK\SpriteFont.h"
#include "DirectXTK\SpriteBatch.h"
#include "DirectXTK\DDSTextureLoader.h"

#ifdef _DEBUG

#ifdef _WIN64
#pragma comment(lib, "DirectXTKdebug64.lib")
#else
#pragma comment(lib, "DirectXTKdebug.lib")
#endif

#else

#ifdef _WIN64
#pragma comment(lib, "DirectXTK64.lib")
#else
#pragma comment(lib, "DirectXTK.lib")
#endif

#endif

/*Font Specific*/
//#include <d3d10_1.h>
//#include <DXGI.h>
//#include <D2D1.h>
//#include <sstream>
//#include <dwrite.h>
//#include <d3dx10.h>
//#pragma comment(lib, "D3D10_1.lib")
//#pragma comment(lib, "DXGI.lib")
//#pragma comment(lib, "D2D1.lib")
//#pragma comment(lib, "dwrite.lib")

#include <DirectXMath.h>
using namespace DirectX;

struct matrix_44
{
	union
	{
		struct
		{
			XMMATRIX sim_mat;
		};
		struct
		{
			XMVECTOR sim_right;
			XMVECTOR sim_up;
			XMVECTOR sim_look;
			XMVECTOR sim_pos;
		};

		struct
		{
			XMFLOAT4X4 mat;
		};

		/*Vectors*/
		struct
		{
			XMFLOAT4 right;
			XMFLOAT4 up;
			XMFLOAT4 look;
			XMFLOAT4 pos;
		};

		/*Parts*/
		struct
		{
			FLOAT xx, xy, xz, xw;
			FLOAT yx, yy, yz, yw;
			FLOAT zx, zy, zz, zw;
			FLOAT wx, wy, wz, ww;
		};
	};

	void make_idendity()
	{
		sim_right = g_XMIdentityR0;
		sim_up = g_XMIdentityR1;
		sim_look = g_XMIdentityR2;
		sim_pos = g_XMIdentityR3;
	}
};

struct SPRITE_VERT_CONSTANTS
{
	XMFLOAT2 sprite_position;
	float sprite_scale;
	float sprite_rotation;
	XMFLOAT2 sprite_clip_ratio;
	XMFLOAT2 sprite_index;
};

class D3DInterface
{
	HWND window_handle;
	ID3D11Device* device;
	IDXGISwapChain* swap_chain;
	ID3D11RenderTargetView* main_render_target_view;
	ID3D11DeviceContext* device_context;
	D3D11_VIEWPORT viewport;
	HRESULT res;
	DXGI_SWAP_CHAIN_DESC swap_chain_desc;
	D3D_FEATURE_LEVEL feature_level;
	ID3D11Resource* back_buffer;
	ID3D11Texture2D* depth_buffer;
	D3D11_TEXTURE2D_DESC depth_desc;
	D3D11_DEPTH_STENCIL_VIEW_DESC depth_stencil_view_desc;
	ID3D11DepthStencilView* main_depth_stencil_view;
	ID3D11RasterizerState* raster_state;

	
	SpriteBatch* sprite_batch;
	D3D11_BLEND_DESC font_blend_desc;
	ID3D11BlendState* font_blendstate;
	ID3D11BlendState* font_blendstate2;

	//general descriptors
	D3D11_BUFFER_DESC constant_buffer_desc;
	D3D11_BUFFER_DESC vertex_buffer_desc;
	D3D11_BUFFER_DESC index_buffer_desc;
	D3D11_SAMPLER_DESC sampler_desc;

	//Screen Quad
	ID3D11Buffer* screen_vertex_buffer;
	ID3D11Buffer* screen_index_buffer;
	ID3D11Buffer* screen_pixel_constant_buffer;
	ID3D11VertexShader* screen_vertex_shader;
	ID3D11PixelShader* screen_pixel_shader;
	ID3D11InputLayout* screen_vertex_layout;
	ID3D11ShaderResourceView* screen_tex;
	ID3D11SamplerState* screen_tex_settings;
	XMFLOAT3 kOffsetsandSides;

	//HUD stuff
	ID3D11ShaderResourceView* hud_tex;
	ID3D11VertexShader* hud_vertex_shader;
	ID3D11PixelShader* hud_pixel_shader;
	ID3D11InputLayout* hud_vertex_layout;

	//pokemon studd
	ID3D11ShaderResourceView* poke_sprite_tex;
	ID3D11VertexShader* sprite_vertex_shader;
	ID3D11PixelShader* sprite_pixel_shader;
	ID3D11InputLayout* sprite_vertex_layout;
	ID3D11Buffer* sprite_vertex_constant_buffer;
	SPRITE_VERT_CONSTANTS sprite_vertex_constants;

	//Post Process
	ID3D11Buffer* post_pixelated_pixel_constant_buffer;
	ID3D11VertexShader* post_pixelated_vertex_shader;
	ID3D11PixelShader* post_pixelated_pixel_shader;
	ID3D11InputLayout* post_pixelated_vertex_layout;

	//alternate render target;
	ID3D11Texture2D* pixelated_texture;
	ID3D11ShaderResourceView* pixelated_render_target_tex;
	ID3D11RenderTargetView* pixelated_render_target;
	D3D11_RENDER_TARGET_VIEW_DESC render_target_desc;
	D3D11_TEXTURE2D_DESC render_target_texture_desc;

public:
	//font stuff
	SpriteFont* sprite_font;
	SpriteFont* consolas_11_font;

	D3DInterface(void);
	~D3DInterface(void);
	void Initialize(HWND& hwnd);
	void RenderMenu(void);
	void RenderLobby(void);
	void RenderGame(void);
	void Render(void);
	void Shutdown(void);



};

