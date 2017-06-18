#pragma once
#include <iostream>
#include <fstream>
#include <vector>
using namespace std;


#pragma region BitStream Class Headers
class BitIStream
{
private:
	// Members
	ifstream bin;
	char buffer;
	int activeBit;

public:
	// Ctor/Dtor
	BitIStream(const char* filePath, char* headerChunk = NULL, int numberOfBytes = 0);
	~BitIStream();

	// Operators/Functions
	BitIStream& operator>>(int &bit);
	bool eof();
	void close();
};

class BitOStream
{
private:
	// Members
	ofstream bout;
	char buffer;
	int activeBit;

public:
	// Ctor/Dtor
	BitOStream(const char* filePath, const char* headerChunk = NULL, int numberOfBytes = 0);
	~BitOStream();

	// Operators/Functions
	BitOStream& operator<<(vector<int> &bits);
	void close();
};
#pragma endregion


#pragma region BitIStream Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : constructor
// Parameters : 	filePath - the path of the file to open for input
//			headerChunk - a chunk of data to be read from the 
//					beginning of the file
//			numberOfBytes - the number of bytes of header information 
//					to read in
/////////////////////////////////////////////////////////////////////////////
BitIStream::BitIStream(const char* filePath, char* headerChunk, int numberOfBytes)
{
	activeBit = 7;
	bin.open(filePath, ios::binary);

	// Header Input
	bin.read(headerChunk, numberOfBytes);

	// Read file's first byte
	bin.read(&buffer, 1);
}

/////////////////////////////////////////////////////////////////////////////
// Function : destructor
/////////////////////////////////////////////////////////////////////////////
BitIStream::~BitIStream()
{
	if (bin.is_open())
		bin.close();
}

/////////////////////////////////////////////////////////////////////////////
// Function : extraction operator
// Parameters : bit - store the next bit here
// Return : BitIStream& - the stream (allows for daisy-chaining extractions)
/////////////////////////////////////////////////////////////////////////////
BitIStream& BitIStream::operator>>(int &bit)
{
	if (activeBit < 0)
	{
		// Wrap around if the whole byte is read,
		// and grab a new one from the file
		activeBit = 7;
		bin.read(&buffer, 1);
		if (bin.eof())
			return *this;
	}

	// Copy the next active bit of buffer into "bit"
	if (buffer & (1 << activeBit))
		bit = 1;
	else
		bit = 0;

	// Next active bit...
	--activeBit;

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : eof
// Return : true if we are at the end of the file, false otherwise
// Notes : should only return true when we have given the user every byte
//		from the file and every bit from the buffer
/////////////////////////////////////////////////////////////////////////////
bool BitIStream::eof()
{
	if (7 == activeBit && bin.eof())
		return true;
	//else
	return false;
}

/////////////////////////////////////////////////////////////////////////////
// Function : close
// Notes : close the file
/////////////////////////////////////////////////////////////////////////////
void BitIStream::close()
{
	bin.close();
}
#pragma endregion

#pragma region BitOStream Definitions
/////////////////////////////////////////////////////////////////////////////// Function : constructor
// Parameters : 	filePath - the path of the file to open for output
//			headerChunk - a chunk of data to be written at the 
//					beginning of the file
//			numberOfBytes - the number of bytes of header information 
//					to write out
/////////////////////////////////////////////////////////////////////////////
BitOStream::BitOStream(const char* filePath, const char* headerChunk, int numberOfBytes)
{
	activeBit = 7;
	buffer = 0;
	bout.open(filePath, ios::binary);

	// Header output
	if (headerChunk)
		bout.write((char*)headerChunk, numberOfBytes);
}

/////////////////////////////////////////////////////////////////////////////
// Function : destructor
/////////////////////////////////////////////////////////////////////////////
BitOStream::~BitOStream()
{
	if (bout.is_open())
		close();
}

/////////////////////////////////////////////////////////////////////////////
// Function : insertion operator
// Parameters : bits - a vector containing some number of 1's and 0's to 
//						stream out to the file
// Return : BitOStream& - the stream (allows for daisy-chaining insertions)
////////////////////////////////////////////////////////////////////////////
BitOStream& BitOStream::operator<<(vector<int> &bits)
{
	for (unsigned int i = 0; i < bits.size(); ++i)
	{
		// Copy each bit into the buffer
		buffer |= bits[i] << activeBit;

		--activeBit;
		if (activeBit < 0)
		{
			// When buffer is full, write it out and reset
			bout.write(&buffer, 1);
			buffer = 0;
			activeBit = 7;
		}
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : close
// Notes : closes the file stream - if remaining bits exist, they are written
//			to the file with trailing 0's. if no remaining bits exist, 
//			simply close the file
/////////////////////////////////////////////////////////////////////////////
void BitOStream::close()
{
	// If buffer isn't empty, write what's there before closing
	if (activeBit != 7)
		bout.write(&buffer, 1);

	bout.close();
}
#pragma endregion