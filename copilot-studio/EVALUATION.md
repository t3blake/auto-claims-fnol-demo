# Auto Claims FNOL — Evaluation Plan & Test Guide

This document outlines how to evaluate the Auto Claims FNOL Copilot Studio agent using Copilot Studio's built-in Evaluation feature. It includes test batches (CSV imports), pass/fail criteria, and troubleshooting.

---

## Overview

Four evaluation batches, run sequentially (do **not** run in parallel):

1. **Batch 1: Smoke Tests** (5 tests) — Basic launch, login, app state detection
2. **Batch 2: Image Upload & Analysis** (7 tests) — Image handling, extraction accuracy, field population
3. **Batch 3: End-to-End Claim Submission** (7 tests) — Full workflow, validation, submit success
4. **Batch 4: Compound & Edge Cases** (4 tests) — Multi-step flows, error recovery, database reset

**Estimated total time:** 30–45 minutes (including manual resets between batches)

---

## Prerequisites

- **Agent deployed in Copilot Studio** (draft mode)
- **Computer Use tool enabled** and pointed at the Cloud PC
- **Work IQ Copilot (Preview) tool enabled** (analyzes the accident image up front)
- **Two models set:** orchestrator/agent → Claude Opus 4.7; Computer Use tool → Claude Sonnet 4.5
- **Web search disabled**
- **Knowledge, Agent Instructions, CUA Tool Instructions configured**
- **App running on Cloud PC:** `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe` — and **runnable by the signed-in user** (no "you don't have permission to open this file" ACL block; this is the #1 blocker)
- **Test image staged twice:** a OneDrive/SharePoint link (for Work IQ Copilot) and a local copy (for the Page 3 upload); pre-downloading avoids a mid-run MFA prompt
- **Database clean:** Log in as admin, reset to defaults before starting

---

## Running an Evaluation

### In Copilot Studio:

1. Open your agent → **Evaluate** tab (left nav)
2. Click **+ New evaluation** → **Import**
3. Select the CSV file (e.g., `evaluation-1-smoke.csv`)
4. Click **Create evaluation**
5. Review test cases displayed
6. Click **Run** to start the batch
7. Monitor progress (each test runs sequentially)
8. When complete, review results:
   - Green checkmark = passed
   - Red X = failed
   - Click individual tests to see details (input prompt, expected output, actual output)

**Important:** Do NOT run multiple evaluations simultaneously. They share the CUA connection. Wait for batch N to finish before starting batch N+1.

---

## Batch 1: Smoke Tests (5 tests)

**Purpose:** Verify basic app functionality and agent launch capability.

**Pre-requisite:** App installed on Cloud PC, database clean

| Test # | Scenario | Input Prompt | Expected Pass Criteria |
|--------|----------|--------------|---------------------------|
| 1.1 | App Launch | "Launch the Auto Claims FNOL app on the Cloud PC." | Window "Auto Claims FNOL - Intake System" opens; no errors |
| 1.2 | Login Success | "Log in with username adjuster1 and password pass123." | Login succeeds; Main Menu displayed |
| 1.3 | Login Failure Handling | "Try to log in with wrong password (e.g., 'wrongpass'). Then log in with correct credentials." | Error message shown on first attempt; correct login succeeds on second |
| 1.4 | Main Menu Navigation | "From Main Menu, navigate to New Claim." | New Claim form Page 1 loads (title shows "Page 1 of 6") |
| 1.5 | App Close & Restart | "Log off and close the app. Relaunch it." | App relaunches successfully; login screen appears again |

**Pass/Fail Criteria:**
- All 5 tests must pass
- If any fail, troubleshoot (see Troubleshooting section)
- If 4/5 pass, proceed to Batch 2 anyway (Batch 1 failures usually environmental, not agent logic)

---

## Batch 2: Image Upload & Analysis (7 tests)

**Purpose:** Verify image handling and that Page 4's analysis fields end up correctly populated — from the up-front Work IQ values in the full flow, or the CUA's derive-from-preview fallback when a test supplies only a local image.

**Pre-requisite:** Batch 1 passed; database clean; test images staged as a OneDrive/SharePoint link (for Work IQ) and as local copies at `C:\Temp\` (for the Page 3 upload)

**Test Images Needed:**
- `accident-sketch-2vehicle-tbone.jpg` — Hand-drawn or simple 2-vehicle T-bone
- `accident-sketch-3vehicle-pileup.jpg` — 3-vehicle chain-reaction sketch
- `accident-sketch-ambiguous.jpg` — Blurry or unclear impact angle (triggers confidence check)

| Test # | Scenario | Input Prompt | Expected Pass Criteria |
|--------|----------|--------------|---------------------------|
| 2.1 | Image Upload | "Upload accident-sketch-2vehicle-tbone.jpg from C:\Temp\ to the New Claim form." | Image appears in the Uploaded Images list; preview displays; status no longer reads "No images uploaded" |
| 2.2 | Page 4 Populated From Up-Front Analysis | "After uploading the 2-vehicle T-bone sketch, proceed to Page 4 (Image Analysis). Verify the fields are populated from the up-front Work IQ analysis." | Page 4 fields filled: Incident Type = Multi-Vehicle, Impact Type = T-Bone, Vehicle count = 2, damage zones checked, confidence = High or Medium |
| 2.3 | Single Vehicle Extraction | "Upload and analyze a single-vehicle accident sketch." | Page 4: Incident Type = Single Vehicle, 1 vehicle detected, damage visible, confidence appropriately set |
| 2.4 | Multi-Vehicle Extraction | "Upload and analyze a 3-vehicle pile-up sketch." | Page 4: Incident Type = Multi-Vehicle, 3 vehicles detected, chain-reaction pattern inferred, confidence = Medium or High |
| 2.5 | Low Confidence Escalation | "Provide the ambiguous impact-angle image. The agent should ask for clarification on impact type during the up-front analysis, before running the form." | Agent pauses during the up-front analysis; asks user a clarifying question (e.g., "Was this head-on or T-bone?"); waits for response; uses the answer when filling Page 4 |
| 2.6 | Image Analysis Validation Block | "Attempt to submit a claim without completing all Page 4 fields (e.g., no damage zone checkbox checked)." | Form blocks submit; error message shows "Image analysis is incomplete"; agent goes back, completes missing field, retries |
| 2.7 | No Image Upload Handling | "Start a new claim but skip image upload. Try to proceed." | Page 4 shows "No image provided"; agent either re-uploads image or form blocks submit |

**Pass/Fail Criteria:**
- 2.1, 2.2, 2.4: Critical (must pass)
- 2.3, 2.6, 2.7: Important (should pass)
- 2.5: Desirable (confidence-gated escalation; may not always trigger depending on image clarity)
- Pass if 5/7 pass; concern if < 4/7

**After Batch 2:**
- Log in as admin, reset database to defaults (prepare for Batch 3)

---

## Batch 3: End-to-End Claim Submission (7 tests)

**Purpose:** Verify complete workflow from login through successful submit and confirmation.

**Pre-requisite:** Batch 2 passed; database reset (clean); test images available

| Test # | Scenario | Input Prompt | Expected Pass Criteria |
|--------|----------|--------------|---------------------------|
| 3.1 | Simple Claim (High Confidence) | "File a claim: hand-drawn 2-vehicle T-bone sketch, claimant Alex Johnson, phone 555-0101, no injuries, no witness." | Claim submitted; confirmation displays claim number (e.g., CLM-2026-001234); status = "Submitted for Review" |
| 3.2 | Claimant Name Validation | "Try to submit a claim without entering claimant name. Agent should catch this and fill it." | Form shows validation error; agent goes back, enters name, retries submit successfully |
| 3.3 | Incident Details Completeness | "Verify agent fills all required incident fields: date, location, type, weather, road, police report flag." | Page 2 all required fields filled; proceeding to Page 3 succeeds |
| 3.4 | Vehicle Damage Classification | "File a claim with visible vehicle damage. Verify agent correctly selects damage level (e.g., Moderate, Severe)." | Page 5: Damage Level dropdown correctly set based on image analysis |
| 3.5 | Injury & Witness Handling | "File a claim where injuries and witness are indicated in image. Verify agent marks both as Yes and optionally captures names." | Page 5: Injuries Reported = Yes (if visible in image); Witness Present = Yes; name captured (optional) |
| 3.6 | Multi-Vehicle Claim Submit | "File a claim with 2+ vehicles. Verify agent fills all vehicle details and submits successfully." | Page 5: Both vehicles documented; Page 6 summary shows all vehicles; submit succeeds; claim number issued |
| 3.7 | Search Submitted Claim | "After submitting a claim, use Search to look up the claim by claim number or claimant name. Verify retrieval." | Claim appears in search results; claim number matches; claimant name matches; full details viewable |

**Pass/Fail Criteria:**
- 3.1, 3.2, 3.3, 3.6: Critical (must pass)
- 3.4, 3.5, 3.7: Important (should pass)
- Pass if 6/7 pass

**After Batch 3:**
- Log in as admin, reset database to defaults (prepare for Batch 4)

---

## Batch 4: Compound & Edge Cases (4 tests)

**Purpose:** Verify error recovery, agent resilience, and demo repeatability.

**Pre-requisite:** Batch 3 passed; database reset

| Test # | Scenario | Input Prompt | Expected Pass Criteria |
|--------|----------|--------------|---------------------------|
| 4.1 | Validation Error Recovery | "File a claim but intentionally trigger a validation error (e.g., missing damage zone on Page 4). Verify agent catches the error, goes back, corrects, and retries." | Agent reads error message; navigates back to problematic page; corrects field; retries submit; succeeds |
| 4.2 | Multi-Step Workflow | "File claim 1, submit it, return to Main Menu, file claim 2, submit it. Verify agent handles sequential claims without confusion." | Claim 1 submitted with number CLM-XXXX; agent returns to Main Menu; claim 2 filed and submitted with different number CLM-YYYY; both searchable |
| 4.3 | Database Reset Admin Flow | "Log in as admin. Navigate to System Administration. Reset database to defaults. Verify database clears and app ready for next demo." | Admin login succeeds; System Admin menu displays; reset confirmation prompt; reset completes; all previous claims gone; database ready |
| 4.4 | End-to-End with Clarification | "File a complex claim (3 vehicles or ambiguous image). Agent should ask clarifying questions where confidence is low, incorporate user response, and successfully submit." | Agent pauses and asks clarifying question; waits for response; updates form with response; continues and submits; claim succeeds |

**Pass/Fail Criteria:**
- All 4 tests should pass
- If any fail, investigate agent's error recovery logic or form validation
- Pass if 4/4 pass

---

## Test CSV Format (Reference)

Each CSV file contains columns:

```
Prompt,Expected Output Grader Type,Expected Output
"Your prompt here...","General quality" or specific grader,"Expected response or behavior"
```

Example row:
```
"Launch the Auto Claims FNOL app on the Cloud PC.","General quality","Window titled 'Auto Claims FNOL - Intake System' appears; no errors"
```

---

## Pass/Fail Scoring

### Overall Pass Criteria
- **All batches 1–4 pass:**  Agent is production-ready for live demo
- **Batches 1–3 pass, Batch 4 partial:** Agent is demo-capable with minor edge-case issues
- **Batches 1–2 pass, Batch 3 has failures:** Agent has core functionality gaps; needs fixes to image analysis or form handling
- **Batch 1 failures:** Environmental issues (app not installed, network, CUA connection); check Cloud PC setup

### Per-Batch Thresholds
| Batch | Minimum Pass Rate | Guidance |
|-------|-------------------|----------|
| 1 (Smoke) | 100% (5/5) | If < 100%, troubleshoot environment before proceeding |
| 2 (Images) | 70% (5/7) | Critical tests (2.1, 2.2, 2.4) must pass; 2.5 is bonus |
| 3 (Claims) | 85% (6/7) | Critical tests (3.1, 3.2, 3.3, 3.6) must pass |
| 4 (Compound) | 100% (4/4) | All edge cases should work |

---

## Troubleshooting

### Common Failures & Fixes

#### Test 1.1 or 1.2: App Won't Launch
- **Symptom:** Window doesn't appear; or "Windows cannot access… / You don't have permission to open this file"
- **Cause:** **#1 blocker** — the signed-in user lacks **execute** rights on the exe. Also: app not installed, path wrong, or Windows 365 not connected.
- **Fix:**
  1. As the signed-in user, double-click `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe` — if it won't open, fix the file/folder ACL (or deploy to a per-user executable location). No instruction change works around this.
  2. Verify the app is installed at that path
  3. Verify Windows 365 Cloud PC is online and CUA connection active
  4. If manual launch works, retry evaluation

#### Sign-In / MFA Prompt Mid-Run
- **Symptom:** Edge shows "Verify your identity" / "Enter the number shown to sign in" while staging the image, and the agent cancels or loops
- **Cause:** downloading the image or opening a OneDrive/SharePoint link triggers Entra MFA
- **Fix:** the CUA should surface the verification number and **wait** for approval — never cancel/loop. Pre-authenticate Edge on the Cloud PC and pre-download the local image copy before the run to avoid it entirely.

#### Test 2.1 or 2.2: Image Upload or Preview Fails
- **Symptom:** "Add Image..." button clicked, file chooser doesn't open; or image doesn't display in preview
- **Cause:** File path incorrect, format unsupported, or file corrupted
- **Fix:**
  1. Verify test image files at `C:\Temp\` on Cloud PC
  2. Ensure format is .jpg, .png, or .bmp
  3. Ensure file size < 10 MB
  4. Manually test upload by running claim form and uploading manually
  5. Regenerate test images if corrupted

#### Test 2.5: Confidence Check Not Triggered
- **Symptom:** Agent fills Page 4 without asking clarifying question
- **Cause:** Image is too clear, agent confidence is high, escalation not needed
- **Fix:**
  1. Regenerate test image with genuine ambiguity (e.g., unclear angle, obscured vehicles)
  2. Or adjust test expectation: clarification only happens if confidence truly is low
  3. Re-run test with adjusted image

#### Test 3.1: Claim Submit Blocked with Error
- **Symptom:** Page 6 submit fails; error message shown
- **Cause:** One or more required fields not filled
- **Fix:**
  1. Read error message carefully (e.g., "Image analysis is incomplete")
  2. Agent should go back, complete missing field, retry
  3. If agent does not auto-correct, review AGENT-INSTRUCTIONS prioritization

#### Test 3.7: Claim Search Returns No Results
- **Symptom:** After submitting claim, search finds no matching claims
- **Cause:** Claim ID or claimant name entered incorrectly, or claim not saved
- **Fix:**
  1. Verify claim was actually submitted (Confirmation page shown with claim #)
  2. Manually verify claim exists in app (log in as adjuster, search manually)
  3. If claim not in database, database reset may have cleared it unintentionally

#### Test 4.1: Agent Doesn't Recover from Validation Error
- **Symptom:** Submit fails, agent doesn't go back or correct
- **Cause:** Agent not reading error message or error recovery logic weak
- **Fix:**
  1. Review AGENT-INSTRUCTIONS section "Error Recovery"
  2. Ensure agent is always taking screenshots and reading error messages
  3. Re-test with agent prompted to "Read the error message and correct it"

#### Test 4.3: Database Reset Fails or Doesn't Clear
- **Symptom:** Admin reset completes, but old claims still appear in database
- **Cause:** Admin not logged in, reset not confirmed, or database file locked
- **Fix:**
  1. Verify admin login succeeded (`admin` / `admin`)
  2. Verify reset confirmation dialog was clicked (2 confirmations required)
  3. Verify reset success message appeared
  4. Manually restart app and verify database is clean
  5. If database locked, close app, delete or restore claims-fnol.db, relaunch

### CUA Connection Issues

#### Timeout or "Computer Use connection lost"
- **Cause:** Cloud PC session disconnected, network latency, or token expired
- **Fix:**
  1. Verify Cloud PC is online
  2. In Copilot Studio, go to **Settings → Connections**
  3. Find Windows 365 connection, click to refresh
  4. Re-run test

#### Clicks or Keystrokes Not Registering
- **Cause:** CUA focus lost, or interaction rate too fast
- **Fix:**
  1. Agent should always take screenshot before acting (verify focus)
  2. Add slight delays between clicks (CUA-TOOL-INSTRUCTIONS recommend screenshot → action → wait pattern)
  3. Reduce rapid click sequences; use Tab to navigate between fields instead

---

## Manual Intervention Points

You may need to manually intervene in these scenarios:

1. **Image quality unclear:** Regenerate test images with clearer or ambiguous content as needed
2. **CUA connection stale:** Manually refresh Windows 365 connection in Copilot Studio settings
3. **Database inconsistency:** Manually reset database via app admin menu
4. **Claim retrieval debugging:** Search for claim manually in app to verify it was saved

---

## Full Demo Run (Option)

If you want to run a complete demo + all evaluations in one session:

1. **Prep:** App installed, database clean, CUA ready
2. **Live Demo:** Run one scenario from DEMO-WALKTHROUGH.md (Scenario 1: ~3 min)
3. **Reset:** Admin reset database (20 sec)
4. **Batch 1:** Run smoke tests (~5 min)
5. **Batch 2:** Run image tests (~10 min)
6. **Reset:** Admin reset database (20 sec)
7. **Batch 3:** Run end-to-end tests (~15 min)
8. **Reset:** Admin reset database (20 sec)
9. **Batch 4:** Run compound tests (~10 min)

**Total time:** ~60 minutes (including resets)

---

## Sign-Off Criteria

Agent is **production-ready** for live demo when:
- ✅ All Batch 1 tests pass (100%)
- ✅ Batch 2 tests pass (5/7, critical tests must pass)
- ✅ Batch 3 tests pass (6/7, critical tests must pass)
- ✅ Batch 4 tests pass (4/4)
- ✅ Live demo run 1+ times successfully
- ✅ Database reset works reliably
- ✅ Agent asks clarifying questions appropriately (when confidence low)
- ✅ Agent never fabricates PII
- ✅ Claim submission end-to-end works with no manual intervention

---

## Next Steps After Evaluation

- **All tests pass:** Schedule live demo. Agent ready for audience.
- **Batch 1/2 issues:** Troubleshoot environment or image extraction logic
- **Batch 3 issues:** Review form validation or field mapping in agent instructions
- **Batch 4 issues:** Refine error recovery behavior or edge case handling

