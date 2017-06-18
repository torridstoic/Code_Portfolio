#ifndef _CLIENT_WRAPPER_H_
#define _CLIENT_WRAPPER_H_

#include "../platform.h"
#include "../definitions.h"

EXPORTED int init(char* address, uint16_t port, uint8_t player);
EXPORTED int run();
EXPORTED void stop();

EXPORTED int sendInput(int8_t keyUp, int8_t keyDown, int8_t keyQuit);
EXPORTED void getState(GameState* target);

#endif
