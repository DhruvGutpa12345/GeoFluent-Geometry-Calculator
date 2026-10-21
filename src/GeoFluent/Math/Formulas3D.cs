namespace GeoFluent.Math;

public static class Formulas3D
{
    public static ShapeCalculationResult Sphere(double r)
    {
        if (r <= 0) throw new ArgumentException("Radius must be positive.");
        double diameter = 2 * r;
        double surfaceArea = 4 * System.Math.PI * r * r;
        double volume = (4.0 / 3.0) * System.Math.PI * System.Math.Pow(r, 3);
        double greatCircumference = 2 * System.Math.PI * r;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("surface_area", "Surface Area (A)", "A", surfaceArea, UnitType.Area, true),
            new("diameter", "Diameter (d)", "d", diameter, UnitType.Length, true),
            new("circumference", "Great Circle Circumference", "C", greatCircumference, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Sphere Volume", "V = (4/3) · π · r³", $"V = (4/3) · π · {r}³ = {volume:F4}"),
            new("Surface Area", "A = 4 · π · r²", $"A = 4 · π · {r}² = {surfaceArea:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult Hemisphere(double r)
    {
        if (r <= 0) throw new ArgumentException("Radius must be positive.");
        double volume = (2.0 / 3.0) * System.Math.PI * System.Math.Pow(r, 3);
        double curvedArea = 2 * System.Math.PI * r * r;
        double baseArea = System.Math.PI * r * r;
        double totalArea = 3 * System.Math.PI * r * r;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("total_area", "Total Surface Area (A)", "A", totalArea, UnitType.Area, true),
            new("curved_area", "Curved Surface Area", "A_c", curvedArea, UnitType.Area),
            new("base_area", "Base Circle Area", "A_b", baseArea, UnitType.Area)
        ];

        List<CalculationStep> steps =
        [
            new("Hemisphere Volume", "V = (2/3) · π · r³", $"V = (2/3) · π · {r}³ = {volume:F4}"),
            new("Total Surface Area", "A = 3 · π · r²", $"A = 3 · π · {r}² = {totalArea:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult Cylinder(double r, double h)
    {
        if (r <= 0 || h <= 0) throw new ArgumentException("Radius and height must be positive.");
        double volume = System.Math.PI * r * r * h;
        double lateralArea = 2 * System.Math.PI * r * h;
        double baseArea = System.Math.PI * r * r;
        double totalArea = 2 * baseArea + lateralArea;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("total_area", "Total Surface Area (A)", "A", totalArea, UnitType.Area, true),
            new("lateral_area", "Lateral Surface Area", "A_l", lateralArea, UnitType.Area),
            new("base_area", "Single Base Area", "A_b", baseArea, UnitType.Area)
        ];

        List<CalculationStep> steps =
        [
            new("Cylinder Volume", "V = π · r² · h", $"V = π · {r}² · {h} = {volume:F4}"),
            new("Lateral Area", "A_l = 2 · π · r · h", $"A_l = 2 · π · {r} · {h} = {lateralArea:F4}"),
            new("Total Area", "A = 2πr² + 2πrh", $"A = 2π({r}²) + {lateralArea:F4} = {totalArea:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult Cone(double r, double h)
    {
        if (r <= 0 || h <= 0) throw new ArgumentException("Radius and height must be positive.");
        double slantHeight = System.Math.Sqrt(r * r + h * h);
        double volume = (1.0 / 3.0) * System.Math.PI * r * r * h;
        double lateralArea = System.Math.PI * r * slantHeight;
        double baseArea = System.Math.PI * r * r;
        double totalArea = baseArea + lateralArea;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("total_area", "Total Surface Area (A)", "A", totalArea, UnitType.Area, true),
            new("slant_height", "Slant Height (s)", "s", slantHeight, UnitType.Length, true),
            new("lateral_area", "Lateral Surface Area", "A_l", lateralArea, UnitType.Area)
        ];

        List<CalculationStep> steps =
        [
            new("Slant Height", "s = √(r² + h²)", $"s = √({r}² + {h}²) = {slantHeight:F4}"),
            new("Cone Volume", "V = (1/3) · π · r² · h", $"V = (1/3) · π · {r}² · {h} = {volume:F4}"),
            new("Lateral Area", "A_l = π · r · s", $"A_l = π · {r} · {slantHeight:F4} = {lateralArea:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult Cube(double a)
    {
        if (a <= 0) throw new ArgumentException("Edge length must be positive.");
        double volume = System.Math.Pow(a, 3);
        double surfaceArea = 6 * a * a;
        double spaceDiagonal = a * System.Math.Sqrt(3.0);
        double faceDiagonal = a * System.Math.Sqrt(2.0);

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("surface_area", "Total Surface Area (A)", "A", surfaceArea, UnitType.Area, true),
            new("space_diagonal", "Space Diagonal (d)", "d", spaceDiagonal, UnitType.Length, true),
            new("face_diagonal", "Face Diagonal (d_f)", "d_f", faceDiagonal, UnitType.Length)
        ];

        List<CalculationStep> steps =
        [
            new("Cube Volume", "V = a³", $"V = {a}³ = {volume:F4}"),
            new("Total Surface Area", "A = 6 · a²", $"A = 6 · {a}² = {surfaceArea:F4}"),
            new("Space Diagonal", "d = a · √3", $"d = {a} · √3 = {spaceDiagonal:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult RectangularPrism(double l, double w, double h)
    {
        if (l <= 0 || w <= 0 || h <= 0) throw new ArgumentException("Dimensions must be positive.");
        double volume = l * w * h;
        double surfaceArea = 2 * (l * w + l * h + w * h);
        double spaceDiagonal = System.Math.Sqrt(l * l + w * w + h * h);

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("surface_area", "Total Surface Area (A)", "A", surfaceArea, UnitType.Area, true),
            new("space_diagonal", "Space Diagonal (d)", "d", spaceDiagonal, UnitType.Length, true)
        ];

        List<CalculationStep> steps =
        [
            new("Cuboid Volume", "V = l · w · h", $"V = {l} · {w} · {h} = {volume:F4}"),
            new("Surface Area", "A = 2(lw + lh + wh)", $"A = 2({l*w} + {l*h} + {w*h}) = {surfaceArea:F4}"),
            new("Space Diagonal", "d = √(l² + w² + h²)", $"d = √({l}² + {w}² + {h}²) = {spaceDiagonal:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult RegularPyramid(double baseSide, double height)
    {
        if (baseSide <= 0 || height <= 0) throw new ArgumentException("Parameters must be positive.");
        double baseArea = baseSide * baseSide;
        double volume = (1.0 / 3.0) * baseArea * height;
        double slantHeight = System.Math.Sqrt(height * height + System.Math.Pow(baseSide / 2.0, 2));
        double lateralArea = 2 * baseSide * slantHeight;
        double totalArea = baseArea + lateralArea;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("total_area", "Total Surface Area (A)", "A", totalArea, UnitType.Area, true),
            new("slant_height", "Slant Height (s)", "s", slantHeight, UnitType.Length, true),
            new("lateral_area", "Lateral Surface Area", "A_l", lateralArea, UnitType.Area)
        ];

        List<CalculationStep> steps =
        [
            new("Pyramid Volume", "V = (1/3) · a² · h", $"V = (1/3) · {baseSide}² · {height} = {volume:F4}"),
            new("Slant Height", "s = √(h² + (a/2)²)", $"s = √({height}² + {baseSide/2:F2}²) = {slantHeight:F4}"),
            new("Lateral Area", "A_l = 2 · a · s", $"A_l = 2 · {baseSide} · {slantHeight:F4} = {lateralArea:F4}")
        ];

        return new(outputs, steps);
    }

    public static ShapeCalculationResult Torus(double majorR, double minorR)
    {
        if (majorR <= 0 || minorR <= 0) throw new ArgumentException("Radii must be positive.");
        if (minorR > majorR) throw new ArgumentException("Minor radius (tube) cannot exceed major radius (ring).");

        double volume = 2 * System.Math.PI * System.Math.PI * majorR * minorR * minorR;
        double surfaceArea = 4 * System.Math.PI * System.Math.PI * majorR * minorR;

        List<CalculationOutput> outputs =
        [
            new("volume", "Volume (V)", "V", volume, UnitType.Volume, true),
            new("surface_area", "Surface Area (A)", "A", surfaceArea, UnitType.Area, true)
        ];

        List<CalculationStep> steps =
        [
            new("Torus Volume", "V = 2π² · R · r²", $"V = 2π² · {majorR} · {minorR}² = {volume:F4}"),
            new("Torus Surface Area", "A = 4π² · R · r", $"A = 4π² · {majorR} · {minorR} = {surfaceArea:F4}")
        ];

        return new(outputs, steps);
    }
}
