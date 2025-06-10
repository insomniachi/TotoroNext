param (
    [string]$projectPath,
    [string]$moduleDir
)

# Check if project path is provided
if (-not $projectPath) {
    Write-Host "Error: Please provide the project path as an argument."
    exit 1
}

$csproj = "$projectPath/$projectPath.csproj"
$baseOutput1 = "Temp/$projectPath/net9.0-windows10.0.26100"
$baseOutput2 = "Temp/$projectPath/net9.0-desktop"
$storePath = "Store/$projectPath"
$modulePath = "$moduleDir/$projectPath"

# Publish the project
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput1" /p:TargetFramework="net9.0-windows10.0.26100"
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput2" /p:TargetFramework="net9.0-desktop"

# Remove unnecessary files
Get-ChildItem -Path $baseOutput1 -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force
Get-ChildItem -Path $baseOutput2 -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force

if (Test-Path "$baseOutput1/runtimes") {
    Remove-Item -Path "$baseOutput1/runtimes" -Recurse -Force
}

if (Test-Path "$baseOutput2/runtimes") {
    Remove-Item -Path "$baseOutput2/runtimes" -Recurse -Force
}


if (-Not (Test-Path "$modulePath")) {
    New-Item -Path "$modulePath" -ItemType Directory -Force
}

Move-Item -Path $baseOutput1 -Destination "$modulePath" -Force
Move-Item -Path $baseOutput2 -Destination "$modulePath" -Force

Remove-Item -Path "Temp" -Recurse -Force
