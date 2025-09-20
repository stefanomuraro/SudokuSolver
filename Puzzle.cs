namespace SudokuSolver
{
    public class Puzzle
    {
        private static int[,] _puzzle = new int[9, 9];

        public Puzzle(int[,] puzzle)
        {
            _puzzle = puzzle;
        }

        public void SetCellDigit(Position position, int digit)
        {
            _puzzle[position.Row, position.Column] = digit;
            // CalculatePossibilityMatrix();
        }
    }
}