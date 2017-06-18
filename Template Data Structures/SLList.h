#pragma once


template <typename Type> class SLLIter;

#pragma region SLList Header
template <typename Type>
class SLList
{
private:
	friend class SLLIter<Type>;
	struct node
	{
		Type contents;
		node* next;
		// Ctor
		node(Type _contents, node* _next) : contents(_contents), next(_next) { }
	};

	unsigned int mSize;
	node* head;

	// Helper Function
	void copyList(node* reader);

public:
	// Ctor
	SLList();

	// Trilogy
	~SLList();
	SLList(const SLList<Type>& that);
	SLList<Type>& operator=(const SLList<Type>& that);

	// Functions
	void addHead(const Type& v);
	void clear();
	void insert(SLLIter<Type>& index, const Type& v);
	void remove(SLLIter<Type>& index);
	unsigned int size() const { return mSize; }
};
#pragma endregion

#pragma region SLLIter Header
template <typename Type>
class SLLIter
{
private:
	friend class SLList<Type>;

	// This iter only uses a single node pointer, which necessitates a couple special cases
	// We will actually point at head whether we're referring to head or head->next
	// The boolean mBegin determines which the iterator is actually referring to
	bool mBegin;
	typename SLList<Type>::node* mPrev;
	SLList<Type>* mList;

public:
	// Ctor/Dtor
	SLLIter(SLList<Type>& listToIterate);
	~SLLIter() { mList = nullptr; mPrev = nullptr; }

	// Functions
	void begin();
	bool end() const;
	SLLIter<Type>& operator++();
	Type& current() const;
};
#pragma endregion


#pragma region SLList Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constructor
// Notes : constructs an empty list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLList<Type>::SLList()
{
	mSize = 0;
	head = nullptr;
}

/////////////////////////////////////////////////////////////////////////////
// Function : Destructor
// Notes : Destroys the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLList<Type>::~SLList()
{
	clear();
}

/////////////////////////////////////////////////////////////////////////////
// Function : Copy Constructor
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLList<Type>::SLList(const SLList<Type>& that)
{
	// Basic initialization
	mSize = 0;
	head = nullptr;
	// Recursive copy
	copyList(that.head);

#pragma region Iterative version
#if 0
	// If the target List isn't empty, copy it
	if (0 != that.size())
	{
		addHead(that.head->contents);
		node* walk = head;

		node* reader = that.head->next;
		while (reader != nullptr)
		{
			node* tmp = new node(reader->contents, nullptr);
			walk->next = tmp;
			mSize++;

			reader = reader->next;
			walk = walk->next;
		}
	}
#endif
#pragma endregion
}

/////////////////////////////////////////////////////////////////////////////
// Function : Assignment Operator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLList<Type>& SLList<Type>::operator=(const SLList<Type>& that)
{
	// Paranoia check
	if (this != &that)
	{
		// Reset current List
		clear();
		// Recursive copy
		copyList(that.head);

#pragma region Iterative version
#if 0
		// If the target List isn't empty, copy it
		if (0 != that.size())
		{

			addHead(that.head->contents);
			node* walk = head;

			node* reader = that.head->next;
			while (reader != nullptr)
			{
				node* tmp = new node(reader->contents, nullptr);
				walk->next = tmp;
				mSize++;

				reader = reader->next;
				walk = walk->next;
			}
		}
#endif
#pragma endregion
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : copyList
// Notes : recursive method of copying a SLList (operator= or Copy Constructor)
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLList<Type>::copyList(node* reader)
{
	// Exit condition
	if (nullptr == reader)
		return;

	// Recursive call with next node
	copyList(reader->next);
	// Add nodes to new List, in reverse, with addHead
	addHead(reader->contents);
}

/////////////////////////////////////////////////////////////////////////////
// Function : addHead
// Parameters :	v - the item to add to the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLList<Type>::addHead(const Type& v)
{
	// Create a node, point it at current head, set new head, increment size
	node* tmp = new node(v, head);
	head = tmp;
	++mSize;
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear
// Notes : clears the list, freeing any dynamic memory
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLList<Type>::clear()
{
	node* walk = head;
	// Increment walk, delete current head, set new head, repeat
	while (walk)
	{
		walk = head->next;
		delete head;
		head = walk;
	}

	// Reset the List data
	head = nullptr;
	mSize = 0;
}

/////////////////////////////////////////////////////////////////////////////
// Function : insert
// Parameters :	index - an iterator to the location to insert at
//				v - the item to insert
// Notes : do nothing on a bad iterator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLList<Type>::insert(SLLIter<Type>& index, const Type& v)
{
	// Exit on a bad iterator
	if (nullptr == index.mPrev)
		return;

	// Insert a new node, and update node pointers
	// Special case: index at 0
	if (index.mBegin)
	{
		addHead(v);
		index.begin();
	}
	else // standard case
	{
		node* tmp = new node(v, index.mPrev->next);
		index.mPrev->next = tmp;
		// Increment size
		++mSize;
	}
}

/////////////////////////////////////////////////////////////////////////////
// Function : remove
// Parameters :	index - an iterator to the location to remove from
// Notes : do nothing on a bad iterator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLList<Type>::remove(SLLIter<Type>& index)
{
	// Exit on a bad iterator
	if (nullptr == index.mPrev)
		return;

	// Make a tmp node* BEFORE index's location
	// (due to single-node-pointer iter functionality)
	node* tmp = index.mPrev;

	// Special case: index = head
	if (index.mBegin)
	{
		// Increment the head (and the iter)
		head = tmp->next;
		index.mPrev = tmp->next;
	}
	else
	{
		// Increment tmp to index's location
		tmp = tmp->next;
		// Move List pointers over the to-be-deleted node
		index.mPrev->next = tmp->next;
	}

	// Delete the node, decrement size
	delete tmp;
	--mSize;
}
#pragma endregion

#pragma region SLLIter Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constructor
// Parameters :	listToIterate - the list to iterate
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLLIter<Type>::SLLIter(SLList<Type>& listToIterate)
{
	mList = &listToIterate;
	begin();
}

/////////////////////////////////////////////////////////////////////////////
// Function : begin
// Notes : moves the iterator to the head of the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void SLLIter<Type>::begin()
{
	mPrev = mList->head;
	mBegin = true;
}

/////////////////////////////////////////////////////////////////////////////
// Function : end
// Notes : returns true if we are at the end of the list, false otherwise
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
bool SLLIter<Type>::end() const
{
	// Single-node-pointer iterator necessitates some unintuitive cases here:

	// Case: empty list
	if (0 == mList->size())
		return true;
	// Case: 1 list element
	if (1 == mList->size() && !mBegin)
		return true;
	// Standard case: iter->prev is pointing at final element ("current" is nullptr)
	if (mList->size() > 1 && nullptr == mPrev->next)
		return true;

	return false;
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator++
// Notes : move the iterator forward one node
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
SLLIter<Type>& SLLIter<Type>::operator++()
{
	// If iter is still at begin(), DO NOT actually move it,
	// instead toggle mBegin to false
	if (mBegin)
		mBegin = false;
	else
		mPrev = mPrev->next;

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : current
// Notes : returns the item at the current iterator location
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
Type& SLLIter<Type>::current() const
{
	if (mBegin)
		return mPrev->contents;
	else
		return mPrev->next->contents;
}
#pragma endregion