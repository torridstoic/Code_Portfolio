//Win Main, Game Networking Lab Suite
//Tim Turcich
//Game Networking
//Full Sail University
//August-September 2013
#define _WINSOCKAPI_ 
#include <WinSock2.h>
#include <Windows.h>
#include "D3DInterface.h"
#include "MainMenu.h"
#include "ChatLobby.h"
#include "XTime.h"
#include <queue>
#include "TCPChatClient.h"
#include "TCPChatServer.h"
#include "resource.h"

#pragma comment(lib, "ws2_32.lib")

class WIN_APP
{
public:
	unsigned int	back_buffer_width;
	unsigned int	back_buffer_height;
	HINSTANCE		application;
	WNDPROC			appWndProc;
	HWND			window;

	//Game Related
	static D3DInterface d3dint;
	static MainMenu menu;
	static ChatLobby lobby;
	static XTime app_timer;
	static XTime transition_timer;
	static bool is_transitioning;
	static bool has_transitioned;

	friend int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPTSTR lpCmdLine,	int nCmdShow );

private:
	WIN_APP(HINSTANCE hinst, WNDPROC proc);
	WIN_APP(const WIN_APP& ref){}
	~WIN_APP(void){}
	bool Run();
	bool Close();
};

D3DInterface WIN_APP::d3dint = D3DInterface();
XTime WIN_APP::app_timer = XTime();
XTime WIN_APP::transition_timer = XTime();
MainMenu WIN_APP::menu = MainMenu();
ChatLobby WIN_APP::lobby = ChatLobby();
bool WIN_APP::is_transitioning = false;
bool WIN_APP::has_transitioned = false;

XTime& GetAppTimer(void)
{
	return WIN_APP::app_timer;
}

XTime& GetTransitionTimer(void)
{
	return WIN_APP::transition_timer;
}

bool& isTransitioning(void)
{
	return WIN_APP::is_transitioning;
}

bool& hasTransitioned(void)
{
	return WIN_APP::has_transitioned;
}

MainMenu& GetMainMenu(void)
{
	return WIN_APP::menu;
}

ChatLobby& GetChatLobby(void)
{
	return WIN_APP::lobby;
}

D3DInterface& GetD3DInt(void)
{
	return WIN_APP::d3dint;
}

WIN_APP::WIN_APP(HINSTANCE hinst, WNDPROC proc)
{
	back_buffer_width = 1280;
	back_buffer_height = 720;

	application = hinst; 
	appWndProc = proc; 

	WNDCLASSEX  wndClass;
	ZeroMemory( &wndClass, sizeof( wndClass ) );
	wndClass.cbSize         = sizeof( WNDCLASSEX );             
	wndClass.lpfnWndProc    = appWndProc;
	wndClass.lpszClassName  = L"Game Networking";
	wndClass.hInstance      = application;
	wndClass.hCursor        = LoadCursor( NULL, IDC_ARROW );
	wndClass.hbrBackground  = ( HBRUSH )( COLOR_WINDOWFRAME );
	wndClass.hIconSm		= LoadIcon(application, MAKEINTRESOURCE(IDI_ICON1));
	wndClass.hIcon			= LoadIcon(application, MAKEINTRESOURCE(IDI_ICON1));
	RegisterClassEx( &wndClass );

	RECT window_size = { 0, 0, back_buffer_width, back_buffer_height };
	AdjustWindowRect(&window_size, WS_OVERLAPPEDWINDOW & ~(WS_THICKFRAME|WS_MAXIMIZEBOX), false);

	window = CreateWindow(	L"Game Networking", L"GNW Lab Suite",WS_OVERLAPPEDWINDOW & ~(WS_THICKFRAME|WS_MAXIMIZEBOX), 
		CW_USEDEFAULT, CW_USEDEFAULT, window_size.right-window_size.left, window_size.bottom-window_size.top,					
		NULL, NULL,	application, this );												

	ShowWindow( window, SW_SHOW );

	srand(GetTickCount());
	app_timer.Restart();
	transition_timer.Restart();
	menu.Initialize(window);
	lobby.Initialize(window);
	d3dint.Initialize(window);
	is_transitioning = false;
	menu.active = true;
	lobby.active = false;
	WSADATA data;
	int ret = WSAStartup(WINSOCK_VERSION, &data);
	int test = 0;
}

bool WIN_APP::Run()
{
	app_timer.Signal();
	transition_timer.Signal();

	if(menu.active)
		menu.CheckInteraction();

	if(lobby.active)
		lobby.CheckInteraction();

	d3dint.Render();
	return true;
}
bool WIN_APP::Close()
{
	if(lobby.server_running)
	{
		lobby.server_running = false;
		lobby.client_running = false;
		lobby.chat_server->stop();
		lobby.chat_client->stop();
		lobby.server_thread.get();
		lobby.client_thread.get();
		delete lobby.chat_server;
		delete lobby.chat_client;
	}
	else if(lobby.client_running)
	{
		lobby.client_running = false;
		lobby.chat_client->stop();
		lobby.client_thread.get();
		delete lobby.chat_client;
	}

	int ret = WSACleanup();

	d3dint.Shutdown();
	return true;
}

LRESULT CALLBACK WndProc( HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam )
{
	
	MainMenu& menu = GetMainMenu();
	ChatLobby& lobby = GetChatLobby();
	switch ( message )
	{
	case WM_CHAR:
		if(menu.active)
		{

			if(menu.get_user_name)
			{
				if( wParam == '\b' )
				{
					if(menu.user_name.size())
						menu.user_name.pop_back();
				}
				else if(wParam >= 32 && wParam < 256 && menu.user_name.length() < 16)
					menu.user_name.push_back((wchar_t)wParam);
			}

			if(menu.get_ip_string)
			{
				if( wParam == '\b' )
				{
					if(menu.ipaddress.size())
						menu.ipaddress.pop_back();
				}
				else if(wParam >= 32 && wParam < 256)
					menu.ipaddress.push_back((wchar_t)wParam);
			}

			if(menu.get_port)
			{
				if( wParam == '\b' )
				{
					if(menu.port.size())
						menu.port.pop_back();
				}
				else if(wParam >= 32 && wParam < 256)
					menu.port.push_back((wchar_t)wParam);
			}
		}
		else if(lobby.active)
		{
			if(lobby.is_typing_chat)
			{
				if( wParam == '\b' )
				{
					if(!lobby.unsent_chat_message.size())
						return 0;

					if(lobby.unsent_chat_message.back() == ' ')
					{
						lobby.unsent_chat_message.pop_back();
						lobby.unsent_chat_message.push_back('_');
					}

					XMFLOAT2 size;
					XMStoreFloat2(&size , GetD3DInt().sprite_font->MeasureString(lobby.unsent_chat_message.c_str()) );

					lobby.unsent_chat_message.pop_back();
					lobby.display_chat_message.pop_back();

					if( size.x*0.11111f > lobby.typing_rect.right - lobby.typing_rect.left && lobby.unsent_chat_message.size() > lobby.display_chat_message.size())
					{
						std::wstring::iterator it = lobby.unsent_chat_message.end() - (lobby.display_chat_message.size()+1);
						wchar_t A = *it;
						std::wstring a;
						a.push_back(A);
						lobby.display_chat_message.insert(0,a.c_str());
					}
				}
				else if(wParam >= 32 && wParam < 256)
				{
					if(wParam != ' ')
					{
						lobby.unsent_chat_message.push_back((wchar_t)wParam);
						lobby.display_chat_message.push_back((wchar_t)wParam);
					}
					else
					{
						lobby.unsent_chat_message.push_back((wchar_t)wParam);
						lobby.display_chat_message.push_back('_');
					}

					XMFLOAT2 size;
					XMStoreFloat2(&size , GetD3DInt().sprite_font->MeasureString(lobby.display_chat_message.c_str()) );
					
					if(wParam == ' ')
					{
						lobby.display_chat_message.pop_back();
						lobby.display_chat_message.push_back(' ');
					}

					if( size.x*0.11111f > lobby.typing_rect.right - lobby.typing_rect.left)
					{
						lobby.display_chat_message.erase(lobby.display_chat_message.begin());
					}
				}
			}
		}
		return 0;
	case WM_MOUSEWHEEL:
		{
			lobby.chat_message_queue_mutex.lock();
			int num_mes = (int)lobby.chat_message_queue.size();
			lobby.chat_message_queue_mutex.unlock();
			if(lobby.active && lobby.chat_window_active)
			{
				int d = GET_WHEEL_DELTA_WPARAM(wParam);
				if(d > 0 && ((num_mes - 10)  > lobby.chat_window_offset))
				{
					lobby.chat_window_offset++;
				}
				else if(d < 0 && lobby.chat_window_offset > 0)
				{
						lobby.chat_window_offset--;
				}
			}
			
			return 0;
		}

	case WM_KEYDOWN:
		if( wParam == VK_ESCAPE ){
			if(menu.active)
				DestroyWindow(hWnd);
		}
		return 0;
	case WM_SYSCOMMAND:
		{
			if((wParam & 0xFFF0) == SC_MOUSEMENU)
				ShellExecute(NULL, L"open", L"http://msdn.microsoft.com/en-us/library/windows/desktop/ms741394(v=vs.85).aspx", NULL, NULL, SW_SHOWNORMAL);
			else
				return DefWindowProc(hWnd, message, wParam, lParam);
		}
		return 0;
	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;
	}
	return DefWindowProc( hWnd, message, wParam, lParam );
}

int WINAPI wWinMain( HINSTANCE hInstance, HINSTANCE, LPTSTR, int )
{
	WIN_APP myApp(hInstance,(WNDPROC)WndProc);	
    MSG msg; ZeroMemory( &msg, sizeof( msg ) );
    while ( msg.message != WM_QUIT && myApp.Run() )
    {	
	    if ( PeekMessage( &msg, NULL, 0, 0, PM_REMOVE ) )
        { 
            TranslateMessage( &msg );
            DispatchMessage( &msg ); 
        }
    }
	myApp.Close();
	return 0; 
}
