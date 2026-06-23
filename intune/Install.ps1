# Auto Claims FNOL — Intune Installation Script
# Installs Auto Claims FNOL application to C:\AutoClaimsFNOL\
# Creates a desktop shortcut for easy access

$InstallPath = "C:\AutoClaimsFNOL"
$AppName = "Auto Claims FNOL"
$ExeName = "AutoClaimsFnolApp.exe"

# Create installation directory
if (-not (Test-Path $InstallPath)) {
    New-Item -ItemType Directory -Path $InstallPath | Out-Null
    Write-Output "Created directory: $InstallPath"
}

# Copy all files from the script's directory to the installation directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
Get-ChildItem -Path $ScriptDir -File | ForEach-Object {
    if ($_.Name -notin @("Install.ps1", "Uninstall.ps1", "Detect.ps1", "IntuneWinAppUtil.exe")) {
        Copy-Item -Path $_.FullName -Destination $InstallPath -Force
        Write-Output "Copied: $($_.Name)"
    }
}

# Create desktop shortcut
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$ShortcutPath = Join-Path -Path $DesktopPath -ChildPath "$AppName.lnk"
$ExePath = Join-Path -Path $InstallPath -ChildPath $ExeName

# Use WScript.Shell COM object to create shortcut
$WshShell = New-Object -ComObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut($ShortcutPath)
$Shortcut.TargetPath = $ExePath
$Shortcut.WorkingDirectory = $InstallPath
$Shortcut.Description = "Auto Claims FNOL - Insurance Claims Processing"
$Shortcut.Save()
Write-Output "Created desktop shortcut: $ShortcutPath"

Write-Output "Installation complete. App installed to: $InstallPath"
Write-Output "Desktop shortcut created: $AppName.lnk"
