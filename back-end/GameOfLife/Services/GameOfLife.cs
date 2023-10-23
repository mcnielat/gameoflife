using ConwaysGameOfLife.Interfaces;
using Newtonsoft.Json;

namespace ConwaysGameOfLife.Services
{
    /// <summary>
    /// The class that holds the functions responsible for the 
    /// computation of the Game of Life's next generation
    /// </summary>
    public class GameOfLife : IGameOfLife
    {
        /// <summary>
        /// Computes for the next generation of the game based on the
        /// current generation input
        /// </summary>
        /// <param name="currentGen">Array that contains the current generation of the game</param>
        /// <returns></returns>
        public string GetNextGeneration(int[][] currentGen)
        {
            //Grid is a square so getting the legnth of 1 dimension should be enough
            var gridLength = currentGen.GetLength(0);

            var newTable = new int[gridLength][];

            // Apply rules and update the grid
            for (int row = 0; row < gridLength; row++)
            {
                int[] newRow = new int[gridLength];
                for (int col = 0; col < gridLength; col++)
                {
                    int cellVal = currentGen[row][col];
                    int neighborCount = CountNeighbors(currentGen, row, col);

                    if (cellVal == 1 && neighborCount < 2)
                    {
                        // If live and <2 live neighbors
                        cellVal = 0;
                    }
                    else if (cellVal == 1 && neighborCount > 3)
                    {
                        // If live and >3 live neighbors
                        cellVal = 0;
                    }
                    else if (cellVal == 0 && neighborCount == 3)
                    {
                        // If dead and 3 live neighbors
                        cellVal = 1;
                    }

                    newRow[col] = cellVal;
                }
                newTable[row] = newRow;
            }

            return JsonConvert.SerializeObject(newTable);
        }

        /// <summary>
        /// Counts the number of adjacent cells to the current cell placed in row x col position
        /// </summary>
        /// <param name="currentGen"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        private static int CountNeighbors(int[][] currentGen, int row, int col)
        {
            // Return the number of live neighbors
            int count = 0;
            int gridLn = currentGen.GetLength(0);

            for (int i = -1; i < 2; i++)
            {
                // This checks the row above and row below
                if (col + i >= 0 && col + i < gridLn - 1)
                {
                    // Check for a valid column
                    if (row > 0 && currentGen[row - 1][col + i] == 1)
                        count++;

                    if (row < gridLn - 1 && currentGen[row + 1][col + i] == 1)
                        count++;
                }
            }

            if (col - 1 >= 0 && currentGen[row][col - 1] == 1)
                // Check the left cell
                count++;

            if (col + 1 < gridLn - 1 && currentGen[row][col + 1] == 1)
                // Check the right cell
                count++;

            return count;
        }
    }
}
