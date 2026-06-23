# Auto Claims FNOL — Intune Packaging Build Script
# Automates the process of building the .intunewin package

param(
    [switch]$Force
)

$ReleaseDir = "..\src\AutoClaimsFnolApp\bin\Release\net8.0-windows"
$SourceDir = ".\source"
$OutputDir = ".\output"
$IntuneWinUtil = ".\IntuneWinAppUtil.exe"

Write-Host "Auto Claims FNOL — Intune Package Builder" -ForegroundColor Cyan

# Check if app is built
if (-not (Test-Path $ReleaseDir)) {
    Write-Host "ERROR: Release build not found at: $ReleaseDir" -ForegroundColor Red
    Write-Host "Please build the app first with: dotnet publish -c Release -r win-x64" -ForegroundColor Yellow
    exit 1
}

# Clean and recreate staging folder
if (Test-Path $SourceDir) {
    Remove-Item $SourceDir -Recurse -Force
}
New-Item -ItemType Directory -Path $SourceDir | Out-Null
Write-Host "Created staging folder: $SourceDir" -ForegroundColor Green

# Copy app files to staging
Write-Host "Copying app files to staging folder..."
Copy-Item "$ReleaseDir\*" $SourceDir -Recurse
Copy-Item ".\Install.ps1" $SourceDir
Copy-Item ".\Uninstall.ps1" $SourceDir
Copy-Item ".\Detect.ps1" $SourceDir
Write-Host "Files copied." -ForegroundColor Green

# Create output directory
if (-not (Test-Path $OutputDir)) {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# Check if IntuneWinAppUtil exists
if (-not (Test-Path $IntuneWinUtil)) {
    Write-Host "ERROR: IntuneWinAppUtil.exe not found" -ForegroundColor Red
    Write-Host "Download from: https://github.com/microsoft/microsoft-intune-app-sdk-dotnet/releases" -ForegroundColor Yellow
    exit 1
}

# Build the package
Write-Host "Building .intunewin package..." -ForegroundColor Cyan
& $IntuneWinUtil -c $SourceDir -s Install.ps1 -o $OutputDir -q

if ($LASTEXITCODE -eq 0) {
    $PackageFile = Get-ChildItem "$OutputDir\*.intunewin" | Select-Object -First 1
    Write-Host "Package built successfully!" -ForegroundColor Green
    Write-Host "Output: $($PackageFile.FullName)" -ForegroundColor Green
    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "1. Open Microsoft Intune admin center"
    Write-Host "2. Go to Apps → Windows → Add → Windows app (Win32)"
    Write-Host "3. Upload: $($PackageFile.FullName)"
    Write-Host "4. Set Install command: powershell.exe -ExecutionPolicy Bypass -File Install.ps1"
    Write-Host "5. Set Uninstall command: powershell.exe -ExecutionPolicy Bypass -File Uninstall.ps1"
    Write-Host "6. Upload Detect.ps1 as custom detection script"
} else {
    Write-Host "ERROR: Package build failed" -ForegroundColor Red
    exit 1
}
