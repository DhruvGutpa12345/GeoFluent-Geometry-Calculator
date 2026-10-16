$code = Get-Content 'scripts\GenerateAssets.cs' -Raw
Add-Type -TypeDefinition $code -ReferencedAssemblies 'System.Drawing'
[AssetGen.IconBuilder]::GenerateAll((Get-Location).Path)
