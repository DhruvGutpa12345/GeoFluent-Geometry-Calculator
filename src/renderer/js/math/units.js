/**
 * GeoFluent Unit Conversion Engine
 * High-precision conversions for length, area, volume, and angle.
 */

export const UNIT_TYPES = {
  LENGTH: 'length',
  AREA: 'area',
  VOLUME: 'volume',
  ANGLE: 'angle',
  COUNT: 'count' // dimensionless (e.g. number of sides n)
};

export const UNITS = {
  length: [
    { id: 'mm', name: 'Millimeters (mm)', symbol: 'mm', factor: 0.001 },
    { id: 'cm', name: 'Centimeters (cm)', symbol: 'cm', factor: 0.01 },
    { id: 'dm', name: 'Decimeters (dm)', symbol: 'dm', factor: 0.1 },
    { id: 'm', name: 'Meters (m)', symbol: 'm', factor: 1.0, isBase: true },
    { id: 'km', name: 'Kilometers (km)', symbol: 'km', factor: 1000.0 },
    { id: 'in', name: 'Inches (in)', symbol: 'in', factor: 0.0254 },
    { id: 'ft', name: 'Feet (ft)', symbol: 'ft', factor: 0.3048 },
    { id: 'yd', name: 'Yards (yd)', symbol: 'yd', factor: 0.9144 },
    { id: 'mi', name: 'Miles (mi)', symbol: 'mi', factor: 1609.344 },
    { id: 'nmi', name: 'Nautical Miles (nmi)', symbol: 'nmi', factor: 1852.0 }
  ],

  area: [
    { id: 'mm2', name: 'Square Millimeters (mm²)', symbol: 'mm²', factor: 1e-6 },
    { id: 'cm2', name: 'Square Centimeters (cm²)', symbol: 'cm²', factor: 0.0001 },
    { id: 'dm2', name: 'Square Decimeters (dm²)', symbol: 'dm²', factor: 0.01 },
    { id: 'm2', name: 'Square Meters (m²)', symbol: 'm²', factor: 1.0, isBase: true },
    { id: 'ha', name: 'Hectares (ha)', symbol: 'ha', factor: 10000.0 },
    { id: 'km2', name: 'Square Kilometers (km²)', symbol: 'km²', factor: 1e6 },
    { id: 'sq_in', name: 'Square Inches (sq in)', symbol: 'in²', factor: 0.00064516 },
    { id: 'sq_ft', name: 'Square Feet (sq ft)', symbol: 'ft²', factor: 0.09290304 },
    { id: 'sq_yd', name: 'Square Yards (sq yd)', symbol: 'yd²', factor: 0.83612736 },
    { id: 'acre', name: 'Acres (ac)', symbol: 'ac', factor: 4046.8564224 },
    { id: 'sq_mi', name: 'Square Miles (sq mi)', symbol: 'mi²', factor: 2589988.110336 }
  ],

  volume: [
    { id: 'mm3', name: 'Cubic Millimeters (mm³)', symbol: 'mm³', factor: 1e-9 },
    { id: 'cm3', name: 'Cubic Centimeters / mL (cm³)', symbol: 'cm³', factor: 1e-6 },
    { id: 'dm3_l', name: 'Liters / dm³ (L)', symbol: 'L', factor: 0.001 },
    { id: 'm3', name: 'Cubic Meters (m³)', symbol: 'm³', factor: 1.0, isBase: true },
    { id: 'cu_in', name: 'Cubic Inches (cu in)', symbol: 'in³', factor: 0.000016387064 },
    { id: 'cu_ft', name: 'Cubic Feet (cu ft)', symbol: 'ft²', factor: 0.028316846592 },
    { id: 'cu_yd', name: 'Cubic Yards (cu yd)', symbol: 'yd³', factor: 0.764554857984 },
    { id: 'fl_oz', name: 'Fluid Ounces (US fl oz)', symbol: 'fl oz', factor: 0.0000295735295625 },
    { id: 'cup', name: 'Cups (US cup)', symbol: 'cup', factor: 0.0002365882365 },
    { id: 'pt', name: 'Pints (US pt)', symbol: 'pt', factor: 0.000473176473 },
    { id: 'qt', name: 'Quarts (US qt)', symbol: 'qt', factor: 0.000946352946 },
    { id: 'gal_us', name: 'Gallons (US gal)', symbol: 'gal', factor: 0.003785411784 },
    { id: 'gal_uk', name: 'Gallons (Imperial gal)', symbol: 'imp gal', factor: 0.00454609 }
  ],

  angle: [
    { id: 'deg', name: 'Degrees (°)', symbol: '°', toRad: deg => (deg * Math.PI) / 180, fromRad: rad => (rad * 180) / Math.PI, isBase: false },
    { id: 'rad', name: 'Radians (rad)', symbol: 'rad', toRad: rad => rad, fromRad: rad => rad, isBase: true },
    { id: 'grad', name: 'Gradians (grad)', symbol: 'grad', toRad: g => (g * Math.PI) / 200, fromRad: rad => (rad * 200) / Math.PI, isBase: false }
  ]
};

/**
 * Convert a given value from a specific unit into base unit (m, m², m³, rad).
 */
export function toBase(value, unitId, type) {
  if (value === null || value === undefined || isNaN(value)) return NaN;
  if (type === UNIT_TYPES.COUNT) return Number(value);

  const unitList = UNITS[type];
  if (!unitList) return Number(value);

  const unit = unitList.find(u => u.id === unitId) || unitList[0];
  if (type === UNIT_TYPES.ANGLE) {
    return unit.toRad(Number(value));
  }
  return Number(value) * unit.factor;
}

/**
 * Convert a value from base unit (m, m², m³, rad) into target unit.
 */
export function fromBase(baseValue, targetUnitId, type) {
  if (baseValue === null || baseValue === undefined || isNaN(baseValue)) return NaN;
  if (type === UNIT_TYPES.COUNT) return Number(baseValue);

  const unitList = UNITS[type];
  if (!unitList) return Number(baseValue);

  const unit = unitList.find(u => u.id === targetUnitId) || unitList[0];
  if (type === UNIT_TYPES.ANGLE) {
    return unit.fromRad(Number(baseValue));
  }
  return Number(baseValue) / unit.factor;
}

/**
 * Convert directly from one unit to another.
 */
export function convert(value, fromUnitId, toUnitId, type) {
  if (fromUnitId === toUnitId) return Number(value);
  const base = toBase(value, fromUnitId, type);
  return fromBase(base, toUnitId, type);
}

/**
 * Get unit object by ID
 */
export function getUnit(unitId, type) {
  const list = UNITS[type];
  if (!list) return null;
  return list.find(u => u.id === unitId) || list[0];
}

/**
 * Format a number nicely for display with precision settings.
 */
export function formatNumber(val, precision = 4) {
  if (val === null || val === undefined || isNaN(val)) return '—';
  if (!isFinite(val)) return val > 0 ? 'Infinity' : '-Infinity';

  const num = Number(val);
  if (Math.abs(num) === 0) return '0';

  // If very large or very small, use scientific notation
  if (Math.abs(num) >= 1e12 || (Math.abs(num) < 1e-5 && Math.abs(num) > 0)) {
    return num.toExponential(precision);
  }

  // Round to specified precision decimals, then strip trailing zeros
  const factor = Math.pow(10, precision);
  const rounded = Math.round(num * factor) / factor;
  return rounded.toLocaleString('en-US', {
    maximumFractionDigits: precision,
    useGrouping: true
  });
}
