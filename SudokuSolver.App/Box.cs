namespace SudokuSolver.App;

public class Box
{
    public Position Start { get; }
    public Position End { get; }

    public Box(Position position)
    {
        int startRow = GetStartIndex(position.Row);
        int startColumn = GetStartIndex(position.Column);

        Start = new Position(startRow, startColumn);
        End = new Position(startRow + 2, startColumn + 2);
    }

    private static int GetStartIndex(int positionIndex)
    {
        return positionIndex < 3 ? 0 : positionIndex < 6 ? 3 : 6;
    }
}