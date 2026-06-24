# Auto Claims FNOL — CUA Tool Instructions

**Use this content in Copilot Studio: Computer Use Tool → Tool Instructions → paste contents (skip this header)**

> **Canonical source.** This file is the source of truth for the Computer Use tool instructions. The deployed `Claims Intake Agent/actions/Computeruse-Computeruse.mcs.yml` (`instructions:` block) is generated from it — edit here first, then sync. Don't hand-edit the YAML copy in isolation.

Automate the Auto Claims FNOL desktop intake app. Always screenshot first. Detect current screen by window title. Use Tab to navigate fields, Enter to submit, clicks for dropdowns/buttons.

Credentials: adjuster1 / pass123
App path: C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe

---

## If Multi-Factor Authentication (MFA) Appears

Sign-in — especially the Edge sign-in in Step 0 — may trigger an MFA prompt such as "Enter the number shown to sign in," an Authenticator approval, or a one-time code. When it does:

1. Take a screenshot that clearly shows the number or code on screen.
2. Surface that screenshot to the user and ask them to approve it in their Microsoft Authenticator app (or supply the code).
3. Wait for the user to finish approving — do not cancel, retry, or abandon the sign-in while waiting.
4. After confirmation, screenshot again to verify sign-in succeeded, then continue.

Never type a guessed MFA code and never skip the prompt.

---

## Required Information Before Launch

Before launching the app, make sure you have the values the desktop form will require:
- Claimant full name
- Claimant phone number
- Claimant email address (optional)
- Policy number (optional)
- Incident date
- Incident time (optional)
- Incident location
- Incident type
- Weather conditions
- Road conditions
- Police report filed (Yes/No) and report number if applicable
- Accident image(s) and a short image description
- Injuries reported (Yes/No)
- Witness present (Yes/No) and witness name if applicable

## Step 0: Stage the Accident Image on the W365 PC (BEFORE APP LAUNCH)

Page 3 uploads a local file, so the accident image must exist on the PC before launching the app.

- **If a local copy already exists** (e.g., the orchestrator or a prior step staged it in `C:\Users\<username>\Downloads\`), use that file and skip the download below.
- **If you must download it** from the OneDrive/SharePoint URL and a sign-in or MFA prompt appears, follow the **MFA** section above — screenshot the number, surface it, and wait. Do not cancel or loop.

1. Open Microsoft Edge
2. Navigate to the OneDrive/SharePoint URL provided
3. Sign in if prompted (use the W365 user's credentials)
4. Download the file: click the download button or use the three-dot menu → Download
5. Default save location: C:\Users\<username>\Downloads\
6. Note the exact filename (e.g., accident.jpg)
7. Close or minimize Edge

Staged image path: C:\Users\<username>\Downloads\<filename> — use this in Page 3.

---

## Launch & Login

1. Press Win+R
2. Type: C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe
3. Press Enter
4. Wait for "Auto Claims FNOL - Intake System" window
5. Click Username field, type: adjuster1
6. Press Tab, type password: pass123
7. Press Enter or click Login

## Main Menu

1. Screenshot to confirm the MAIN MENU page
2. Click the "New Claim" button directly (the menu uses buttons, not radio buttons)

## Pages 1-3: Claimant, Incident, Image

**Page 1 - Claimant:**
1. Click Claimant Full Name field
2. Type user-provided name
3. Press Tab, type phone number
4. Press Tab, skip Email (optional), Tab again, skip Policy (optional)
5. Click [Next >]

**Page 2 - Incident:**
1. Click Incident Date, select today or type MM/DD/YYYY
2. Press Tab, skip Time (optional)
3. Type location address
4. Tab to Incident Type dropdown, click, select "Multi-Vehicle" (or appropriate type)
5. Select Weather & Road Conditions dropdowns
6. Select Police Report: "No" (or "Yes" if applicable)
7. Click [Next >]

**Page 3 - Image Upload:**
1. Click into the "Image Description" field and type a short description (e.g., "Front-end collision damage")
2. Click the [Add Image...] button
3. In the Windows file dialog, navigate to C:\Users\<username>\Downloads\
4. Select the image downloaded in Step 0, click [Open]
5. Verify the file appears in the "Uploaded Images" list, the preview shows it, and the status no longer reads "No images uploaded"
6. At least one image is required to proceed. Click [Next >]

## Page 4: Image Analysis (CRITICAL - ALL FIELDS REQUIRED)

This page blocks the Next button until every required field is complete. The orchestrator has already analyzed the accident image with Work IQ Copilot and is passing you the values to enter here. **Enter the values you were given.** Only if a specific value is missing should you derive it from the image preview shown on this page.

**Required Fields (all must be populated):**
- Incident Type: dropdown
- Number of Vehicles: text integer
- Estimated Impact Type: dropdown (Head-On, T-Bone, Rear-End, Side-Swipe)
- Vehicle 1 Position: dropdown (N/S/E/W)
- Vehicle 1 Direction of Travel: dropdown
- Vehicle 2 Position (if multi-vehicle): dropdown
- Vehicle 2 Direction of Travel (if multi-vehicle): dropdown
- Damage Zones: checkboxes - select at least 1 (Front, Rear, Driver Side, Passenger Side, Roof, Undercarriage)
- Road/Scene Factors: checkboxes - select at least 1 (Intersection, Lane Merge, Parked Vehicle, Median, Gravel, Wet Surface)
- Confidence Level: dropdown (High, Medium, Low)
- Assumptions & Ambiguities: optional text field

**Fill Page 4 with the values provided to you:**
1. Click Incident Type dropdown, select the provided type
2. Tab to Number of Vehicles, type the provided count
3. Tab to Impact Type dropdown, click, select the provided type
4. Continue tabbing through remaining dropdowns using the provided values: Vehicle 1 Position, Vehicle 1 Direction, Vehicle 2 Position, Vehicle 2 Direction
5. Scroll down to Damage Zones, check each provided zone (at least 1)
6. Check each provided Road/Scene Factor (at least 1)
7. Click Confidence Level dropdown, select the provided level (High/Medium/Low)
8. Tab to Assumptions field, type the provided notes if any
9. Verify all required fields filled (Next button should be enabled)
10. Click [Next >]

If a value wasn't provided, derive it from the on-screen image preview. If Next is disabled, scroll to find the empty field and complete it.

## Pages 5-6: Vehicle Info & Submit

**Page 5 - Vehicle & Injury:**
1. Click Primary Vehicle Make field, type vehicle make (e.g., Toyota)
2. Tab through: Model, Year
3. Click Primary Damage Level dropdown, select level (None / Minor / Moderate / Severe / Total Loss)
4. Check "Multi-vehicle incident" if applicable
5. If multi-vehicle, fill Other Vehicle fields (Make, Model, Damage Level)
6. Select Injuries Reported: click Yes or No
7. Select Witness Present: click Yes or No
8. If Witness = Yes, optionally type witness name
9. Click [Next >]

**Page 6 - Review & Submit:**
1. Screenshot confirms Page 6
2. Review claim summary for accuracy
3. Click [Submit] button
4. Wait for Confirmation page

## Confirmation

1. Screenshot confirms "CLAIM SUBMITTED SUCCESSFULLY"
2. Capture and report Claim Number (e.g., CLM-2026-001234)
3. Click [Return to Main Menu]