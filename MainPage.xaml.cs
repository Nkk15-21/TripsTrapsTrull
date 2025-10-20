using TripsTrapsTrull.Core;

namespace TripsTrapsTrull;

public partial class MainPage : ContentPage
{
    readonly GameEngine _engine = new(3);
    readonly Dictionary<Button, (int r, int c)> _map = new();
    readonly Random _rand = new();
    GameStats _stats = StatsStore.Load();

    public MainPage()
    {
        InitializeComponent();
        BuildBoard();
        UpdateTurnLabel();
    }

    void BuildBoard()
    {
        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();
        _map.Clear();

        for (int i = 0; i < _engine.Size; i++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        for (int r = 0; r < _engine.Size; r++)
            for (int c = 0; c < _engine.Size; c++)
            {
                var btn = new Button { FontSize = 36, BackgroundColor = Colors.Beige, TextColor = Colors.Black, CornerRadius = 8 };
                btn.Clicked += OnCellClicked;
                Grid.SetRow(btn, r);
                Grid.SetColumn(btn, c);
                BoardGrid.Children.Add(btn);
                _map[btn] = (r, c);
            }
    }

    async void OnCellClicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn) return;
        var (r, c) = _map[btn];

        if (!_engine.MakeMove(r, c)) return;

        var state = _engine.Board[r, c];
        btn.Text = state == CellState.X ? "X" : "O";
        btn.TextColor = state == CellState.X ? Colors.DodgerBlue : Colors.IndianRed;
        btn.IsEnabled = false;

        if (_engine.IsGameOver)
        {
            foreach (var child in BoardGrid.Children.OfType<Button>())
                child.IsEnabled = false;

            string msg;
            if (_engine.Winner == CellState.X) { _stats.XWins++; msg = "X võitis!"; }
            else if (_engine.Winner == CellState.O) { _stats.OWins++; msg = "O võitis!"; }
            else { _stats.Draws++; msg = "Viik!"; }
            StatsStore.Save(_stats);

            var again = await DisplayAlert("Mäng läbi", $"{msg}\nKas soovid veel mängida?", "Jah", "Ei");
            if (again)
            {
                _engine.Reset();
                BuildBoard();
                UpdateTurnLabel();
            }
            else
            {
                UpdateTurnLabel(final: true);
            }
            return;
        }

        UpdateTurnLabel();
    }

    void UpdateTurnLabel(bool final = false)
    {
        if (final) { TurnLabel.Text = "Mäng on läbi."; return; }
        var p = _engine.CurrentPlayer == Player.X ? "X" : "O";
        TurnLabel.Text = $"Kelle kord? — {p}";
    }

    void OnStartClicked(object? s, EventArgs e)
    {
        _engine.Reset();
        BuildBoard();
        UpdateTurnLabel();
    }

    void OnWhoStartsClicked(object? s, EventArgs e)
    {
        var first = (_rand.NextDouble() < 0.5) ? Player.X : Player.O;
        _engine.Reset(first);
        BuildBoard();
        UpdateTurnLabel();
    }

    async void OnStatsClicked(object? s, EventArgs e)
    {
        await DisplayAlert("Statistika",
            $"Mänge kokku: {_stats.Games}\nX võidud: {_stats.XWins}\nO võidud: {_stats.OWins}\nViigid: {_stats.Draws}",
            "OK");
    }

    async void OnClearStatsClicked(object? s, EventArgs e)
    {
        var ok = await DisplayAlert("Nulli statistika", "Kas oled kindel?", "Jah", "Ei");
        if (!ok) return;
        _stats = new GameStats();
        StatsStore.Clear();
        await DisplayAlert("Valmis", "Statistika nullitud.", "OK");
    }
}
