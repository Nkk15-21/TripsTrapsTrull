namespace TripsTrapsTrull.Core;

public enum CellState { Empty, X, O }
public enum Player { X, O }

public class GameEngine // логика игры
{
    public int Size { get; }
    public CellState[,] Board { get; }
    public Player CurrentPlayer { get; private set; } = Player.X;
    public bool IsGameOver { get; private set; }
    public CellState Winner { get; private set; } = CellState.Empty;

    public GameEngine(int size = 3) // размер доски size x size
    {
        Size = size;
        Board = new CellState[size, size];
    }

    public void Reset(Player? first = null) // сброс игры
    {
        Array.Clear(Board, 0, Board.Length);
        Winner = CellState.Empty;
        IsGameOver = false;
        CurrentPlayer = first ?? Player.X;
    }

    public bool MakeMove(int r, int c) // выйгрыш или ничья
    {
        if (IsGameOver) return false;
        if (Board[r, c] != CellState.Empty) return false;

        var symbol = (CurrentPlayer == Player.X) ? CellState.X : CellState.O;
        Board[r, c] = symbol;

        if (CheckWin(r, c))
        {
            Winner = symbol;
            IsGameOver = true;
        }
        else if (IsFull())
        {
            Winner = CellState.Empty; // ничья
            IsGameOver = true;
        }
        else
        {
            CurrentPlayer = (CurrentPlayer == Player.X) ? Player.O : Player.X;
        }

        return true;
    }

    bool IsFull() // проверка на заполненность доски
    {
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (Board[r, c] == CellState.Empty) return false;
        return true;
    }

    bool CheckWin(int r, int c) // проверка на выйгрыш
    {
        var me = Board[r, c];
        if (me == CellState.Empty) return false;

        // ряд
        bool rowOk = true;
        for (int j = 0; j < Size; j++)
            if (Board[r, j] != me) { rowOk = false; break; }
        if (rowOk) return true;

        // колонка
        bool colOk = true;
        for (int i = 0; i < Size; i++)
            if (Board[i, c] != me) { colOk = false; break; }
        if (colOk) return true;

        // главная диагональ
        if (r == c)
        {
            bool d1Ok = true;
            for (int i = 0; i < Size; i++)
                if (Board[i, i] != me) { d1Ok = false; break; }
            if (d1Ok) return true;
        }

        // побочная диагональ
        if (r + c == Size - 1)
        {
            bool d2Ok = true;
            for (int i = 0; i < Size; i++)
                if (Board[i, Size - 1 - i] != me) { d2Ok = false; break; }
            if (d2Ok) return true;
        }

        return false;
    }
}
