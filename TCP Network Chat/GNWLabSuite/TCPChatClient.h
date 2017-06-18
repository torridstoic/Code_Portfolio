#pragma once
#include "ChatLobby.h"//Interface With Game Functionality
#include "NetDefines.h"
#include <stdint.h>//specifided data length types included (uint16_t)
#include <vector>

class TCPChatClient
{
	ChatLobby& chat_interface;//For making calls to add/remove users and display text
	SOCKET mSocket;
	uint8_t mId;
	bool running;
	std::vector<tcpclient> mUserlist;

public:
	TCPChatClient(ChatLobby& chat_int);
	~TCPChatClient(void);
	//Establishes a connection, Only called in Separate Client Network Thread Once, Returns false if a connection can't be established, otherwise send a register to server and return true.
	bool init(std::string name, std::string ip_address, uint16_t port);
	//Recieves data from server, parses it, responds accordingly, Only called in Separate Client Network Thread, Will be continuously called until return = false;
	bool run(void);
	//Called outside of Seaparate Client Network Thread, Called upon getting user input
	bool send_message(std::string message);
	//Notifies server that it is closing and closes down socket, Can be called on multiple threads
	bool stop(void);

private:
	int SendMsg(char* buffer, int len);
};

