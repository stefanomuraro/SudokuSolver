using SudokuSolver.App;

namespace SudokuSolver.Tests;

public class SudokuSolverTests
{
    public static IEnumerable<object[]> PuzzleData =>
        [
            [
                new int[9, 9]
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
                },
                new int[9, 9]
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
                }
            ],
            [
                new int[,]
                {
                    { 0, 0, 9, 6, 0, 4, 5, 0, 0 },
                    { 6, 0, 0, 0, 9, 0, 0, 0, 7 },
                    { 0, 7, 0, 0, 1, 0, 0, 4, 0 },
                    { 0, 1, 3, 0, 0, 0, 8, 6, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 6, 2, 0, 0, 0, 1, 9, 0 },
                    { 0, 4, 0, 0, 5, 0, 0, 3, 0 },
                    { 3, 0, 0, 0, 2, 0, 0, 0, 1 },
                    { 0, 0, 7, 3, 0, 1, 6, 0, 0 }
                },
                new int[,]
                {
                    { 1, 3, 9, 6, 7, 4, 5, 2, 8 },
                    { 6, 2, 4, 5, 9, 8, 3, 1, 7 },
                    { 5, 7, 8, 2, 1, 3, 9, 4, 6 },
                    { 7, 1, 3, 9, 4, 5, 8, 6, 2 },
                    { 9, 8, 5, 1, 6, 2, 4, 7, 3 },
                    { 4, 6, 2, 8, 3, 7, 1, 9, 5 },
                    { 8, 4, 1, 7, 5, 6, 2, 3, 9 },
                    { 3, 5, 6, 4, 2, 9, 7, 8, 1 },
                    { 2, 9, 7, 3, 8, 1, 6, 5, 4 }
                }
            ]
        ];


    [Theory]
    [MemberData(nameof(PuzzleData))]
    public void TrySolve_WithValidPuzzle_ReturnsExpectedSolution(int[,] puzzle, int[,] solution)
    {
        // Act
        SudokuResult result = App.SudokuSolver.TrySolve(puzzle);

        // Assert
        var expected = SudokuResult.Success(solution);
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void TrySolve_WithInvalidDimensions_ReturnsFailureResult()
    {
        // Arrange: create 8x9 instead of 9x9
        int[,] puzzle = new int[8, 9];

        // Act
        SudokuResult result = App.SudokuSolver.TrySolve(puzzle);

        // Assert
        var errorMessage = "Sudoku grid must be 9x9.";
        var expected = SudokuResult.Failure(errorMessage);
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void TrySolve_WithNotEnoughGivens_ReturnsFailureResult()
    {
        // Arrange: valid 9x9 but only one given
        int[,] puzzle = new int[9, 9];
        puzzle[0, 0] = 5;

        // Act
        SudokuResult result = App.SudokuSolver.TrySolve(puzzle);

        // Assert
        var errorMessage = "A standard 9x9 Sudoku puzzle requires at least 17 givens to guarantee a unique solution";
        var expected = SudokuResult.Failure(errorMessage);
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public void TrySolve_WithInvalidGivens_ReturnsFailureResult()
    {
        // Arrange: valid 9x9 with enough givens but an invalid value
        int[,] puzzle = new int[9, 9];

        // Put 17 givens
        for (int i = 0; i < 17; i++)
        {
            puzzle[i / 9, i % 9] = (i % 9) + 1; // values between 1–9
        }

        // Introduce an invalid given (outside range 1–9)
        puzzle[0, 0] = 42;

        // Act
        SudokuResult result = App.SudokuSolver.TrySolve(puzzle);

        // Assert
        var errorMessage = "Invalid givens.";
        var expected = SudokuResult.Failure(errorMessage);
        Assert.Equivalent(expected, result);
    }
}