param (
    [string]$projectPath,
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

# Publish the project
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput" /p:TargetFramework=$targetFramework

# Remove unnecessary files
Get-ChildItem -Path $baseOutput -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force

if (Test-Path "$baseOutput/runtimes") {
    Remove-Item -Path "$baseOutput/runtimes" -Recurse -Force
}

# Create the ZIP archive
Compress-Archive -Path "$baseOutput\*" -DestinationPath "$baseOutput.zip" -Force

# Remove the base output directory
Remove-Item -Path $baseOutput -Recurse -Force

# Ensure the destination folder exists
if (-Not (Test-Path $storePath)) {
    New-Item -ItemType Directory -Path $storePath -Force
}

# Move the ZIP file and replace if it exists
Move-Item -Path "$baseOutput.zip" -Destination "$storePath/$targetFramework.zip" -Force

# Remove the temporary directory
Remove-Item -Path "Temp" -Recurse -Force