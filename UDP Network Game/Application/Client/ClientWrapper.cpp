#include "ClientWrapper.h"
#include "Client.h"

Client* client = new Client();

int init(char* address, uint16_t port, uint8_t player)
{
	if (startup())
		return STARTUP_ERROR;

	return client->init(address, port, player);
	stop();
}

int run()
{
	return client->run();
}

void stop()
{
	client->stop();
	shutdown();
}

int sendInput(int8_t keyUp, int8_t keyDown, int8_t keyQuit)
{
	return client->sendInput(keyUp, keyDown, keyQuit);
}

void getState(GameState* target)
{
	return client->getState(target);
}
