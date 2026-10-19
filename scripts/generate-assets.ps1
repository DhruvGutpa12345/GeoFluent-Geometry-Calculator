Add-Type -AssemblyName System.Drawing

$srcPath = Resolve-Path "assets/icon.png"
$src = [System.Drawing.Image]::FromFile($srcPath)

$destDirs = @("build/appx", "assets/store", "assets/appx")
foreach ($dir in $destDirs) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
    }
}

function Save-ResizedSquare($size, $filename) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImage($src, 0, 0, $size, $size)
    $g.Dispose()
    
    foreach ($dir in $destDirs) {
        $outPath = Join-Path $dir $filename
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    $bmp.Dispose()
    Write-Host "Generated $filename ($size x $size)"
}

function Save-WideTile($width, $height, $filename) {
    $bmp = New-Object System.Drawing.Bitmap $width, $height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    
    # Background fill matching tile theme
    $bgColor = [System.Drawing.ColorTranslator]::FromHtml("#13171f")
    $g.Clear($bgColor)
    
    # Draw centered icon with padding
    $iconSize = [int]($height * 0.86)
    $x = [int](($width - $iconSize) / 2)
    $y = [int](($height - $iconSize) / 2)
    $g.DrawImage($src, $x, $y, $iconSize, $iconSize)
    $g.Dispose()

    foreach ($dir in $destDirs) {
        $outPath = Join-Path $dir $filename
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    $bmp.Dispose()
    Write-Host "Generated $filename ($width x $height)"
}

function Save-SplashScreen($width, $height, $filename) {
    $bmp = New-Object System.Drawing.Bitmap $width, $height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    
    $bgColor = [System.Drawing.ColorTranslator]::FromHtml("#13171f")
    $g.Clear($bgColor)
    
    $iconSize = 180
    $x = [int](($width - $iconSize) / 2)
    $y = [int](($height - $iconSize) / 2)
    $g.DrawImage($src, $x, $y, $iconSize, $iconSize)
    $g.Dispose()

    foreach ($dir in $destDirs) {
        $outPath = Join-Path $dir $filename
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    }
    $bmp.Dispose()
    Write-Host "Generated $filename ($width x $height)"
}

# 1. Standard Square Logos
Save-ResizedSquare 150 "Square150x150Logo.png"
Save-ResizedSquare 44  "Square44x44Logo.png"
Save-ResizedSquare 50  "StoreLogo.png"
Save-ResizedSquare 310 "Square310x310Logo.png"
Save-ResizedSquare 310 "LargeTile.png"
Save-ResizedSquare 71  "Square71x71Logo.png"
Save-ResizedSquare 71  "SmallTile.png"
Save-ResizedSquare 24  "BadgeLogo.png"

# 2. Wide and Splash Tiles
Save-WideTile 310 150 "Wide310x150Logo.png"
Save-SplashScreen 620 300 "SplashScreen.png"

$src.Dispose()

# 3. Create Windows Multi-Size ICO File (16, 32, 48, 64, 128, 256)
$icoSizes = @(16, 32, 48, 64, 128, 256)
$pngStreams = @()
$srcPng = [System.Drawing.Image]::FromFile($srcPath)

foreach ($size in $icoSizes) {
    $bmp = New-Object System.Drawing.Bitmap $size, $size
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImage($srcPng, 0, 0, $size, $size)
    $g.Dispose()
    
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $pngStreams += @{ Size = $size; Bytes = $ms.ToArray() }
    $ms.Dispose()
}
$srcPng.Dispose()

$icoFile = [System.IO.File]::Create("assets/icon.ico")
$bw = New-Object System.IO.BinaryWriter($icoFile)

# ICONDIR header
$bw.Write([uint16]0) # Reserved
$bw.Write([uint16]1) # Type (1 for ICO)
$bw.Write([uint16]$icoSizes.Count) # Image count

$dataOffset = 6 + ($icoSizes.Count * 16)

# ICONDIRENTRY for each size
foreach ($entry in $pngStreams) {
    $size = $entry.Size
    $w = if ($size -ge 256) { 0 } else { [byte]$size }
    $h = if ($size -ge 256) { 0 } else { [byte]$size }
    $bw.Write([byte]$w)
    $bw.Write([byte]$h)
    $bw.Write([byte]0)   # Color count
    $bw.Write([byte]0)   # Reserved
    $bw.Write([uint16]1) # Color planes
    $bw.Write([uint16]32) # Bits per pixel
    $bw.Write([uint32]$entry.Bytes.Length) # Image size in bytes
    $bw.Write([uint32]$dataOffset)         # Image offset
    $dataOffset += $entry.Bytes.Length
}

# Image data
foreach ($entry in $pngStreams) {
    $bw.Write($entry.Bytes)
}

$bw.Flush()
$bw.Close()
$icoFile.Close()
Write-Host "Generated assets/icon.ico successfully with $( $icoSizes.Count ) embedded sizes!"
