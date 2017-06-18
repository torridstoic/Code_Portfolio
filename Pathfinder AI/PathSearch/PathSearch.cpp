#include "PathSearch.h"
#include <Windows.h>

#define HWEIGHT 1.2f
#define DEBUG 1
#if DEBUG
#include <iostream>
#endif

namespace fullsail_ai
{
	namespace algorithms
	{
#pragma region Private Helper Functions
		/// <summary>
		/// Sorting function for our Priority Queue.
		/// This sorts Planner Nodes within the queue by their Final Cost.
		/// (final cost = weighted sum of [cost of path so far] and [estimation to goal])
		/// </summary>
		/// <param name="pLhs">Pointer to first Planner Node</param>
		/// <param name="pRhs">Pointer to second Planner Node</param>
		/// <returns>bool comparison of PNodes' final costs</returns>
		bool isGreaterCost(PlannerNode* const& pLhs, PlannerNode* const& pRhs)
		{
			return (pLhs->finalCost > pRhs->finalCost);
		}

		/// <summary>
		/// Given two tiles, determines if they share an edge.
		/// </summary>
		/// <param name="pTile">Pointer to first tile</param>
		/// <param name="pSideTile">Pointer to second tile</param>
		/// <returns>true or false</returns>
		bool PathSearch::areAdjacent(Tile* pTile, Tile* pSideTile)
		{
			if (NULL == pTile
				|| NULL == pSideTile
				|| 0 == pSideTile->getWeight())
			{
				// If TileWeight is 0, it's impassable.
				return false;
			}

			// Find the row & column values for both tiles:
			const int row = pTile->getRow();
			const int col = pTile->getColumn();
			const int secondRow = pSideTile->getRow();
			const int secondCol = pSideTile->getColumn();
			// If row or col difference is more than 1,
			// the tiles can't possibly be adjacent.
			if (abs(row - secondRow) > 1 || abs(col - secondCol) > 1)
			{
				return false;
			}

			/*
			 * Since tiles are hexes,
			 * we need to account for diagonal adjacency, or row offsets.
			 * There are two different cases:
			 * 1) Main tile row is even.
			 * 2) Main tile row is odd.
			 * Example: [row,col] [1,1] is adj to [0,2] and [2,2]
			 * but NOT adj to [0,0] or [2,0].
			 * [4,4] is adj to [3,3] and [5,3]
			 * but NOT adj to [3,5] or [5,5].
			*/
			if (row % 2 == 0) // even row
			{
				if (row != secondRow && secondCol > col)
				{
					return false;
				}
			}
			else // odd row
			{
				if (row != secondRow && col > secondCol)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Clears and redraws the visual map display.
		/// Fills visited tiles blue,
		/// marks "open" tiles (can be moved to) green,
		/// outlines neighbor tiles (adj to current) orange
		/// and draws orange lines from curr to neighbors,
		/// and draws a red line for path from start->current.
		/// </summary>
		/// <param name="pCurrPNode">Pointer to the currently occupied Planner Node</param>
		void PathSearch::updateDisplay(PlannerNode* pCurrPNode)
		{
			pTileMap->resetTileDrawing();

			// The starting tile is always filled blue:
			pStartTile->setFill(0xFF0000FF);
			for (PlannerNode* pPNode : setVisited)
			{
				// FILL all visited tiles BLUE:
				pPNode->pSearchNode->pTile->setFill(0xFF0000FF);
			}
#if 0
			for (std::pair<SearchNode*, PlannerNode*> element : mPlannerGraph)
			{
				if (element.second->pParentPNode != nullptr)
				{
					// FILL all visited tiles BLUE
					element.first->pTile->setFill(0xFF0000FF);
				}
			}
#endif

			int colorVal = 0xFF;
			std::vector<PlannerNode*> vOpenNodes;
			pqOpenNodes.enumerate(vOpenNodes);
			for (int i = vOpenNodes.size() - 1; i >= 0; --i)
			{
				// MARK all open tiles GREEN (gradient).
				// Tiles at top of priority queue are very green,
				// lower in the queue are less vivid:
				vOpenNodes[i]->pSearchNode->pTile->setMarker(colorVal * 0x100 + 0xFF000000);
				colorVal = (colorVal > 10) ? (colorVal - 10) : 1;
			}

			// Only color the neighbors if we haven't found the solution yet:
			if (!bSolutionFound)
			{
				for (Edge edge : pCurrPNode->pSearchNode->edges)
				{
					// OUTLINE all neighbors ORANGE:
					edge.pEndpointSNode->pTile->setOutline(0xFFFFA000);
					// ADD LINES to all neighbor tiles (ORANGE):
					pCurrPNode->pSearchNode->pTile->addLineTo(edge.pEndpointSNode->pTile, 0xFFFFA000);
				}
			}

			// For drawing our path, walk through the nodes like a linked list:
			for (PlannerNode* pWalkPNode = pCurrPNode;
				pWalkPNode->pParentPNode != nullptr;
				pWalkPNode = pWalkPNode->pParentPNode)
			{
				// ADD LINE from curr node to start (RED):
				pWalkPNode->pSearchNode->pTile->addLineTo(pWalkPNode->pParentPNode->pSearchNode->pTile, 0xFFFF0000);
			}
		}

		/// <summary>
		/// Calculates the distance between two tiles.
		/// This method is used for heuristic values.
		/// Finds the [x,y] distance, NOT the path difference.
		/// Does not take impassable terrain or tileweight into account.
		/// Example: which available tile is closest to the goal?
		/// </summary>
		/// <param name="pTile">Pointer to first tile</param>
		/// <param name="pSecTile">Pointer to second tile</param>
		/// <returns>Distance (float) between tiles</returns>
		float PathSearch::estimateDist(Tile* pTile, Tile* pSecTile)
		{
			float xDifference = abs(pTile->getXCoordinate() - pSecTile->getXCoordinate());
			float yDifference = abs(pTile->getYCoordinate() - pSecTile->getYCoordinate());
			return sqrt(xDifference*xDifference + yDifference*yDifference);
		}
#pragma endregion

#pragma region Ctor/Dtor
		PathSearch::PathSearch() : pqOpenNodes(PriorityQueue<PlannerNode*>(isGreaterCost))
		{
			/* Member Initializer List sets up our Open Nodes priority queue
			 * to use the isGreaterCost() method as a comparer.
			 * This will sort "potential next nodes" with
			 * [lower cost/weight from the start] to the top of queue.
			*/

			// Determine how heavily to weight heuristic estimates:
			heuristicWeight = HWEIGHT;
			// Initialize member variables:
			shutdown();
		}

		PathSearch::~PathSearch()
		{
			// Free any allocated memory:
			shutdown();
		}
#pragma endregion

#pragma region Public Functions
		/// <summary>
		/// Initializes PathSearch member fields.
		/// Generates SearchGraph: creates and fills SearchNodes.
		/// Creates PlannerNodes (but does not fill them).
		/// This method is called after the TileMap is loaded.
		/// </summary>
		/// <param name="_tileMap">Logical representation of the tile map</param>
		void PathSearch::initialize(TileMap* _tileMap)
		{
			// Reset member variables:
			shutdown();

			pTileMap = _tileMap;
			const int mapRows = pTileMap->getRowCount();
			const int mapCols = pTileMap->getColumnCount();

			// Create SearchNodes and add to SearchGraph
			for (int row = 0; row < mapRows; ++row)
			{
				for (int col = 0; col < mapCols; ++col)
				{
					// Each SearchNode contains a pointer to its Tile.
					Tile* pCurrTile = pTileMap->getTile(row, col);
					if (NULL != pCurrTile && pCurrTile->getWeight() > 0)
					{
						// Only make SearchNodes for traversable tiles (weight > 0)
						mSearchGraph[pCurrTile] = new SearchNode(pCurrTile);
					}
				}
			}
			// Create Edges from new SearchNodes.
			// Every SearchNode should contain an Edge for every adjancent Tile.
			for (std::pair<Tile*, SearchNode*> element : mSearchGraph)
			{
				const int y = element.first->getRow();
				const int x = element.first->getColumn();

				for (int row = y - 1; row <= y + 1; ++row)
				{
					for (int col = x - 1; col <= x + 1; ++col)
					{
						// For each SearchNode's tile,
						// check nearby tiles to find all adjancent:
						Tile* pCurrTile = pTileMap->getTile(row, col);
						if (areAdjacent(element.first, pCurrTile))
						{
							// If they're adj, create an Edge struct
							// and add it to the SearchNode's vector of edges:
							element.second->edges.push_back(Edge
								(mSearchGraph[pCurrTile],
									estimateDist(element.first, pCurrTile) * pCurrTile->getWeight()));

							// Draw all edges
							element.first->addLineTo(pCurrTile, 0xFF0000FF);
						}
					}
				}

				// Fill the PlannerGraph while we're here
				mPlannerGraph[element.second] = new PlannerNode(element.second);
			}
		}

		/// <summary>
		/// Prepares to perform a search between the given coordinates.
		/// This method is called before any update of the path planner.
		/// </summary>
		/// <param name="startRow">Row of starting point</param>
		/// <param name="startColumn">Column of starting point</param>
		/// <param name="goalRow">Row of finish/goal point</param>
		/// <param name="goalColumn">Column of finish point</param>
		void PathSearch::enter(int startRow, int startColumn, int goalRow, int goalColumn)
		{
			// Reset search variables for a new search:
			exit();
			bSolutionFound = false;

			// Find & store the tiles for start & finish points:
			pStartTile = pTileMap->getTile(startRow, startColumn);
			SearchNode* pStartNode = mSearchGraph[pTileMap->getTile(startRow, startColumn)];
			pGoalTile = pTileMap->getTile(goalRow, goalColumn);

			// Start the timer:
			startTimeMillis = GetTickCount();

			// Fill in starting position's Planner Node's data (parent=null, and costs):
			PlannerNode* pStartPlanner = mPlannerGraph[pStartNode];
			pStartPlanner->pParentPNode = nullptr;
			pStartPlanner->heuristicCost = estimateDist(pStartNode->pTile, pGoalTile);
			pStartPlanner->finalCost = pStartPlanner->heuristicCost * heuristicWeight;

			// Use the starting PNode to begin the Visited & Open datasets:
			setVisited.insert(pStartPlanner);
			pqOpenNodes.push(pStartPlanner);

			// Update display:
#if DEBUG
			pTileMap->resetTileDrawing();
			updateDisplay(pqOpenNodes.front());
#endif
		}

		/// <summary>
		/// Performs the search.
		/// A time limit may be provided, or we can run to completion.
		/// If the given timeslice is 0, only one iteration is run.
		/// </summary>
		/// <param name="timeslice">How long to continue searching (ms)</param>
		void PathSearch::update(long timeslice)
		{
#if DEBUG
			// Determine what time (ms) to stop searching:
			DWORD goalTime = GetTickCount() + timeslice;
#endif
			// Stop searching if solution is found.
			// If there are no open nodes, the solution isn't possible.
			while (!pqOpenNodes.empty() && !bSolutionFound)
			{
				// Remove the current PNode from the Open dataset:
				PlannerNode* pCurrPNode = pqOpenNodes.front();
				pqOpenNodes.pop();

				// Check if we're finished:
				if (pCurrPNode->pSearchNode->pTile == pGoalTile)
				{
					bSolutionFound = true;
					for (PlannerNode* pWalkPNode = pCurrPNode;
						nullptr != pWalkPNode;
						pWalkPNode = pWalkPNode->pParentPNode)
					{
						// Build the solution path (vector) : finish to start.
						// Walk backward similar to a linked list.
						vSolutionPath.push_back(pWalkPNode->pSearchNode->pTile);
					}

#if DEBUG
					// Print console info:
					//updateDisplay(pCurrPNode);
					std::cout << "Solution found!\n";
					std::cout << "Path Length: " << vSolutionPath.size() << std::endl;
					std::cout << "Final Cost: " << pCurrPNode->finalCost << std::endl;
					std::cout << "Completion Time: " << (GetTickCount() - startTimeMillis) << "\n\n\n";
#endif
				}

				// For all of the current node's Edges...
				for (Edge edge : pCurrPNode->pSearchNode->edges)
				{
					SearchNode* pAdjNode = edge.pEndpointSNode;
					PlannerNode* pAdjPlanner = mPlannerGraph[pAdjNode];
					
					// Find the cost to get to the adj tile,
					// *from this path*:
					float adjGivenCost = pCurrPNode->givenCost + edge.cost;

					// If we haven't visited the adj tile...
					if (setVisited.end() == setVisited.find(pAdjPlanner))
					{
						// Fill the adj Planner Node's data (parent=curr, and costs):
						pAdjPlanner->pParentPNode = pCurrPNode;
						pAdjPlanner->heuristicCost = estimateDist(pAdjNode->pTile, pGoalTile);
						pAdjPlanner->givenCost = adjGivenCost;
						pAdjPlanner->finalCost = adjGivenCost + (pAdjPlanner->heuristicCost * heuristicWeight);

						// And add it to Visited & Open datasets:
						setVisited.insert(pAdjPlanner);
						pqOpenNodes.push(mPlannerGraph[pAdjNode]);
					}
					// If we HAVE visited, but the cost on this path is SMALLER...
					else if (adjGivenCost < pAdjPlanner->givenCost)
					{
						/*
						 * We need to update the PNode's cost (and parent).
						 * The Open dataset is priority-sorted by cost
						 * whenever a node is added to the queue,
						 * so we need to remove the node from the queue,
						 * update it, and add it back in.
						*/

						// Remove the PNode from the Open queue:
						pqOpenNodes.remove(pAdjPlanner);

						// Update the PNode's data (parent=curr, and new costs):
						pAdjPlanner->pParentPNode = pCurrPNode;
						pAdjPlanner->givenCost = adjGivenCost;
						pAdjPlanner->finalCost = adjGivenCost + (pAdjPlanner->heuristicCost * heuristicWeight);
						
						// Add it back to the (newly sorted) Open queue:
						pqOpenNodes.push(pAdjPlanner);
					}
				}

#if DEBUG
				updateDisplay(pCurrPNode);

				if (GetTickCount() >= goalTime)
				{
					// Break out if we've passed the timeslice
					break;
				}
#else
				if (0 == timeslice)
				{
					break;
				}
#endif
			}
		}

		/// <summary>
		/// Cleans up and/or resets any search data:
		/// solution path, visited set, and open set.
		/// This method is called when the current search data is no longer needed.
		/// </summary>
		void PathSearch::exit()
		{
			vSolutionPath.clear();
			setVisited.clear();
#if 0
			for (std::pair<SearchNode*, PlannerNode*> element : mPlannerGraph)
			{
				// "Reset" all nodes in the Planner Graph
				element.second->givenCost = 0;
				element.second->heuristicCost = 0;
				element.second->finalCost = 0;
				element.second->pParentPNode = nullptr;
			}
#endif
			pqOpenNodes.clear();
		}

		/// <summary>
		/// Cleans up any tilemap-related memory.
		/// This method is called when we're done with the current tilemap.
		/// </summary>
		void PathSearch::shutdown()
		{
			// Clear the search data, if we have any:
			exit();

			// Delete Planner Nodes:
			for (std::pair<SearchNode*, PlannerNode*> element : mPlannerGraph)
			{
				delete element.second;
			}
			mPlannerGraph.clear();

			// Delete Search Nodes:
			for (std::pair<Tile*, SearchNode*> element : mSearchGraph)
			{
				delete element.second;
			}
			mSearchGraph.clear();

			// Clear our pointers & solution data:
			pTileMap = nullptr;
			pStartTile = pGoalTile = nullptr;
			bSolutionFound = false;
		}

		/// <summary>
		/// Checks if the solution is found.
		/// The main application queries this.
		/// </summary>
		/// <returns>Unicorns</returns>
		bool PathSearch::isDone() const
		{
			return bSolutionFound;
		}

		/// <summary>
		/// Gets a vector of tiles constructing the solution path.
		/// The main application queries this.
		/// </summary>
		/// <remarks>
		/// The solution vector is in reverse order (finish->start)
		/// </remarks>
		/// <returns>Vector of tiles making the solution path</returns>
		std::vector<Tile const*> const PathSearch::getSolution() const
		{
			return vSolutionPath;
		}
#pragma endregion
	}
}  // namespace fullsail_ai::algorithms

