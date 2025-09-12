using System.Collections.Immutable;

public static class SudokuSolver
{
    private const int MinStartingNumberCount = 17;
    private static readonly ImmutableHashSet<int> _allPossibleNumbers = [.. Enumerable.Range(1, 9)];
    private static int[,] _sudoku = new int[9, 9];
    private static readonly ImmutableHashSet<int>[,] _possibilityMatrix = new ImmutableHashSet<int>[9, 9];

    public static void Run(int[,] puzzle)
    {
        int startingNumberCount = puzzle.Cast<int>().Count(n => n != 0);
        if (startingNumberCount < MinStartingNumberCount)
            throw new ArgumentException("A standard 9x9 Sudoku puzzle requires at least 17 starting numbers to guarantee a unique solution", nameof(puzzle));

        _sudoku = puzzle;

        Solve();
        Print();
    }

    private static void Print()
    {
        var divider = "-------------------------------------";
        for (int i = 0; i < 9; i++)
        {
            Console.WriteLine(divider);
            for (int j = 0; j < 9; j++)
            {
                Console.Write($"| {_sudoku[i, j]} ");
            }
            Console.Write("|");
            Console.Write(Environment.NewLine);
        }
        Console.WriteLine(divider);
    }

    private static void Solve()
    {
        bool isSolved;

        do
        {
            int[,] prevSudoku;
            do
            {
                prevSudoku = (int[,])_sudoku.Clone();
                Method1();
            } while (!AreEqual(prevSudoku));

            Method2();
            isSolved = _sudoku.Cast<int>().All(n => n != 0);
        } while (!isSolved);
    }

    private static bool AreEqual(int[,] prevSudoku)
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (prevSudoku[i, j] != _sudoku[i, j]) return false;
            }
        }

        return true;
    }

    private static void Method1()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (_sudoku[i, j] != 0) continue;

                ImmutableHashSet<int> missingRowNumbers = GetMissingRowNumbers(i);

                if (missingRowNumbers.Count == 1)
                {
                    _sudoku[i, j] = missingRowNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> missingColumnNumbers = GetMissingColumnNumbers(j);

                if (missingColumnNumbers.Count == 1)
                {
                    _sudoku[i, j] = missingColumnNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> missingSubMatrixNumbers = GetMissingSubMatrixNumbers(new Position(i, j));

                if (missingSubMatrixNumbers.Count == 1)
                {
                    _sudoku[i, j] = missingSubMatrixNumbers.Single();
                    continue;
                }

                ImmutableHashSet<int> possibleNumbers = missingSubMatrixNumbers.Intersect(missingColumnNumbers.Intersect(missingRowNumbers));

                if (possibleNumbers.Count == 1)
                {
                    _sudoku[i, j] = possibleNumbers.Single();
                    continue;
                }

                _possibilityMatrix[i, j] = possibleNumbers;
            }
        }

        ImmutableHashSet<int> GetMissingRowNumbers(int row)
        {
            HashSet<int> presentNumbers = [];

            for (int j = 0; j < 9; j++)
            {
                if (_sudoku[row, j] == 0) continue;

                presentNumbers.Add(_sudoku[row, j]);
            }

            return _allPossibleNumbers.Except(presentNumbers);
        }

        ImmutableHashSet<int> GetMissingColumnNumbers(int column)
        {
            HashSet<int> presentNumbers = [];

            for (int i = 0; i < 9; i++)
            {
                if (_sudoku[i, column] == 0) continue;

                presentNumbers.Add(_sudoku[i, column]);
            }

            return _allPossibleNumbers.Except(presentNumbers);
        }

        ImmutableHashSet<int> GetMissingSubMatrixNumbers(Position position)
        {
            HashSet<int> presentNumbers = [];
            SubMatrix subMatrix = new(position);

            for (int i = subMatrix.Start.Row; i <= subMatrix.End.Row; i++)
            {
                for (int j = subMatrix.Start.Column; j <= subMatrix.End.Column; j++)
                {
                    if (_sudoku[i, j] == 0) continue;

                    presentNumbers.Add(_sudoku[i, j]);
                }
            }

            return _allPossibleNumbers.Except(presentNumbers);
        }
    }

    private static void Method2()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (_sudoku[i, j] != 0) continue;

                Position position = new(i, j);

                ImmutableHashSet<int> possibleNumbersInColumn = GetPossibleNumbersInColumn(position);

                if (possibleNumbersInColumn.Count == 1)
                {
                    _sudoku[i, j] = possibleNumbersInColumn.Single();
                    Method1();
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInRow = GetPossibleNumbersInRow(position);

                if (possibleNumbersInRow.Count == 1)
                {
                    _sudoku[i, j] = possibleNumbersInRow.Single();
                    Method1();
                    continue;
                }

                ImmutableHashSet<int> possibleNumbersInSubMatrix = GetPossibleNumbersInSubMatrix(position);

                if (possibleNumbersInSubMatrix.Count == 1)
                {
                    _sudoku[i, j] = possibleNumbersInSubMatrix.Single();
                    Method1();
                    continue;
                }
            }
        }

        ImmutableHashSet<int> GetPossibleNumbersInColumn(Position position)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int i = 0; i < 9; i++)
            {
                if (_sudoku[i, position.Column] != 0) continue;
                if (position.Row == i) continue;

                notPossibleNumbers.UnionWith(_possibilityMatrix[i, position.Column]);
            }

            return _possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }

        ImmutableHashSet<int> GetPossibleNumbersInRow(Position position)
        {
            HashSet<int> notPossibleNumbers = [];
            for (int j = 0; j < 9; j++)
            {
                if (_sudoku[position.Row, j] != 0) continue;
                if (position.Column == j) continue;

                notPossibleNumbers.UnionWith(_possibilityMatrix[position.Row, j]);
            }

            return _possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }

        ImmutableHashSet<int> GetPossibleNumbersInSubMatrix(Position position)
        {
            SubMatrix subMatrix = new(position);

            HashSet<int> notPossibleNumbers = [];
            for (int i = subMatrix.Start.Row; i <= subMatrix.End.Row; i++)
            {
                for (int j = subMatrix.Start.Column; j <= subMatrix.End.Column; j++)
                {
                    if (_sudoku[i, j] != 0) continue;
                    if (position.Row == i && position.Column == j) continue;

                    notPossibleNumbers.UnionWith(_possibilityMatrix[i, j]);
                }
            }

            return _possibilityMatrix[position.Row, position.Column].Except(notPossibleNumbers);
        }
    }
}