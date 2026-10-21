using System.Globalization;
using GeoFluent.Services;

namespace GeoFluent.Math;

public enum UnitType
{
    Length,
    Area,
    Volume,
    Angle,
    Count
}

public readonly record struct UnitDefinition(string Id, string Name, string Symbol, double Factor, bool IsBase = false);

public static class UnitConverter
{
    public static readonly UnitDefinition[] LengthUnits =
    [
        new("mm", "Millimeters (mm)", "mm", 0.001),
        new("cm", "Centimeters (cm)", "cm", 0.01),
        new("dm", "Decimeters (dm)", "dm", 0.1),
        new("m", "Meters (m)", "m", 1.0, true),
        new("km", "Kilometers (km)", "km", 1000.0),
        new("in", "Inches (in)", "in", 0.0254),
        new("ft", "Feet (ft)", "ft", 0.3048),
        new("yd", "Yards (yd)", "yd", 0.9144),
        new("mi", "Miles (mi)", "mi", 1609.344),
        new("nmi", "Nautical Miles (nmi)", "nmi", 1852.0)
    ];

    public static readonly UnitDefinition[] AreaUnits =
    [
        new("mm2", "Square Millimeters (mm²)", "mm²", 1e-6),
        new("cm2", "Square Centimeters (cm²)", "cm²", 0.0001),
        new("dm2", "Square Decimeters (dm²)", "dm²", 0.01),
        new("m2", "Square Meters (m²)", "m²", 1.0, true),
        new("ha", "Hectares (ha)", "ha", 10000.0),
        new("km2", "Square Kilometers (km²)", "km²", 1e6),
        new("sq_in", "Square Inches (sq in)", "in²", 0.00064516),
        new("sq_ft", "Square Feet (sq ft)", "ft²", 0.09290304),
        new("sq_yd", "Square Yards (sq yd)", "yd²", 0.83612736),
        new("acre", "Acres (ac)", "ac", 4046.8564224),
        new("sq_mi", "Square Miles (sq mi)", "mi²", 2589988.110336)
    ];

    public static readonly UnitDefinition[] VolumeUnits =
    [
        new("mm3", "Cubic Millimeters (mm³)", "mm³", 1e-9),
        new("cm3", "Cubic Centimeters (cm³ / mL)", "cm³", 1e-6),
        new("dm3_l", "Liters (L)", "L", 0.001),
        new("m3", "Cubic Meters (m³)", "m³", 1.0, true),
        new("cu_in", "Cubic Inches (cu in)", "in³", 0.000016387064),
        new("cu_ft", "Cubic Feet (cu ft)", "ft³", 0.028316846592),
        new("cu_yd", "Cubic Yards (cu yd)", "yd³", 0.764554857984),
        new("fl_oz", "Fluid Ounces (US fl oz)", "fl oz", 0.0000295735295625),
        new("cup", "Cups (US cup)", "cup", 0.0002365882365),
        new("pt", "Pints (US pt)", "pt", 0.000473176473),
        new("qt", "Quarts (US qt)", "qt", 0.000946352946),
        new("gal_us", "Gallons (US gal)", "gal", 0.003785411784),
        new("gal_uk", "Gallons (UK gal)", "imp gal", 0.00454609)
    ];

    public static readonly UnitDefinition[] AngleUnits =
    [
        new("deg", "Degrees (°)", "°", System.Math.PI / 180.0),
        new("rad", "Radians (rad)", "rad", 1.0, true),
        new("grad", "Gradians (grad)", "grad", System.Math.PI / 200.0)
    ];

    public static ReadOnlySpan<UnitDefinition> GetUnits(UnitType type) => type switch
    {
        UnitType.Length => LengthUnits,
        UnitType.Area => AreaUnits,
        UnitType.Volume => VolumeUnits,
        UnitType.Angle => AngleUnits,
        _ => []
    };

    public static double ToBase(double value, string unitId, UnitType type)
    {
        if (double.IsNaN(value) || type == UnitType.Count) return value;
        var units = GetUnits(type);
        foreach (var u in units)
        {
            if (u.Id.Equals(unitId, StringComparison.OrdinalIgnoreCase))
                return value * u.Factor;
        }
        return value;
    }

    public static double FromBase(double baseValue, string targetUnitId, UnitType type)
    {
        if (double.IsNaN(baseValue) || type == UnitType.Count) return baseValue;
        var units = GetUnits(type);
        foreach (var u in units)
        {
            if (u.Id.Equals(targetUnitId, StringComparison.OrdinalIgnoreCase))
                return baseValue / u.Factor;
        }
        return baseValue;
    }

    public static double Convert(double value, string fromUnitId, string toUnitId, UnitType type)
    {
        if (fromUnitId.Equals(toUnitId, StringComparison.OrdinalIgnoreCase)) return value;
        double baseVal = ToBase(value, fromUnitId, type);
        return FromBase(baseVal, toUnitId, type);
    }

    public static string GetDefaultUnitId(UnitType type) => type switch
    {
        UnitType.Length => "cm",
        UnitType.Area => "cm2",
        UnitType.Volume => "cm3",
        UnitType.Angle => "deg",
        _ => ""
    };

    public static string GetSymbol(string unitId, UnitType type)
    {
        var units = GetUnits(type);
        foreach (var u in units)
        {
            if (u.Id.Equals(unitId, StringComparison.OrdinalIgnoreCase))
                return u.Symbol;
        }
        return unitId;
    }

    /// <summary>
    /// Formats a numerical value according to precision settings.
    /// When precision is -1 (or omitted), intelligent Auto precision formatting is applied.
    /// </summary>
    public static string FormatNumber(double val, int? precision = null)
    {
        if (double.IsNaN(val)) return "—";
        if (double.IsPositiveInfinity(val)) return "∞";
        if (double.IsNegativeInfinity(val)) return "-∞";
        if (System.Math.Abs(val) < 1e-12) return "0";

        int effectivePrec = precision ?? UserSettingsService.Precision;

        // Fixed precision mode (0, 2, 3, 4, 6, 8, etc.)
        if (effectivePrec >= 0)
        {
            if (System.Math.Abs(val) >= 1e12 || (System.Math.Abs(val) < 1e-4 && System.Math.Abs(val) > 0))
                return val.ToString($"E{effectivePrec}", CultureInfo.CurrentCulture);

            return val.ToString($"N{effectivePrec}", CultureInfo.CurrentCulture);
        }

        // Auto (Intelligent) precision mode
        // 1. Extreme numbers outside typical engineering scales use compact general notation
        if (System.Math.Abs(val) >= 1e10 || (System.Math.Abs(val) < 1e-4 && System.Math.Abs(val) > 0))
        {
            return val.ToString("G6", CultureInfo.CurrentCulture);
        }

        // 2. Integers or near-integers (e.g. 25.0, 1000.000000001) display cleanly without decimal points
        double nearestInt = System.Math.Round(val);
        if (System.Math.Abs(val - nearestInt) < 1e-9)
        {
            return nearestInt.ToString("N0", CultureInfo.CurrentCulture);
        }

        // 3. Floating numbers: round to at most 6 decimal places and strip redundant trailing zeros
        double rounded = System.Math.Round(val, 6, MidpointRounding.AwayFromZero);
        if (System.Math.Abs(rounded - System.Math.Round(rounded)) < 1e-9)
        {
            return System.Math.Round(rounded).ToString("N0", CultureInfo.CurrentCulture);
        }

        string formatted = rounded.ToString("N6", CultureInfo.CurrentCulture);
        string decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        if (formatted.Contains(decSep))
        {
            formatted = formatted.TrimEnd('0');
            if (formatted.EndsWith(decSep))
            {
                formatted = formatted[..^decSep.Length];
            }
        }
        return formatted;
    }
}
