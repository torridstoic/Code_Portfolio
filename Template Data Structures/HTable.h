#pragma once
#include "SLList.h"


#pragma region HTable Header
template <typename Type>
class HTable
{
private:
	unsigned int mBuckets;
	unsigned int(*mHashFunc)(const Type&);
	SLList<Type> *mArray;

public:
	// Ctor
	HTable(unsigned int numOfBuckets, unsigned int(*hFunction)(const Type &v));
	
	// Trilogy
	~HTable();
	HTable<Type>& operator=(const HTable<Type>& that);
	HTable(const HTable<Type>& that);

	// Functions
	void insert(const Type& v);
	bool findAndRemove(const Type& v);
	void clear();
	int find(const Type& v) const;
	void printSomeStuff(const char* filePath = "hashdata.txt");
};
#pragma endregion


#pragma region HTable Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constructor 
// Parameters : numOfBuckets - the number of buckets
//              hFunction - pointer to the hash function for this table
// Notes : constructs an empty hash table
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
HTable<Type>::HTable(unsigned int numOfBuckets, unsigned int(*hFunction)(const Type &v))
{
	mBuckets = numOfBuckets;
	mHashFunc = hFunction;
	mArray = new SLList<Type>[mBuckets];
}

/////////////////////////////////////////////////////////////////////////////
// Function : Destructor
// Notes : destroys a hash table
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
HTable<Type>::~HTable()
{
	delete[] mArray;
}

/////////////////////////////////////////////////////////////////////////////
// Function : Assignment Operator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
HTable<Type>& HTable<Type>::operator=(const HTable<Type>& that)
{
	// Paranoia Check
	if (this != &that)
	{
		// Clean up current list
		delete[] mArray;

		// Copy data
		mBuckets = that.mBuckets;
		mHashFunc = that.mHashFunc;
		mArray = new SLList<Type>[mBuckets];

		// Copy Lists
		for (unsigned int i = 0; i < mBuckets; ++i)
			mArray[i] = that.mArray[i];
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : Copy Constructor
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
HTable<Type>::HTable(const HTable<Type>& that)
{
	// Copy data
	mBuckets = that.mBuckets;
	mHashFunc = that.mHashFunc;
	mArray = new SLList<Type>[mBuckets];
	
	// Copy Lists
	for (unsigned int i = 0; i < mBuckets; ++i)
		mArray[i] = that.mArray[i];
}

/////////////////////////////////////////////////////////////////////////////
// Function : insert
// Parameters : v - the item to insert into the hash table
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void HTable<Type>::insert(const Type& v)
{
	mArray[mHashFunc(v)].addHead(v);
}

/////////////////////////////////////////////////////////////////////////////
// Function : findAndRemove
// Parameters : v - the item to remove(if it is found)
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
bool HTable<Type>::findAndRemove(const Type& v)
{
	// Find the bucket, point an iter at it
	unsigned int bucket = mHashFunc(v);
	SLLIter<Type> iter(mArray[bucket]);
	iter.begin();

	// Iterate through the bucket
	while (!iter.end())
	{
		if (iter.current() == v)
		{
			// If item is found, remove and return
			mArray[bucket].remove(iter);
			return true;
		}

		++iter;
	}

	// else, not found:
	return false;
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear
// Notes : clears the hash table
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void HTable<Type>::clear()
{
	// Remove all data from each bucket
	for (unsigned int i = 0; i < mBuckets; ++i)
		mArray[i].clear();
	// Keep the Hash Table's empty members
}

/////////////////////////////////////////////////////////////////////////////
// Function : find
// Parameters : v - the item to look for
// Return : the bucket we found the item in or -1 if we didn’t find the item.
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
int HTable<Type>::find(const Type& v) const
{
	// Find the bucket, point an iter at it
	unsigned int bucket = mHashFunc(v);
	SLLIter<Type> iter(mArray[bucket]);
	iter.begin();

	// Iterate through the bucket
	while (!iter.end())
	{
		// If item is found, return the correct bucket
		if (iter.current() == v)
			return bucket;

		++iter;
	}

	// Else, return -1
	return -1;
}

/////////////////////////////////////////////////////////////////////////////
// Function : printSomeStuff 
// Parameters : filePath - file to stream output into
// Notes : gives information about hash table's functionality:
//		   distribution info, # of empty buckets, etc.
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void HTable<Type>::printSomeStuff(const char* filePath = "hashdata.txt")
{
	// we're gonna need to open a file for all this data
	ofstream outFile(filePath);

	// let's make sure the file got opened
	if (outFile.is_open())
	{
		// number of empty buckets, total count of elements, index of emptiest bucket, index of fullest bucket
		unsigned int empty = 0;
		unsigned int totalCount = 0;
		unsigned int loIndex = 0;
		unsigned int hiIndex = 0;

		// loop through all the buckets
		for (unsigned int i = 0; i < mBuckets; ++i)
		{
			// add the number of elements in each bucket to the total count
			totalCount += mArray[i].size();
			// print the index of this bucket and its size to the file
			outFile << i << " : " << mArray[i].size() << '\n';

			// if this list is empty, increment the empty count
			if (mArray[i].size() == 0)
				++empty;

			// if this list has less elements than the lowest one recorded so far, store this as the new lowest
			if (mArray[i].size() < mArray[loIndex].size())
				loIndex = i;
			// else, if this list has more elements than the highest one recorded so far, store this as the new highest
			else if (mArray[i].size() > mArray[hiIndex].size())
				hiIndex = i;
		}

		// print the total count of items and number of buckets to the file
		outFile << '\n' << totalCount << " Total items stored in " << mBuckets << " buckets\n";
		// print the number of empty buckets
		outFile << '\n' << empty << " Buckets are empty\n\n";
		// get the number of elements in the emptiest bucket
		unsigned int Low = mArray[loIndex].size();
		// get the range fullest-emptiest+1
		unsigned int range = mArray[hiIndex].size() - Low + 1;
		// print this info to the file
		outFile << "each bucket contains between " << Low << " and " << Low + range - 1 << " items.\n\n";

		// make a new array to count how many buckets contain each number of items between the emptiest and fullest
		// and initialize each cell to 0
		unsigned int* arr = new unsigned int[range];
		for (unsigned int j = 0; j < range; ++j)
			arr[j] = 0;

		// now we go through and count how many buckets contain each number of items...
		// 3 buckets have 15 items
		// 5 buckets have 16 items
		// etc.
		for (unsigned int k = 0; k < mBuckets; ++k)
			++arr[mArray[k].size() - Low];

		// now print this data to the file
		for (unsigned int p = 0; p < range; ++p)
			outFile << arr[p] << " buckets have " << p + Low << " items\n";

		// delete the array we made a minute ago, we are done with it
		delete[] arr;
	}
}
#pragma endregion