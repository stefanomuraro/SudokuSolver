using System.Collections.Immutable;

public class Program
{
    private static readonly ImmutableHashSet<int> _allNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public static void Main()
    {
        int[,] sudoku = new int[9, 9]
        {
            { 0, 0, 1, 0, 0, 3, 0, 2, 0 },
            { 5, 0, 0, 0, 4, 0, 0, 6, 0 },
            { 4, 8, 0, 0, 7, 6, 0, 0, 0 },
            { 0, 3, 0, 0, 2, 0, 0, 0, 0 },
            { 2, 4, 0, 0, 0, 0, 0, 9, 3 },
            { 0, 0, 0, 0, 6, 0, 0, 1, 0 },
            { 0, 0, 0, 6, 3, 0, 0, 7, 5 },
            { 0, 2, 0, 0, 9, 0, 0, 0, 6 },
            { 0, 7, 0, 1, 0, 0, 4, 0, 0 }
        };
        SolveSudoku(sudoku);
        PrintSudoku(sudoku);
    }

    static private void SolveSudoku(int[,] sudoku)
    {
        bool isSolved;
        ImmutableHashSet<int>[,] possibilityMatrix = new ImmutableHashSet<int>[9, 9];

        do
        {
            int[,] prevSudoku;
            do
            {
                prevSudoku = (int[,])sudoku.Clone();
                Method1(sudoku, possibilityMatrix);
            } while (!AreEqual(prevSudoku, sudoku));

            Method2(sudoku, possibilityMatrix);
            isSolved = sudoku.Cast<int>().All(n => n != 0);
        } while (!isSolved);
    }

    private static bool AreEqual(int[,] sudoku1, int[,] sudoku2)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku1[i, j] != sudoku2[i, j]) return false;
            }
        }

        return true;
    }

    private static void Method1(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[i, j] != 0) continue;

                ImmutableHashSet<int> missingRowNumbers = GetMissingRowNumbers(sudoku, i);

                if (missingRowNumbers.Count == 1)
                {
                    sudoku[i, j] = missingRowNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> missingColumnNumbers = GetMissingColumnNumbers(sudoku, j);

                if (missingColumnNumbers.Count == 1)
                {
                    sudoku[i, j] = missingColumnNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> missingSubMatrixNumbers = GetMissingSubMatrixNumbers(sudoku, i, j);

                if (missingSubMatrixNumbers.Count == 1)
                {
                    sudoku[i, j] = missingSubMatrixNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> possibleNumbers = missingSubMatrixNumbers.Intersect(missingColumnNumbers.Intersect(missingRowNumbers));

                if (possibleNumbers.Count == 1)
                {
                    sudoku[i, j] = possibleNumbers.Single();
                    continue;
                }

                possibilityMatrix[i, j] = possibleNumbers;
            }
        }
    }

    static private void Method2(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[i, j] != 0) continue;

                ImmutableHashSet<int> possibleNumbersInColumn = GetPossibleNumbersInColumn(sudoku, possibilityMatrix, i, j);

                if (possibleNumbersInColumn.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInColumn.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInRow = GetPossibleNumbersInRow(sudoku, possibilityMatrix, i, j);

                if (possibleNumbersInRow.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInRow.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInSubMatrix = GetPossibleNumbersInSubMatrix(sudoku, possibilityMatrix, i, j);

                if (possibleNumbersInSubMatrix.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInSubMatrix.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }
            }
        }

        static ImmutableHashSet<int> GetPossibleNumbersInColumn(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, int i, int j)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int m = 0; m < 9; m++)
            {
                if (sudoku[m, j] != 0) continue;
                if (i == m) continue;

                notPossibleNumbers.UnionWith(possibilityMatrix[m, j]);
            }

            return possibilityMatrix[i, j].Except(notPossibleNumbers);
        }

        static ImmutableHashSet<int> GetPossibleNumbersInRow(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, int i, int j)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int n = 0; n < 9; n++)
            {
                if (sudoku[i, n] != 0) continue;
                if (j == n) continue;

                notPossibleNumbers.UnionWith(possibilityMatrix[i, n]);
            }

            return possibilityMatrix[i, j].Except(notPossibleNumbers);
        }

        static ImmutableHashSet<int> GetPossibleNumbersInSubMatrix(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, int i, int j)
        {
            var (rowStart, rowEnd, columnStart, columnEnd) = GetSubMatrix(i, j);

            HashSet<int> notPossibleNumbers = [];
            for (int m = rowStart; m < rowEnd; m++)
            {
                for (int n = columnStart; n < columnEnd; n++)
                {
                    if (sudoku[m, n] != 0) continue;
                    if (i == m && j == n) continue;

                    notPossibleNumbers.UnionWith(possibilityMatrix[m, n]);
                }
            }

            return possibilityMatrix[i, j].Except(notPossibleNumbers);
        }
    }

    static private ImmutableHashSet<int> GetMissingRowNumbers(int[,] sudoku, int i)
    {
        HashSet<int> presentNumbers = [];

        for (int j = 0; j < 9; j++)
        {
            if (sudoku[i, j] == 0) continue;

            presentNumbers.Add(sudoku[i, j]);
        }

        return _allNumbers.Except(presentNumbers);
    }

    static private ImmutableHashSet<int> GetMissingColumnNumbers(int[,] sudoku, int j)
    {
        HashSet<int> presentNumbers = [];

        for (int i = 0; i < 9; i++)
        {
            if (sudoku[i, j] == 0) continue;

            presentNumbers.Add(sudoku[i, j]);
        }

        return _allNumbers.Except(presentNumbers);
    }

    static private ImmutableHashSet<int> GetMissingSubMatrixNumbers(int[,] sudoku, int i, int j)
    {
        HashSet<int> presentNumbers = [];
        var (rowStart, rowEnd, columnStart, columnEnd) = GetSubMatrix(i, j);

        for (int m = rowStart; m < rowEnd; m++)
        {
            for (int n = columnStart; n < columnEnd; n++)
            {
                if (sudoku[m, n] == 0) continue;

                presentNumbers.Add(sudoku[m, n]);
            }
        }

        return _allNumbers.Except(presentNumbers);
    }

    static private (int rowStart, int rowEnd, int columnStart, int columnEnd) GetSubMatrix(int i, int j)
    {
        int rowStart = GetStartIndex(i);
        int columnStart = GetStartIndex(j);
        int rowEnd = rowStart + 3;
        int columnEnd = columnStart + 3;

        return (rowStart, rowEnd, columnStart, columnEnd);

        static int GetStartIndex(int positionIndex)
        {
            return positionIndex < 3 ? 0 : positionIndex < 6 ? 3 : 6;
        }
    }

    static private void PrintSudoku(int[,] sudoku)
    {
		var divider = "-------------------------------------";
        for (int i = 0; i < 9; i++)
        {
            Console.WriteLine(divider);
            for (int j = 0; j < 9; j++)
            {
                Console.Write($"| {sudoku[i, j]} ");
            }
            Console.Write("|");
            Console.Write(Environment.NewLine);
        }
        Console.WriteLine(divider);
    }
}
