#pragma once
#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <WinSock2.h>
#define MAX_NAME_LENGTH 17
#define MAX_CLIENTS 4

enum NET_MESSAGE_TYPE
{
	// From server to client.
	sv_cnt = 1,	// server has granted you access.
	sv_list,	// a complete list of connected clients including yourself.
	sv_add,		// a new client is being added.
	sv_full,	// server as denied you access.
	sv_remove,	// a client has been removed.

	// To/From both.
	sv_cl_msg,	// Chat msg
	sv_cl_close,

	// From client to server.
	cl_reg,		// Client registers a name.
	cl_get,		// Request the buddy list.
};

struct tcpclient
{
	unsigned char ID;
	char name[MAX_NAME_LENGTH];
};

int tcp_recv_whole(SOCKET s, char* buf, int len, int flags);