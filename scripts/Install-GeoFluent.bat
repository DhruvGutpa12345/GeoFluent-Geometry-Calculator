@echo off
setlocal
cd /d "%~dp0"
title GeoFluent Windows 11 MSIX Installer

:: Check for Administrative privileges to trust the certificate in LocalMachine
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo Requesting Administrator privileges to trust the publisher certificate...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Start-Process cmd -ArgumentList '/c \"\"%~f0\"\"' -Verb RunAs"
    exit /b
)

echo ==========================================================
echo        GeoFluent Windows 11 MSIX Package Installer        
echo ==========================================================
echo.

if exist "GeoFluentDev.cer" (
    echo [1/2] Installing publisher certificate into LocalMachine\TrustedPeople store...
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Import-Certificate -FilePath '%~dp0GeoFluentDev.cer' -CertStoreLocation 'Cert:\LocalMachine\TrustedPeople' | Out-Null; Import-Certificate -FilePath '%~dp0GeoFluentDev.cer' -CertStoreLocation 'Cert:\CurrentUser\TrustedPeople' | Out-Null"
    echo       Certificate installed and trusted successfully.
) else (
    echo [!] GeoFluentDev.cer certificate not found in this folder.
)

echo [2/2] Installing GeoFluent MSIX package...
for %%f in (*.msix *.appx) do (
    powershell -NoProfile -ExecutionPolicy Bypass -Command "Add-AppxPackage -Path '%%~ff'"
    goto :done
)

echo [!] Error: No .msix package found in this directory.
pause
exit /b 1

:done
echo.
echo ==========================================================
echo    SUCCESS! GeoFluent Geometry Calculator is installed!   
echo ==========================================================
echo.
echo Launching GeoFluent...
powershell -NoProfile -ExecutionPolicy Bypass -Command "Get-AppxPackage *GeoFluent* | ForEach-Object { Start-Process ('shell:AppsFolder\' + $_.PackageFamilyName + '!GeoFluent') }" -ErrorAction SilentlyContinue
timeout /t 3
