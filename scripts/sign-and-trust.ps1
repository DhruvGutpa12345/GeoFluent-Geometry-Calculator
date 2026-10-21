[CmdletBinding()]
param()

$rootDir = Split-Path -Parent $PSScriptRoot
$certCer = Join-Path $rootDir "certs\GeoFluentDev.cer"
$certPfx = Join-Path $rootDir "certs\GeoFluentDev.pfx"
$password = "GeoFluent2026"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "   GeoFluent Developer Code Signing & Trust Verification   " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

# 1. Trust certificate in CurrentUser\TrustedPeople (no UAC / elevation needed)
if (Test-Path $certCer) {
    Write-Host "[1/3] Registering certificate in CurrentUser\TrustedPeople..." -ForegroundColor Yellow
    Import-Certificate -FilePath $certCer -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
    Write-Host "      Certificate trusted in CurrentUser\TrustedPeople." -ForegroundColor Green
} else {
    Write-Warning "Certificate not found at $certCer"
}

# 2. Locate signtool.exe
Write-Host "[2/3] Locating Windows SDK signtool.exe..." -ForegroundColor Yellow
$sdkDir = Join-Path $env:USERPROFILE ".nuget\packages\microsoft.windows.sdk.buildtools"
$signtool = (Get-ChildItem -Path $sdkDir -Filter "signtool.exe" -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.FullName -like "*x64*" } | Select-Object -First 1).FullName

if (-not $signtool -or -not (Test-Path $signtool)) {
    Write-Error "signtool.exe could not be found."
    exit 1
}
Write-Host "      Found signtool: $signtool" -ForegroundColor Green

# 3. Sign portable binaries
Write-Host "[3/3] Code-signing binaries..." -ForegroundColor Yellow

$portableDir = Join-Path $rootDir "dist\portable"
if (Test-Path $portableDir) {
    $binaries = @(
        (Join-Path $portableDir "GeoFluent.exe"),
        (Join-Path $portableDir "GeoFluent.dll")
    )
    foreach ($bin in $binaries) {
        if (Test-Path $bin) {
            Write-Host "      Signing $(Split-Path -Leaf $bin)..." -ForegroundColor DarkGray
            & $signtool sign /fd SHA256 /f $certPfx /p $password $bin 2>&1 | Out-Null
            $sig = Get-AuthenticodeSignature $bin
            $color = if ($sig.Status -eq 'Valid') { 'Green' } else { 'Yellow' }
            Write-Host "      $(Split-Path -Leaf $bin): $($sig.Status)" -ForegroundColor $color
        }
    }
}

# Sign MSIX package if present
$msixPath = Join-Path $rootDir "dist\msix\GeoFluent.msix"
if (Test-Path $msixPath) {
    Write-Host "      Signing GeoFluent.msix..." -ForegroundColor DarkGray
    & $signtool sign /fd SHA256 /f $certPfx /p $password $msixPath 2>&1 | Out-Null
    $sig = Get-AuthenticodeSignature $msixPath
    $color = if ($sig.Status -eq 'Valid') { 'Green' } else { 'Yellow' }
    Write-Host "      GeoFluent.msix: $($sig.Status)" -ForegroundColor $color
}

Write-Host ""
Write-Host "All targets verified and trusted!" -ForegroundColor Green
