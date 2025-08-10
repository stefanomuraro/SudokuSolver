using System.Collections.Immutable;

public class Program
{
    private static readonly ImmutableHashSet<int> _allNumbers = [.. Enumerable.Range(1, 9)];

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

                ImmutableHashSet<int> missingSubMatrixNumbers = GetMissingSubMatrixNumbers(sudoku, new Position(i, j));

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

                Position position = new(i, j);

                ImmutableHashSet<int> possibleNumbersInColumn = GetPossibleNumbersInColumn(sudoku, possibilityMatrix, position);

                if (possibleNumbersInColumn.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInColumn.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInRow = GetPossibleNumbersInRow(sudoku, possibilityMatrix, position);

                if (possibleNumbersInRow.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInRow.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInSubMatrix = GetPossibleNumbersInSubMatrix(sudoku, possibilityMatrix, position);

                if (possibleNumbersInSubMatrix.Count == 1)
                {
                    sudoku[i, j] = possibleNumbersInSubMatrix.Single();
                    Method1(sudoku, possibilityMatrix);
                    continue;
                }
            }
        }

        static ImmutableHashSet<int> GetPossibleNumbersInColumn(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, Position position)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int i = 0; i < 9; i++)
            {
                if (sudoku[i, position.Column] != 0) continue;
                if (position.Row == i) continue;

                notPossibleNumbers.UnionWith(possibilityMatrix[i, position.Column]);
            }

            return possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }

        static ImmutableHashSet<int> GetPossibleNumbersInRow(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, Position position)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int j = 0; j < 9; j++)
            {
                if (sudoku[position.Row, j] != 0) continue;
                if (position.Column == j) continue;

                notPossibleNumbers.UnionWith(possibilityMatrix[position.Row, j]);
            }

            return possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }

        static ImmutableHashSet<int> GetPossibleNumbersInSubMatrix(int[,] sudoku, ImmutableHashSet<int>[,] possibilityMatrix, Position position)
        {
            SubMatrix subMatrix = new(position);

            HashSet<int> notPossibleNumbers = [];
            for (int i = subMatrix.Start.Row; i <= subMatrix.End.Row; i++)
            {
                for (int j = subMatrix.Start.Column; j <= subMatrix.End.Column; j++)
                {
                    if (sudoku[i, j] != 0) continue;
                    if (position.Row == i && position.Column == j) continue;

                    notPossibleNumbers.UnionWith(possibilityMatrix[i, j]);
                }
            }

            return possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }
    }

    static private ImmutableHashSet<int> GetMissingRowNumbers(int[,] sudoku, int row)
    {
        HashSet<int> presentNumbers = [];

        for (int j = 0; j < 9; j++)
        {
            if (sudoku[row, j] == 0) continue;

            presentNumbers.Add(sudoku[row, j]);
        }

        return _allNumbers.Except(presentNumbers);
    }

    static private ImmutableHashSet<int> GetMissingColumnNumbers(int[,] sudoku, int column)
    {
        HashSet<int> presentNumbers = [];

        for (int i = 0; i < 9; i++)
        {
            if (sudoku[i, column] == 0) continue;

            presentNumbers.Add(sudoku[i, column]);
        }

        return _allNumbers.Except(presentNumbers);
    }

    static private ImmutableHashSet<int> GetMissingSubMatrixNumbers(int[,] sudoku, Position position)
    {
        HashSet<int> presentNumbers = [];
        SubMatrix subMatrix = new(position);

        for (int i = subMatrix.Start.Row; i <= subMatrix.End.Row; i++)
        {
            for (int j = subMatrix.Start.Column; j <= subMatrix.End.Column; j++)
            {
                if (sudoku[i, j] == 0) continue;

                presentNumbers.Add(sudoku[i, j]);
            }
        }

        return _allNumbers.Except(presentNumbers);
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
