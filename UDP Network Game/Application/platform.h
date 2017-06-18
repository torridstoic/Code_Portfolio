#ifndef _PLATFORM_H_
#define _PLATFORM_H_

// Platform-specific headings for Winsock (Win32) networking
#if defined(_WIN32)
	
	#include <winsock2.h>
	#pragma comment(lib, "Ws2_32.lib")
	#include "stdint.h"

	typedef int socklen_t;

	// Winsock startup function.
	inline int startup()
	{
	    WSADATA wsadata;
		return WSAStartup(WINSOCK_VERSION, &wsadata);
	}

	// Winsock shutdown function
	inline int shutdown()
	{
		return WSACleanup();
	}

	// Standard sockets close() function
	inline int close(SOCKET s)
	{
		return closesocket(s);
	}

	// Standard io-control function
	inline int ioctl(SOCKET s, long cmd, u_long *argp)
	{
		return ioctlsocket(s, cmd, argp);
	}

	// A helper function to identify the last network error.
	inline int getError()
	{
		return WSAGetLastError();
	}

	// Platform-specific critical section
	// Class for accessing critical sections.
	class CriticalSection
	{
		private:
			CRITICAL_SECTION cs;

		public:
			CriticalSection() { InitializeCriticalSection(&cs); }
			~CriticalSection() { DeleteCriticalSection(&cs); }

			void enter() { EnterCriticalSection(&cs); }
			void leave() { LeaveCriticalSection(&cs); }
	};

	// Platform-specific library export line (in Windows, DLL export.)
	#define EXPORTED extern "C" __declspec(dllexport) 

	// Define the standard error codes for sockets-based systems.
	// (Note: these may clash with file IO definitions in Windows.)
	#define EINTR					WSAEINTR
	#define EBADF					WSAEBADF
	#define EACCES					WSAEACCES
	#define EFAULT					WSAEFAULT
	#define EINVAL					WSAEINVAL
	#define EMFILE					WSAEMFILE
	#define EWOULDBLOCK				WSAEWOULDBLOCK
	#define EINPROGRESS				WSAEINPROGRESS
	#define EALREADY				WSAEALREADY
	#define ENOTSOCK				WSAENOTSOCK
	#define EDESTADDRREQ			WSAEDESTADDRREQ
	#define EMSGSIZE				WSAEMSGSIZE
	#define EPROTOTYPE				WSAEPROTOTYPE
	#define ENOPROTOOPT				WSAENOPROTOOPT
	#define EPROTONOSUPPORT			WSAEPROTONOSUPPORT
	#define ESOCKTNOSUPPORT			WSAESOCKTNOSUPPORT
	#define EOPNOTSUPP				WSAEOPNOTSUPP
	#define EPFNOSUPPORT			WSAEPFNOSUPPORT
	#define EAFNOSUPPORT			WSAEAFNOSUPPORT
	#define EADDRINUSE				WSAEADDRINUSE
	#define EADDRNOTAVAIL			WSAEADDRNOTAVAIL
	#define ENETDOWN				WSAENETDOWN
	#define ENETUNREACH				WSAENETUNREACH
	#define ENETRESET				WSAENETRESET
	#define ECONNABORTED			WSAECONNABORTED
	#define ECONNRESET				WSAECONNRESET
	#define ENOBUFS					WSAENOBUFS
	#define EISCONN					WSAEISCONN
	#define ENOTCONN				WSAENOTCONN
	#define ESHUTDOWN				WSAESHUTDOWN
	#define ETOOMANYREFS			WSAETOOMANYREFS
	#define ETIMEDOUT				WSAETIMEDOUT
	#define ECONNREFUSED			WSAECONNREFUSED
	#define ELOOP					WSAELOOP
	#define ENAMETOOLONG			WSAENAMETOOLONG
	#define EHOSTDOWN				WSAEHOSTDOWN
	#define EHOSTUNREACH			WSAEHOSTUNREACH
	#define HOST_NOT_FOUND			WSAHOST_NOT_FOUND
	#define TRY_AGAIN				WSATRY_AGAIN
	#define NO_RECOVERY				WSANO_RECOVERY
	#define NO_DATA					WSANO_DATA

	#define ENOTEMPTY				WSAENOTEMPTY
	#define EPROCLIM				WSAEPROCLIM
	#define EUSERS					WSAEUSERS
	#define EDQUOT					WSAEDQUOT
	#define ESTALE					WSAESTALE
	#define EREMOTE					WSAEREMOTE
	#define EDISCON					WSAEDISCON
	#define ENOMORE					WSAENOMORE
	#define ECANCELLED				WSAECANCELLED
	#define EINVALIDPROCTABLE		WSAEINVALIDPROCTABLE
	#define EINVALIDPROVIDER		WSAEINVALIDPROVIDER
	#define EPROVIDERFAILEDINIT		WSAEPROVIDERFAILEDINIT

	#define SYSNOTREADY				WSASYSNOTREADY
	#define VERNOTSUPPORTED			WSAVERNOTSUPPORTED
	#define NOTINITIALISED			WSANOTINITIALISED

	#define SYSCALLFAILURE			WSASYSCALLFAILURE
	#define SERVICE_NOT_FOUND		WSASERVICE_NOT_FOUND
	#define TYPE_NOT_FOUND			WSATYPE_NOT_FOUND

#else
	// Platform-specific headers for unix networking
	#include <sys/socket.h>
	#include <sys/ioctl.h>
	#include <arpa/inet.h>
	#include <pthread.h>
	#include <unistd.h>
	#include <stdlib.h>
	#include <stdint.h>
	#include <errno.h>


	typedef int SOCKET;

	inline int startup()
	{
		return 0;
	}

	inline int shutdown()
	{
		return 0;
	}

	// A helper function to identify the last network error.
	inline int getError()
	{
		return errno;
	}

	// Platform-specific critical section
	// Class for accessing critical sections.
	class CriticalSection
	{
		private:
			pthread_mutex_t cs;

		public:
			CriticalSection() { pthread_mutex_init(&cs, NULL); }
			~CriticalSection() { pthread_mutex_destroy(&cs); }

			void enter() { pthread_mutex_lock(&cs); }
			void leave() { pthread_mutex_unlock(&cs); }
	};

	// Defines to help Unix deal with some undefined identifiers
	#define EXPORTED extern "C" 

	#define INVALID_SOCKET -1
	#define SD_BOTH 2
	#define SOCKET_ERROR -1
#endif

/*****************
 **  Functions  **
 *****************/

// A helper function to identify the last network error. Returns the
// string value of the last error.
const char *getErrorString();

// Helper function for sending TCP data.
int sendTcpData(SOCKET skSocket, const char *data, uint16_t length);

#endif
