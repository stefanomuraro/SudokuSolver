namespace SudokuSolver.App;

public class PuzzleResult
{
    public bool IsSuccessful { get; }
    public int[,]? SolvedPuzzle { get; }
    public string? ErrorMessage { get; }

    private PuzzleResult(bool isSuccessful, int[,]? solvedPuzzle, string? errorMessage)
    {
        IsSuccessful = isSuccessful;
        SolvedPuzzle = solvedPuzzle;
        ErrorMessage = errorMessage;
    }

    public static PuzzleResult Success(int[,] solvedPuzzle) => new(true, solvedPuzzle, null);
    public static PuzzleResult Failure(string errorMessage) => new(false, null, errorMessage);
}
