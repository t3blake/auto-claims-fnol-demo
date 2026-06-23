# Auto Claims FNOL — Intune Uninstallation Script
# Removes the Auto Claims FNOL application

$InstallPath = "C:\AutoClaimsFNOL"
$AppName = "Auto Claims FNOL"
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$ShortcutPath = Join-Path -Path $DesktopPath -ChildPath "$AppName.lnk"

# Remove installation directory
if (Test-Path $InstallPath) {
    Remove-Item -Path $InstallPath -Recurse -Force
    Write-Output "Removed installation directory: $InstallPath"
}

# Remove desktop shortcut
if (Test-Path $ShortcutPath) {
    Remove-Item -Path $ShortcutPath -Force
    Write-Output "Removed desktop shortcut: $ShortcutPath"
}

Write-Output "Uninstallation complete."
