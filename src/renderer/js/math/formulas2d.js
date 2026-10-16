/**
 * GeoFluent 2D Geometry Engine
 * Comprehensive formulas, derivation steps, parameter extraction, and coordinates for rendering.
 */

import { UNIT_TYPES } from './units.js';

export const SHAPES_2D = [
  // 1. TRIANGLE
  {
    id: 'triangle',
    name: 'Triangle',
    category: '2d',
    description: 'Three-sided polygon with rich geometric properties, altitudes, medians, and inscribed/circumscribed circles.',
    icon: 'triangle',
    methods: [
      {
        id: 'heron',
        name: 'Three Sides (Heron\'s Formula)',
        description: 'Provide all three side lengths a, b, and c.',
        inputs: [
          { id: 'a', label: 'Side a', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 },
          { id: 'b', label: 'Side b', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 },
          { id: 'c', label: 'Side c', symbol: 'c', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, b, c } = vals;
          if (a + b <= c || a + c <= b || b + c <= a) {
            throw new Error('Triangle Inequality Violated: The sum of any two sides must be strictly greater than the third side.');
          }
          const s = (a + b + c) / 2;
          const area = Math.sqrt(Math.max(0, s * (s - a) * (s - b) * (s - c)));
          const perimeter = a + b + c;

          // Law of cosines for angles in radians
          const alpha = Math.acos(Math.max(-1, Math.min(1, (b * b + c * c - a * a) / (2 * b * c))));
          const beta = Math.acos(Math.max(-1, Math.min(1, (a * a + c * c - b * b) / (2 * a * c))));
          const gamma = Math.PI - alpha - beta;

          // Altitudes: h = 2A / side
          const ha = (2 * area) / a;
          const hb = (2 * area) / b;
          const hc = (2 * area) / c;

          // Medians: m_a = 0.5 * sqrt(2b^2 + 2c^2 - a^2)
          const ma = 0.5 * Math.sqrt(Math.max(0, 2 * b * b + 2 * c * c - a * a));
          const mb = 0.5 * Math.sqrt(Math.max(0, 2 * a * a + 2 * c * c - b * b));
          const mc = 0.5 * Math.sqrt(Math.max(0, 2 * a * a + 2 * b * b - c * c));

          // Radii
          const inradius = area / s;
          const circumradius = (a * b * c) / (4 * area);

          // Classification
          const sidesSorted = [a, b, c].sort((x, y) => x - y);
          let sideType = 'Scalene';
          if (Math.abs(a - b) < 1e-9 && Math.abs(b - c) < 1e-9) sideType = 'Equilateral';
          else if (Math.abs(a - b) < 1e-9 || Math.abs(b - c) < 1e-9 || Math.abs(a - c) < 1e-9) sideType = 'Isosceles';

          const pythDiff = sidesSorted[0] ** 2 + sidesSorted[1] ** 2 - sidesSorted[2] ** 2;
          let angleType = 'Acute';
          if (Math.abs(pythDiff) < 1e-7 * sidesSorted[2] ** 2) angleType = 'Right-angled';
          else if (pythDiff < 0) angleType = 'Obtuse';

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'semi_perimeter', label: 'Semi-perimeter (s)', symbol: 's', value: s, type: UNIT_TYPES.LENGTH },
              { id: 'angle_alpha', label: 'Angle α (opposite a)', symbol: 'α', value: alpha, type: UNIT_TYPES.ANGLE },
              { id: 'angle_beta', label: 'Angle β (opposite b)', symbol: 'β', value: beta, type: UNIT_TYPES.ANGLE },
              { id: 'angle_gamma', label: 'Angle γ (opposite c)', symbol: 'γ', value: gamma, type: UNIT_TYPES.ANGLE },
              { id: 'ha', label: 'Altitude to a (hₐ)', symbol: 'hₐ', value: ha, type: UNIT_TYPES.LENGTH },
              { id: 'hb', label: 'Altitude to b (hb)', symbol: 'hb', value: hb, type: UNIT_TYPES.LENGTH },
              { id: 'hc', label: 'Altitude to c (hc)', symbol: 'hc', value: hc, type: UNIT_TYPES.LENGTH },
              { id: 'ma', label: 'Median to a (mₐ)', symbol: 'mₐ', value: ma, type: UNIT_TYPES.LENGTH },
              { id: 'mb', label: 'Median to b (mb)', symbol: 'mb', value: mb, type: UNIT_TYPES.LENGTH },
              { id: 'mc', label: 'Median to c (mc)', symbol: 'mc', value: mc, type: UNIT_TYPES.LENGTH },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH },
              { id: 'classification', label: 'Classification', symbol: 'Type', textValue: `${sideType}, ${angleType}` }
            ],
            steps: [
              { title: 'Semi-perimeter calculation', formula: 's = (a + b + c) / 2', substitution: `s = (${a} + ${b} + ${c}) / 2 = ${s}` },
              { title: 'Heron\'s Area Formula', formula: 'A = \\sqrt{s(s - a)(s - b)(s - c)}', substitution: `A = \\sqrt{${s}(${s - a})(${s - b})(${s - c})} = ${area}` },
              { title: 'Inradius', formula: 'r = A / s', substitution: `r = ${area} / ${s} = ${inradius}` },
              { title: 'Circumradius', formula: 'R = (a \\cdot b \\cdot c) / (4A)', substitution: `R = (${a} \\cdot ${b} \\cdot ${c}) / (4 \\cdot ${area}) = ${circumradius}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0, label: 'A' },
                { x: c, y: 0, label: 'B' },
                { x: b * Math.cos(alpha), y: b * Math.sin(alpha), label: 'C' }
              ],
              inradius,
              circumradius
            }
          };
        }
      },
      {
        id: 'base_height',
        name: 'Base & Height',
        description: 'Provide base length b and vertical height h.',
        inputs: [
          { id: 'b', label: 'Base (b)', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'h', label: 'Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { b, h } = vals;
          const area = 0.5 * b * h;
          // Assume symmetric isosceles for default visualization
          const leg = Math.sqrt((b / 2) ** 2 + h ** 2);
          const perimeter = b + 2 * leg;
          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P, Isosceles)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'leg', label: 'Side Leg (Isosceles)', symbol: 'a = c', value: leg, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Triangle Area', formula: 'A = \\frac{1}{2} b \\cdot h', substitution: `A = 0.5 \\cdot ${b} \\cdot ${h} = ${area}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0, label: 'A' },
                { x: b, y: 0, label: 'B' },
                { x: b / 2, y: h, label: 'C' }
              ]
            }
          };
        }
      },
      {
        id: 'sas',
        name: 'Two Sides & Included Angle (SAS)',
        description: 'Provide side a, side b, and included angle γ.',
        inputs: [
          { id: 'a', label: 'Side a', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'b', label: 'Side b', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'gamma', label: 'Angle γ', symbol: 'γ', type: UNIT_TYPES.ANGLE, default: (60 * Math.PI) / 180, min: 0.0001, max: Math.PI }
        ],
        calculate: (vals) => {
          const { a, b, gamma } = vals;
          const c = Math.sqrt(Math.max(0, a * a + b * b - 2 * a * b * Math.cos(gamma)));
          const area = 0.5 * a * b * Math.sin(gamma);
          const perimeter = a + b + c;
          const alpha = Math.asin(Math.max(-1, Math.min(1, (a * Math.sin(gamma)) / c)));
          const beta = Math.PI - gamma - alpha;

          return {
            outputs: [
              { id: 'c', label: 'Third Side (c)', symbol: 'c', value: c, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'alpha', label: 'Angle α', symbol: 'α', value: alpha, type: UNIT_TYPES.ANGLE },
              { id: 'beta', label: 'Angle β', symbol: 'β', value: beta, type: UNIT_TYPES.ANGLE }
            ],
            steps: [
              { title: 'Law of Cosines for side c', formula: 'c = \\sqrt{a^2 + b^2 - 2ab\\cos(\\gamma)}', substitution: `c = \\sqrt{${a}^2 + ${b}^2 - 2(${a})(${b})\\cos(${gamma})} = ${c}` },
              { title: 'Area from SAS', formula: 'A = \\frac{1}{2}ab\\sin(\\gamma)', substitution: `A = 0.5 \\cdot ${a} \\cdot ${b} \\cdot \\sin(${gamma}) = ${area}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0, label: 'C' },
                { x: a, y: 0, label: 'B' },
                { x: b * Math.cos(gamma), y: b * Math.sin(gamma), label: 'A' }
              ]
            }
          };
        }
      },
      {
        id: 'equilateral',
        name: 'Equilateral Triangle',
        description: 'Provide side length a of equilateral triangle.',
        inputs: [
          { id: 'a', label: 'Side length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const area = (Math.sqrt(3) / 4) * a * a;
          const perimeter = 3 * a;
          const height = (Math.sqrt(3) / 2) * a;
          const inradius = a / (2 * Math.sqrt(3));
          const circumradius = a / Math.sqrt(3);

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'height', label: 'Altitude / Height (h)', symbol: 'h', value: height, type: UNIT_TYPES.LENGTH },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Equilateral Area', formula: 'A = \\frac{\\sqrt{3}}{4} a^2', substitution: `A = (\\sqrt{3}/4) \\cdot ${a}^2 = ${area}` },
              { title: 'Height', formula: 'h = \\frac{\\sqrt{3}}{2} a', substitution: `h = (\\sqrt{3}/2) \\cdot ${a} = ${height}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0, label: 'A' },
                { x: a, y: 0, label: 'B' },
                { x: a / 2, y: height, label: 'C' }
              ],
              inradius,
              circumradius
            }
          };
        }
      },
      {
        id: 'right_triangle',
        name: 'Right-Angled Triangle (Legs a & b)',
        description: 'Provide perpendicular legs a and b.',
        inputs: [
          { id: 'a', label: 'Leg a', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 3, min: 0.0001 },
          { id: 'b', label: 'Leg b', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, b } = vals;
          const c = Math.sqrt(a * a + b * b);
          const area = 0.5 * a * b;
          const perimeter = a + b + c;
          const alpha = Math.atan2(a, b);
          const beta = Math.atan2(b, a);
          const inradius = (a + b - c) / 2;
          const circumradius = c / 2;
          const altitudeHyp = (a * b) / c;

          return {
            outputs: [
              { id: 'c', label: 'Hypotenuse (c)', symbol: 'c', value: c, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'altitudeHyp', label: 'Altitude to Hypotenuse', symbol: 'h_c', value: altitudeHyp, type: UNIT_TYPES.LENGTH },
              { id: 'alpha', label: 'Angle α', symbol: 'α', value: alpha, type: UNIT_TYPES.ANGLE },
              { id: 'beta', label: 'Angle β', symbol: 'β', value: beta, type: UNIT_TYPES.ANGLE },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Pythagorean Theorem', formula: 'c = \\sqrt{a^2 + b^2}', substitution: `c = \\sqrt{${a}^2 + ${b}^2} = ${c}` },
              { title: 'Right Triangle Area', formula: 'A = \\frac{1}{2} a \\cdot b', substitution: `A = 0.5 \\cdot ${a} \\cdot ${b} = ${area}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0, label: 'C (90°)' },
                { x: b, y: 0, label: 'A' },
                { x: 0, y: a, label: 'B' }
              ]
            }
          };
        }
      }
    ]
  },

  // 2. SQUARE
  {
    id: 'square',
    name: 'Square',
    category: '2d',
    description: 'Equilateral and equiangular regular quadrilateral with 4 equal sides and 90° angles.',
    icon: 'square',
    methods: [
      {
        id: 'by_side',
        name: 'By Side Length (a)',
        description: 'Calculate all properties from side length a.',
        inputs: [
          { id: 'a', label: 'Side length (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a } = vals;
          const perimeter = 4 * a;
          const area = a * a;
          const diagonal = a * Math.SQRT2;
          const inradius = a / 2;
          const circumradius = diagonal / 2;

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diagonal', label: 'Diagonal (d)', symbol: 'd', value: diagonal, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'inradius', label: 'Incircle Radius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumcircle Radius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Area', formula: 'A = a^2', substitution: `A = ${a}^2 = ${area}` },
              { title: 'Perimeter', formula: 'P = 4a', substitution: `P = 4 \\cdot ${a} = ${perimeter}` },
              { title: 'Diagonal', formula: 'd = a\\sqrt{2}', substitution: `d = ${a}\\sqrt{2} = ${diagonal}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0 },
                { x: a, y: 0 },
                { x: a, y: a },
                { x: 0, y: a }
              ],
              inradius,
              circumradius
            }
          };
        }
      },
      {
        id: 'by_diagonal',
        name: 'By Diagonal (d)',
        description: 'Calculate from diagonal length d.',
        inputs: [
          { id: 'd', label: 'Diagonal (d)', symbol: 'd', type: UNIT_TYPES.LENGTH, default: 7.071, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { d } = vals;
          const a = d / Math.SQRT2;
          const perimeter = 4 * a;
          const area = (d * d) / 2;
          return {
            outputs: [
              { id: 'a', label: 'Side length (a)', symbol: 'a', value: a, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Side from Diagonal', formula: 'a = d / \\sqrt{2}', substitution: `a = ${d} / \\sqrt{2} = ${a}` },
              { title: 'Area', formula: 'A = d^2 / 2', substitution: `A = ${d}^2 / 2 = ${area}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [{ x: 0, y: 0 }, { x: a, y: 0 }, { x: a, y: a }, { x: 0, y: a }]
            }
          };
        }
      },
      {
        id: 'by_area',
        name: 'By Area (A)',
        description: 'Calculate side, diagonal, and perimeter from area.',
        inputs: [
          { id: 'A', label: 'Area (A)', symbol: 'A', type: UNIT_TYPES.AREA, default: 25, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { A } = vals;
          const a = Math.sqrt(A);
          const diagonal = a * Math.SQRT2;
          const perimeter = 4 * a;
          return {
            outputs: [
              { id: 'a', label: 'Side length (a)', symbol: 'a', value: a, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diagonal', label: 'Diagonal (d)', symbol: 'd', value: diagonal, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Side from Area', formula: 'a = \\sqrt{A}', substitution: `a = \\sqrt{${A}} = ${a}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [{ x: 0, y: 0 }, { x: a, y: 0 }, { x: a, y: a }, { x: 0, y: a }]
            }
          };
        }
      }
    ]
  },

  // 3. RECTANGLE
  {
    id: 'rectangle',
    name: 'Rectangle',
    category: '2d',
    description: 'Four-sided polygon with four right angles and opposite equal sides.',
    icon: 'rectangle',
    methods: [
      {
        id: 'length_width',
        name: 'Length & Width (l, w)',
        description: 'Provide length and width.',
        inputs: [
          { id: 'l', label: 'Length (l)', symbol: 'l', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'w', label: 'Width (w)', symbol: 'w', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { l, w } = vals;
          const area = l * w;
          const perimeter = 2 * (l + w);
          const diagonal = Math.sqrt(l * l + w * w);
          const aspectRatio = l / w;
          const circumradius = diagonal / 2;

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diagonal', label: 'Diagonal (d)', symbol: 'd', value: diagonal, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'aspect_ratio', label: 'Aspect Ratio (l : w)', symbol: 'l:w', textValue: `${l >= w ? (l / w).toFixed(2) + ' : 1' : '1 : ' + (w / l).toFixed(2)}` },
              { id: 'circumradius', label: 'Circumscribed Radius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Area', formula: 'A = l \\cdot w', substitution: `A = ${l} \\cdot ${w} = ${area}` },
              { title: 'Perimeter', formula: 'P = 2(l + w)', substitution: `P = 2(${l} + ${w}) = ${perimeter}` },
              { title: 'Diagonal', formula: 'd = \\sqrt{l^2 + w^2}', substitution: `d = \\sqrt{${l}^2 + ${w}^2} = ${diagonal}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [{ x: 0, y: 0 }, { x: l, y: 0 }, { x: l, y: w }, { x: 0, y: w }],
              circumradius
            }
          };
        }
      },
      {
        id: 'diagonal_side',
        name: 'Diagonal & One Side',
        description: 'Provide diagonal d and one side length.',
        inputs: [
          { id: 'd', label: 'Diagonal (d)', symbol: 'd', type: UNIT_TYPES.LENGTH, default: 10, min: 0.0001 },
          { id: 'l', label: 'Known Side (l)', symbol: 'l', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { d, l } = vals;
          if (l >= d) throw new Error('Known side must be strictly less than the diagonal.');
          const w = Math.sqrt(d * d - l * l);
          const area = l * w;
          const perimeter = 2 * (l + w);
          return {
            outputs: [
              { id: 'w', label: 'Other Side (w)', symbol: 'w', value: w, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Other side from Pythagoras', formula: 'w = \\sqrt{d^2 - l^2}', substitution: `w = \\sqrt{${d}^2 - ${l}^2} = ${w}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [{ x: 0, y: 0 }, { x: l, y: 0 }, { x: l, y: w }, { x: 0, y: w }]
            }
          };
        }
      }
    ]
  },

  // 4. CIRCLE
  {
    id: 'circle',
    name: 'Circle',
    category: '2d',
    description: 'The set of all points in a plane equidistant from a central point.',
    icon: 'circle',
    methods: [
      {
        id: 'by_radius',
        name: 'By Radius (r)',
        description: 'Enter radius r.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { r } = vals;
          const diameter = 2 * r;
          const circumference = 2 * Math.PI * r;
          const area = Math.PI * r * r;
          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'circumference', label: 'Circumference (C)', symbol: 'C', value: circumference, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'diameter', label: 'Diameter (d)', symbol: 'd', value: diameter, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Diameter', formula: 'd = 2r', substitution: `d = 2 \\cdot ${r} = ${diameter}` },
              { title: 'Circumference', formula: 'C = 2\\pi r', substitution: `C = 2 \\cdot \\pi \\cdot ${r} = ${circumference}` },
              { title: 'Area', formula: 'A = \\pi r^2', substitution: `A = \\pi \\cdot ${r}^2 = ${area}` }
            ],
            svgData: { shape: 'circle', r }
          };
        }
      },
      {
        id: 'by_diameter',
        name: 'By Diameter (d)',
        description: 'Enter diameter d.',
        inputs: [
          { id: 'd', label: 'Diameter (d)', symbol: 'd', type: UNIT_TYPES.LENGTH, default: 10, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { d } = vals;
          const r = d / 2;
          const circumference = Math.PI * d;
          const area = (Math.PI * d * d) / 4;
          return {
            outputs: [
              { id: 'radius', label: 'Radius (r)', symbol: 'r', value: r, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'circumference', label: 'Circumference (C)', symbol: 'C', value: circumference, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Radius', formula: 'r = d / 2', substitution: `r = ${d} / 2 = ${r}` },
              { title: 'Area', formula: 'A = \\frac{\\pi d^2}{4}', substitution: `A = (\\pi \\cdot ${d}^2) / 4 = ${area}` }
            ],
            svgData: { shape: 'circle', r }
          };
        }
      },
      {
        id: 'by_area',
        name: 'By Area (A)',
        description: 'Enter area A.',
        inputs: [
          { id: 'A', label: 'Area (A)', symbol: 'A', type: UNIT_TYPES.AREA, default: 78.5398, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { A } = vals;
          const r = Math.sqrt(A / Math.PI);
          const d = 2 * r;
          const c = 2 * Math.PI * r;
          return {
            outputs: [
              { id: 'r', label: 'Radius (r)', symbol: 'r', value: r, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'd', label: 'Diameter (d)', symbol: 'd', value: d, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'c', label: 'Circumference (C)', symbol: 'C', value: c, type: UNIT_TYPES.LENGTH, isPrimary: true }
            ],
            steps: [
              { title: 'Radius from Area', formula: 'r = \\sqrt{A / \\pi}', substitution: `r = \\sqrt{${A} / \\pi} = ${r}` }
            ],
            svgData: { shape: 'circle', r }
          };
        }
      }
    ]
  },

  // 5. ELLIPSE
  {
    id: 'ellipse',
    name: 'Ellipse',
    category: '2d',
    description: 'A smooth curve surrounding two focal points such that the sum of distances is constant.',
    icon: 'ellipse',
    methods: [
      {
        id: 'axes',
        name: 'Semi-Major & Semi-Minor Axes (a, b)',
        description: 'Provide semi-major axis a and semi-minor axis b.',
        inputs: [
          { id: 'a', label: 'Semi-Major Axis (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'b', label: 'Semi-Minor Axis (b)', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, b } = vals;
          const major = Math.max(a, b);
          const minor = Math.min(a, b);
          const area = Math.PI * major * minor;
          const linearEccentricity = Math.sqrt(Math.max(0, major * major - minor * minor));
          const eccentricity = linearEccentricity / major;

          // Ramanujan's approximation for ellipse perimeter
          const h = ((major - minor) ** 2) / ((major + minor) ** 2);
          const perimeter = Math.PI * (major + minor) * (1 + (3 * h) / (10 + Math.sqrt(4 - 3 * h)));

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter / Circumference (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'eccentricity', label: 'Eccentricity (e)', symbol: 'e', textValue: eccentricity.toFixed(4) },
              { id: 'focal_dist', label: 'Focal Distance (2c)', symbol: '2c', value: 2 * linearEccentricity, type: UNIT_TYPES.LENGTH },
              { id: 'major_axis', label: 'Full Major Axis (2a)', symbol: '2a', value: 2 * major, type: UNIT_TYPES.LENGTH },
              { id: 'minor_axis', label: 'Full Minor Axis (2b)', symbol: '2b', value: 2 * minor, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Ellipse Area', formula: 'A = \\pi a b', substitution: `A = \\pi \\cdot ${major} \\cdot ${minor} = ${area}` },
              { title: 'Ramanujan Perimeter Formula', formula: 'P \\approx \\pi(a+b)\\left(1 + \\frac{3h}{10+\\sqrt{4-3h}}\\right)', substitution: `P \\approx ${perimeter}` },
              { title: 'Eccentricity', formula: 'e = \\frac{\\sqrt{a^2 - b^2}}{a}', substitution: `e = \\frac{\\sqrt{${major}^2 - ${minor}^2}}{${major}} = ${eccentricity}` }
            ],
            svgData: { shape: 'ellipse', rx: major, ry: minor }
          };
        }
      }
    ]
  },

  // 6. PARALLELOGRAM
  {
    id: 'parallelogram',
    name: 'Parallelogram',
    category: '2d',
    description: 'Quadrilateral with opposite sides parallel and equal.',
    icon: 'parallelogram',
    methods: [
      {
        id: 'base_side_angle',
        name: 'Sides & Included Angle (a, b, θ)',
        description: 'Provide base b, side a, and angle θ.',
        inputs: [
          { id: 'b', label: 'Base (b)', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'a', label: 'Side (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'theta', label: 'Included Angle (θ)', symbol: 'θ', type: UNIT_TYPES.ANGLE, default: (60 * Math.PI) / 180, min: 0.0001, max: Math.PI }
        ],
        calculate: (vals) => {
          const { b, a, theta } = vals;
          const height = a * Math.sin(theta);
          const area = b * height;
          const perimeter = 2 * (a + b);
          const d1 = Math.sqrt(a * a + b * b - 2 * a * b * Math.cos(theta));
          const d2 = Math.sqrt(a * a + b * b + 2 * a * b * Math.cos(theta));

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'height', label: 'Height (h)', symbol: 'h', value: height, type: UNIT_TYPES.LENGTH },
              { id: 'd1', label: 'Diagonal 1 (d₁)', symbol: 'd₁', value: d1, type: UNIT_TYPES.LENGTH },
              { id: 'd2', label: 'Diagonal 2 (d₂)', symbol: 'd₂', value: d2, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Height', formula: 'h = a \\cdot \\sin(\\theta)', substitution: `h = ${a} \\cdot \\sin(${theta}) = ${height}` },
              { title: 'Area', formula: 'A = b \\cdot h', substitution: `A = ${b} \\cdot ${height} = ${area}` },
              { title: 'Perimeter', formula: 'P = 2(a + b)', substitution: `P = 2(${a} + ${b}) = ${perimeter}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0 },
                { x: b, y: 0 },
                { x: b + a * Math.cos(theta), y: height },
                { x: a * Math.cos(theta), y: height }
              ]
            }
          };
        }
      }
    ]
  },

  // 7. RHOMBUS
  {
    id: 'rhombus',
    name: 'Rhombus',
    category: '2d',
    description: 'Quadrilateral whose four sides all have the same length with perpendicular diagonals.',
    icon: 'rhombus',
    methods: [
      {
        id: 'diagonals',
        name: 'Two Diagonals (d₁, d₂)',
        description: 'Provide perpendicular diagonals d1 and d2.',
        inputs: [
          { id: 'd1', label: 'Diagonal 1 (d₁)', symbol: 'd₁', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'd2', label: 'Diagonal 2 (d₂)', symbol: 'd₂', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { d1, d2 } = vals;
          const area = 0.5 * d1 * d2;
          const a = 0.5 * Math.sqrt(d1 * d1 + d2 * d2);
          const perimeter = 4 * a;
          const height = area / a;
          const inradius = height / 2;

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'a', label: 'Side Length (a)', symbol: 'a', value: a, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'height', label: 'Height (h)', symbol: 'h', value: height, type: UNIT_TYPES.LENGTH },
              { id: 'inradius', label: 'Inradius (r)', symbol: 'r', value: inradius, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Area from Diagonals', formula: 'A = \\frac{1}{2} d_1 d_2', substitution: `A = 0.5 \\cdot ${d1} \\cdot ${d2} = ${area}` },
              { title: 'Side Length', formula: 'a = \\frac{1}{2}\\sqrt{d_1^2 + d_2^2}', substitution: `a = 0.5 \\cdot \\sqrt{${d1}^2 + ${d2}^2} = ${a}` },
              { title: 'Perimeter', formula: 'P = 4a', substitution: `P = 4 \\cdot ${a} = ${perimeter}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: d1 / 2, y: 0 },
                { x: d1, y: d2 / 2 },
                { x: d1 / 2, y: d2 },
                { x: 0, y: d2 / 2 }
              ],
              inradius
            }
          };
        }
      }
    ]
  },

  // 8. TRAPEZOID (TRAPEZIUM)
  {
    id: 'trapezoid',
    name: 'Trapezoid (Trapezium)',
    category: '2d',
    description: 'Quadrilateral with at least one pair of parallel sides (bases).',
    icon: 'trapezoid',
    methods: [
      {
        id: 'bases_height',
        name: 'Parallel Bases & Height (a, b, h)',
        description: 'Provide two parallel bases a and b, and height h.',
        inputs: [
          { id: 'a', label: 'Top Base (a)', symbol: 'a', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 },
          { id: 'b', label: 'Bottom Base (b)', symbol: 'b', type: UNIT_TYPES.LENGTH, default: 9, min: 0.0001 },
          { id: 'h', label: 'Height (h)', symbol: 'h', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { a, b, h } = vals;
          const area = 0.5 * (a + b) * h;
          const midline = 0.5 * (a + b);
          // Symmetric isosceles assumption for preview legs
          const leg = Math.sqrt(h * h + ((b - a) / 2) ** 2);
          const perimeter = a + b + 2 * leg;

          return {
            outputs: [
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'midline', label: 'Median / Midline (m)', symbol: 'm', value: midline, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (Isosceles)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH },
              { id: 'leg', label: 'Side Leg (Isosceles)', symbol: 'c = d', value: leg, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Trapezoid Area', formula: 'A = \\frac{a + b}{2} \\cdot h', substitution: `A = \\frac{${a} + ${b}}{2} \\cdot ${h} = ${area}` },
              { title: 'Median', formula: 'm = \\frac{a + b}{2}', substitution: `m = (${a} + ${b})/2 = ${midline}` }
            ],
            svgData: {
              shape: 'polygon',
              points: [
                { x: 0, y: 0 },
                { x: b, y: 0 },
                { x: (b - a) / 2 + a, y: h },
                { x: (b - a) / 2, y: h }
              ]
            }
          };
        }
      }
    ]
  },

  // 9. REGULAR POLYGON
  {
    id: 'regular_polygon',
    name: 'Regular Polygon (n-gon)',
    category: '2d',
    description: 'Equiangular and equilateral polygon with n sides (Pentagon, Hexagon, Octagon, etc.).',
    icon: 'polygon',
    methods: [
      {
        id: 'sides_length',
        name: 'Sides Count (n) & Side Length (s)',
        description: 'Provide number of sides n and length of each side s.',
        inputs: [
          { id: 'n', label: 'Number of Sides (n)', symbol: 'n', type: UNIT_TYPES.COUNT, default: 6, min: 3, max: 100 },
          { id: 's', label: 'Side Length (s)', symbol: 's', type: UNIT_TYPES.LENGTH, default: 4, min: 0.0001 }
        ],
        calculate: (vals) => {
          const n = Math.round(vals.n);
          const s = vals.s;
          const perimeter = n * s;
          const apothem = s / (2 * Math.tan(Math.PI / n)); // Inradius
          const circumradius = s / (2 * Math.sin(Math.PI / n));
          const area = 0.5 * perimeter * apothem;
          const interiorAngle = ((n - 2) * Math.PI) / n;
          const exteriorAngle = (2 * Math.PI) / n;
          const diagonals = (n * (n - 3)) / 2;

          // Points for SVG centered at (circumradius, circumradius)
          const points = [];
          for (let i = 0; i < n; i++) {
            const angle = (2 * Math.PI * i) / n - Math.PI / 2;
            points.push({
              x: circumradius + circumradius * Math.cos(angle),
              y: circumradius + circumradius * Math.sin(angle)
            });
          }

          let name = `${n}-gon`;
          if (n === 3) name = 'Equilateral Triangle';
          else if (n === 4) name = 'Square';
          else if (n === 5) name = 'Regular Pentagon';
          else if (n === 6) name = 'Regular Hexagon';
          else if (n === 7) name = 'Regular Heptagon';
          else if (n === 8) name = 'Regular Octagon';
          else if (n === 10) name = 'Regular Decagon';
          else if (n === 12) name = 'Regular Dodecagon';

          return {
            outputs: [
              { id: 'polygon_name', label: 'Polygon Name', symbol: 'Name', textValue: name },
              { id: 'area', label: 'Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'perimeter', label: 'Perimeter (P)', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'apothem', label: 'Apothem / Inradius (r)', symbol: 'r', value: apothem, type: UNIT_TYPES.LENGTH },
              { id: 'circumradius', label: 'Circumradius (R)', symbol: 'R', value: circumradius, type: UNIT_TYPES.LENGTH },
              { id: 'interior_angle', label: 'Interior Angle', symbol: 'α_int', value: interiorAngle, type: UNIT_TYPES.ANGLE },
              { id: 'exterior_angle', label: 'Exterior Angle', symbol: 'α_ext', value: exteriorAngle, type: UNIT_TYPES.ANGLE },
              { id: 'diagonals', label: 'Total Diagonals', symbol: 'D', textValue: `${diagonals}` }
            ],
            steps: [
              { title: 'Perimeter', formula: 'P = n \\cdot s', substitution: `P = ${n} \\cdot ${s} = ${perimeter}` },
              { title: 'Apothem (Inradius)', formula: 'r = \\frac{s}{2\\tan(\\pi / n)}', substitution: `r = \\frac{${s}}{2\\tan(\\pi / ${n})} = ${apothem}` },
              { title: 'Area', formula: 'A = \\frac{1}{2} P \\cdot r', substitution: `A = 0.5 \\cdot ${perimeter} \\cdot ${apothem} = ${area}` }
            ],
            svgData: { shape: 'polygon', points, inradius: apothem, circumradius }
          };
        }
      }
    ]
  },

  // 10. CIRCULAR SECTOR & SEGMENT
  {
    id: 'sector',
    name: 'Circular Sector & Segment',
    category: '2d',
    description: 'Portion of a disk enclosed by two radii and an arc, and the corresponding chord segment.',
    icon: 'sector',
    methods: [
      {
        id: 'radius_angle',
        name: 'Radius & Central Angle (r, θ)',
        description: 'Provide radius r and angle θ.',
        inputs: [
          { id: 'r', label: 'Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 6, min: 0.0001 },
          { id: 'theta', label: 'Central Angle (θ)', symbol: 'θ', type: UNIT_TYPES.ANGLE, default: (45 * Math.PI) / 180, min: 0.0001, max: 2 * Math.PI }
        ],
        calculate: (vals) => {
          const { r, theta } = vals;
          const arcLength = r * theta;
          const chordLength = 2 * r * Math.sin(theta / 2);
          const sectorArea = 0.5 * r * r * theta;
          const triangleArea = 0.5 * r * r * Math.sin(theta);
          const segmentArea = sectorArea - triangleArea;
          const perimeter = 2 * r + arcLength;

          return {
            outputs: [
              { id: 'sector_area', label: 'Sector Area (A_sec)', symbol: 'A_sec', value: sectorArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'segment_area', label: 'Segment Area (A_seg)', symbol: 'A_seg', value: segmentArea, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'arc_length', label: 'Arc Length (s)', symbol: 's', value: arcLength, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'chord_length', label: 'Chord Length (c)', symbol: 'c', value: chordLength, type: UNIT_TYPES.LENGTH },
              { id: 'perimeter', label: 'Sector Perimeter', symbol: 'P', value: perimeter, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Arc Length', formula: 's = r \\cdot \\theta', substitution: `s = ${r} \\cdot ${theta} = ${arcLength}` },
              { title: 'Sector Area', formula: 'A_{sec} = \\frac{1}{2} r^2 \\theta', substitution: `A_{sec} = 0.5 \\cdot ${r}^2 \\cdot ${theta} = ${sectorArea}` },
              { title: 'Segment Area', formula: 'A_{seg} = \\frac{1}{2} r^2 (\\theta - \\sin\\theta)', substitution: `A_{seg} = ${segmentArea}` }
            ],
            svgData: { shape: 'sector', r, theta }
          };
        }
      }
    ]
  },

  // 11. ANNULUS (RING)
  {
    id: 'annulus',
    name: 'Annulus (Ring)',
    category: '2d',
    description: 'The region between two concentric circles with different radii.',
    icon: 'annulus',
    methods: [
      {
        id: 'radii',
        name: 'Outer & Inner Radii (R, r)',
        description: 'Provide outer radius R and inner radius r.',
        inputs: [
          { id: 'R', label: 'Outer Radius (R)', symbol: 'R', type: UNIT_TYPES.LENGTH, default: 8, min: 0.0001 },
          { id: 'r', label: 'Inner Radius (r)', symbol: 'r', type: UNIT_TYPES.LENGTH, default: 5, min: 0.0001 }
        ],
        calculate: (vals) => {
          const { R, r } = vals;
          if (r >= R) throw new Error('Inner radius (r) must be strictly less than outer radius (R).');
          const width = R - r;
          const area = Math.PI * (R * R - r * r);
          const outerCircumference = 2 * Math.PI * R;
          const innerCircumference = 2 * Math.PI * r;

          return {
            outputs: [
              { id: 'area', label: 'Annulus Area (A)', symbol: 'A', value: area, type: UNIT_TYPES.AREA, isPrimary: true },
              { id: 'width', label: 'Ring Width (w)', symbol: 'w', value: width, type: UNIT_TYPES.LENGTH, isPrimary: true },
              { id: 'outer_c', label: 'Outer Circumference (C_out)', symbol: 'C_out', value: outerCircumference, type: UNIT_TYPES.LENGTH },
              { id: 'inner_c', label: 'Inner Circumference (C_in)', symbol: 'C_in', value: innerCircumference, type: UNIT_TYPES.LENGTH }
            ],
            steps: [
              { title: 'Annulus Area', formula: 'A = \\pi(R^2 - r^2)', substitution: `A = \\pi(${R}^2 - ${r}^2) = ${area}` },
              { title: 'Ring Width', formula: 'w = R - r', substitution: `w = ${R} - ${r} = ${width}` }
            ],
            svgData: { shape: 'annulus', R, r }
          };
        }
      }
    ]
  }
];
