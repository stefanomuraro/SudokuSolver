using System.Collections.Immutable;

namespace SudokuSolver.App;

public static class PuzzleSolver
{
    private static readonly ImmutableHashSet<int> _allDigits = [.. Enumerable.Range(1, 9)];
    private static int[,] _grid = new int[9, 9];
    private static readonly ImmutableHashSet<int>[,] _candidates = new ImmutableHashSet<int>[9, 9];

    public static PuzzleResult Run(int[,] puzzle)
    {
        if (!PuzzleValidator.IsValidPuzzle(puzzle, out string error))
            return PuzzleResult.Failure(error);

        _grid = puzzle;

        Solve();

        return PuzzleResult.Success(_grid);
    }

    private static void Solve()
    {
        do
        {
            CalculateCandidates();
            FillFirstSingle();
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

        void FillFirstSingle()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (_grid[i, j] != 0) continue;

                    if (_candidates[i, j].Count == 1)
                    {
                        _grid[i, j] = _candidates[i, j].Single();
                        return;
                    }

                    Position position = new(i, j);

                    ImmutableHashSet<int> columnCandidates = GetColumnCandidates(position);

                    if (columnCandidates.Count == 1)
                    {
                        _grid[i, j] = columnCandidates.Single();
                        return;
                    }

                    ImmutableHashSet<int> rowCandidates = GetRowCandidates(position);

                    if (rowCandidates.Count == 1)
                    {
                        _grid[i, j] = rowCandidates.Single();
                        return;
                    }

                    ImmutableHashSet<int> boxCandidates = GetBoxCandidates(position);

                    if (boxCandidates.Count == 1)
                    {
                        _grid[i, j] = boxCandidates.Single();
                        return;
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