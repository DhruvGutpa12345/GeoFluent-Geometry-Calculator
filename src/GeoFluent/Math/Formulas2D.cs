using System.Globalization;

namespace GeoFluent.Math;

public readonly record struct CalculationOutput(
    string Id,
    string Label,
    string Symbol,
    double Value,
    UnitType UnitType,
    bool IsPrimary = false,
    string? TextValue = null
)
{
    public string DisplayValue => TextValue ?? UnitConverter.FormatNumber(Value);
}

public readonly record struct CalculationStep(
    string Title,
    string Formula,
    string Substitution
);

public readonly record struct Point2D(double X, double Y);

public record ShapeCalculationResult(
    IReadOnlyList<CalculationOutput> Outputs,
    IReadOnlyList<CalculationStep> Steps,
    IReadOnlyList<Point2D>? PolygonPoints = null
);

public static class Formulas2D
{
    public static ShapeCalculationResult TriangleHeron(double a, double b, double c)
    {
        if (a + b <= c || a + c <= b || b + c <= a)
            throw new ArgumentException("Triangle Inequality Violated: The sum of any two sides must exceed the third.");

        double s = (a + b + c) / 2.0;
        double area = System.Math.Sqrt(System.Math.Max(0, s * (s - a) * (s - b) * (s - c)));
        double perimeter = a + b + c;

        double alpha = System.Math.Acos(System.Math.Clamp((b * b + c * c - a * a) / (2 * b * c), -1.0, 1.0));
        double beta = System.Math.Acos(System.Math.Clamp((a * a + c * c - b * b) / (2 * a * c), -1.0, 1.0));
        double gamma = System.Math.PI - alpha - beta;

        double ha = (2.0 * area) / a;
        double hb = (2.0 * area) / b;
        double hc = (2.0 * area) / c;

        double inradius = area / s;
        double circumradius = (a * b * c) / (4.0 * area);

        // Classification
        double[] sorted = [a, b, c];
        Array.Sort(sorted);
        string sideType = (System.Math.Abs(a - b) < 1e-9 && System.Math.Abs(b - c) < 1e-9) ? "Equilateral"
            : (System.Math.Abs(a - b) < 1e-9 || System.Math.Abs(b - c) < 1e-9 || System.Math.Abs(a - c) < 1e-9) ? "Isosceles"
            : "Scalene";

        double pythDiff = sorted[0] * sorted[0] + sorted[1] * sorted[1] - sorted[2] * sorted[2];
        string angleType = System.Math.Abs(pythDiff) < 1e-7 * sorted[2] * sorted[2] ? "Right-angled"
            : pythDiff < 0 ? "Obtuse" : "Acute";

        // Polygon points for visualization
        double cx = (c * c - a * a + b * b) / (2 * c);
        double cy = System.Math.Sqrt(System.Math.Max(0, b * b - cx * cx));
        List<Point2D> pts = [new(0, 0), new(c, 0), new(cx, cy)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("semi_p", "Semi-perimeter (s)", "s", s, UnitType.Length),
            new("alpha", "Angle α", "α", alpha * (180.0 / System.Math.PI), UnitType.Angle),
            new("beta", "Angle β", "β", beta * (180.0 / System.Math.PI), UnitType.Angle),
            new("gamma", "Angle γ", "γ", gamma * (180.0 / System.Math.PI), UnitType.Angle),
            new("ha", "Altitude to a (hₐ)", "hₐ", ha, UnitType.Length),
            new("hb", "Altitude to b (hb)", "hb", hb, UnitType.Length),
            new("hc", "Altitude to c (hc)", "hc", hc, UnitType.Length),
            new("inradius", "Inradius (r)", "r", inradius, UnitType.Length),
            new("circumradius", "Circumradius (R)", "R", circumradius, UnitType.Length),
            new("class", "Classification", "Type", 0, UnitType.Count, false, $"{sideType}, {angleType}")
        ];

        List<CalculationStep> steps =
        [
            new("Semi-perimeter", "s = (a + b + c) / 2", $"s = ({a} + {b} + {c}) / 2 = {s:F4}"),
            new("Heron's Area Formula", "A = √(s(s - a)(s - b)(s - c))", $"A = √({s:F2} · {s - a:F2} · {s - b:F2} · {s - c:F2}) = {area:F4}"),
            new("Perimeter", "P = a + b + c", $"P = {a} + {b} + {c} = {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Rectangle(double length, double width)
    {
        if (length <= 0 || width <= 0) throw new ArgumentException("Dimensions must be positive.");
        double area = length * width;
        double perimeter = 2 * (length + width);
        double diagonal = System.Math.Sqrt(length * length + width * width);
        double circumradius = diagonal / 2.0;

        List<Point2D> pts = [new(0, 0), new(length, 0), new(length, width), new(0, width)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("diagonal", "Diagonal (d)", "d", diagonal, UnitType.Length, true),
            new("circumradius", "Circumradius (R)", "R", circumradius, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = l · w", $"A = {length} · {width} = {area:F4}"),
            new("Perimeter Formula", "P = 2 · (l + w)", $"P = 2 · ({length} + {width}) = {perimeter:F4}"),
            new("Diagonal Formula", "d = √(l² + w²)", $"d = √({length}² + {width}²) = {diagonal:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Square(double side)
    {
        if (side <= 0) throw new ArgumentException("Side must be positive.");
        double area = side * side;
        double perimeter = 4 * side;
        double diagonal = side * System.Math.Sqrt(2.0);
        double inradius = side / 2.0;
        double circumradius = diagonal / 2.0;

        List<Point2D> pts = [new(0, 0), new(side, 0), new(side, side), new(0, side)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("diagonal", "Diagonal (d)", "d", diagonal, UnitType.Length, true),
            new("inradius", "Inradius (r)", "r", inradius, UnitType.Length),
            new("circumradius", "Circumradius (R)", "R", circumradius, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = a²", $"A = {side}² = {area:F4}"),
            new("Perimeter Formula", "P = 4 · a", $"P = 4 · {side} = {perimeter:F4}"),
            new("Diagonal Formula", "d = a · √2", $"d = {side} · √2 = {diagonal:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Circle(double radius)
    {
        if (radius <= 0) throw new ArgumentException("Radius must be positive.");
        double diameter = 2 * radius;
        double area = System.Math.PI * radius * radius;
        double circumference = 2 * System.Math.PI * radius;

        List<Point2D> pts = [];
        const int segments = 48;
        for (int i = 0; i <= segments; i++)
        {
            double theta = (2 * System.Math.PI * i) / segments;
            pts.Add(new(radius + radius * System.Math.Cos(theta), radius + radius * System.Math.Sin(theta)));
        }

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("circumference", "Circumference (C)", "C", circumference, UnitType.Length, true),
            new("diameter", "Diameter (d)", "d", diameter, UnitType.Length, true)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = π · r²", $"A = π · {radius}² = {area:F4}"),
            new("Circumference Formula", "C = 2 · π · r", $"C = 2 · π · {radius} = {circumference:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Parallelogram(double a, double b, double height)
    {
        if (a <= 0 || b <= 0 || height <= 0) throw new ArgumentException("Parameters must be positive.");
        if (height > b) throw new ArgumentException("Altitude height cannot exceed slant side b.");

        double area = a * height;
        double perimeter = 2 * (a + b);
        double angleRad = System.Math.Asin(System.Math.Clamp(height / b, -1.0, 1.0));
        double angleDeg = angleRad * (180.0 / System.Math.PI);
        double dx = System.Math.Sqrt(System.Math.Max(0, b * b - height * height));

        List<Point2D> pts = [new(0, 0), new(a, 0), new(a + dx, height), new(dx, height)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("angle", "Base Angle (θ)", "θ", angleDeg, UnitType.Angle)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = base · height", $"A = {a} · {height} = {area:F4}"),
            new("Perimeter Formula", "P = 2 · (a + b)", $"P = 2 · ({a} + {b}) = {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Rhombus(double d1, double d2)
    {
        if (d1 <= 0 || d2 <= 0) throw new ArgumentException("Diagonals must be positive.");
        double area = 0.5 * d1 * d2;
        double side = 0.5 * System.Math.Sqrt(d1 * d1 + d2 * d2);
        double perimeter = 4 * side;
        double inradius = (d1 * d2) / (4.0 * side);

        List<Point2D> pts = [new(d1 / 2.0, 0), new(d1, d2 / 2.0), new(d1 / 2.0, d2), new(0, d2 / 2.0)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("side", "Side Length (a)", "a", side, UnitType.Length, true),
            new("inradius", "Inradius (r)", "r", inradius, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = (d₁ · d₂) / 2", $"A = ({d1} · {d2}) / 2 = {area:F4}"),
            new("Side Length", "a = √(d₁² + d₂²) / 2", $"a = √({d1}² + {d2}²) / 2 = {side:F4}"),
            new("Perimeter Formula", "P = 4 · a", $"P = 4 · {side:F4} = {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Trapezoid(double a, double b, double h)
    {
        if (a <= 0 || b <= 0 || h <= 0) throw new ArgumentException("Parameters must be positive.");
        double area = 0.5 * (a + b) * h;
        double dx = System.Math.Abs(a - b) / 2.0;
        double leg = System.Math.Sqrt(dx * dx + h * h);
        double perimeter = a + b + 2 * leg;

        List<Point2D> pts = [new(0, 0), new(a, 0), new((a + b) / 2.0, h), new((a - b) / 2.0, h)];

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("leg", "Side Leg (c)", "c", leg, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = (a + b) · h / 2", $"A = ({a} + {b}) · {h} / 2 = {area:F4}"),
            new("Isosceles Perimeter", "P = a + b + 2 · c", $"P = {a} + {b} + 2 · {leg:F4} = {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult RegularPolygon(int n, double side)
    {
        if (n < 3) throw new ArgumentException("Polygon must have at least 3 sides.");
        if (side <= 0) throw new ArgumentException("Side length must be positive.");

        double perimeter = n * side;
        double apothem = side / (2.0 * System.Math.Tan(System.Math.PI / n));
        double area = 0.5 * perimeter * apothem;
        double circumradius = side / (2.0 * System.Math.Sin(System.Math.PI / n));
        double interiorAngle = ((n - 2.0) * 180.0) / n;

        List<Point2D> pts = [];
        for (int i = 0; i < n; i++)
        {
            double angle = (2 * System.Math.PI * i) / n - (System.Math.PI / 2.0);
            pts.Add(new(circumradius + circumradius * System.Math.Cos(angle), circumradius + circumradius * System.Math.Sin(angle)));
        }

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("apothem", "Apothem (inradius)", "r", apothem, UnitType.Length, true),
            new("circumradius", "Circumradius (R)", "R", circumradius, UnitType.Length),
            new("angle", "Interior Angle", "θ", interiorAngle, UnitType.Angle)
        ];

        List<CalculationStep> steps =
        [
            new("Apothem", "r = s / (2 · tan(π/n))", $"r = {side} / (2 · tan(π/{n})) = {apothem:F4}"),
            new("Area Formula", "A = (n · s · r) / 2", $"A = ({n} · {side} · {apothem:F4}) / 2 = {area:F4}"),
            new("Perimeter Formula", "P = n · s", $"P = {n} · {side} = {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }

    public static ShapeCalculationResult Ellipse(double a, double b)
    {
        if (a <= 0 || b <= 0) throw new ArgumentException("Semi-axes must be positive.");
        double area = System.Math.PI * a * b;
        // Ramanujan approximation for perimeter
        double h = System.Math.Pow((a - b) / (a + b), 2);
        double perimeter = System.Math.PI * (a + b) * (1.0 + (3 * h) / (10.0 + System.Math.Sqrt(4.0 - 3 * h)));
        double c = System.Math.Sqrt(System.Math.Abs(a * a - b * b));
        double eccentricity = c / System.Math.Max(a, b);

        List<Point2D> pts = [];
        const int stepsCount = 48;
        for (int i = 0; i <= stepsCount; i++)
        {
            double theta = (2 * System.Math.PI * i) / stepsCount;
            pts.Add(new(a + a * System.Math.Cos(theta), b + b * System.Math.Sin(theta)));
        }

        List<CalculationOutput> outputs =
        [
            new("area", "Area (A)", "A", area, UnitType.Area, true),
            new("perimeter", "Perimeter (P)", "P", perimeter, UnitType.Length, true),
            new("focal_distance", "Focal Distance (c)", "c", c, UnitType.Length),
            new("eccentricity", "Eccentricity (e)", "e", eccentricity, UnitType.Count)
        ];

        List<CalculationStep> steps =
        [
            new("Area Formula", "A = π · a · b", $"A = π · {a} · {b} = {area:F4}"),
            new("Ramanujan Perimeter", "P ≈ π(a+b)[1 + 3h / (10 + √(4-3h))]", $"P ≈ {perimeter:F4}")
        ];

        return new(outputs, steps, pts);
    }
}
