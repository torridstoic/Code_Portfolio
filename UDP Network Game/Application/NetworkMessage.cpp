// NetworkMessage.cpp : Implements a message class for UDP send & receive.

#include "NetworkMessage.h"

// Constructor.
NetworkMessage::NetworkMessage(IO _type)
{
	type = _type;
	begin = end = 0;
}

// Returns the size of the buffer in bytes.
int NetworkMessage::getBufferSize()
{
	return MESSAGE_SIZE;
}

// Returns a pointer to the buffer (for reading.)
char* NetworkMessage::getReadBuffer()
{
	if (type == _OUTPUT)
		throw NetMessageException("Tried to get read buffer for output message NetworkMessage.getReadBuffer()");

	return dataBuffer;
}

// Returns a pointer to the buffer (for sending.)
const char* NetworkMessage::getSendBuffer()
{
	if (type == _INPUT)
		throw NetMessageException("Tried to get send buffer for input message NetworkMessage.getSendBuffer()");

	return dataBuffer;
}

// Get the length of the message.
int NetworkMessage::getLength()
{
	return end;
}

// Set the end of the message (only used after receiving a message.)
void NetworkMessage::setEnd(int _end)
{
	if (type == _OUTPUT)
		throw NetMessageException("Tried to set end of output message NetworkMessage.getReadBuffer()");

	end = _end;
}

// Functions for reading data from the message.
int NetworkMessage::read(char* data, int offset, int length)
{
	return rawRead((uint8_t*) data+offset, length, true);
}

int NetworkMessage::readString(char* data, int length)
{
	return rawRead((uint8_t*) data, length, 0, true);
}

char NetworkMessage::readByte()
{
	char result;
	rawRead((uint8_t*) &result, 1, false);
	return result;
}

int16_t NetworkMessage::readShort()
{
	int16_t result;
	rawRead((uint8_t*) &result, 2, false);
	return ntohs(result);
}

int32_t NetworkMessage::readInt()
{
	int32_t result;
	rawRead((uint8_t*) &result, 4, false);
	return ntohl(result);
}

// Methods from writing data to the message.
void NetworkMessage::write(const char* data, int offset, int length)
{
	rawWrite((uint8_t*) data+offset, length);
}

void NetworkMessage::writeString(const char *data)
{
	rawWrite((uint8_t*) data, (int) strlen(data) + 1, 0);
}

void NetworkMessage::writeString(const char *data, int length)
{
	rawWrite((uint8_t*) data, length, 0);
}

void NetworkMessage::writeByte(char output)
{
	rawWrite((uint8_t*) &output, 1);
}

void NetworkMessage::writeShort(int16_t output)
{
	output = htons(output);
	rawWrite((uint8_t*) &output, 2);
}

void NetworkMessage::writeInt(int32_t output)
{
	output = htonl(output);
	rawWrite((uint8_t*) &output, 4);
}

// Wipe the buffer contents.
void NetworkMessage::reset()
{
	begin = end = 0;
}

// Wipe the buffer contents and reset the type.
void NetworkMessage::reset(IO _type)
{
	type = _type;
	begin = end = 0;
}

// Get the number of bytes available in the data buffer.
//
// For OUTPUT, returns the number of empty bytes in the send buffer.
// For INPUT, returns the number of bytes available to be read from
// the receive buffer.
int NetworkMessage::bytesAvailable()
{
	if (type == _INPUT)
		return end - begin;
	else if (type == _OUTPUT)
		return MESSAGE_SIZE - end;
	else
		throw NetMessageException("Unknown message type in NetworkMessage.bytesAvailable()");
}

// Writing raw data to and from the stream buffer.
int NetworkMessage::rawRead(uint8_t* data, int length, int delimiter, bool partialOkay)
{
	if (type == _OUTPUT)
		throw NetMessageException("Tried to read data from output message in NetworkMessage.rawRead()");

	int bytes = 0;
	for (int index = begin; bytes != length; index++)
	{
		if (index == end)
		{
			if (partialOkay)
				break;
			else
				throw NetMessageException("Tried to read past end of data in NetworkMessage.rawRead()");
		}

		data[bytes] = dataBuffer[index];
		bytes++;

		if (data[bytes - 1] == delimiter)
			break;
	}

	begin += bytes;

	return bytes;
}

void NetworkMessage::rawWrite(const uint8_t* data, int length, int delimiter)
{
	if (type == _INPUT)
		throw NetMessageException("Tried to write data to input message in NetworkMessage.rawWrite()");

	for (int bytes = 0; bytes != length; bytes++)
	{
		if (end >= MESSAGE_SIZE)
			throw NetMessageException("Tried to write past end of buffer in NetworkMessage.rawWrite()");

		dataBuffer[end] = data[bytes];
		end++;

		if (data[bytes] == delimiter)
			break;
	}
}

int recvNetMessage(SOCKET s, NetworkMessage& message)
{
	int result;

	result = recv(s, message.getReadBuffer(), message.getBufferSize(), 0);

	if (result > 0)
	{
		message.reset();
		message.setEnd(result);
	}

	return result;
}

int recvfromNetMessage(SOCKET s, NetworkMessage& message, sockaddr_in* source)
{
	int result;
	int addrLen = sizeof(sockaddr_in);

	result = recvfrom(s, message.getReadBuffer(), message.getBufferSize(), 0, (sockaddr*) source, &addrLen);

	if (result > 0)
	{
		message.reset();
		message.setEnd(result);
	}

	return result;
}

int sendNetMessage(SOCKET s, NetworkMessage& message)
{
	return send(s, message.getSendBuffer(), message.getLength(), 0);
}

int sendtoNetMessage(SOCKET s, NetworkMessage& message, const sockaddr_in* source)
{
	int addrLen = sizeof(sockaddr_in);

	return sendto(s, message.getSendBuffer(), message.getLength(), 0, (sockaddr*) source, addrLen);
}
