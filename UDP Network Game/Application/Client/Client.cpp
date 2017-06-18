// Client.cpp : This file handles all client network functions.
#include "Client.h"
#include "../NetworkMessage.h"

/// <summary>
/// Initializes the client.
/// Attempts to connect to the server and request a connection.
/// </summary>
/// <param name="address">Server's address</param>
/// <param name="port">Server's port</param>
/// <param name="_player">Player's ID</param>
/// <returns>int representing result of connection attempt : refer Definitions.h</returns>
int Client::init(char* address, uint16_t port, uint8_t _player)
{
	// First, set some default variables:
	state.player0.keyUp = state.player0.keyDown = false;
	state.player1.keyUp = state.player1.keyDown = false;
	state.gamePhase = WAITING;

	//       1) Set the player.
	player = _player;

	//       2) Set up the connection.
	// Create the Client Socket:
	clSocket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	if (INVALID_SOCKET == clSocket)
	{
		stop();
		return SETUP_ERROR;
	}
	// Create the Sockaddr to connect with: (using given address and port)
	SOCKADDR_IN sAddr;
	sAddr.sin_family = AF_INET;
	sAddr.sin_port = ntohs(port);
	sAddr.sin_addr.s_addr = inet_addr(address);
	if (INADDR_NONE == sAddr.sin_addr.s_addr || INADDR_ANY == sAddr.sin_addr.s_addr)
	{
		stop();
		return ADDRESS_ERROR;
	}

	//       3) Connect to the server.
	if (SOCKET_ERROR == connect(clSocket, (SOCKADDR*)&sAddr, sizeof(sAddr)))
	{
		stop();
		return DISCONNECT;
	}

	// NetworkMessage is a provided helper class.
	// It streamlines sending & receiving data through the Sockets.
	// Construct a "Want to connect" message:
	NetworkMessage msgOut(_OUTPUT);
	msgOut.writeByte(CL_CONNECT);
	msgOut.writeByte((int8_t)player);
	// Send the msg:
	if (SOCKET_ERROR == sendNetMessage(clSocket, msgOut))
	{
		stop();
		return DISCONNECT;
	}

	//       4) Get response from server.
	// If "Connect" was successfully sent, we're expecting a response.
	// Prepare a NetworkMessage object to receive it, and do so:
	NetworkMessage msgIn(_INPUT);
	if (SOCKET_ERROR == recvNetMessage(clSocket, msgIn))
	{
		stop();
		return DISCONNECT;
	}

	// Message received. Parse it:
	/* currSequence is our saved "Sequence Number."
	 * Tracking this makes sure we only update our local gamestate
	 * with NEW information from the server, and not outdated snapshots.
	*/
	currSequence = msgIn.readShort();
	uint8_t msgType = msgIn.readByte();
	switch (msgType)
	{
	case(SV_OKAY):
		// Connection success! Continue...
		break;
	case(SV_FULL):
		// The game is full. Shutdown and exit:
		stop();
		return SHUTDOWN;
		break;
	default:
		// something went wrong
		stop();
		return DISCONNECT;
		break;
	}

	//       5) Make sure to mark the client as running.
	active = true;
	state.gamePhase = RUNNING;

	/* snapshotCount is tracked so we know how often to remind the Server
	 * that we are still alive.
	 * We need to periodically send a message, or the Server
	 * will assume we've disconnected and stop the game.
	*/
	snapshotCount = currSequence;

	return SUCCESS;
}

/// <summary>
/// Receive and process messages from the server.
/// Once begun, run continues until an error or disconnect.
/// </summary>
/// <returns>int representing type of exit : refer Definitions.h</returns>
int Client::run()
{
	/* Continuously process messages from the server as long as the client is running.
	 * Once run is entered, a while loop is run continuously until an error or disconnect.
	 * Each time through the loop, we receive a NetworkMessage containing a game snapshot.
	 * Parse the message, save the snapshot, and track our progress.
	*/
	while (true)
	{
		// Prepare an object to receive the next snapshot:
		NetworkMessage msg(_INPUT);
		int result = recvNetMessage(clSocket, msg);
		if (result <= 0)
		{
			stop();
			return SHUTDOWN;
		}
		// Get the Sequence Number first. If this is lower than our current value,
		// then the incoming snapshot is old, and we don't want to use it for an update.
		uint16_t seqNum = msg.readShort();

		uint8_t msgType = msg.readByte();
		switch (msgType)
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

		case(SV_SNAPSHOT):
		{
			// Only use new snapshots for updates:
			if (seqNum > currSequence)
			{
				// Get the gamestate from this snapshot,
				// and update saved values:
				state.gamePhase = msg.readByte();
				state.ballX = msg.readShort();
				state.ballY = msg.readShort();
				state.player0.y = msg.readShort();
				state.player0.score = msg.readShort();
				state.player1.y = msg.readShort();
				state.player1.score = msg.readShort();

				// Update local variables:
				snapshotCount += (seqNum - currSequence);
				if (snapshotCount >= 10)
				{
					// Every 10 snapshots, we need to tell the Server we're still here:
					if (SOCKET_ERROR == sendAlive())
					{
						stop();
						return DISCONNECT;
					}
					snapshotCount -= 10;
				}

				// Important: make sure to update our local sequence number:
				currSequence = seqNum;
			}
			break;
		}
		case(SV_CL_CLOSE):
			stop();
			return SHUTDOWN;
			break;
		default:
			// Bad Message
			stop();
			return MESSAGE_ERROR;
			break;
		}
	}
	// Should never hit this....
	stop();
	return DISCONNECT;
}

/// <summary>
/// Clean up and shut down the client.
/// Before shutdown, this sends a Close message to the server.
/// </summary>
void Client::stop()
{
	//       1) Make sure to send a SV_CL_CLOSE message.
	sendClose();
	//       2) Make sure to mark the client as shutting down and close the socket.
	player = -1;
	shutdown(clSocket, SD_BOTH);
	closesocket(clSocket);
	//       3) Set the game phase to DISCONNECTED.
	state.gamePhase = DISCONNECTED;
}

/// <summary>
/// Send the player's input (up/down) to the server.
/// </summary>
/// <param name="keyUp">(bool) Is "up" pressed?</param>
/// <param name="keyDown">(bool) Is "down" pressed?</param>
/// <param name="keyQuit">(bool) Is "quit" pressed?</param>
/// <returns>int representing send result : refer Definitions.h</returns>
int Client::sendInput(int8_t keyUp, int8_t keyDown, int8_t keyQuit)
{
	if (keyQuit)
	{
		// Shutdown and close the Socket connection:
		stop();
		return SHUTDOWN;
	}

	// Update the gamestate (player positions) according to user inputs:
	cs.enter();
	if (player == 0)
	{
		state.player0.keyUp = keyUp;
		state.player0.keyDown = keyDown;
	}
	else
	{
		state.player1.keyUp = keyUp;
		state.player1.keyDown = keyDown;
	}
	cs.leave();

	// Transmit the player's input status to Server.
	// First, construct a NetworkMessage with the local player's gamestate:
	NetworkMessage msg(_OUTPUT);
	msg.writeByte(CL_KEYS);
	if (player == 0)
	{
		msg.writeByte(state.player0.keyUp);
		msg.writeByte(state.player0.keyDown);
	}
	else if (player == 1)
	{
		msg.writeByte(state.player1.keyUp);
		msg.writeByte(state.player1.keyDown);
	}
	// Now, send the message. Continue on success, exit on failure:
	if (SOCKET_ERROR == sendNetMessage(clSocket, msg))
	{
		stop();
		return DISCONNECT;
	}

	return SUCCESS;
}

/// <summary>
/// Copies the current state into the struct pointed to by target.
/// </summary>
void Client::getState(GameState* target)
{
	/* My note: this is a "helper" method that I never used.
	 * This could be called to assist in sending the local gamestate to the Server.
	*/
	*target = state;
}

/// <summary>
/// Sends a SV_CL_CLOSE message to the server (private, suggested)
/// </summary>
void Client::sendClose()
{
	/* This constructs a simple "I'm shutting down" NetworkMessage
	 * and sends it to the Server.
	 * This method is called whenever Client needs to disconnect,
	 * in order to do so gracefully.
	 * (instead of making the server wait for a timeout)
	*/
	NetworkMessage msg(_OUTPUT);
	msg.writeByte(SV_CL_CLOSE);
	sendNetMessage(clSocket, msg);
}

/// <summary>
/// Sends a CL_ALIVE message to the server (private, suggested)
/// </summary>
/// <returns>int representing send result : refer Definitions.h</returns>
int Client::sendAlive()
{
	/* This constructs a simple "I'm alive" NetworkMessage
	 * and sends it to the Server.
	 * This method is called periodically
	 * (whenever we've received >=10 snapshots)
	 * in order to prevent from being assumed missing and auto-disconnected.
	*/
	NetworkMessage msg(_OUTPUT);
	msg.writeByte(CL_ALIVE);

	if (SOCKET_ERROR == sendNetMessage(clSocket, msg))
	{
		return SOCKET_ERROR;
	}
	return SUCCESS;
}
