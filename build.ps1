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
        -p:AppxPackageDir="$msixDist\" `
        -p:AppxBundle=Never `
        -p:UapAppxPackageBuildMode=StoreUpload

    if ($LASTEXITCODE -ne 0) {
        Write-Error "MSIX build failed."
        exit $LASTEXITCODE
    }
    Write-Host "MSIX package succeeded in: $msixDist" -ForegroundColor Green
}

Write-Host ""
Write-Host "=================================================================" -ForegroundColor Green
Write-Host "  Build Pipeline Completed Successfully!                         " -ForegroundColor Green
Write-Host "=================================================================" -ForegroundColor Green
