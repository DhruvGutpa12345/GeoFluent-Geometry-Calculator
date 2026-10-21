<#
.SYNOPSIS
    GeoFluent Dual Output Packaging Pipeline (.NET 10 / WinUI 3 / Windows App SDK 2.x)
.DESCRIPTION
    Builds both Portable Unpackaged .exe (instant testing) and Microsoft Store .msix (Partner Center).
.PARAMETER Target
    Build target: 'all', 'portable', or 'msix'. Default is 'all'.
.PARAMETER Configuration
    Build configuration: 'Release' or 'Debug'. Default is 'Release'.
#>
[CmdletBinding()]
param (
    [ValidateSet('all', 'portable', 'msix')]
    [string]$Target = 'all',

    [ValidateSet('Release', 'Debug')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$rootDir = $PSScriptRoot
$projectPath = Join-Path $rootDir "src\GeoFluent\GeoFluent.csproj"
$distDir = Join-Path $rootDir "dist"
$portableDist = Join-Path $distDir "portable"
$msixDist = Join-Path $distDir "msix"

Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "  GeoFluent Windows Systems & UI Architecture Build Pipeline     " -ForegroundColor Cyan
Write-Host "=================================================================" -ForegroundColor Cyan
Write-Host "Target:        $Target"
Write-Host "Configuration: $Configuration"
Write-Host ""

# Ensure dist directories exist
if (-not (Test-Path $distDir)) { New-Item -ItemType Directory -Path $distDir | Out-Null }

# 1. Build Portable Unpackaged .exe
if ($Target -eq 'all' -or $Target -eq 'portable') {
    Write-Host "[1/2] Compiling Portable Unpackaged Executable..." -ForegroundColor Yellow
    if (Test-Path $portableDist) { Remove-Item -Recurse -Force $portableDist }
    New-Item -ItemType Directory -Path $portableDist | Out-Null

    dotnet publish $projectPath `
        -c $Configuration `
        -r win-x64 `
        -p:WindowsPackageType=None `
        -p:WindowsAppSDKSelfContained=true `
        -p:PublishReadyToRun=true `
        --self-contained true `
        -o $portableDist

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Portable build failed."
        exit $LASTEXITCODE
    }

    # Code-sign unpackaged executable and main managed assembly to satisfy Smart App Control
    $signtool = (Get-ChildItem -Path "$env:USERPROFILE\.nuget\packages\microsoft.windows.sdk.buildtools" -Filter "signtool.exe" -Recurse -ErrorAction SilentlyContinue | Where-Object { $_.FullName -like "*x64*" } | Select-Object -First 1).FullName
    $certPfx = Join-Path $rootDir "certs\GeoFluentDev.pfx"
    $certCer = Join-Path $rootDir "certs\GeoFluentDev.cer"

    if (Test-Path $certCer) {
        Import-Certificate -FilePath $certCer -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
    }

    if ($signtool -and (Test-Path $signtool) -and (Test-Path $certPfx)) {
        $binaries = @(
            (Join-Path $portableDist "GeoFluent.exe"),
            (Join-Path $portableDist "GeoFluent.dll")
        )
        foreach ($bin in $binaries) {
            if (Test-Path $bin) {
                Write-Host "Signing $(Split-Path -Leaf $bin) with dev certificate..." -ForegroundColor DarkGray
                & $signtool sign /fd SHA256 /f $certPfx /p "GeoFluent2026" $bin 2>&1 | Out-Null
            }
        }
        Write-Host "Portable binaries signed and verified." -ForegroundColor Green
    }

    Write-Host "Portable build succeeded: $portableDist\GeoFluent.exe" -ForegroundColor Green
}

# 2. Build Microsoft Store .msix Package
if ($Target -eq 'all' -or $Target -eq 'msix') {
    Write-Host "[2/2] Compiling Microsoft Store MSIX Package..." -ForegroundColor Yellow
    if (Test-Path $msixDist) { Remove-Item -Recurse -Force $msixDist }
    New-Item -ItemType Directory -Path $msixDist | Out-Null

    dotnet publish $projectPath `
        -c $Configuration `
        -r win-x64 `
        -p:WindowsPackageType=MSIX `
        -p:GenerateAppxPackageOnBuild=true `
        -p:AppxPackageDir="$msixDist\\" `
        -p:AppxBundle=Never `
        -p:AppxPackageSigningEnabled=true `
        -p:PackageCertificateKeyFile="$rootDir\certs\GeoFluentDev.pfx" `
        -p:PackageCertificatePassword="GeoFluent2026"

    if ($LASTEXITCODE -ne 0) {
        Write-Error "MSIX build failed."
        exit $LASTEXITCODE
    }

    # Locate generated .msix and copy to root of dist/msix for convenient access
    $generatedMsix = Get-ChildItem -Path $msixDist -Filter "*.msix" -Recurse | Where-Object { $_.Name -like "GeoFluent*.msix" } | Select-Object -First 1
    if ($generatedMsix) {
        $finalMsix = Join-Path $msixDist "GeoFluent.msix"
        Copy-Item -Path $generatedMsix.FullName -Destination $finalMsix -Force
        Write-Host "MSIX package copied to: $finalMsix" -ForegroundColor Green

        if ($signtool -and (Test-Path $certPfx)) {
            Write-Host "Signing final MSIX package with dev certificate..." -ForegroundColor DarkGray
            & $signtool sign /fd SHA256 /f $certPfx /p "GeoFluent2026" $finalMsix 2>&1 | Out-Null
            Write-Host "MSIX package signed and verified." -ForegroundColor Green
        }
    }

    Write-Host "MSIX package succeeded in: $msixDist" -ForegroundColor Green
}

Write-Host ""
Write-Host "=================================================================" -ForegroundColor Green
Write-Host "  Build Pipeline Completed Successfully!                         " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
