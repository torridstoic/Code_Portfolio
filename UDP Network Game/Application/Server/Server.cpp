// Server.cpp : Contains all functions of the server.
#include "Server.h"

/// <summary>
/// Initializes a server and prepares to accept connections.
/// (NOTE: Listens, but does not wait for player connections!)
/// </summary>
/// <returns>int representing connection success : refer Definitions.h</returns>
int Server::init(uint16_t port)
{
	// Setup the gamestate:
	initState();

	//       1) Set up a socket for listening.
	// Make a Server Socket:
	svSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	u_long optVal = 1;
	// Set the Socket to Nonblocking mode.
	// (we do NOT want to block while waiting):
	if (INVALID_SOCKET == svSocket ||
		SOCKET_ERROR == ioctlsocket(svSocket, FIONBIO, &optVal))
	{
		stop();
		return SETUP_ERROR;
	}

	// Create a Sockaddr and Bind it:
	SOCKADDR_IN sAddr;
	sAddr.sin_family = AF_INET;
	sAddr.sin_port = htons(port);
	sAddr.sin_addr.s_addr = INADDR_ANY;
	if (SOCKET_ERROR == bind(svSocket, (SOCKADDR*)&sAddr, sizeof(sAddr)))
	{
		stop();
		return BIND_ERROR;
	}

	//       2) Mark the server as active.
	active = true;
	for (uint16_t n : currSequence)
	{
		// Initialize currSequence for both players to 0:
		n = 0;
	}
	//state.gamePhase = WAITING;

	return SUCCESS;
}

/// <summary>
/// Updates the server.
/// Reads & parses messages and updates the gamestate.
/// This is repeatedly called each server "tick".
/// </summary>
/// <returns>int representing status : refer Definitions.h</returns>
int Server::update()
{
	if (!active) { return SHUTDOWN; }

	//        1) Get player input and process it.
	// Create a NetworkMessage object and a Sockaddr,
	// then receive the incoming message:
	SOCKADDR_IN sAddrInc;
	NetworkMessage msg(_INPUT);
	int errCode = recvfromNetMessage(svSocket, msg, &sAddrInc);

	// Call the "parseMessage" method to read & respond.
	// Break out if we received: SOCKET_ERROR, DISCONNECT, or MESSAGE_ERROR:
	if (SOCKET_ERROR != errCode)
	{
		int result = parseMessage(sAddrInc, msg);
		if (DISCONNECT == result || MESSAGE_ERROR == result)
		{
			return result;
		}
	}
	else if (WSAGetLastError() != EWOULDBLOCK)
	{
		stop();
		return DISCONNECT;
	}

	//        2) If any player's timer exceeds 50, "disconnect" the player.
	// Clients should be sending keep_alive messages every 10 ticks.
	// If we haven't received one in 50, assume they've timed out.
	for (int i = 0; i < 2; ++i)
	{
		if (playerTimer[i] > 50)
		{
			disconnectClient(i);
		}
	}

	//        3) Update the state and send the current snapshot to each player.
	updateState();
	if (SOCKET_ERROR == sendState())
	{
		stop();
		return DISCONNECT;
	}

	return (active) ? SUCCESS : SHUTDOWN;
}

/// <summary>
/// (Gracefully) stops the server.
/// Also notifies all Clients of disconnect.
/// </summary>
void Server::stop()
{
	//       1) Sends a "close" message to each client.
	sendClose();
	//       2) Shuts down the server gracefully (update method should exit with SHUTDOWN code.)
	active = false;
	shutdown(svSocket, SD_BOTH);
	closesocket(svSocket);
}

/// <summary>
/// Parses a message and responds if necessary. (private, suggested)
/// </summary>
/// <param name="source">(memaddress of) SockAddr of message source</param>
/// <param name="message">(memaddress of) incoming message to parse</param>
/// <returns>int representing success/failure type : refer Definitions.h</returns>
int Server::parseMessage(sockaddr_in& source, NetworkMessage& message)
{
	/* MESSAGE TYPES:
	 * 1 = CL_CONNECT: Client request to connect to the server.
	 * 2 = CL_KEYS: Informs the client of the current state of the player's input.
	 * 3 = CL_ALIVE: Notifies the server that the client is still there.
	 * 4 = SV_OKAY: Informs the client that it has successfully connected to the server.
	 * 5 = SV_FULL: Informs the client that the server is full and that it cannot connect.
	 * 6 = SV_SNAPSHOT: Provides client with most recent game state.
	 * 7 = SV_CL_CLOSE: Informs the receiver of a disconnect (from client or server).
	*/

	// The first byte of the message determines the message type:
	uint8_t type = message.readByte();
	switch (type)
	{
	// Message type: Connection request:
	case(CL_CONNECT):
	{
		// Read the first byte. This is the client's ID:
		uint8_t id = message.readByte();
		if (INADDR_NONE == playerAddress[id].sin_addr.s_addr)
		{
			// Here, INADDR_NONE represents an empty slot.
			// There is space for a new client,
			// so connect him and init/reset his Sequence Number to 0:
			connectClient(id, source);
			currSequence[id] = 0;

			// Send the client a confirmation/okay message:
			if (SOCKET_ERROR == sendOkay(source))
			{
				stop();
				return DISCONNECT;
			}
		}
		else
		{
			// No space for client.
			// Send him a "game is full" rejection message:
			if (SOCKET_ERROR == sendFull(source))
			{
				stop();
				return DISCONNECT;
			}
		}
		break;
	}
	// Message type: Player input:
	case(CL_KEYS):
	{
		// Check the source SockAddr against stored players, and
		// update the appropriate player's position in the gamestate:
		if (playerAddress[0].sin_port == source.sin_port &&
			playerAddress[0].sin_addr.s_addr == source.sin_addr.s_addr)
		{
			state.player0.keyUp = message.readByte();
			state.player0.keyDown = message.readByte();
		}
		else if (playerAddress[1].sin_port == source.sin_port &&
			playerAddress[1].sin_addr.s_addr == source.sin_addr.s_addr)
		{
			state.player1.keyUp = message.readByte();
			state.player1.keyDown = message.readByte();
		}
		break;
	}
	// Message type: KeepAlive message:
	case(CL_ALIVE):
	{
		for (int i = 0; i < 2; ++i)
		{
			// Check the source SockAddr against stored players, and
			// reset the appropriate player's timeout countdown:
			if (playerAddress[i].sin_port == source.sin_port &&
				playerAddress[i].sin_addr.s_addr == source.sin_addr.s_addr)
			{
				playerTimer[i] = 0;
			}
		}
		break;
	}
	// Message type: Client is disconnecting:
	case(SV_CL_CLOSE):
	{
		for (int i = 0; i < 2; ++i)
		{
			// Check the source SockAddr against stored players,
			// and disconnect/remove the appropriate player:
			if (playerAddress[i].sin_port == source.sin_port &&
				playerAddress[i].sin_addr.s_addr == source.sin_addr.s_addr)
			{
				// The disconnectClient helper method handles 
				// clearing the player's data and updating the server gamestate:
				disconnectClient(i);
			}
		}
		break;
	}
	default:
		// Bad Message
		stop();
		return MESSAGE_ERROR;
		break;
	}

	return SUCCESS;
}

/// <summary>
/// Sends the "SV_OKAY" message to destination. (private, suggested)
/// </summary>
/// <param name="destination">(memaddress of) destination for message</param>
/// <returns>int: SOCKET_ERROR or SUCCESS : refer Definitions.h</returns>
int Server::sendOkay(sockaddr_in& destination)
{
	// Send "SV_OKAY" to the destination.
	NetworkMessage msg(_OUTPUT);
	msg.writeShort((int16_t)0); // fake sequence num (meaningless)
	msg.writeByte(SV_OKAY);
	return (SOCKET_ERROR == sendtoNetMessage(svSocket, msg, &destination))
		? SOCKET_ERROR : SUCCESS;
}

/// <summary>
/// Sends the "SV_FULL" message to destination. (private, suggested)
/// </summary>
/// <param name="destination">(memaddress of)destination for message</param>
/// <returns>int: SOCKET_ERROR or SUCCESS : refer Definitions.h</returns>
int Server::sendFull(sockaddr_in& destination)
{
	// Send "SV_FULL" to the destination.
	NetworkMessage msg(_OUTPUT);
	msg.writeShort((int16_t)0); // fake sequence num (meaningless)
	msg.writeByte(SV_FULL);
	return (SOCKET_ERROR == sendtoNetMessage(svSocket, msg, &destination))
		? SOCKET_ERROR : SUCCESS;
}

/// <summary>
/// Sends the current snapshot to all players. (private, suggested)
/// </summary>
/// <returns>int: SOCKET_ERROR or SUCCESS : refer Definitions.h</returns>
int Server::sendState()
{
	// For all connected clients/players:
	for (int i = 0; i < 2; ++i)
	{
		if (INADDR_NONE != playerAddress[i].sin_addr.s_addr)
		{
			// Copy current state to a NetMsg buffer
			NetworkMessage msg(_OUTPUT);
			msg.writeShort(currSequence[i]++);
			msg.writeByte(SV_SNAPSHOT);
			msg.writeByte(state.gamePhase);
			msg.writeShort(state.ballX);
			msg.writeShort(state.ballY);
			msg.writeShort(state.player0.y);
			msg.writeShort(state.player0.score);
			msg.writeShort(state.player1.y);
			msg.writeShort(state.player1.score);

			// Send the NetMsg to each player
			if (SOCKET_ERROR == sendtoNetMessage(svSocket, msg, &playerAddress[i]))
			{
				return SOCKET_ERROR;
			}
		}
	}
	return SUCCESS;
}

/// <summary>
/// Sends the "SV_CL_CLOSE" message to all clients. (private, suggested)
/// This is used prior to disconnecting the server,
/// so message-sending failures don't need to be logged/returned.
/// </summary>
void Server::sendClose()
{
	// For all connected clients/players:
	for (int i = 0; i < 2; ++i)
	{
		if (INADDR_NONE != playerAddress[i].sin_addr.s_addr)
		{
			// Construct a "disconnecting" message, and send it:
			NetworkMessage msg(_OUTPUT);
			msg.writeShort(currSequence[i]++);
			msg.writeByte(SV_CL_CLOSE);
			sendtoNetMessage(svSocket, msg, &playerAddress[i]);

			// This helper method handles clearing the client's data
			// and updating the server gamestate:
			disconnectClient(i);
		}
	}
}

// Server message-sending helper method. (private, suggested)
int Server::sendMessage(sockaddr_in& destination, NetworkMessage& message)
{
	// Send the message in the buffer to the destination.
	return sendtoNetMessage(svSocket, message, &destination);
}

/// <summary>
/// Marks a client as connected and adjusts the game state.
/// </summary>
/// <param name="player">Player's ID, sent on his connection request</param>
/// <param name="source">(memaddress of)SockAddr source of client</param>
void Server::connectClient(int player, sockaddr_in& source)
{
	// Save the player's SockAddr
	playerAddress[player] = source;
	// Initialize/reset the player's timeout counter
	playerTimer[player] = 0;

	// Update the gamestate according to playercount
	noOfPlayers++;
	if (noOfPlayers == 1)
		state.gamePhase = WAITING;
	else
		state.gamePhase = RUNNING;
}

/// <summary>
/// Marks a client as disconnected and adjusts the game state.
/// </summary>
/// <param name="player">ID of player to disconnect</param>
void Server::disconnectClient(int player)
{
	// Clear the playerslot's SockAddr (port & address)
	playerAddress[player].sin_addr.s_addr = INADDR_NONE;
	playerAddress[player].sin_port = 0;

	// Update the gamestate according to playercount
	noOfPlayers--;
	if (noOfPlayers == 1)
		state.gamePhase = WAITING;
	else
		state.gamePhase = DISCONNECTED;
}

/// <summary>
/// Updates the state of the game.
/// This gets called once per tick,
/// and updates according to player input.
/// The resulting gamestate is used to update the clients.
/// </summary>
void Server::updateState()
{
	// Tick counter.
	static int timer = 0;

	// Update the tick counter.
	timer++;

	// Next, update the game state.
	if (state.gamePhase == RUNNING)
	{
		// Update the player tick counters (for ALIVE messages.)
		playerTimer[0]++;
		playerTimer[1]++;

		// Update the positions of the player paddles
		if (state.player0.keyUp)
			state.player0.y -= 3;
		if (state.player0.keyDown)
			state.player0.y += 3;

		if (state.player1.keyUp)
			state.player1.y -= 3;
		if (state.player1.keyDown)
			state.player1.y += 3;

		// Make sure the paddle new positions are within the bounds.
		if (state.player0.y < 0)
			state.player0.y = 0;
		else if (state.player0.y > FIELDY - PADDLEY)
			state.player0.y = FIELDY - PADDLEY;

		if (state.player1.y < 0)
			state.player1.y = 0;
		else if (state.player1.y > FIELDY - PADDLEY)
			state.player1.y = FIELDY - PADDLEY;

		//just in case it get stuck...
		if (ballVecX)
			storedBallVecX = ballVecX;
		else
			ballVecX = storedBallVecX;

		if (ballVecY)
			storedBallVecY = ballVecY;
		else
			ballVecY = storedBallVecY;

		state.ballX += ballVecX;
		state.ballY += ballVecY;

		// Check for paddle collisions & scoring
		if (state.ballX < PADDLEX)
		{
			// If the ball has struck the paddle...
			if (state.ballY + BALLY > state.player0.y && state.ballY < state.player0.y + PADDLEY)
			{
				state.ballX = PADDLEX;
				ballVecX *= -1;
			}
			// Otherwise, the second player has scored.
			else
			{
				newBall();
				state.player1.score++;
				ballVecX *= -1;
			}
		}
		else if (state.ballX >= FIELDX - PADDLEX - BALLX)
		{
			// If the ball has struck the paddle...
			if (state.ballY + BALLY > state.player1.y && state.ballY < state.player1.y + PADDLEY)
			{
				state.ballX = FIELDX - PADDLEX - BALLX - 1;
				ballVecX *= -1;
			}
			// Otherwise, the first player has scored.
			else
			{
				newBall();
				state.player0.score++;
				ballVecX *= -1;
			}
		}

		// Check for Y position "bounce"
		if (state.ballY < 0)
		{
			state.ballY = 0;
			ballVecY *= -1;
		}
		else if (state.ballY >= FIELDY - BALLY)
		{
			state.ballY = FIELDY - BALLY - 1;
			ballVecY *= -1;
		}
	}

	// If the game is over...
	if ((state.player0.score > 10 || state.player1.score > 10) && state.gamePhase == RUNNING)
	{
		state.gamePhase = GAMEOVER;
		timer = 0;
	}
	if (state.gamePhase == GAMEOVER)
	{
		if (timer > 30)
		{
			initState();
			state.gamePhase = RUNNING;
		}
	}
}

/// <summary>
/// Initializes the state of the game.
/// </summary>
void Server::initState()
{
	playerAddress[0].sin_addr.s_addr = INADDR_NONE;
	playerAddress[1].sin_addr.s_addr = INADDR_NONE;
	playerTimer[0] = playerTimer[1] = 0;
	noOfPlayers = 0;

	state.gamePhase = DISCONNECTED;

	state.player0.y = 0;
	state.player1.y = FIELDY - PADDLEY - 1;
	state.player0.score = state.player1.score = 0;
	state.player0.keyUp = state.player0.keyDown = false;
	state.player1.keyUp = state.player1.keyDown = false;

	newBall(); // Get new random ball position

	// Get a new random ball vector that is reasonable
	ballVecX = (rand() % 10) - 5;
	if ((ballVecX >= 0) && (ballVecX < 2))
		ballVecX = 2;
	if ((ballVecX < 0) && (ballVecX > -2))
		ballVecX = -2;

	ballVecY = (rand() % 10) - 5;
	if ((ballVecY >= 0) && (ballVecY < 2))
		ballVecY = 2;
	if ((ballVecY < 0) && (ballVecY > -2))
		ballVecY = -2;


}

/// <summary>
/// Places the ball randomly within the middle half of the field.
/// </summary>
void Server::newBall()
{
	// (randomly in 1/2 playable area) + (1/4 playable area) + (left buffer) + (half ball)
	state.ballX = (rand() % ((FIELDX - 2 * PADDLEX - BALLX) / 2)) +
		((FIELDX - 2 * PADDLEX - BALLX) / 4) + PADDLEX + (BALLX / 2);

	// (randomly in 1/2 playable area) + (1/4 playable area) + (half ball)
	state.ballY = (rand() % ((FIELDY - BALLY) / 2)) + ((FIELDY - BALLY) / 4) + (BALLY / 2);
}
