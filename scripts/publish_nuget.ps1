Set-Location ..

Set-Location src
$path = &"${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -latest -prerelease -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe
& $path /property:Configuration=Release

Set-Location BurnSystems.WPF
& dotnet pack BurnSystems.WPF.csproj -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg -p:Configuration=Release
Set-Location ..

Set-Location ..

Copy-Item src/BurnSystems.WPF/bin/Release/*.nupkg nugets/
Copy-Item src/BurnSystems.WPF/bin/Release/*.snupkg nugets/

Set-Location nugets

$apikey = (Get-ChildItem -Path env:/nuget-apikey).Value
Get-ChildItem . -Filter *.nupkg | 
Foreach-Object {
    $content = $_.FullName

    Write-Output "dotnet nuget push $content"
    & dotnet nuget push $content -s https://api.nuget.org/v3/index.json --api-key=$apikey
}

Get-ChildItem . -Filter *.snupkg | 
Foreach-Object {
    $content = $_.FullName

    Write-Output "dotnet nuget push $content"
    & dotnet nuget push $content -s https://api.nuget.org/v3/index.json --api-key=$apikey
}

Set-Location ..


Set-Location scripts