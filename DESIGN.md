# Auto Claims FNOL — Legacy Claims Management System

## Design Document

> **Note:** This is a design-first specification. All decisions are locked before implementation begins.

> **How to read this doc:** Sections 1–15 are the **original design** — the locked, pre-implementation specification. The final **Lessons Learned After Testing** section records where the shipped agent diverged once we ran it against a real Windows 365 Cloud PC. Read the design for intent; read the lessons for what actually works.

---

## 1. Project Goal

Build a legacy **auto insurance claims intake application** that demonstrates how a **Copilot Studio Computer Use Agent (CUA)** can:

1. Accept a hand-drawn accident sketch or photo provided by a user
2. Analyze the image using vision capabilities
3. Extract incident reconstruction details (impact point, vehicle positions, road factors)
4. Fill a legacy desktop claims form end-to-end
5. Submit the claim for adjuster review

The "legacy" aesthetic signals "no API, no modern integration" — the agent reads and types like a human would.

---

## 2. Design Principles

| # | Principle | Rationale |
|---|-----------|-----------|
| 1 | **Image-driven claims intake** | User provides visual context. Agent extracts facts. App captures structured output. Demonstrates real CUA value. |
| 2 | **Explicit image-analysis section** | Not hidden. Required. Shows what CUA actually did with the image. Users understand the agent's reasoning. |
| 3 | **Hard-block on missing analysis** | Form cannot submit without image details section complete. Prevents agent skipping core value-add work. |
| 4 | **Autonomous with one clarification** | CUA fills most fields independently. Only asks user to confirm if confidence is low on a critical field (e.g., "Was impact head-on or T-bone?"). Balances speed with accuracy. |
| 5 | **Legacy desktop UI** | Windows forms-era appearance. No web, no modern SPA. Signals "no backend API". Fixed layout, clearly deterministic screens. |
| 6 | **Deterministic screen states** | Every screen has unambiguous visual markers. CUA can reliably detect state and act. |
| 7 | **Constrained inputs** | Dropdowns, checkboxes, radio buttons wherever possible. Reduces freeform typing risk. CUA maps observations to enum values. |
| 8 | **Resettable demo state** | Admin reset function re-seeds database. Every demo starts clean. |
| 9 | **No real PII** | All test names, addresses, phone numbers are obviously fake. No real SSN/VIN patterns. |
| 10 | **Single-file portable exe** | Drop on a Windows machine, double-click, works. No runtime, no installer pain. |

---

## 3. End-to-End User + Agent Workflow

### User Entry Point
User in Teams or M365 Chat: *"I have a photo of an accident. Can you file a claim for me?"*
User attaches image file (PNG, JPG, or hand-drawn sketch).

### Agent Workflow (via CUA)
1. **Launch app** → Desktop shortcut or Run dialog
2. **Login** → Predefined claims adjuster credentials
3. **Create New Claim** → Navigate from main menu
4. **Upload image** → File chooser, select user's image
5. **Image analysis section**
   - CUA vision reads the sketch/photo
   - Populates: vehicle count, impact type, direction of travel, road factors, damage zones, confidence level, assumptions/ambiguities
6. **Claimant details** → Names, contact info (agent uses sensible defaults or asks user if needed)
7. **Incident details** → Date, time, location, incident type, weather, road conditions
8. **Loss details** → Vehicle damage summary, injury indicator, police report indicator, witness info
9. **Validation page** → App checks all required fields (blocks submit if image analysis is incomplete)
10. **Submit to adjuster queue** → Claim stored with state "Submitted for Review"
11. **Confirmation** → Agent reports back to user with claim number and next steps

### Demo Reset
After demo ends, claims adjuster logs in, goes to Admin, selects "Reset Database to Defaults" → all claims wiped, app returns to clean state for next demo.

> **As built:** the shipped agent moved image analysis up front (Work IQ Copilot) and runs a single Computer Use pass *after* confirming fields with the user — so step 5's "CUA vision reads the sketch" happens before the form is touched, not during it. See **Lessons Learned After Testing** at the end.

---

## 4. Core Domain Model (Auto Accident FNOL)

### Entities

**Claim Header**
- Claim ID (auto-generated)
- Claimant name, phone, email
- Policy number (optional)
- Incident date/time
- Status: [Draft, Submitted, Assigned, Closed]
- Submission timestamp
- Adjuster assigned (defaults to null until review)

**Incident Details**
- Type: [Single Vehicle, Multi-Vehicle, Parked Vehicle, Other]
- Location address
- Weather: [Clear, Rain, Snow, Fog, Dark, Other]
- Road: [Highway, Local Road, Intersection, Parking Lot, School Zone, Construction Zone]
- Police report filed: Yes/No
- Police report number (if yes)

**Vehicle(s) Involved**
- Vehicle 1: Make, Model, Year, Color, License Plate (or "Unknown")
- Vehicle 1 position: [North, South, East, West, Center]
- Vehicle 1 damage: [None, Minor, Moderate, Severe, Total Loss]
- Vehicle 1 direction of travel before impact: [N, NE, E, SE, S, SW, W, NW]
- Vehicle 2 (if multi-vehicle): same fields
- Vehicle position diagram interpretation (from image analysis)

**Injury & Witnesses**
- Injuries reported: Yes/No
- Witness present: Yes/No
- Witness name (if yes, optional — can be blank if agent cannot confirm)

**Image Analysis** (Required, populated by CUA)
- Image filename
- Extraction timestamp
- Incident type guess (from image): [Single Vehicle, Multi-Vehicle, Parked Vehicle, Unknown]
- Number of vehicles/objects detected
- Vehicle 1 position estimate
- Vehicle 2 position estimate (if present)
- Impact type estimate: [Head-On, T-Bone, Rear-End, Side Swipe, Unknown]
- Direction of travel per vehicle (before impact)
- Damage zone estimates: [Front, Rear, Driver Side, Passenger Side, Roof, Undercarriage]
- Road/scene factors detected: [Intersection, Lane Merge, Parked Vehicle Involved, Median/Divider, Gravel/Dirt, Wet Surface]
- Confidence level: [High, Medium, Low]
- Assumptions & ambiguities (free text)
- **This section is HARD-REQUIRED before form can submit.**

**Narrative** (Optional, but recommended)
- Free-text description typed or summarized by agent from image analysis + user clarifications

---

## 5. Application Architecture

### Technology Stack

| Component | Choice | Rationale |
|-----------|--------|-----------|
| **Language** | C# / .NET 8 | Familiar to enterprise demos, strong tooling |
| **UI Framework** | WPF | Desktop GUI, deterministic rendering, legacy aesthetic control |
| **Publish mode** | Self-contained single-file exe | Drop-and-go, no runtime pre-install |
| **Database** | SQLite (`Microsoft.Data.Sqlite`) | Embedded, portable, persistent, resettable |
| **Version** | 1.0.0 | Initial release |

### Layers

```
┌─────────────────────────────────────────────┐
│        Copilot Studio Agent (CUA)            │
│        (operates via screenshot/click/type)  │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│        Legacy Claims Desktop App (WPF)       │
│      ┌───────────────────────────────────┐  │
│      │    Screen Renderer (Forms UI)     │  │
│      ├───────────────────────────────────┤  │
│      │    Input Handler (TextBox/etc)    │  │
│      ├───────────────────────────────────┤  │
│      │    Business Logic (Claim workflow)│  │
│      ├───────────────────────────────────┤  │
│      │    Data Access (SQLite queries)   │  │
│      └───────────────────────────────────┘  │
└──────────────────┬──────────────────────────┘
                   │
┌──────────────────▼──────────────────────────┐
│      SQLite Database (claims-fnol.db)        │
└──────────────────────────────────────────────┘
```

### Visual Design

| Property | Value |
|----------|-------|
| Window size | 1024 × 768 px (default), minimum 800 × 600 |
| Background | Light gray (`#E8E8E8`) — Windows 2000-era standard |
| Text color | Dark gray/black (`#000000`) |
| Control appearance | Classic Windows forms: gray buttons, text input boxes, dropdown lists, checkboxes |
| Font | **Segoe UI 10pt** — corporate desktop standard |
| Aesthetic era | 2000–2005 (unmistakably pre-cloud, pre-Bootstrap) |
| Determinism | Every form state has unambiguous screen title, field labels, and button layout |

---

## 6. Screen Flow & Data Contracts

### Login Screen
- App launch (manual)
- Two inputs: Username, Password
- Credentials (hardcoded for demo):
  - `adjuster1` / `pass123` → Claims Adjuster (Downtown Branch)
  - `admin` / `admin` → System Admin
- Success → Main Menu
- Failure (3 attempts) → Lock and exit

### Main Menu
- New Claim
- Search Claim
- System Administration
- Log Off

### Create New Claim (Flow)
1. **Claim Intake Form (Page 1: Claimant)**
   - Claimant Name (required)
   - Phone (required)
   - Email (optional)
   - Policy # (optional)
   - Next / Back / Cancel

2. **Claim Intake Form (Page 2: Incident)**
   - Incident Date (date picker or text)
   - Incident Time (HH:MM, optional)
   - Incident Location (address, required)
   - Incident Type (dropdown: Single Vehicle, Multi-Vehicle, Parked Vehicle, Other)
   - Weather (dropdown)
   - Road Conditions (dropdown)
   - Police Report Filed (Yes/No radio)
   - Police Report # (text, enabled only if Yes)
   - Next / Back / Cancel

3. **Image Upload**
   - File chooser (accepts .png, .jpg, .bmp)
   - Show image preview after upload
   - Display filename
   - Status line: shows "No images uploaded" until at least one image is added
   - Next / Back / Cancel

4. **Image Analysis (REQUIRED)**
   - Read-only display of image preview
   - Auto-populated fields (prefilled by external image-analysis step before this screen):
     - Incident Type (guess from image)
     - Number of Vehicles
     - Vehicle 1 Position
     - Vehicle 2 Position (if multi)
     - Impact Type Estimate
     - Directions of Travel
     - Damage Zones
     - Road/Scene Factors (checkboxes)
     - Confidence Level (High/Medium/Low dropdown)
     - Assumptions & Ambiguities (multiline text)
   - Edit buttons for each field
   - **This entire section MUST be complete before moving forward.**
   - Next (blocked if any required field empty) / Back / Cancel

5. **Vehicle Details**
   - Vehicle 1 make/model/year
   - Vehicle 1 color, plate (optional)
   - Vehicle 1 damage level (dropdown)
   - [If Multi-Vehicle] Vehicle 2 same fields
   - Next / Back / Cancel

6. **Injury & Witnesses**
   - Injuries reported (Yes/No)
   - Witness present (Yes/No)
   - Witness name (text, optional if witness)
   - Next / Back / Cancel

7. **Review & Submit**
   - Summary of all entered data
   - Validation checklist:
     - ✓ Claimant info complete
     - ✓ Incident details complete
     - ✓ Image analysis complete
     - ✓ Vehicle details complete
   - "Submit Claim for Adjuster Review" button
   - "Back to Edit" button
   - "Save as Draft" button (optional alternate)

8. **Confirmation**
   - Success message: "Claim submitted successfully"
   - Claim number displayed
   - "Return to Main Menu" button

### Search Claim
- Search by claim ID or claimant name
- Display results
- View full claim details (read-only)
- Return to Main Menu

### System Administration
- Admin credentials required (already logged in as admin or prompts)
- Menu:
  - Reset Database to Defaults
  - View App Version & Build Info
  - Return to Main Menu
- Reset confirmation: "Are you sure? This cannot be undone."
- Reset re-seeds database with fresh claims and user data

---

## 7. Copilot Studio Agent Design

### Agent Role
**Claims Intake Operator** — autonomous assistant that processes user-provided accident images and populates claim forms in a legacy desktop system.

> **As built:** two models split the work — orchestrator/agent = **Claude Opus 4.7**, Computer Use tool = **Claude Sonnet 4.5** — and web search is disabled. The behaviors below are the original design intent; see **Lessons Learned After Testing** at the end for the shipped flow.

### Knowledge Base
- Screen layouts and field descriptions
- All dropdown/enum options for each field
- Example claim scenarios
- Image analysis best practices (what to look for in an accident sketch)

### Agent Instructions (Key Behaviors)
1. **Always screenshot first** — Verify current screen before any action
2. **Image priority** — Treat image upload and analysis as the core deliverable
3. **Deterministic navigation** — Follow prescribed menu routes; never attempt alternate paths
4. **Required-field discipline** — Never submit incomplete image analysis section
5. **Confidence-gated escalation** — If confidence < "High" on impact type or vehicle position, ask user one clarifying question
6. **No fabrication** — Never invent witness names, police report numbers, or injury details
7. **Completion assurance** — Validate all required fields before hitting submit
8. **Error recovery** — If error message appears, read it, go back, retry or ask user

### CUA Tool Instructions (Screen-by-Screen)
- How to detect each screen state (window title, visible buttons, prompt text)
- What keystrokes/clicks to issue (specific coordinates, tab order)
- How to interpret "Press any key to continue" vs. text input box
- When to pause and await user input

---

## 8. Data Seed & Demo Scenarios

### Pre-Populated Test Data (for evaluation/walk-through)

**Adjuster Credentials:**
- Username: `adjuster1`
- Password: `pass123`

**Test Claimant (optional pre-loaded for quick demos):**
- Name: Alex Johnson
- Phone: 555-0101
- Email: alex.johnson@example.com
- Policy #: POL-2026-987654

### Demo Scenario 1: Simple Hand-Drawn Sketch
- User provides hand-drawn accident diagram (two vehicles, T-bone impact)
- Agent uploads image
- Agent extracts: 2 vehicles, T-bone impact, clear intersection scene
- Agent fills form end-to-end
- Claim submitted
- **Key value:** CUA read the sketch and filled the form → user didn't have to type anything

### Demo Scenario 2: Photo + Clarification
- User provides phone photo of accident scene
- Agent analyzes, has medium confidence on impact type
- Agent asks: "Was this a head-on collision or a T-bone impact?"
- User clarifies
- Agent completes form
- **Key value:** CUA understood the visual, asked smart clarification, proceeded

### Demo Scenario 3: Multi-Vehicle with Witness
- Complex accident: 3 vehicles, possible injury
- Agent analyzes and maps positions
- Agent fills all details
- Notes "Witness present" (observed in image)
- Agent asks if witness contact info available, but allows submit if not
- **Key value:** CUA handled complexity and still completed workflow

---

## 9. Evaluation Plan

Four CSV test batches to run in Copilot Studio (in order, one per day/session):

### Batch 1: Smoke Tests (5 tests)
- App launch
- Login success
- Login failure (wrong credentials)
- Main menu navigation
- App reset admin function

### Batch 2: Image Upload & Analysis (7 tests)
- Upload image → verify preview
- Auto-populate image analysis section
- Validate image analysis fields required
- Single vehicle accident analysis
- Multi-vehicle accident analysis
- Unclear image (low confidence) → agent asks for clarification
- Skip image upload (form blocks submit)

### Batch 3: End-to-End Claim Submission (7 tests)
- Complete simple claim (hand-drawn sketch)
- Complete complex claim (multi-vehicle)
- Submit with optional witness info
- Submit without optional narrative
- Validation catches missing claimant name
- Validation catches incomplete image analysis
- Search and view submitted claim

### Batch 4: Compound & Edge Cases (4 tests)
- Offline scenario: image too large or corrupted format
- Agent handles validation error and retries
- Agent completes claim, returns to main menu, creates second claim
- Demo reset works → database cleared

---

## 10. Repository Structure

```
auto-claims-fnol-demo/
├── README.md                          ← Quick start guide
├── DESIGN.md                          ← This file
├── AGENT-GUIDE.md                     ← Complete technical reference
├── DEMO-WALKTHROUGH.md                ← Scripted demo scenarios
├── LICENSE
│
├── copilot-studio/
│   ├── SETUP-GUIDE.md                 ← Step-by-step agent setup
│   ├── KNOWLEDGE.md                   ← Upload as knowledge source
│   ├── AGENT-INSTRUCTIONS.md          ← Paste into agent instructions
│   ├── CUA-TOOL-INSTRUCTIONS.md       ← Paste into CUA tool instructions
│   ├── EVALUATION.md                  ← Test plan & troubleshooting
│   ├── evaluation-1-smoke.csv
│   ├── evaluation-2-images.csv
│   ├── evaluation-3-claims.csv
│   ├── evaluation-4-compound.csv
│   └── Claims Intake Agent/           ← Generated agent export (Copilot Studio extension)
│
├── src/                               ← C# / .NET 8 WPF source
│   └── AutoClaimsFnolApp/
│       ├── App.xaml / App.xaml.cs
│       ├── MainWindow.xaml / MainWindow.xaml.cs
│       ├── AssemblyInfo.cs
│       └── AutoClaimsFnolApp.csproj
│
├── intune/                            ← Intune packaging (self-contained build)
│   ├── Build.ps1                      ← Publishes the app + builds the .intunewin
│   ├── Build-IntuneWin.ps1
│   ├── Install.ps1
│   ├── Uninstall.ps1
│   ├── Detect.ps1
│   ├── source/ (staging — gitignored)
│   └── output/ (built .intunewin — gitignored)
│
├── docs/images/                       ← Screenshots used in the docs
│
├── .github/workflows/                 ← CI: builds & attaches release assets
│
└── .gitignore
```

> **Deviation from the original design (kept for honesty):**
> - **App layout** — Originally planned as a multi-project layout (`Program.cs`, `Screens/`, `Business/`, `Data/`, `Models/`, EF Core `ClaimsDbContext`). It shipped as a single WPF project, `src/AutoClaimsFnolApp/`, with one `MainWindow` hosting all six pages — the demo only needed a linear wizard, so the extra service/data layering added complexity without value. The SQLite `claims-fnol.db` is auto-created next to the exe on first run rather than via EF migrations.
> - **No `dist/` folder** — Originally the repo was going to carry a pre-built `AutoClaimsFnolApp.exe` + seeded `claims-fnol.db`. Binaries were removed from the repo; the prebuilt app now ships only as **release assets** (`AutoClaimsFNOL.intunewin`, `ManualInstall.zip`) so the tree stays clean and history small.
> - **`IntuneWinAppUtil.exe` not committed** — Originally listed in `intune/`. It's the Microsoft Win32 Content Prep Tool; `Build.ps1`/CI fetch it on demand instead of vendoring the binary.

---

## 11. Deployment Options

### Option A: Manual Install (Quickest for Dev)
1. Download release .zip
2. Extract to `C:\AutoClaimsFNOL\`
3. Run exe or create desktop shortcut
4. Database auto-seeds on first run

### Option B: Intune Deploy (Production-like)
1. Package exe + scripts via IntuneWinAppUtil
2. Create Win32 app in Intune admin center
3. Deploy to device group
4. Installs to `C:\AutoClaimsFNOL\`
5. Desktop shortcut created

---

## 12. Key Implementation Notes

### Image Analysis Mock (For v1 Demo)
For initial demo, image analysis **prefill** will use a simplified rule-based mock:
- If image shows two vehicles → "Multi-Vehicle"
- If vehicles overlap → "Possible T-Bone"
- If one vehicle clearly off-road → "Single Vehicle"
- Confidence randomized to Medium/High (Low only on complex ambiguous images)

In production, this would connect to actual vision/ML backend.

### Database Reset Seed Data
Reset script re-creates:
- 3 adjuster accounts
- 5 pre-loaded reference accident scenarios (stored but no claims associated)
- Empty claims table
- Schema version bump for future migrations

### No Real Backend Integration
This demo is **self-contained**. No cloud, no APIs, no network calls.  
The adjuster "queue" is just a local SQLite table viewed in-app.

---

## 13. Success Criteria

✅ Agent reliably uploads image  
✅ Image analysis section auto-populates with deterministic values  
✅ Agent completes all form pages without manual intervention  
✅ Validation enforces image analysis completion  
✅ Agent asks clarifying question when confidence is low  
✅ Claim submits successfully  
✅ Database resets cleanly for repeat demos  
✅ All four evaluation batches pass  

---

## 14. Differences from cobol-banker-demo (Intentional)

| Aspect | cobol-banker | auto-claims-fnol | Reason |
|--------|--------------|-----------------|--------|
| **Domain** | Banking transactions | Insurance claims | Different vertical; claims is more visual |
| **UI Style** | Green-screen terminal | Windows forms-era desktop | Accident sketches belong on a claims form, not a terminal |
| **Input Style** | Menu-driven (numeric selections) | Form-driven (text fields, dropdowns) | Claims require structured data, not menu sequences |
| **Image handling** | None | Central to workflow | Image analysis is the demo's core value |
| **Primary value artifact** | Completed transaction | Completed claim + image analysis | Shows what CUA actually did |
| **Data model complexity** | Simpler (customers, accounts, transfers) | Richer (multi-vehicle incidents, image analysis, nested details) | Reflects real insurance complexity |
| **Validation** | Permit submission of partial data | Hard-block on incomplete image analysis | Image analysis MUST be complete |

---

## 15. What's NOT in v1

- Real vision/ML backend (rule-based mock only)
- Multi-tenant support
- Audit trail / claim modification history
- Document attachment (except single image)
- Policy lookup / underwriting rules
- Payment / settlement module
- Mobile app
- Web portal for end users
- Real insurance data (all fake/seed data only)

---

## Decision Lock Summary

✓ Legacy UI: Windows forms-era desktop  
✓ Image input: Hand-drawn sketches + real photos  
✓ Submission: To Adjuster Queue (not draft)  
✓ Validation: Hard-block on missing image analysis  
✓ CUA autonomy: Autonomous + one clarification on low confidence  

**Next step:** Draft AGENT-GUIDE.md (complete screen reference) and DEMO-WALKTHROUGH.md (scripted demos).

---

## Lessons Learned After Testing

Everything above is the original, pre-build design. After wiring the agent to a real Windows 365 Cloud PC and running it end-to-end, several things changed. This is the "where we ended up" counterpart to the design above — each lesson is *what we designed → what we learned → what we changed*.

### 1. Image analysis moved up front (Work IQ–first)
- **Designed:** the CUA would "see" the sketch during Page 4 and populate the analysis fields inline.
- **Learned:** doing vision *and* form-driving in the same pass made the run slower and harder to verify, and any misread surfaced only after the form was half-filled.
- **Changed:** the **orchestrator** analyzes the image up front with **Work IQ Copilot** (from a OneDrive/SharePoint link), confirms every field with the user, then hands the confirmed values to a single Computer Use pass. On Page 4 the CUA just transcribes those values; it only re-derives from the on-screen preview if a value is missing.

### 2. Two models, two jobs
- **Designed:** one model for the whole agent.
- **Learned:** the up-front image analysis needs strong vision/reasoning, but the form navigation just needs to be fast and reliable.
- **Changed:** **orchestrator/agent = Claude Opus 4.7** (image analysis); **Computer Use tool = Claude Sonnet 4.5** (form navigation). Web search is disabled — everything needed is in Knowledge + instructions.

### 3. Confirm once, then stop asking
- **Designed:** "autonomous + one clarification on low confidence."
- **Learned:** the agent over-asked — it re-explained the process and re-requested details the user had already given, and once flagged a valid incident date as "in the future" (a model-clock artifact). Users had to say "that's all I have" more than once.
- **Changed:** gather and confirm everything in one pass, then proceed. Don't re-litigate fields that are already provided, and don't second-guess a plausible date.

### 4. MFA must pause, not loop
- **Learned:** when downloading the image triggered Entra MFA, the CUA repeatedly clicked Cancel and tried other accounts instead of waiting.
- **Changed:** the CUA now surfaces the verification number and **waits** for the user to approve — never cancels or loops. Pre-authenticating Edge and pre-downloading the image avoids the prompt entirely.

### 5. The #1 blocker was environmental, not the agent
- **Learned:** the single biggest failure was the app not launching at all — `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe` returned "you don't have permission to open this file" because the interactive user lacked **execute** rights. No instruction tuning can work around this.
- **Changed:** treat "the exe is runnable by the signed-in user" as a hard prerequisite, fixed at the file/folder ACL (or by deploying to a per-user executable location) before any run.

### 6. Stage the image twice
- **Learned:** the image is needed in two forms — a OneDrive/SharePoint **link** for Work IQ Copilot to analyze, and a **local file** for the Page 3 upload.
- **Changed:** stage both up front; pre-downloading the local copy also sidesteps the mid-run MFA prompt.

> **Deliberately unchanged:** the original domain-model fields above (vehicle color, license plate, free-text narrative, Save-as-Draft, the road-condition enum) are kept as the historical design record even though the shipped app and flow don't all exercise them. They document intent, not current behavior.
