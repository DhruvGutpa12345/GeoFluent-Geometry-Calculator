namespace GeoFluent.Math;

public static class Formulas3D
{
    public static readonly IReadOnlyList<ShapeDefinition> All =
    [
        // 1. SPHERE
        new(
            "sphere",
            "Sphere",
            "3D",
            "Perfect round geometrical object in three-dimensional space.",
            "\uF158",
            [
                new(
                    "by_radius",
                    "By Radius (r)",
                    "Calculate sphere from radius r.",
                    [ new("r", "Radius (r)", "r", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double r = vals["r"];
                        double d = 2 * r;
                        double a = 4 * System.Math.PI * r * r;
                        double v = (4.0 / 3.0) * System.Math.PI * System.Math.Pow(r, 3);
                        double c = 2 * System.Math.PI * r;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true),
                                new("diameter", "Diameter (d)", "d", d, UnitType.Length, true),
                                new("circumference", "Great Circumference", "C", c, UnitType.Length)
                            ],
                            [
                                new("Sphere Volume", "V = (4/3) · π · r³", $"V = (4/3) · π · {r:F2}³ = {v:F4}"),
                                new("Surface Area", "A = 4 · π · r²", $"A = 4 · π · {r:F2}² = {a:F4}")
                            ],
                            null,
                            GenerateSphereLines(r)
                        );
                    }
                ),
                new(
                    "by_diameter",
                    "By Diameter (d)",
                    "Calculate sphere from diameter d.",
                    [ new("d", "Diameter (d)", "d", UnitType.Length, 10.0) ],
                    vals =>
                    {
                        double d = vals["d"];
                        double r = d / 2.0;
                        double a = System.Math.PI * d * d;
                        double v = (System.Math.PI * System.Math.Pow(d, 3)) / 6.0;
                        return new(
                            [
                                new("radius", "Radius (r)", "r", r, UnitType.Length, true),
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true)
                            ],
                            [
                                new("Radius", "r = d / 2", $"r = {d:F2} / 2 = {r:F4}"),
                                new("Volume", "V = (π · d³) / 6", $"V = (π · {d:F2}³) / 6 = {v:F4}")
                            ],
                            null,
                            GenerateSphereLines(r)
                        );
                    }
                ),
                new(
                    "by_volume",
                    "By Volume (V)",
                    "Calculate sphere radius and surface area from volume V.",
                    [ new("V", "Volume (V)", "V", UnitType.Volume, 523.6) ],
                    vals =>
                    {
                        double v = vals["V"];
                        double r = System.Math.Cbrt((3.0 * v) / (4.0 * System.Math.PI));
                        double a = 4 * System.Math.PI * r * r;
                        double d = 2 * r;
                        return new(
                            [
                                new("radius", "Radius (r)", "r", r, UnitType.Length, true),
                                new("diameter", "Diameter (d)", "d", d, UnitType.Length, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true)
                            ],
                            [
                                new("Radius from Volume", "r = ∛(3V / 4π)", $"r = ∛(3·{v:F2} / 4π) = {r:F4}")
                            ],
                            null,
                            GenerateSphereLines(r)
                        );
                    }
                )
            ]
        ),

        // 2. HEMISPHERE
        new(
            "hemisphere",
            "Hemisphere",
            "3D",
            "Half of a sphere bounded by a great circular flat plane.",
            "\uF158",
            [
                new(
                    "by_radius",
                    "By Radius (r)",
                    "Calculate hemisphere from radius r.",
                    [ new("r", "Radius (r)", "r", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double r = vals["r"];
                        double v = (2.0 / 3.0) * System.Math.PI * System.Math.Pow(r, 3);
                        double curved = 2 * System.Math.PI * r * r;
                        double baseArea = System.Math.PI * r * r;
                        double total = curved + baseArea;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("curved_area", "Curved Surface Area", "A_c", curved, UnitType.Area),
                                new("base_area", "Base Circle Area", "A_b", baseArea, UnitType.Area)
                            ],
                            [
                                new("Hemisphere Volume", "V = (2/3) · π · r³", $"V = (2/3) · π · {r:F2}³ = {v:F4}"),
                                new("Total Surface Area", "A = 3 · π · r²", $"A = 3 · π · {r:F2}² = {total:F4}")
                            ],
                            null,
                            GenerateHemisphereLines(r)
                        );
                    }
                )
            ]
        ),

        // 3. CUBE
        new(
            "cube",
            "Cube",
            "3D",
            "Regular hexahedron with six congruent square faces.",
            "\uF158",
            [
                new(
                    "by_edge",
                    "By Edge Length (a)",
                    "Calculate cube from edge length a.",
                    [ new("a", "Edge Length (a)", "a", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double v = System.Math.Pow(a, 3);
                        double area = 6 * a * a;
                        double spaceD = a * System.Math.Sqrt(3.0);
                        double faceD = a * System.Math.Sqrt(2.0);
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", area, UnitType.Area, true),
                                new("space_diag", "Space Diagonal (d)", "d", spaceD, UnitType.Length, true),
                                new("face_diag", "Face Diagonal (d_f)", "d_f", faceD, UnitType.Length)
                            ],
                            [
                                new("Cube Volume", "V = a³", $"V = {a:F2}³ = {v:F4}"),
                                new("Surface Area", "A = 6 · a²", $"A = 6 · {a:F2}² = {area:F4}"),
                                new("Space Diagonal", "d = a · √3", $"d = {a:F2} · √3 = {spaceD:F4}")
                            ],
                            null,
                            GenerateCuboidLines(a, a, a)
                        );
                    }
                )
            ]
        ),

        // 4. CUBOID (RECTANGULAR PRISM)
        new(
            "cuboid",
            "Cuboid (Rectangular Prism)",
            "3D",
            "Six-faced convex solid with rectangular faces meeting at right angles.",
            "\uF158",
            [
                new(
                    "by_dims",
                    "Length, Width & Height",
                    "Calculate cuboid from length l, width w, and height h.",
                    [
                        new("l", "Length (l)", "l", UnitType.Length, 8.0),
                        new("w", "Width (w)", "w", UnitType.Length, 5.0),
                        new("h", "Height (h)", "h", UnitType.Length, 4.0)
                    ],
                    vals =>
                    {
                        double l = vals["l"], w = vals["w"], h = vals["h"];
                        double v = l * w * h;
                        double a = 2 * (l*w + l*h + w*h);
                        double d = System.Math.Sqrt(l*l + w*w + h*h);
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true),
                                new("diagonal", "Space Diagonal (d)", "d", d, UnitType.Length, true)
                            ],
                            [
                                new("Cuboid Volume", "V = l · w · h", $"V = {l:F2} · {w:F2} · {h:F2} = {v:F4}"),
                                new("Surface Area", "A = 2(lw + lh + wh)", $"A = 2({l*w:F2} + {l*h:F2} + {w*h:F2}) = {a:F4}"),
                                new("Diagonal", "d = √(l² + w² + h²)", $"d = √({l:F2}² + {w:F2}² + {h:F2}²) = {d:F4}")
                            ],
                            null,
                            GenerateCuboidLines(l, w, h)
                        );
                    }
                )
            ]
        ),

        // 5. CYLINDER
        new(
            "cylinder",
            "Cylinder",
            "3D",
            "Curvilinear geometric solid bounded by a circular cylinder surface and two parallel disks.",
            "\uF158",
            [
                new(
                    "by_radius_height",
                    "Radius (r) & Height (h)",
                    "Calculate cylinder from base radius r and height h.",
                    [
                        new("r", "Radius (r)", "r", UnitType.Length, 4.0),
                        new("h", "Height (h)", "h", UnitType.Length, 10.0)
                    ],
                    vals =>
                    {
                        double r = vals["r"], h = vals["h"];
                        double v = System.Math.PI * r * r * h;
                        double lateral = 2 * System.Math.PI * r * h;
                        double baseA = System.Math.PI * r * r;
                        double total = 2 * baseA + lateral;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("lateral_area", "Lateral Surface Area", "A_l", lateral, UnitType.Area),
                                new("base_area", "Base Area (A_b)", "A_b", baseA, UnitType.Area)
                            ],
                            [
                                new("Cylinder Volume", "V = π · r² · h", $"V = π · {r:F2}² · {h:F2} = {v:F4}"),
                                new("Lateral Area", "A_l = 2 · π · r · h", $"A_l = 2 · π · {r:F2} · {h:F2} = {lateral:F4}")
                            ],
                            null,
                            GenerateCylinderLines(r, h)
                        );
                    }
                )
            ]
        ),

        // 6. CONE
        new(
            "cone",
            "Cone",
            "3D",
            "Three-dimensional geometric shape that tapers smoothly from a flat base to an apex point.",
            "\uF158",
            [
                new(
                    "by_radius_height",
                    "Radius (r) & Height (h)",
                    "Calculate cone from radius r and height h.",
                    [
                        new("r", "Base Radius (r)", "r", UnitType.Length, 4.0),
                        new("h", "Height (h)", "h", UnitType.Length, 9.0)
                    ],
                    vals =>
                    {
                        double r = vals["r"], h = vals["h"];
                        double s = System.Math.Sqrt(r*r + h*h);
                        double v = (1.0 / 3.0) * System.Math.PI * r * r * h;
                        double lateral = System.Math.PI * r * s;
                        double baseA = System.Math.PI * r * r;
                        double total = baseA + lateral;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("slant_height", "Slant Height (s)", "s", s, UnitType.Length, true),
                                new("lateral_area", "Lateral Surface Area", "A_l", lateral, UnitType.Area)
                            ],
                            [
                                new("Slant Height", "s = √(r² + h²)", $"s = √({r:F2}² + {h:F2}²) = {s:F4}"),
                                new("Cone Volume", "V = (1/3) · π · r² · h", $"V = (1/3) · π · {r:F2}² · {h:F2} = {v:F4}")
                            ],
                            null,
                            GenerateConeLines(r, h)
                        );
                    }
                )
            ]
        ),

        // 7. FRUSTUM OF A CONE (TRUNCATED CONE)
        new(
            "frustum_cone",
            "Frustum of a Cone",
            "3D",
            "Portion of a cone that lies between two parallel planes cutting through it.",
            "\uF158",
            [
                new(
                    "by_radii_height",
                    "Top & Bottom Radii & Height",
                    "Calculate truncated cone from top radius r1, bottom radius r2, and height h.",
                    [
                        new("r1", "Top Radius (r₁)", "r₁", UnitType.Length, 3.0),
                        new("r2", "Bottom Radius (r₂)", "r₂", UnitType.Length, 6.0),
                        new("h", "Height (h)", "h", UnitType.Length, 8.0)
                    ],
                    vals =>
                    {
                        double r1 = vals["r1"], r2 = vals["r2"], h = vals["h"];
                        double dr = System.Math.Abs(r2 - r1);
                        double s = System.Math.Sqrt(dr*dr + h*h);
                        double v = (System.Math.PI * h / 3.0) * (r1*r1 + r1*r2 + r2*r2);
                        double lateral = System.Math.PI * (r1 + r2) * s;
                        double aTop = System.Math.PI * r1 * r1;
                        double aBot = System.Math.PI * r2 * r2;
                        double total = aTop + aBot + lateral;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("slant_height", "Slant Height (s)", "s", s, UnitType.Length, true),
                                new("lateral_area", "Lateral Surface Area", "A_l", lateral, UnitType.Area)
                            ],
                            [
                                new("Frustum Volume", "V = (πh/3) · (r₁² + r₁r₂ + r₂²)", $"V = (π·{h:F2}/3) · ({r1:F2}² + {r1*r2:F2} + {r2:F2}²) = {v:F4}"),
                                new("Slant Height", "s = √((r₂ - r₁)² + h²)", $"s = {s:F4}")
                            ],
                            null,
                            GenerateFrustumLines(r1, r2, h)
                        );
                    }
                )
            ]
        ),

        // 8. REGULAR PYRAMID
        new(
            "regular_pyramid",
            "Regular Pyramid",
            "3D",
            "Pyramid whose base is a regular polygon and whose apex is directly above base center.",
            "\uF158",
            [
                new(
                    "by_base_height",
                    "Base Side (a) & Height (h)",
                    "Calculate square pyramid from base side a and height h.",
                    [
                        new("a", "Square Base Side (a)", "a", UnitType.Length, 6.0),
                        new("h", "Height (h)", "h", UnitType.Length, 8.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], h = vals["h"];
                        double baseA = a * a;
                        double v = (1.0 / 3.0) * baseA * h;
                        double s = System.Math.Sqrt(h*h + System.Math.Pow(a / 2.0, 2));
                        double lateral = 2 * a * s;
                        double total = baseA + lateral;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("slant_height", "Face Slant Height (s)", "s", s, UnitType.Length, true),
                                new("lateral_area", "Lateral Surface Area", "A_l", lateral, UnitType.Area)
                            ],
                            [
                                new("Pyramid Volume", "V = (1/3) · a² · h", $"V = (1/3) · {a:F2}² · {h:F2} = {v:F4}"),
                                new("Slant Height", "s = √(h² + (a/2)²)", $"s = {s:F4}")
                            ],
                            null,
                            GeneratePyramidLines(a, h)
                        );
                    }
                )
            ]
        ),

        // 9. TRIANGULAR PRISM
        new(
            "triangular_prism",
            "Triangular Prism",
            "3D",
            "Three-sided prism with two congruent triangular bases and three rectangular lateral sides.",
            "\uF158",
            [
                new(
                    "by_sides_length",
                    "Base Triangle Sides (a, b, c) & Length (L)",
                    "Calculate triangular prism from three base edges and prism length.",
                    [
                        new("a", "Base Side a", "a", UnitType.Length, 3.0),
                        new("b", "Base Side b", "b", UnitType.Length, 4.0),
                        new("c", "Base Side c", "c", UnitType.Length, 5.0),
                        new("L", "Prism Length (L)", "L", UnitType.Length, 10.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], c = vals["c"], L = vals["L"];
                        double s = (a + b + c) / 2.0;
                        double baseA = System.Math.Sqrt(System.Math.Max(0, s * (s - a) * (s - b) * (s - c)));
                        double v = baseA * L;
                        double lateral = (a + b + c) * L;
                        double total = 2 * baseA + lateral;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("base_area", "Single Base Area", "A_b", baseA, UnitType.Area),
                                new("lateral_area", "Lateral Surface Area", "A_l", lateral, UnitType.Area)
                            ],
                            [
                                new("Base Area (Heron)", "A_b = √(s(s-a)(s-b)(s-c))", $"A_b = {baseA:F4}"),
                                new("Prism Volume", "V = A_b · L", $"V = {baseA:F4} · {L:F2} = {v:F4}")
                            ],
                            null,
                            GenerateTriangularPrismLines(a, b, c, L)
                        );
                    }
                )
            ]
        ),

        // 10. TORUS (DONUT)
        new(
            "torus",
            "Torus (Donut)",
            "3D",
            "Surface of revolution generated by revolving a circle in three-dimensional space.",
            "\uF158",
            [
                new(
                    "by_radii",
                    "Major Radius (R) & Minor Radius (r)",
                    "Calculate torus from ring center radius R and tube cross-section radius r.",
                    [
                        new("R", "Major Radius (R)", "R", UnitType.Length, 8.0),
                        new("r", "Minor Radius (r)", "r", UnitType.Length, 2.0)
                    ],
                    vals =>
                    {
                        double R = vals["R"], r = vals["r"];
                        if (r > R) throw new ArgumentException("Minor tube radius r cannot exceed major ring radius R.");
                        double v = 2 * System.Math.PI * System.Math.PI * R * r * r;
                        double a = 4 * System.Math.PI * System.Math.PI * R * r;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true)
                            ],
                            [
                                new("Torus Volume", "V = 2π² · R · r²", $"V = 2π² · {R:F2} · {r:F2}² = {v:F4}"),
                                new("Torus Surface Area", "A = 4π² · R · r", $"A = 4π² · {R:F2} · {r:F2} = {a:F4}")
                            ],
                            null,
                            GenerateTorusLines(R, r)
                        );
                    }
                )
            ]
        ),

        // 11. ELLIPSOID
        new(
            "ellipsoid",
            "Ellipsoid",
            "3D",
            "Closed quadric surface that is a three-dimensional analogue of an ellipse.",
            "\uF158",
            [
                new(
                    "by_semi_axes",
                    "Semi-Axes (a, b, c)",
                    "Calculate ellipsoid from semi-principal axes a, b, and c.",
                    [
                        new("a", "Semi-Axis a", "a", UnitType.Length, 6.0),
                        new("b", "Semi-Axis b", "b", UnitType.Length, 4.0),
                        new("c", "Semi-Axis c", "c", UnitType.Length, 3.0)
                    ],
                    vals =>
                    {
                        double a = vals["a"], b = vals["b"], c = vals["c"];
                        double v = (4.0 / 3.0) * System.Math.PI * a * b * c;
                        // Knud Thomsen approximation for surface area with p ≈ 1.6075
                        const double p = 1.6075;
                        double ap = System.Math.Pow(a*b, p) + System.Math.Pow(a*c, p) + System.Math.Pow(b*c, p);
                        double sa = 4 * System.Math.PI * System.Math.Pow(ap / 3.0, 1.0 / p);
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A ≈)", "A", sa, UnitType.Area, true)
                            ],
                            [
                                new("Ellipsoid Volume", "V = (4/3) · π · a · b · c", $"V = (4/3) · π · {a:F2} · {b:F2} · {c:F2} = {v:F4}"),
                                new("Knud Thomsen Surface Area", "A ≈ 4π · [((ab)^p + (ac)^p + (bc)^p)/3]^(1/p)", $"A ≈ {sa:F4}")
                            ],
                            null,
                            GenerateEllipsoidLines(a, b, c)
                        );
                    }
                )
            ]
        ),

        // 12. CAPSULE
        new(
            "capsule",
            "Capsule",
            "3D",
            "Geometric solid composed of a cylinder bounded by two hemispherical caps.",
            "\uF158",
            [
                new(
                    "by_radius_length",
                    "Cap Radius (r) & Cylinder Length (L)",
                    "Calculate capsule from end cap radius r and middle cylinder length L.",
                    [
                        new("r", "Cap Radius (r)", "r", UnitType.Length, 3.0),
                        new("L", "Cylinder Length (L)", "L", UnitType.Length, 8.0)
                    ],
                    vals =>
                    {
                        double r = vals["r"], L = vals["L"];
                        double v = System.Math.PI * r * r * ((4.0 / 3.0) * r + L);
                        double a = 2 * System.Math.PI * r * (2 * r + L);
                        double totalLen = L + 2 * r;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Surface Area (A)", "A", a, UnitType.Area, true),
                                new("total_len", "Total Length", "L_tot", totalLen, UnitType.Length)
                            ],
                            [
                                new("Capsule Volume", "V = πr² · ((4/3)r + L)", $"V = π·{r:F2}² · ((4/3)·{r:F2} + {L:F2}) = {v:F4}"),
                                new("Surface Area", "A = 2πr · (2r + L)", $"A = 2π·{r:F2} · (2·{r:F2} + {L:F2}) = {a:F4}")
                            ],
                            null,
                            GenerateCapsuleLines(r, L)
                        );
                    }
                )
            ]
        ),

        // 13. SPHERICAL CAP (DOME)
        new(
            "spherical_cap",
            "Spherical Cap (Dome)",
            "3D",
            "Portion of a sphere cut off by a plane.",
            "\uF158",
            [
                new(
                    "by_radius_height",
                    "Sphere Radius (R) & Cap Height (h)",
                    "Calculate dome from sphere radius R and cap height h.",
                    [
                        new("R", "Sphere Radius (R)", "R", UnitType.Length, 6.0),
                        new("h", "Cap Height (h)", "h", UnitType.Length, 3.0)
                    ],
                    vals =>
                    {
                        double R = vals["R"], h = vals["h"];
                        if (h > 2 * R) throw new ArgumentException("Cap height cannot exceed sphere diameter (2R).");
                        double aBase = System.Math.Sqrt(System.Math.Max(0, h * (2 * R - h)));
                        double v = (System.Math.PI * h * h / 3.0) * (3 * R - h);
                        double curved = 2 * System.Math.PI * R * h;
                        double baseArea = System.Math.PI * aBase * aBase;
                        double total = curved + baseArea;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("total_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("curved_area", "Curved Cap Area", "A_c", curved, UnitType.Area),
                                new("base_radius", "Base Circle Radius (a)", "a", aBase, UnitType.Length)
                            ],
                            [
                                new("Spherical Cap Volume", "V = (πh²/3) · (3R - h)", $"V = (π·{h:F2}²/3) · (3·{R:F2} - {h:F2}) = {v:F4}")
                            ],
                            null,
                            GenerateSphericalCapLines(R, h)
                        );
                    }
                )
            ]
        ),

        // 14. HOLLOW CYLINDER (PIPE)
        new(
            "hollow_cylinder",
            "Hollow Cylinder (Pipe)",
            "3D",
            "Cylindrical pipe with an outer radius R, inner hole radius r, and height h.",
            "\uF158",
            [
                new(
                    "by_radii_height",
                    "Radii (R, r) & Height (h)",
                    "Calculate pipe from outer radius R, inner radius r, and length h.",
                    [
                        new("R", "Outer Radius (R)", "R", UnitType.Length, 5.0),
                        new("r", "Inner Radius (r)", "r", UnitType.Length, 3.0),
                        new("h", "Height (h)", "h", UnitType.Length, 10.0)
                    ],
                    vals =>
                    {
                        double R = vals["R"], r = vals["r"], h = vals["h"];
                        if (r >= R) throw new ArgumentException("Inner radius r must be strictly less than outer radius R.");
                        double v = System.Math.PI * h * (R*R - r*r);
                        double latOut = 2 * System.Math.PI * R * h;
                        double latIn = 2 * System.Math.PI * r * h;
                        double ends = 2 * System.Math.PI * (R*R - r*r);
                        double total = latOut + latIn + ends;
                        return new(
                            [
                                new("volume", "Solid Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", total, UnitType.Area, true),
                                new("outer_area", "Outer Lateral Area", "A_out", latOut, UnitType.Area),
                                new("inner_area", "Inner Lateral Area", "A_in", latIn, UnitType.Area)
                            ],
                            [
                                new("Hollow Cylinder Volume", "V = π · h · (R² - r²)", $"V = π · {h:F2} · ({R:F2}² - {r:F2}²) = {v:F4}")
                            ],
                            null,
                            GenerateHollowCylinderLines(R, r, h)
                        );
                    }
                )
            ]
        ),

        // 15. REGULAR TETRAHEDRON
        new(
            "tetrahedron",
            "Regular Tetrahedron",
            "3D",
            "Platonic solid with four equilateral triangular faces, four vertices, and six edges.",
            "\uF158",
            [
                new(
                    "by_edge",
                    "By Edge Length (a)",
                    "Calculate regular tetrahedron from edge length a.",
                    [ new("a", "Edge Length (a)", "a", UnitType.Length, 6.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double v = (System.Math.Pow(a, 3)) / (6.0 * System.Math.Sqrt(2.0));
                        double area = System.Math.Sqrt(3.0) * a * a;
                        double h = a * System.Math.Sqrt(2.0 / 3.0);
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", area, UnitType.Area, true),
                                new("height", "Height (h)", "h", h, UnitType.Length, true)
                            ],
                            [
                                new("Tetrahedron Volume", "V = a³ / (6√2)", $"V = {a:F2}³ / (6√2) = {v:F4}"),
                                new("Surface Area", "A = √3 · a²", $"A = √3 · {a:F2}² = {area:F4}")
                            ],
                            null,
                            GenerateTetrahedronLines(a)
                        );
                    }
                )
            ]
        ),

        // 16. REGULAR OCTAHEDRON
        new(
            "octahedron",
            "Regular Octahedron",
            "3D",
            "Platonic solid with eight equilateral triangular faces, six vertices, and twelve edges.",
            "\uF158",
            [
                new(
                    "by_edge",
                    "By Edge Length (a)",
                    "Calculate regular octahedron from edge length a.",
                    [ new("a", "Edge Length (a)", "a", UnitType.Length, 5.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double v = (System.Math.Sqrt(2.0) / 3.0) * System.Math.Pow(a, 3);
                        double area = 2 * System.Math.Sqrt(3.0) * a * a;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", area, UnitType.Area, true)
                            ],
                            [
                                new("Octahedron Volume", "V = (√2 / 3) · a³", $"V = (√2 / 3) · {a:F2}³ = {v:F4}"),
                                new("Surface Area", "A = 2√3 · a²", $"A = 2√3 · {a:F2}² = {area:F4}")
                            ],
                            null,
                            GenerateOctahedronLines(a)
                        );
                    }
                )
            ]
        ),

        // 17. REGULAR ICOSAHEDRON
        new(
            "icosahedron",
            "Regular Icosahedron",
            "3D",
            "Platonic solid with twenty equilateral triangular faces and twelve vertices.",
            "\uF158",
            [
                new(
                    "by_edge",
                    "By Edge Length (a)",
                    "Calculate regular icosahedron from edge length a.",
                    [ new("a", "Edge Length (a)", "a", UnitType.Length, 4.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double v = (5.0 * (3 + System.Math.Sqrt(5.0)) / 12.0) * System.Math.Pow(a, 3);
                        double area = 5 * System.Math.Sqrt(3.0) * a * a;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", area, UnitType.Area, true)
                            ],
                            [
                                new("Icosahedron Volume", "V = (5(3+√5)/12) · a³", $"V = {v:F4}"),
                                new("Surface Area", "A = 5√3 · a²", $"A = {area:F4}")
                            ],
                            null,
                            GenerateIcosahedronLines(a)
                        );
                    }
                )
            ]
        ),

        // 18. REGULAR DODECAHEDRON
        new(
            "dodecahedron",
            "Regular Dodecahedron",
            "3D",
            "Platonic solid with twelve congruent regular pentagonal faces and twenty vertices.",
            "\uF158",
            [
                new(
                    "by_edge",
                    "By Edge Length (a)",
                    "Calculate regular dodecahedron from edge length a.",
                    [ new("a", "Edge Length (a)", "a", UnitType.Length, 4.0) ],
                    vals =>
                    {
                        double a = vals["a"];
                        double v = ((15.0 + 7.0 * System.Math.Sqrt(5.0)) / 4.0) * System.Math.Pow(a, 3);
                        double pConst = System.Math.Sqrt(25.0 + 10.0 * System.Math.Sqrt(5.0));
                        double area = 3 * pConst * a * a;
                        return new(
                            [
                                new("volume", "Volume (V)", "V", v, UnitType.Volume, true),
                                new("surface_area", "Total Surface Area (A)", "A", area, UnitType.Area, true)
                            ],
                            [
                                new("Dodecahedron Volume", "V = ((15 + 7√5) / 4) · a³", $"V = {v:F4}"),
                                new("Surface Area", "A = 3√(25 + 10√5) · a²", $"A = {area:F4}")
                            ],
                            null,
                            GenerateDodecahedronLines(a)
                        );
                    }
                )
            ]
        )
    ];

    #region 3D Wireframe Line Generators

    public static List<Line3D> GenerateSphereLines(double r, int latRings = 5, int lonRings = 8, int segs = 24)
    {
        List<Line3D> lines = [];
        // Latitude rings
        for (int i = 1; i < latRings; i++)
        {
            double phi = System.Math.PI * i / latRings - (System.Math.PI / 2.0);
            double ringR = r * System.Math.Cos(phi);
            double y = r * System.Math.Sin(phi);
            for (int j = 0; j < segs; j++)
            {
                double th1 = (2 * System.Math.PI * j) / segs;
                double th2 = (2 * System.Math.PI * (j + 1)) / segs;
                lines.Add(new(
                    new(ringR * System.Math.Cos(th1), y, ringR * System.Math.Sin(th1)),
                    new(ringR * System.Math.Cos(th2), y, ringR * System.Math.Sin(th2))
                ));
            }
        }
        // Longitude rings
        for (int i = 0; i < lonRings; i++)
        {
            double lon = (System.Math.PI * i) / lonRings;
            for (int j = 0; j < segs; j++)
            {
                double th1 = (2 * System.Math.PI * j) / segs;
                double th2 = (2 * System.Math.PI * (j + 1)) / segs;
                lines.Add(new(
                    new(r * System.Math.Cos(th1) * System.Math.Cos(lon), r * System.Math.Sin(th1), r * System.Math.Cos(th1) * System.Math.Sin(lon)),
                    new(r * System.Math.Cos(th2) * System.Math.Cos(lon), r * System.Math.Sin(th2), r * System.Math.Cos(th2) * System.Math.Sin(lon))
                ));
            }
        }
        return lines;
    }

    public static List<Line3D> GenerateHemisphereLines(double r, int segs = 24)
    {
        List<Line3D> lines = [];
        // Base disk
        for (int j = 0; j < segs; j++)
        {
            double th1 = (2 * System.Math.PI * j) / segs;
            double th2 = (2 * System.Math.PI * (j + 1)) / segs;
            lines.Add(new(new(r * System.Math.Cos(th1), 0, r * System.Math.Sin(th1)), new(r * System.Math.Cos(th2), 0, r * System.Math.Sin(th2))));
        }
        // Dome rings
        for (int i = 1; i <= 3; i++)
        {
            double phi = (System.Math.PI / 2.0) * i / 4.0;
            double ringR = r * System.Math.Cos(phi);
            double y = r * System.Math.Sin(phi);
            for (int j = 0; j < segs; j++)
            {
                double th1 = (2 * System.Math.PI * j) / segs;
                double th2 = (2 * System.Math.PI * (j + 1)) / segs;
                lines.Add(new(new(ringR * System.Math.Cos(th1), y, ringR * System.Math.Sin(th1)), new(ringR * System.Math.Cos(th2), y, ringR * System.Math.Sin(th2))));
            }
        }
        return lines;
    }

    public static List<Line3D> GenerateCuboidLines(double l, double w, double h)
    {
        double x = l / 2.0, y = h / 2.0, z = w / 2.0;
        Point3D[] v =
        [
            new(-x, -y, -z), new(x, -y, -z), new(x, y, -z), new(-x, y, -z),
            new(-x, -y, z), new(x, -y, z), new(x, y, z), new(-x, y, z)
        ];
        return
        [
            // Bottom face
            new(v[0], v[1]), new(v[1], v[2]), new(v[2], v[3]), new(v[3], v[0]),
            // Top face
            new(v[4], v[5]), new(v[5], v[6]), new(v[6], v[7]), new(v[7], v[4]),
            // Vertical pillars
            new(v[0], v[4]), new(v[1], v[5]), new(v[2], v[6]), new(v[3], v[7])
        ];
    }

    public static List<Line3D> GenerateCylinderLines(double r, double h, int segs = 24)
    {
        List<Line3D> lines = [];
        double yBot = -h / 2.0, yTop = h / 2.0;
        for (int j = 0; j < segs; j++)
        {
            double th1 = (2 * System.Math.PI * j) / segs;
            double th2 = (2 * System.Math.PI * (j + 1)) / segs;
            Point3D b1 = new(r * System.Math.Cos(th1), yBot, r * System.Math.Sin(th1));
            Point3D b2 = new(r * System.Math.Cos(th2), yBot, r * System.Math.Sin(th2));
            Point3D t1 = new(r * System.Math.Cos(th1), yTop, r * System.Math.Sin(th1));
            Point3D t2 = new(r * System.Math.Cos(th2), yTop, r * System.Math.Sin(th2));

            lines.Add(new(b1, b2));
            lines.Add(new(t1, t2));
            if (j % 4 == 0) lines.Add(new(b1, t1));
        }
        return lines;
    }

    public static List<Line3D> GenerateConeLines(double r, double h, int segs = 24)
    {
        List<Line3D> lines = [];
        double yBot = -h / 2.0, yTop = h / 2.0;
        Point3D apex = new(0, yTop, 0);
        for (int j = 0; j < segs; j++)
        {
            double th1 = (2 * System.Math.PI * j) / segs;
            double th2 = (2 * System.Math.PI * (j + 1)) / segs;
            Point3D b1 = new(r * System.Math.Cos(th1), yBot, r * System.Math.Sin(th1));
            Point3D b2 = new(r * System.Math.Cos(th2), yBot, r * System.Math.Sin(th2));
            lines.Add(new(b1, b2));
            if (j % 4 == 0) lines.Add(new(b1, apex));
        }
        return lines;
    }

    public static List<Line3D> GenerateFrustumLines(double r1, double r2, double h, int segs = 24)
    {
        List<Line3D> lines = [];
        double yBot = -h / 2.0, yTop = h / 2.0;
        for (int j = 0; j < segs; j++)
        {
            double th1 = (2 * System.Math.PI * j) / segs;
            double th2 = (2 * System.Math.PI * (j + 1)) / segs;
            Point3D b1 = new(r2 * System.Math.Cos(th1), yBot, r2 * System.Math.Sin(th1));
            Point3D b2 = new(r2 * System.Math.Cos(th2), yBot, r2 * System.Math.Sin(th2));
            Point3D t1 = new(r1 * System.Math.Cos(th1), yTop, r1 * System.Math.Sin(th1));
            Point3D t2 = new(r1 * System.Math.Cos(th2), yTop, r1 * System.Math.Sin(th2));

            lines.Add(new(b1, b2));
            lines.Add(new(t1, t2));
            if (j % 4 == 0) lines.Add(new(b1, t1));
        }
        return lines;
    }

    public static List<Line3D> GeneratePyramidLines(double a, double h)
    {
        double x = a / 2.0, yBot = -h / 2.0, yTop = h / 2.0;
        Point3D apex = new(0, yTop, 0);
        Point3D[] b = [new(-x, yBot, -x), new(x, yBot, -x), new(x, yBot, x), new(-x, yBot, x)];
        return
        [
            new(b[0], b[1]), new(b[1], b[2]), new(b[2], b[3]), new(b[3], b[0]),
            new(b[0], apex), new(b[1], apex), new(b[2], apex), new(b[3], apex)
        ];
    }

    public static List<Line3D> GenerateTriangularPrismLines(double a, double b, double c, double L)
    {
        double z1 = -L / 2.0, z2 = L / 2.0;
        double s = (a + b + c) / 2.0;
        double area = System.Math.Sqrt(System.Math.Max(0, s * (s - a) * (s - b) * (s - c)));
        double h = (2.0 * area) / c;
        double cx = (c*c - a*a + b*b) / (2 * c);

        Point3D[] v1 = [new(-c/2, -h/2, z1), new(c/2, -h/2, z1), new(cx - c/2, h/2, z1)];
        Point3D[] v2 = [new(-c/2, -h/2, z2), new(c/2, -h/2, z2), new(cx - c/2, h/2, z2)];

        return
        [
            new(v1[0], v1[1]), new(v1[1], v1[2]), new(v1[2], v1[0]),
            new(v2[0], v2[1]), new(v2[1], v2[2]), new(v2[2], v2[0]),
            new(v1[0], v2[0]), new(v1[1], v2[1]), new(v1[2], v2[2])
        ];
    }

    public static List<Line3D> GenerateTorusLines(double R, double r, int majorSegs = 16, int minorSegs = 12)
    {
        List<Line3D> lines = [];
        for (int i = 0; i < majorSegs; i++)
        {
            double u = (2 * System.Math.PI * i) / majorSegs;
            double cosU = System.Math.Cos(u), sinU = System.Math.Sin(u);

            for (int j = 0; j < minorSegs; j++)
            {
                double v1 = (2 * System.Math.PI * j) / minorSegs;
                double v2 = (2 * System.Math.PI * (j + 1)) / minorSegs;

                Point3D p1 = new((R + r * System.Math.Cos(v1)) * cosU, r * System.Math.Sin(v1), (R + r * System.Math.Cos(v1)) * sinU);
                Point3D p2 = new((R + r * System.Math.Cos(v2)) * cosU, r * System.Math.Sin(v2), (R + r * System.Math.Cos(v2)) * sinU);
                lines.Add(new(p1, p2));
            }
        }
        return lines;
    }

    public static List<Line3D> GenerateEllipsoidLines(double a, double b, double c, int rings = 6, int segs = 20)
    {
        List<Line3D> lines = [];
        for (int i = 1; i < rings; i++)
        {
            double phi = System.Math.PI * i / rings - (System.Math.PI / 2.0);
            double y = b * System.Math.Sin(phi);
            double cosPhi = System.Math.Cos(phi);
            for (int j = 0; j < segs; j++)
            {
                double th1 = (2 * System.Math.PI * j) / segs;
                double th2 = (2 * System.Math.PI * (j + 1)) / segs;
                lines.Add(new(
                    new(a * cosPhi * System.Math.Cos(th1), y, c * cosPhi * System.Math.Sin(th1)),
                    new(a * cosPhi * System.Math.Cos(th2), y, c * cosPhi * System.Math.Sin(th2))
                ));
            }
        }
        return lines;
    }

    public static List<Line3D> GenerateCapsuleLines(double r, double L)
    {
        List<Line3D> lines = GenerateCylinderLines(r, L, 16);
        return lines;
    }

    public static List<Line3D> GenerateSphericalCapLines(double R, double h)
    {
        return GenerateHemisphereLines(R, 16);
    }

    public static List<Line3D> GenerateHollowCylinderLines(double R, double r, double h)
    {
        List<Line3D> lines = GenerateCylinderLines(R, h, 16);
        lines.AddRange(GenerateCylinderLines(r, h, 16));
        return lines;
    }

    public static List<Line3D> GenerateTetrahedronLines(double a)
    {
        double s = a / System.Math.Sqrt(2.0);
        Point3D[] v = [new(s, s, s), new(-s, -s, s), new(-s, s, -s), new(s, -s, -s)];
        return
        [
            new(v[0], v[1]), new(v[0], v[2]), new(v[0], v[3]),
            new(v[1], v[2]), new(v[2], v[3]), new(v[3], v[1])
        ];
    }

    public static List<Line3D> GenerateOctahedronLines(double a)
    {
        double s = a / System.Math.Sqrt(2.0);
        Point3D[] v = [new(s, 0, 0), new(-s, 0, 0), new(0, s, 0), new(0, -s, 0), new(0, 0, s), new(0, 0, -s)];
        return
        [
            new(v[0], v[2]), new(v[2], v[1]), new(v[1], v[3]), new(v[3], v[0]),
            new(v[0], v[4]), new(v[1], v[4]), new(v[2], v[4]), new(v[3], v[4]),
            new(v[0], v[5]), new(v[1], v[5]), new(v[2], v[5]), new(v[3], v[5])
        ];
    }

    public static List<Line3D> GenerateIcosahedronLines(double a)
    {
        double phi = (1.0 + System.Math.Sqrt(5.0)) / 2.0;
        double scale = a / 2.0;
        Point3D[] v =
        [
            new(0, scale, phi * scale), new(0, -scale, phi * scale), new(0, scale, -phi * scale), new(0, -scale, -phi * scale),
            new(scale, phi * scale, 0), new(-scale, phi * scale, 0), new(scale, -phi * scale, 0), new(-scale, -phi * scale, 0),
            new(phi * scale, 0, scale), new(-phi * scale, 0, scale), new(phi * scale, 0, -scale), new(-phi * scale, 0, -scale)
        ];
        List<Line3D> lines = [];
        for (int i = 0; i < v.Length; i++)
        {
            for (int j = i + 1; j < v.Length; j++)
            {
                double dist = System.Math.Sqrt(System.Math.Pow(v[i].X - v[j].X, 2) + System.Math.Pow(v[i].Y - v[j].Y, 2) + System.Math.Pow(v[i].Z - v[j].Z, 2));
                if (System.Math.Abs(dist - a) < a * 0.15)
                {
                    lines.Add(new(v[i], v[j]));
                }
            }
        }
        return lines;
    }

    public static List<Line3D> GenerateDodecahedronLines(double a)
    {
        double phi = (1.0 + System.Math.Sqrt(5.0)) / 2.0;
        double s = a / (phi - 1.0);
        Point3D[] v =
        [
            new(s, s, s), new(s, s, -s), new(s, -s, s), new(s, -s, -s),
            new(-s, s, s), new(-s, s, -s), new(-s, -s, s), new(-s, -s, -s),
            new(0, s / phi, s * phi), new(0, s / phi, -s * phi), new(0, -s / phi, s * phi), new(0, -s / phi, -s * phi),
            new(s / phi, s * phi, 0), new(s / phi, -s * phi, 0), new(-s / phi, s * phi, 0), new(-s / phi, -s * phi, 0),
            new(s * phi, 0, s / phi), new(s * phi, 0, -s / phi), new(-s * phi, 0, s / phi), new(-s * phi, 0, -s / phi)
        ];
        List<Line3D> lines = [];
        for (int i = 0; i < v.Length; i++)
        {
            for (int j = i + 1; j < v.Length; j++)
            {
                double dist = System.Math.Sqrt(System.Math.Pow(v[i].X - v[j].X, 2) + System.Math.Pow(v[i].Y - v[j].Y, 2) + System.Math.Pow(v[i].Z - v[j].Z, 2));
                if (System.Math.Abs(dist - (2 * s / phi)) < s * 0.1)
                {
                    lines.Add(new(v[i], v[j]));
                }
            }
        }
        return lines;
    }

    #endregion
}
