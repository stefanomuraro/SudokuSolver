namespace SudokuSolver.App;

public static class IO
{
    public static void Print(int[,] grid)
    {
        int rowCount = grid.GetLength(0);
        int colCount = grid.GetLength(1);

        const int CellWidth = 4;
        var divider = new string('-', colCount * CellWidth + 1); ;

        for (int i = 0; i < rowCount; i++)
        {
            Console.WriteLine(divider);
            for (int j = 0; j < colCount; j++)
            {
                Console.Write($"| {grid[i, j]} ");
            }
            Console.Write("|");
            Console.Write(Environment.NewLine);
        }
        Console.WriteLine(divider);
    }
}