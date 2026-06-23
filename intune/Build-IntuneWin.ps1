# Auto Claims FNOL — Custom Intune Package Builder
# Creates .intunewin package without external dependencies
# .intunewin format: ZIP file with specific metadata structure

param(
    [string]$SourceDir = ".\source",
    [string]$OutputDir = ".\output",
    [string]$PackageName = "AutoClaimsFnolApp"
)

Write-Host "Auto Claims FNOL — Intune Package Builder (Custom)" -ForegroundColor Cyan

$ReleaseDir = "..\src\AutoClaimsFnolApp\bin\Release\net8.0-windows"

# Check if app is built
if (-not (Test-Path $ReleaseDir)) {
    Write-Host "ERROR: Release build not found at: $ReleaseDir" -ForegroundColor Red
    Write-Host "Please build the app first" -ForegroundColor Yellow
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

# Create metadata file (required for .intunewin)
$metadata = @{
    "packageIdentifier" = $PackageName
    "packageFormat" = "WindowsMobileMSI"
    "isBundle" = $false
    "displayVersion" = "1.0.0"
    "publisher" = "Blake"
    "description" = "Auto Claims FNOL App"
} | ConvertTo-Json

$metadataPath = "$SourceDir\IntunePackageMetadata.json"
$metadata | Out-File -FilePath $metadataPath -Encoding UTF8

# Create the .intunewin package (ZIP format)
Write-Host "Building .intunewin package..." -ForegroundColor Cyan

$intunewinPath = "$OutputDir\${PackageName}.intunewin"

# Remove existing package
if (Test-Path $intunewinPath) {
    Remove-Item $intunewinPath -Force
}

# Create ZIP with all source files
# Using .NET's built-in compression since we can't rely on external tools
Add-Type -AssemblyName System.IO.Compression.FileSystem

try {
    [System.IO.Compression.ZipFile]::CreateFromDirectory($SourceDir, $intunewinPath, [System.IO.Compression.CompressionLevel]::Optimal, $false)
    Write-Host "Package created successfully!" -ForegroundColor Green
    Write-Host "Output: $intunewinPath" -ForegroundColor Green
    Write-Host "Size: $((Get-Item $intunewinPath).Length / 1MB) MB" -ForegroundColor Green
    
    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "1. Open Microsoft Intune admin center (https://intune.microsoft.com)"
    Write-Host "2. Go to Apps → Windows → Add → Windows app (Win32)"
    Write-Host "3. Upload: $intunewinPath"
    Write-Host "4. Set Install command: powershell.exe -ExecutionPolicy Bypass -File Install.ps1"
    Write-Host "5. Set Uninstall command: powershell.exe -ExecutionPolicy Bypass -File Uninstall.ps1"
    Write-Host "6. Upload Detect.ps1 as custom detection script"
} catch {
    Write-Host "ERROR: Failed to create package: $_" -ForegroundColor Red
    exit 1
}
