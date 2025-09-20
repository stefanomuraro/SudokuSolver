namespace SudokuSolver.App;

public class Response
{
    public bool IsSuccessful { get; set; }
    public int[,]? SolvedPuzzle { get; set; }
    public string? ErrorMessage { get; set; }
}