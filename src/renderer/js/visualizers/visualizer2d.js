/**
 * GeoFluent 2D SVG Visualizer
 * Dynamic SVG rendering engine for 2D geometric shapes with auto-scaling,
 * dimension callouts, vertex dots, angle arcs, and inscribed/circumscribed guides.
 */

export class Visualizer2D {
  constructor(containerElement) {
    this.container = containerElement;
    this.currentData = null;
    this.showGuides = true;
  }

  setShowGuides(show) {
    this.showGuides = show;
    if (this.currentData) {
      this.render(this.currentData);
    }
  }

  render(svgData) {
    this.currentData = svgData;
    if (!this.container || !svgData) return;

    this.container.innerHTML = '';
    const width = this.container.clientWidth || 400;
    const height = this.container.clientHeight || 300;
    const padding = 50;

    const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
    svg.setAttribute('width', '100%');
    svg.setAttribute('height', '100%');
    svg.setAttribute('viewBox', `0 0 ${width} ${height}`);
    svg.style.overflow = 'visible';

    // Defs for arrows & gradients
    const defs = document.createElementNS('http://www.w3.org/2000/svg', 'defs');
    defs.innerHTML = `
      <linearGradient id="fluent2dGrad" x1="0%" y1="0%" x2="100%" y2="100%">
        <stop offset="0%" stop-color="#0078D4" stop-opacity="0.35"/>
        <stop offset="100%" stop-color="#60CDFF" stop-opacity="0.15"/>
      </linearGradient>
      <marker id="arrow" viewBox="0 0 10 10" refX="5" refY="5" markerWidth="6" markerHeight="6" orient="auto-start-reverse">
        <path d="M 0 0 L 10 5 L 0 10 z" fill="#60CDFF" />
      </marker>
    `;
    svg.appendChild(defs);

    const group = document.createElementNS('http://www.w3.org/2000/svg', 'g');

    if (svgData.shape === 'polygon' && svgData.points && svgData.points.length > 0) {
      this._renderPolygon(svg, group, svgData, width, height, padding);
    } else if (svgData.shape === 'circle') {
      this._renderCircle(svg, group, svgData, width, height, padding);
    } else if (svgData.shape === 'ellipse') {
      this._renderEllipse(svg, group, svgData, width, height, padding);
    } else if (svgData.shape === 'sector') {
      this._renderSector(svg, group, svgData, width, height, padding);
    } else if (svgData.shape === 'annulus') {
      this._renderAnnulus(svg, group, svgData, width, height, padding);
    }

    svg.appendChild(group);
    this.container.appendChild(svg);
  }

  _renderPolygon(svg, group, data, width, height, padding) {
    const pts = data.points;
    let minX = Infinity, maxX = -Infinity, minY = Infinity, maxY = -Infinity;
    pts.forEach(p => {
      minX = Math.min(minX, p.x);
      maxX = Math.max(maxX, p.x);
      minY = Math.min(minY, p.y);
      maxY = Math.max(maxY, p.y);
    });

    const shapeW = Math.max(0.001, maxX - minX);
    const shapeH = Math.max(0.001, maxY - minY);
    const availW = width - 2 * padding;
    const availH = height - 2 * padding;
    const scale = Math.min(availW / shapeW, availH / shapeH);

    const offsetX = padding + (availW - shapeW * scale) / 2 - minX * scale;
    const offsetY = height - (padding + (availH - shapeH * scale) / 2) + minY * scale;

    const transformPt = (p) => ({
      x: offsetX + p.x * scale,
      y: offsetY - p.y * scale
    });

    const screenPts = pts.map(transformPt);

    // Inradius / Circumradius guidelines if available
    if (this.showGuides && (data.inradius || data.circumradius)) {
      // Find centroid of screen pts
      const cx = screenPts.reduce((acc, p) => acc + p.x, 0) / screenPts.length;
      const cy = screenPts.reduce((acc, p) => acc + p.y, 0) / screenPts.length;

      if (data.circumradius) {
        const rC = data.circumradius * scale;
        const circCircle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
        circCircle.setAttribute('cx', cx);
        circCircle.setAttribute('cy', cy);
        circCircle.setAttribute('r', rC);
        circCircle.setAttribute('fill', 'none');
        circCircle.setAttribute('stroke', '#a78bfa');
        circCircle.setAttribute('stroke-width', '1.5');
        circCircle.setAttribute('stroke-dasharray', '4 4');
        circCircle.setAttribute('opacity', '0.6');
        group.appendChild(circCircle);
      }

      if (data.inradius) {
        const rIn = data.inradius * scale;
        const inCircle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
        inCircle.setAttribute('cx', cx);
        inCircle.setAttribute('cy', cy);
        inCircle.setAttribute('r', rIn);
        inCircle.setAttribute('fill', 'none');
        inCircle.setAttribute('stroke', '#34d399');
        inCircle.setAttribute('stroke-width', '1.5');
        inCircle.setAttribute('stroke-dasharray', '3 3');
        inCircle.setAttribute('opacity', '0.7');
        group.appendChild(inCircle);
      }
    }

    // Polygon shape
    const polygon = document.createElementNS('http://www.w3.org/2000/svg', 'polygon');
    const pointsStr = screenPts.map(p => `${p.x},${p.y}`).join(' ');
    polygon.setAttribute('points', pointsStr);
    polygon.setAttribute('fill', 'url(#fluent2dGrad)');
    polygon.setAttribute('stroke', '#0078D4');
    polygon.setAttribute('stroke-width', '2.5');
    polygon.setAttribute('stroke-linejoin', 'round');
    group.appendChild(polygon);

    // Vertices dots and labels
    screenPts.forEach((p, idx) => {
      const circle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
      circle.setAttribute('cx', p.x);
      circle.setAttribute('cy', p.y);
      circle.setAttribute('r', '4.5');
      circle.setAttribute('fill', '#60CDFF');
      circle.setAttribute('stroke', '#FFFFFF');
      circle.setAttribute('stroke-width', '2');
      group.appendChild(circle);

      const labelText = pts[idx].label || (pts.length <= 8 ? String.fromCharCode(65 + idx) : '');
      if (labelText) {
        const text = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        text.setAttribute('x', p.x);
        text.setAttribute('y', p.y - 10);
        text.setAttribute('text-anchor', 'middle');
        text.setAttribute('fill', 'currentColor');
        text.setAttribute('font-size', '12');
        text.setAttribute('font-weight', '600');
        text.textContent = labelText;
        group.appendChild(text);
      }
    });
  }

  _renderCircle(svg, group, data, width, height, padding) {
    const cx = width / 2;
    const cy = height / 2;
    const r = Math.min((width - 2 * padding) / 2, (height - 2 * padding) / 2);

    const circle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    circle.setAttribute('cx', cx);
    circle.setAttribute('cy', cy);
    circle.setAttribute('r', r);
    circle.setAttribute('fill', 'url(#fluent2dGrad)');
    circle.setAttribute('stroke', '#0078D4');
    circle.setAttribute('stroke-width', '2.5');
    group.appendChild(circle);

    // Center dot
    const center = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    center.setAttribute('cx', cx);
    center.setAttribute('cy', cy);
    center.setAttribute('r', '4');
    center.setAttribute('fill', '#60CDFF');
    group.appendChild(center);

    // Radius line
    const radiusLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    radiusLine.setAttribute('x1', cx);
    radiusLine.setAttribute('y1', cy);
    radiusLine.setAttribute('x2', cx + r);
    radiusLine.setAttribute('y2', cy);
    radiusLine.setAttribute('stroke', '#60CDFF');
    radiusLine.setAttribute('stroke-width', '2');
    radiusLine.setAttribute('stroke-dasharray', '4 2');
    group.appendChild(radiusLine);

    const label = document.createElementNS('http://www.w3.org/2000/svg', 'text');
    label.setAttribute('x', cx + r / 2);
    label.setAttribute('y', cy - 8);
    label.setAttribute('text-anchor', 'middle');
    label.setAttribute('fill', 'currentColor');
    label.setAttribute('font-size', '13');
    label.setAttribute('font-weight', '600');
    label.textContent = `r`;
    group.appendChild(label);
  }

  _renderEllipse(svg, group, data, width, height, padding) {
    const cx = width / 2;
    const cy = height / 2;
    const maxR = Math.min((width - 2 * padding) / 2, (height - 2 * padding) / 2);
    const aspect = data.ry / data.rx;
    const rx = maxR;
    const ry = maxR * Math.min(1, Math.max(0.3, aspect));

    const ellipse = document.createElementNS('http://www.w3.org/2000/svg', 'ellipse');
    ellipse.setAttribute('cx', cx);
    ellipse.setAttribute('cy', cy);
    ellipse.setAttribute('rx', rx);
    ellipse.setAttribute('ry', ry);
    ellipse.setAttribute('fill', 'url(#fluent2dGrad)');
    ellipse.setAttribute('stroke', '#0078D4');
    ellipse.setAttribute('stroke-width', '2.5');
    group.appendChild(ellipse);

    // Center dot & axes
    const center = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    center.setAttribute('cx', cx);
    center.setAttribute('cy', cy);
    center.setAttribute('r', '4');
    center.setAttribute('fill', '#60CDFF');
    group.appendChild(center);

    // Semi-major axis line
    const aLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    aLine.setAttribute('x1', cx);
    aLine.setAttribute('y1', cy);
    aLine.setAttribute('x2', cx + rx);
    aLine.setAttribute('y2', cy);
    aLine.setAttribute('stroke', '#60CDFF');
    aLine.setAttribute('stroke-width', '2');
    aLine.setAttribute('stroke-dasharray', '4 2');
    group.appendChild(aLine);

    // Semi-minor axis line
    const bLine = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    bLine.setAttribute('x1', cx);
    bLine.setAttribute('y1', cy);
    bLine.setAttribute('x2', cx);
    bLine.setAttribute('y2', cy - ry);
    bLine.setAttribute('stroke', '#34d399');
    bLine.setAttribute('stroke-width', '2');
    bLine.setAttribute('stroke-dasharray', '4 2');
    group.appendChild(bLine);
  }

  _renderSector(svg, group, data, width, height, padding) {
    const cx = width / 2;
    const cy = height / 2 + 30;
    const r = Math.min((width - 2 * padding) / 2, (height - 2 * padding) / 2);
    const theta = data.theta;

    // Angles
    const startAngle = -theta / 2 - Math.PI / 2;
    const endAngle = theta / 2 - Math.PI / 2;

    const x1 = cx + r * Math.cos(startAngle);
    const y1 = cy + r * Math.sin(startAngle);
    const x2 = cx + r * Math.cos(endAngle);
    const y2 = cy + r * Math.sin(endAngle);

    const largeArc = theta > Math.PI ? 1 : 0;
    const d = `M ${cx} ${cy} L ${x1} ${y1} A ${r} ${r} 0 ${largeArc} 1 ${x2} ${y2} Z`;

    const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
    path.setAttribute('d', d);
    path.setAttribute('fill', 'url(#fluent2dGrad)');
    path.setAttribute('stroke', '#0078D4');
    path.setAttribute('stroke-width', '2.5');
    group.appendChild(path);

    // Center dot
    const center = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    center.setAttribute('cx', cx);
    center.setAttribute('cy', cy);
    center.setAttribute('r', '4');
    center.setAttribute('fill', '#60CDFF');
    group.appendChild(center);

    // Chord line
    const chord = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    chord.setAttribute('x1', x1);
    chord.setAttribute('y1', y1);
    chord.setAttribute('x2', x2);
    chord.setAttribute('y2', y2);
    chord.setAttribute('stroke', '#f59e0b');
    chord.setAttribute('stroke-width', '1.5');
    chord.setAttribute('stroke-dasharray', '3 3');
    group.appendChild(chord);
  }

  _renderAnnulus(svg, group, data, width, height, padding) {
    const cx = width / 2;
    const cy = height / 2;
    const maxR = Math.min((width - 2 * padding) / 2, (height - 2 * padding) / 2);
    const rRatio = Math.max(0.1, Math.min(0.9, data.r / data.R));
    const rOut = maxR;
    const rIn = maxR * rRatio;

    // Outer circle
    const outer = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    outer.setAttribute('cx', cx);
    outer.setAttribute('cy', cy);
    outer.setAttribute('r', rOut);
    outer.setAttribute('fill', 'url(#fluent2dGrad)');
    outer.setAttribute('stroke', '#0078D4');
    outer.setAttribute('stroke-width', '2.5');
    group.appendChild(outer);

    // Inner hole circle
    const inner = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
    inner.setAttribute('cx', cx);
    inner.setAttribute('cy', cy);
    inner.setAttribute('r', rIn);
    inner.setAttribute('fill', 'var(--mica-bg, #1c1c1e)');
    inner.setAttribute('stroke', '#60CDFF');
    inner.setAttribute('stroke-width', '2');
    inner.setAttribute('stroke-dasharray', '4 2');
    group.appendChild(inner);
  }
}
