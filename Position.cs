public class Position
{
    public int Row { get; }
    public int Column { get; }

    public Position(int row, int column)
    {
        if (row < 0 || row > 8)
            throw new ArgumentOutOfRangeException(nameof(row), "Row number must be in [0, 8]");
        if (column < 0 || column > 8)
            throw new ArgumentOutOfRangeException(nameof(column), "Column number must be in [0, 8]");

        Row = row;
        Column = column;
    }
}