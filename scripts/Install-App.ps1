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

# 1. Locate certificate
$certPath = Join-Path $scriptDir "GeoFluentDev.cer"
if (-not (Test-Path $certPath)) {
    $certPath = Join-Path (Join-Path $scriptDir "certs") "GeoFluentDev.cer"
}

if (-not (Test-Path $certPath)) {
    Write-Host "[!] Warning: GeoFluentDev.cer not found in current directory." -ForegroundColor Yellow
} else {
    Write-Host "[1/2] Installing Publisher Certificate into TrustedPeople store..." -ForegroundColor White
    try {
        # First attempt LocalMachine (requires admin), then fallback to CurrentUser
        Import-Certificate -FilePath $certPath -CertStoreLocation "Cert:\LocalMachine\TrustedPeople" -ErrorAction SilentlyContinue | Out-Null
        Write-Host "      Installed to LocalMachine\TrustedPeople store." -ForegroundColor Green
    } catch {
        # Fallback to CurrentUser (no elevation required)
        Import-Certificate -FilePath $certPath -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
        Write-Host "      Installed to CurrentUser\TrustedPeople store." -ForegroundColor Green
    }
}

# 2. Locate MSIX package
$msix = Get-ChildItem -Path $scriptDir -Filter "*.msix" | Select-Object -First 1
if (-not $msix) {
    $msix = Get-ChildItem -Path $scriptDir -Filter "*.appx" | Select-Object -First 1
}

if (-not $msix) {
    Write-Host "[!] Error: No .msix or .appx package found in $scriptDir" -ForegroundColor Red
    Pause
    exit 1
}

Write-Host "[2/2] Installing package: $($msix.Name)..." -ForegroundColor White
Add-AppxPackage -Path $msix.FullName

Write-Host ""
Write-Host "==========================================================" -ForegroundColor Green
Write-Host "   SUCCESS! GeoFluent Geometry Calculator is installed!   " -ForegroundColor Green
Write-Host "   You can now launch it from your Windows Start Menu.   " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
Write-Host ""
Start-Sleep -Seconds 2
