//! \file PathSearch.h
//! \brief Defines the <code>fullsail_ai::algorithms::PathSearch</code> class interface.
//! \author Cromwell D. Enage, 2009; Jeremiah Blanchard, 2012
#ifndef _FULLSAIL_AI_PATH_PLANNER_PATH_SEARCH_H_
#define _FULLSAIL_AI_PATH_PLANNER_PATH_SEARCH_H_

// change this to start the program on whatever default map as you like from the table below
#define USEDEFAULTMAP hex035x035

#define hex006x006 "./Data/hex006x006.txt"
#define hex014x006 "./Data/hex014x006.txt"
#define hex035x035 "./Data/hex035x035.txt"
#define hex054x045 "./Data/hex054x045.txt"
#define hex098x098 "./Data/hex098x098.txt"
#define hex113x083 "./Data/hex113x083.txt"

// change this to 1(true), and change the data below when you want to test specific starting and goal locations on startup
#define OVERRIDE_DEFAULT_STARTING_DATA 0

// Make sure your start and goal are valid locations!
#define DEFAULT_START_ROW 31
#define DEFAULT_START_COL 19
#define DEFAULT_GOAL_ROW 3
#define DEFAULT_GOAL_COL 15

#include "../TileSystem/Tile.h"
#include "../TileSystem/TileMap.h"
#include "../platform.h"
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include "../PriorityQueue.h"
//#include <deque>

namespace fullsail_ai
{
	namespace algorithms
	{
		// Forward Declarations
		struct Edge;

#pragma region Structs
		/// <summary>
		/// SearchNodes create the SearchGraph for pathfinding.
		/// Each node contains a Tile and Edges to all adjacent nodes.
		/// </summary>
		struct SearchNode
		{
			Tile* pTile;
			std::vector<Edge> edges;

			SearchNode(Tile* pT) :
				pTile(pT) { }
		};

		/// <summary>
		/// Edges denote connections between adjancent SearchNodes.
		/// Each Edge is contained in a SearchNode,
		/// and contains a pointer to the adjacent node and the cost of traversal.
		/// </summary>
		struct Edge
		{
			SearchNode* pEndpointSNode;
			float cost;

			Edge(SearchNode* pSN) :
				pEndpointSNode(pSN), cost(0) { }
			Edge(SearchNode* pSN, float c) :
				pEndpointSNode(pSN), cost(c) { }
		};

		/// <summary>
		/// PlannerNodes are used in finding ideal/correct paths.
		/// Each PlannerNode contains a pointer to its SearchNode,
		/// as well as a pointer to its parent (previous) node
		/// and cost values.
		/// </summary>
		struct PlannerNode
		{
			SearchNode* pSearchNode;
			PlannerNode* pParentPNode;

			// Heuristic Cost is an estimation of distance from the goal.
			float heuristicCost;
			// Given Cost is the actual cost of this path from the start.
			float givenCost;
			// Final Cost is the weighted sum of both previous costs.
			// Weight is determined by HWEIGHT.
			float finalCost;

			PlannerNode(SearchNode* pSN) :
				pSearchNode(pSN), pParentPNode(nullptr),
				heuristicCost(0), givenCost(0), finalCost(0) { }
		};
#pragma endregion

		// Sorting function for the Priority Queue
		bool isGreaterCost(PlannerNode* const& pLhs, PlannerNode* const& pRhs);

#pragma region PathSearch
		class PathSearch
		{
		private:
#pragma region Member Variables
			bool bSolutionFound;
			float heuristicWeight;
			TileMap* pTileMap;
			Tile* pStartTile;
			Tile* pGoalTile;
			DWORD startTimeMillis;
#pragma endregion
#pragma region Member Containers
			// Unordered maps and sets have generally faster direct lookup,
			// so we sacrifice some extra memory here for increased speed.
			std::unordered_map<Tile*, SearchNode*> mSearchGraph;
			std::unordered_map<SearchNode*, PlannerNode*> mPlannerGraph;
			std::unordered_set<PlannerNode*> setVisited;
			// This Priority Queue will sort our open nodes by final cost for us.
			PriorityQueue<PlannerNode*> pqOpenNodes;
			std::vector<Tile const*> vSolutionPath;
#if 0
			// Containers used in previous application versions
			std::map<SearchNode*, PlannerNode*> mVisitedNodes;
			std::deque<PlannerNode*> dqOpenNodes;
#endif
#pragma endregion
#pragma region Helper Methods
			bool areAdjacent(Tile* pTile, Tile* pSideTile);
			void updateDisplay(PlannerNode* pCurrPNode);
			float estimateDist(Tile* pTile, Tile* pSecTile);
#pragma endregion

		public:
			//! \brief Default constructor.
			DLLEXPORT PathSearch();

			//! \brief Destructor.
			DLLEXPORT ~PathSearch();

			//! \brief Sets the tile map.
			//!
			//! Invoked when the user opens a tile map file.
			//!
			//! \param   _tileMap  the data structure that this algorithm will use
			//!                    to access each tile's location and weight data.
			DLLEXPORT void initialize(TileMap* _tileMap);

			//! \brief Enters and performs the first part of the algorithm.
			//!
			//! Invoked when the user presses one of the play buttons.
			//!
			//! \param   startRow         the row where the start tile is located.
			//! \param   startColumn      the column where the start tile is located.
			//! \param   goalRow          the row where the goal tile is located.
			//! \param   goalColumn       the column where the goal tile is located.
			DLLEXPORT void enter(int startRow, int startColumn, int goalRow, int goalColumn);

			//! \brief Returns <code>true</code> if and only if no nodes are left open.
			//!
			//! \return  <code>true</code> if no nodes are left open, <code>false</code> otherwise.
			DLLEXPORT bool isDone() const;

			//! \brief Performs the main part of the algorithm until the specified time has elapsed or
			//! no nodes are left open.
			DLLEXPORT void update(long timeslice);

			//! \brief Returns an unmodifiable view of the solution path found by this algorithm.
			DLLEXPORT std::vector<Tile const*> const getSolution() const;

			//! \brief Resets the algorithm.
			DLLEXPORT void exit();

			//! \brief Uninitializes the algorithm before the tile map is unloaded.
			DLLEXPORT void shutdown();
		};
#pragma endregion
	}
}  // namespace fullsail_ai::algorithms

#endif  // _FULLSAIL_AI_PATH_PLANNER_PATH_SEARCH_H_

