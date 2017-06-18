//Chat Lobby, Game Networking Lab Suite
//Tim Turcich
//Game Networking
//Full Sail University
//August-September 2013
#include "ChatLobby.h"
#include "MainMenu.h"
#include "D3DInterface.h"
#include "XTime.h"
#include "TCPChatClient.h"
#include "TCPChatServer.h"

MainMenu& GetMainMenu(void);
D3DInterface& GetD3DInt(void);
XTime& GetTransitionTimer(void);

ChatLobby::ChatLobby(void)
{
	active = false;
	chat_server = 0;
	chat_client = 0;
	client_running = false;
	server_running = false;
}

ChatLobby::~ChatLobby(void)
{
}

ChatLobby::ChatLobby(ChatLobby& copy)
{
	*this = copy;
}

ChatLobby& ChatLobby::operator=(ChatLobby& rhs)
{
	
	return rhs;
}

void ChatLobby::Initialize(HWND hWnd)
{
	window = hWnd;
	active = true;
	is_typing_chat = false;
	unsent_chat_message.clear();
	user_list_selected = false;
	chat_window_offset = 0;
	chat_window_active = false;
	previous_index = -1;
	is_hosting = false;

	client_running = false;
	server_running = false;
}

bool ChatLobby::AddNameToUserList(std::string user_name, unsigned int id)
{
	/*Converting to Wide*/
	std::wstring name;
	name.resize(user_name.size());
	mbstowcs(&(*name.begin()),&(*user_name.begin()),user_name.size());

	user_name_mutex.lock();
	if(users.size() < 4)
	{
		users[id] = name;
		user_name_mutex.unlock();
		return true;
	}
	user_name_mutex.unlock();
	return false;
}

bool ChatLobby::RemoveNameFromUserList(unsigned int id)
{
	user_name_mutex.lock();
	if(users.count(id))//if the user exists
	{
		users.erase(users.find(id));
		user_name_mutex.unlock();
		return true;
	}
	user_name_mutex.unlock();
	return false;
}

bool ChatLobby::AddChatMessage(std::string chat_message, unsigned int id_from)
{
	/*Converting to Wide*/
	std::wstring message;
	message.resize(chat_message.size());
	mbstowcs(&(*message.begin()),&(*chat_message.begin()),chat_message.size());

	chat_message_queue_mutex.lock();
	D3DInterface& d3d = GetD3DInt();
	if(users.count(id_from))//if the user exists
	{
		std::wstring s = users[id_from];
		s.append(std::wstring(L": "));
		s.append(message);

		XMFLOAT2 size;
		XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));

		if(size.x*0.0875 < message_rect.right - message_rect.left)
			chat_message_queue.push_front(s);
		else
		{
			do
			{
				int i = 46;
				std::wstring line;
				if(*(s.begin()+i) == ' ')
				{
					line.append(s.begin(),s.begin()+i);
					chat_message_queue.push_front(line);
					s.erase(s.begin(),s.begin()+i);
					XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));
				}
				else
				{
					i--;
					while(*(s.begin()+i) != ' ' && i > 0)
						i--;

					if(!i)
						i = 46;

					line.append(s.begin(),s.begin()+i);
					chat_message_queue.push_front(line);
					s.erase(s.begin(),s.begin()+i);
					XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));
				}
			}while(size.x*0.0875 > message_rect.right - message_rect.left);

			chat_message_queue.push_front(s);
		}
		chat_message_queue_mutex.unlock();
		return true;
	}
	chat_message_queue_mutex.unlock();
	return false;
}

bool ChatLobby::DisplayString(std::string display)
{
	/*Converting to Wide*/
	std::wstring s;
	s.resize(display.size());
	mbstowcs(&(*s.begin()),&(*display.begin()),display.size());

	chat_message_queue_mutex.lock();
	D3DInterface& d3d = GetD3DInt();

	XMFLOAT2 size;
	XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));

	if(size.x*0.0875 < message_rect.right - message_rect.left)
		chat_message_queue.push_front(s);
	else
	{
		do
		{
			int i = 46;
			std::wstring line;
			if(*(s.begin()+i) == ' ')
			{
				line.append(s.begin(),s.begin()+i);
				chat_message_queue.push_front(line);
				s.erase(s.begin(),s.begin()+i);
				XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));
			}
			else
			{
				i--;
				while(*(s.begin()+i) != ' ' && i > 0)
					i--;

				if(!i)
					i = 46;

				line.append(s.begin(),s.begin()+i);
				chat_message_queue.push_front(line);
				s.erase(s.begin(),s.begin()+i);
				XMStoreFloat2(&size, d3d.consolas_11_font->MeasureString(s.c_str()));
			}
		}while(size.x*0.0875 > message_rect.right - message_rect.left);

		chat_message_queue.push_front(s);
	}
	chat_message_queue_mutex.unlock();
	return true;
}

bool& isTransitioning(void);
bool& hasTransitioned(void);

void ChatLobby::CheckInteraction(void)
{
	XTime& trans = GetTransitionTimer();
	MainMenu& menu = GetMainMenu();
	if(isTransitioning() && !hasTransitioned())
	{
		if(trans.TotalTime() > 3.0f )
		{
			hasTransitioned() = true;
			menu.active = true;//turn on lobby
			active = false;//turn off menu
			PlaySound(L"pipe.wav", NULL, SND_ASYNC);
		}
		return;
	}

	RECT window_rect;
	RECT client_rect;
	GetWindowRect(window,&window_rect);
	GetClientRect(window,&client_rect);
	POINT mouse_position;
	GetCursorPos(&mouse_position);
	POINT mouse_to_client = mouse_position;
	ScreenToClient(window,&mouse_to_client);

	hover_leave = false;
	if(PtInRect(&this->leave_rect,mouse_to_client))
	{
		hover_leave = true;
	}

	static bool left_buffered = false;
	SHORT left_pressed = GetAsyncKeyState(VK_LBUTTON);
	if(left_pressed && !left_buffered && PtInRect(&client_rect,mouse_to_client))
	{
		left_buffered = true;

		is_typing_chat = false;
		user_list_selected = false;
		chat_window_active = false;
		if(PtInRect(&this->typing_rect,mouse_to_client))
		{
			is_typing_chat = true;
		}

		if(PtInRect(&this->user_list_rect,mouse_to_client))
		{
			user_list_selected = true;
		}

		if(PtInRect(&this->message_rect,mouse_to_client))
		{
			chat_window_active = true;
		}

		if(PtInRect(&this->leave_rect,mouse_to_client))
		{
			GetTransitionTimer().Restart();
			is_hosting = false;
			isTransitioning() = true;
			hasTransitioned() = false;
			if(server_running)
			{
				server_running = false;
				client_running = false;
				chat_server->stop();
				chat_client->stop();
				server_thread.get();
				client_thread.get();
				delete chat_server;
				delete chat_client;
			}
			else if(client_running)
			{
				client_running = false;
				chat_client->stop();
				client_thread.get();
				delete chat_client;
			}

			chat_message_queue.clear();
			chat_window_offset = 0;
			users.clear();
			previous_message_queue.clear();
			previous_display_queue.clear();
		}
	}
	else if(!left_pressed)
		left_buffered = false;

	static bool enter_buffered = false;
	SHORT enter_pressed = GetAsyncKeyState(VK_RETURN);
	if(enter_pressed && !enter_buffered)
	{
		if(is_typing_chat)
		{
			if(unsent_chat_message.size())
			{
				std::string s;
				s.resize(unsent_chat_message.size());
				int ret = WideCharToMultiByte(CP_ACP,0,&(*unsent_chat_message.begin()),(int)unsent_chat_message.size(),&(*s.begin()),(int)unsent_chat_message.size(),0,0);
				this->chat_client->send_message(s);
				previous_message_queue.push_front(this->unsent_chat_message);
				previous_display_queue.push_front(this->display_chat_message);
				unsent_chat_message.clear();
				display_chat_message.clear();
				previous_index = -1;
			}
		}

		is_typing_chat = !is_typing_chat;
		enter_buffered = true;
	}
	else if(!enter_pressed)
	{
		enter_buffered = false;
	}

	static bool up_buffered = false;
	SHORT up_pressed = GetAsyncKeyState(VK_UP);
	if(up_pressed && !up_buffered)
	{
		int size = ((int)previous_message_queue.size());
		if(is_typing_chat && size)
		{
			if(previous_index < size-1)
				previous_index++;

			unsent_chat_message = previous_message_queue[previous_index];
			display_chat_message = previous_display_queue[previous_index];
		}

		up_buffered = true;
	}
	else if(!up_pressed)
	{
		up_buffered = false;
	}

	static bool down_buffered = false;
	SHORT down_pressed = GetAsyncKeyState(VK_DOWN);
	if(down_pressed && !down_buffered)
	{

		int size = ((int)previous_message_queue.size());
		if(is_typing_chat && size)
		{
			if(previous_index > -1)
			{
				previous_index--;
			}
			if(previous_index > -1)
			{
				unsent_chat_message = previous_message_queue[previous_index];
				display_chat_message = previous_display_queue[previous_index];
			}
			else
			{
				unsent_chat_message.clear();
				display_chat_message.clear();
			}
		}

		down_buffered = true;
	}
	else if(!down_pressed)
	{
		down_buffered = false;
	}
}
