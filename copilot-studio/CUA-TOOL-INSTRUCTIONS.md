# Auto Claims FNOL — CUA Tool Instructions

**Use this content in Copilot Studio: Computer Use Tool → Tool Instructions → paste contents (skip this header)**

Automate the Auto Claims FNOL desktop intake app. Always screenshot first. Detect current screen by window title. Use Tab to navigate fields, Enter to submit, clicks for dropdowns/buttons.

Credentials: adjuster1 / pass123
App path: C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe

---

## Launch & Login

1. Press Win+R
2. Type: C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe
3. Press Enter
4. Wait for "Auto Claims FNOL — Intake System" window
5. Click Username field, type: adjuster1
6. Press Tab, type password: pass123
7. Press Enter or click Login

## Main Menu

1. Screenshot to confirm "MAIN MENU" page
2. Click radio button next to "New Claim"
3. Click [Select] button

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
1. Click [Browse...] button
2. Select image file from Windows dialog, click [Open]
3. Verify image preview appears and Status = "Image attached"
4. Click [Next >]

## Page 4: Image Analysis (CRITICAL - ALL FIELDS REQUIRED)

This page blocks Next button until complete. Analyze image first, then fill all fields.

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

**Fill Page 4:**
1. Click Incident Type dropdown, select appropriate type
2. Tab to Number of Vehicles, type count
3. Tab to Impact Type dropdown, click, select type from list
4. Continue tabbing through remaining dropdowns: Vehicle 1 Position, Vehicle 1 Direction, Vehicle 2 Position, Vehicle 2 Direction
5. Scroll down to Damage Zones, click at least 1 checkbox
6. Check at least 1 Road/Scene Factors checkbox
7. Click Confidence Level dropdown, select High/Medium/Low
8. Tab to Assumptions field, type observations if needed
9. Verify all required fields filled (Next button should be enabled)
10. Click [Next >]

If Next button is disabled, scroll to find empty field and complete it.

## Pages 5-6: Vehicle Info & Submit

**Page 5 - Vehicle & Injury:**
1. Click Primary Vehicle Make field, type vehicle make (e.g., Toyota)
2. Tab through: Model, Year, Color, License Plate (optional)
3. Click Damage Level dropdown, select level
4. Scroll down, check "Multi-vehicle Incident" if applicable
5. If multi-vehicle, fill Other Vehicle fields (Make, Model, Year, Color, Plate, Damage Level)
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