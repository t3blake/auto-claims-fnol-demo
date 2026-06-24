# Auto Claims FNOL — Intune Packaging Build Script
# Automates the process of building the .intunewin package

param(
    [switch]$Force
)

$ProjectPath = "..\src\AutoClaimsFnolApp\AutoClaimsFnolApp.csproj"
$ReleaseDir = "..\src\AutoClaimsFnolApp\bin\Release\net8.0-windows\publish-sc"
$SourceDir = ".\source"
$OutputDir = ".\output"
$IntuneWinUtil = ".\IntuneWinAppUtil.exe"

Write-Host "Auto Claims FNOL — Intune Package Builder" -ForegroundColor Cyan

# Publish a SELF-CONTAINED win-x64 build so the target machine needs NO .NET runtime installed.
# A framework-dependent build would require the .NET 8 Desktop Runtime as a separate prerequisite;
# bundling the runtime keeps the .intunewin a single standalone artifact that runs on a clean
# Windows box (the W365 Cloud PC has no .NET runtime pre-installed).
Write-Host "Publishing self-contained win-x64 build..." -ForegroundColor Cyan
dotnet publish $ProjectPath -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o $ReleaseDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: dotnet publish failed" -ForegroundColor Red
    exit 1
}

# Confirm the runtime was actually bundled (self-contained), not just the app DLLs
if (-not (Test-Path (Join-Path $ReleaseDir "coreclr.dll"))) {
    Write-Host "ERROR: publish output is not self-contained (coreclr.dll missing)" -ForegroundColor Red
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
    Write-Host "Download from: https://github.com/microsoft/Microsoft-Win32-Content-Prep-Tool" -ForegroundColor Yellow
    exit 1
}

# Build the package
Write-Host "Building .intunewin package..." -ForegroundColor Cyan
& $IntuneWinUtil -c $SourceDir -s Install.ps1 -o $OutputDir -q

if ($LASTEXITCODE -eq 0) {
    # IntuneWinAppUtil names the output after the setup file (Install.ps1 -> Install.intunewin).
    # Rename to the release-facing asset name so local builds, the GitHub Action, and the
    # published GitHub release all use the same descriptive name.
    $finalPath = Join-Path $OutputDir "AutoClaimsFNOL.intunewin"
    Move-Item -Path (Join-Path $OutputDir "Install.intunewin") -Destination $finalPath -Force
    $PackageFile = Get-Item $finalPath
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
