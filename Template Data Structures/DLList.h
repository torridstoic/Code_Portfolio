#pragma once


template <typename Type> class DLLIter;

#pragma region DLList Header
template <typename Type>
class DLList
{
private:
	friend class DLLIter<Type>;
	struct node
	{
		Type contents;
		node* next;
		node* prev;
		// Ctor
		node(Type _contents, node* _next, node* _prev) : contents(_contents), next(_next), prev(_prev) { }
	};

	unsigned int mSize;
	node* head;
	node* tail;

public:
	// Ctor
	DLList();

	// Trilogy
	~DLList();
	DLList(const DLList<Type>& that);
	DLList<Type>& operator=(const DLList<Type>& that);

	// Functions
	void addHead(const Type& v);
	void addTail(const Type& v);
	void clear();
	void insert(DLLIter<Type>& index, const Type& v);
	void remove(DLLIter<Type>& index);
};
#pragma endregion

#pragma region DLLIter Header
template <typename Type>
class DLLIter
{
private:
	friend class DLList<Type>;

	typename DLList<Type>::node* mCurr;
	DLList<Type>* mList;

public:
	// Ctor/Dtor
	DLLIter(DLList<Type>& listToIterate);
	~DLLIter() { mList = nullptr; mCurr = nullptr; }

	// Operators
	DLLIter<Type>& operator++();
	DLLIter<Type>& operator--();

	// Functions
	void beginHead();
	void beginTail();
	bool end() const;
	Type& current() const;
};
#pragma endregion


#pragma region DLList Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constructor
// Notes : constructs an empty list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLList<Type>::DLList()
{
	mSize = 0;
	head = tail = nullptr;
}

/////////////////////////////////////////////////////////////////////////////
// Function : Destructor
// Notes : Destroys a list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLList<Type>::~DLList()
{
	clear();
}

/////////////////////////////////////////////////////////////////////////////
// Function : Copy Constructor
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLList<Type>::DLList(const DLList<Type>& that)
{
	// Basic initialization
	mSize = 0;
	head = tail = nullptr;

	// Copy the list backwards with addHead functionality
	for (node* reader = that.tail; reader != nullptr; reader = reader->prev)
	{
		addHead(reader->contents);
		if (that.tail == reader)
			tail = head;
	}
}

/////////////////////////////////////////////////////////////////////////////
// Function : Assignment Operator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLList<Type>& DLList<Type>::operator=(const DLList<Type>& that)
{
	// Paranoia check
	if (this != &that)
	{
		// Reset current list
		clear();

		// Copy the list backwards with addHead functionality
		for (node* reader = that.tail; reader != nullptr; reader = reader->prev)
		{
			addHead(reader->contents);
			if (that.tail == reader)
				tail = head;
		}
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : addHead
// Parameters : v - the item to add to the head of the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLList<Type>::addHead(const Type& v)
{
	// Make new node, adjust pointers
	node* tmp = new node(v, head, nullptr);
	if (mSize > 0)
		head->prev = tmp;

	// Adjust List members
	head = tmp;
	++mSize;

	// Special case: List previously empty
	if (1 == mSize)
		tail = tmp;
}

/////////////////////////////////////////////////////////////////////////////
// Function : addTail
// Parameters : v - the item to add to the tail of the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLList<Type>::addTail(const Type& v)
{
	// Make new node, adjust pointers
	node* tmp = new node(v, nullptr, tail);
	if (mSize > 0)
		tail->next = tmp;

	// Adjust List members
	tail = tmp;
	++mSize;

	// Special case: List previously empty
	if (1 == mSize)
		head = tmp;
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear
// Notes : clears the list, freeing any dynamic memory
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLList<Type>::clear()
{
	// Walk through the list, deleting the head
	node* walk = head;
	while (walk)
	{
		walk = head->next;
		delete head;
		head = walk;
	}

	// Reset the List data
	head = tail = nullptr;
	mSize = 0;
}

/////////////////////////////////////////////////////////////////////////////
// Function : insert
// Parameters :	index - an iterator to the location to insert at
//				v - the item to insert
// Notes : do nothing on a bad iterator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLList<Type>::insert(DLLIter<Type>& index, const Type& v)
{
	// Exit on a bad iterator
	if (index.end())
		return;

	// Insert a new node, and update node pointers
	// Special case: insert at head
	if (index.mCurr == head)
	{
		addHead(v);
		index.beginHead();
	}
	else
	{
		node* tmp = new node(v, index.mCurr, index.mCurr->prev);
		index.mCurr->prev->next = tmp;
		index.mCurr->prev = tmp;
		--index;
		++mSize;
	}
}

/////////////////////////////////////////////////////////////////////////////
// Function : remove
// Parameters :	index - an iterator to the location to remove from
// Notes : do nothing on a bad iterator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLList<Type>::remove(DLLIter<Type>& index)
{
	// Exit on a bad iterator
	if (index.end())
		return;

	// Special case: Head = Tail
	if (head == tail)
		head = tail = nullptr;
	// Special case: Remove Head
	else if (index.mCurr == head)
	{
		index.mCurr->next->prev = nullptr;
		head = index.mCurr->next;
	}
	// Special case: Remove Tail
	else if (index.mCurr == tail)
	{
		index.mCurr->prev->next = nullptr;
		tail = index.mCurr->prev;
	}
	else if (mSize > 1)
	{
		// Adjust pointers surrounding removal node
		index.mCurr->next->prev = index.mCurr->prev;
		index.mCurr->prev->next = index.mCurr->next;
	}

	// Move the iterator (to next) and delete the node
	node* tmp = index.mCurr;
	++index;
	delete tmp;
	--mSize;
}
#pragma endregion

#pragma region DLLIter Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constructor
// Parameters :	listToIterate - the list to iterate
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLLIter<Type>::DLLIter(DLList<Type>& listToIterate)
{
	mList = &listToIterate;
	beginHead();
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator++
// Notes : move the iterator forward one node
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLLIter<Type>& DLLIter<Type>::operator++()
{
	if (mCurr != nullptr)
		mCurr = mCurr->next;
	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator--
// Notes : move the iterator backward one node
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
DLLIter<Type>& DLLIter<Type>::operator--()
{
	if (mCurr != nullptr)
		mCurr = mCurr->prev;
	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function : beginHead
// Notes : moves the iterator to the head of the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLLIter<Type>::beginHead()
{
	mCurr = mList->head;
}

/////////////////////////////////////////////////////////////////////////////
// Function : beginTail
// Notes : moves the iterator to the tail of the list
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void DLLIter<Type>::beginTail()
{
	mCurr = mList->tail;
}

/////////////////////////////////////////////////////////////////////////////
// Function : end
// Notes : returns true if we are at the end of the list, false otherwise
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
bool DLLIter<Type>::end() const
{
	return(!mCurr);
}

/////////////////////////////////////////////////////////////////////////////
// Function : current
// Notes : returns the item at the current iterator location
////////////////////////////////////////////////////////////////////////////
template <typename Type>
Type& DLLIter<Type>::current() const
{
	return mCurr->contents;
}
#pragma endregion