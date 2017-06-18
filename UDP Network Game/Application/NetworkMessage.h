// NetworkMessage.h : Implements a message class for UDP send & receive.

#ifndef _NETWORK_MESSAGE_H_
#define _NETWORK_MESSAGE_H_

#include "stdint.h"
#include "platform.h"
#include "exceptions.h"

// The size of the data buffer.
#define MESSAGE_SIZE 512

// Enumeration for input & output flush / clear.
enum IO { _INPUT, _OUTPUT };

// Stream for sending & receiving network data.
class NetworkMessage
{
    private:
        // Data storage buffers.
        char dataBuffer[MESSAGE_SIZE];

		// Tracks the beginning and ending of the data.
		int begin, end;

		// Marks the message as a reading or writing message.
		IO type;

    public:
        // Constructor.
        NetworkMessage(IO _type);

		// Returns the size of the buffer in bytes.
		int getBufferSize();

		// Gets the data's buffer so a message can be read into it.
		char* getReadBuffer();

		// Gets the data's buffer so that the data within it can be sent.
		const char* getSendBuffer();

		// Get the length of the message.
		int getLength();

		// Set the end of the message (only used after receiving a message.)
		void setEnd(int _end);

		// Reads length bytes into data from message, starting at offset.
		// (Returns bytes read.)
 		int read(char* data, int offset, int length);

		// Reads length bytes into data from message until a null terminator
		// is read (returns bytes read.)
		int readString(char* data, int length);

		// Will read a data type from the message and return it.
		int8_t readByte();
        int16_t readShort();
		int32_t readInt();

		// Writes length bytes into message from data, starting at offset.
		// (Returns bytes written.)
		void write(const char* data, int offset, int length);

		// Writes bytes into message from data until a null terminator
		// is written to the buffer.
		void writeString(const char *data);
		void writeString(const char *data, int length);

		// Will write a data type to the message buffer without sending it.
		void writeByte(int8_t output);
        void writeShort(int16_t output);
        void writeInt(int32_t output);

		// Clears all data in the data buffer.
		void reset(IO _type);

		// Clears all data in the data buffer and resets the type.
		void reset();

		// Returns the number of bytes available (for reading or writing.)
		int bytesAvailable();


	private:
		// Simplified functions for reading / writing raw data to the buffer.
		int rawRead(uint8_t* data, int length, bool partialOkay);
		void rawWrite(const uint8_t* data, int length);

		// Robust functions for reading / writing raw data to the buffer.
		int rawRead(uint8_t* data, int length, int delimiter, bool partialOkay);
		void rawWrite(const uint8_t* data, int length, int delimiter);
};

/***********************************************
 **                                           **
 **             Inline Functions              **
 **                                           **
 ***********************************************/

inline int NetworkMessage::rawRead(uint8_t* data, int length, bool partialOkay)
{
	return rawRead(data, length, -1, partialOkay);
}

inline void NetworkMessage::rawWrite(const uint8_t* data, int length)
{
	rawWrite(data, length, -1);
}

/***********************************************
 **                                           **
 **             Helper Functions              **
 **                                           **
 ***********************************************/
int recvNetMessage(SOCKET s, NetworkMessage& messages);
int recvfromNetMessage(SOCKET s, NetworkMessage& message, sockaddr_in* source);

int sendNetMessage(SOCKET s, NetworkMessage& message);
int sendtoNetMessage(SOCKET s, NetworkMessage& message, const sockaddr_in* source);

#endif

