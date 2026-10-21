<#
.SYNOPSIS
    Installs GeoFluent Geometry Calculator MSIX package and trusts its developer certificate.
.DESCRIPTION
    Fixes Windows Error 0x800B010A (CERT_E_CHAINING) by adding the publisher's certificate
    to the TrustedPeople store, then seamlessly installs the MSIX package.
#>

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "       GeoFluent Windows 11 MSIX Package Installer        " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host ""

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$rootDir = Split-Path -Parent $scriptDir

# 1. Locate certificate
$certPath = Join-Path $rootDir "certs\GeoFluentDev.cer"
if (-not (Test-Path $certPath)) {
    $certPath = Join-Path $scriptDir "GeoFluentDev.cer"
}

if (-not (Test-Path $certPath)) {
    Write-Host "[!] Warning: GeoFluentDev.cer not found." -ForegroundColor Yellow
} else {
    Write-Host "[1/2] Installing Publisher Certificate into TrustedPeople store..." -ForegroundColor White
    try {
        # Fallback to CurrentUser (no elevation required)
        Import-Certificate -FilePath $certPath -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
        Write-Host "      Installed to CurrentUser\TrustedPeople store." -ForegroundColor Green
    } catch {
        Write-Warning "Failed to import certificate: $_"
    }
}

# 2. Locate MSIX package
$msix = Join-Path $rootDir "dist\msix\GeoFluent.msix"
if (-not (Test-Path $msix)) {
    $msix = (Get-ChildItem -Path (Join-Path $rootDir "dist\msix") -Filter "*.msix" -Recurse | Select-Object -First 1).FullName
}

if (-not $msix -or -not (Test-Path $msix)) {
    Write-Host "[!] Error: No .msix package found in dist/msix/" -ForegroundColor Red
    exit 1
}

Write-Host "[2/2] Installing package: $(Split-Path -Leaf $msix)..." -ForegroundColor White
Add-AppxPackage -Path $msix -ForceApplicationShutdown

Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "   SUCCESS! GeoFluent Geometry Calculator is installed!   " -ForegroundColor Green
Write-Host "   You can now launch it from your Windows Start Menu.   " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
Write-Host ""
Start-Sleep -Seconds 2
