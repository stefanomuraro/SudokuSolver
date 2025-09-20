using SudokuSolver.App;

namespace SudokuSolver.Tests;

public class SudokuSolverTests
{
    [Fact]
    public void Solve_Valid_Puzzle()
    {
        var puzzle = new int[9, 9]
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

        PuzzleResult actual = Solver.Run(puzzle);

        var solvedPuzzle = new int[9, 9]
            {
                { 7, 6, 1, 5, 8, 3, 9, 2, 4 },
                { 5, 9, 3, 2, 4, 1, 8, 6, 7 },
                { 4, 8, 2, 9, 7, 6, 3, 5, 1 },
                { 1, 3, 6, 7, 2, 9, 5, 4, 8 },
                { 2, 4, 7, 8, 1, 5, 6, 9, 3 },
                { 8, 5, 9, 3, 6, 4, 7, 1, 2 },
                { 9, 1, 4, 6, 3, 8, 2, 7, 5 },
                { 3, 2, 5, 4, 9, 7, 1, 8, 6 },
                { 6, 7, 8, 1, 5, 2, 4, 3, 9 }
            };
        var expected = PuzzleResult.Success(solvedPuzzle);

        Assert.Equivalent(expected, actual);
    }
}