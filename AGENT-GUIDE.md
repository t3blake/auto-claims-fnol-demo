# AGENT-GUIDE.md — Computer-Use Agent Reference for Auto Claims FNOL

> **Purpose:** Complete authoritative reference for building a Windows 365 Agent (Computer-Use) that interacts with the Auto Claims FNOL desktop application. Describes every screen, input flow, edge case, field definition, and enum value.

---

## 0. Launching the Application

The app does **not** launch automatically. The agent must open it.

### Recommended: Windows Run Dialog

| Step | Agent Action |
|------|-------------|
| 1 | Press **Win+R** to open the Run dialog |
| 2 | Type: `C:\AutoClaimsFNOL\auto-claims-fnol.exe` |
| 3 | Press **Enter** |
| 4 | Wait for window titled **"Auto Claims FNOL — Intake System"** to appear |
| 5 | Click the window to ensure focus |

### Alternative: Desktop Shortcut

| Step | Agent Action |
|------|-------------|
| 1 | Minimize all windows (press **Win+D**) to expose desktop |
| 2 | Locate shortcut labeled **"Auto Claims FNOL"** |
| 3 | **Double-click** the shortcut |
| 4 | Wait for window titled **"Auto Claims FNOL — Intake System"** to appear |

### Verifying the App Is Running

- **Window title:** `Auto Claims FNOL — Intake System`
- **Process name:** `auto-claims-fnol`
- **Visual confirmation:** Gray WPF form with input fields, buttons, dropdown lists

Once window is open and focused, proceed to Section 1 (Login Screen).

---

## 1. Application Overview

**Auto Claims FNOL** is a WPF desktop application that simulates a legacy auto insurance claims intake system. It is installed locally on Windows and launched via Run dialog, desktop shortcut, or exe. The app is a single-window desktop form styled to look unmistakably pre-modern (2000–2005 era).

- **Exe name:** `auto-claims-fnol.exe`
- **Window title:** `Auto Claims FNOL — Intake System`
- **Install path (Intune):** `C:\AutoClaimsFNOL\auto-claims-fnol.exe`
- **Database:** `claims-fnol.db` (SQLite, auto-created next to exe on first run)
- **Desktop shortcut:** `C:\Users\Public\Desktop\Auto Claims FNOL.lnk` (label: **"Auto Claims FNOL"**)

---

## 2. Window Structure & Visual Recognition

### Layout

```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │  ← Window title
├────────────────────────────────────────────────────────┤
│                                                        │
│  [Screen Title: "Login" or "New Claim" etc.]           │
│  ────────────────────────────────────────────          │
│                                                        │
│  [Form fields: text boxes, dropdowns, checkboxes]      │  ← Content area
│  [Buttons: Next, Back, Cancel, Save, Submit, etc.]     │
│                                                        │
│                                                        │
│  [Status bar at bottom if applicable]                  │
│                                                        │
└────────────────────────────────────────────────────────┘
```

### Visual Properties

| Property | Value |
|----------|-------|
| Window size | 1024 × 768 px (default), min 800 × 600 |
| Background | Light gray (`#E8E8E8`) |
| Text color | Black (`#000000`) |
| Font | Segoe UI 10pt |
| Button style | Classic Windows gray buttons with black text |
| Dropdown lists | Standard gray with dropdown arrow |
| Text boxes | White background with black border |
| Checkboxes/Radios | Standard Windows style |
| Era aesthetic | 2000–2005 enterprise system (no modern design) |

### How to Detect Form State

The agent must distinguish between screen states:

| State | Visual Cue | Agent Action |
|---|---|---|
| **Form with text input** | Text box with cursor visible | Click into box, type, press Tab or Enter |
| **Form with dropdown** | Gray box with down arrow | Click dropdown, select from list |
| **Form with radio/checkbox** | Circular/square box | Click to toggle |
| **Waiting for page navigation** | Next/Back/Cancel buttons visible | Click appropriate button |
| **Submit blocked** | Submit button grayed out or error message | Review validation errors, correct fields, retry |
| **Success confirmation** | Green success message displayed | Read message, click OK or Next |

---

## 3. Login Screen

**Trigger:** App launch (automatic first screen)

**Screen layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  LOGIN REQUIRED                                         │
│                                                        │
│  Username:  [____________________]                      │
│  Password:  [____________________]                      │
│                                                        │
│                      [Login]  [Exit]                    │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Input sequence:**
1. Click Username field
2. Type username
3. Press Tab
4. Type password (characters display as `*`)
5. Click Login button

**Valid Credentials:**

| Username | Password | Role | Display Name |
|----------|----------|------|--------------|
| `adjuster1` | `pass123` | Claims Adjuster | Sarah Chen — Downtown Branch |
| `adjuster2` | `pass123` | Claims Adjuster | Mike Davis — West Side Branch |
| `admin` | `admin` | System Administrator | System Admin |

**Outcomes:**
- **Success:** Message `"Login successful. Welcome, [Name]."` in green. Then Main Menu appears.
- **Failure:** Message `"Invalid username or password. Attempt X of 3."` in red. Text boxes cleared. Ready for retry.
- **3 failures:** Message `"Maximum login attempts exceeded. Terminal locked."` App exits after user clicks OK.

**Recommendation:** Use `adjuster1` / `pass123` for normal agent workflows.

---

## 4. Main Menu

**Trigger:** After successful login

**Screen layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  MAIN MENU                                              │
│  Logged in as: Sarah Chen, Downtown Branch              │
│                                                        │
│  [ ] New Claim                                          │
│  [ ] Search Existing Claim                              │
│  [ ] System Administration (Admin only)                 │
│  [ ] Log Off                                            │
│                                                        │
│                              [Select]                  │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Interaction:**
- Click one of the four options (mutually exclusive radio buttons)
- Click **Select** button to navigate
- Or double-click an option to go directly

**Workflows:**
| Option | Destination | Requires |
|--------|-------------|----------|
| New Claim | Claim Intake Form (Page 1) | Adjuster role |
| Search Existing Claim | Claim Search Screen | Adjuster role |
| System Administration | Admin Menu | Admin role only |
| Log Off | Login Screen | Any role |

---

## 5. Create New Claim — Multi-Page Form

The claim intake is a **6-page wizard**. Navigation: Next / Back / Cancel on each page.

### 5.1 Page 1: Claimant Information

**Screen title:** `NEW CLAIM — Page 1 of 6: Claimant Information`

**Fields:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Claimant Full Name | Text | Yes | First + Last, e.g., "Alex Johnson" |
| Phone Number | Text | Yes | Format: 555-XXXX (demo fake) or +1-XXX-XXX-XXXX |
| Email Address | Text | No | e.g., alex@example.com |
| Policy Number | Text | No | e.g., POL-2026-987654 |
| Alternate Phone | Text | No | Secondary contact |

**Interaction:**
1. Click into Claimant Full Name field
2. Type the name
3. Tab to Phone Number field
4. Type phone
5. (Optional) Fill Email and Policy Number
6. Click **Next >** to proceed to Page 2

**Validation:**
- Claimant Full Name: Required, min 3 characters
- Phone Number: Required, must contain digits
- Email: Optional, but if filled must be valid format
- Policy Number: Optional

---

### 5.2 Page 2: Incident Details

**Screen title:** `NEW CLAIM — Page 2 of 6: Incident Details`

**Fields:**

| Field | Type | Required | Options/Format |
|-------|------|----------|--------|
| Incident Date | Date picker | Yes | MM/DD/YYYY or calendar widget |
| Incident Time (HH:MM) | Time | No | 24-hour format, e.g., 14:30 |
| Incident Location | Text | Yes | Street address, city, state, zip |
| Incident Type | Dropdown | Yes | Single Vehicle, Multi-Vehicle, Parked Vehicle, Other |
| Weather Conditions | Dropdown | Yes | Clear, Rain, Snow, Fog, Dark, Overcast, Other |
| Road Conditions | Dropdown | Yes | Dry, Wet, Ice/Snow, Gravel, Pothole/Debris, Other |
| Police Report Filed | Radio (Yes/No) | Yes | |
| Police Report Number | Text | Conditional | Enabled only if "Police Report Filed" = Yes |

**Interaction:**
1. Click Incident Date field or calendar icon
2. Select date from calendar or type MM/DD/YYYY
3. Tab to Incident Time (optional)
4. Type time or skip
5. Tab to Incident Location, type address
6. Click Incident Type dropdown, select option
7. Click Weather Conditions dropdown, select option
8. Click Road Conditions dropdown, select option
9. Select "Yes" or "No" for Police Report Filed
10. If Yes, type Police Report Number
11. Click **Next >** to proceed to Page 3

**Dropdown Values:**

**Incident Type:**
- Single Vehicle
- Multi-Vehicle
- Parked Vehicle (not moving, hit stationary)
- Other

**Weather Conditions:**
- Clear
- Rain
- Snow
- Fog
- Dark (night, no street light)
- Overcast (daylight, cloudy)
- Other

**Road Conditions:**
- Dry
- Wet (recent rain, puddles)
- Ice/Snow
- Gravel
- Pothole/Debris
- Other

---

### 5.3 Page 3: Image Upload & Preview

**Screen title:** `NEW CLAIM — Page 3 of 6: Upload Accident Image`

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  UPLOAD ACCIDENT IMAGE                                  │
│  (Hand-drawn sketch or photograph)                      │
│                                                        │
│  [Browse...]  [Clear Image]                             │
│  Status: No image selected                              │
│                                                        │
│  ┌──────────────────────────────────┐                  │
│  │                                  │                  │
│  │        [Image Preview Area]       │  ← 400x300 px   │
│  │        (Empty or shows image)     │                  │
│  │                                  │                  │
│  └──────────────────────────────────┘                  │
│                                                        │
│  Filename: (none) or "accident-sketch.jpg"              │
│                                                        │
│                    < Back   [Next >]                    │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Interaction:**
1. Click **[Browse...]** button
2. System opens file chooser (Windows dialog)
3. Navigate to and select image file (.jpg, .png, .bmp)
4. File chooser closes
5. Image preview displays in the preview area
6. Filename appears below preview
7. Status changes to "Image attached"
8. Click **[Next >]** to proceed to Page 4

**Accepted Formats:** `.jpg`, `.png`, `.bmp`  
**Max file size:** 10 MB (enforced by file chooser filter)

**Validation:**
- Image upload is **optional** at this stage
- But if no image is uploaded, Page 4 (Image Analysis) will be prefilled with "No image provided" and agent will not be able to proceed unless image is re-uploaded

**If user clicks [Clear Image]:**
- Image preview cleared
- Status changes back to "No image selected"
- Filename field shows "(none)"
- Agent can go back and re-upload or proceed without image (not recommended)

---

### 5.4 Page 4: Image Analysis (REQUIRED — Hard Block)

**Screen title:** `NEW CLAIM — Page 4 of 6: Image Analysis & Interpretation`

**CRITICAL:** This section is **hard-required**. Form CANNOT submit without all fields complete.

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  IMAGE ANALYSIS & INTERPRETATION                        │
│                                                        │
│  [Image preview thumbnail]  [Filename: accident.jpg]    │
│                                                        │
│  ────────────────────────────────────────────────────  │
│  INCIDENT TYPE (from image analysis)                    │
│  [Single Vehicle ▼]                                     │
│                                                        │
│  NUMBER OF VEHICLES/OBJECTS DETECTED                    │
│  [2]                                                    │
│                                                        │
│  ESTIMATED IMPACT TYPE                                  │
│  [T-Bone ▼]                                             │
│                                                        │
│  VEHICLE 1 POSITION (before impact)                     │
│  [North ▼]                                              │
│                                                        │
│  VEHICLE 1 DIRECTION OF TRAVEL (before impact)          │
│  [North ▼]                                              │
│                                                        │
│  [If Multi-Vehicle] VEHICLE 2 POSITION                  │
│  [South ▼]                                              │
│                                                        │
│  [If Multi-Vehicle] VEHICLE 2 DIRECTION OF TRAVEL       │
│  [South ▼]                                              │
│                                                        │
│  DAMAGE ZONES DETECTED (check all that apply)           │
│  ☐ Front     ☐ Rear     ☐ Driver Side                   │
│  ☐ Passenger Side     ☐ Roof     ☐ Undercarriage        │
│                                                        │
│  ROAD/SCENE FACTORS (check all that apply)              │
│  ☐ Intersection     ☐ Lane Merge     ☐ Parked Vehicle   │
│  ☐ Median/Divider   ☐ Gravel/Dirt    ☐ Wet Surface      │
│                                                        │
│  CONFIDENCE LEVEL IN THIS ANALYSIS                      │
│  [High ▼]  (High / Medium / Low)                         │
│                                                        │
│  ASSUMPTIONS & AMBIGUITIES (free text)                  │
│  ┌──────────────────────────────────────────────────┐  │
│  │ [Cursor here. Type any notes about confidence   │  │
│  │  or things that couldn't be determined from     │  │
│  │  the image.]                                     │  │
│  └──────────────────────────────────────────────────┘  │
│                                                        │
│                    < Back   [Next >]                    │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Field Descriptions & Enums:**

| Field | Type | Required | Options/Format |
|-------|------|----------|--------|
| Incident Type (from image) | Dropdown | Yes | Single Vehicle, Multi-Vehicle, Parked Vehicle, Unknown |
| Number of Vehicles/Objects | Text (int) | Yes | 1, 2, 3, etc. |
| Estimated Impact Type | Dropdown | Yes | Head-On, T-Bone, Rear-End, Side-Swipe, Unknown |
| Vehicle 1 Position | Dropdown | Yes | North, South, East, West, Center |
| Vehicle 1 Direction of Travel | Dropdown | Yes | N, NE, E, SE, S, SW, W, NW |
| Vehicle 2 Position | Dropdown | Conditional (if multi-vehicle) | North, South, East, West, Center |
| Vehicle 2 Direction of Travel | Dropdown | Conditional (if multi-vehicle) | N, NE, E, SE, S, SW, W, NW |
| Damage Zones Detected | Checkboxes | Yes (at least 1) | Front, Rear, Driver Side, Passenger Side, Roof, Undercarriage |
| Road/Scene Factors | Checkboxes | Yes (at least 1) | Intersection, Lane Merge, Parked Vehicle, Median/Divider, Gravel/Dirt, Wet Surface |
| Confidence Level | Dropdown | Yes | High, Medium, Low |
| Assumptions & Ambiguities | Multi-line Text | Yes (but can be empty or brief) | Free text, max 500 characters |

**Interaction:**
1. Review image preview at top
2. Click Incident Type dropdown, select option
3. Enter Number of Vehicles
4. Click Estimated Impact Type, select option
5. Click Vehicle 1 Position, select option
6. Click Vehicle 1 Direction of Travel, select option
7. If multi-vehicle, click Vehicle 2 Position and Direction
8. Check damage zones (at least one required)
9. Check road/scene factors (at least one required)
10. Click Confidence Level, select option
11. Click into Assumptions field and enter any notes
12. Once all required fields filled, **[Next >]** button becomes enabled
13. Click **[Next >]** to proceed to Page 5

**Validation Rules:**
- ALL dropdown fields must be selected (no blank allowed)
- At least ONE damage zone checkbox must be checked
- At least ONE road/scene factor checkbox must be checked
- Confidence Level must be selected
- Assumptions field can be empty, but if agent provides notes, they are recorded
- **If validation fails, error message displayed at top of form in red**
- Agent must correct and retry before proceeding

**Agent Behavior — Key Points:**
- **Always prefill all fields** (via external image analysis before agent navigates to this page)
- **Never submit with incomplete image analysis**
- If confidence is **Low** and agent must proceed, agent should ask user one clarifying question (e.g., "Was the impact head-on or T-bone?") and update the field before clicking Next
- If image could not be processed, display "No image provided" and block next unless image re-uploaded

---

### 5.5 Page 5: Vehicle & Injury Details

**Screen title:** `NEW CLAIM — Page 5 of 6: Vehicle & Injury Information`

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  VEHICLE & INJURY INFORMATION                           │
│                                                        │
│  PRIMARY VEHICLE (at-fault)                             │
│  ────────────────────────────────                       │
│  Make:          [____________________]  (e.g., Toyota)  │
│  Model:         [____________________]  (e.g., Camry)   │
│  Year:          [____________________]  (e.g., 2022)    │
│  Color:         [____________________]  (e.g., Blue)    │
│  License Plate: [____________________]  (optional)      │
│  Damage Level:  [Moderate ▼]                            │
│                                                        │
│  OTHER VEHICLE (if applicable)                          │
│  ────────────────────────────────                       │
│  ☐ Multi-vehicle incident (check to enable)             │
│                                                        │
│  Make:          [____________________]  (grayed if no)  │
│  Model:         [____________________]                  │
│  Year:          [____________________]                  │
│  Color:         [____________________]                  │
│  License Plate: [____________________]                  │
│  Damage Level:  [Moderate ▼]                            │
│                                                        │
│  INJURY & WITNESS INFO                                  │
│  ────────────────────────────────                       │
│  Injuries Reported:     ○ Yes   ○ No  (required)        │
│  Witness Present:       ○ Yes   ○ No  (required)        │
│  Witness Name (if yes): [____________________] (optional)│
│                                                        │
│                    < Back   [Next >]                    │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Fields:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Primary Vehicle Make | Text | No | e.g., Toyota, Ford |
| Primary Vehicle Model | Text | No | e.g., Camry, Mustang |
| Primary Vehicle Year | Text (int) | No | 4-digit year, e.g., 2022 |
| Primary Vehicle Color | Text | No | e.g., Blue, Silver |
| Primary Vehicle License Plate | Text | No | e.g., ABC-1234 |
| Primary Vehicle Damage Level | Dropdown | No | None, Minor, Moderate, Severe, Total Loss |
| Multi-vehicle Incident | Checkbox | No | If checked, enables other vehicle fields |
| Other Vehicle Make | Text | Conditional | Enabled if multi-vehicle |
| Other Vehicle Model | Text | Conditional | Enabled if multi-vehicle |
| Other Vehicle Year | Text | Conditional | Enabled if multi-vehicle |
| Other Vehicle Color | Text | Conditional | Enabled if multi-vehicle |
| Other Vehicle License Plate | Text | Conditional | Enabled if multi-vehicle |
| Other Vehicle Damage Level | Dropdown | Conditional | Enabled if multi-vehicle |
| Injuries Reported | Radio (Yes/No) | Yes | |
| Witness Present | Radio (Yes/No) | Yes | |
| Witness Name | Text | Conditional | Enabled if "Witness Present" = Yes; optional even if enabled |

**Interaction:**
1. Type Primary Vehicle info (optional fields, but recommended)
2. If multi-vehicle, check the "Multi-vehicle Incident" checkbox
3. Fill in Other Vehicle info (now enabled)
4. Select "Injuries Reported" Yes or No
5. Select "Witness Present" Yes or No
6. If witness, optionally type witness name (or leave blank)
7. Click **[Next >]** to proceed to Page 6

**Dropdown Options:**

**Damage Level:**
- None
- Minor (small scratches, dent)
- Moderate (multiple dents, broken window, bumper damage)
- Severe (frame damage, door won't open)
- Total Loss (vehicle not repairable)

---

### 5.6 Page 6: Review & Submit

**Screen title:** `NEW CLAIM — Page 6 of 6: Review & Submit`

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  REVIEW CLAIM SUMMARY                                   │
│                                                        │
│  ═══════════════════════════════════════════════════   │
│  REQUIRED FIELDS CHECKLIST                              │
│  ═══════════════════════════════════════════════════   │
│                                                        │
│  ✓ Claimant Information (Name, Phone)                   │
│  ✓ Incident Details (Date, Location, Type)              │
│  ✓ Image Upload & Analysis (COMPLETE)                   │
│  ✓ Vehicle & Injury Info                                │
│                                                        │
│  ═══════════════════════════════════════════════════   │
│  CLAIM SUMMARY                                          │
│  ═══════════════════════════════════════════════════   │
│                                                        │
│  Claimant: Alex Johnson                                 │
│  Phone: 555-0101                                        │
│  Incident Date: 06/22/2026, 14:30                       │
│  Location: 123 Main St, Anytown, CA 90210               │
│  Incident Type: Multi-Vehicle T-Bone Impact             │
│  Image Analysis Confidence: High                        │
│  Vehicles: 2 (Toyota Camry — Moderate, Ford F-150 — Severe) │
│  Injuries: No    Witness: Yes (John Smith)              │
│                                                        │
│  ═══════════════════════════════════════════════════   │
│                                                        │
│              [Back to Edit]  [Save as Draft]  [Submit] │
│                                                        │
│  * All required fields must be complete to submit      │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Interaction:**
1. Review summary of all entered data
2. If correction needed, click **[Back to Edit]** to return to any page
3. To save without submitting, click **[Save as Draft]** (optional flow)
4. To submit, click **[Submit]**

**Validation Before Submit:**
- Claimant name and phone must be present
- Incident date and location must be present
- Image analysis section must be 100% complete (all required fields filled)
- Injuries and Witness questions must be answered

If validation fails, error message displayed in red at top, form does not close.

**On Successful Submit:**
- Claim is saved to database with status "Submitted for Review"
- Unique claim ID is generated (e.g., CLM-2026-001234)
- Screen transitions to confirmation page

---

### 5.7 Confirmation Page

**Screen title:** `CLAIM SUBMITTED SUCCESSFULLY`

**Layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  ✓ CLAIM SUBMITTED SUCCESSFULLY                         │
│                                                        │
│  Your claim has been submitted for adjuster review.     │
│                                                        │
│  Claim Number: CLM-2026-001234                          │
│  Claimant: Alex Johnson                                 │
│  Status: Submitted for Review                           │
│                                                        │
│  Next steps:                                            │
│  • An adjuster will contact you within 24 hours         │
│  • Your claim number is CLM-2026-001234                 │
│  • Reference this number in all correspondence           │
│                                                        │
│                   [Return to Main Menu]                │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Interaction:**
1. Agent reads confirmation and claim number
2. Agent reports back to user with claim number
3. Agent clicks **[Return to Main Menu]** to start next claim or log off

---

## 6. Search Existing Claim

**Trigger:** Main Menu → "Search Existing Claim" option

**Screen layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  SEARCH CLAIMS                                          │
│                                                        │
│  Search by:  ○ Claim ID    ○ Claimant Name   (required) │
│                                                        │
│  Enter search term:  [____________________________]     │
│                                                        │
│                      [Search]  [Back]                   │
│                                                        │
│  Results (if search performed):                         │
│  ┌────────────────────────────────────────────────┐   │
│  │ CLM-2026-001234  Alex Johnson  06/22/2026      │   │
│  │ CLM-2026-001235  Sarah Davis   06/21/2026      │   │
│  └────────────────────────────────────────────────┘   │
│                                                        │
│  [Select claim to view details]                         │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Interaction:**
1. Select search method (Claim ID or Claimant Name)
2. Enter search term
3. Click **[Search]**
4. Results displayed
5. Click claim to view full details (read-only)

---

## 7. System Administration (Admin Only)

**Trigger:** Main Menu → "System Administration" (accessible only to admin user)

**Screen layout:**
```
┌────────────────────────────────────────────────────────┐
│ Auto Claims FNOL — Intake System                        │
├────────────────────────────────────────────────────────┤
│                                                        │
│  SYSTEM ADMINISTRATION                                  │
│  (Admin access only)                                    │
│                                                        │
│  [ ] Reset Database to Defaults                         │
│  [ ] View Application Version & Build Info              │
│  [ ] View Database Statistics                           │
│  [ ] Return to Main Menu                                │
│                                                        │
│                              [Select]                  │
│                                                        │
└────────────────────────────────────────────────────────┘
```

**Admin Options:**

### Reset Database to Defaults
- **Confirmation dialog:** "Are you sure? This will erase all claims and reset to factory defaults. This cannot be undone."
- If confirmed:
  - All claims deleted
  - Database re-seeded with reference data
  - Message: "Database reset successfully. System ready for demo."
  - Return to Main Menu

### View Application Version & Build Info
- Shows:
  - Application: Auto Claims FNOL
  - Version: 1.0.0
  - Build Date: [date]
  - Database Schema Version: 1.0
  - SQLite Version: [version]
  - OK button to return

### View Database Statistics
- Shows:
  - Total Claims: [count]
  - Claims Submitted: [count]
  - Claims Draft: [count]
  - Last Claim Created: [timestamp]

---

## 8. Field Data Types & Validation

### Text Fields
- Claimant Name: 3–50 characters, letters + spaces
- Phone: Must contain digits (format flexible: 555-0101, 5550101, etc.)
- Email: Optional, must match basic regex if provided
- Address: 5–100 characters
- License Plate: Optional, 0–10 characters

### Numeric Fields
- Year: 4 digits (1900–2099)
- Number of Vehicles: 1–5
- Time (HH:MM): 00:00 – 23:59

### Date Fields
- Incident Date: MM/DD/YYYY, must not be in future

---

## 9. Common Error Messages & Recovery

| Error | Meaning | Recovery |
|-------|---------|----------|
| "Claimant name is required" | Name field empty | Go back to Page 1, enter name, retry |
| "Please select an incident type" | Dropdown not selected on Page 2 or 4 | Select option from dropdown, retry |
| "Image analysis is incomplete" | One or more required fields on Page 4 empty | Complete all fields on Page 4, retry |
| "At least one damage zone must be checked" | No checkboxes selected on Page 4 | Check at least one, retry |
| "Please answer all required questions" | Injuries or Witness question not answered on Page 5 | Select Yes or No, retry |
| "Invalid date format" | Date entered incorrectly | Use MM/DD/YYYY format, retry |
| "Invalid email format" | Email does not match pattern | Correct email or leave blank, retry |

**Agent Recovery Pattern:**
1. Read error message
2. Identify which page/field is invalid
3. Navigate back to that page (click Back buttons as needed)
4. Correct the field
5. Click Next to re-proceed through subsequent pages (form remembers prior entries)
6. Retry submit

---

## 10. Agent Best Practices & Key Behaviors

### Always Screenshot First
Before any action, take a screenshot to confirm current screen state.

### Image Analysis is the MVP
- Treat image upload and image analysis section as the core deliverable
- Never skip or minimize this section
- If image analysis incomplete, do not proceed past Page 4

### Confidence-Gated Clarification
- If Image Analysis confidence is "Low" on critical fields (impact type, vehicle position), ask user one clarifying question
- Example: "I analyzed the image as a T-Bone impact, but I'm not fully confident. Can you confirm the type of collision?"
- Update field based on user answer, then proceed

### No Fabrication
- Never invent witness names if not visible in image or provided by user
- Never fabricate police report numbers
- Never assert injury presence if not indicated

### Deterministic Navigation
- Follow prescribed page order: 1 → 2 → 3 → 4 → 5 → 6 → Submit
- Never attempt alternate navigation paths

### Validation Discipline
- Always check for and read validation error messages
- Correct fields and retry; do not circumvent validation

### Completion Assurance
- Before clicking Submit, mentally verify all visible required fields are filled
- Read the Review page summary to confirm data accuracy

---

## 11. Demo Walkthrough Example

**Scenario:** Hand-drawn accident sketch (2-vehicle T-bone)

**Agent steps:**
1. Screenshot → Verify "Login" screen
2. Type `adjuster1` in Username
3. Tab to Password, type `pass123`
4. Click Login button
5. Screenshot → Verify Main Menu
6. Select "New Claim", click Select
7. Screenshot → Verify Page 1 (Claimant)
8. Enter: Name = "Alex Johnson", Phone = "555-0101"
9. Click Next
10. Screenshot → Verify Page 2 (Incident)
11. Set Date = 06/22/2026, Location = "123 Main St, Anytown, CA"
12. Set Type = Multi-Vehicle, Weather = Clear, Road = Dry
13. Set Police Report = No
14. Click Next
15. Screenshot → Verify Page 3 (Image Upload)
16. Click Browse, select hand-drawn sketch file
17. Screenshot → Verify preview shows image
18. Click Next
19. Screenshot → Verify Page 4 (Image Analysis) — all fields prefilled
20. Confirm: Incident Type = Multi-Vehicle, Impact = T-Bone, Vehicles = 2
21. Check damage zones: Front, Rear
22. Check road factors: Intersection
23. Set Confidence = High
24. Add note: "Clear T-bone at intersection, vehicles at right angles"
25. Click Next
26. Screenshot → Verify Page 5 (Vehicles)
27. Enter: Primary vehicle Toyota Camry, moderate damage
28. Check "Multi-vehicle" checkbox
29. Enter: Other vehicle Ford F-150, severe damage
30. Set Injuries = No, Witness = Yes, Witness name = optional
31. Click Next
32. Screenshot → Verify Page 6 (Review)
33. Read summary, confirm all correct
34. Click Submit
35. Screenshot → Verify Confirmation page with claim number
36. Report back to user: "Claim submitted successfully as CLM-2026-001234"

---

## 12. Appendix: Seed Data (Reference)

### Predefined Users
| Username | Password | Name | Branch | Role |
|----------|----------|------|--------|------|
| adjuster1 | pass123 | Sarah Chen | Downtown | Claims Adjuster |
| adjuster2 | pass123 | Mike Davis | West Side | Claims Adjuster |
| admin | admin | System Admin | HQ | Administrator |

### Pre-Loaded Reference Scenarios (in database after reset, but no associated claims)
- 2-vehicle T-bone at intersection
- 3-vehicle pile-up on highway
- Single-vehicle hit parked car
- Multi-vehicle rear-end chain collision
- Motorcycle vs. car intersection impact

These are reference records, not submitted claims. They help populate database size for demo statistics view.

---

## 13. What's Out of Scope

- Real image ML/AI backend (uses mock extraction for demo)
- Actual adjuster assignment workflow
- Document attachment beyond one image
- Policy lookup / underwriting rules
- Payment / settlement module
- Mobile app
- Web portal
- Real insurance data

