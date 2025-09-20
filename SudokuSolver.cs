using System.Collections.Immutable;

namespace SudokuSolver
{
    public static class SudokuSolver
    {
        private const int MinGivenCount = 17;
        private static readonly ImmutableHashSet<int> _allDigits = [.. Enumerable.Range(1, 9)];
        private static int[,] _grid = new int[9, 9];
        private static readonly ImmutableHashSet<int>[,] _candidates = new ImmutableHashSet<int>[9, 9];

        // TODO add setter for _grid

        public static Response Run(int[,] puzzle)
        {
            // TODO verify puzzle in separate method
            int givenCount = puzzle.Cast<int>().Count(n => n != 0);
            if (givenCount < MinGivenCount)
                return new Response
                {
                    IsSuccessful = false,
                    ErrorMessage = "A standard 9x9 Sudoku puzzle requires at least 17 givens (starting numbers) to guarantee a unique solution"
                };

            // TODO add method to verify if puzzle is valid

            _grid = puzzle;

            Solve();
            Print();

            return new Response
            {
                IsSuccessful = true,
                SolvedPuzzle = _grid
            };
        }

        private static void Print()
        {
            var divider = "-------------------------------------";
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine(divider);
                for (int j = 0; j < 9; j++)
                {
                    Console.Write($"| {_grid[i, j]} ");
                }
                Console.Write("|");
                Console.Write(Environment.NewLine);
            }
            Console.WriteLine(divider);
        }

        private static void Solve()
        {
            do
            {
                CalculateCandidates();
                FillSingles();
            } while (!IsSolved());

            bool IsSolved() => _grid.Cast<int>().All(n => n != 0);

            void CalculateCandidates()
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (_grid[i, j] != 0) continue;

                        ImmutableHashSet<int> missingRowDigits = GetMissingRowDigits(i);

                        if (missingRowDigits.Count == 1)
                        {
                            _candidates[i, j] = missingRowDigits;
                            continue;
                        }

                        ImmutableHashSet<int> missingColumnDigits = GetMissingColumnDigits(j);

                        if (missingColumnDigits.Count == 1)
                        {
                            _candidates[i, j] = missingColumnDigits;
                            continue;
                        }

                        ImmutableHashSet<int> missingBoxDigits = GetMissingBoxDigits(new Position(i, j));

                        if (missingBoxDigits.Count == 1)
                        {
                            _candidates[i, j] = missingBoxDigits;
                            continue;
                        }

                        ImmutableHashSet<int> candidates = missingBoxDigits.Intersect(missingColumnDigits.Intersect(missingRowDigits));

                        _candidates[i, j] = candidates;
                    }
                }

                ImmutableHashSet<int> GetMissingRowDigits(int row)
                {
                    HashSet<int> existingDigits = [];

                    for (int j = 0; j < 9; j++)
                    {
                        if (_grid[row, j] == 0) continue;

                        existingDigits.Add(_grid[row, j]);
                    }

                    return _allDigits.Except(existingDigits);
                }

                ImmutableHashSet<int> GetMissingColumnDigits(int column)
                {
                    HashSet<int> existingDigits = [];

                    for (int i = 0; i < 9; i++)
                    {
                        if (_grid[i, column] == 0) continue;

                        existingDigits.Add(_grid[i, column]);
                    }

                    return _allDigits.Except(existingDigits);
                }

                ImmutableHashSet<int> GetMissingBoxDigits(Position position)
                {
                    HashSet<int> existingDigits = [];
                    Box box = new(position);

                    for (int i = box.Start.Row; i <= box.End.Row; i++)
                    {
                        for (int j = box.Start.Column; j <= box.End.Column; j++)
                        {
                            if (_grid[i, j] == 0) continue;

                            existingDigits.Add(_grid[i, j]);
                        }
                    }

                    return _allDigits.Except(existingDigits);
                }
            }

            void FillSingles()
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (_grid[i, j] != 0) continue;

                        if (_candidates[i, j].Count == 1)
                        {
                            _grid[i, j] = _candidates[i, j].Single();
                            CalculateCandidates();
                            continue;
                        }

                        Position position = new(i, j);

                        ImmutableHashSet<int> columnCandidates = GetColumnCandidates(position);

                        if (columnCandidates.Count == 1)
                        {
                            _grid[i, j] = columnCandidates.Single();
                            CalculateCandidates();
                            continue;
                        }

                        ImmutableHashSet<int> rowCandidates = GetRowCandidates(position);

                        if (rowCandidates.Count == 1)
                        {
                            _grid[i, j] = rowCandidates.Single();
                            CalculateCandidates();
                            continue;
                        }

                        ImmutableHashSet<int> boxCandidates = GetBoxCandidates(position);

                        if (boxCandidates.Count == 1)
                        {
                            _grid[i, j] = boxCandidates.Single();
                            CalculateCandidates();
                            continue;
                        }
                    }
                }

                ImmutableHashSet<int> GetColumnCandidates(Position position)
                {
                    HashSet<int> nonCandidates = [];
                    for (int i = 0; i < 9; i++)
                    {
                        if (_grid[i, position.Column] != 0) continue;
                        if (position.Row == i) continue;

                        nonCandidates.UnionWith(_candidates[i, position.Column]);
                    }

                    return _candidates[position.Row, position.Column].Except(nonCandidates);
                }

                ImmutableHashSet<int> GetRowCandidates(Position position)
                {
                    HashSet<int> nonCandidates = [];
                    for (int j = 0; j < 9; j++)
                    {
                        if (_grid[position.Row, j] != 0) continue;
                        if (position.Column == j) continue;

                        nonCandidates.UnionWith(_candidates[position.Row, j]);
                    }

                    return _candidates[position.Row, position.Column].Except(nonCandidates);
                }

                ImmutableHashSet<int> GetBoxCandidates(Position position)
                {
                    Box box = new(position);

                    HashSet<int> nonCandidates = [];
                    for (int i = box.Start.Row; i <= box.End.Row; i++)
                    {
                        for (int j = box.Start.Column; j <= box.End.Column; j++)
                        {
                            if (_grid[i, j] != 0) continue;
                            if (position.Row == i && position.Column == j) continue;

                            nonCandidates.UnionWith(_candidates[i, j]);
                        }
                    }

                    return _candidates[position.Row, position.Column].Except(nonCandidates);
                }
            }
        }
    }
}