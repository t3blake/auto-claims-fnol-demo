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
- `IntuneWinAppUtil.exe` (included in this folder, or download from [Microsoft](https://github.com/microsoft/microsoft-intune-app-sdk-dotnet/releases))

### Steps

1. **Copy app files to staging folder:**
   ```powershell
   Copy-Item ..\src\AutoClaimsFnolApp\bin\Release\net8.0-windows\* intune\source\ -Recurse
   ```

2. **Run the packaging script:**
   ```powershell
   .\intune\IntuneWinAppUtil.exe -c intune\source -s Install.ps1 -o intune\output -q
   ```

   This generates `AutoClaimsFnolApp.intunewin` in the `output/` folder.

3. **Upload to Intune:**
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

4. **Detection Rule:**
   - Select "Use a custom detection script"
   - Upload `Detect.ps1` from this folder

5. **Assign the app:**
   - Add assignment to device or user group
   - Set as **Required**
   - Wait for deployment to Windows 365 Cloud PC

## Result

After deployment, the application will be installed to `C:\AutoClaimsFNOL\` and a desktop shortcut will appear on the user's desktop.

## Troubleshooting

- **App fails to install:** Check that all DLL dependencies are included in the `source/` folder
- **Shortcut doesn't appear:** Run `Install.ps1` manually with admin privileges
- **Detection fails:** Verify `AutoClaimsFnolApp.exe` exists at `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe`
