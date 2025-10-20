namespace TripsTrapsTrull.Core;

public enum CellState { Empty, X, O }
public enum Player { X, O }

public class GameEngine
{
    public int Size { get; }
    public CellState[,] Board { get; }
    public Player CurrentPlayer { get; private set; } = Player.X;
    public bool IsGameOver { get; private set; }
    public CellState Winner { get; private set; } = CellState.Empty;

    public GameEngine(int size = 3)
    {
        Size = size;
        Board = new CellState[size, size];
    }

    public void Reset(Player? first = null)
    {
        Array.Clear(Board, 0, Board.Length);
        Winner = CellState.Empty;
        IsGameOver = false;
        CurrentPlayer = first ?? Player.X;
    }

    public bool MakeMove(int r, int c)
    {
        if (IsGameOver) return false;
        if (Board[r, c] != CellState.Empty) return false;

        Board[r, c] = (CurrentPlayer == Player.X) ? CellState.X : CellState.O;

        if (CheckWin(r, c))
        {
            Winner = Board[r, c];
            IsGameOver = true;
            return true;
        }
        if (IsFull())
        {
            Winner = CellState.Empty;
            IsGameOver = true;
            return true;
        }

        CurrentPlayer = (CurrentPlayer == Player.X) ? Player.O : Player.X;
        return true;
    }

    bool IsFull()
    {
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (Board[r, c] == CellState.Empty) return false;
        return true;
    }

    bool CheckWin(int lastR, int lastC)
    {
        var me = Board[lastR, lastC];
        if (me == CellState.Empty) return false;

        bool rowOk = true;
        for (int c = 0; c < Size; c++)
            if (Board[lastR, c] != me) { rowOk = false; break; }

        bool colOk = true;
        for (int r = 0; r < Size; r++)
            if (Board[r, lastC] != me) { colOk = false; break; }

        bool d1Ok = true;
        if (lastR == lastC)
            for (int i = 0; i < Size; i++)
                if (Board[i, i] != me) { d1Ok = false; break; }
                else d1Ok = false;

        bool d2Ok = true;
        if (lastR + lastC == Size - 1)
            for (int i = 0; i < Size; i++)
                if (Board[i, Size - 1 - i] != me) { d2Ok = false; break; }
                else d2Ok = false;

        return rowOk || colOk || d1Ok || d2Ok;
    }
}
