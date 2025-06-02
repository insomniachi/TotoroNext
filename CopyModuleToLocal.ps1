param (
    [string]$projectPath,
    [string]$moduleDir,
    [string]$targetFramework = "net9.0-windows10.0.26100"
)

# Check if project path is provided
if (-not $projectPath) {
    Write-Host "Error: Please provide the project path as an argument."
    exit 1
}

$csproj = "$projectPath/$projectPath.csproj"
$baseOutput = "Temp/$projectPath/$targetFramework"
$storePath = "Store/$projectPath"
$modulePath = "$moduleDir/$projectPath"

# Publish the project
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput" /p:TargetFramework=$targetFramework

# Remove unnecessary files
Get-ChildItem -Path $baseOutput -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force

if (Test-Path "$baseOutput/runtimes") {
    Remove-Item -Path "$baseOutput/runtimes" -Recurse -Force
}

if (-Not (Test-Path "$modulePath")) {
    New-Item -Path "$modulePath" -ItemType Directory -Force
}

Move-Item -Path $baseOutput -Destination "$modulePath" -Force

Remove-Item -Path "Temp" -Recurse -Force
