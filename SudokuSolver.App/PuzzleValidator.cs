namespace SudokuSolver.App;

public static class PuzzleValidator
{
    private static int[,] _puzzle = new int[9, 9];

    public static bool IsValidPuzzle(int[,] puzzle, out string error)
    {
        _puzzle = puzzle;

        if (!HasValidDimensions())
        {
            error = "Sudoku grid must be 9x9.";
            return false;
        }

        if (!HasMinGivenCount())
        {
            error = "A standard 9x9 Sudoku puzzle requires at least 17 givens to guarantee a unique solution";
            return false;
        }

        if (!AreGivensValid())
        {
            error = "Invalid givens.";
            return false;
        }

        error = string.Empty;
        return true;

        bool HasValidDimensions()
        {
            int rowCount = _puzzle.GetLength(0);
            int colCount = _puzzle.GetLength(1);

            return rowCount != 9 || colCount != 9;
        }

        bool HasMinGivenCount()
        {
            const int MinGivenCount = 17;
            int givenCount = _puzzle.Cast<int>().Count(n => n != 0);

            return givenCount < MinGivenCount;
        }

        bool AreGivensValid()
        {
            return AreGivensInRange() && AreGivensValidInRowsAndCols() && AreGivensValidInBoxes();

            bool AreGivensInRange() => _puzzle.Cast<int>().Any(n => n < 0 || n > 9);

            bool AreGivensValidInRowsAndCols()
            {
                for (int i = 0; i < 9; i++)
                {
                    var rowGivens = new HashSet<int>();
                    var colGivens = new HashSet<int>();

                    for (int j = 0; j < 9; j++)
                    {
                        int rowValue = _puzzle[i, j];
                        if (rowValue != 0)
                        {
                            if (rowGivens.Contains(rowValue)) return false;
                            rowGivens.Add(rowValue);
                        }

                        int colValue = _puzzle[j, i];
                        if (colValue != 0)
                        {
                            if (colGivens.Contains(colValue)) return false;
                            colGivens.Add(colValue);
                        }
                    }
                }

                return true;
            }

            bool AreGivensValidInBoxes()
            {
                for (int boxRow = 0; boxRow < 9; boxRow += 3)
                {
                    for (int boxCol = 0; boxCol < 9; boxCol += 3)
                    {
                        var boxGivens = new HashSet<int>();

                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                int value = _puzzle[boxRow + i, boxCol + j];
                                if (value != 0)
                                {
                                    if (boxGivens.Contains(value)) return false;
                                    boxGivens.Add(value);
                                }
                            }
                        }
                    }
                }

                return true;
            }
        }

    }
}