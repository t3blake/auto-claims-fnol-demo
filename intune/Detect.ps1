# Auto Claims FNOL — Intune Detection Script
# Intune uses this to verify the application is installed
# Returns 0 if installed (success), non-zero if not found

$InstallPath = "C:\AutoClaimsFNOL"
$ExeName = "AutoClaimsFnolApp.exe"
$ExePath = Join-Path -Path $InstallPath -ChildPath $ExeName

if (Test-Path $ExePath) {
    Write-Output "Auto Claims FNOL is installed at: $ExePath"
    exit 0
} else {
    Write-Output "Auto Claims FNOL not found at: $ExePath"
    exit 1
}
