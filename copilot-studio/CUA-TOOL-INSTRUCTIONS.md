# Auto Claims FNOL — CUA Tool Instructions

**Use this content in Copilot Studio: Computer Use Tool → Tool Instructions → paste contents (skip this header)**

This guide teaches the Computer Use tool how to interact with the Auto Claims FNOL desktop app. It covers screen detection, click targets, text input, and navigation.

---

## General Principles

1. **Always screenshot first** before any action
2. **Detect current screen** by window title and visible elements
3. **Use click coordinates** for buttons, dropdowns, and fields (or tab to navigate between fields)
4. **Type text** into focused text boxes
5. **Press Tab** to move to next field
6. **Press Enter** to submit forms or activate buttons
7. **Press Alt+F4** only in emergencies (usually not needed)

---

## Screen 0: Windows Run Dialog (App Launch)

**How to Get Here:**
- Press **Win+R**

**Visual Cues:**
- Dialog box titled "Run"
- Input field labeled "Open:"
- OK and Cancel buttons

**Interaction:**
1. Type: `C:\AutoClaimsFNOL\auto-claims-fnol.exe`
2. Press Enter (or click OK)

**Next Screen:**
- Auto Claims FNOL window appears with Login screen

---

## Screen 1: Login

**Window Title:** "Auto Claims FNOL — Intake System"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ Auto Claims FNOL — Intake System│
├─────────────────────────────────┤
│                                 │
│  LOGIN REQUIRED                  │
│                                 │
│  Username:  [_________________] │
│  Password:  [_________________] │
│                                 │
│              [Login]  [Exit]    │
│                                 │
└─────────────────────────────────┘
```

**Text Fields:**
- Username field (initially focused or top field)
- Password field (below username)

**Buttons:**
- Login (primary action button)
- Exit (close app)

**Interaction:**

For recommended credentials (`adjuster1` / `pass123`):

1. Screenshot to confirm screen
2. Click Username field (if not already focused)
3. Type: `adjuster1`
4. Press Tab
5. Type: `pass123`
6. Press Tab to navigate to Login button (or click it directly)
7. Press Enter (or click Login button)

**Expected Outcomes:**
- **Success:** Message "Login successful" appears in green, then Main Menu
- **Failure:** Message "Invalid username or password. Attempt X of 3" in red, form clears, ready for retry

---

## Screen 2: Main Menu

**Window Title:** "Auto Claims FNOL — Intake System"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ Auto Claims FNOL — Intake System│
├─────────────────────────────────┤
│                                 │
│  MAIN MENU                       │
│  Logged in as: Sarah Chen...     │
│                                 │
│  ○ New Claim                     │
│  ○ Search Existing Claim         │
│  ○ System Administration         │
│  ○ Log Off                       │
│                                 │
│             [Select]             │
│                                 │
└─────────────────────────────────┘
```

**Elements:**
- Four radio button options (mutually exclusive)
- Select button

**Interaction:**

To start a new claim:

1. Screenshot to confirm screen
2. Click the radio button next to "New Claim"
3. Click [Select] button
4. Wait for Page 1 to load

**Alternative:** Double-click "New Claim" to go directly (no need to click Select)

**To navigate to other areas:**
- Admin: Click "System Administration" radio, click Select
- Search: Click "Search Existing Claim" radio, click Select
- Logout: Click "Log Off" radio, click Select

---

## Screen 3: Claim Intake — Page 1 (Claimant Information)

**Screen Title Visible:** "NEW CLAIM — Page 1 of 6: Claimant Information"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ NEW CLAIM — Page 1 of 6         │
├─────────────────────────────────┤
│                                 │
│  Claimant Full Name              │
│  [_____________________]          │
│                                 │
│  Phone Number                    │
│  [_____________________]          │
│                                 │
│  Email Address (optional)        │
│  [_____________________]          │
│                                 │
│  Policy Number (optional)        │
│  [_____________________]          │
│                                 │
│  < Back   [Next >]               │
│                                 │
└─────────────────────────────────┘
```

**Fields:**
- Claimant Full Name (required, text input)
- Phone Number (required, text input)
- Email Address (optional, text input)
- Policy Number (optional, text input)

**Buttons:**
- Back (goes to Main Menu)
- Next (proceeds to Page 2)

**Interaction:**

1. Screenshot to confirm Page 1
2. Click Claimant Full Name field
3. Type: `Alex Johnson` (or user-provided name)
4. Press Tab
5. Type: `555-0101` (or user-provided phone)
6. Press Tab (move to Email field, optional)
7. Leave Email blank or tab again (move to Policy Number, optional)
8. Press Tab to navigate to Next button (or click it)
9. Press Enter (or click Next button)

**Validation:**
- If Name or Phone empty, error message in red at top of form
- Correct the field and retry Next

**Next Screen:** Page 2

---

## Screen 4: Claim Intake — Page 2 (Incident Details)

**Screen Title:** "NEW CLAIM — Page 2 of 6: Incident Details"

**Fields:**
- Incident Date (date picker or text, required)
- Incident Time (text HH:MM, optional)
- Incident Location (text address, required)
- Incident Type (dropdown, required)
- Weather Conditions (dropdown, required)
- Road Conditions (dropdown, required)
- Police Report Filed (radio: Yes/No, required)
- Police Report Number (text, conditional — enabled if Police = Yes)

**Interaction:**

1. Screenshot to confirm Page 2
2. Click Incident Date field
3. If calendar picker appears, click today's date or type MM/DD/YYYY
4. Press Tab (move to Time field, optional)
5. Leave Time blank or type 14:30 (example)
6. Press Tab (move to Location field)
7. Type location: `123 Main St, Anytown, CA 90210`
8. Press Tab (move to Incident Type field)
9. Click Incident Type dropdown
10. From list, click or arrow-down and press Enter to select `Multi-Vehicle` (or appropriate type)
11. Click Weather Conditions dropdown, select `Clear` (example)
12. Click Road Conditions dropdown, select `Dry` (example)
13. Click "No" radio for Police Report (example — or "Yes" if applicable)
14. If "Yes" selected, a Police Report # field enables below; type or leave blank
15. Click Next button
16. Press Enter (or click Next)

**Dropdown Navigation (Alternative to Click):**
- Click dropdown
- Press Down arrow to highlight options
- Press Enter to select
- Or click the option directly

**Next Screen:** Page 3

---

## Screen 5: Claim Intake — Page 3 (Image Upload & Preview)

**Screen Title:** "NEW CLAIM — Page 3 of 6: Upload Accident Image"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ Page 3: Upload Accident Image    │
├─────────────────────────────────┤
│                                 │
│  [Browse...]  [Clear Image]      │
│  Status: No image selected       │
│                                 │
│  ┌──────────────────────────┐   │
│  │                          │   │
│  │  [Image Preview Area]    │   │
│  │                          │   │
│  │  or (Empty if no image)  │   │
│  │                          │   │
│  └──────────────────────────┘   │
│                                 │
│  Filename: (none)               │
│                                 │
│  < Back   [Next >]              │
│                                 │
└─────────────────────────────────┘
```

**Buttons:**
- Browse (opens file chooser)
- Clear Image (removes selected image)
- Back
- Next

**Interaction:**

1. Screenshot to confirm Page 3
2. Click [Browse...] button
3. File chooser opens (Windows system dialog)
4. Navigate to image file (e.g., `C:\Temp\accident-sketch.jpg`)
5. Click the file to select it
6. Click [Open] button in file chooser (or double-click the file)
7. File chooser closes
8. Image preview appears in the preview area
9. Status changes to "Image attached"
10. Filename displays (e.g., "accident-sketch.jpg")
11. Click [Next >] button
12. Press Enter (or click Next)

**If No Image:**
- Click Next anyway (optional at this stage)
- Page 4 will show "No image provided" and will not allow proceeding without re-uploading

**Next Screen:** Page 4

---

## Screen 6: Claim Intake — Page 4 (Image Analysis) — CRITICAL

**Screen Title:** "NEW CLAIM — Page 4 of 6: Image Analysis & Interpretation"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ Page 4: Image Analysis           │
├─────────────────────────────────┤
│ [Image preview thumbnail]        │
│                                 │
│ INCIDENT TYPE (from image)      │
│ [Single Vehicle ▼]               │
│                                 │
│ NUMBER OF VEHICLES/OBJECTS       │
│ [2]                              │
│                                 │
│ ESTIMATED IMPACT TYPE            │
│ [T-Bone ▼]                       │
│                                 │
│ [... more fields ...]            │
│                                 │
│ CONFIDENCE LEVEL                 │
│ [High ▼]                         │
│                                 │
│ ASSUMPTIONS & AMBIGUITIES        │
│ [____________________]            │
│                                 │
│ < Back   [Next >] (disabled if incomplete)  │
│                                 │
└─────────────────────────────────┘
```

**Required Fields (All Must Be Populated):**
- Incident Type (dropdown)
- Number of Vehicles/Objects (text integer)
- Estimated Impact Type (dropdown)
- Vehicle 1 Position (dropdown)
- Vehicle 1 Direction of Travel (dropdown)
- [If multi-vehicle] Vehicle 2 Position (dropdown)
- [If multi-vehicle] Vehicle 2 Direction of Travel (dropdown)
- Damage Zones Detected (checkboxes, at least 1 required)
- Road/Scene Factors (checkboxes, at least 1 required)
- Confidence Level (dropdown)
- Assumptions & Ambiguities (free text, optional but recommended)

**Interaction (Agent Should Pre-Analyze Image):**

**Before filling this form:**
1. Agent analyzes the uploaded image
2. Extracts: number of vehicles, impact type, positions, damage, confidence
3. Prepares field values

**Then fills the form:**

1. Screenshot to confirm Page 4
2. Click Incident Type dropdown
3. Select appropriate type (Multi-Vehicle, Single Vehicle, etc.)
4. Press Tab (move to Number of Vehicles field)
5. Type: `2` (or appropriate number)
6. Press Tab (move to Impact Type dropdown)
7. Click Impact Type dropdown
8. Select: `T-Bone` (example)
9. Continue with Vehicle 1 Position, Vehicle 1 Direction, etc. (all dropdowns)
10. Scroll down to Damage Zones checkboxes
11. Click checkbox next to "Front" (check it)
12. Click checkbox next to "Rear" (check it)
13. Scroll down to Road/Scene Factors
14. Click checkbox next to "Intersection" (check it)
15. Click Confidence Level dropdown, select `High`
16. Click into Assumptions text field
17. Type: `"Clear T-bone at intersection. Both vehicles at right angles."`
18. Verify all required fields filled
19. Click [Next >] button (should now be enabled)
20. Press Enter

**Validation:**
- If [Next >] button is disabled (grayed out), one or more required fields are empty
- Scroll up/down to find the empty field
- Fill it, then retry [Next >]

**Dropdown Options Quick Reference:**

**Incident Type:**
- Single Vehicle
- Multi-Vehicle ← common for demo
- Parked Vehicle
- Unknown

**Impact Type:**
- Head-On
- T-Bone ← common for demo
- Rear-End
- Side-Swipe
- Unknown

**Damage Zones (checkboxes):**
- Front ✓ (check at least one)
- Rear
- Driver Side
- Passenger Side
- Roof
- Undercarriage

**Road/Scene Factors (checkboxes):**
- Intersection ✓ (check at least one)
- Lane Merge
- Parked Vehicle
- Median/Divider
- Gravel/Dirt
- Wet Surface

**Next Screen:** Page 5

---

## Screen 7: Claim Intake — Page 5 (Vehicle & Injury)

**Screen Title:** "NEW CLAIM — Page 5 of 6: Vehicle & Injury Information"

**Fields:**
- Primary Vehicle: Make, Model, Year, Color, License Plate (text, optional), Damage Level (dropdown)
- Multi-vehicle Incident (checkbox)
- Other Vehicle fields (conditional, enabled if multi-vehicle checkbox checked)
- Injuries Reported (radio: Yes/No, required)
- Witness Present (radio: Yes/No, required)
- Witness Name (text, optional, enabled if Witness = Yes)

**Interaction:**

1. Screenshot to confirm Page 5
2. Click Primary Vehicle Make field
3. Type: `Toyota`
4. Press Tab → Model: `Camry`
5. Press Tab → Year: `2022`
6. Press Tab → Color: `Blue`
7. Press Tab → License Plate: `ABC-1234` (optional)
8. Press Tab → Damage Level dropdown: Click, select `Moderate`
9. Scroll down to "Multi-vehicle Incident" checkbox
10. Click checkbox to enable Other Vehicle fields
11. Fill Other Vehicle: Make, Model, Year, Color, License Plate, Damage Level (same pattern)
12. Scroll to Injuries section
13. Click "No" radio (or "Yes" if applicable)
14. Click "No" radio for Witness (or "Yes" if applicable)
15. If Witness = Yes, optionally type witness name
16. Click [Next >] button
17. Press Enter

**Next Screen:** Page 6

---

## Screen 8: Claim Intake — Page 6 (Review & Submit)

**Screen Title:** "NEW CLAIM — Page 6 of 6: Review & Submit"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ Page 6: Review & Submit          │
├─────────────────────────────────┤
│                                 │
│  ✓ Claimant Information          │
│  ✓ Incident Details              │
│  ✓ Image Upload & Analysis       │
│  ✓ Vehicle & Injury Info         │
│                                 │
│  CLAIM SUMMARY                   │
│  Claimant: Alex Johnson          │
│  Phone: 555-0101                 │
│  Incident Date: 06/22/2026       │
│  [... more summary ...]          │
│                                 │
│  [Back to Edit] [Save Draft] [Submit]  │
│                                 │
└─────────────────────────────────┘
```

**Buttons:**
- Back to Edit (returns to any page)
- Save as Draft (optional)
- Submit (primary path)

**Interaction:**

1. Screenshot to confirm Page 6
2. Read the summary (optionally scroll to verify all data)
3. If correction needed, click [Back to Edit]
4. Otherwise, click [Submit] button
5. Wait for confirmation page

**Expected Outcome:** Page transitions to Confirmation (Screen 9)

---

## Screen 9: Confirmation

**Screen Title:** "CLAIM SUBMITTED SUCCESSFULLY"

**Visual Layout:**
```
┌─────────────────────────────────┐
│ CLAIM SUBMITTED SUCCESSFULLY     │
├─────────────────────────────────┤
│                                 │
│  ✓ Your claim has been submitted  │
│    for adjuster review.           │
│                                 │
│  Claim Number: CLM-2026-001234   │
│  Claimant: Alex Johnson          │
│  Status: Submitted for Review    │
│                                 │
│  Next steps:                     │
│  • An adjuster will contact you  │
│    within 24 hours               │
│  • Reference: CLM-2026-001234    │
│                                 │
│  [Return to Main Menu]           │
│                                 │
└─────────────────────────────────┘
```

**Button:**
- Return to Main Menu

**Interaction:**

1. Screenshot to confirm Confirmation page
2. Read and capture Claim Number (e.g., CLM-2026-001234)
3. Click [Return to Main Menu]
4. Report back to user with claim number

**Next Screen:** Main Menu (back to Screen 2)

---

## Error Handling

**If You See a Validation Error on Page 6 (Submit Blocked):**

Example error: "Image analysis is incomplete"

1. Screenshot to confirm error message
2. Click [Back to Edit] button
3. Navigate back to Page 4 (Image Analysis)
4. Find the empty field (e.g., no damage zone checkbox selected)
5. Complete the missing data
6. Click [Next >] to re-proceed through subsequent pages (form remembers prior entries)
7. Reach Page 6 again
8. Click [Submit]

**If Login Fails 3 Times:**

1. Screenshot confirms lock message
2. App displays: "Maximum login attempts exceeded. Terminal locked."
3. Close app (window X or Alt+F4)
4. Launch app again (Win+R, `C:\AutoClaimsFNOL\auto-claims-fnol.exe`)
5. Retry login from fresh start

**If Image Preview Doesn't Load:**

1. Check file path and format (.jpg, .png, .bmp)
2. Go back to Page 3
3. Click [Clear Image]
4. Re-browse for image file
5. Verify file is < 10 MB
6. Try upload again

---

## Navigation Shortcuts

| Goal | Keystroke/Click |
|------|---------|
| Next Page | Click [Next >] or press Enter (if focused on Next button) |
| Previous Page | Click [Back] |
| Cancel / Quit | Press Alt+F4 (last resort; normally use Back to Main Menu → Log Off) |
| Move between fields | Press Tab (forward) or Shift+Tab (backward) |
| Submit form | Click [Submit] or press Enter (if focused on Submit) |
| Open dropdown | Click dropdown, or Tab into field and press Space or Down arrow |
| Select from dropdown | Click option or arrow-keys + Enter |

---

## Tips for Reliable Interaction

1. **Always screenshot first** — Verify you're on the expected screen before acting
2. **Click text input fields before typing** — Ensure focus
3. **Press Tab to move between fields** — More reliable than clicking every field
4. **Use dropdowns (click + select) rather than free text** — Less ambiguous
5. **If dropdown looks stuck, press Escape** and try clicking again
6. **Wait for page to fully load** before clicking (especially after Page transitions)
7. **Check for error messages at top of form in red** — Address them before retrying

---

## Summary

- **Launch:** Win+R → `C:\AutoClaimsFNOL\auto-claims-fnol.exe` → Enter
- **Login:** adjuster1 / pass123
- **Main Menu:** Select "New Claim" → Click Select
- **Pages 1–5:** Fill fields, click Next (or Tab through fields, press Enter on Next button)
- **Page 4 (Critical):** Complete ALL image analysis fields before Next
- **Page 6:** Review, then click Submit
- **Confirmation:** Capture claim number, click Return to Main Menu

