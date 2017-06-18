#include "ServerWrapper.h"
#include "Server.h"

Server* server = new Server();

int init(uint16_t port)
{
	if (startup())
		return STARTUP_ERROR;

	return server->init(port);
	stop();
}

int update()
{
	return server->update();
}

void stop()
{
	server->stop();
	shutdown();
}
