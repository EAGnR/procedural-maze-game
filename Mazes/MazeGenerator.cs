using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mazes.Utils;

namespace Mazes
{
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

        private readonly int ROWS, COLS;
        private readonly int CELL_NUM;
        private readonly int MAX_ADJACENCY = 4;

        // Grid of maze cells represented by disjoint sets.
        private Cell[] grid;
        private DisjointSets sets;
        private Stack<int> cellStack = new Stack<int>();
        private System.Random rand = new System.Random();

        public MazeGenerator(int rows, int cols)
        {
            ROWS = rows;
            COLS = cols;
            CELL_NUM = ROWS * COLS;
            grid = new Cell[CELL_NUM];
            sets = new DisjointSets(CELL_NUM);

            for (int i = 0; i < grid.Length; ++i)
            {
                grid[i] = new Cell(true, true, true, true);
            }
        }

        public void GenerateMaze()
        {
            int visitedCells = 1;
            int currCell = rand.Next(CELL_NUM);
            int adjCell;
            bool adjacentFound;

            // Index - 0: above, 1: below, 2: left, 3: right
            // If adjacentCells[Index] = -1 then that adjacent cell does not exist.
            int[] adjCells = new int[MAX_ADJACENCY];

            cellStack.Clear();

            while (visitedCells < CELL_NUM)
            {
                adjCell = -1;
                adjacentFound = false;

                if (currCell < COLS)
                {
                    adjCells[0] = -1;
                }
                else
                {
                    adjCells[0] = currCell - COLS;
                }

                if (currCell >= (CELL_NUM - COLS))
                {
                    adjCells[1] = -1;
                }
                else
                {
                    adjCells[1] = currCell + COLS;
                }

                if (currCell % COLS == 0)
                {
                    adjCells[2] = -1;
                }
                else
                {
                    adjCells[2] = currCell - 1;
                }

                if ((currCell + 1) % COLS == 0)
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
                        wallIndex = rand.Next(MAX_ADJACENCY);
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

            cellStack.Clear();
        }

        public Cell GetGridCell(int i)
        {
            return grid[i];
        }
    }
}
