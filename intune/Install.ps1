# Auto Claims FNOL — Intune Installation Script
# Installs Auto Claims FNOL application to C:\AutoClaimsFNOL\
# Creates a desktop shortcut for easy access

$InstallPath = "C:\AutoClaimsFNOL"
$AppName = "Auto Claims FNOL"
$ExeName = "AutoClaimsFnolApp.exe"
$AppVersion = "1.2.0"

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

# --- Make the app launchable + writable by the (non-elevated) account the agent drives ---
# Intune runs this script as SYSTEM, so it can reset ownership and ACLs without an interactive
# UAC prompt. The Computer Use agent cannot pass UAC (consent renders on the secure desktop it
# can't click) and the app manifest is asInvoker, so the exe must be runnable by the agent's
# NON-elevated token. This covers both leading causes of the launch blocker:
#   - Mark-of-the-Web on browser-downloaded binaries -> Unblock-File
#   - NTFS ACL not granting the interactive user     -> icacls grant (Modify, because the app
#     writes claims-fnol.db and its log file inside this folder)
$AclLog = Join-Path $InstallPath "install-acl.log"
"[$(Get-Date -Format o)] BEFORE`r`n$(icacls $InstallPath 2>&1 | Out-String)" | Out-File -FilePath $AclLog -Encoding utf8

Get-ChildItem -Path $InstallPath -Recurse -File | Unblock-File

# Well-known SIDs are locale-independent: S-1-5-32-544 = BUILTIN\Administrators, S-1-5-32-545 = BUILTIN\Users
& icacls.exe $InstallPath /setowner "*S-1-5-32-544" /T /C | Out-Null
& icacls.exe $InstallPath /grant "*S-1-5-32-545:(OI)(CI)M" /T /C | Out-Null

"[$(Get-Date -Format o)] AFTER`r`n$(icacls $InstallPath 2>&1 | Out-String)" | Out-File -FilePath $AclLog -Append -Encoding utf8
Write-Output "Set ACLs (Users: Modify) and cleared Mark-of-the-Web. ACL log: $AclLog"

# Create desktop shortcut (Public desktop so it's visible to the agent's user, not just SYSTEM)
$DesktopPath = [Environment]::GetFolderPath("CommonDesktopDirectory")
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

# Version marker — written LAST so its presence proves the install (including the ACL fix) completed.
# Use a FILE marker, not registry: the Intune install context and the detection script can differ in
# bitness, and HKLM:\SOFTWARE is redirected to WOW6432Node under 32-bit, which would desync them.
# A file under C:\AutoClaimsFNOL has no such redirection. Bump $AppVersion each release so existing
# installs fail detection and get reinstalled (no supersedence needed).
Set-Content -Path (Join-Path $InstallPath "version.txt") -Value $AppVersion -Encoding ascii -Force
Write-Output "Wrote version marker: $InstallPath\version.txt = $AppVersion"
