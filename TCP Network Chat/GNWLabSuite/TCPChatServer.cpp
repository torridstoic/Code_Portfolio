#include "TCPChatServer.h"

TCPChatServer::TCPChatServer(ChatLobby& chat_int) : chat_interface(chat_int)
{ }
TCPChatServer::~TCPChatServer(void)
{ }

/// <summary>
/// Establishes a listening Socket.
/// Only called once in separate Server network thread.
/// </summary>
/// <returns>true when listening has started on proper port, false on error</returns>
bool TCPChatServer::init(uint16_t port)
{
	// Prep member vars (client list, userlist, socket set)
	for (int i = 0; i < MAX_CLIENTS; ++i)
	{
		mClients[i] = NULL;
		mUserlist[i].ID = -1;
	}
	FD_ZERO(&mMasterSockSet);

#pragma region Listener Socket
	// Create listener socket
	mListener = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (INVALID_SOCKET == mListener)
	{
		stop();
		return false;
	}

	// Create appropriate sockaddr
	SOCKADDR_IN sAddr;
	sAddr.sin_family = AF_INET;
	sAddr.sin_port = htons(port);
	sAddr.sin_addr.s_addr = INADDR_ANY;

	// Bind socket & sockaddr, then begin listening for clients
	if (SOCKET_ERROR == bind(mListener, (SOCKADDR*)&sAddr, sizeof(sAddr)))
	{
		stop();
		return false;
	}
	FD_SET(mListener, &mMasterSockSet);
	if (SOCKET_ERROR == listen(mListener, MAX_CLIENTS))
	{
		stop();
		return false;
	}
#pragma endregion

	// Display success for user, then continue
	chat_interface.DisplayString("SERVER: NOW HOSTING SERVER");
	running = true;
	return true;
}

/// <summary>
/// Receives data from clients, parses it, and responds accordingly.
/// Only called in separate Server network thread.
/// Will be continuously called until it returns false.
/// </summary>
/// <returns>true on successful message parsing & response, false on disconnect or error</returns>
bool TCPChatServer::run(void)
{
	if (!running)
	{
		return false;
	}
	bool ret = true;

	// Call "select" to get the set of readable sockets.
	// This includes clients that are sending, and incoming connection requests.
	fd_set readySocketSet = mMasterSockSet;
	int numRdySocks = select(0, &readySocketSet, NULL, NULL, NULL);

#pragma region Process the Listener
	if (FD_ISSET(mListener, &readySocketSet))
	{
		// Incoming client connection. Check if there's room.
		bool clientsFull = true;

		for (int i = 0; i < MAX_CLIENTS; ++i)
		{
			if (NULL == mClients[i])
			{
				// Open client slot found.
				// Accept the request and add new client to the Master socket set.
				// (the new client will soon send a request for the client list, but not yet)
				clientsFull = false;

				mClients[i] = accept(mListener, NULL, NULL);
				if (SOCKET_ERROR == mClients[i])
				{
					stop();
					return false;
				}
				FD_SET(mClients[i], &mMasterSockSet);

				break;
			}
		}

		if (clientsFull)
		{
			// No open slots found.
			// Create a temp socket to accept the new client,
			// in order to communicate with it.
			SOCKET tmpSock = accept(mListener, NULL, NULL);
			if (SOCKET_ERROR == tmpSock)
			{
				stop();
				return false;
			}

			// Send a message notifying that the chatroom is full
			uint16_t size = 1;
			uint8_t type = sv_full;
			char* msg = new char[size + 2];
			memcpy(msg, &size, sizeof(size));
			memcpy(msg + 2, &type, sizeof(type));
			if (SOCKET_ERROR == SendMsg(msg, size + 2, tmpSock))
			{
				stop();
				return false;
			}

			// Cleanup loose memory, shutdown the socket, and continue
			delete[] msg;
			shutdown(tmpSock, SD_BOTH);
			closesocket(tmpSock);
		}

		// Listening socket has been handled.
		// Remove it from the ready set, and continue to the clients.
		FD_CLR(mListener, &readySocketSet);
		--numRdySocks;
	}
#pragma endregion

#pragma region Process Clients
	if (numRdySocks > 0)
	{
		for (int i = 0; i < MAX_CLIENTS; ++i)
		{
			// Check to see if current clients are ready
			if (FD_ISSET(mClients[i], &readySocketSet))
			{
				// Ready client found:
				// Read the message header (length of incoming msg)
				char* header = new char[2];
				if (SOCKET_ERROR == tcp_recv_whole(mClients[i], header, 2, 0))
				{
					stop();
					delete[] header;
					return false;
				}
				uint16_t len;
				std::memcpy(&len, header, sizeof(len));
				delete[] header;

				// Recv the message
				char* buffer = new char[len + 1];
				if (SOCKET_ERROR == tcp_recv_whole(mClients[i], buffer, len, 0))
				{
					stop();
					delete[] buffer;
					return false;
				}

#pragma region Message Parsing
				switch (buffer[0])
				{
				// Chat message:
				case(sv_cl_msg):
				{
					// Re-format the message for transfer
					char* msg = new char[len + 2];
					std::memcpy(msg, &len, sizeof(len));
					std::memcpy(msg + 2, buffer, len);
					for (int j = 0; j < MAX_CLIENTS; ++j)
					{
						// Send the message to all clients
						if (NULL != mClients[j] &&
							SOCKET_ERROR == SendMsg(msg, len + 2, mClients[j]))
						{
							ret = false;
							break;
						}
					}
					delete[] msg;

					break;
				}
				// Client is disconnecting:
				case(sv_cl_close):
				{
					// Send sv_remove to ALL other users
					// (notify them a client is leaving)
					uint16_t size = 2;
					uint8_t type = sv_remove;
					char* msg = new char[size + 2];
					std::memcpy(msg, &size, sizeof(size));
					std::memcpy(msg + 2, &type, sizeof(type));
					msg[3] = i;
					for (int j = 0; j < MAX_CLIENTS; ++j)
					{
						if (j != i &&
							NULL != mClients[j] &&
							SOCKET_ERROR == SendMsg(msg, size + 2, mClients[j]))
						{
							ret = false;
							break;
						}
					}
					delete[] msg;

					// "Remove" leaving client from local userlist
					mUserlist[i].ID = -1;

					// Clean up the leaving client's socket
					FD_CLR(mClients[i], &mMasterSockSet);
					shutdown(mClients[i], SD_BOTH);
					closesocket(mClients[i]);
					mClients[i] = NULL;

					break;
				}
				// Newly connected client is registering for chat:
				case(cl_reg):
				{
					// Add new user's data to userlist array
					mUserlist[i].ID = i;
					std::strcpy((char*)mUserlist[i].name, buffer + 1);

					// Send sv_cnt to new client (connection confirmation)
					uint16_t size = 2;
					uint8_t type = sv_cnt;
					char* msg = new char[size + 2];
					std::memcpy(msg, &size, sizeof(size));
					std::memcpy(msg + 2, &type, sizeof(type));
					msg[3] = i;
					if (SOCKET_ERROR == SendMsg(msg, size + 2, mClients[i]))
					{
						ret = false;
						break;
					}
					delete[] msg;

					// Send sv_add to ALL other clients (add new client to userlists)
					size = MAX_NAME_LENGTH + 2;
					type = sv_add;
					msg = new char[size + 2];
					std::memcpy(msg, &size, sizeof(size));
					std::memcpy(msg + 2, &type, sizeof(type));
					msg[3] = i;
					std::strcpy(msg + 4, buffer + 1); // username
					for (int j = 0; j < MAX_CLIENTS; ++j)
					{
						if (j != i &&
							NULL != mClients[j] &&
							SOCKET_ERROR == SendMsg(msg, size + 2, mClients[j]))
						{
							ret = false;
							break;
						}
					}
					delete[] msg;

					break;
				}
				// New client is requesting the userlist:
				case(cl_get):
				{
					// Prep message: sv_list
					uint8_t numUsers = 0;
					for (int j = 0; j < MAX_CLIENTS; ++j)
					{
						if (NULL != mClients[j])
						{
							++numUsers;
						}
					}
					uint16_t size = numUsers * (MAX_NAME_LENGTH + 1) + 2;
					uint8_t type = sv_list;
					char* msg = new char[size + 2];
					std::memcpy(msg, &size, sizeof(size));
					std::memcpy(msg + 2, &type, sizeof(type));
					std::memcpy(msg + 3, &numUsers, sizeof(numUsers));
					for (uint8_t j = 0, index = 0; j < numUsers; ++j, ++index)
					{
						// Skip "inactive" users
						while (mUserlist[index].ID != index)
						{
							++index;
						}

						// Add all active users' data to the message
						int activeByte = 4 + ((MAX_NAME_LENGTH + 1) * j);
						msg[activeByte++] = mUserlist[index].ID;
						std::memcpy(msg + activeByte, &(mUserlist[index].name), MAX_NAME_LENGTH);
					}

					// Send message to the querying client socket
					if (SOCKET_ERROR == SendMsg(msg, size + 2, mClients[i]))
					{
						ret = false;
					}
					delete[] msg;

					break;
				}
				}
#pragma endregion
#pragma endregion
				delete[] buffer;
			}
		}
	}

	return ret;
}

/// <summary>
/// Notifies Clients of closure & closes down sockets.
/// Can be called on multiple threads.
/// </summary>
bool TCPChatServer::stop(void)
{
	bool ret = true;

	if (running)
	{
		// Send sv_cl_close to all clients
		uint16_t size = 2;
		uint8_t type = sv_cl_close;
		char* msg = new char[size + 2];
		std::memcpy(msg, &size, sizeof(size));
		std::memcpy(msg, &type, sizeof(type));
		msg[3] = '0';
		for (int i = 0; i < MAX_CLIENTS; ++i)
		{
			if (NULL != mClients[i])
			{
				if (SOCKET_ERROR == SendMsg(msg, size + 2, mClients[i]))
				{
					ret = false;
				}
				// and close each client socket
				shutdown(mClients[i], SD_BOTH);
				closesocket(mClients[i]);
				mClients[i] = NULL;
			}
			// "Reset" the userlist
			mUserlist[i].ID = -1;
		}
		delete[] msg;

		// Clean up Listener and master socket set
		shutdown(mListener, SD_BOTH);
		closesocket(mListener);
		FD_ZERO(&mMasterSockSet);
		running = false;
	}

	return ret;
}

/// <summary>
/// Helper method to send pre-constructed messages to another Socket.
/// </summary>
/// <param name="buffer">Pointer to start of pre-constructed message to send</param>
/// <param name="len">Length of message to send</param>
/// <param name="sock">Destination Socket</param>
/// <returns>Result of transfer: success or SOCKET_ERROR</returns>
int TCPChatServer::SendMsg(char* buffer, int len, SOCKET sock)
{
	int bytesSent = 0;
	int sendResult;

	while (bytesSent < len)
	{
		sendResult = send(sock, buffer + bytesSent, len - bytesSent, 0);
		if (SOCKET_ERROR == sendResult)
		{
			stop();
			return SOCKET_ERROR;
		}
		bytesSent += sendResult;
	}

	return sendResult;
}
