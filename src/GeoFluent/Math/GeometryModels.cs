namespace GeoFluent.Math;

public readonly record struct InputDefinition(
    string Id,
    string Label,
    string Symbol,
    UnitType UnitType,
    double DefaultValue,
    double Min = 0.0001,
    double Max = double.MaxValue
);

public readonly record struct CalculationOutput(
    string Id,
    string Label,
    string Symbol,
    double Value, // In standard base unit (m, m², m³, rad) or raw count
    UnitType UnitType,
    bool IsPrimary = false,
    string? TextValue = null
)
{
    public string Format(string? unitId = null, int? precision = null)
    {
        if (TextValue != null) return TextValue;
        if (UnitType == UnitType.Count) return UnitConverter.FormatNumber(Value, precision);

        string targetUnit = unitId ?? UnitConverter.GetDefaultUnitId(UnitType);
        double converted = UnitConverter.FromBase(Value, targetUnit, UnitType);
        string sym = UnitConverter.GetSymbol(targetUnit, UnitType);
        return $"{UnitConverter.FormatNumber(converted, precision)} {sym}";
    }
}

public readonly record struct CalculationStep(
    string Title,
    string Formula,
    string Substitution
);

public readonly record struct Point2D(double X, double Y);

public readonly record struct Point3D(double X, double Y, double Z);

public readonly record struct Line3D(Point3D Start, Point3D End);

public record ShapeCalculationResult(
    IReadOnlyList<CalculationOutput> Outputs,
    IReadOnlyList<CalculationStep> Steps,
    IReadOnlyList<Point2D>? PolygonPoints = null,
    IReadOnlyList<Line3D>? SolidLines = null,
    double Inradius = 0,
    double Circumradius = 0
);

public record MethodDefinition(
    string Id,
    string Name,
    string Description,
    IReadOnlyList<InputDefinition> Inputs,
    Func<IReadOnlyDictionary<string, double>, ShapeCalculationResult> Calculate
);

public record ShapeDefinition(
    string Id,
    string Name,
    string Category, // "2D" | "3D"
    string Description,
    string Glyph, // Segoe Fluent Icon glyph
    IReadOnlyList<MethodDefinition> Methods
);
