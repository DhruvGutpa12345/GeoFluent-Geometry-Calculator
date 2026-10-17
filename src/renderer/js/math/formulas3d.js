/**
 * GeoFluent 3D Geometry Engine
 * Complete 3D solid shapes formulas, multi-method calculation, step derivations, and 3D visualizer parameters.
 */

import { UNIT_TYPES } from './units.js';

export const SHAPES_3D = [
  // 1. SPHERE
  {
    id: 'sphere',
    name: 'Sphere',
    category: '3d',
    description: 'Perfect round geometrical object in three-dimensional space that is the surface of a completely round ball.',
    icon: 'sphere',
    methods: [
      {
        id: 'by_radius',
        name: 'By Radius (r)',
        description: 'Provide radius r.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r } = vals;
          const diameter = 2 * r;
          const surfaceArea = 4 * Math.PI * r * r;
          const volume = (4 / 3) * Math.PI * Math.pow(r, 3);
          const greatCircumference = 2 * Math.PI * r;
          const greatArea = Math.PI * r * r;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Surface Area (A)', symbol: 'A', value: surfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'diameter', label: 'Diameter (d)', symbol: 'd', value: diameter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'great_circumference', label: 'Great Circle Circumference', symbol: 'C_gc', value: greatCircumference, type: UNIT_TYPES.LENGTH },
              { id: 'great_area', label: 'Great Circle Area', symbol: 'A_gc', value: greatArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Sphere Volume', formula: 'V = \\frac{4}{3}\\pi r^3', substitution: `V = (4/3) \\cdot \\pi \\cdot ${r}^3 = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 4\\pi r^2', substitution: `A = 4 \\cdot \\pi \\cdot ${r}^2 = ${surfaceArea}` }
            ],
            threeData: { type: 'sphere', radius: r }
          };
        }
      },
      {
        id: 'by_diameter',
        name: 'By Diameter (d)',
        description: 'Provide diameter d.',
        inputs: [
          { id: 'd', label: 'Diameter (d)', symbol: 'd', type: UNIT_TYPES.LENGTH, default: 10, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { d } = vals;
          const r = d / 2;
          const surfaceArea = Math.PI * d * d;
          const volume = (Math.PI * Math.pow(d, 3)) / 6;

          return {
            outputs: [
              { id: 'radius', label: 'Radius (r)', symbol: 'r', value: r, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Surface Area (A)', symbol: 'A', value: surfaceArea, type: UNIT_TYPES.AREA, isPrimary: true }
            ],
            steps: [
              { title: 'Radius', formula: 'r = d / 2', substitution: `r = ${d} / 2 = ${r}` },
              { title: 'Volume', formula: 'V = \\frac{\\pi d^3}{6}', substitution: `V = (\\pi \\cdot ${d}^3) / 6 = ${volume}` }
            ],
            threeData: { type: 'sphere', radius: r }
          };
        }
      },
      {
        id: 'by_volume',
        name: 'By Volume (V)',
        description: 'Provide volume V.',
        inputs: [
          { id: 'V', label: 'Volume (V)', symbol: 'V', type: UNIT_TYPES.VOLUME, default: 523.598, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { V } = vals;
          const r = Math.cbrt((3 * V) / (4 * Math.PI));
          const surfaceArea = 4 * Math.PI * r * r;
          const diameter = 2 * r;

          return {
            outputs: [
              { id: 'radius', label: 'Radius (r)', symbol: 'r', value: r, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diameter', label: 'Diameter (d)', symbol: 'd', value: diameter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'surface_area', label: 'Surface Area (A)', symbol: 'A', value: surfaceArea, type: UNIT_TYPES.AREA, isPrimary: true }
            ],
            steps: [
              { title: 'Radius from Volume', formula: 'r = \\sqrt[3]{\\frac{3V}{4\\pi}}', substitution: `r = \\sqrt[3]{(3 \\cdot ${V}) / (4\\pi)} = ${r}` }
            ],
            threeData: { type: 'sphere', radius: r }
          };
        }
      }
    ]
  },

  // 2. HEMISPHERE
  {
    id: 'hemisphere',
    name: 'Hemisphere',
    category: '3d',
    description: 'Half of a sphere bounded by a great circular flat plane.',
    icon: 'hemisphere',
    methods: [
      {
        id: 'by_radius',
        name: 'By Radius (r)',
        description: 'Provide radius r.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r } = vals;
          const diameter = 2 * r;
          const volume = (2 / 3) * Math.PI * Math.pow(r, 3);
          const curvedSurfaceArea = 2 * Math.PI * r * r;
          const baseArea = Math.PI * r * r;
          const totalSurfaceArea = 3 * Math.PI * r * r;
          const centroidHeight = (3 / 8) * r;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A_total)', symbol: 'A_total', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'curved_area', label: 'Curved Surface Area (A_c)', symbol: 'A_c', value: curvedSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'base_area', label: 'Base Area (A_base)', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA },
              { id: 'centroid', label: 'Centroid from Base', symbol: 'h_c', value: centroidHeight, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{2}{3}\\pi r^3', substitution: `V = (2/3) \\cdot \\pi \\cdot ${r}^3 = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 3\\pi r^2', substitution: `A = 3 \\cdot \\pi \\cdot ${r}^2 = ${totalSurfaceArea}` }
            ],
            threeData: { type: 'hemisphere', radius: r }
          };
        }
      }
    ]
  },

  // 3. CUBE
  {
    id: 'cube',
    name: 'Cube',
    category: '3d',
    description: 'Regular 3D solid bounded by six equal square faces with 12 edges and 8 vertices.',
    icon: 'cube',
    methods: [
      {
        id: 'by_side',
        name: 'By Side Length (a)',
        description: 'Provide side length a.',
        inputs: [
          { id: 'a', label: 'Side length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const volume = Math.pow(a, 3);
          const totalSurfaceArea = 6 * a * a;
          const lateralSurfaceArea = 4 * a * a;
          const faceDiagonal = a * Math.SQRT2;
          const spaceDiagonal = a * Math.sqrt(3);
          const inradius = a / 2;
          const circumradius = (a * Math.sqrt(3)) / 2;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'space_diagonal', label: 'Space Diagonal (d_s)', symbol: 'd_s', value: spaceDiagonal, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'face_diagonal', label: 'Face Diagonal (d_f)', symbol: 'd_f', value: faceDiagonal, type: UNIT_TYPES.LENGTH },
              { id: 'inradius', label: 'Inscribed Sphere Radius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumscribed Sphere Radius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Volume', formula: 'V = a^3', substitution: `V = ${a}^3 = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 6a^2', substitution: `A = 6 \\cdot ${a}^2 = ${totalSurfaceArea}` },
              { title: 'Space Diagonal', formula: 'd_s = a\\sqrt{3}', substitution: `d_s = ${a}\\sqrt{3} = ${spaceDiagonal}` }
            ],
            threeData: { type: 'cube', size: a }
          };
        }
      },
      {
        id: 'by_volume',
        name: 'By Volume (V)',
        description: 'Provide volume V.',
        inputs: [
          { id: 'V', label: 'Volume (V)', symbol: 'V', type: UNIT_TYPES.VOLUME, default: 64, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { V } = vals;
          const a = Math.cbrt(V);
          const totalSurfaceArea = 6 * a * a;
          const spaceDiagonal = a * Math.sqrt(3);

          return {
            outputs: [
              { id: 'side', label: 'Side Length (a)', symbol: 'a', value: a, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'space_diagonal', label: 'Space Diagonal (d_s)', symbol: 'd_s', value: spaceDiagonal, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Side from Volume', formula: 'a = \\sqrt[3]{V}', substitution: `a = \\sqrt[3]{${V}} = ${a}` }
            ],
            threeData: { type: 'cube', size: a }
          };
        }
      }
    ]
  },

  // 4. CUBOID (RECTANGULAR PRISM)
  {
    id: 'cuboid',
    name: 'Cuboid (Rectangular Prism)',
    category: '3d',
    description: 'Convex polyhedron bounded by six rectangular faces.',
    icon: 'cuboid',
    methods: [
      {
        id: 'dimensions',
        name: 'Length, Width & Height (l, w, h)',
        description: 'Provide length l, width w, and height h.',
        inputs: [
          { id: 'l', label: 'Length (l)', symbol: 'l', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'w', label: 'Width (w)', symbol: 'w', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'h', label: 'Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { l, w, h } = vals;
          const volume = l * w * h;
          const totalSurfaceArea = 2 * (l * w + l * h + w * h);
          const lateralSurfaceArea = 2 * h * (l + w);
          const baseArea = l * w;
          const basePerimeter = 2 * (l + w);
          const spaceDiagonal = Math.sqrt(l * l + w * w + h * h);
          const d_lw = Math.sqrt(l * l + w * w);
          const d_lh = Math.sqrt(l * l + h * h);
          const d_wh = Math.sqrt(w * w + h * h);

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'space_diagonal', label: 'Space Diagonal (d)', symbol: 'd', value: spaceDiagonal, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'base_area', label: 'Base Face Area (l \\times w)', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA },
              { id: 'base_perimeter', label: 'Base Perimeter', symbol: 'P_b', value: basePerimeter, type: UNIT_TYPES.LENGTH },
              { id: 'd_lw', label: 'Base Diagonal', symbol: 'd_base', value: d_lw, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Cuboid Volume', formula: 'V = l \\cdot w \\cdot h', substitution: `V = ${l} \\cdot ${w} \\cdot ${h} = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 2(lw + lh + wh)', substitution: `A = 2(${l}\\cdot${w} + ${l}\\cdot${h} + ${w}\\cdot${h}) = ${totalSurfaceArea}` },
              { title: 'Space Diagonal', formula: 'd = \\sqrt{l^2 + w^2 + h^2}', substitution: `d = \\sqrt{${l}^2 + ${w}^2 + ${h}^2} = ${spaceDiagonal}` }
            ],
            threeData: { type: 'cuboid', width: l, height: h, depth: w }
          };
        }
      }
    ]
  },

  // 5. CYLINDER
  {
    id: 'cylinder',
    name: 'Cylinder',
    category: '3d',
    description: 'Curvilinear geometric shape with straight parallel sides and two circular bases.',
    icon: 'cylinder',
    methods: [
      {
        id: 'radius_height',
        name: 'Radius & Height (r, h)',
        description: 'Provide radius r and height h.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'h', label: 'Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r, h } = vals;
          const diameter = 2 * r;
          const baseArea = Math.PI * r * r;
          const bothBasesArea = 2 * baseArea;
          const lateralSurfaceArea = 2 * Math.PI * r * h;
          const totalSurfaceArea = lateralSurfaceArea + bothBasesArea;
          const volume = baseArea * h;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A_total)', symbol: 'A_total', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'base_area', label: 'Single Base Area (A_base)', symbol: 'A_base', value: baseArea, type: UNIT_TYPES.AREA },
              { id: 'diameter', label: 'Diameter (d)', symbol: 'd', value: diameter, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\pi r^2 h', substitution: `V = \\pi \\cdot ${r}^2 \\cdot ${h} = ${volume}` },
              { title: 'Lateral Surface Area', formula: 'A_{lat} = 2\\pi r h', substitution: `A_{lat} = 2\\pi \\cdot ${r} \\cdot ${h} = ${lateralSurfaceArea}` },
              { title: 'Total Surface Area', formula: 'A = 2\\pi r(r + h)', substitution: `A = 2\\pi \\cdot ${r}(${r} + ${h}) = ${totalSurfaceArea}` }
            ],
            threeData: { type: 'cylinder', radius: r, height: h }
          };
        }
      }
    ]
  },

  // 6. CONE
  {
    id: 'cone',
    name: 'Cone',
    category: '3d',
    description: 'Three-dimensional geometric shape that tapers smoothly from a flat circular base to a point called the apex.',
    icon: 'cone',
    methods: [
      {
        id: 'radius_height',
        name: 'Radius & Vertical Height (r, h)',
        description: 'Provide radius r and height h.',
        inputs: [
          { id: 'r', label: 'Base Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'h', label: 'Vertical Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 7, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r, h } = vals;
          const slantHeight = Math.sqrt(r * r + h * h);
          const baseArea = Math.PI * r * r;
          const lateralSurfaceArea = Math.PI * r * slantHeight;
          const totalSurfaceArea = baseArea + lateralSurfaceArea;
          const volume = (1 / 3) * Math.PI * r * r * h;
          const vertexAngle = 2 * Math.atan2(r, h);

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A_total)', symbol: 'A_total', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'slant_height', label: 'Slant Height (l)', symbol: 'l', value: slantHeight, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'base_area', label: 'Base Area (A_base)', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA },
              { id: 'vertex_angle', label: 'Apex Full Angle', symbol: '2θ', value: vertexAngle, type: UNIT_TYPES.ANGLE }
            ],
            steps: [
              { title: 'Slant Height (Pythagoras)', formula: 'l = \\sqrt{r^2 + h^2}', substitution: `l = \\sqrt{${r}^2 + ${h}^2} = ${slantHeight}` },
              { title: 'Volume', formula: 'V = \\frac{1}{3}\\pi r^2 h', substitution: `V = (1/3) \\cdot \\pi \\cdot ${r}^2 \\cdot ${h} = ${volume}` },
              { title: 'Lateral Surface Area', formula: 'A_{lat} = \\pi r l', substitution: `A_{lat} = \\pi \\cdot ${r} \\cdot ${slantHeight} = ${lateralSurfaceArea}` },
              { title: 'Total Surface Area', formula: 'A = \\pi r(r + l)', substitution: `A = \\pi \\cdot ${r}(${r} + ${slantHeight}) = ${totalSurfaceArea}` }
            ],
            threeData: { type: 'cone', radius: r, height: h }
          };
        }
      },
      {
        id: 'radius_slant',
        name: 'Radius & Slant Height (r, l)',
        description: 'Provide radius r and slant height l.',
        inputs: [
          { id: 'r', label: 'Base Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 },
          { id: 'l', label: 'Slant Height (l)', symbol: 'l', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r, l } = vals;
          if (l <= r) throw new Error('Slant height (l) must be strictly greater than base radius (r).');
          const h = Math.sqrt(l * l - r * r);
          const volume = (1 / 3) * Math.PI * r * r * h;
          const lateralSurfaceArea = Math.PI * r * l;
          const totalSurfaceArea = Math.PI * r * (r + l);

          return {
            outputs: [
              { id: 'h', label: 'Vertical Height (h)', symbol: 'h', value: h, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true }
            ],
            steps: [
              { title: 'Vertical Height', formula: 'h = \\sqrt{l^2 - r^2}', substitution: `h = \\sqrt{${l}^2 - ${r}^2} = ${h}` }
            ],
            threeData: { type: 'cone', radius: r, height: h }
          };
        }
      }
    ]
  },

  // 7. FRUSTUM OF A CONE (TRUNCATED CONE)
  {
    id: 'frustum',
    name: 'Frustum of Cone (Truncated Cone)',
    category: '3d',
    description: 'Portion of a cone that lies between two parallel planes cutting it.',
    icon: 'frustum',
    methods: [
      {
        id: 'radii_height',
        name: 'Top Radius, Bottom Radius & Height (r, R, h)',
        description: 'Provide top radius r, bottom radius R, and height h.',
        inputs: [
          { id: 'R', label: 'Bottom Radius (R)', symbol: 'R', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'r', label: 'Top Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 2.5, min: 0.0001 },
          { id: 'h', label: 'Vertical Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { R, r, h } = vals;
          const slantHeight = Math.sqrt(h * h + Math.pow(R - r, 2));
          const topArea = Math.PI * r * r;
          const bottomArea = Math.PI * R * R;
          const lateralSurfaceArea = Math.PI * (R + r) * slantHeight;
          const totalSurfaceArea = topArea + bottomArea + lateralSurfaceArea;
          const volume = (1 / 3) * Math.PI * h * (R * R + R * r + r * r);

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'slant_height', label: 'Slant Height (l)', symbol: 'l', value: slantHeight, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'bottom_area', label: 'Bottom Base Area (A_bottom)', symbol: 'A_b', value: bottomArea, type: UNIT_TYPES.AREA },
              { id: 'top_area', label: 'Top Base Area (A_top)', symbol: 'A_t', value: topArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Slant Height', formula: 'l = \\sqrt{h^2 + (R - r)^2}', substitution: `l = \\sqrt{${h}^2 + (${R} - ${r})^2} = ${slantHeight}` },
              { title: 'Volume', formula: 'V = \\frac{1}{3}\\pi h (R^2 + Rr + r^2)', substitution: `V = (1/3)\\pi(${h})(${R}^2 + ${R}\\cdot${r} + ${r}^2) = ${volume}` },
              { title: 'Lateral Surface Area', formula: 'A_{lat} = \\pi (R + r) l', substitution: `A_{lat} = \\pi(${R} + ${r})(${slantHeight}) = ${lateralSurfaceArea}` }
            ],
            threeData: { type: 'frustum', radiusBottom: R, radiusTop: r, height: h }
          };
        }
      }
    ]
  },

  // 8. REGULAR PYRAMID (SQUARE PYRAMID)
  {
    id: 'pyramid',
    name: 'Square Pyramid',
    category: '3d',
    description: 'Polyhedron formed by connecting a square base and an apex.',
    icon: 'pyramid',
    methods: [
      {
        id: 'base_height',
        name: 'Base Side & Vertical Height (a, h)',
        description: 'Provide base edge length a and vertical height h.',
        inputs: [
          { id: 'a', label: 'Base Side (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'h', label: 'Vertical Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, h } = vals;
          const slantHeight = Math.sqrt(h * h + Math.pow(a / 2, 2));
          const lateralEdge = Math.sqrt(h * h + Math.pow(a, 2) / 2);
          const baseArea = a * a;
          const basePerimeter = 4 * a;
          const lateralSurfaceArea = 2 * a * slantHeight;
          const totalSurfaceArea = baseArea + lateralSurfaceArea;
          const volume = (1 / 3) * baseArea * h;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'slant_height', label: 'Face Slant Height (s)', symbol: 's', value: slantHeight, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'lateral_edge', label: 'Lateral Edge Length (e)', symbol: 'e', value: lateralEdge, type: UNIT_TYPES.LENGTH },
              { id: 'base_area', label: 'Base Area (A_base)', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Slant Height', formula: 's = \\sqrt{h^2 + (a/2)^2}', substitution: `s = \\sqrt{${h}^2 + (${a}/2)^2} = ${slantHeight}` },
              { title: 'Volume', formula: 'V = \\frac{1}{3} a^2 h', substitution: `V = (1/3) \\cdot ${a}^2 \\cdot ${h} = ${volume}` },
              { title: 'Lateral Surface Area', formula: 'A_{lat} = 2 a s', substitution: `A_{lat} = 2 \\cdot ${a} \\cdot ${slantHeight} = ${lateralSurfaceArea}` }
            ],
            threeData: { type: 'pyramid', baseSize: a, height: h }
          };
        }
      }
    ]
  },

  // 9. TRIANGULAR PRISM
  {
    id: 'triangular_prism',
    name: 'Triangular Prism',
    category: '3d',
    description: 'Three-sided prism made of two parallel triangular bases and three rectangular sides.',
    icon: 'prism',
    methods: [
      {
        id: 'equilateral_length',
        name: 'Equilateral Base Side & Prism Length (a, L)',
        description: 'Provide equilateral triangle base side a and length L.',
        inputs: [
          { id: 'a', label: 'Base Triangle Side (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'L', label: 'Prism Length / Height (L)', symbol: 'L', type: UNIT_TYPES.LENGTH, default: 10, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, L } = vals;
          const baseArea = (Math.sqrt(3) / 4) * a * a;
          const basePerimeter = 3 * a;
          const lateralSurfaceArea = basePerimeter * L;
          const totalSurfaceArea = 2 * baseArea + lateralSurfaceArea;
          const volume = baseArea * L;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'total_surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'lateral_surface_area', label: 'Lateral Surface Area (A_lat)', symbol: 'A_lat', value: lateralSurfaceArea, type: UNIT_TYPES.AREA },
              { id: 'base_area', label: 'Triangle Base Area (A_b)', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Base Area', formula: 'A_b = \\frac{\\sqrt{3}}{4} a^2', substitution: `A_b = (\\sqrt{3}/4) \\cdot ${a}^2 = ${baseArea}` },
              { title: 'Volume', formula: 'V = A_b \\cdot L', substitution: `V = ${baseArea} \\cdot ${L} = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 2 A_b + 3 a L', substitution: `A = 2(${baseArea}) + 3(${a})(${L}) = ${totalSurfaceArea}` }
            ],
            threeData: { type: 'prism', baseA: a, length: L }
          };
        }
      }
    ]
  },

  // 10. TORUS (DONUT)
  {
    id: 'torus',
    name: 'Torus (Donut)',
    category: '3d',
    description: 'Surface of revolution generated by revolving a circle in three-dimensional space about an axis coplanar with the circle.',
    icon: 'torus',
    methods: [
      {
        id: 'radii',
        name: 'Major & Minor / Tube Radii (R, r)',
        description: 'Provide major radius R (center of hole to center of tube) and tube radius r.',
        inputs: [
          { id: 'R', label: 'Major Radius (R)', symbol: 'R', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'r', label: 'Tube Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 2.5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { R, r } = vals;
          if (r >= R) throw new Error('Tube radius (r) must be strictly less than major radius (R) to form a standard ring torus.');
          const surfaceArea = 4 * Math.PI * Math.PI * R * r;
          const volume = 2 * Math.PI * Math.PI * R * r * r;
          const innerDiameter = 2 * (R - r);
          const outerDiameter = 2 * (R + r);
          const aspectRatio = R / r;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Surface Area (A)', symbol: 'A', value: surfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'outer_diameter', label: 'Overall Outer Diameter (D_out)', symbol: 'D_out', value: outerDiameter, type: UNIT_TYPES.LENGTH },
              { id: 'inner_diameter', label: 'Central Hole Diameter (D_hole)', symbol: 'D_hole', value: innerDiameter, type: UNIT_TYPES.LENGTH },
              { id: 'aspect_ratio', label: 'Aspect Ratio (R / r)', symbol: 'R/r', textValue: aspectRatio.toFixed(2) }
            ],
            steps: [
              { title: 'Torus Volume', formula: 'V = 2\\pi^2 R r^2', substitution: `V = 2\\pi^2(${R})(${r}^2) = ${volume}` },
              { title: 'Torus Surface Area', formula: 'A = 4\\pi^2 R r', substitution: `A = 4\\pi^2(${R})(${r}) = ${surfaceArea}` }
            ],
            threeData: { type: 'torus', radius: R, tube: r }
          };
        }
      }
    ]
  },

  // 11. ELLIPSOID
  {
    id: 'ellipsoid',
    name: 'Ellipsoid',
    category: '3d',
    description: 'Quadratic surface that is a three-dimensional analogue of an ellipse with semi-axes a, b, c.',
    icon: 'ellipsoid',
    methods: [
      {
        id: 'semi_axes',
        name: 'Three Semi-Axes (a, b, c)',
        description: 'Provide principal semi-axes a, b, and c.',
        inputs: [
          { id: 'a', label: 'Semi-Axis a', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'b', label: 'Semi-Axis b', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'c', label: 'Semi-Axis c', symbol: 'c', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, b, c } = vals;
          const volume = (4 / 3) * Math.PI * a * b * c;
          // Knudsen's formula for ellipsoid surface area: p ≈ 1.6075
          const p = 1.6075;
          const ap = Math.pow(a, p);
          const bp = Math.pow(b, p);
          const cp = Math.pow(c, p);
          const surfaceArea = 4 * Math.PI * Math.pow((ap * bp + ap * cp + bp * cp) / 3, 1 / p);

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Surface Area (A, Knudsen)', symbol: 'A', value: surfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'cross_ab', label: 'Cross-section Area (xy)', symbol: 'A_xy', value: Math.PI * a * b, type: UNIT_TYPES.AREA },
              { id: 'cross_ac', label: 'Cross-section Area (xz)', symbol: 'A_xz', value: Math.PI * a * c, type: UNIT_TYPES.AREA },
              { id: 'cross_bc', label: 'Cross-section Area (yz)', symbol: 'A_yz', value: Math.PI * b * c, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{4}{3}\\pi a b c', substitution: `V = (4/3)\\pi(${a})(${b})(${c}) = ${volume}` },
              { title: 'Knudsen Surface Area (p = 1.6075)', formula: 'A \\approx 4\\pi \\left(\\frac{a^p b^p + a^p c^p + b^p c^p}{3}\\right)^{1/p}', substitution: `A \\approx ${surfaceArea}` }
            ],
            threeData: { type: 'ellipsoid', a, b, c }
          };
        }
      }
    ]
  },

  // 12. CAPSULE
  {
    id: 'capsule',
    name: 'Capsule (Stadium of Revolution)',
    category: '3d',
    description: 'Basic 3D geometric shape consisting of a cylinder with hemispherical ends.',
    icon: 'capsule',
    methods: [
      {
        id: 'radius_cylinder_length',
        name: 'Radius & Cylinder Length (r, L)',
        description: 'Provide cap radius r and cylindrical section length L.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 },
          { id: 'L', label: 'Cylinder Length (L)', symbol: 'L', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r, L } = vals;
          const totalLength = L + 2 * r;
          const cylinderVolume = Math.PI * r * r * L;
          const spheresVolume = (4 / 3) * Math.PI * Math.pow(r, 3);
          const totalVolume = cylinderVolume + spheresVolume;

          const cylinderSurface = 2 * Math.PI * r * L;
          const spheresSurface = 4 * Math.PI * r * r;
          const totalSurfaceArea = cylinderSurface + spheresSurface;

          return {
            outputs: [
              { id: 'volume', label: 'Total Volume (V)', symbol: 'V', value: totalVolume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalSurfaceArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'total_length', label: 'Overall Tip-to-Tip Length', symbol: 'L_total', value: totalLength, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diameter', label: 'Diameter (d)', symbol: 'd', value: 2 * r, type: UNIT_TYPES.LENGTH },
              { id: 'cyl_volume', label: 'Cylinder Section Volume', symbol: 'V_cyl', value: cylinderVolume, type: UNIT_TYPES.VOLUME }
            ],
            steps: [
              { title: 'Total Length', formula: 'L_{total} = L + 2r', substitution: `L_{total} = ${L} + 2(${r}) = ${totalLength}` },
              { title: 'Volume', formula: 'V = \\pi r^2 L + \\frac{4}{3}\\pi r^3', substitution: `V = \\pi(${r}^2)(${L}) + (4/3)\\pi(${r}^3) = ${totalVolume}` },
              { title: 'Surface Area', formula: 'A = 2\\pi r L + 4\\pi r^2', substitution: `A = 2\\pi(${r})(${L}) + 4\\pi(${r}^2) = ${totalSurfaceArea}` }
            ],
            threeData: { type: 'capsule', radius: r, length: L }
          };
        }
      }
    ]
  },

  // 13. SPHERICAL CAP (DOME)
  {
    id: 'spherical_cap',
    name: 'Spherical Cap (Dome)',
    category: '3d',
    description: 'Portion of a sphere cut off by a planar slice, forming a dome with a circular base.',
    icon: 'spherical_cap',
    methods: [
      {
        id: 'by_sphere_radius_and_height',
        name: 'By Sphere Radius & Cap Height (R, h)',
        description: 'Provide sphere radius R and cap height h.',
        inputs: [
          { id: 'R', label: 'Sphere Radius (R)', symbol: 'R', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'h', label: 'Cap Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { R, h } = vals;
          if (h > 2 * R) {
            throw new Error('Cap height h cannot exceed the sphere diameter (2R)');
          }
          const a = Math.sqrt(Math.max(0, h * (2 * R - h)));
          const curvedArea = 2 * Math.PI * R * h;
          const baseArea = Math.PI * a * a;
          const totalArea = curvedArea + baseArea;
          const volume = (Math.PI * h * h * (3 * R - h)) / 3;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'curved_area', label: 'Curved Dome Surface Area', symbol: 'A_c', value: curvedArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'base_radius', label: 'Base Circle Radius (a)', symbol: 'a', value: a, type: UNIT_TYPES.LENGTH },
              { id: 'base_area', label: 'Base Circle Area', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Base Circle Radius', formula: 'a = \\sqrt{h(2R - h)}', substitution: `a = \\sqrt{${h}(2(${R}) - ${h})} = ${a}` },
              { title: 'Curved Dome Area', formula: 'A_{curved} = 2\\pi R h', substitution: `A_{curved} = 2\\pi(${R})(${h}) = ${curvedArea}` },
              { title: 'Cap Volume', formula: 'V = \\frac{\\pi h^2}{3}(3R - h)', substitution: `V = \\frac{\\pi(${h}^2)}{3}(3(${R}) - ${h}) = ${volume}` }
            ],
            threeData: { type: 'spherical_cap', radius: R, height: h }
          };
        }
      },
      {
        id: 'by_base_radius_and_height',
        name: 'By Base Radius & Cap Height (a, h)',
        description: 'Provide base circle radius a and cap height h.',
        inputs: [
          { id: 'a', label: 'Base Radius (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'h', label: 'Cap Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, h } = vals;
          const R = (a * a + h * h) / (2 * h);
          const curvedArea = Math.PI * (a * a + h * h);
          const baseArea = Math.PI * a * a;
          const totalArea = curvedArea + baseArea;
          const volume = (Math.PI * h * (3 * a * a + h * h)) / 6;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'sphere_radius', label: 'Parent Sphere Radius (R)', symbol: 'R', value: R, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'curved_area', label: 'Curved Dome Surface Area', symbol: 'A_c', value: curvedArea, type: UNIT_TYPES.AREA },
              { id: 'base_area', label: 'Base Circle Area', symbol: 'A_b', value: baseArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Sphere Radius', formula: 'R = \\frac{a^2 + h^2}{2h}', substitution: `R = \\frac{${a}^2 + ${h}^2}{2(${h})} = ${R}` },
              { title: 'Cap Volume', formula: 'V = \\frac{\\pi h}{6}(3a^2 + h^2)', substitution: `V = \\frac{\\pi(${h})}{6}(3(${a}^2) + ${h}^2) = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = \\pi(2a^2 + h^2)', substitution: `A = \\pi(2(${a}^2) + ${h}^2) = ${totalArea}` }
            ],
            threeData: { type: 'spherical_cap', radius: R, height: h }
          };
        }
      }
    ]
  },

  // 14. HOLLOW CYLINDER (PIPE)
  {
    id: 'hollow_cylinder',
    name: 'Hollow Cylinder (Pipe / Tube)',
    category: '3d',
    description: 'Circular cylindrical tube or pipe with an outer wall and inner concentric cylindrical bore.',
    icon: 'hollow_cylinder',
    methods: [
      {
        id: 'by_radii_and_height',
        name: 'By Outer & Inner Radii, Height (R, r, h)',
        description: 'Provide outer radius R, inner radius r, and height h.',
        inputs: [
          { id: 'R', label: 'Outer Radius (R)', symbol: 'R', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'r', label: 'Inner Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 },
          { id: 'h', label: 'Height / Length (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 10, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { R, r, h } = vals;
          if (r >= R) {
            throw new Error('Inner radius (r) must be strictly less than outer radius (R)');
          }
          const thickness = R - r;
          const outerLateral = 2 * Math.PI * R * h;
          const innerLateral = 2 * Math.PI * r * h;
          const ringAreaEach = Math.PI * (R * R - r * r);
          const totalArea = outerLateral + innerLateral + 2 * ringAreaEach;
          const volume = Math.PI * h * (R * R - r * r);

          return {
            outputs: [
              { id: 'volume', label: 'Material Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'wall_thickness', label: 'Wall Thickness (t)', symbol: 't', value: thickness, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'outer_lateral', label: 'Outer Lateral Surface Area', symbol: 'A_{out}', value: outerLateral, type: UNIT_TYPES.AREA },
              { id: 'inner_lateral', label: 'Inner Lateral Surface Area', symbol: 'A_{in}', value: innerLateral, type: UNIT_TYPES.AREA },
              { id: 'bore_capacity', label: 'Inner Bore Capacity', symbol: 'V_{bore}', value: Math.PI * r * r * h, type: UNIT_TYPES.VOLUME }
            ],
            steps: [
              { title: 'Wall Thickness', formula: 't = R - r', substitution: `t = ${R} - ${r} = ${thickness}` },
              { title: 'Material Volume', formula: 'V = \\pi h(R^2 - r^2)', substitution: `V = \\pi(${h})(${R}^2 - ${r}^2) = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 2\\pi(R + r)h + 2\\pi(R^2 - r^2)', substitution: `A = 2\\pi(${R} + ${r})(${h}) + 2\\pi(${R}^2 - ${r}^2) = ${totalArea}` }
            ],
            threeData: { type: 'hollow_cylinder', outerRadius: R, innerRadius: r, height: h }
          };
        }
      }
    ]
  },

  // 15. REGULAR TETRAHEDRON
  {
    id: 'tetrahedron',
    name: 'Regular Tetrahedron',
    category: '3d',
    description: 'Platonic solid with four congruent equilateral triangular faces, four vertices, and six edges.',
    icon: 'tetrahedron',
    methods: [
      {
        id: 'by_edge',
        name: 'By Edge Length (a)',
        description: 'Provide edge length a.',
        inputs: [
          { id: 'a', label: 'Edge Length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const faceArea = (Math.sqrt(3) / 4) * a * a;
          const totalArea = Math.sqrt(3) * a * a;
          const volume = (Math.pow(a, 3)) / (6 * Math.SQRT2);
          const height = Math.sqrt(2 / 3) * a;
          const circumradius = (Math.sqrt(6) / 4) * a;
          const inradius = a / Math.sqrt(24);

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'height', label: 'Vertical Height (H)', symbol: 'H', value: height, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'face_area', label: 'Single Face Area', symbol: 'A_f', value: faceArea, type: UNIT_TYPES.AREA },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{a^3}{6\\sqrt{2}}', substitution: `V = \\frac{${a}^3}{6\\sqrt{2}} = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = \\sqrt{3} a^2', substitution: `A = \\sqrt{3}(${a}^2) = ${totalArea}` },
              { title: 'Vertical Height', formula: 'H = \\sqrt{2/3} a', substitution: `H = \\sqrt{2/3}(${a}) = ${height}` }
            ],
            threeData: { type: 'tetrahedron', edge: a }
          };
        }
      }
    ]
  },

  // 16. REGULAR OCTAHEDRON
  {
    id: 'octahedron',
    name: 'Regular Octahedron',
    category: '3d',
    description: 'Platonic solid with eight congruent equilateral triangular faces, six vertices, and twelve edges.',
    icon: 'octahedron',
    methods: [
      {
        id: 'by_edge',
        name: 'By Edge Length (a)',
        description: 'Provide edge length a.',
        inputs: [
          { id: 'a', label: 'Edge Length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const faceArea = (Math.sqrt(3) / 4) * a * a;
          const totalArea = 2 * Math.sqrt(3) * a * a;
          const volume = (Math.SQRT2 / 3) * Math.pow(a, 3);
          const circumradius = a / Math.SQRT2;
          const inradius = a / Math.sqrt(6);
          const midradius = a / 2;

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'midradius', label: 'Midradius (ρ)', symbol: 'ρ', value: midradius, type: UNIT_TYPES.LENGTH },
              { id: 'face_area', label: 'Single Face Area', symbol: 'A_f', value: faceArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{\\sqrt{2}}{3} a^3', substitution: `V = \\frac{\\sqrt{2}}{3}(${a}^3) = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 2\\sqrt{3} a^2', substitution: `A = 2\\sqrt{3}(${a}^2) = ${totalArea}` },
              { title: 'Circumradius', formula: 'R = \\frac{a}{\\sqrt{2}}', substitution: `R = \\frac{${a}}{\\sqrt{2}} = ${circumradius}` }
            ],
            threeData: { type: 'octahedron', edge: a }
          };
        }
      }
    ]
  },

  // 17. REGULAR ICOSAHEDRON
  {
    id: 'icosahedron',
    name: 'Regular Icosahedron',
    category: '3d',
    description: 'Platonic solid with twenty equilateral triangular faces, twelve vertices, and thirty edges.',
    icon: 'icosahedron',
    methods: [
      {
        id: 'by_edge',
        name: 'By Edge Length (a)',
        description: 'Provide edge length a.',
        inputs: [
          { id: 'a', label: 'Edge Length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const faceArea = (Math.sqrt(3) / 4) * a * a;
          const totalArea = 5 * Math.sqrt(3) * a * a;
          const volume = ((5 * (3 + Math.sqrt(5))) / 12) * Math.pow(a, 3);
          const circumradius = (a / 4) * Math.sqrt(10 + 2 * Math.sqrt(5));
          const inradius = (a / 12) * Math.sqrt(3) * (3 + Math.sqrt(5));
          const midradius = (a / 4) * (1 + Math.sqrt(5));

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'midradius', label: 'Midradius (ρ)', symbol: 'ρ', value: midradius, type: UNIT_TYPES.LENGTH },
              { id: 'face_area', label: 'Single Face Area', symbol: 'A_f', value: faceArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{5(3 + \\sqrt{5})}{12} a^3', substitution: `V = \\frac{5(3 + \\sqrt{5})}{12}(${a}^3) = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 5\\sqrt{3} a^2', substitution: `A = 5\\sqrt{3}(${a}^2) = ${totalArea}` },
              { title: 'Circumradius', formula: 'R = \\frac{a}{4}\\sqrt{10 + 2\\sqrt{5}}', substitution: `R = \\frac{${a}}{4}\\sqrt{10 + 2\\sqrt{5}} = ${circumradius}` }
            ],
            threeData: { type: 'icosahedron', edge: a }
          };
        }
      }
    ]
  },

  // 18. REGULAR DODECAHEDRON
  {
    id: 'dodecahedron',
    name: 'Regular Dodecahedron',
    category: '3d',
    description: 'Platonic solid with twelve congruent regular pentagonal faces, twenty vertices, and thirty edges.',
    icon: 'dodecahedron',
    methods: [
      {
        id: 'by_edge',
        name: 'By Edge Length (a)',
        description: 'Provide edge length a.',
        inputs: [
          { id: 'a', label: 'Edge Length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const pentagonConst = Math.sqrt(25 + 10 * Math.sqrt(5));
          const faceArea = (1 / 4) * pentagonConst * a * a;
          const totalArea = 3 * pentagonConst * a * a;
          const volume = ((15 + 7 * Math.sqrt(5)) / 4) * Math.pow(a, 3);
          const circumradius = (a / 4) * (Math.sqrt(3) + Math.sqrt(15));
          const inradius = (a / 20) * Math.sqrt(250 + 110 * Math.sqrt(5));
          const midradius = (a / 4) * (3 + Math.sqrt(5));

          return {
            outputs: [
              { id: 'volume', label: 'Volume (V)', symbol: 'V', value: volume, type: UNIT_TYPES.VOLUME, isPrimary: true },
              { id: 'surface_area', label: 'Total Surface Area (A)', symbol: 'A', value: totalArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'midradius', label: 'Midradius (ρ)', symbol: 'ρ', value: midradius, type: UNIT_TYPES.LENGTH },
              { id: 'face_area', label: 'Single Pentagonal Face Area', symbol: 'A_f', value: faceArea, type: UNIT_TYPES.AREA }
            ],
            steps: [
              { title: 'Volume', formula: 'V = \\frac{15 + 7\\sqrt{5}}{4} a^3', substitution: `V = \\frac{15 + 7\\sqrt{5}}{4}(${a}^3) = ${volume}` },
              { title: 'Total Surface Area', formula: 'A = 3\\sqrt{25 + 10\\sqrt{5}} a^2', substitution: `A = 3\\sqrt{25 + 10\\sqrt{5}}(${a}^2) = ${totalArea}` },
              { title: 'Circumradius', formula: 'R = \\frac{a}{4}(\\sqrt{3} + \\sqrt{15})', substitution: `R = \\frac{${a}}{4}(\\sqrt{3} + \\sqrt{15}) = ${circumradius}` }
            ],
            threeData: { type: 'dodecahedron', edge: a }
          };
        }
      }
    ]
  }
];
