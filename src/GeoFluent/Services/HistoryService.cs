using System.Collections.ObjectModel;
using GeoFluent.Math;

namespace GeoFluent.Services;

public record SavedInputRecord(string Key, string Label, double Value, string UnitId, string UnitSymbol);
public record SavedOutputRecord(string Label, string ValueWithUnit, string Symbol);

public record FullHistoryEntry(
    string Id,
    string ShapeId,
    string ShapeName,
    string Category, // "2D" | "3D"
    string MethodId,
    string MethodName,
    IReadOnlyList<SavedInputRecord> Inputs,
    IReadOnlyList<SavedOutputRecord> Outputs,
    IReadOnlyList<CalculationStep> Steps,
    string Timestamp
)
{
    public string PrimaryMetric => Outputs.Count > 0 ? Outputs[0].ValueWithUnit : "";

    public string ToMarkdown()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"# GeoFluent Geometry Report: {ShapeName}");
        sb.AppendLine();
        sb.AppendLine($"**Date**: {Timestamp}");
        sb.AppendLine($"**Category**: {Category} Geometry");
        sb.AppendLine($"**Method**: {MethodName}");
        sb.AppendLine();
        sb.AppendLine("## Input Parameters");
        sb.AppendLine();
        sb.AppendLine("| Parameter | Value | Unit |");
        sb.AppendLine("|---|---|---|");
        foreach (var inp in Inputs)
        {
            sb.AppendLine($"| {inp.Label} | {inp.Value} | {inp.UnitSymbol} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Calculated Results");
        sb.AppendLine();
        sb.AppendLine("| Property | Value | Symbol |");
        sb.AppendLine("|---|---|---|");
        foreach (var outP in Outputs)
        {
            sb.AppendLine($"| {outP.Label} | {outP.ValueWithUnit} | {outP.Symbol} |");
        }
        if (Steps != null && Steps.Count > 0)
        {
            sb.AppendLine();
            sb.AppendLine("## Step-by-Step Derivation");
            sb.AppendLine();
            for (int i = 0; i < Steps.Count; i++)
            {
                sb.AppendLine($"### {i + 1}. {Steps[i].Title}");
                sb.AppendLine($"- **Formula**: `{Steps[i].Formula}`");
                sb.AppendLine($"- **Substitution**: {Steps[i].Substitution}");
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }
}

public static class HistoryService
{
    public static ObservableCollection<FullHistoryEntry> Entries { get; } = [];

    // Pending restore request
    public static FullHistoryEntry? PendingRestore { get; set; }

    public static void Add(
        string shapeId,
        string shapeName,
        string category,
        string methodId,
        string methodName,
        IReadOnlyList<SavedInputRecord> inputs,
        IReadOnlyList<SavedOutputRecord> outputs,
        IReadOnlyList<CalculationStep> steps)
    {
        var entry = new FullHistoryEntry(
            Guid.NewGuid().ToString("N"),
            shapeId,
            shapeName,
            category,
            methodId,
            methodName,
            inputs,
            outputs,
            steps,
            DateTime.Now.ToString("g")
        );

        Entries.Insert(0, entry);
        if (Entries.Count > 100)
        {
            Entries.RemoveAt(Entries.Count - 1);
        }
    }

    public static void Remove(string id)
    {
        var found = Entries.FirstOrDefault(e => e.Id == id);
        if (found != null)
        {
            Entries.Remove(found);
        }
    }

    public static void Clear() => Entries.Clear();

    public static string ExportAllToMarkdown()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("# GeoFluent Saved Calculation History Export");
        sb.AppendLine($"Exported at: {DateTime.Now:F}");
        sb.AppendLine();
        sb.AppendLine("---");
        sb.AppendLine();
        foreach (var entry in Entries)
        {
            sb.AppendLine(entry.ToMarkdown());
            sb.AppendLine("---");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
