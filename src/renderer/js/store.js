/**
 * GeoFluent History & Storage Engine
 * Manages history of calculations, preferences, and export formats.
 */

const HISTORY_KEY = 'geofluent_calculation_history';
const SETTINGS_KEY = 'geofluent_user_settings';

export const Store = {
  getSettings() {
    try {
      const s = localStorage.getItem(SETTINGS_KEY);
      return s ? JSON.parse(s) : {
        theme: 'system',
        defaultLengthUnit: 'cm',
        defaultAreaUnit: 'cm2',
        defaultVolumeUnit: 'cm3',
        defaultAngleUnit: 'deg',
        precision: 4
      };
    } catch {
      return { theme: 'system', defaultLengthUnit: 'cm', defaultAreaUnit: 'cm2', defaultVolumeUnit: 'cm3', defaultAngleUnit: 'deg', precision: 4 };
    }
  },

  saveSettings(settings) {
    try {
      localStorage.setItem(SETTINGS_KEY, JSON.stringify(settings));
    } catch (e) {
      console.error('Failed to save settings', e);
    }
  },

  getHistory() {
    try {
      const h = localStorage.getItem(HISTORY_KEY);
      return h ? JSON.parse(h) : [];
    } catch {
      return [];
    }
  },

  addHistory(entry) {
    try {
      const history = this.getHistory();
      const newEntry = {
        id: Date.now().toString(),
        timestamp: new Date().toISOString(),
        ...entry
      };
      // Keep up to 50 recent items
      history.unshift(newEntry);
      if (history.length > 50) history.pop();
      localStorage.setItem(HISTORY_KEY, JSON.stringify(history));
      return newEntry;
    } catch (e) {
      console.error('Failed to save history', e);
    }
  },

  deleteHistoryItem(id) {
    try {
      const history = this.getHistory().filter(item => item.id !== id);
      localStorage.setItem(HISTORY_KEY, JSON.stringify(history));
    } catch (e) {
      console.error('Failed to delete item', e);
    }
  },

  clearHistory() {
    try {
      localStorage.removeItem(HISTORY_KEY);
    } catch (e) {
      console.error('Failed to clear history', e);
    }
  },

  exportToMarkdown(entry) {
    let md = `# GeoFluent Geometry Report: ${entry.shapeName}\n\n`;
    md += `**Date**: ${new Date(entry.timestamp).toLocaleString()}\n`;
    md += `**Method**: ${entry.methodName}\n\n`;
    md += `## Input Parameters\n\n| Parameter | Value | Unit |\n|---|---|---|\n`;
    entry.inputs.forEach(inp => {
      md += `| ${inp.label} | ${inp.value} | ${inp.unitSymbol} |\n`;
    });
    md += `\n## Calculated Results\n\n| Property | Value | Unit |\n|---|---|---|\n`;
    entry.outputs.forEach(out => {
      md += `| ${out.label} | ${out.formattedValue || out.value} | ${out.unitSymbol || ''} |\n`;
    });
    if (entry.steps && entry.steps.length > 0) {
      md += `\n## Step-by-Step Derivation\n\n`;
      entry.steps.forEach((step, idx) => {
        md += `### ${idx + 1}. ${step.title}\n`;
        md += `- Formula: \`${step.formula}\`\n`;
        md += `- Substitution: ${step.substitution}\n\n`;
      });
    }
    return md;
  }
};
