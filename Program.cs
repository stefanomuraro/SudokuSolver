using System.Collections.Immutable;
using System.Linq;

public class Program
{
    private static readonly ImmutableHashSet<int> _allNumbers = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public static void Main()
    {
        int[,] problem = new int[9, 9]
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
        int[,] solution = SolveSudoku(problem);
        PrintSudoku(solution);
    }

    static private int[,] SolveSudoku(int[,] sudoku)
    {
        bool isSolved;
        ImmutableHashSet<int>[,] possibilityMatrix = new ImmutableHashSet<int>[9, 9];

        do
        {
            Method1(sudoku, possibilityMatrix);
            Method2(sudoku, possibilityMatrix);
            isSolved = sudoku.Cast<int>().All(n => n != 0);
        } while (!isSolved);

        return sudoku;
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

                ImmutableHashSet<int> possibleNumbers = possibilityMatrix[i, j].Except(notPossibleNumbers);

                if (possibleNumbers.Count == 1)
                {
                    sudoku[i, j] = possibleNumbers.Single();
                    continue;
                }
            }
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
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Console.Write(sudoku[i, j]);
            }
            Console.Write(Environment.NewLine);
        }
    }
}
