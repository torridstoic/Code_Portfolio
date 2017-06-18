#pragma once
#include <queue>
#include <iostream>
using namespace std;
#include "SLList.h"
#include "DynArray.h"


#pragma region Graph Header
template <typename Type>
class Graph
{
#pragma region Encapsulated Structs
public:
	struct Edge
	{
		unsigned int toVertex;
		// Ctor
		Edge(unsigned int _v) : toVertex(_v) { }
	};
	struct Vertex
	{
		// the data that this vertex is storing
		Type element;
		// the list of edges that connect this vertex to another vertex
		SLList<Edge> edges;

		Vertex() { }
		Vertex(Type _v) : element(_v) { }
		void addEdge(const unsigned int& toVertex);
	};
#pragma endregion

private:
	DynArray<Vertex> mVertices;

public:
	// Ctor/Dtor
	Graph() { }
	~Graph() { clear(); }

	// Functions
	unsigned int addVertex(const Type& value);
	Vertex& operator[](const unsigned int& index);
	const Vertex& operator[](const unsigned int& index) const;
	unsigned int size() const;
	void clear();
	void printBreadthFirst(const unsigned int& startVertex);
};
#pragma endregion


#pragma region Graph Definitions
///////////////////////////////////////////////////////////////////////////
// Function : addEdge
// Parameters : toVertex - the index of the vertex we are adjacent to
///////////////////////////////////////////////////////////////////////////
template <typename Type>
void Graph<Type>::Vertex::addEdge(const unsigned int& toVertex)
{
	edges.addHead(Edge(toVertex));
}

/////////////////////////////////////////////////////////////////////////////
// Function : addVertex
// Parameters : value - the data to store in this vertex
// Return : unsigned int - the index this vertex was added at
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
unsigned int Graph<Type>::addVertex(const Type& value)
{
	Vertex tmp(value);
	mVertices.append(tmp);
	return (mVertices.size() - 1);
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index in the graph to access
// Return : Vertex& - the vertex stored at the specified index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
typename Graph<Type>::Vertex& Graph<Type>::operator[](const unsigned int& index)
{
	return mVertices[index];
}

/////////////////////////////////////////////////////////////////////////////
// Function : operator[]
// Parameters : index - the index in the graph to access
// Return : Vertex& - the vertex stored at the specified index
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
const typename Graph<Type>::Vertex& Graph<Type>::operator[](const unsigned int& index) const
{
	return mVertices[index];
}

/////////////////////////////////////////////////////////////////////////////
// Function : size
// Return : unsigned int - the number of vertices in the graph
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
unsigned int Graph<Type>::size() const
{
	return mVertices.size();
}

/////////////////////////////////////////////////////////////////////////////
// Function : clear
// Notes : clears the graph and readies it for re-use
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void Graph<Type>::clear()
{
	mVertices.clear();
}

/////////////////////////////////////////////////////////////////////////////
// Function : printBreadthFirst
// Parameters : startVertex - the vertex to start the traversal at
// Notes : prints contents of the graph in depth order 
//			(from closest to farthest)
/////////////////////////////////////////////////////////////////////////////
template <typename Type>
void Graph<Type>::printBreadthFirst(const unsigned int& startVertex)
{
	// Make a parallel DynArray, storing the depth of all Vertices
	// Set initial state for untouched vertices = size()
	// size() is an unreachable depth, used for testing
	unsigned int numVer = size();
	DynArray<unsigned int> depthArr;
	depthArr.reserve(numVer);
	for (unsigned int i = 0; i < numVer; ++i)
		depthArr.append(numVer);

	// Make a traversal queue and push the first Vertex
	queue<unsigned int> traversal;
	traversal.push(startVertex);
	depthArr[startVertex] = 0;

	while (!traversal.empty())
	{
		// Print top Vertex's data, pop it
		unsigned int i = traversal.front();
		cout << mVertices[i].element << " : " << depthArr[i] << endl;
		traversal.pop();

		// Iterate through that Vertex's edges
		SLLIter<Edge> iter(mVertices[i].edges);
		for (iter.begin(); !iter.end(); ++iter)
		{
			// Push untouched Vertices onto the queue
			unsigned int tmpV = iter.current().toVertex;
			if (depthArr[tmpV] == numVer)
			{
				traversal.push(tmpV);
				depthArr[tmpV] = depthArr[i] + 1;
			}
		}
	}
}
#pragma endregion