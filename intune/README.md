# Intune Packaging for Auto Claims FNOL

This folder contains the scripts and configuration needed to package Auto Claims FNOL as a Win32 app for Intune deployment.

## Files

- **Install.ps1** — Installation script (runs with System privileges)
- **Uninstall.ps1** — Removal script
- **Detect.ps1** — Custom detection script (Intune uses this to verify installation)
- **source/** — Staging folder (files to be packaged)
- **output/** — Built `.intunewin` package (ready for Intune upload)

## Build Instructions

### Prerequisites
- Windows machine with PowerShell 5.1+
- **.NET 8 SDK** (the build publishes the app from source) — https://dotnet.microsoft.com/download/dotnet/8.0
- `IntuneWinAppUtil.exe` (the **Microsoft Win32 Content Prep Tool** — not committed here; download the latest from [microsoft/Microsoft-Win32-Content-Prep-Tool](https://github.com/microsoft/Microsoft-Win32-Content-Prep-Tool) and place it in this folder. `Build.ps1` / CI fetch it automatically.)

> The package is published **self-contained** (`--self-contained true`), so the .NET 8 runtime is bundled inside the app folder. Target machines (including the W365 Cloud PC) need **no .NET runtime installed** — that's only a build-time requirement here.

### Steps

1. **Build the self-contained package:**
   ```powershell
   cd intune
   .\Build.ps1
   ```

   This publishes a self-contained `win-x64` build (the .NET runtime is bundled, so target machines need no runtime), stages it, and generates `AutoClaimsFNOL.intunewin` in the `output/` folder.

2. **Upload to Intune:**
   - Open Microsoft Intune admin center
   - Go to **Apps** → **Windows** → **Add** → **Windows app (Win32)**
   - Upload the `.intunewin` file as the app package
   - Configure:
     - **Name:** Auto Claims FNOL
     - **Publisher:** Insurance (Demo)
     - **Install command:** `powershell.exe -ExecutionPolicy Bypass -File Install.ps1`
     - **Uninstall command:** `powershell.exe -ExecutionPolicy Bypass -File Uninstall.ps1`
     - **Install behavior:** System
     - **OS architecture:** 64-bit
     - **Minimum OS:** Windows 10 1903

3. **Detection Rule:**
   - Select "Use a custom detection script"
   - Upload `Detect.ps1` from this folder

4. **Assign the app:**
   - Add assignment to device or user group
   - Set as **Required**
   - Wait for deployment to Windows 365 Cloud PC

## Result

After deployment, the application will be installed to `C:\AutoClaimsFNOL\` and a desktop shortcut will appear on the user's desktop.

## Troubleshooting

- **App fails to install:** Check that all DLL dependencies are included in the `source/` folder
- **Shortcut doesn't appear:** Run `Install.ps1` manually with admin privileges
- **Detection fails:** Verify `AutoClaimsFnolApp.exe` exists at `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe`
