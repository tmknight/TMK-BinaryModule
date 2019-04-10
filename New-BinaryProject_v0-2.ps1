$module = "TMK-BinaryModule"
$src = "C:\Users\b002837\Source\repos\TMK\$module"
New-Item -Path $src -ItemType Directory -ErrorAction Continue | Out-Null
Set-Location -Path $src
dotnet.exe new --install Microsoft.PowerShell.Standard.Module.Template
dotnet.exe new psmodule

<# ## Edit the .CS, then build the DLL
dotnet.exe build
 #>

<# Example 
Import-Module .\bin\Debug\netstandard2.0\$module
Get-Command -Name Test-SampleCmdlet -Syntax
Test-SampleCmdlet -FavoriteNumber 5
 #>
