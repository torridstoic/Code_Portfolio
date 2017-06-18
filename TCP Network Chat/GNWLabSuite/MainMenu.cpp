//Main Menu, Game Networking Lab Suite
//Tim Turcich
//Game Networking
//Full Sail University
//August-September 2013
#include "MainMenu.h"
#include <fstream>
using namespace std;
#include "XTime.h"
#include "ChatLobby.h"
#include "NetThreads.h"
#pragma comment(lib, "Winmm.lib")

XTime& GetTransitionTimer(void);
ChatLobby& GetChatLobby(void);

MainMenu::MainMenu(void)
{
	
}

void MainMenu::Initialize(HWND hWnd)
{
	active = true;
	window = hWnd;
	hover_host = false;
	hover_join = false;
	hover_ip = false;
	hover_port = false;
	hover_name = false;
	get_ip_string = false;
	get_port = false;
	get_user_name = false;
	ipaddress = L"127.0.0.1";
	port = L"9001";

	wifstream in;
	in.open(L"Pokemon.txt");
	unsigned int r = (rand() % 649) + 1;
	while(in.good() && r)
	{
		in >> user_name;
		r--;
	}
	in.close();
  
}

MainMenu::~MainMenu(void)
{
}

bool& isTransitioning(void);
bool& hasTransitioned(void);

void MainMenu::CheckInteraction(void)
{
	XTime& trans = GetTransitionTimer();
	ChatLobby& lobby = GetChatLobby();
	if(isTransitioning() && !hasTransitioned())
	{
		if(trans.TotalTime() > 3.0f )
		{
			hasTransitioned() = true;
			lobby.active = true;//turn on lobby
			active = false;//turn off menu
			PlaySound(L"pipe.wav", NULL, SND_ASYNC);
		}
		return;
	}

	hover_host = false;
	hover_join = false;
	hover_ip = false;
	hover_port = false;
	hover_name = false;

	RECT window_rect;
	RECT client_rect;
	GetWindowRect(window,&window_rect);
	GetClientRect(window,&client_rect);
	
	GetCursorPos(&mouse_position);
	POINT mouse_to_client = mouse_position;
	ScreenToClient(window,&mouse_to_client);

	if(PtInRect(&join_button_rect,mouse_to_client))
	{
		hover_join = true;
	}

	if(PtInRect(&host_button_rect,mouse_to_client))
	{
		hover_host = true;
	}

	if(PtInRect(&ip_rect,mouse_to_client))
	{
		hover_ip = true;
	}

	if(PtInRect(&port_rect,mouse_to_client))
	{
		hover_port = true;
	}

	if(PtInRect(&user_name_rect,mouse_to_client))
	{
		hover_name = true;
	}
	
	//check for button collisions
	static bool left_buffered = false;
	SHORT left_pressed = (SHORT)GetAsyncKeyState(VK_LBUTTON);
	if(left_pressed && !left_buffered && PtInRect(&client_rect,mouse_to_client))
	{
		left_buffered = true;

		if(PtInRect(&join_button_rect,mouse_to_client))
		{
			ChatLobby& lobby = GetChatLobby();

			GetTransitionTimer().Restart();
			lobby.is_hosting = false;//Mark as not Host
			isTransitioning() = true;
			hasTransitioned() = false;
			//Start Client Thread
			lobby.chat_client = new TCPChatClient(lobby);
			lobby.client_running = true;
			lobby.client_thread = std::async(std::launch::async, run_chat_client, lobby.chat_client);
		}

		if(PtInRect(&host_button_rect,mouse_to_client))
		{
			ChatLobby& lobby = GetChatLobby();

			GetTransitionTimer().Restart();
			lobby.is_hosting = true;//Mark as Host
			isTransitioning() = true;
			hasTransitioned() = false;
			//Start Both Threads
			lobby.chat_client = new TCPChatClient(lobby);
			lobby.chat_server = new TCPChatServer(lobby);
			lobby.server_is_finished = false;
			lobby.client_is_waiting = true;
			lobby.client_running = true;
			lobby.server_running = true;
			lobby.client_thread = std::async(std::launch::async, run_chat_client, lobby.chat_client);
			lobby.server_thread = std::async(std::launch::async, run_chat_server, lobby.chat_server);
		}

		get_ip_string = false;
		get_port = false;
		get_user_name = false;

		if(!ipaddress.size())
				get_ip_string = true;

		if(!port.size())
				get_port = true;

		if(!user_name.size())
				get_user_name = true;

		if(PtInRect(&ip_rect,mouse_to_client) && !get_port && !get_user_name)
		{
			get_ip_string = true;
		}

		if(PtInRect(&port_rect,mouse_to_client) && !get_ip_string && !get_user_name)
		{
			get_port = true;
		}

		if(PtInRect(&user_name_rect,mouse_to_client) && !get_ip_string && !get_port)
		{
			get_user_name = true;
		}
	}
	else if(!left_pressed)
		left_buffered = false;

	if(GetAsyncKeyState(VK_RETURN))
	{
		if(user_name.size() && get_user_name)
			get_user_name = false;
		else if(ipaddress.size() && get_ip_string)
			get_ip_string = false;
		else if(port.size() && get_port)
			get_port = false;
	}

	static bool tab_buffered = false;
	SHORT tab_pressed = GetAsyncKeyState(VK_TAB);
	if(tab_pressed && !tab_buffered)
	{
		tab_buffered = true;

		if(!get_ip_string && !get_port && !get_user_name)
		{
			get_user_name = true;
		}
		else if(user_name.size() && get_user_name)
		{
			get_user_name = false;
			get_ip_string = true;
		}
		else if(ipaddress.size() && get_ip_string)
		{
			get_ip_string = false;
			get_port = true;
		}
		else if(port.size() && get_port)
		{
			get_port = false;
			//get_ip_string = true;
		}
	}
	else if(!tab_pressed)
		tab_buffered = false;
}
