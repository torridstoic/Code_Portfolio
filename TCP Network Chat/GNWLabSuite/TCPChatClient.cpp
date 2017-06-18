#include "TCPChatClient.h"

TCPChatClient::TCPChatClient(ChatLobby& chat_int) : chat_interface(chat_int)
{ }
TCPChatClient::~TCPChatClient(void)
{ }

/// <summary>
/// Establishes a connection.
/// Only called in separate Client Network Thread once.
/// </summary>
/// <returns>false if unable to connect; true once connected and "register" msg sent</returns>
bool TCPChatClient::init(std::string name, std::string ip_address, uint16_t port)
{
	// Create a SOCKET
	mSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (INVALID_SOCKET == mSocket)
	{
		stop();
		return false;
	}

	// Create a SOCKADDR
	SOCKADDR_IN sAddr;
	sAddr.sin_family = AF_INET;
	sAddr.sin_port = ntohs(port);
	sAddr.sin_addr.s_addr = inet_addr(ip_address.c_str());
	if (INADDR_NONE == sAddr.sin_addr.s_addr || INADDR_ANY == sAddr.sin_addr.s_addr)
	{
		stop();
		return false;
	}

	// Connect
	if (SOCKET_ERROR == connect(mSocket, (SOCKADDR*)&sAddr, sizeof(sAddr)))
	{
		stop();
		return false;
	}

	// Send "Register" msg
	char* buffer = new char[20];
	uint16_t size = MAX_NAME_LENGTH + 1;
	uint8_t type = cl_reg;
	std::memcpy(buffer, &size, sizeof(size));
	std::memcpy(buffer + 2, &type, sizeof(type));
	std::memcpy(buffer + 3, name.c_str(), MAX_NAME_LENGTH);
	if (SOCKET_ERROR == SendMsg(buffer, 20))
	{
		return false;
	}

	chat_interface.DisplayString("CLIENT: CONNECTED TO SERVER");
	running = true;
	return true;
}

/// <summary>
/// Receives data from server, parses it, responds accordingly.
/// Only called in separate Client network thread.
/// Will be continuously called until it returns false.
/// </summary>
/// <returns>true on successful message parsing & response, false on disconnect or error</returns>
bool TCPChatClient::run(void)
{
	if (!running)
	{
		return false;
	}

	// Read the message Header (length)
	// Header: always 2 chars, and gives the length of incoming msg.
	char* header = new char[2];
	if (SOCKET_ERROR == tcp_recv_whole(mSocket, header, 2, 0))
	{
		stop();
		delete[] header;
		return false;
	}
	// Convert the char* header into a usable number
	uint16_t len;
	std::memcpy(&len, header, sizeof(len));
	delete[] header;

	// Read the rest of the message
	char* buffer = new char[len + 1];
	if (SOCKET_ERROR == tcp_recv_whole(mSocket, buffer, len, 0))
	{
		stop();
		delete[] buffer;
		return false;
	}

	// Message received! Parse it and continue...
	// (the first char of buffer determines the type of incoming msg)
	bool retVal = false;
	switch (buffer[0])
	{
	// The server's confirmation that the client has registered:
	case(sv_cnt):
	{
		mId = buffer[1];
		chat_interface.DisplayString("CLIENT: REGISTERED IN SERVER");

		// Now we request the userlist.
		// Create & send the cl_get message:
		char* msg = new char[3];
		uint16_t size = 1;
		uint8_t type = cl_get;
		std::memcpy(msg, &size, sizeof(size));
		std::memcpy(msg + 2, &type, sizeof(type));
		if (SOCKET_ERROR == SendMsg(msg, 3))
		{
			retVal = false;
			break;
		}
		// else
		retVal = true;
		break;
	}
	// The server's reject message (chatroom is full):
	case(sv_full):
		chat_interface.DisplayString("CLIENT: SERVER IS FULL");
		retVal = true;
		break;
	// The "connected clients" list sent by the server:
	case(sv_list):
	{
		int numUsers = buffer[1];

		int activeByte = 2;
		for (uint8_t i = 0; i < numUsers; ++i)
		{
			// Copy each user ID & name
			tcpclient user;
			user.ID = buffer[activeByte++];
			std::strcpy((char*)user.name, (buffer + activeByte));

			// Add each user to the list display
			if (!chat_interface.AddNameToUserList(user.name, user.ID))
			{
				retVal = false;
				break;
			}

			// Add each user to the userlist vector
			mUserlist.push_back(user);
			// Increment "activeByte" (move the reader) and continue
			activeByte += (MAX_NAME_LENGTH);
			retVal = true;
		}

		if (true == retVal)
		{
			chat_interface.DisplayString("CLIENT: RECEIVED USER LIST");
		}
		break;
	}
	// The server informing all clients that a new client joined:
	case(sv_add):
	{
		// Create a new user object and fill his ID & name
		tcpclient user;
		user.ID = buffer[1];
		std::strcpy((char*)user.name, (buffer + 2));

		if (chat_interface.AddNameToUserList(user.name, user.ID))
		{
			// Display a message to the user
			std::string disp = "CLIENT: ";
			disp.append(user.name);
			disp.append(" JOINED");
			chat_interface.DisplayString(disp);

			// Add him to the userlist vector and continue
			mUserlist.push_back(user);
			retVal = true;
		}
		break;
	}
	// The server notifying all clients that a client has left:
	case(sv_remove):
		for (std::vector<tcpclient>::iterator iter = mUserlist.begin();
			iter != mUserlist.end();
			++iter)
		{
			// Find the absent client in the userlist vector
			if ((*iter).ID == buffer[1])
			{
				if (chat_interface.RemoveNameFromUserList(buffer[1]))
				{
					// Display a message to the user
					std::string disp = "CLIENT: ";
					disp.append((*iter).name);
					disp.append(" LEFT");
					chat_interface.DisplayString(disp);
					
					// Remove the client from the vector, and continue
					mUserlist.erase(iter);
					retVal = true;
					break;
				}
			}
		}
		// else default retVal = false
		break;
	// A message is sent to display to the user:
	case(sv_cl_msg):
	{
		uint8_t uid = buffer[1];
		if (chat_interface.AddChatMessage((buffer + 2), uid))
		{
			retVal = true;
		}
		break;
	}
	// Notification to shutdown/disconnect:
	case(sv_cl_close):
		chat_interface.DisplayString("CLIENT: SERVER DISCONNECTED");
		stop();
		retVal = true;
		break;
	}

	// Cleanup loose memory and return
	delete[] buffer;
	return retVal;
}

/// <summary>
/// Called outside of separate Client network thread.
/// Called upon getting user input.
/// </summary>
/// <param name="message">User's message (typed input)</param>
/// <returns>true on successful message sending, false on disconnect or error</returns>
bool TCPChatClient::send_message(std::string message)
{
	// Construct a message string formatted for transfer
	uint16_t size = message.length() + 3;
	uint8_t type = sv_cl_msg;
	char* msg = new char[size + 2];
	std::memcpy(msg, &size, sizeof(size));
	std::memcpy(msg + 2, &type, sizeof(type));
	std::memcpy(msg + 3, &mId, sizeof(mId));
	std::memcpy(msg + 4, message.c_str(), message.length());
	msg[size + 1] = '\0';

	// Call helper method to send the message
	if (SOCKET_ERROR == SendMsg(msg, size + 2))
	{
		// Display failure to user, and exit
		chat_interface.DisplayString("CLIENT: ERROR SENDING CHAT MSG");
		return false;
	}
	return true;
}

/// <summary>
/// Notifies server that it is closing, and closes down socket.
/// Can be called on multiple threads.
/// </summary>
bool TCPChatClient::stop(void)
{
	if (running)
	{
		// Constuct a "closing" message for transfer
		uint16_t size = 2;
		uint8_t type = sv_cl_close;
		char* msg = new char[size + 2];
		std::memcpy(msg, &size, sizeof(size));
		std::memcpy(msg + 2, &type, sizeof(type));
		std::memcpy(msg + 3, &mId, sizeof(mId));
		
		// Call helper method to send the message
		if (SendMsg(msg, size + 2) > 0)
		{
			// Successfully sent. Shutdown the socket and exit.
			shutdown(mSocket, SD_BOTH);
			closesocket(mSocket);
			running = false;
		}
	}
	return false;
}

/// <summary>
/// Helper method to send messages to a Server Socket.
/// </summary>
/// <param name="buffer">Pointer to start of pre-constructed message to send</param>
/// <param name="len">Length of message to send</param>
/// <returns>Result of transfer: success or SOCKET_ERROR</returns>
int TCPChatClient::SendMsg(char* buffer, int len)
{
	int bytesSent = 0;
	int sendResult;

	while (bytesSent < len)
	{
		// Calling "send" does not guarantee the full message is sent,
		// however, it will always send "in-order."
		// The return value is how many bytes were successfully sent,
		// so continue sending until it's all done.
		sendResult = send(mSocket, buffer + bytesSent, len - bytesSent, 0);
		if (SOCKET_ERROR == sendResult)
		{
			// Upon a socket error, immediately stop and return the result
			stop();
			return SOCKET_ERROR;
		}
		bytesSent += sendResult;
	}

	return sendResult;
}
