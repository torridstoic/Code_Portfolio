#pragma once


#pragma region BST Header
template <typename Type>
class BST
{
private:
	struct node
	{
		Type contents;
		node *parent, *left, *right;
		// Ctor
		node(const Type _contents) { contents = _contents; parent = left = right = nullptr; }
	};

	// Members
	node* root;

	// Helper Functions (recursive)
	void CopyRecur(node* _cur);
	void InsertRecur(node* _newN, node* _cur);
	node* FindRecur(node* _cur, const Type& _v) const;
	void ClearRecur(node* _cur);
	void PrintRecur(node* _cur) const;

public:
	// Ctor
	BST();

	// Trilogy
	~BST();
	BST& operator=(const BST& that);
	BST(const BST& that);

	// Functions
	void insert(const Type& v);
	bool findAndRemove(const Type& v);
	bool find(const Type& v) const;
	void clear();
	void printInOrder() const;
};
#pragma endregion


#pragma region BST Helper Functions
template <typename Type>
void BST<Type>::CopyRecur(node* _cur)
{
	// Exit
	if (!_cur)
		return;

	// Pre-Order Traversal
	insert(_cur->contents);
	CopyRecur(_cur->left);
	CopyRecur(_cur->right);
}

template <typename Type>
void BST<Type>::InsertRecur(node* _newN, node* _cur)
{
	if (_newN->contents < _cur->contents) // Less than
	{
		// Insert the node if ->left is empty,
		// otherwise continue down the Tree
		if (!(_cur->left))
		{
			_cur->left = _newN;
			_newN->parent = _cur;
		}
		else
			InsertRecur(_newN, _cur->left);
	}
	else // Greater/Equal To
	{
		// Insert the node if ->right is empty,
		// otherwise continue down the Tree
		if (!(_cur->right))
		{
			_cur->right = _newN;
			_newN->parent = _cur;
		}
		else
			InsertRecur(_newN, _cur->right);
	}
}

template <typename Type>
typename BST<Type>::node* BST<Type>::FindRecur(node* _cur, const Type& _v) const
{
	// Exit Conditions
	if (!_cur)
		return nullptr;
	if (_v == _cur->contents)
		return _cur;

	// Else, recursive search
	if (_v < _cur->contents)
		return FindRecur(_cur->left, _v);
	else
		return FindRecur(_cur->right, _v);
}

template <typename Type>
void BST<Type>::ClearRecur(node* _cur)
{
	// Exit
	if (!_cur)
		return;

	// Post-Order Traversal
	ClearRecur(_cur->left);
	ClearRecur(_cur->right);
	delete _cur;
}

template <typename Type>
void BST<Type>::PrintRecur(node* _cur) const
{
	// Exit
	if (!_cur)
		return;

	// In-Order Traversal
	PrintRecur(_cur->left);
	cout << (_cur->contents) << " ";
	PrintRecur(_cur->right);
}
#pragma endregion

#pragma region BST Definitions
/////////////////////////////////////////////////////////////////////////////
// Function : Constuctor
// Notes : constucts an empty BST
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
BST<Type>::BST()
{
	root = nullptr;
}

/////////////////////////////////////////////////////////////////////////////
// Function : Destructor
// Notes : destroys the BST cleaning up any dynamic memory
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
BST<Type>::~BST()
{
	clear();
}

/////////////////////////////////////////////////////////////////////////////
// Function : assignment operator
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
BST<Type>& BST<Type>::operator=(const BST& that)
{
	// Paranoia check
	if (this != &that)
	{
		clear();
		// Pre-Order Recursion
		CopyRecur(that.root);
	}

	return *this;
}

/////////////////////////////////////////////////////////////////////////////
// Function: copy constructor
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
BST<Type>::BST(const BST& that)
{
	root = nullptr;
	// Pre-Order Recursion
	CopyRecur(that.root);
}

/////////////////////////////////////////////////////////////////////////////
// Function : insert
// Parameters :  v - the item to insert 
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void BST<Type>::insert(const Type& v)
{
	node* tmp = new node(v);

	// Special case: Empty Tree
	if (!root)
		root = tmp;
	// Normal case: Recursive Insertion
	else
		InsertRecur(tmp, root);
}

/////////////////////////////////////////////////////////////////////////////
// Function : findAndRemove
// Parameters : v - the item to find (and remove if it is found)
// Return : bool - true if the item was removed, false otherwise
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
bool BST<Type>::findAndRemove(const Type& v)
{
	node* tmp = FindRecur(root, v);
	if (!tmp) // Not Found
		return false;

	// Case 2: TWO children
	// (this leads into case 0 or 1)
	if ((tmp->left) && (tmp->right))
	{
		// Find a new node position (minimum of right subtree)
		node* leaf = tmp->right;
		while (leaf->left)
			leaf = leaf->left;
		// Swap node contents
		tmp->contents = leaf->contents;
		leaf->contents = v;

		// Re-point tmp, and continue to new case
		tmp = leaf;
	}

	node* prev = tmp->parent;

	// Case 0 AND 1a: NO children or ONE child (left)
	if (!(tmp->right))
	{
		if (root == tmp)
			root = tmp->left;
		else
		{
			if (prev->left == tmp) // parent's left
				prev->left = tmp->left;
			else // parent's right
				prev->right = tmp->left;

			if (tmp->left)
				tmp->left->parent = prev;
		}
	}
	// Case 1b: ONE child (right)
	else if (!(tmp->left))
	{
		if (root == tmp)
			root = tmp->right;
		else
		{
			if (prev->left == tmp) // parent's left
				prev->left = tmp->right;
			else // parent's right
				prev->right = tmp->right;

			tmp->right->parent = prev;
		}
	}

	delete tmp;
	return true;
}

/////////////////////////////////////////////////////////////////////////////
// Function : find
// Parameters : v - the item to find
// Return : bool - true if the item was found, false otherwise
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
bool BST<Type>::find(const Type& v) const
{
	if (FindRecur(root, v))
		return true;
	else
		return false;
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear
// Notes : clears the BST, cleaning up any dynamic memory
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void BST<Type>::clear()
{
	// Post-Order Recursion
	ClearRecur(root);
	root = nullptr;
}

/////////////////////////////////////////////////////////////////////////////
// Function : printInOrder
// Notes : prints the contents of the BST to the screen, in ascending order
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void BST<Type>::printInOrder() const
{
	// In-Order Recursion
	PrintRecur(root);
	cout << endl;
}
#pragma endregion