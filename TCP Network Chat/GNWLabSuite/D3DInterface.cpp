//D3D11 Interface, Game Networking Lab Suite
//Tim Turcich
//Game Networking
//Full Sail University
//August-September 2013
#include "D3DInterface.h"

//shader includes
#include "screen_vert.csh"
#include "screen_frag.csh"
#include "pixelated_vert.csh"
#include "pixelated_frag.csh"
#include "hud_vert.csh"
#include "hud_frag.csh"
#include "sprite_sheet_vert.csh"
#include "sprite_sheet_frag.csh"
#include "XTime.h"
#include "MainMenu.h"
#include "ChatLobby.h"
XTime& GetAppTimer(void);
MainMenu& GetMainMenu(void);
XTime& GetTransitionTimer(void);
ChatLobby& GetChatLobby(void);
bool& isTransitioning(void);

D3DInterface::D3DInterface(void)
{
	ZeroMemory(&swap_chain_desc,sizeof(swap_chain_desc));
	ZeroMemory(&depth_desc,sizeof(depth_desc));
	ZeroMemory(&constant_buffer_desc,sizeof(constant_buffer_desc));
	ZeroMemory(&vertex_buffer_desc,sizeof(vertex_buffer_desc));
	ZeroMemory(&index_buffer_desc,sizeof(index_buffer_desc));
	ZeroMemory(&sampler_desc,sizeof(sampler_desc));
	ZeroMemory(&render_target_desc,sizeof(render_target_desc));
	ZeroMemory(&render_target_texture_desc,sizeof(render_target_texture_desc));
}

D3DInterface::~D3DInterface(void)
{

}

D3D11_INPUT_ELEMENT_DESC SCREEN_VERTEX_DESC[] = 
{
	{"POSITION" ,0 , DXGI_FORMAT_R32G32B32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0},
	{"TEXCOORD" ,0 , DXGI_FORMAT_R32G32_FLOAT, 0, D3D11_APPEND_ALIGNED_ELEMENT, D3D11_INPUT_PER_VERTEX_DATA, 0}
};

struct SCREEN_VERTEX
{
	XMFLOAT3 position;
	XMFLOAT2 tex_coords;
};

void D3DInterface::Initialize(HWND& hwnd)
{
	window_handle = hwnd;
	RECT window_size;
	GetClientRect(hwnd,&window_size); 

	viewport.TopLeftY = window_size.top;
	viewport.TopLeftX = window_size.left;
	viewport.Height = window_size.bottom;
	viewport.Width = window_size.right;
	viewport.MinDepth = 0.0f;
	viewport.MaxDepth = 1.0f;

	swap_chain_desc.BufferCount = 1;
	swap_chain_desc.BufferDesc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
	swap_chain_desc.BufferDesc.Height = viewport.Height;
	swap_chain_desc.BufferDesc.Width = viewport.Width;
	swap_chain_desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
	swap_chain_desc.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
	swap_chain_desc.Windowed = TRUE;
	swap_chain_desc.SampleDesc.Count = 8;
	swap_chain_desc.SampleDesc.Quality = D3D11_CENTER_MULTISAMPLE_PATTERN;
	swap_chain_desc.OutputWindow = hwnd;

	res = D3D11CreateDeviceAndSwapChain
		(0/*I choose you*/
		,D3D_DRIVER_TYPE_HARDWARE
		,0/*where do I find one*/
		,
#ifdef _DEBUG 
		D3D11_CREATE_DEVICE_DEBUG
#else 
		0
#endif
		,0/*Feature Level array or NULL*/
		,0/*number of features in the level array*/
		,D3D11_SDK_VERSION
		,&swap_chain_desc
		,&swap_chain
		,&device
		,&feature_level
		,&device_context
		);

	//linking back buffer to context
	res = swap_chain->GetBuffer(0, __uuidof(back_buffer), (void**)&back_buffer);
	res = device->CreateRenderTargetView(back_buffer,0, &main_render_target_view);

	//describing the depth buffer
	depth_desc.Width = viewport.Width;
	depth_desc.Height = viewport.Height;
	depth_desc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
	depth_desc.Format = DXGI_FORMAT_D32_FLOAT;
	depth_desc.Usage = D3D11_USAGE_DEFAULT;
	depth_desc.CPUAccessFlags = 0;
	depth_desc.ArraySize = 1;
	depth_desc.MipLevels = 1;
	depth_desc.SampleDesc.Count = 8;
	depth_desc.SampleDesc.Quality = D3D11_CENTER_MULTISAMPLE_PATTERN;
	//make the depth buffer
	res = device->CreateTexture2D(&depth_desc, 0, &depth_buffer);
	res = device->CreateDepthStencilView(depth_buffer,0,&main_depth_stencil_view);

	//setting some defaults
	device_context->RSSetViewports(1,&viewport);
	device_context->OMSetRenderTargets(1,&main_render_target_view,main_depth_stencil_view);

	//creating alternate render target and giving associated views
	render_target_texture_desc.Width = viewport.Width;
	render_target_texture_desc.Height = viewport.Height;
	render_target_texture_desc.BindFlags = D3D11_BIND_RENDER_TARGET | D3D11_BIND_SHADER_RESOURCE;
	render_target_texture_desc.Format = DXGI_FORMAT_B8G8R8A8_UNORM;
	render_target_texture_desc.Usage = D3D11_USAGE_DEFAULT;
	render_target_texture_desc.CPUAccessFlags = 0;
	render_target_texture_desc.ArraySize = 1;
	render_target_texture_desc.MipLevels = 1;
	render_target_texture_desc.SampleDesc.Count = 8;
	render_target_texture_desc.SampleDesc.Quality = D3D11_CENTER_MULTISAMPLE_PATTERN;

	res = device->CreateTexture2D(&render_target_texture_desc,0,&pixelated_texture);
	res = device->CreateRenderTargetView(pixelated_texture,0,&pixelated_render_target);
	D3D11_SHADER_RESOURCE_VIEW_DESC desc;
	ZeroMemory(&desc,sizeof(desc));
	desc.Texture2D.MipLevels =1;
	desc.Texture2D.MostDetailedMip = 0;
	desc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2DMS;
	res = device->CreateShaderResourceView(pixelated_texture,&desc,&pixelated_render_target_tex);//Needs a texture of post process work while drawing as a quad

	//font stuff
	/*Currently Size 72 Consolas Font, font size 96 is large enough that it will usually require down 
	scaling and most definately never need more than a 2x up scale when being drawn.  
	This solves any issues with blurry text related to scaling the font during the drawing period.*/
	sprite_font = new SpriteFont(device,L"consolas");
	sprite_batch = new SpriteBatch(device_context);

	consolas_11_font = new SpriteFont(device,L"consolas11");

	MainMenu& menu = GetMainMenu();
	XMFLOAT2 font_size;
	XMFLOAT2 font_origin;
	XMStoreFloat2(&font_size,sprite_font->MeasureString(L"JOIN"));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.join_button_rect.top = window_size.bottom - font_size.y;
	menu.join_button_rect.bottom  = window_size.bottom;
	menu.join_button_rect.left = window_size.left;
	menu.join_button_rect.right = menu.join_button_rect.left + font_size.x;
	XMStoreFloat2(&font_size,sprite_font->MeasureString(L"HOST"));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.host_button_rect.top = window_size.bottom - font_size.y;
	menu.host_button_rect.bottom  = window_size.bottom;
	menu.host_button_rect.left = window_size.right - font_size.x - 24.0f;
	menu.host_button_rect.right = menu.host_button_rect.left + font_size.x;
	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.ipaddress.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.ip_rect.top = window_size.bottom/2.0f - font_origin.y - 0.0f;
	menu.ip_rect.left = window_size.right/2.0f - font_origin.x;
	menu.ip_rect.bottom = window_size.bottom/2.0f + font_origin.y - 0.0f;
	menu.ip_rect.right = window_size.right/2.0f + font_origin.x;

	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.port.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.port_rect.top = window_size.bottom/2.0f - font_origin.y + sprite_font->GetLineSpacing();
	menu.port_rect.left = window_size.right/2.0f - font_origin.x;
	menu.port_rect.bottom = window_size.bottom/2.0f + font_origin.y + sprite_font->GetLineSpacing();
	menu.port_rect.right = window_size.right/2.0f + font_origin.x;

	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.user_name.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.user_name_rect.top = window_size.bottom/2.0f - font_origin.y - sprite_font->GetLineSpacing();
	menu.user_name_rect.left = window_size.right/2.0f - font_origin.x;
	menu.user_name_rect.bottom = window_size.bottom/2.0f + font_origin.y - sprite_font->GetLineSpacing();
	menu.user_name_rect.right = window_size.right/2.0f + font_origin.x;

	ChatLobby& lobby = GetChatLobby();
	XMStoreFloat2(&font_size,sprite_font->MeasureString(L"Welcome to Chat Communications"/*lobby.unsent_chat_message.c_str()*/));
	font_origin.x = font_size.x * 0.11111f;
	font_origin.y = font_size.y * 0.11111f;
	lobby.typing_rect.top = window_size.bottom - font_origin.y;
	lobby.typing_rect.left = window_size.right/2.0f - font_origin.x;
	lobby.typing_rect.bottom = window_size.bottom + font_origin.y;/* - sprite_font->GetLineSpacing()*0.111111f*/;
	lobby.typing_rect.right = window_size.right/2.0f + font_origin.x;

	XMStoreFloat2(&font_size,consolas_11_font->MeasureString(L"QWERTYUIOPASDFGHJK"/*lobby.unsent_chat_message.c_str()*/));
	font_origin.x = font_size.x * 0.0875;
	font_origin.y = font_size.y * 0.0875;
	lobby.user_list_rect.top = window_size.bottom - font_origin.y*0.725*5.70f;
	lobby.user_list_rect.left = window_size.right/2.0f - font_origin.x/2.0f;
	lobby.user_list_rect.bottom = window_size.bottom - font_origin.y*0.725*1.70f;/* - sprite_font->GetLineSpacing()*0.111111f*/;
	lobby.user_list_rect.right = window_size.right/2.0f + font_origin.x/2.0f;

	XMStoreFloat2(&font_size, consolas_11_font->MeasureString(L"OOOOOOOOOOHHHHHHHHHHGGGGGGGGGGDDDDDDDDDDWWWWWWW"));
	font_origin.x = font_size.x * 0.0875;
	font_origin.y = font_size.y * 0.0875;
	lobby.message_rect.top = window_size.bottom - font_origin.y*0.725*16.75f;
	lobby.message_rect.left = window_size.right/2.0f - font_origin.x/2.0f;
	lobby.message_rect.bottom = window_size.bottom - font_origin.y*0.725*6.75f;
	lobby.message_rect.right = window_size.right/2.0f + font_origin.x/2.0f;

	XMStoreFloat2(&font_size, consolas_11_font->MeasureString(L"LEAVE"));
	font_origin.x = font_size.x * 0.2;
	font_origin.y = font_size.y * 0.2;
	lobby.leave_rect.top = window_size.bottom - font_origin.y*1.8f;
	lobby.leave_rect.left = window_size.right/2.0f - font_origin.x/2.0f - 125;
	lobby.leave_rect.bottom = window_size.bottom - font_origin.y*0.8f;
	lobby.leave_rect.right = window_size.right/2.0f + font_origin.x/2.0f - 125;

	//describing the blending operations for the fonts while drawing
	font_blend_desc.AlphaToCoverageEnable = FALSE;
	font_blend_desc.IndependentBlendEnable = FALSE;
	font_blend_desc.RenderTarget[0].BlendEnable = TRUE;
	font_blend_desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
	font_blend_desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
	font_blend_desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

	device->CreateBlendState(&font_blend_desc,&font_blendstate);

	font_blend_desc.AlphaToCoverageEnable = FALSE;
	font_blend_desc.IndependentBlendEnable = FALSE;
	font_blend_desc.RenderTarget[0].BlendEnable = TRUE;
	font_blend_desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
	font_blend_desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
	font_blend_desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_SRC_ALPHA;
	font_blend_desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_DEST_ALPHA;
	font_blend_desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;

	device->CreateBlendState(&font_blend_desc,&font_blendstate2);

	//Initialize Screen Drawing

	SCREEN_VERTEX screen_verts[4];
	screen_verts[0].position = XMFLOAT3(-1.0f,-1.0f,0.0f);
	screen_verts[1].position = XMFLOAT3(-1.0f,1.0f,0.0f);
	screen_verts[2].position = XMFLOAT3(1.0f,1.0f,0.0f);
	screen_verts[3].position = XMFLOAT3(1.0f,-1.0f,0.0f);
	screen_verts[0].tex_coords = XMFLOAT2(0.0f,1.0f);
	screen_verts[1].tex_coords = XMFLOAT2(0.0f,0.0f);
	screen_verts[2].tex_coords = XMFLOAT2(1.0f,0.0f);
	screen_verts[3].tex_coords = XMFLOAT2(1.0f,1.0f);

	USHORT screen_indicies[6];
	screen_indicies[0] = 0;
	screen_indicies[1] = 1;
	screen_indicies[2] = 2;
	screen_indicies[3] = 0;
	screen_indicies[4] = 2;
	screen_indicies[5] = 3;

	//describing and creating the screen vertex buffer
	vertex_buffer_desc.Usage = D3D11_USAGE_IMMUTABLE;
	vertex_buffer_desc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
	vertex_buffer_desc.CPUAccessFlags = 0;
	vertex_buffer_desc.ByteWidth = 4 * sizeof(SCREEN_VERTEX);

	D3D11_SUBRESOURCE_DATA dat;
	ZeroMemory(&dat,sizeof(dat));
	dat.pSysMem = screen_verts;

	res = device->CreateBuffer(&vertex_buffer_desc, &dat, &screen_vertex_buffer);

	//describing and creating the screen index buffer
	index_buffer_desc.Usage = D3D11_USAGE_IMMUTABLE;
	index_buffer_desc.BindFlags = D3D11_BIND_INDEX_BUFFER;
	index_buffer_desc.CPUAccessFlags = 0;
	index_buffer_desc.ByteWidth = 6 * sizeof(USHORT);

	ZeroMemory(&dat,sizeof(dat));
	dat.pSysMem = screen_indicies;

	res = device->CreateBuffer(&index_buffer_desc, &dat, &screen_index_buffer);

	//creating the screen shaders
	res = device->CreateVertexShader(screen_vert,sizeof(screen_vert),0,&screen_vertex_shader);
	res = device->CreatePixelShader(screen_frag,sizeof(screen_frag),0,&screen_pixel_shader);

	//linking vertex input layout in vertex shader
	UINT num_eles = sizeof(SCREEN_VERTEX_DESC)/sizeof(D3D11_INPUT_ELEMENT_DESC);
	res = device->CreateInputLayout(SCREEN_VERTEX_DESC,num_eles,screen_vert,sizeof(screen_vert),&screen_vertex_layout);

	//initializing screen ratio textures
	res = CreateDDSTextureFromFile(device,L"cockpit.dds",0,&hud_tex);
	res = CreateDDSTextureFromFile(device,L"matrix.dds",0,&screen_tex);
	sampler_desc.Filter = D3D11_FILTER_ANISOTROPIC;
	sampler_desc.MaxAnisotropy = 16;
	sampler_desc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
	sampler_desc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
	sampler_desc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
	res = device->CreateSamplerState(&sampler_desc,&screen_tex_settings);

	constant_buffer_desc.Usage = D3D11_USAGE_DYNAMIC;
	constant_buffer_desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	constant_buffer_desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
	constant_buffer_desc.ByteWidth = 16;//HAS TO BE ATLEAST 16
	res = device->CreateBuffer(&constant_buffer_desc,0,&screen_pixel_constant_buffer);

	/*Post Process Shaders*/

	//creating the pixelated shaders
	res = device->CreateVertexShader(pixelated_vert,sizeof(pixelated_vert),0,&post_pixelated_vertex_shader);
	res = device->CreatePixelShader(pixelated_frag,sizeof(pixelated_frag),0,&post_pixelated_pixel_shader);

	//linking vertex input layout in vertex shader
	num_eles = sizeof(SCREEN_VERTEX_DESC)/sizeof(D3D11_INPUT_ELEMENT_DESC);
	res = device->CreateInputLayout(SCREEN_VERTEX_DESC,num_eles,pixelated_vert,sizeof(pixelated_vert),&post_pixelated_vertex_layout);

	constant_buffer_desc.Usage = D3D11_USAGE_DYNAMIC;
	constant_buffer_desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	constant_buffer_desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
	constant_buffer_desc.ByteWidth = 16;//HAS TO BE ATLEAST 16
	res = device->CreateBuffer(&constant_buffer_desc,0,&post_pixelated_pixel_constant_buffer);

	kOffsetsandSides = XMFLOAT3(0.0f,0.0f,4.0f);

	D3D11_RASTERIZER_DESC raster_desc;
	ZeroMemory(&raster_desc,sizeof(raster_state));
	raster_desc.FillMode = D3D11_FILL_SOLID;
	raster_desc.CullMode = D3D11_CULL_BACK;
	raster_desc.DepthClipEnable = TRUE;
	raster_desc.MultisampleEnable = TRUE;
	device->CreateRasterizerState(&raster_desc,&raster_state);
	device_context->RSSetState(raster_state);

	//creating the screen shaders
	res = device->CreateVertexShader(hud_vert,sizeof(hud_vert),0,&hud_vertex_shader);
	res = device->CreatePixelShader(hud_frag,sizeof(hud_frag),0,&hud_pixel_shader);
	num_eles = sizeof(SCREEN_VERTEX_DESC)/sizeof(D3D11_INPUT_ELEMENT_DESC);
	res = device->CreateInputLayout(SCREEN_VERTEX_DESC,num_eles,hud_vert,sizeof(hud_vert),&hud_vertex_layout);

	//pokemon sprites
	res = CreateDDSTextureFromFile(device,L"pokemon_sprites.dds",0,&poke_sprite_tex);
	res = device->CreateVertexShader(sprite_sheet_vert,sizeof(sprite_sheet_vert),0,&sprite_vertex_shader);
	res = device->CreatePixelShader(sprite_sheet_frag,sizeof(sprite_sheet_frag),0,&sprite_pixel_shader);
	num_eles = sizeof(SCREEN_VERTEX_DESC)/sizeof(D3D11_INPUT_ELEMENT_DESC);
	res = device->CreateInputLayout(SCREEN_VERTEX_DESC,num_eles,sprite_sheet_vert,sizeof(sprite_sheet_vert),&sprite_vertex_layout);

	constant_buffer_desc.Usage = D3D11_USAGE_DYNAMIC;
	constant_buffer_desc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
	constant_buffer_desc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
	constant_buffer_desc.ByteWidth = 32;//HAS TO BE ATLEAST 16
	res = device->CreateBuffer(&constant_buffer_desc,0,&sprite_vertex_constant_buffer);

	sprite_vertex_constants.sprite_clip_ratio.x = 2.0f/viewport.Width * 48.0f;
	sprite_vertex_constants.sprite_clip_ratio.y = 2.0f/viewport.Height * 48.0f;
	sprite_vertex_constants.sprite_scale = 1.0f;
	sprite_vertex_constants.sprite_position = XMFLOAT2(0.0f,0.0f);
	sprite_vertex_constants.sprite_rotation = 1.5f;
	sprite_vertex_constants.sprite_index.x = 0;
	sprite_vertex_constants.sprite_index.y = 0;
}

void D3DInterface::Render(void)
{
	if(GetMainMenu().active)
		RenderMenu();
	if(GetChatLobby().active)
		RenderLobby();
}

/*Used in both Render Menu and Render Lobby*/
static float ac = 0;
static float ab = 0;
static float vel = 4.0f;

void D3DInterface::RenderMenu(void)
{
	float delta_time = GetAppTimer().Delta();
	float time = GetAppTimer().TotalTime();
	/*Temp kscope update code*/
	
	ab += 0.1f*delta_time;
	ac += 0.3f*delta_time;
	vel += 3.0f*delta_time*sin(time*0.2f);
	kOffsetsandSides.z = vel;
	kOffsetsandSides.x = sin(ac);
	kOffsetsandSides.y = cos(ab);
	/*end temp update kscope*/

	FLOAT cc[4] = {0.0f,0.0f,0.0f,1.0f};
	ID3D11ShaderResourceView* temp = 0;
	device_context->PSSetShaderResources(0,1,&temp);//GETS RID OF WARNING
	device_context->OMSetRenderTargets(1,&pixelated_render_target,main_depth_stencil_view);
	device_context->ClearRenderTargetView(pixelated_render_target,cc);
	device_context->ClearDepthStencilView(main_depth_stencil_view,D3D11_CLEAR_DEPTH,1.0f,0);

	//preparing to draw screen grid
	device_context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	UINT stride = sizeof(SCREEN_VERTEX);
	UINT offset = 0;
	device_context->IASetVertexBuffers(0,1,&screen_vertex_buffer,&stride,&offset);
	device_context->IASetIndexBuffer(screen_index_buffer,DXGI_FORMAT_R16_UINT,0);
	device_context->VSSetShader(screen_vertex_shader,0,0);
	device_context->PSSetShader(screen_pixel_shader,0,0);
	device_context->IASetInputLayout(screen_vertex_layout);
	device_context->PSSetSamplers(0,1,&screen_tex_settings);
	device_context->PSSetShaderResources(0,1,&screen_tex);
	D3D11_MAPPED_SUBRESOURCE resource;
	device_context->Map(screen_pixel_constant_buffer,0,D3D11_MAP_WRITE_DISCARD,0,&resource);
	*((XMFLOAT3*)resource.pData) = kOffsetsandSides;
	device_context->Unmap(screen_pixel_constant_buffer,0);

	device_context->PSSetConstantBuffers(0,1,&screen_pixel_constant_buffer);

	device_context->DrawIndexed(6,0,0);

	XMFLOAT4 font_color(1.0f,1.0f,1.0f,0.66f);
	XMFLOAT4 font_color2(0.0f,0.8f,1.0f,0.66f);
	XMFLOAT4 font_color3(1.0f,0.0f,1.0f,0.66f);
	XMFLOAT4 font_color4(0.8f,0.0f,0.0f,0.66f);
	XMFLOAT2 a(0.0f,0.0f);
	XMFLOAT2 A(4.0f*sin(5.0f*time),4.0f*cos(5.0f*time));
	XMFLOAT2 A1(4.0f*sin(5.0f*time),4.0f*cos(3.0f*time));
	XMFLOAT2 A2(4.0f*sin(3.0f*time),4.0f*cos(4.0f*time));
	XMFLOAT2 A3(4.0f*sin(-4.0f*time),4.0f*cos(4.0f*time));
	XMFLOAT2 A4(4.0f*sin(-5.0f*time),4.0f*cos(-5.0f*time));

	XMVECTOR b = XMLoadFloat2(&a);
	XMVECTOR B = XMLoadFloat2(&A);
	XMVECTOR c = XMLoadFloat4(&font_color);
	XMVECTOR C = XMLoadFloat4(&font_color2);
	
	sprite_batch->Begin(DirectX::SpriteSortMode_Deferred,font_blendstate);
	sprite_font->DrawString(sprite_batch,L" Enter The Kaleidoscope",b,XMLoadFloat4(&font_color3),0.0f,g_XMZero,0.75f);
	sprite_font->DrawString(sprite_batch,L" Enter The Kaleidoscope",B,C,0.0f,g_XMZero,0.75f);

	MainMenu& menu = GetMainMenu();
	RECT window_size;
	GetClientRect(window_handle,&window_size);
	XMFLOAT2 font_size;
	XMFLOAT2 font_origin;
	XMStoreFloat2(&font_size,sprite_font->MeasureString(L"HOST"));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;

	if(menu.hover_host)
	{
		sprite_font->DrawString(sprite_batch,L"HOST",XMLoadFloat2(&XMFLOAT2((menu.host_button_rect.right + menu.host_button_rect.left)/2.0f + A.x,(menu.host_button_rect.bottom+ menu.host_button_rect.top)/2.0f + A.y)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,L"HOST",XMLoadFloat2(&XMFLOAT2((menu.host_button_rect.right + menu.host_button_rect.left)/2.0f,(menu.host_button_rect.bottom+ menu.host_button_rect.top)/2.0f)),C,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}
	else
	{
		sprite_font->DrawString(sprite_batch,L"HOST",XMLoadFloat2(&XMFLOAT2((menu.host_button_rect.right + menu.host_button_rect.left)/2.0f + A.x,(menu.host_button_rect.bottom+ menu.host_button_rect.top)/2.0f + A.y)),XMLoadFloat4(&font_color4),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,L"HOST",XMLoadFloat2(&XMFLOAT2((menu.host_button_rect.right + menu.host_button_rect.left)/2.0f,(menu.host_button_rect.bottom+ menu.host_button_rect.top)/2.0f)),c,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}

	XMStoreFloat2(&font_size,sprite_font->MeasureString(L"JOIN"));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;

	if(menu.hover_join)
	{
		sprite_font->DrawString(sprite_batch,L"JOIN",XMLoadFloat2(&XMFLOAT2((menu.join_button_rect.right + menu.join_button_rect.left)/2.0f+A1.x,(menu.join_button_rect.bottom+ menu.join_button_rect.top)/2.0f+A1.y)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,L"JOIN",XMLoadFloat2(&XMFLOAT2((menu.join_button_rect.right + menu.join_button_rect.left)/2.0f,(menu.join_button_rect.bottom+ menu.join_button_rect.top)/2.0f)),C,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}
	else
	{
		sprite_font->DrawString(sprite_batch,L"JOIN",XMLoadFloat2(&XMFLOAT2((menu.join_button_rect.right + menu.join_button_rect.left)/2.0f+A1.x,(menu.join_button_rect.bottom+ menu.join_button_rect.top)/2.0f+A1.y)),XMLoadFloat4(&font_color4),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,L"JOIN",XMLoadFloat2(&XMFLOAT2((menu.join_button_rect.right + menu.join_button_rect.left)/2.0f,(menu.join_button_rect.bottom+ menu.join_button_rect.top)/2.0f)),c,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}

	/*DRAWING USER NAME*/
	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.user_name.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.user_name_rect.top = window_size.bottom/2.0f - font_origin.y - sprite_font->GetLineSpacing();
	menu.user_name_rect.left = window_size.right/2.0f - font_origin.x;
	menu.user_name_rect.bottom = window_size.bottom/2.0f + font_origin.y - sprite_font->GetLineSpacing();
	menu.user_name_rect.right = window_size.right/2.0f + font_origin.x;
	if(menu.get_user_name)
	{
		static XTime name_flicker;
		name_flicker.Signal();
		if(name_flicker.TotalTime() < 0.3)
		{
			menu.user_name.push_back('_');
			XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.user_name.c_str()));
			font_origin.x = font_size.x / 2.0f;
			font_origin.y = font_size.y / 2.0f;
			sprite_font->DrawString(sprite_batch,menu.user_name.c_str(),XMLoadFloat2(&XMFLOAT2((menu.user_name_rect.right + menu.user_name_rect.left)/2.0f,(menu.user_name_rect.bottom+ menu.user_name_rect.top)/2.0f)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
			menu.user_name.pop_back();
		}
		else if(name_flicker.TotalTime() > 0.6)
			name_flicker.Restart();

	}
	else if(menu.hover_name)
	{
		sprite_font->DrawString(sprite_batch,menu.user_name.c_str(),XMLoadFloat2(&XMFLOAT2((menu.user_name_rect.right + menu.user_name_rect.left)/2.0f+A2.x,(menu.user_name_rect.bottom+ menu.user_name_rect.top)/2.0f+A2.y)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.user_name.c_str(),XMLoadFloat2(&XMFLOAT2((menu.user_name_rect.right + menu.user_name_rect.left)/2.0f,(menu.user_name_rect.bottom+ menu.user_name_rect.top)/2.0f)),C,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}
	else
	{
		sprite_font->DrawString(sprite_batch,menu.user_name.c_str(),XMLoadFloat2(&XMFLOAT2((menu.user_name_rect.right + menu.user_name_rect.left)/2.0f+A2.x,(menu.user_name_rect.bottom+ menu.user_name_rect.top)/2.0f+A2.y)),XMLoadFloat4(&font_color4),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.user_name.c_str(),XMLoadFloat2(&XMFLOAT2((menu.user_name_rect.right + menu.user_name_rect.left)/2.0f,(menu.user_name_rect.bottom+ menu.user_name_rect.top)/2.0f)),c,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}


	/*DRAWING IP ADDRESS*/
	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.ipaddress.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.ip_rect.top = window_size.bottom/2.0f - font_origin.y - 0.0f;
	menu.ip_rect.left = window_size.right/2.0f - font_origin.x;
	menu.ip_rect.bottom = window_size.bottom/2.0f + font_origin.y - 0.0f;
	menu.ip_rect.right = window_size.right/2.0f + font_origin.x;
	if(menu.get_ip_string)
	{
		static XTime ip_flicker;
		ip_flicker.Signal();
		if(ip_flicker.TotalTime() < 0.3)
		{
			menu.ipaddress.push_back('_');
			XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.ipaddress.c_str()));
			font_origin.x = font_size.x / 2.0f;
			font_origin.y = font_size.y / 2.0f;
			sprite_font->DrawString(sprite_batch,menu.ipaddress.c_str(),XMLoadFloat2(&XMFLOAT2((menu.ip_rect.right + menu.ip_rect.left)/2.0f,(menu.ip_rect.bottom+ menu.ip_rect.top)/2.0f)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
			menu.ipaddress.pop_back();
		}
		else if(ip_flicker.TotalTime() > 0.6)
			ip_flicker.Restart();

	}
	else if(menu.hover_ip)
	{
		sprite_font->DrawString(sprite_batch,menu.ipaddress.c_str(),XMLoadFloat2(&XMFLOAT2((menu.ip_rect.right + menu.ip_rect.left)/2.0f+A3.x,(menu.ip_rect.bottom+ menu.ip_rect.top)/2.0f+A3.y)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.ipaddress.c_str(),XMLoadFloat2(&XMFLOAT2((menu.ip_rect.right + menu.ip_rect.left)/2.0f,(menu.ip_rect.bottom+ menu.ip_rect.top)/2.0f)),C,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}
	else
	{
		sprite_font->DrawString(sprite_batch,menu.ipaddress.c_str(),XMLoadFloat2(&XMFLOAT2((menu.ip_rect.right + menu.ip_rect.left)/2.0f+A3.x,(menu.ip_rect.bottom+ menu.ip_rect.top)/2.0f+A3.y)),XMLoadFloat4(&font_color4),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.ipaddress.c_str(),XMLoadFloat2(&XMFLOAT2((menu.ip_rect.right + menu.ip_rect.left)/2.0f,(menu.ip_rect.bottom+ menu.ip_rect.top)/2.0f)),c,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}

	/*DRAWING PORT*/
	XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.port.c_str()));
	font_origin.x = font_size.x / 2.0f;
	font_origin.y = font_size.y / 2.0f;
	menu.port_rect.top = window_size.bottom/2.0f - font_origin.y + sprite_font->GetLineSpacing();
	menu.port_rect.left = window_size.right/2.0f - font_origin.x;
	menu.port_rect.bottom = window_size.bottom/2.0f + font_origin.y + sprite_font->GetLineSpacing();
	menu.port_rect.right = window_size.right/2.0f + font_origin.x;
	if(menu.get_port)
	{
		static XTime port_flicker;
		port_flicker.Signal();
		if(port_flicker.TotalTime() < 0.3)
		{
			menu.port.push_back('_');
			XMStoreFloat2(&font_size,sprite_font->MeasureString(menu.port.c_str()));
			font_origin.x = font_size.x / 2.0f;
			font_origin.y = font_size.y / 2.0f;
			sprite_font->DrawString(sprite_batch,menu.port.c_str(),XMLoadFloat2(&XMFLOAT2((menu.port_rect.right + menu.port_rect.left)/2.0f,(menu.port_rect.bottom+ menu.port_rect.top)/2.0f)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
			menu.port.pop_back();
		}
		else if(port_flicker.TotalTime() > 0.6)
			port_flicker.Restart();

	}
	else if(menu.hover_port)
	{
		sprite_font->DrawString(sprite_batch,menu.port.c_str(),XMLoadFloat2(&XMFLOAT2((menu.port_rect.right + menu.port_rect.left)/2.0f + A4.x,(menu.port_rect.bottom+ menu.port_rect.top)/2.0f+ A4.y)),XMLoadFloat4(&font_color3),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.port.c_str(),XMLoadFloat2(&XMFLOAT2((menu.port_rect.right + menu.port_rect.left)/2.0f,(menu.port_rect.bottom+ menu.port_rect.top)/2.0f)),C,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}
	else
	{
		sprite_font->DrawString(sprite_batch,menu.port.c_str(),XMLoadFloat2(&XMFLOAT2((menu.port_rect.right + menu.port_rect.left)/2.0f + A4.x,(menu.port_rect.bottom+ menu.port_rect.top)/2.0f+ A4.y)),XMLoadFloat4(&font_color4),0.0f,XMLoadFloat2(&font_origin),1.0f);
		sprite_font->DrawString(sprite_batch,menu.port.c_str(),XMLoadFloat2(&XMFLOAT2((menu.port_rect.right + menu.port_rect.left)/2.0f,(menu.port_rect.bottom+ menu.port_rect.top)/2.0f)),c,0.0f,XMLoadFloat2(&font_origin),1.0f);
	}

	sprite_batch->End();
	bool is_trans = isTransitioning();
	XTime& trans = GetTransitionTimer();
	float tot = trans.TotalTime();
	if(is_trans && tot < 3.0f)
	{
		kOffsetsandSides.x = 25.0f*(tot+0.1f);
		kOffsetsandSides.y = 25.0f*(tot+0.1f);
	}
	else if(is_trans && tot < 6.0f)
	{
		kOffsetsandSides.x = 25.0f*(6.0f - tot + 0.1f);
		kOffsetsandSides.y = 25.0f*(6.0f - tot + 0.1f);
	}
	else
	{
		kOffsetsandSides.x = 1.0f;
		kOffsetsandSides.y = 1.0f;
	}

	/*Post Process*/
	device_context->OMSetRenderTargets(1,&main_render_target_view,main_depth_stencil_view);
	device_context->ClearRenderTargetView(main_render_target_view,cc);
	//preparing to draw screen grid
	device_context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	stride = sizeof(SCREEN_VERTEX);
	offset = 0;
	device_context->IASetVertexBuffers(0,1,&screen_vertex_buffer,&stride,&offset);
	device_context->IASetIndexBuffer(screen_index_buffer,DXGI_FORMAT_R16_UINT,0);
	device_context->VSSetShader(post_pixelated_vertex_shader,0,0);
	device_context->PSSetShader(post_pixelated_pixel_shader,0,0);
	device_context->IASetInputLayout(screen_vertex_layout);
	device_context->PSSetSamplers(0,1,&screen_tex_settings);
	device_context->PSSetShaderResources(0,1,&pixelated_render_target_tex);
	//D3D11_MAPPED_SUBRESOURCE resource;
	device_context->Map(screen_pixel_constant_buffer,0,D3D11_MAP_WRITE_DISCARD,0,&resource);
	(*((XMFLOAT4*)resource.pData)).x = kOffsetsandSides.x;
	(*((XMFLOAT4*)resource.pData)).y = kOffsetsandSides.y;
	(*((XMFLOAT4*)resource.pData)).z = viewport.Width;
	(*((XMFLOAT4*)resource.pData)).w = viewport.Height;
	device_context->Unmap(screen_pixel_constant_buffer,0);

	device_context->PSSetConstantBuffers(0,1,&screen_pixel_constant_buffer);
	device_context->DrawIndexed(6,0,0);

	swap_chain->Present(DXGI_SWAP_EFFECT::DXGI_SWAP_EFFECT_SEQUENTIAL,0);
}
void D3DInterface::RenderLobby(void)
{
	float delta_time = GetAppTimer().Delta();
	float time = GetAppTimer().TotalTime();
	/*Temp kscope update code*/
	
	ab += 0.1f*delta_time;
	ac += 0.3f*delta_time;
	vel += 3.0f*delta_time*sin(time*0.2f);
	kOffsetsandSides.z = vel;
	kOffsetsandSides.x = sin(ac);
	kOffsetsandSides.y = cos(ab);
	/*end temp update kscope*/

	FLOAT cc[4] = {0.0f,0.0f,0.0f,1.0f};
	ID3D11ShaderResourceView* temp = 0;
	device_context->PSSetShaderResources(0,1,&temp);//GETS RID OF WARNING
	device_context->OMSetRenderTargets(1,&pixelated_render_target,main_depth_stencil_view);
	device_context->ClearRenderTargetView(pixelated_render_target,cc);
	device_context->ClearDepthStencilView(main_depth_stencil_view,D3D11_CLEAR_DEPTH,1.0f,0);

	//preparing to draw screen grid
	device_context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	UINT stride = sizeof(SCREEN_VERTEX);
	UINT offset = 0;
	device_context->IASetVertexBuffers(0,1,&screen_vertex_buffer,&stride,&offset);
	device_context->IASetIndexBuffer(screen_index_buffer,DXGI_FORMAT_R16_UINT,0);
	device_context->VSSetShader(screen_vertex_shader,0,0);
	device_context->PSSetShader(screen_pixel_shader,0,0);
	device_context->IASetInputLayout(screen_vertex_layout);
	device_context->PSSetSamplers(0,1,&screen_tex_settings);
	device_context->PSSetShaderResources(0,1,&screen_tex);
	D3D11_MAPPED_SUBRESOURCE resource;
	device_context->Map(screen_pixel_constant_buffer,0,D3D11_MAP_WRITE_DISCARD,0,&resource);
	*((XMFLOAT3*)resource.pData) = kOffsetsandSides;
	device_context->Unmap(screen_pixel_constant_buffer,0);

	device_context->PSSetConstantBuffers(0,1,&screen_pixel_constant_buffer);

	device_context->DrawIndexed(6,0,0);

	static XTime sprite_timer;
	sprite_timer.Signal();
	float total_sprite_time = sprite_timer.TotalTime();
	if(total_sprite_time < 2.0f)
	{
		sprite_vertex_constants.sprite_scale = total_sprite_time*5.0f;
		sprite_vertex_constants.sprite_position.x = 0.3*cos(3.0f*time);
		sprite_vertex_constants.sprite_position.y = 0.3*sin(3.0f*time);
		sprite_vertex_constants.sprite_rotation = 3.0f*total_sprite_time;
	}
	else if(total_sprite_time < 2.5f)
	{

	}
	else if(total_sprite_time < 3.5f)
	{
		sprite_vertex_constants.sprite_position.x += 0.3*total_sprite_time*sprite_vertex_constants.sprite_position.x * delta_time;
		sprite_vertex_constants.sprite_position.y += 0.3*total_sprite_time*sprite_vertex_constants.sprite_position.y * delta_time;
		sprite_vertex_constants.sprite_scale+=3.0*delta_time;
	}
	else if(total_sprite_time < 4.5f)
	{
		sprite_vertex_constants.sprite_position.x += (total_sprite_time+4.0)*sprite_vertex_constants.sprite_position.x * delta_time;
		sprite_vertex_constants.sprite_position.y += (total_sprite_time+4.0)*sprite_vertex_constants.sprite_position.y * delta_time;
		sprite_vertex_constants.sprite_scale+=50.0*delta_time;
	}
	else
	{
		int rando = rand() % 21;
		if(rando < 20)
		{
			sprite_vertex_constants.sprite_index.x = rand() % 31;
		}
		else
			sprite_vertex_constants.sprite_index.x = rand() % 29;

		sprite_vertex_constants.sprite_index.y = rando;

		sprite_timer.Restart();
	}

	device_context->PSSetShaderResources(0,1,&poke_sprite_tex);
	device_context->VSSetShader(sprite_vertex_shader,0,0);
	device_context->PSSetShader(sprite_pixel_shader,0,0);
	device_context->IASetInputLayout(sprite_vertex_layout);
	device_context->Map(sprite_vertex_constant_buffer,0,D3D11_MAP_WRITE_DISCARD,0,&resource);
	(*((SPRITE_VERT_CONSTANTS*)resource.pData)) = sprite_vertex_constants;
	device_context->Unmap(sprite_vertex_constant_buffer,0);

	device_context->VSSetConstantBuffers(0,1,&sprite_vertex_constant_buffer);

	device_context->DrawIndexed(6,0,0);

	device_context->PSSetShaderResources(0,1,&hud_tex);
	device_context->VSSetShader(hud_vertex_shader,0,0);
	device_context->PSSetShader(hud_pixel_shader,0,0);
	device_context->IASetInputLayout(hud_vertex_layout);
	device_context->DrawIndexed(6,0,0);

	
	XMFLOAT4 font_color(1.0f,1.0f,1.0f,0.66f);
	XMFLOAT4 font_color2(0.0f,0.8f,1.0f,0.66f);
	XMFLOAT4 font_color3(1.0f,0.0f,1.0f,0.66f);
	XMFLOAT4 font_color4(0.8f,0.0f,0.0f,0.66f);
	XMFLOAT2 a(0.0f,0.0f);
	XMFLOAT2 A(4.0f*sin(5.0f*time),4.0f*cos(5.0f*time));
	XMFLOAT2 A1(4.0f*sin(5.0f*time),4.0f*cos(3.0f*time));
	XMFLOAT2 A2(4.0f*sin(3.0f*time),4.0f*cos(4.0f*time));
	XMFLOAT2 A3(4.0f*sin(-4.0f*time),4.0f*cos(4.0f*time));
	XMFLOAT2 A4(4.0f*sin(-5.0f*time),4.0f*cos(-5.0f*time));

	XMVECTOR b = XMLoadFloat2(&a);
	XMVECTOR B = XMLoadFloat2(&A);
	XMVECTOR c = XMLoadFloat4(&font_color);
	XMVECTOR C = XMLoadFloat4(&font_color2);
	ChatLobby& lobby = GetChatLobby();
	
	sprite_batch->Begin(DirectX::SpriteSortMode_Deferred,font_blendstate);

	XMFLOAT2 text_position = XMFLOAT2(450.0f,510.0f);
	XMFLOAT4 text_color(0.0f,1.0f,0.0f,1.0f);

	XMFLOAT4 text_color2(0.0f,1.0f,0.0f,0.77f);

	
	RECT window_size;
	GetClientRect(window_handle,&window_size);
	XMFLOAT2 font_size;
	XMFLOAT2 font_origin;

	float space = consolas_11_font->GetLineSpacing()*0.0875;
	space*= 0.725f;

	XMFLOAT4 users_col;

	if(lobby.chat_window_active)
		users_col = text_color;
	else
		users_col = font_color2;

	float placement = space;
	unsigned int i = 0;
	lobby.chat_message_queue_mutex.lock();
	if(lobby.chat_message_queue.size())
	{
		for(std::deque<std::wstring>::iterator iter = lobby.chat_message_queue.begin()+lobby.chat_window_offset; iter != lobby.chat_message_queue.end() && i < 10; iter++, i++)
		{
			consolas_11_font->DrawString(sprite_batch,iter->c_str(),XMLoadFloat2(&XMFLOAT2(lobby.message_rect.left,lobby.message_rect.bottom - placement*(float)(i+1))),XMLoadFloat4(&users_col),0.0f,g_XMZero,0.0875f);
		}
	}
	lobby.chat_message_queue_mutex.unlock();
	if(lobby.user_list_selected)
		users_col = text_color;
	else
		users_col = font_color2;

	placement = 0.0f;

	lobby.user_name_mutex.lock();
	for(std::map<unsigned int, std::wstring>::iterator it = lobby.users.begin(); it != lobby.users.end(); it++)
	{
		XMStoreFloat2(&font_size,consolas_11_font->MeasureString(it->second.c_str()));
		font_origin.x = font_size.x / 2.0f;
		font_origin.y = font_size.y / 2.0f;
		consolas_11_font->DrawString(sprite_batch,it->second.c_str(),XMLoadFloat2(&XMFLOAT2((lobby.user_list_rect.right+lobby.user_list_rect.left)/2.0f,lobby.user_list_rect.top+space/2.0f + placement)),XMLoadFloat4(&users_col),0.0f,XMLoadFloat2(&font_origin),0.0875f);
		placement += space; 
	}
	lobby.user_name_mutex.unlock();

	if(lobby.is_typing_chat)
	{
		lobby.display_chat_message.push_back('>');
		sprite_font->DrawString(sprite_batch,lobby.display_chat_message.c_str(),XMLoadFloat2(&XMFLOAT2(lobby.typing_rect.left,lobby.typing_rect.top)),XMLoadFloat4(&text_color),0.0f,g_XMZero,0.1111f);
		lobby.display_chat_message.pop_back();
	}
	else
		sprite_font->DrawString(sprite_batch,lobby.display_chat_message.c_str(),XMLoadFloat2(&XMFLOAT2(lobby.typing_rect.left,lobby.typing_rect.top)),XMLoadFloat4(&font_color2),0.0f,g_XMZero,0.1111f);

	static float acum_time = 0;
	if(lobby.hover_leave)
		acum_time += delta_time;
		XMFLOAT2 off(4.0f*sin(5.0f*acum_time),4.0f*cos(5.0f*acum_time));
	if(lobby.hover_leave)
	{
		sprite_font->DrawString(sprite_batch,L"LEAVE?",XMLoadFloat2(&XMFLOAT2(lobby.leave_rect.left+off.y,lobby.leave_rect.top)),XMLoadFloat4(&font_color2),0.0f,g_XMZero,0.2f);
		sprite_font->DrawString(sprite_batch,L"LEAVE?",XMLoadFloat2(&XMFLOAT2(lobby.leave_rect.left+off.x,lobby.leave_rect.top)),XMLoadFloat4(&text_color2),0.0f,g_XMZero,0.2f);
	}

	sprite_batch->End();

	XTime& trans = GetTransitionTimer();
	bool is_trans = isTransitioning();
	float tot = trans.TotalTime();
	if(is_trans && tot < 3.0f)
	{
		kOffsetsandSides.x = 25.0f*(tot+0.1f);
		kOffsetsandSides.y = 25.0f*(tot+0.1f);
	}
	else if(is_trans && tot < 6.0f)
	{
		kOffsetsandSides.x = 25.0f*(6.0f - tot + 0.1f);
		kOffsetsandSides.y = 25.0f*(6.0f - tot + 0.1f);
	}
	else
	{
		kOffsetsandSides.x = 1.0f;
		kOffsetsandSides.y = 1.0f;
	}

	/*Post Process*/
	device_context->OMSetRenderTargets(1,&main_render_target_view,main_depth_stencil_view);
	device_context->ClearRenderTargetView(main_render_target_view,cc);
	//preparing to draw screen grid
	device_context->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
	stride = sizeof(SCREEN_VERTEX);
	offset = 0;
	device_context->IASetVertexBuffers(0,1,&screen_vertex_buffer,&stride,&offset);
	device_context->IASetIndexBuffer(screen_index_buffer,DXGI_FORMAT_R16_UINT,0);
	device_context->VSSetShader(post_pixelated_vertex_shader,0,0);
	device_context->PSSetShader(post_pixelated_pixel_shader,0,0);
	device_context->IASetInputLayout(screen_vertex_layout);
	device_context->PSSetSamplers(0,1,&screen_tex_settings);
	device_context->PSSetShaderResources(0,1,&pixelated_render_target_tex);
	//D3D11_MAPPED_SUBRESOURCE resource;
	device_context->Map(screen_pixel_constant_buffer,0,D3D11_MAP_WRITE_DISCARD,0,&resource);
	(*((XMFLOAT4*)resource.pData)).x = kOffsetsandSides.x;
	(*((XMFLOAT4*)resource.pData)).y = kOffsetsandSides.y;
	(*((XMFLOAT4*)resource.pData)).z = viewport.Width;
	(*((XMFLOAT4*)resource.pData)).w = viewport.Height;
	device_context->Unmap(screen_pixel_constant_buffer,0);

	device_context->PSSetConstantBuffers(0,1,&screen_pixel_constant_buffer);
	device_context->DrawIndexed(6,0,0);

	swap_chain->Present(DXGI_SWAP_EFFECT::DXGI_SWAP_EFFECT_SEQUENTIAL,0);
}
void D3DInterface::RenderGame(void)
{
}

void D3DInterface::Shutdown(void)
{
	

	if(sprite_font)
		delete sprite_font;

	if(consolas_11_font)
		delete consolas_11_font;
	
	if(sprite_batch)
		delete sprite_batch;

	font_blendstate->Release();
	font_blendstate2->Release();

	//Screen Quad
	screen_vertex_buffer->Release();
	screen_index_buffer->Release();
	screen_pixel_constant_buffer->Release();
	screen_vertex_shader->Release();
	screen_pixel_shader->Release();
	screen_vertex_layout->Release();
	screen_tex->Release();
	screen_tex_settings->Release();

	//HUD stuff
	hud_tex->Release();
	hud_vertex_shader->Release();
	hud_pixel_shader->Release();
	hud_vertex_layout->Release();

	//pokemon studd
	poke_sprite_tex->Release();
	sprite_vertex_shader->Release();
	sprite_pixel_shader->Release();
	sprite_vertex_layout->Release();
	sprite_vertex_constant_buffer->Release();

	//Post Process
	post_pixelated_pixel_constant_buffer->Release();
	post_pixelated_vertex_shader->Release();
	post_pixelated_pixel_shader->Release();
	post_pixelated_vertex_layout->Release();

	//alternate render target;
	pixelated_texture->Release();
	pixelated_render_target_tex->Release();
	pixelated_render_target->Release();

	device->Release();
	swap_chain->Release();
	main_render_target_view->Release();
	device_context->Release();
	back_buffer->Release();
	depth_buffer->Release();
	main_depth_stencil_view->Release();
	raster_state->Release();
}
