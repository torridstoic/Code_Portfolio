#pragma once

#include "TCPChatClient.h"
#include "TCPChatServer.h"
#include "MainMenu.h"
#include "ChatLobby.h"

MainMenu& GetMainMenu(void);
ChatLobby& GetChatLobby(void);

bool run_chat_client(TCPChatClient* client)
{
	ChatLobby& lobby = GetChatLobby();
	if(lobby.is_hosting)//If hosting then wait for server to finish initializing
	{
		//Make sure corresponding server thread is finished initing
		while(!lobby.server_is_finished)//if server hasn't init then keeep waiting
		{
			lobby.server_started_condition.wait(std::unique_lock<std::mutex>(std::mutex()));
		}
		lobby.client_is_waiting = false;
	}

	MainMenu& menu = GetMainMenu();
	char ip[128];
	char p[128];
	char n[128];
	int ret = WideCharToMultiByte(CP_ACP,0,menu.ipaddress.c_str(),(int)menu.ipaddress.length()+1,ip,128,0,0);
	ret = WideCharToMultiByte(CP_ACP,0,menu.port.c_str(),(int)menu.port.length()+1,p,128,0,0);
	ret = WideCharToMultiByte(CP_ACP,0,menu.user_name.c_str(),(int)menu.user_name.length()+1,n,128,0,0);
	string address(ip);
	bool r = client->init(n,address,atoi(p));

	if(!r)
	{
		lobby.DisplayString("CLIENT INITIALIZE : FAIL");
		client->stop();
		return false;
	}

	while(r && lobby.client_running)
		r = client->run();

	if(!r && lobby.client_running)
	{
		lobby.DisplayString("CLIENT RUN : TERMINATED");
		r = client->stop();
	}

	if(!r)
		return false;

	return true;
}

bool run_chat_server(TCPChatServer* server)
{
	MainMenu& menu = GetMainMenu();
	ChatLobby& lobby = GetChatLobby();
	char p[128];
	int ret = WideCharToMultiByte(CP_ACP,0,menu.port.c_str(),(int)menu.port.length()+1,p,128,0,0);
	bool r = server->init(atoi(p));

	//let corrisponding client know that the server has init
	if(lobby.is_hosting)//If hosting, aslong as client is waiting, keep notifying
	{
		lobby.server_is_finished = true;
		while(lobby.client_is_waiting)
			lobby.server_started_condition.notify_all();
	}

	if(!r)
	{
		lobby.DisplayString("SERVER INITIALIZE : FAIL");
		server->stop();
		return false;
	}

	while(r && lobby.server_running)
		r = server->run();

	if(!r && lobby.server_running)
	{
		lobby.DisplayString("SERVER RUN : TERMINATED");
		r = server->stop();
	}	

	if(!r)
		return false;

	return true;
}