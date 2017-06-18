#pragma once
#define _WINSOCKAPI_ 
#include <Windows.h>
#include <queue>
#include <deque>
#include <map>
#include <string>
#include <future>
#include <mutex>
#include <atomic>

class TCPChatClient;
class TCPChatServer;

class ChatLobby
{
	friend bool run_chat_server(TCPChatServer* server);
	friend bool run_chat_client(TCPChatClient* client);
	friend class D3DInterface;
	friend class MainMenu;
	friend class WIN_APP;
	friend LRESULT CALLBACK WndProc( HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam );
private:
	/*Net*/
	bool is_hosting;
	std::atomic_bool client_running;
	std::atomic_bool server_running;
	std::future<bool> client_thread;
	std::future<bool> server_thread;
	std::mutex chat_message_queue_mutex;
	std::mutex user_name_mutex;
	std::atomic_bool server_is_finished;
	std::atomic_bool client_is_waiting;
	std::unique_lock<std::mutex> hosting_start_safty;
	std::condition_variable server_started_condition;
	TCPChatServer* chat_server;
	TCPChatClient* chat_client;

	int chat_window_offset;
	bool chat_window_active;
	HWND window;
	bool active;
	bool is_typing_chat;
	std::wstring unsent_chat_message;
	std::wstring display_chat_message;
	std::wstring chat_window;
	RECT typing_rect;
	RECT message_rect;
	RECT user_list_rect;
	RECT leave_rect;
	bool hover_leave;
	bool user_list_selected;
	std::deque<std::wstring> chat_message_queue;
	std::deque<std::wstring> previous_message_queue;
	std::deque<std::wstring> previous_display_queue;
	int previous_index;
	std::map<unsigned int,std::wstring> users;
	ChatLobby(void);
	~ChatLobby(void);
	ChatLobby(ChatLobby& copy);
	ChatLobby& operator=(ChatLobby& rhs);
	void Initialize(HWND hWnd);
	void CheckInteraction(void);
public:
	bool AddNameToUserList(std::string name, unsigned int id);
	bool RemoveNameFromUserList(unsigned int id);
	bool AddChatMessage(std::string message, unsigned int id_from);
	bool DisplayString(std::string display);
};

