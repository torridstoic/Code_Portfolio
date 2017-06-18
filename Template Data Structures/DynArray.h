#pragma once


#pragma region DynArray Header
template <typename Type>
class DynArray
{
private:
	unsigned int mSize;
	unsigned int mCapacity;

protected:
	Type* mArray;
	void decrement() { --mSize; }

public:
	// Ctor
	DynArray();

	// Trilogy
	~DynArray();
	DynArray(const DynArray<Type>& that);
	DynArray<Type>& operator=(const DynArray<Type>& that);

	// Operators
	Type& operator[](const unsigned int index);
	const Type& operator[](const unsigned int index) const;

	// Accessors
	unsigned int size() const;
	unsigned int capacity() const;

	// Other Functions
	void clear();
	void append(const Type& item);
	void reserve(const unsigned int & newCap = 0);

	// Insert/Remove
	void insert(const Type val, const unsigned int index);
	void insert(const Type * val, const unsigned int n, const unsigned int index);
	void remove(const unsigned int index);
	void remove(const unsigned int index, const unsigned int n);
};
#pragma endregion


#pragma region DynArray Definitions
/////////////////////////////////////////////////////////////////////////////
// Function :	Constructor
// Notes : Constructs an empty array (Size 0 Capacity 0)
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DynArray<Type>::DynArray()
{
	mArray = nullptr;
	mSize = mCapacity = 0;
}

/////////////////////////////////////////////////////////////////////////////
// Function :	Destructor
// Notes : cleans up any dynamic memory
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DynArray<Type>::~DynArray()
{
	clear();
}

/////////////////////////////////////////////////////////////////////////////
// Function :	Copy Constructor
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DynArray<Type>::DynArray(const DynArray<Type>& that)
{
	// Copy values
	mArray = nullptr;
	mSize = that.size();
	mCapacity = that.capacity();

	// Copy the Array
	mArray = new Type[mCapacity];
	for (unsigned int i = 0; i < mSize; ++i)
		mArray[i] = that.mArray[i];
}

/////////////////////////////////////////////////////////////////////////////
// Function :	Assignment Operator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DynArray<Type>& DynArray<Type>::operator=(const DynArray<Type>& that)
{
	// Paranoia check
	if (this != &that)
	{
		clear();

		// Copy values
		mSize = that.size();
		mCapacity = that.capacity();

		// Copy the array
		mArray = new Type[mCapacity];
		for (unsigned int i = 0; i < mSize; ++i)
			mArray[i] = that.mArray[i];
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index to access
// Return : Type & - returns the array element at [index]
// Notes : performs no error checking. user should ensure index is 
//		valid with the size() method
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
Type& DynArray<Type>::operator[](const unsigned int index)
{
	return mArray[index];
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index to access
// Return : const Type & - returns the array element at [index]
// Notes : performs no error checking. user should ensure index is 
//		valid with the size() method
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
const Type& DynArray<Type>::operator[](const unsigned int index) const
{
	return mArray[index];
}

/////////////////////////////////////////////////////////////////////////////
// Function :	size
// Returns : int - returns the number of items being stored
// Notes : this function returns the number of items being stored, 
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
unsigned int DynArray<Type>::size() const
{
	return mSize;
}

/////////////////////////////////////////////////////////////////////////////
// Function : capacity
// Returns : int - returns the number of items the array can store before 
//		the next resize
// Notes : this function returns the number of items the array can store, 
//		not the number of bytes
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
unsigned int DynArray<Type>::capacity() const
{
	return mCapacity;
}

/////////////////////////////////////////////////////////////////////////////
// Function :	clear
// Notes : cleans up any dynamic memory and resets size and capacity to 0
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::clear()
{
	delete[] mArray;
	mSize = mCapacity = 0;
	mArray = nullptr;
}

/////////////////////////////////////////////////////////////////////////////
// Function : append
// Parameters : item - the item to be appended to the next open spot
// Notes : this function will append the item to the next open spot. if 
//		no room exists, the array's capacity will be doubled and then 
//		the item will be added
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::append(const Type& item)
{
	// Make more space if the Array is full
	if (mSize == mCapacity)
		reserve();

	// Add the item and increase mSize
	mArray[mSize] = item;
	++mSize;
}

/////////////////////////////////////////////////////////////////////////////
// Function : reserve
// Parameters : newCap - the new capacity
// Notes : 	- default parameter - reserve more space in the array, based on
//		the expansion rate (100%, 1 minimum).
//		- non-default parameter, expand to the specified capacity
//		- if newCap is LESS than the current capacity, do nothing. 
//		This function should NOT make the array smaller.
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::reserve(const unsigned int & newCap = 0)
{
	// Find new array size
	if (0 == newCap)
	{
		// Special case: current capacity is 0
		if (0 == mCapacity)
			mCapacity = 1;
		// Standard case:
		else
			mCapacity *= 2;
	}
	else if (newCap <= mCapacity) // Error check
		return;
	else
		mCapacity = newCap;

	// Create a new array and copy current contents
	Type* tmpArray = new Type[mCapacity];
	unsigned int i = 0;
	for (; i < mSize; ++i)
		tmpArray[i] = mArray[i];

	// Save the new array
	delete[] mArray;
	mArray = tmpArray;
}

/////////////////////////////////////////////////////////////////////////////
// Function :	insert
// Parameters : val - the value to insert
//		   index - the index to insert at
// Notes : if the array is full, this function should expand the array at 
//		the default expansion rate (double the capacity, 1 minimum)
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::insert(const Type val, const unsigned int index)
{
	// Call the bigdaddy function with n=1
	insert(&val, 1, index);
}

/////////////////////////////////////////////////////////////////////////////
// Function :	insert
// Parameters : val - the items to insert
//		   n - the number of items to insert
//		   index - the index to insert at
// Notes : if the array is full, this function should expand the array at 
//		the default expansion rate (double the capacity, 1 minimum) 
//		before inserting
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::insert(const Type * val, const unsigned int n, const unsigned int index)
{
	// Error check
	if (index > mSize)
		return;

	// Check for a size upgrade
	while (mCapacity < mSize + n)
		reserve();

	// Copy contents after "index"
	if (mSize > 0)
	{
		for (unsigned int i = mSize - 1; i >= index; --i)
		{
			mArray[i + n] = mArray[i];
			if (0 == i)
				break;
		}
	}

	// Insert new values
	for (int i = 0; i < n; ++i)
		mArray[index + i] = val[i];

	// Adjust size
	mSize += n;
}

/////////////////////////////////////////////////////////////////////////////
// Function :	remove
// Parameters : index - the index to remove from
// Notes : this function removes one item from the specified index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::remove(const unsigned int index)
{
	// Just call the bigger function with n=1
	remove(index, 1);
}

/////////////////////////////////////////////////////////////////////////////
// Function :	remove
// Parameters : index - the index to remove from
//		   n - the number of items to remove
// Notes : this function removes multiple items from the specified index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DynArray<Type>::remove(const unsigned int index, const unsigned int n)
{
	// Error check
	if (index + n > mSize)
		return;

	// Shift values after [index+n] backward
	for (unsigned int i = index + n; i < mSize; ++i)
		mArray[i - n] = mArray[i];

	// Adjust size
	mSize -= n;
}
#pragma endregion
