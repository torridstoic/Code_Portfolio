#include "platform.h"

const char *getErrorString()
{
	switch (getError())
	{
		case EINTR:					return "EINTR";
		case EBADF:					return "EBADF";
		case EACCES:				return "EACCES";
		case EFAULT:				return "EFAULT";
		case EINVAL:				return "EINVAL";
		case EMFILE:				return "EMFILE";
		case EWOULDBLOCK:			return "EWOULDBLOCK";
		case EINPROGRESS:			return "EINPROGRESS";
		case EALREADY:				return "EALREADY";
		case ENOTSOCK:				return "ENOTSOCK";
		case EDESTADDRREQ:			return "EDESTADDRREQ";
		case EMSGSIZE:				return "EMSGSIZE";
		case EPROTOTYPE:			return "EPROTOTYPE";
		case ENOPROTOOPT:			return "ENOPROTOOPT";
		case EPROTONOSUPPORT:		return "EPROTONOSUPPORT";
		case ESOCKTNOSUPPORT:		return "ESOCKTNOSUPPORT";
		case EOPNOTSUPP:			return "EOPNOTSUPP";
		case EPFNOSUPPORT:			return "EPFNOSUPPORT";
		case EAFNOSUPPORT:			return "EAFNOSUPPORT";
		case EADDRINUSE:			return "EADDRINUSE";
		case EADDRNOTAVAIL:			return "EADDRNOTAVAIL";
		case ENETDOWN:				return "ENETDOWN";
		case ENETUNREACH:			return "ENETUNREACH";
		case ENETRESET:				return "ENETRESET";
		case ECONNABORTED:			return "ECONNABORTED";
		case ECONNRESET:			return "ECONNRESET";
		case ENOBUFS:				return "ENOBUFS";
		case EISCONN:				return "EISCONN";
		case ENOTCONN:				return "ENOTCONN";
		case ESHUTDOWN:				return "ESHUTDOWN";
		case ETOOMANYREFS:			return "ETOOMANYREFS";
		case ETIMEDOUT:				return "ETIMEDOUT";
		case ECONNREFUSED:			return "ECONNREFUSED";
		case ELOOP:					return "ELOOP";
		case ENAMETOOLONG:			return "ENAMETOOLONG";
		case EHOSTDOWN:				return "EHOSTDOWN";
		case EHOSTUNREACH:			return "EHOSTUNREACH";
		case ENOTEMPTY:				return "ENOTEMPTY";
		case EUSERS:				return "EUSERS";
		case EDQUOT:				return "EDQUOT";
		case ESTALE:				return "ESTALE";
		case EREMOTE:				return "EREMOTE";
		default:					return "NO_ERROR";
	}

	return NULL;
}

int sendTcpData(SOCKET skSocket, const char *data, uint16_t length)
{
	int result;
	int bytesSent = 0;

	while (bytesSent < length)
	{
		result = send(skSocket, (const char *) data + bytesSent, length - bytesSent, 0);

		if (result <= 0)
			return result;

		bytesSent += result;
	}

	return bytesSent;
}
