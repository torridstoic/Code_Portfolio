//! \file TileMap.h
//! \brief Defines the <code>fullsail_ai::TileMap</code> class.
//! \author Cromwell D. Enage
#ifndef _FULLSAIL_AI_PATH_PLANNER_TILE_MAP_H_
#define _FULLSAIL_AI_PATH_PLANNER_TILE_MAP_H_

#include "Tile.h"
#include "../platform.h"

namespace fullsail_ai {

	//! \brief Logical representation of a tile map.
	//!
	//! All tile information can be accessed from this data structure.  You can treat it like
	//! a 2-D tile grid by passing row and column indices to the <code>getTile()</code> method.
	class TileMap
	{
		int row_count;
		int column_count;
		Tile** tiles;
		double tile_radius;
		unsigned int weight_sum_squared;

	public:
		//! \brief Constructs a new <code>%TileMap</code> object.
		//!
		//! Invoked by the application.
		//!
		//! \note
		//!   - Do not construct your own <code>%TileMap</code> objects during lab.
		DLLEXPORT TileMap();

		//! \brief Copy constructor.
		//!
		//! \note
		//!   - Do not construct your own <code>%TileMap</code> objects during lab.
		DLLEXPORT TileMap(TileMap const&);

		//! \brief Assignment operator.
		DLLEXPORT TileMap& operator=(TileMap const&);

		//! \brief Destroys this <code>%TileMap</code> object.
		DLLEXPORT ~TileMap();

		//! \brief Cleans up the underlying tiles and array memory.  Also zeroes the row count,
		//! the column count, and the tile radius.
		//!
		//! The application must reset any search algorithms using this tile map after invoking
		//! this method.
		DLLEXPORT void reset();

		//! \brief Creates the underlying tile array.
		//!
		//! The application must reset any search algorithms using this tile map after invoking
		//! this method.
		DLLEXPORT void createTileArray(int num_rows, int num_columns);

		//! \brief Sets the radius of the largest circle that can be circumscribed by a tile.
		//!
		//! Also sets the radius of any tiles previously created.  The application must therefore
		//! reset any search algorithms using this tile map after invoking this method.
		DLLEXPORT void setRadius(double radius);

		//! \brief Creates a <code>Tile</code> object and adds it to the appropriate location in
		//! the array.
		//!
		//! The application must reset any search algorithms using this tile map after invoking
		//! this method.
		//!
		//! \param   row     the row-coordinate of the tile's location.
		//! \param   column  the column-coordinate of the tile's location.
		//! \param   data    the weight of the tile.
		//!
		//! \pre
		//!   - The underlying tile array must not be <code>NULL</code>.
		DLLEXPORT void addTile(int row, int column, unsigned char data);

		//! \brief Returns a pointer to the tile at the specified location.
		//!
		//! \param   row     the row-coordinate of the tile's location.
		//! \param   column  the column-coordinate of the tile's location.
		//! \return  the tile at the specified location, or <code>NULL</code> if either of the
		//!          coordinates are out of bounds.
		DLLEXPORT Tile* getTile(int row, int column) const;

		//! \brief Computes the square of all tile weights added together.
		//!
		//! The application must reset any search algorithms using this tile map after invoking
		//! this method.
		//!
		//! \pre
		//!   - The underlying tile array must not be <code>NULL</code>.
		DLLEXPORT void computeWeightSumSquared();

		//! \brief Resets all drawing colors set in the tiles to transparent black (0x00000000).
		//!
		//! \pre
		//!   - The underlying tile array must not be <code>NULL</code>.
		DLLEXPORT void resetTileDrawing();

		//! \brief Returns the square of all tile weights added together.
		inline unsigned int getWeightSumSquared() const
		{
			return weight_sum_squared;
		}

		//! \brief Returns one past the upper bound of a tile's row coordinate.
		inline int getRowCount() const
		{
			return row_count;
		}

		//! \brief Returns one past the upper bound of a tile's column coordinate.
		inline int getColumnCount() const
		{
			return column_count;
		}

		//! \brief Returns the radius of the largest circle that can be circumscribed by a tile.
		inline double getTileRadius() const
		{
			return tile_radius;
		}
	};
}  // namespace fullsail_ai

#endif  // _FULLSAIL_AI_PATH_PLANNER_TILE_MAP_H_

