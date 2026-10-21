namespace GeoFluent.Math;

public static class Formulas2D
{
    public static readonly IReadOnlyList<ShapeDefinition> All =
    [
        // 1. TRIANGLE
        new(
            "triangle",
            "Triangle",
            "2D",
            "Three-sided polygon with altitudes, medians, and inscribed/circumscribed circles.",
            "\uE946",
            [
                new(
                    "heron",
                    "Three Sides (Heron's Formula)",
                    "Calculate triangle using all three side lengths a, b, and c.",
                    [
                        new("a", "Side a", "a", UnitType.Length, 3.0),
                        new("b", "Side b", "b", UnitType.Length, 4.0),
                        new("c", "Side c", "c", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], c = vals["c"];
                        if (a + b <= c || a + c <= b || b + c <= a)
                            throw new ArgumentException("Triangle inequality violated: sum of any two sides must exceed the third.");

                        double s = (a + b + c) / 2.0;
                        double area = System.Math.Sqrt(System.Math.Max(0, s * (s - a) * (s - b) * (s - c)));
                        double p = a + b + c;

                        double alpha = System.Math.Acos(System.Math.Clamp((b*b + c*c - a*a)/(2*b*c), -1.0, 1.0));
                        double beta  = System.Math.Acos(System.Math.Clamp((a*a + c*c - b*b)/(2*a*c), -1.0, 1.0));
                        double gamma = System.Math.PI - alpha - beta;

                        double ha = (2.0 * area) / a;
                        double hb = (2.0 * area) / b;
                        double hc = (2.0 * area) / c;

                        double ma = 0.5 * System.Math.Sqrt(System.Math.Max(0, 2*b*b + 2*c*c - a*a));
                        double mb = 0.5 * System.Math.Sqrt(System.Math.Max(0, 2*a*a + 2*c*c - b*b));
                        double mc = 0.5 * System.Math.Sqrt(System.Math.Max(0, 2*a*a + 2*b*b - c*c));

                        double inr = area / s;
                        double circ = (a * b * c) / (4.0 * area);

                        double cx = (c*c - a*a + b*b) / (2 * c);
                        double cy = System.Math.Sqrt(System.Math.Max(0, b*b - cx*cx));
                        List<Point2D> pts = [new(0, 0), new(c, 0), new(cx, cy)];

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("semi_p", "Semi-perimeter (s)", "s", s, UnitType.Length),
                                new("alpha", "Angle α", "α", alpha, UnitType.Angle),
                                new("beta", "Angle β", "β", beta, UnitType.Angle),
                                new("gamma", "Angle γ", "γ", gamma, UnitType.Angle),
                                new("ha", "Altitude ha", "hₐ", ha, UnitType.Length),
                                new("hb", "Altitude hb", "hb", hb, UnitType.Length),
                                new("hc", "Altitude hc", "hc", hc, UnitType.Length),
                                new("ma", "Median ma", "mₐ", ma, UnitType.Length),
                                new("mb", "Median mb", "mb", mb, UnitType.Length),
                                new("mc", "Median mc", "mc", mc, UnitType.Length),
                                new("inradius", "Inradius (r)", "r", inr, UnitType.Length),
                                new("circumradius", "Circumradius (R)", "R", circ, UnitType.Length)
                            ],
                            [
                                new("Semi-perimeter", "s = (a + b + c) / 2", $"s = ({a:F3} + {b:F3} + {c:F3}) / 2 = {s:F4}"),
                                new("Heron's Formula", "A = √(s(s - a)(s - b)(s - c))", $"A = √({s:F2} · {s-a:F2} · {s-b:F2} · {s-c:F2}) = {area:F4}"),
                                new("Law of Cosines (α)", "α = arccos((b² + c² - a²) / (2bc))", $"α = {alpha * 180 / System.Math.PI:F2}°")
                            ],
                            pts,
                            null,
                            inr,
                            circ
                        );
                    }
                ),
                new(
                    "base_height",
                    "Base & Height",
                    "Calculate area from base and perpendicular height.",
                    [
                        new("base", "Base (b)", "b", UnitType.Length, 6.0),
                        new("height", "Height (h)", "h", UnitType.Length, 4.0)
                    ],
                    vals =>
                    {
                        double b = vals["base"], h = vals["height"];
                        double area = 0.5 * b * h;
                        List<Point2D> pts = [new(0, 0), new(b, 0), new(b / 2.0, h)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("base", "Base (b)", "b", b, UnitType.Length),
                                new("height", "Height (h)", "h", h, UnitType.Length)
                            ],
                            [
                                new("Triangle Area", "A = 0.5 · b · h", $"A = 0.5 · {b:F3} · {h:F3} = {area:F4}")
                            ],
                            pts
                        );
                    }
                ),
                new(
                    "sas",
                    "Two Sides & Included Angle (SAS)",
                    "Calculate triangle using two sides and the angle between them.",
                    [
                        new("a", "Side a", "a", UnitType.Length, 5.0),
                        new("b", "Side b", "b", UnitType.Length, 7.0),
                        new("gamma", "Included Angle (γ)", "γ", UnitType.Angle, System.Math.PI / 3.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], gamma = vals["gamma"];
                        double area = 0.5 * a * b * System.Math.Sin(gamma);
                        double c = System.Math.Sqrt(a*a + b*b - 2*a*b*System.Math.Cos(gamma));
                        double p = a + b + c;
                        List<Point2D> pts = [new(0, 0), new(b, 0), new(a * System.Math.Cos(gamma), a * System.Math.Sin(gamma))];

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("c", "Opposite Side (c)", "c", c, UnitType.Length, true)
                            ],
                            [
                                new("SAS Area", "A = 0.5 · a · b · sin(γ)", $"A = 0.5 · {a:F2} · {b:F2} · sin({gamma*180/System.Math.PI:F1}°) = {area:F4}"),
                                new("Law of Cosines (c)", "c = √(a² + b² - 2ab·cos(γ))", $"c = {c:F4}")
                            ],
                            pts
                        );
                    }
                ),
                new(
                    "equilateral",
                    "Equilateral Triangle",
                    "Calculate equilateral triangle with all equal sides.",
                    [
                        new("a", "Side Length (a)", "a", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"];
                        double area = (System.Math.Sqrt(3.0) / 4.0) * a * a;
                        double p = 3 * a;
                        double h = (System.Math.Sqrt(3.0) / 2.0) * a;
                        double inr = a / (2.0 * System.Math.Sqrt(3.0));
                        double circ = a / System.Math.Sqrt(3.0);
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a / 2.0, h)];

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("height", "Height (h)", "h", h, UnitType.Length, true),
                                new("inradius", "Inradius (r)", "r", inr, UnitType.Length),
                                new("circumradius", "Circumradius (R)", "R", circ, UnitType.Length)
                            ],
                            [
                                new("Equilateral Area", "A = (√3 / 4) · a²", $"A = (√3 / 4) · {a:F2}² = {area:F4}"),
                                new("Equilateral Height", "h = (√3 / 2) · a", $"h = (√3 / 2) · {a:F2} = {h:F4}")
                            ],
                            pts,
                            null,
                            inr,
                            circ
                        );
                    }
                )
            ]
        ),

        // 2. SQUARE
        new(
            "square",
            "Square",
            "2D",
            "Equilateral quadrilateral with four right angles and congruent diagonals.",
            "\uE946",
            [
                new(
                    "side",
                    "By Side Length (a)",
                    "Calculate square properties from side length.",
                    [ new("a", "Side Length (a)", "a", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double area = a * a;
                        double p = 4 * a;
                        double d = a * System.Math.Sqrt(2.0);
                        double inr = a / 2.0;
                        double circ = d / 2.0;
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a, a), new(0, a)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("diagonal", "Diagonal (d)", "d", d, UnitType.Length, true),
                                new("inradius", "Inradius (r)", "r", inr, UnitType.Length),
                                new("circumradius", "Circumradius (R)", "R", circ, UnitType.Length)
                            ],
                            [
                                new("Square Area", "A = a²", $"A = {a:F2}² = {area:F4}"),
                                new("Square Perimeter", "P = 4 · a", $"P = 4 · {a:F2} = {p:F4}"),
                                new("Diagonal", "d = a · √2", $"d = {a:F2} · √2 = {d:F4}")
                            ],
                            pts,
                            null,
                            inr,
                            circ
                        );
                    }
                ),
                new(
                    "diagonal",
                    "By Diagonal (d)",
                    "Calculate square properties from diagonal length.",
                    [ new("d", "Diagonal (d)", "d", UnitType.Length, 7.071) ],
                    vals =>
                    {
                        double d = vals["d"];
                        double a = d / System.Math.Sqrt(2.0);
                        double area = 0.5 * d * d;
                        double p = 4 * a;
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a, a), new(0, a)];
                        return new(
                            [
                                new("side", "Side Length (a)", "a", a, UnitType.Length, true),
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true)
                            ],
                            [
                                new("Side from Diagonal", "a = d / √2", $"a = {d:F2} / √2 = {a:F4}"),
                                new("Area from Diagonal", "A = 0.5 · d²", $"A = 0.5 · {d:F2}² = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 3. RECTANGLE
        new(
            "rectangle",
            "Rectangle",
            "2D",
            "Quadrilateral with four right angles and congruent opposite sides.",
            "\uE946",
            [
                new(
                    "sides",
                    "Length & Width",
                    "Calculate rectangle properties from length and width.",
                    [
                        new("l", "Length (l)", "l", UnitType.Length, 8.0),
                        new("w", "Width (w)", "w", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double l = vals["l"], w = vals["w"];
                        double area = l * w;
                        double p = 2 * (l + w);
                        double d = System.Math.Sqrt(l*l + w*w);
                        double circ = d / 2.0;
                        List<Point2D> pts = [new(0, 0), new(l, 0), new(l, w), new(0, w)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("diagonal", "Diagonal (d)", "d", d, UnitType.Length, true),
                                new("circumradius", "Circumradius (R)", "R", circ, UnitType.Length)
                            ],
                            [
                                new("Rectangle Area", "A = l · w", $"A = {l:F2} · {w:F2} = {area:F4}"),
                                new("Perimeter", "P = 2 · (l + w)", $"P = 2 · ({l:F2} + {w:F2}) = {p:F4}"),
                                new("Diagonal", "d = √(l² + w²)", $"d = √({l:F2}² + {w:F2}²) = {d:F4}")
                            ],
                            pts,
                            null,
                            0,
                            circ
                        );
                    }
                ),
                new(
                    "diagonal_side",
                    "Diagonal & One Side",
                    "Calculate rectangle properties from diagonal and one side.",
                    [
                        new("d", "Diagonal (d)", "d", UnitType.Length, 10.0),
                        new("a", "Known Side (a)", "a", UnitType.Length, 6.0)
                    ],
                    vals =>
                    {
                        double d = vals["d"], a = vals["a"];
                        if (a >= d) throw new ArgumentException("Side must be strictly less than diagonal.");
                        double b = System.Math.Sqrt(d*d - a*a);
                        double area = a * b;
                        double p = 2 * (a + b);
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a, b), new(0, b)];
                        return new(
                            [
                                new("other_side", "Other Side (b)", "b", b, UnitType.Length, true),
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true)
                            ],
                            [
                                new("Other Side", "b = √(d² - a²)", $"b = √({d:F2}² - {a:F2}²) = {b:F4}"),
                                new("Area", "A = a · b", $"A = {a:F2} · {b:F4} = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 4. CIRCLE
        new(
            "circle",
            "Circle",
            "2D",
            "Set of all points in a plane equidistant from a central center point.",
            "\uE946",
            [
                new(
                    "radius",
                    "By Radius (r)",
                    "Calculate circle from radius.",
                    [ new("r", "Radius (r)", "r", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double r = vals["r"];
                        double d = 2 * r;
                        double area = System.Math.PI * r * r;
                        double c = 2 * System.Math.PI * r;
                        List<Point2D> pts = GenerateCirclePoints(r);
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("circumference", "Circumference (C)", "C", c, UnitType.Length, true),
                                new("diameter", "Diameter (d)", "d", d, UnitType.Length, true)
                            ],
                            [
                                new("Circle Area", "A = π · r²", $"A = π · {r:F2}² = {area:F4}"),
                                new("Circumference", "C = 2 · π · r", $"C = 2 · π · {r:F2} = {c:F4}")
                            ],
                            pts
                        );
                    }
                ),
                new(
                    "diameter",
                    "By Diameter (d)",
                    "Calculate circle from diameter.",
                    [ new("d", "Diameter (d)", "d", UnitType.Length, 10.0) ],
                    vals =>
                    {
                        double d = vals["d"];
                        double r = d / 2.0;
                        double area = System.Math.PI * r * r;
                        double c = System.Math.PI * d;
                        List<Point2D> pts = GenerateCirclePoints(r);
                        return new(
                            [
                                new("radius", "Radius (r)", "r", r, UnitType.Length, true),
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("circumference", "Circumference (C)", "C", c, UnitType.Length, true)
                            ],
                            [
                                new("Radius", "r = d / 2", $"r = {d:F2} / 2 = {r:F4}"),
                                new("Area", "A = (π · d²) / 4", $"A = (π · {d:F2}²) / 4 = {area:F4}")
                            ],
                            pts
                        );
                    }
                ),
                new(
                    "circumference",
                    "By Circumference (C)",
                    "Calculate circle from circumference.",
                    [ new("C", "Circumference (C)", "C", UnitType.Length, 31.416) ],
                    vals =>
                    {
                        double c = vals["C"];
                        double r = c / (2 * System.Math.PI);
                        double d = 2 * r;
                        double area = System.Math.PI * r * r;
                        List<Point2D> pts = GenerateCirclePoints(r);
                        return new(
                            [
                                new("radius", "Radius (r)", "r", r, UnitType.Length, true),
                                new("diameter", "Diameter (d)", "d", d, UnitType.Length, true),
                                new("area", "Area (A)", "A", area, UnitType.Area, true)
                            ],
                            [
                                new("Radius from Circumference", "r = C / (2π)", $"r = {c:F2} / (2π) = {r:F4}"),
                                new("Area", "A = C² / (4π)", $"A = {c:F2}² / (4π) = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 5. ELLIPSE
        new(
            "ellipse",
            "Ellipse",
            "2D",
            "Curved plane figure where sum of distances from any point to two foci is constant.",
            "\uE946",
            [
                new(
                    "semi_axes",
                    "Semi-Major & Semi-Minor Axes",
                    "Calculate ellipse from semi-axes a and b.",
                    [
                        new("a", "Semi-Major Axis (a)", "a", UnitType.Length, 8.0),
                        new("b", "Semi-Minor Axis (b)", "b", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"];
                        double area = System.Math.PI * a * b;
                        double h = System.Math.Pow((a - b) / (a + b), 2);
                        double p = System.Math.PI * (a + b) * (1.0 + (3 * h) / (10.0 + System.Math.Sqrt(4.0 - 3 * h)));
                        double c = System.Math.Sqrt(System.Math.Abs(a*a - b*b));
                        double ecc = c / System.Math.Max(a, b);

                        List<Point2D> pts = [];
                        for (int i = 0; i <= 48; i++)
                        {
                            double th = (2 * System.Math.PI * i) / 48.0;
                            pts.Add(new(a + a * System.Math.Cos(th), b + b * System.Math.Sin(th)));
                        }

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P ≈)", "P", p, UnitType.Length, true),
                                new("focal_distance", "Focal Distance (c)", "c", c, UnitType.Length),
                                new("eccentricity", "Eccentricity (e)", "e", ecc, UnitType.Count)
                            ],
                            [
                                new("Ellipse Area", "A = π · a · b", $"A = π · {a:F2} · {b:F2} = {area:F4}"),
                                new("Ramanujan Perimeter", "P ≈ π(a+b)[1 + 3h / (10 + √(4-3h))]", $"P ≈ {p:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 6. PARALLELOGRAM
        new(
            "parallelogram",
            "Parallelogram",
            "2D",
            "Quadrilateral with two pairs of parallel opposite sides.",
            "\uE946",
            [
                new(
                    "base_height",
                    "Base, Side & Height",
                    "Calculate parallelogram from base, slant side, and altitude.",
                    [
                        new("a", "Base Side (a)", "a", UnitType.Length, 10.0),
                        new("b", "Slant Side (b)", "b", UnitType.Length, 6.0),
                        new("h", "Height (h)", "h", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], h = vals["h"];
                        if (h > b) throw new ArgumentException("Height cannot exceed slant side b.");
                        double area = a * h;
                        double p = 2 * (a + b);
                        double dx = System.Math.Sqrt(System.Math.Max(0, b*b - h*h));
                        double theta = System.Math.Asin(System.Math.Clamp(h / b, -1.0, 1.0));
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a + dx, h), new(dx, h)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("angle", "Base Angle (θ)", "θ", theta, UnitType.Angle)
                            ],
                            [
                                new("Parallelogram Area", "A = a · h", $"A = {a:F2} · {h:F2} = {area:F4}"),
                                new("Perimeter", "P = 2 · (a + b)", $"P = 2 · ({a:F2} + {b:F2}) = {p:F4}")
                            ],
                            pts
                        );
                    }
                ),
                new(
                    "sides_angle",
                    "Sides & Angle (SAS)",
                    "Calculate parallelogram from two sides and included angle.",
                    [
                        new("a", "Base Side (a)", "a", UnitType.Length, 8.0),
                        new("b", "Slant Side (b)", "b", UnitType.Length, 5.0),
                        new("theta", "Included Angle (θ)", "θ", UnitType.Angle, System.Math.PI / 4.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], theta = vals["theta"];
                        double h = b * System.Math.Sin(theta);
                        double area = a * b * System.Math.Sin(theta);
                        double p = 2 * (a + b);
                        double dx = b * System.Math.Cos(theta);
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a + dx, h), new(dx, h)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("height", "Height (h)", "h", h, UnitType.Length)
                            ],
                            [
                                new("Area from Angle", "A = a · b · sin(θ)", $"A = {a:F2} · {b:F2} · sin({theta*180/System.Math.PI:F1}°) = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 7. RHOMBUS
        new(
            "rhombus",
            "Rhombus",
            "2D",
            "Equilateral parallelogram whose diagonals intersect at perpendicular right angles.",
            "\uE946",
            [
                new(
                    "diagonals",
                    "Two Diagonals (d₁, d₂)",
                    "Calculate rhombus from its two orthogonal diagonals.",
                    [
                        new("d1", "Diagonal 1 (d₁)", "d₁", UnitType.Length, 8.0),
                        new("d2", "Diagonal 2 (d₂)", "d₂", UnitType.Length, 6.0)
                    ],
                    vals =>
                    {
                        double d1 = vals["d1"], d2 = vals["d2"];
                        double area = 0.5 * d1 * d2;
                        double side = 0.5 * System.Math.Sqrt(d1*d1 + d2*d2);
                        double p = 4 * side;
                        double inr = (d1 * d2) / (4.0 * side);
                        List<Point2D> pts = [new(d1 / 2.0, 0), new(d1, d2 / 2.0), new(d1 / 2.0, d2), new(0, d2 / 2.0)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("side", "Side Length (a)", "a", side, UnitType.Length, true),
                                new("inradius", "Inradius (r)", "r", inr, UnitType.Length)
                            ],
                            [
                                new("Rhombus Area", "A = 0.5 · d₁ · d₂", $"A = 0.5 · {d1:F2} · {d2:F2} = {area:F4}"),
                                new("Side Length", "a = 0.5 · √(d₁² + d₂²)", $"a = 0.5 · √({d1:F2}² + {d2:F2}²) = {side:F4}")
                            ],
                            pts,
                            null,
                            inr
                        );
                    }
                )
            ]
        ),

        // 8. TRAPEZOID (GENERAL)
        new(
            "trapezoid",
            "Trapezoid",
            "2D",
            "Quadrilateral with at least one pair of parallel sides (bases).",
            "\uE946",
            [
                new(
                    "bases_height",
                    "Two Bases & Height",
                    "Calculate trapezoid from parallel bases a, b and perpendicular height h.",
                    [
                        new("a", "Long Base (a)", "a", UnitType.Length, 10.0),
                        new("b", "Short Base (b)", "b", UnitType.Length, 6.0),
                        new("h", "Height (h)", "h", UnitType.Length, 4.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], h = vals["h"];
                        double area = 0.5 * (a + b) * h;
                        double dx = System.Math.Abs(a - b) / 2.0;
                        double leg = System.Math.Sqrt(dx*dx + h*h);
                        double p = a + b + 2 * leg;
                        List<Point2D> pts = [new(0, 0), new(a, 0), new((a + b)/2.0, h), new((a - b)/2.0, h)];
                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Isosceles Perimeter (P)", "P", p, UnitType.Length, true),
                                new("leg", "Slant Leg (c)", "c", leg, UnitType.Length)
                            ],
                            [
                                new("Trapezoid Area", "A = 0.5 · (a + b) · h", $"A = 0.5 · ({a:F2} + {b:F2}) · {h:F2} = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 9. REGULAR POLYGON
        new(
            "regular_polygon",
            "Regular Polygon",
            "2D",
            "Equiangular and equilateral polygon with n sides (Pentagon, Hexagon, Octagon, etc.).",
            "\uE946",
            [
                new(
                    "sides_count_length",
                    "Sides Count (n) & Side Length (s)",
                    "Calculate regular polygon from side count n and edge length s.",
                    [
                        new("n", "Number of Sides (n)", "n", UnitType.Count, 6.0, 3.0, 100.0),
                        new("s", "Side Length (s)", "s", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        int n = (int)System.Math.Round(vals["n"]);
                        double s = vals["s"];
                        double p = n * s;
                        double apothem = s / (2.0 * System.Math.Tan(System.Math.PI / n));
                        double area = 0.5 * p * apothem;
                        double circ = s / (2.0 * System.Math.Sin(System.Math.PI / n));
                        double intAngle = ((n - 2.0) * System.Math.PI) / n;

                        List<Point2D> pts = [];
                        for (int i = 0; i < n; i++)
                        {
                            double th = (2 * System.Math.PI * i) / n - (System.Math.PI / 2.0);
                            pts.Add(new(circ + circ * System.Math.Cos(th), circ + circ * System.Math.Sin(th)));
                        }

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("apothem", "Apothem (r)", "r", apothem, UnitType.Length, true),
                                new("circumradius", "Circumradius (R)", "R", circ, UnitType.Length),
                                new("int_angle", "Interior Angle", "θ", intAngle, UnitType.Angle)
                            ],
                            [
                                new("Apothem", "r = s / (2 · tan(π/n))", $"r = {s:F2} / (2 · tan(π/{n})) = {apothem:F4}"),
                                new("Polygon Area", "A = 0.5 · n · s · r", $"A = 0.5 · {n} · {s:F2} · {apothem:F4} = {area:F4}")
                            ],
                            pts,
                            null,
                            apothem,
                            circ
                        );
                    }
                )
            ]
        ),

        // 10. CIRCULAR SECTOR
        new(
            "circular_sector",
            "Circular Sector",
            "2D",
            "Portion of a disk enclosed by two radii and an arc (pie slice).",
            "\uE946",
            [
                new(
                    "radius_angle",
                    "Radius (r) & Central Angle (θ)",
                    "Calculate sector from radius and central angle.",
                    [
                        new("r", "Radius (r)", "r", UnitType.Length, 6.0),
                        new("theta", "Central Angle (θ)", "θ", UnitType.Angle, System.Math.PI / 3.0)
                    ],
                    vals =>
                    {
                        double r = vals["r"], theta = vals["theta"];
                        double arc = r * theta;
                        double area = 0.5 * r * r * theta;
                        double p = 2 * r + arc;
                        double chord = 2 * r * System.Math.Sin(theta / 2.0);

                        List<Point2D> pts = [new(r, r)];
                        int segs = 32;
                        for (int i = 0; i <= segs; i++)
                        {
                            double th = -theta / 2.0 + (theta * i) / segs;
                            pts.Add(new(r + r * System.Math.Cos(th), r + r * System.Math.Sin(th)));
                        }

                        return new(
                            [
                                new("area", "Sector Area (A)", "A", area, UnitType.Area, true),
                                new("arc_length", "Arc Length (L)", "L", arc, UnitType.Length, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("chord", "Chord Length (c)", "c", chord, UnitType.Length)
                            ],
                            [
                                new("Sector Area", "A = 0.5 · r² · θ", $"A = 0.5 · {r:F2}² · {theta:F4} rad = {area:F4}"),
                                new("Arc Length", "L = r · θ", $"L = {r:F2} · {theta:F4} = {arc:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 11. ANNULUS (RING)
        new(
            "annulus",
            "Annulus (Ring)",
            "2D",
            "Region bounded by two concentric circles of different radii.",
            "\uE946",
            [
                new(
                    "radii",
                    "Outer Radius (R) & Inner Radius (r)",
                    "Calculate ring from outer radius R and inner radius r.",
                    [
                        new("R", "Outer Radius (R)", "R", UnitType.Length, 8.0),
                        new("r", "Inner Radius (r)", "r", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double R = vals["R"], r = vals["r"];
                        if (r >= R) throw new ArgumentException("Inner radius r must be strictly less than outer radius R.");
                        double area = System.Math.PI * (R*R - r*r);
                        double pOut = 2 * System.Math.PI * R;
                        double pIn = 2 * System.Math.PI * r;
                        double width = R - r;

                        List<Point2D> pts = GenerateCirclePoints(R);

                        return new(
                            [
                                new("area", "Ring Area (A)", "A", area, UnitType.Area, true),
                                new("total_perim", "Total Perimeter (P_out + P_in)", "P", pOut + pIn, UnitType.Length, true),
                                new("width", "Ring Width (w)", "w", width, UnitType.Length),
                                new("p_out", "Outer Circumference", "C_out", pOut, UnitType.Length),
                                new("p_in", "Inner Circumference", "C_in", pIn, UnitType.Length)
                            ],
                            [
                                new("Annulus Area", "A = π · (R² - r²)", $"A = π · ({R:F2}² - {r:F2}²) = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 12. KITE (DELTOID)
        new(
            "kite",
            "Kite (Deltoid)",
            "2D",
            "Quadrilateral with two distinct pairs of equal-length adjacent sides.",
            "\uE946",
            [
                new(
                    "diagonals",
                    "Diagonals & Sides",
                    "Calculate kite from perpendicular diagonals d1, d2 and side lengths a, b.",
                    [
                        new("d1", "Diagonal 1 (d₁)", "d₁", UnitType.Length, 8.0),
                        new("d2", "Diagonal 2 (d₂)", "d₂", UnitType.Length, 6.0),
                        new("a", "Short Side (a)", "a", UnitType.Length, 4.0),
                        new("b", "Long Side (b)", "b", UnitType.Length, 6.0)
                    ],
                    vals =>
                    {
                        double d1 = vals["d1"], d2 = vals["d2"], a = vals["a"], b = vals["b"];
                        double area = 0.5 * d1 * d2;
                        double p = 2 * (a + b);
                        List<Point2D> pts = [new(d1 / 2.0, 0), new(d1, d2 * 0.4), new(d1 / 2.0, d2), new(0, d2 * 0.4)];

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true)
                            ],
                            [
                                new("Kite Area", "A = 0.5 · d₁ · d₂", $"A = 0.5 · {d1:F2} · {d2:F2} = {area:F4}"),
                                new("Perimeter", "P = 2 · (a + b)", $"P = 2 · ({a:F2} + {b:F2}) = {p:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 13. ISOSCELES TRAPEZOID
        new(
            "isosceles_trapezoid",
            "Isosceles Trapezoid",
            "2D",
            "Trapezoid where base angles are equal and non-parallel legs are equal.",
            "\uE946",
            [
                new(
                    "bases_leg",
                    "Bases (a, b) & Leg (c)",
                    "Calculate isosceles trapezoid from bases a, b and equal leg c.",
                    [
                        new("a", "Long Base (a)", "a", UnitType.Length, 10.0),
                        new("b", "Short Base (b)", "b", UnitType.Length, 6.0),
                        new("c", "Slant Leg (c)", "c", UnitType.Length, 5.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], c = vals["c"];
                        double dx = System.Math.Abs(a - b) / 2.0;
                        if (c <= dx) throw new ArgumentException("Leg c must be strictly greater than (a - b) / 2.");
                        double h = System.Math.Sqrt(c*c - dx*dx);
                        double area = 0.5 * (a + b) * h;
                        double p = a + b + 2 * c;
                        double d = System.Math.Sqrt(a*b + c*c);
                        List<Point2D> pts = [new(0, 0), new(a, 0), new(a - dx, h), new(dx, h)];

                        return new(
                            [
                                new("area", "Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("height", "Height (h)", "h", h, UnitType.Length, true),
                                new("diagonal", "Diagonal (d)", "d", d, UnitType.Length)
                            ],
                            [
                                new("Height", "h = √(c² - ((a-b)/2)²)", $"h = √({c:F2}² - {dx:F2}²) = {h:F4}"),
                                new("Area", "A = 0.5 · (a + b) · h", $"A = 0.5 · ({a:F2} + {b:F2}) · {h:F4} = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 14. CIRCULAR SEGMENT
        new(
            "circular_segment",
            "Circular Segment",
            "2D",
            "Region bounded by an arc and the chord connecting its endpoints.",
            "\uE946",
            [
                new(
                    "radius_angle",
                    "Radius (r) & Angle (θ)",
                    "Calculate circular segment from radius and central angle.",
                    [
                        new("r", "Radius (r)", "r", UnitType.Length, 6.0),
                        new("theta", "Central Angle (θ)", "θ", UnitType.Angle, System.Math.PI / 2.0)
                    ],
                    vals =>
                    {
                        double r = vals["r"], theta = vals["theta"];
                        double area = 0.5 * r * r * (theta - System.Math.Sin(theta));
                        double chord = 2 * r * System.Math.Sin(theta / 2.0);
                        double arc = r * theta;
                        double height = r * (1.0 - System.Math.Cos(theta / 2.0));
                        double p = chord + arc;

                        List<Point2D> pts = [];
                        int segs = 32;
                        for (int i = 0; i <= segs; i++)
                        {
                            double th = -theta / 2.0 + (theta * i) / segs;
                            pts.Add(new(r + r * System.Math.Cos(th), r + r * System.Math.Sin(th)));
                        }

                        return new(
                            [
                                new("area", "Segment Area (A)", "A", area, UnitType.Area, true),
                                new("chord", "Chord Length (c)", "c", chord, UnitType.Length, true),
                                new("height", "Sagitta / Height (h)", "h", height, UnitType.Length),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length)
                            ],
                            [
                                new("Segment Area", "A = 0.5 · r² · (θ - sin(θ))", $"A = 0.5 · {r:F2}² · ({theta:F4} - sin({theta:F4})) = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 15. ANNULUS SECTOR
        new(
            "annulus_sector",
            "Annulus Sector",
            "2D",
            "Sector cut from a circular ring bounded by radii R, r and angle θ.",
            "\uE946",
            [
                new(
                    "params",
                    "Radii (R, r) & Angle (θ)",
                    "Calculate ring sector from outer radius R, inner radius r, and central angle θ.",
                    [
                        new("R", "Outer Radius (R)", "R", UnitType.Length, 8.0),
                        new("r", "Inner Radius (r)", "r", UnitType.Length, 5.0),
                        new("theta", "Angle (θ)", "θ", UnitType.Angle, System.Math.PI / 3.0)
                    ],
                    vals =>
                    {
                        double R = vals["R"], r = vals["r"], theta = vals["theta"];
                        if (r >= R) throw new ArgumentException("Inner radius r must be strictly less than outer radius R.");
                        double area = 0.5 * theta * (R*R - r*r);
                        double arcOut = R * theta;
                        double arcIn = r * theta;
                        double p = 2 * (R - r) + arcOut + arcIn;

                        List<Point2D> pts = [];
                        int segs = 24;
                        for (int i = 0; i <= segs; i++)
                        {
                            double th = (theta * i) / segs;
                            pts.Add(new(R * System.Math.Cos(th), R * System.Math.Sin(th)));
                        }
                        for (int i = segs; i >= 0; i--)
                        {
                            double th = (theta * i) / segs;
                            pts.Add(new(r * System.Math.Cos(th), r * System.Math.Sin(th)));
                        }

                        return new(
                            [
                                new("area", "Sector Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("arc_out", "Outer Arc Length", "L_out", arcOut, UnitType.Length),
                                new("arc_in", "Inner Arc Length", "L_in", arcIn, UnitType.Length)
                            ],
                            [
                                new("Annulus Sector Area", "A = 0.5 · θ · (R² - r²)", $"A = 0.5 · {theta:F4} · ({R:F2}² - {r:F2}²) = {area:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 16. REGULAR STAR POLYGON
        new(
            "star_polygon",
            "Regular Star Polygon",
            "2D",
            "Equiangular star polygon with n outer points and alternating inner valleys.",
            "\uE946",
            [
                new(
                    "star_params",
                    "Points (n) & Radii (R, r)",
                    "Calculate star from points count n, outer radius R, and inner valley radius r.",
                    [
                        new("n", "Points Count (n)", "n", UnitType.Count, 5.0, 3.0, 24.0),
                        new("R", "Outer Radius (R)", "R", UnitType.Length, 8.0),
                        new("r", "Inner Radius (r)", "r", UnitType.Length, 3.5)
                    ],
                    vals =>
                    {
                        int n = (int)System.Math.Round(vals["n"]);
                        double R = vals["R"], r = vals["r"];
                        if (r >= R) throw new ArgumentException("Inner radius r must be strictly less than outer radius R.");
                        double angleStep = System.Math.PI / n;
                        double edge = System.Math.Sqrt(R*R + r*r - 2*R*r*System.Math.Cos(angleStep));
                        double p = 2 * n * edge;
                        double area = n * R * r * System.Math.Sin(angleStep);

                        List<Point2D> pts = [];
                        for (int i = 0; i < 2 * n; i++)
                        {
                            double rad = (i % 2 == 0) ? R : r;
                            double a = i * angleStep - (System.Math.PI / 2.0);
                            pts.Add(new(R + rad * System.Math.Cos(a), R + rad * System.Math.Sin(a)));
                        }

                        return new(
                            [
                                new("area", "Star Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("edge", "Single Edge Length", "e", edge, UnitType.Length),
                                new("span", "Tip-to-Tip Span", "2R", 2 * R, UnitType.Length)
                            ],
                            [
                                new("Star Area", "A = n · R · r · sin(π/n)", $"A = {n} · {R:F2} · {r:F2} · sin(π/{n}) = {area:F4}"),
                                new("Edge Length", "e = √(R² + r² - 2Rr·cos(π/n))", $"e = {edge:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        ),

        // 17. HEART SHAPE (CARDIOID)
        new(
            "heart_cardioid",
            "Heart Shape (Cardioid)",
            "2D",
            "Smooth mathematical heart curve based on cardioid parametrizations.",
            "\uE946",
            [
                new(
                    "scale_param",
                    "Scale Parameter (a)",
                    "Calculate mathematical heart curve from characteristic scale a.",
                    [ new("a", "Scale Parameter (a)", "a", UnitType.Length, 4.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double area = 1.5 * System.Math.PI * a * a;
                        double p = 8 * a;
                        double maxW = 4 * a;
                        double maxH = 3.5 * a;

                        List<Point2D> pts = [];
                        const int totalPts = 64;
                        for (int i = 0; i <= totalPts; i++)
                        {
                            double t = (2 * System.Math.PI * i) / totalPts;
                            double x = 16 * System.Math.Pow(System.Math.Sin(t), 3);
                            double y = -(13 * System.Math.Cos(t) - 5 * System.Math.Cos(2*t) - 2 * System.Math.Cos(3*t) - System.Math.Cos(4*t));
                            pts.Add(new((x * a) / 8.0 + maxW / 2.0, (y * a) / 8.0 + maxH / 2.0));
                        }

                        return new(
                            [
                                new("area", "Heart Area (A)", "A", area, UnitType.Area, true),
                                new("perimeter", "Perimeter (P)", "P", p, UnitType.Length, true),
                                new("max_w", "Maximum Width", "W", maxW, UnitType.Length),
                                new("max_h", "Maximum Height", "H", maxH, UnitType.Length)
                            ],
                            [
                                new("Cardioid Area", "A = 1.5 · π · a²", $"A = 1.5 · π · {a:F2}² = {area:F4}"),
                                new("Perimeter", "P = 8 · a", $"P = 8 · {a:F2} = {p:F4}")
                            ],
                            pts
                        );
                    }
                )
            ]
        )
    ];

    private static List<Point2D> GenerateCirclePoints(double r, int segments = 48)
    {
        List<Point2D> pts = [];
        for (int i = 0; i <= segments; i++)
        {
            double th = (2 * System.Math.PI * i) / segments;
            pts.Add(new(r + r * System.Math.Cos(th), r + r * System.Math.Sin(th)));
        }
        return pts;
    }
}
