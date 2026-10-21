param (
    [string]$VersionProps,
    [string]$ManifestPath
)

if (Test-Path $VersionProps) {
    [xml]$xml = Get-Content $VersionProps
    $rev = [int]$xml.Project.PropertyGroup.RevisionVersion + 1
    $xml.Project.PropertyGroup.RevisionVersion = $rev.ToString()
    $maj = $xml.Project.PropertyGroup.MajorVersion
    $min = $xml.Project.PropertyGroup.MinorVersion
    $bld = $xml.Project.PropertyGroup.BuildVersion
    $fullVer = "$maj.$min.$bld.$rev"
    $xml.Project.PropertyGroup.Version = $fullVer
    $xml.Project.PropertyGroup.AssemblyVersion = $fullVer
    $xml.Project.PropertyGroup.FileVersion = $fullVer
    $xml.Project.PropertyGroup.InformationalVersion = $fullVer
    $xml.Save($VersionProps)
    Write-Host "AutoIncrementVersion: Incremented version to $fullVer"

    if (Test-Path $ManifestPath) {
        [xml]$mXml = Get-Content $ManifestPath
        if ($mXml.Package.Identity.Version) {
            $mXml.Package.Identity.Version = $fullVer
            $mXml.Save($ManifestPath)
            Write-Host "AutoIncrementVersion: Synchronized Package.appxmanifest to $fullVer"
        }
    }
}
