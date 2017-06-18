#pragma once
#define _WINSOCKAPI_ 
#include <Windows.h>
#include <math.h>
#include <DirectXMath.h>
#include <string>

class MainMenu
{
public:
	
	std::wstring user_name;
	std::wstring ipaddress;
	std::wstring port;

	bool get_user_name;
	bool get_ip_string;
	bool get_port;
	
	RECT user_name_rect;
	RECT ip_rect;
	RECT port_rect;
	RECT host_button_rect;
	RECT join_button_rect;

	POINT mouse_position;
	POINT mouse_previous;

	BOOL hover_name;
	BOOL hover_host;
	BOOL hover_join;
	BOOL hover_ip;
	BOOL hover_port;

	bool active;
	
	HWND window;

	MainMenu(void);
	~MainMenu(void);
	void Initialize(HWND hWnd);
	void CheckInteraction(void);
};

