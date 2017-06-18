#pragma once
#include "DynArray.h"


#pragma region BinaryHeap Header
template <typename Type>
class BinaryHeap :
	protected DynArray<Type>
{
public:
	// Ctor/Dtor
	BinaryHeap() { }
	~BinaryHeap() { }

	// Operators
	Type& operator[](const unsigned int index);
	const Type& operator[](const unsigned int index) const;

	// Accessor
	unsigned int size();

	// Other functions
	void enqueue(const Type &v);
	Type dequeue();
	void clear();
};
#pragma endregion


#pragma region BinaryHeap Definitions
///////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index to access
// Return : Type& - the item in the index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
Type& BinaryHeap<Type>::operator[](const unsigned int index)
{
	return DynArray<Type>::operator[](index);
}

///////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index to access
// Return : Type& - the item in the index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
const Type& BinaryHeap<Type>::operator[](const unsigned int index) const
{
	return DynArray<Type>::operator[](index);
}

///////////////////////////////////////////////////////////////////////////////
// Function : size
// Return : the number of valid items in the heap
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
unsigned int BinaryHeap<Type>::size()
{
	return DynArray<Type>::size();
}

/////////////////////////////////////////////////////////////////////////////
// Function : enqueue
// Parameters : v - the item to add to the heap
// Notes : after the new item is added, this function ensures that the 
//	smallest value in the heap is in [0]
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void BinaryHeap<Type>::enqueue(const Type &v)
{
	// Append the new item
	DynArray<Type>::append(v);

	// "Sort" the heap
	int curIndex = size() - 1;
	int parentIndex = (curIndex - 1) >> 1;
	while (parentIndex >= 0)
	{
		// If new value is < parent value
		if (v < mArray[parentIndex])
		{
			// Swap parent and child
			mArray[curIndex] = mArray[parentIndex];
			mArray[parentIndex] = v;

			// Set new indexes for next check
			curIndex = parentIndex;
			parentIndex = (curIndex - 1) >> 1;
		}
		// else, tree is sorted
		else
			break;
	}
}

/////////////////////////////////////////////////////////////////////////////
// Function : dequeue 
// Return : the smallest item in the heap, or Type() if the heap is empty
// Notes : after the smallest item is dequeued, this function ensures that 
//	the smallest item is in [0]
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
Type BinaryHeap<Type>::dequeue()
{
	// Error check
	if (0 == size())
		return Type();

	// Store value to return, decrement size, move last element to "front" of array
	Type retVal = mArray[0];
	DynArray<Type>::decrement();
	mArray[0] = mArray[size()];

	// Re-sort the heap
	unsigned int curIndex = 0;
	unsigned int childIndex = curIndex;
	while (true)
	{
		// Find the smallest child
		// (make sure to check that index values are in-range)
		if (curIndex * 2 + 1 < size())
		{
			childIndex = curIndex * 2 + 1;
			if (childIndex + 1 < size() && mArray[childIndex + 1] < mArray[childIndex])
				childIndex++;
		}

		// If child is < current value, swap them...
		if (mArray[childIndex] < mArray[curIndex])
		{
			Type tmp = mArray[curIndex];
			mArray[curIndex] = mArray[childIndex];
			mArray[childIndex] = tmp;
			// ...and set a new index for the loop
			curIndex = childIndex;
		}
		// else, we're sorted: break and return
		else
			break;
	}
	
	return retVal;
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear	
// Notes : clears the heap out
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void BinaryHeap<Type>::clear()
{
	DynArray<Type>::clear();
}
#pragma endregion