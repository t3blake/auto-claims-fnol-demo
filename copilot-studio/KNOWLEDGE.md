# Auto Claims FNOL — Knowledge Base

This document is uploaded as a **Knowledge Source** in Copilot Studio. The agent searches it when building understanding of the system, workflows, and edge cases.

---

## 1. System Overview

**Auto Claims FNOL** is a legacy desktop application for intake of first-notice-of-loss (FNOL) auto insurance claims.

- **Platform:** Windows desktop (WPF, legacy appearance)
- **Purpose:** Adjuster-facing claims intake tool
- **Core workflow:** Login → New Claim → Multi-page form (claimant, incident, image, image analysis, vehicles, injuries) → Submit to adjuster queue
- **Database:** SQLite, local, persistent
- **No APIs or backend services** — all data local to the machine

---

## 2. Login Credentials

### Standard Users

| Username | Password | Role | Notes |
|----------|----------|------|-------|
| adjuster1 | pass123 | Claims Adjuster | Recommended for agent workflows |
| adjuster2 | pass123 | Claims Adjuster | Alternative adjuster |
| admin | admin | System Administrator | For database reset only |

All credentials are hardcoded for demo purposes. No real PII.

---

## 3. Main Menu Options

After successful login, the app displays Main Menu with four options:

1. **New Claim** — Start multi-page claim intake wizard
2. **Search Existing Claim** — Look up submitted claim by ID or claimant name
3. **System Administration** — Admin-only: reset database, view version info, view stats
4. **Log Off** — Return to login screen

---

## 4. Create New Claim — 6-Page Wizard

### Page 1: Claimant Information

**Required fields:**
- Claimant Full Name (3–50 characters)
- Phone Number (must contain digits)

**Optional fields:**
- Email Address
- Policy Number
- Alternate Phone

Navigation: Next / Back / Cancel

---

### Page 2: Incident Details

**Required fields:**
- Incident Date (MM/DD/YYYY, not future)
- Incident Location (street address)
- Incident Type (dropdown: Single Vehicle, Multi-Vehicle, Parked Vehicle, Other)
- Weather Conditions (dropdown: Clear, Rain, Snow, Fog, Dark, Overcast, Other)
- Road Conditions (dropdown: Dry, Wet, Ice/Snow, Gravel, Pothole/Debris, Other)
- Police Report Filed (radio: Yes / No)

**Conditional field:**
- Police Report Number (enabled only if Police Report = Yes)

**Optional field:**
- Incident Time (HH:MM, 24-hour format)

Navigation: Next / Back / Cancel

---

### Page 3: Image Upload & Preview

**Purpose:** User uploads hand-drawn accident sketch or photo.

**Accepted formats:** .jpg, .png, .bmp  
**Max size:** 10 MB

**Interaction:**
- Click Browse button to open file chooser
- Select image file
- Image preview displays below
- Status shows "Image attached" with filename
- Can click Clear to remove image

**Note:** Image upload is optional at this stage, but Page 4 (Image Analysis) requires image data to be meaningful.

Navigation: Next / Back / Cancel

---

### Page 4: Image Analysis (HARD REQUIRED)

**This section is the core deliverable.** Form cannot submit without all fields complete.

**Auto-populated by agent vision analysis:**
- Incident Type (from image): dropdown
- Number of Vehicles/Objects Detected: integer
- Estimated Impact Type: dropdown
- Vehicle Positions (1 and 2, if multi-vehicle): dropdown positions
- Directions of Travel (1 and 2): compass directions
- Damage Zones Detected: checkboxes (at least 1 required)
- Road/Scene Factors: checkboxes (at least 1 required)
- Confidence Level: dropdown (High, Medium, Low)
- Assumptions & Ambiguities: free text, optional but recommended

**Dropdown values:**

**Incident Type (from image):**
- Single Vehicle
- Multi-Vehicle
- Parked Vehicle (hit stationary vehicle)
- Unknown

**Impact Type Estimate:**
- Head-On (vehicles collide head-first, opposite directions)
- T-Bone (right-angle collision)
- Rear-End (following vehicle hits rear of leading vehicle)
- Side-Swipe (vehicles glance each other, parallel paths)
- Unknown

**Positions:**
- North
- South
- East
- West
- Center

**Directions of Travel:**
- N, NE, E, SE, S, SW, W, NW (compass)

**Damage Zones:**
- Front (bumper, hood, windshield)
- Rear (trunk, rear bumper, back windshield)
- Driver Side (left side of vehicle)
- Passenger Side (right side of vehicle)
- Roof (roof damage)
- Undercarriage (underside, alignment issues)

**Road/Scene Factors:**
- Intersection (multi-direction cross-traffic)
- Lane Merge (vehicles changing lanes)
- Parked Vehicle (stationary or parking-related)
- Median/Divider (highway median, guardrail involved)
- Gravel/Dirt (non-standard road surface)
- Wet Surface (rain, puddles, wet conditions visible)

**Confidence Level:**
- High (clear, unambiguous interpretation)
- Medium (mostly clear, some uncertainty on one element)
- Low (significant ambiguity, key details unclear)

**Validation:**
- All dropdowns required
- At least 1 damage zone checkbox required
- At least 1 road/scene factor checkbox required
- Confidence level required
- Assumptions field can be empty, but any text is recorded

Navigation: Next (blocked until all required fields filled) / Back / Cancel

---

### Page 5: Vehicle & Injury Information

**Primary Vehicle (required concept, fields optional):**
- Make, Model, Year, Color
- License Plate (optional)
- Damage Level: None, Minor, Moderate, Severe, Total Loss

**Multi-Vehicle Indicator:**
- Checkbox: "Multi-vehicle incident" (enables Other Vehicle fields)

**Other Vehicle (conditional on multi-vehicle checkbox):**
- Make, Model, Year, Color, License Plate, Damage Level (same as primary)

**Injury & Witness Information (required):**
- Injuries Reported (radio: Yes / No)
- Witness Present (radio: Yes / No)
- Witness Name (text, optional even if Yes selected)

Navigation: Next / Back / Cancel

---

### Page 6: Review & Submit

**Display:**
- Checklist of required sections (all should show ✓)
- Summary of all entered data
- Claimant name, phone, incident details, image analysis confidence, vehicle summary, injury/witness flags

**Buttons:**
- Back to Edit (returns to any page)
- Save as Draft (optional alternate path)
- Submit (submits to adjuster queue, primary path)

**Validation Before Submit:**
- All required fields on all pages must be complete
- Image analysis section must be 100% complete
- Database saves claim with status "Submitted for Review"
- Unique claim ID generated (e.g., CLM-2026-001234)

---

### Confirmation Page

**Display:**
- Success message in green
- Claim number (e.g., CLM-2026-001234)
- Claimant name and status: "Submitted for Review"
- Next steps message (adjuster will contact within 24 hours)

**Button:**
- Return to Main Menu

---

## 5. Search Existing Claim

**Inputs:**
- Search type (radio: Claim ID or Claimant Name)
- Search term (text field)

**Results:**
- List of matching claims with ID, claimant, incident date
- Click to view full claim details (read-only)

---

## 6. System Administration (Admin Only)

**Options:**
1. **Reset Database to Defaults**
   - Confirmation: "Are you sure? This will erase all claims and reset to factory defaults."
   - If confirmed: All claims deleted, database re-seeded with reference data
   - Message: "Database reset successfully. System ready for demo."
   - Return to Main Menu

2. **View Application Version & Build Info**
   - Displays: App name, version (1.0.0), build date, schema version
   - OK to dismiss

3. **View Database Statistics**
   - Displays: Total claims count, submitted count, draft count, last claim created timestamp

---

## 7. Error Messages & Recovery

| Error Message | Cause | Recovery |
|---------------|-------|----------|
| "Claimant name is required" | Name field empty on Page 1 | Go back, enter name, retry |
| "Please enter a valid phone number" | Phone empty or no digits on Page 1 | Go back, enter phone, retry |
| "Invalid date format" | Date not MM/DD/YYYY on Page 2 | Go back, correct date, retry |
| "Please select an incident type" | Dropdown on Page 2 or 4 not selected | Go back, select from dropdown, retry |
| "Image analysis is incomplete" | One or more required fields on Page 4 empty | Complete all fields on Page 4, retry |
| "At least one damage zone must be checked" | No checkboxes on Page 4 damage zones | Check at least one, retry |
| "Please answer all required questions" | Injury or Witness radio not selected on Page 5 | Select Yes/No, retry |
| "All required fields must be complete to submit" | Generic validation failure on Page 6 | Review summary, go back, correct, retry |

---

## 8. Workflow Patterns

### Typical Successful Flow
1. Login (adjuster1 / pass123)
2. Main Menu → New Claim
3. Fill 6 pages in order
4. Submit from Page 6
5. View confirmation with claim number
6. Return to Main Menu or log off

### With Confidence Check (Agent Asks User)
1. Pages 1–3 (claimant, incident, image)
2. Page 4: Agent analyzes image, confidence is Medium or Low on impact type
3. Agent pauses and asks: "Based on the image, I think the impact was [Type], but I'm not fully confident. Can you confirm?"
4. User provides clarification
5. Agent updates field, continues
6. Pages 5–6, submit

### With Incomplete Data
1. Pages 1–5 completed
2. Page 6 (Review): Agent notices Police Report is blank (optional, OK) but Image Analysis missing (required, NOT OK)
3. Error message displayed: "Image analysis is incomplete"
4. Agent goes back to Page 4, reviews what's missing, completes it, retries submit

### Database Reset
1. Log in as admin
2. Main Menu → System Administration → Reset Database to Defaults
3. Confirm dialog twice
4. Database cleared, app ready for next demo

---

## 9. Field Constraints & Validation Rules

### Text Fields
- **Claimant Name:** 3–50 characters, alphanumeric + spaces
- **Phone:** Must contain at least one digit (format flexible)
- **Email:** Must match basic email regex if provided
- **Address:** 5–100 characters
- **License Plate:** 0–10 characters, optional
- **Witness Name:** 0–50 characters, optional
- **Assumptions:** 0–500 characters, free text

### Numeric Fields
- **Year:** 4 digits (1900–2099)
- **Number of Vehicles:** 1–5
- **Time (HH:MM):** 00:00 – 23:59

### Date Fields
- **Incident Date:** MM/DD/YYYY format, must not be in future

### Dropdown Fields
- All required dropdowns must have a selection (no blank)

### Checkbox Fields
- At least one must be checked if group is required (e.g., damage zones)

---

## 10. Demo Scenarios (Reference)

### Scenario 1: Simple Two-Vehicle T-Bone
- Hand-drawn sketch showing clear 90-degree collision
- Agent uploads image, extracts: 2 vehicles, T-bone, intersection
- All confidence high
- No clarification needed
- Submit successful

### Scenario 2: Ambiguous Photo + Clarification
- Real or hand-drawn photo with unclear impact angle
- Agent recognizes medium confidence on impact type
- Agent asks: "Was this head-on or T-bone?"
- User responds
- Agent proceeds with updated data

### Scenario 3: Complex Multi-Vehicle Pile-Up
- Three vehicles, chain-reaction rear-end
- Image analysis extraction more complex
- Possible injuries and witness
- Agent handles without getting confused
- Submit successful

---

## 11. Agent Behavior Guidelines (Key Priorities)

1. **Image analysis is the MVP.** Treat it as the core deliverable. Never skip or minimize.
2. **Confidence gates decisions.** If confidence < High on critical fields, ask user one clarifying question.
3. **No fabrication.** Don't invent witness names, police report numbers, or injuries.
4. **Deterministic navigation.** Follow prescribed page order: 1 → 2 → 3 → 4 → 5 → 6 → Submit.
5. **Hard-required fields.** Image analysis cannot be partial. All fields must be filled.
6. **Error recovery.** Read error messages, go back, correct, retry.
7. **Validation discipline.** Never try to submit with incomplete data.
8. **Completion assurance.** Before final submit, verify all visible required fields are filled.

---

## 12. What to Search This Knowledge Base For

**Agent, use this knowledge base to find:**
- Valid dropdown values for any field
- Required vs. optional field status
- How to handle validation errors
- What constitutes successful image analysis completion
- Login credentials
- Error message meanings and recovery steps
- Typical successful workflows
- Demo scenario patterns
- Field constraints and validation rules

