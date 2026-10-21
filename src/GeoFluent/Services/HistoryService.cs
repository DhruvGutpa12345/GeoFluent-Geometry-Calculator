using System.Collections.ObjectModel;

namespace GeoFluent.Services;

public readonly record struct HistoryEntry(
    string ShapeName,
    string Category,
    string PrimaryMetric,
    string Timestamp
);

public static class HistoryService
{
    public static ObservableCollection<HistoryEntry> Entries { get; } = [];

    public static void Add(string shapeName, string category, string primaryMetric)
    {
        Entries.Insert(0, new(shapeName, category, primaryMetric, DateTime.Now.ToString("g")));
        if (Entries.Count > 100)
        {
            Entries.RemoveAt(Entries.Count - 1);
        }
    }

    public static void Clear() => Entries.Clear();
}
