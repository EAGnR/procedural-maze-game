using System.Collections.Generic;
using UnityEngine;
using Mazes.Utils;

namespace Mazes
{
    /// <summary>
    /// This class generates the 2-dimensional data for a procedural maze, being generated through the use
    /// of the Disjoint-Set ADT.<para /> 
    /// The data can be accessed through a grid which holds all the cells of the maze, with each
    /// cell being a single unit of the maze with a max adjacency of 4 walls.
    /// </summary>
    /// <seealso cref="DisjointSets"/>
    public class MazeGenerator
    {
        public struct Cell
        {
            public readonly bool aboveWall, belowWall, leftWall, rightWall;

            public Cell(bool above, bool below, bool left, bool right)
            {
                aboveWall = above;
                belowWall = below;
                leftWall = left;
                rightWall = right;
            }
        }

        private readonly int Rows, Cols;
        private readonly int CellNum;
        private readonly int Max_Adjacency = 4;

        // Grid of maze cells represented by disjoint sets.
        private Cell[] grid;
        private DisjointSets sets;
        private System.Random rand = new System.Random();

        /// <summary>
        /// Set the number of rows and columns for the maze to be generated, be sure to call <see cref="GenerateMaze()"/>
        /// after invoking this constructor.
        /// </summary>
        /// <param name="rows">The number of rows for the maze.</param>
        /// <param name="cols">The number of columns for the maze.</param>
        public MazeGenerator(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            CellNum = Rows * Cols;
            grid = new Cell[CellNum];
            sets = new DisjointSets(CellNum);

            for (int i = 0; i < grid.Length; ++i)
            {
                grid[i] = new Cell(true, true, true, true);
            }
        }

        /// <summary>
        /// Procedurally generates the 2-dimensional data for a maze with the dimensions set by the 
        /// <see cref="MazeGenerator(int, int)"/> constructor.<para />
        /// Calling this method consecutively will always generate an entirely new maze but of same Rows by Cols 
        /// dimensions of the given object instance. To change the dimensions, a new object must be instantiated.
        /// </summary>
        public void GenerateMaze()
        {
            int visitedCells = 1;
            int currCell = rand.Next(CellNum);
            int adjCell;
            bool adjacentFound;

            // Index - 0: above, 1: below, 2: left, 3: right
            // If adjacentCells[Index] = -1 then that adjacent cell does not exist.
            int[] adjCells = new int[Max_Adjacency];

            Stack<int> cellStack = new Stack<int>();

            while (visitedCells < CellNum)
            {
                adjCell = -1;
                adjacentFound = false;

                if (currCell < Cols)
                {
                    adjCells[0] = -1;
                }
                else
                {
                    adjCells[0] = currCell - Cols;
                }

                if (currCell >= (CellNum - Cols))
                {
                    adjCells[1] = -1;
                }
                else
                {
                    adjCells[1] = currCell + Cols;
                }

                if (currCell % Cols == 0)
                {
                    adjCells[2] = -1;
                }
                else
                {
                    adjCells[2] = currCell - 1;
                }

                if ((currCell + 1) % Cols == 0)
                {
                    adjCells[3] = -1;
                }
                else
                {
                    adjCells[3] = currCell + 1;
                }

                for (int i = 0; i < adjCells.Length; ++i)
                {
                    if (adjCells[i] >= 0 
                        && (sets.Find(currCell) != sets.Find(adjCells[i])))
                    {
                        adjacentFound = true;
                    }
                    else
                    {
                        adjCells[i] = -1;
                    }
                }

                if (adjacentFound)
                {
                    int wallIndex;
                    do
                    {
                        wallIndex = rand.Next(Max_Adjacency);
                        adjCell = adjCells[wallIndex];
                    }
                    while (adjCell < 0);

                    sets.Union(currCell, adjCell);

                    // Adjacent cell is - 0: above, 1: below, 2: left, 3: right
                    switch (wallIndex)
                    {
                        case 0: // Connect currentCell with adjacentCell above.
                            grid[currCell] = new Cell(false, grid[currCell].belowWall, grid[currCell].leftWall, grid[currCell].rightWall);
                            grid[adjCell] = new Cell(grid[adjCell].aboveWall, false, grid[adjCell].leftWall, grid[adjCell].rightWall);
                            break;
                        case 1: // Connect currentCell with adjacentCell below.
                            grid[currCell] = new Cell(grid[currCell].aboveWall, false, grid[currCell].leftWall, grid[currCell].rightWall);
                            grid[adjCell] = new Cell(false, grid[adjCell].belowWall, grid[adjCell].leftWall, grid[adjCell].rightWall);
                            break;
                        case 2: // Connect currentCell with adjacentCell on left.
                            grid[currCell] = new Cell(grid[currCell].aboveWall, grid[currCell].belowWall, false, grid[currCell].rightWall);
                            grid[adjCell] = new Cell(grid[adjCell].aboveWall, grid[adjCell].belowWall, grid[adjCell].leftWall, false);
                            break;
                        case 3: // Connect currentCell with adjacentCell on right.
                            grid[currCell] = new Cell(grid[currCell].aboveWall, grid[currCell].belowWall, grid[currCell].leftWall, false);
                            grid[adjCell] = new Cell(grid[adjCell].aboveWall, grid[adjCell].belowWall, false, grid[adjCell].rightWall);
                            break;
                        default:
                            Debug.LogError
                                ("GenerateMaze() error: invalid wallIndex value.");
                            break;
                    }

                    cellStack.Push(currCell);
                    currCell = adjCell;
                    ++visitedCells;
                }
                else
                {
                    if (cellStack.Count > 0)
                        currCell = cellStack.Pop();
                }
            }
        }

        /// <summary>
        /// Allows access to <see cref="grid"/> which holds the 2-dimensional maze data. <para /> 
        /// One maze unit of type <see cref="Cell"/> can returned at a time, addressed through its grid index.
        /// </summary>
        /// <param name="index">The grid index of the maze cell to be accessed.</param>
        /// <returns>
        /// Returns a single maze unit of type <see cref="Cell"/>.
        /// </returns>
        public Cell GetGridCell(int index)
        {
            return grid[index];
        }
    }
}
