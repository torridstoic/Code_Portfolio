#include "NetDefines.h"

int tcp_recv_whole(SOCKET s, char* buf, int len, int flags)
{
	int total = 0;

	do
	{
		int ret = recv(s,buf+total,len-total,flags);
		if(ret < 1)
			return ret;
		else
			total += ret;

	}while(total < len);

	return total;
}