namespace SudokuSolver.App;

public class SudokuResult
{
    public bool IsSuccessful { get; }
    public int[,]? SolvedPuzzle { get; }
    public string? ErrorMessage { get; }

    private SudokuResult(bool isSuccessful, int[,]? solvedPuzzle, string? errorMessage)
    {
        IsSuccessful = isSuccessful;
        SolvedPuzzle = solvedPuzzle;
        ErrorMessage = errorMessage;
    }

    public static SudokuResult Success(int[,] solvedPuzzle) => new(true, solvedPuzzle, null);
    public static SudokuResult Failure(string errorMessage) => new(false, null, errorMessage);
}
