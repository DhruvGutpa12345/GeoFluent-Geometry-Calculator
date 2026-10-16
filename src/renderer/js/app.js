/**
 * GeoFluent Geometry Calculator Main Application Controller
 */

import { UNITS, UNIT_TYPES, toBase, fromBase, formatNumber, getUnit } from './math/units.js';
import { SHAPES_2D } from './math/formulas2d.js';
import { SHAPES_3D } from './math/formulas3d.js';
import { Visualizer2D } from './visualizers/visualizer2d.js';
import { Visualizer3D } from './visualizers/visualizer3d.js';
import { Store } from './store.js';

class App {
  constructor() {
    this.currentMode = '2d'; // '2d' | '3d'
    this.currentShapeList = SHAPES_2D;
    this.currentShape = SHAPES_2D[0];
    this.currentMethod = this.currentShape.methods[0];

    // State for viewing units on output
    this.viewUnits = {
      length: 'cm',
      area: 'cm2',
      volume: 'cm3',
      angle: 'deg',
      precision: 4
    };

    // Current input values & selected units
    // Map of inputId -> { val: number, unitId: string, type: string }
    this.inputState = {};

    // Current computation result
    this.latestCalculation = null;

    // Visualizers
    this.visualizer2D = null;
    this.visualizer3D = null;
    this.activeVisualizerType = '2d';

    this.init();
  }

  init() {
    this.setupWindowControls();
    this.setupNavigation();
    this.setupOutputUnitBar();
    this.setupVisualizers();
    this.setupUnitConverter();
    this.setupTheme();
    this.setupActionButtons();

    // Load initial shape
    this.setMode('2d');
  }

  setupWindowControls() {
    const minBtn = document.getElementById('btn-minimize');
    const maxBtn = document.getElementById('btn-maximize');
    const closeBtn = document.getElementById('btn-close');

    if (window.electronAPI) {
      if (minBtn) minBtn.addEventListener('click', () => window.electronAPI.minimizeWindow());
      if (maxBtn) maxBtn.addEventListener('click', () => window.electronAPI.maximizeWindow());
      if (closeBtn) closeBtn.addEventListener('click', () => window.electronAPI.closeWindow());
    }
  }

  setupNavigation() {
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
      item.addEventListener('click', () => {
        navItems.forEach(n => n.classList.remove('active'));
        item.classList.add('active');
        const tab = item.dataset.tab;
        this.switchView(tab);
      });
    });
  }

  switchView(tab) {
    const panels = ['view-calculator', 'view-converter', 'view-history', 'view-settings'];
    panels.forEach(p => {
      const el = document.getElementById(p);
      if (el) el.style.display = 'none';
    });

    if (tab === '2d' || tab === '3d') {
      document.getElementById('view-calculator').style.display = 'flex';
      this.setMode(tab);
    } else if (tab === 'converter') {
      document.getElementById('view-converter').style.display = 'block';
      this.updateConverterCalculation();
    } else if (tab === 'history') {
      document.getElementById('view-history').style.display = 'block';
      this.renderHistoryList();
    } else if (tab === 'settings') {
      document.getElementById('view-settings').style.display = 'block';
    }
  }

  setMode(mode) {
    this.currentMode = mode;
    this.currentShapeList = mode === '2d' ? SHAPES_2D : SHAPES_3D;
    this.currentShape = this.currentShapeList[0];
    this.currentMethod = this.currentShape.methods[0];

    // Toggle volume selector visibility on toolbar
    const volWrapper = document.getElementById('volume-unit-wrapper');
    if (volWrapper) {
      volWrapper.style.display = mode === '3d' ? 'flex' : 'none';
    }

    this.renderShapePills();
    this.renderShapeView();
  }

  setupOutputUnitBar() {
    // Populate length units
    const lenSelect = document.getElementById('view-unit-length');
    lenSelect.innerHTML = UNITS.length.map(u => `<option value="${u.id}">${u.symbol} (${u.name.split(' ')[0]})</option>`).join('');
    lenSelect.value = this.viewUnits.length;
    lenSelect.addEventListener('change', (e) => {
      this.viewUnits.length = e.target.value;
      this.recalculate();
    });

    // Populate area units
    const areaSelect = document.getElementById('view-unit-area');
    areaSelect.innerHTML = UNITS.area.map(u => `<option value="${u.id}">${u.symbol}</option>`).join('');
    areaSelect.value = this.viewUnits.area;
    areaSelect.addEventListener('change', (e) => {
      this.viewUnits.area = e.target.value;
      this.recalculate();
    });

    // Populate volume units
    const volSelect = document.getElementById('view-unit-volume');
    volSelect.innerHTML = UNITS.volume.map(u => `<option value="${u.id}">${u.symbol}</option>`).join('');
    volSelect.value = this.viewUnits.volume;
    volSelect.addEventListener('change', (e) => {
      this.viewUnits.volume = e.target.value;
      this.recalculate();
    });

    // Populate angle units
    const angleSelect = document.getElementById('view-unit-angle');
    angleSelect.innerHTML = UNITS.angle.map(u => `<option value="${u.id}">${u.symbol} (${u.name.split(' ')[0]})</option>`).join('');
    angleSelect.value = this.viewUnits.angle;
    angleSelect.addEventListener('change', (e) => {
      this.viewUnits.angle = e.target.value;
      this.recalculate();
    });

    // Precision select
    const precSelect = document.getElementById('view-precision');
    precSelect.value = this.viewUnits.precision;
    precSelect.addEventListener('change', (e) => {
      this.viewUnits.precision = parseInt(e.target.value, 10);
      this.recalculate();
    });
  }

  renderShapePills() {
    const container = document.getElementById('shapes-pill-container');
    container.innerHTML = '';

    this.currentShapeList.forEach(shape => {
      const pill = document.createElement('div');
      pill.className = `shape-pill ${shape.id === this.currentShape.id ? 'active' : ''}`;
      pill.innerHTML = `<span>${shape.name}</span>`;
      pill.addEventListener('click', () => {
        this.selectShape(shape);
      });
      container.appendChild(pill);
    });
  }

  selectShape(shape) {
    this.currentShape = shape;
    this.currentMethod = shape.methods[0];

    // Update active pill
    const pills = document.querySelectorAll('.shape-pill');
    pills.forEach((p, idx) => {
      if (this.currentShapeList[idx].id === shape.id) {
        p.classList.add('active');
        p.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
      } else {
        p.classList.remove('active');
      }
    });

    this.renderShapeView();
  }

  renderShapeView() {
    document.getElementById('shape-heading').textContent = this.currentShape.name;
    document.getElementById('shape-desc').textContent = this.currentShape.description;

    // Populate Methods dropdown
    const methodSelect = document.getElementById('method-selector');
    methodSelect.innerHTML = this.currentShape.methods.map((m, idx) =>
      `<option value="${idx}">${m.name}</option>`
    ).join('');
    methodSelect.value = '0';

    const descEl = document.getElementById('method-desc');
    descEl.textContent = this.currentMethod.description || '';

    methodSelect.onchange = (e) => {
      const idx = parseInt(e.target.value, 10);
      this.currentMethod = this.currentShape.methods[idx];
      descEl.textContent = this.currentMethod.description || '';
      this.renderInputs();
    };

    this.renderInputs();
  }

  renderInputs() {
    const container = document.getElementById('inputs-container');
    container.innerHTML = '';
    this.inputState = {};

    this.currentMethod.inputs.forEach(inp => {
      // Pick initial unit
      let initialUnit = inp.type === UNIT_TYPES.LENGTH ? this.viewUnits.length :
                        inp.type === UNIT_TYPES.AREA ? this.viewUnits.area :
                        inp.type === UNIT_TYPES.VOLUME ? this.viewUnits.volume :
                        inp.type === UNIT_TYPES.ANGLE ? this.viewUnits.angle : null;

      // In case default unit is in radians, convert initial value if default is deg
      let initialDisplayVal = inp.default;
      if (inp.type === UNIT_TYPES.ANGLE && initialUnit === 'deg') {
        initialDisplayVal = (inp.default * 180) / Math.PI;
      }

      this.inputState[inp.id] = {
        val: initialDisplayVal,
        unitId: initialUnit,
        type: inp.type,
        min: inp.min,
        max: inp.max
      };

      const group = document.createElement('div');
      group.className = 'param-input-group';

      const label = document.createElement('label');
      label.className = 'input-label';
      label.textContent = inp.label;
      group.appendChild(label);

      const inputWrap = document.createElement('div');
      inputWrap.className = 'input-with-unit';

      const input = document.createElement('input');
      input.type = 'number';
      input.className = 'fluent-input';
      input.value = initialDisplayVal;
      input.step = inp.type === UNIT_TYPES.COUNT ? '1' : 'any';
      if (inp.min !== undefined) input.min = inp.min;
      if (inp.max !== undefined) input.max = inp.max;

      input.addEventListener('input', (e) => {
        const val = parseFloat(e.target.value);
        this.inputState[inp.id].val = val;
        this.recalculate();
      });

      inputWrap.appendChild(input);

      // Unit selector dropdown if applicable
      if (inp.type !== UNIT_TYPES.COUNT && UNITS[inp.type]) {
        const unitSelect = document.createElement('select');
        unitSelect.className = 'unit-select';
        unitSelect.innerHTML = UNITS[inp.type].map(u => `<option value="${u.id}">${u.symbol}</option>`).join('');
        unitSelect.value = initialUnit;

        unitSelect.addEventListener('change', (e) => {
          const oldUnit = this.inputState[inp.id].unitId;
          const newUnit = e.target.value;
          this.inputState[inp.id].unitId = newUnit;

          // Convert current value so magnitude is preserved in physical reality
          const curVal = this.inputState[inp.id].val;
          if (!isNaN(curVal)) {
            const base = toBase(curVal, oldUnit, inp.type);
            const converted = fromBase(base, newUnit, inp.type);
            const rounded = Math.round(converted * 1e6) / 1e6;
            this.inputState[inp.id].val = rounded;
            input.value = rounded;
          }
          this.recalculate();
        });

        inputWrap.appendChild(unitSelect);
      }

      group.appendChild(inputWrap);
      container.appendChild(group);
    });

    this.recalculate();
  }

  recalculate() {
    const errorEl = document.getElementById('calc-error');
    if (errorEl) errorEl.remove();

    // Convert inputs to base units
    const baseInputs = {};
    let hasInvalid = false;

    for (const [id, state] of Object.entries(this.inputState)) {
      if (isNaN(state.val) || state.val === null) {
        hasInvalid = true;
        break;
      }
      baseInputs[id] = toBase(state.val, state.unitId, state.type);
    }

    if (hasInvalid) {
      this.renderOutputs({ outputs: [], steps: [] });
      return;
    }

    try {
      const result = this.currentMethod.calculate(baseInputs);
      this.latestCalculation = {
        shapeId: this.currentShape.id,
        shapeName: this.currentShape.name,
        methodId: this.currentMethod.id,
        methodName: this.currentMethod.name,
        inputs: Object.entries(this.inputState).map(([k, s]) => {
          const inpDef = this.currentMethod.inputs.find(i => i.id === k);
          const unitObj = s.type !== UNIT_TYPES.COUNT ? getUnit(s.unitId, s.type) : null;
          return {
            label: inpDef ? inpDef.label : k,
            value: s.val,
            unitSymbol: unitObj ? unitObj.symbol : ''
          };
        }),
        result
      };

      this.renderOutputs(result);
      this.updateVisualizer(result);
    } catch (err) {
      this.renderError(err.message);
    }
  }

  renderError(msg) {
    const heroContainer = document.getElementById('hero-results');
    heroContainer.innerHTML = `
      <div style="grid-column: 1 / -1; padding: 14px; background: rgba(239, 68, 68, 0.15); border: 1px solid rgba(239, 68, 68, 0.3); border-radius: var(--radius-md); color: #fca5a5; font-size: 13.5px; display: flex; align-items: center; gap: 8px;">
        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><line x1="12" y1="8" x2="12" y2="12"/><line x1="12" y1="16" x2="12.01" y2="16"/></svg>
        <span>${msg}</span>
      </div>
    `;
    document.getElementById('all-results-container').innerHTML = '';
    document.getElementById('steps-container').innerHTML = '';
  }

  renderOutputs(result) {
    const heroContainer = document.getElementById('hero-results');
    const allResultsContainer = document.getElementById('all-results-container');
    const stepsContainer = document.getElementById('steps-container');

    heroContainer.innerHTML = '';
    allResultsContainer.innerHTML = '';
    stepsContainer.innerHTML = '';

    if (!result || !result.outputs) return;

    // Format all outputs into target viewing units
    const formattedOutputs = result.outputs.map(out => {
      if (out.textValue) {
        return { ...out, displayVal: out.textValue, unitSymbol: '' };
      }
      const targetUnitId = out.type === UNIT_TYPES.LENGTH ? this.viewUnits.length :
                           out.type === UNIT_TYPES.AREA ? this.viewUnits.area :
                           out.type === UNIT_TYPES.VOLUME ? this.viewUnits.volume :
                           out.type === UNIT_TYPES.ANGLE ? this.viewUnits.angle : null;

      const convertedVal = fromBase(out.value, targetUnitId, out.type);
      const unitObj = getUnit(targetUnitId, out.type);
      const displayVal = formatNumber(convertedVal, this.viewUnits.precision);
      const unitSymbol = unitObj ? unitObj.symbol : '';

      return {
        ...out,
        displayVal,
        unitSymbol,
        convertedVal
      };
    });

    // 1. Hero Badges (Primary metrics e.g. Area, Perimeter, Volume, Surface Area)
    const primaries = formattedOutputs.filter(o => o.isPrimary);
    primaries.forEach(p => {
      const badge = document.createElement('div');
      badge.className = 'result-hero-badge';
      badge.innerHTML = `
        <span class="result-hero-label">${p.label}</span>
        <div class="result-hero-value">
          <span>${p.displayVal}</span>
          <span class="result-hero-unit">${p.unitSymbol}</span>
        </div>
      `;
      heroContainer.appendChild(badge);
    });

    // 2. Full Derived Parameters List
    formattedOutputs.forEach(out => {
      const row = document.createElement('div');
      row.className = 'result-row';
      row.innerHTML = `
        <span class="result-label">${out.label} (${out.symbol})</span>
        <span class="result-val">${out.displayVal} ${out.unitSymbol}</span>
      `;
      allResultsContainer.appendChild(row);
    });

    // 3. Step-by-Step Derivations
    if (result.steps && result.steps.length > 0) {
      result.steps.forEach((step, idx) => {
        const item = document.createElement('div');
        item.className = 'step-item';
        item.innerHTML = `
          <div class="step-title">${idx + 1}. ${step.title}</div>
          <div class="step-formula">Formula: ${step.formula}</div>
          <div class="step-subst">${step.substitution}</div>
        `;
        stepsContainer.appendChild(item);
      });
    }
  }

  setupVisualizers() {
    const container = document.getElementById('visualizer-container');
    const toolbar = document.getElementById('visualizer-toolbar');

    this.visualizer2D = new Visualizer2D(container);
    this.visualizer3D = new Visualizer3D(container);

    // Initial toolbar setup
    this.updateVisualizerToolbar();
  }

  updateVisualizerToolbar() {
    const toolbar = document.getElementById('visualizer-toolbar');
    toolbar.innerHTML = '';

    if (this.currentMode === '2d') {
      const guideBtn = document.createElement('button');
      guideBtn.className = 'fluent-icon-btn active';
      guideBtn.innerHTML = `<span>Guides & Circles</span>`;
      let guidesActive = true;
      guideBtn.onclick = () => {
        guidesActive = !guidesActive;
        guideBtn.classList.toggle('active', guidesActive);
        this.visualizer2D.setShowGuides(guidesActive);
      };
      toolbar.appendChild(guideBtn);
    } else {
      // 3D Toolbar
      const autoRotBtn = document.createElement('button');
      autoRotBtn.className = 'fluent-icon-btn active';
      autoRotBtn.innerHTML = `<span>Auto-Rotate</span>`;
      let autoRot = true;
      autoRotBtn.onclick = () => {
        autoRot = !autoRot;
        autoRotBtn.classList.toggle('active', autoRot);
        this.visualizer3D.setAutoRotate(autoRot);
      };
      toolbar.appendChild(autoRotBtn);

      const wireBtn = document.createElement('button');
      wireBtn.className = 'fluent-icon-btn active';
      wireBtn.innerHTML = `<span>Wireframe</span>`;
      let wire = true;
      wireBtn.onclick = () => {
        wire = !wire;
        wireBtn.classList.toggle('active', wire);
        this.visualizer3D.setWireframe(wire);
      };
      toolbar.appendChild(wireBtn);

      const resetBtn = document.createElement('button');
      resetBtn.className = 'fluent-icon-btn';
      resetBtn.innerHTML = `<span>Reset View</span>`;
      resetBtn.onclick = () => {
        this.visualizer3D.resetView();
      };
      toolbar.appendChild(resetBtn);
    }
  }

  updateVisualizer(result) {
    const titleEl = document.getElementById('visualizer-title');
    this.updateVisualizerToolbar();

    if (this.currentMode === '2d') {
      titleEl.textContent = 'Live 2D Diagram & Scaling';
      if (this.visualizer3D.renderer && this.visualizer3D.renderer.domElement.parentNode) {
        this.visualizer3D.renderer.domElement.style.display = 'none';
      }
      this.visualizer2D.render(result.svgData);
    } else {
      titleEl.textContent = 'Interactive 3D Solid (Drag to Rotate, Scroll to Zoom)';
      const container = document.getElementById('visualizer-container');
      const svg = container.querySelector('svg');
      if (svg) svg.remove();

      if (this.visualizer3D.renderer) {
        this.visualizer3D.renderer.domElement.style.display = 'block';
      }
      this.visualizer3D.render(result.threeData);
    }
  }

  setupUnitConverter() {
    const inputVal = document.getElementById('conv-input-val');
    const fromUnitSelect = document.getElementById('conv-from-unit');
    const outputVal = document.getElementById('conv-output-val');
    const toUnitSelect = document.getElementById('conv-to-unit');

    let currentConvType = 'length';

    const populateUnits = (type) => {
      currentConvType = type;
      const list = UNITS[type];
      fromUnitSelect.innerHTML = list.map(u => `<option value="${u.id}">${u.name}</option>`).join('');
      toUnitSelect.innerHTML = list.map(u => `<option value="${u.id}">${u.name}</option>`).join('');

      fromUnitSelect.value = list[0].id;
      toUnitSelect.value = list.length > 1 ? list[1].id : list[0].id;
      this.updateConverterCalculation();
    };

    const typeBtns = document.querySelectorAll('[data-conv-type]');
    typeBtns.forEach(btn => {
      btn.addEventListener('click', () => {
        typeBtns.forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        populateUnits(btn.dataset.convType);
      });
    });

    populateUnits('length');

    inputVal.addEventListener('input', () => this.updateConverterCalculation());
    fromUnitSelect.addEventListener('change', () => this.updateConverterCalculation());
    toUnitSelect.addEventListener('change', () => this.updateConverterCalculation());
  }

  updateConverterCalculation() {
    const inputVal = document.getElementById('conv-input-val');
    const fromUnitSelect = document.getElementById('conv-from-unit');
    const outputVal = document.getElementById('conv-output-val');
    const toUnitSelect = document.getElementById('conv-to-unit');
    const activeTypeBtn = document.querySelector('[data-conv-type].active');
    const type = activeTypeBtn ? activeTypeBtn.dataset.convType : 'length';

    const val = parseFloat(inputVal.value);
    if (isNaN(val)) {
      outputVal.value = '—';
      return;
    }

    const base = toBase(val, fromUnitSelect.value, type);
    const converted = fromBase(base, toUnitSelect.value, type);
    outputVal.value = formatNumber(converted, 6);
  }

  setupTheme() {
    const themeBtns = document.querySelectorAll('[data-theme-btn]');
    themeBtns.forEach(btn => {
      btn.addEventListener('click', () => {
        themeBtns.forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        const theme = btn.dataset.themeBtn;
        document.body.dataset.theme = theme;
        this.showToast(`Switched to ${theme} theme`);
      });
    });
  }

  setupActionButtons() {
    // Copy full report
    const copyBtn = document.getElementById('btn-copy-report');
    if (copyBtn) {
      copyBtn.addEventListener('click', () => {
        if (!this.latestCalculation) {
          this.showToast('No active calculation to copy.');
          return;
        }
        const md = Store.exportToMarkdown({
          shapeName: this.latestCalculation.shapeName,
          methodName: this.latestCalculation.methodName,
          timestamp: new Date().toISOString(),
          inputs: this.latestCalculation.inputs,
          outputs: this.latestCalculation.result.outputs.map(out => {
            const targetUnitId = out.type === UNIT_TYPES.LENGTH ? this.viewUnits.length :
                                 out.type === UNIT_TYPES.AREA ? this.viewUnits.area :
                                 out.type === UNIT_TYPES.VOLUME ? this.viewUnits.volume :
                                 out.type === UNIT_TYPES.ANGLE ? this.viewUnits.angle : null;
            const converted = fromBase(out.value, targetUnitId, out.type);
            const unit = getUnit(targetUnitId, out.type);
            return {
              label: out.label,
              value: out.textValue || formatNumber(converted, this.viewUnits.precision),
              unitSymbol: unit ? unit.symbol : ''
            };
          }),
          steps: this.latestCalculation.result.steps
        });

        navigator.clipboard.writeText(md).then(() => {
          this.showToast('Report copied to clipboard as Markdown!');
        }).catch(() => {
          this.showToast('Failed to copy to clipboard.');
        });
      });
    }

    // Save calculation
    const saveBtn = document.getElementById('btn-save-calc');
    if (saveBtn) {
      saveBtn.addEventListener('click', () => {
        if (!this.latestCalculation) {
          this.showToast('No active calculation to save.');
          return;
        }
        Store.addHistory({
          shapeName: this.latestCalculation.shapeName,
          methodName: this.latestCalculation.methodName,
          inputs: this.latestCalculation.inputs,
          outputs: this.latestCalculation.result.outputs.map(out => {
            const targetUnitId = out.type === UNIT_TYPES.LENGTH ? this.viewUnits.length :
                                 out.type === UNIT_TYPES.AREA ? this.viewUnits.area :
                                 out.type === UNIT_TYPES.VOLUME ? this.viewUnits.volume :
                                 out.type === UNIT_TYPES.ANGLE ? this.viewUnits.angle : null;
            const converted = fromBase(out.value, targetUnitId, out.type);
            const unit = getUnit(targetUnitId, out.type);
            return {
              label: out.label,
              value: out.textValue || formatNumber(converted, this.viewUnits.precision),
              unitSymbol: unit ? unit.symbol : ''
            };
          }),
          steps: this.latestCalculation.result.steps
        });
        this.showToast('Calculation saved to History!');
      });
    }

    // Clear history
    const clearHistBtn = document.getElementById('btn-clear-history');
    if (clearHistBtn) {
      clearHistBtn.addEventListener('click', () => {
        Store.clearHistory();
        this.renderHistoryList();
        this.showToast('History cleared.');
      });
    }
  }

  renderHistoryList() {
    const container = document.getElementById('history-list-container');
    const items = Store.getHistory();
    container.innerHTML = '';

    if (items.length === 0) {
      container.innerHTML = `
        <div style="padding: 24px; text-align: center; color: var(--text-tertiary); font-size: 14px;">
          No saved calculations yet. Compute a shape and click "Save" to keep records here.
        </div>
      `;
      return;
    }

    items.forEach(item => {
      const card = document.createElement('div');
      card.className = 'fluent-card';
      card.innerHTML = `
        <div class="fluent-card-header">
          <div>
            <div style="font-size: 16px; font-weight: 700; color: var(--accent-light);">${item.shapeName}</div>
            <div style="font-size: 12px; color: var(--text-tertiary);">${new Date(item.timestamp).toLocaleString()} • ${item.methodName}</div>
          </div>
          <button class="fluent-icon-btn" data-delete-id="${item.id}" title="Delete">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>
          </button>
        </div>
        <div style="display: flex; flex-wrap: wrap; gap: 8px; font-size: 12.5px;">
          ${item.inputs.map(i => `<span style="background: rgba(255,255,255,0.06); padding: 4px 8px; border-radius: 4px;"><strong>${i.label}:</strong> ${i.value} ${i.unitSymbol}</span>`).join('')}
        </div>
        <div style="display: flex; flex-wrap: wrap; gap: 8px; font-size: 12.5px; border-top: 1px solid var(--border-subtle); padding-top: 8px;">
          ${item.outputs.slice(0, 4).map(o => `<span style="color: #34d399;"><strong>${o.label}:</strong> ${o.value} ${o.unitSymbol}</span>`).join('')}
        </div>
      `;

      card.querySelector(`[data-delete-id="${item.id}"]`).addEventListener('click', () => {
        Store.deleteHistoryItem(item.id);
        this.renderHistoryList();
      });

      container.appendChild(card);
    });
  }

  showToast(message) {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.innerHTML = `
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#60CDFF" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>
      <span>${message}</span>
    `;
    container.appendChild(toast);
    setTimeout(() => {
      toast.style.opacity = '0';
      toast.style.transform = 'translateY(16px)';
      toast.style.transition = 'all 0.25s ease';
      setTimeout(() => toast.remove(), 250);
    }, 2800);
  }
}

// Instantiate immediately if DOM already loaded, or wait for DOMContentLoaded
console.log('app.js evaluated, document.readyState:', document.readyState);
if (document.readyState === 'loading') {
  window.addEventListener('DOMContentLoaded', () => {
    console.log('DOMContentLoaded fired, instantiating App');
    new App();
  });
} else {
  console.log('DOM already loaded, instantiating App immediately');
  new App();
}

