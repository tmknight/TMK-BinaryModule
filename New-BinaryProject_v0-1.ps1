$module = "TMK-BinaryModule"
$src = "C:\Users\b002837\Source\repos\TMK\$module"
Set-Location -Path $src
New-Item -Path 'src' -Type Directory
New-Item -Path 'Output' -Type Directory
New-Item -Path 'Tests' -Type Directory
New-Item -Path $module -Type Directory
Set-Location -Path "$src\src"

dotnet.exe new classlib --name $module
Move-Item -Path .\$module\* -Destination .\
Remove-Item $module -Recurse

$ver = dotnet.exe --version
dotnet.exe new globaljson --sdk-version $ver
$pslVer = (Get-Package -Name PowerShellStandard.Library).Version
dotnet.exe add package PowerShellStandard.Library --version $pslVer
