param (
    [string]$projectPath
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

# Publish the project
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput1" /p:TargetFramework=net9.0-windows10.0.26100
msbuild $csproj /r /t:Publish /p:Configuration=Debug /p:PublishDir="../$baseOutput2" /p:TargetFramework=net9.0-desktop

# Remove unnecessary files
Get-ChildItem -Path $baseOutput1 -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force
Get-ChildItem -Path $baseOutput2 -File | Where-Object { $_.Name -match "Microsoft|Uno|WinRT|Skia" } | Remove-Item -Force

if (Test-Path "$baseOutput1/runtimes") {
    Remove-Item -Path "$baseOutput1/runtimes" -Recurse -Force
}

if (Test-Path "$baseOutput2/runtimes") {
    Remove-Item -Path "$baseOutput2/runtimes" -Recurse -Force
}

# Create the ZIP archive
Compress-Archive -Path "$baseOutput1\*" -DestinationPath "$baseOutput1.zip" -Force
Compress-Archive -Path "$baseOutput2\*" -DestinationPath "$baseOutput2.zip" -Force

# Remove the base output directory
Remove-Item -Path $baseOutput1 -Recurse -Force
Remove-Item -Path $baseOutput2 -Recurse -Force

# Ensure the destination folder exists
if (-Not (Test-Path $storePath)) {
    New-Item -ItemType Directory -Path $storePath -Force
}

# Move the ZIP file and replace if it exists
Move-Item -Path "$baseOutput1.zip" -Destination "$storePath/net9.0-windows10.0.26100.zip" -Force
Move-Item -Path "$baseOutput2.zip" -Destination "$storePath/net9.0-desktop.zip" -Force


# Remove the temporary directory
Remove-Item -Path "Temp" -Recurse -Force