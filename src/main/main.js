/**
 * GeoFluent Geometry Calculator - Electron Main Process
 * Optimized for Windows 11 Fluent 2 Design, Mica backdrop, and Microsoft Store packaging.
 */

const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');

let mainWindow = null;

function createWindow() {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 840,
    minWidth: 920,
    minHeight: 620,
    title: 'GeoFluent Geometry Calculator',
    icon: path.join(__dirname, '../../assets/icon.png'),
    frame: false,
    backgroundMaterial: 'mica', // Windows 11 native Mica backdrop
    titleBarStyle: 'hidden',
    backgroundColor: '#18191c',
    webPreferences: {
      preload: path.join(__dirname, 'preload.js'),
      nodeIntegration: false,
      contextIsolation: true,
      sandbox: false
    }
  });

  mainWindow.webContents.on('console-message', (event, level, message, line, sourceId) => {
    console.log(`[RENDERER LOG ${level}] ${message} (${sourceId}:${line})`);
  });

  mainWindow.loadFile(path.join(__dirname, '../renderer/index.html'));

  // Window IPC controls
  ipcMain.on('window-minimize', () => {
    if (mainWindow) mainWindow.minimize();
  });

  ipcMain.on('window-maximize', () => {
    if (mainWindow) {
      if (mainWindow.isMaximized()) mainWindow.unmaximize();
      else mainWindow.maximize();
    }
  });

  ipcMain.on('window-close', () => {
    if (mainWindow) mainWindow.close();
  });

  ipcMain.handle('window-is-maximized', () => {
    return mainWindow ? mainWindow.isMaximized() : false;
  });

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});
