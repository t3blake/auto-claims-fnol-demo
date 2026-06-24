# Auto Claims FNOL — Intune Detection Script
# Intune uses this to verify the application is installed
# Returns 0 if installed (success), non-zero if not found

$InstallPath = "C:\AutoClaimsFNOL"
$ExeName = "AutoClaimsFnolApp.exe"
$ExePath = Join-Path -Path $InstallPath -ChildPath $ExeName
$ExpectedVersion = "1.0.0"
$MarkerPath = Join-Path -Path $InstallPath -ChildPath "version.txt"

# Report "installed" only when the exe exists AND the version marker matches. The marker is written
# at the end of Install.ps1 (after the ACL fix), so an old install lacking it fails detection and
# Intune reinstalls — applying the launch-blocker fix in place. A file marker avoids 32/64-bit
# registry redirection between the SYSTEM install context and the detection script.
$InstalledVersion = if (Test-Path $MarkerPath) { (Get-Content -Path $MarkerPath -TotalCount 1).Trim() } else { $null }

if ((Test-Path $ExePath) -and ($InstalledVersion -eq $ExpectedVersion)) {
    Write-Output "Auto Claims FNOL v$InstalledVersion is installed at: $ExePath"
    exit 0
} else {
    Write-Output "Auto Claims FNOL not detected (exe: $(Test-Path $ExePath); marker: '$InstalledVersion'; expected: '$ExpectedVersion')"
    exit 1
}
