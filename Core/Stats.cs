using System.Text.Json;
using Microsoft.Maui.Storage;

namespace TripsTrapsTrull.Core;

public class GameStats // статистика игр
{
    public int XWins { get; set; }
    public int OWins { get; set; }
    public int Draws { get; set; }
    public int Games => XWins + OWins + Draws;
}

public static class StatsStore // сохранение и загрузка статистики
{
    const string Key = "trips_stats_v1";
    public static GameStats Load() // загрузка статистики
    {
        var json = Preferences.Get(Key, "");
        return string.IsNullOrWhiteSpace(json)
            ? new GameStats()
            : (JsonSerializer.Deserialize<GameStats>(json) ?? new GameStats());
    }
    public static void Save(GameStats s) =>
        Preferences.Set(Key, JsonSerializer.Serialize(s));
    public static void Clear() => Preferences.Remove(Key);
}
