#include "platform.h"

#ifndef _DEFINITIONS_H_
#define _DEFINITIONS_H_

// Game Phases
const uint8_t DISCONNECTED = 0, WAITING = 1, RUNNING = 2, GAMEOVER = 3;

// Message Types
const uint8_t CL_CONNECT = 1, CL_KEYS = 2, CL_ALIVE = 3,
              SV_OKAY = 4, SV_FULL = 5, SV_SNAPSHOT = 6, SV_CL_CLOSE = 7;

// Return Values
const uint8_t SUCCESS = 0, SHUTDOWN = 1, DISCONNECT = 2,
              BIND_ERROR = 3, CONNECT_ERROR = 4, SETUP_ERROR = 5,
              STARTUP_ERROR = 6, ADDRESS_ERROR = 7, PARAMETER_ERROR = 8, MESSAGE_ERROR = 9;

struct Player
{
	int16_t y;
	int16_t score;

	// NOTE: These are boolean variables, but bool is platform dependent.
	int8_t keyUp;
	int8_t keyDown;
};

struct GameState 
{
	uint8_t gamePhase;

	int16_t ballX;
	int16_t ballY;

	Player player0;
	Player player1;
};

#define PADDLEX 64
#define PADDLEY 192
#define BALLX 64
#define BALLY 64
#define FIELDX 640
#define FIELDY 480

#define MAX_DATA 512
#endif
