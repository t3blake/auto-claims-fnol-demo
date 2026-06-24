# DEMO-WALKTHROUGH.md — Scripted Demo Scenarios

Three fully scripted demo flows you can run live or use as reference. Each tests different agent capabilities.

---

## Setup Before Any Demo

1. **Verify app is running** on the Cloud PC
   - Launch `AutoClaimsFnolApp.exe` from Run dialog: `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe`
   - Verify window title: "Auto Claims FNOL - Intake System"
   - **Confirm it opens as the signed-in user the agent controls.** If you see "Windows cannot access… / You don't have permission to open this file," that user lacks execute rights on the exe — fix the file/folder ACL before demoing. No agent instruction can work around this.

2. **Test images staged twice**
   - Prepare a simple hand-drawn accident sketch (PNG or JPG) and a photo/second sketch for alternate scenarios.
   - Put each image in **OneDrive/SharePoint** so you have a shareable link — this is what Work IQ Copilot analyzes up front.
   - Also **pre-download a local copy** to a known path (e.g., `C:\Temp\accident-sketch.jpg`) for the Page 3 upload. Pre-downloading avoids an MFA prompt mid-run.

3. **Database clean**
   - Log in as admin (`admin` / `admin`)
   - System Administration → Reset Database to Defaults
   - Confirm reset
   - Log off

4. **Agent ready**
   - Copilot Studio agent in draft mode (do not publish)
   - Computer Use tool enabled and pointed at the Cloud PC
   - Work IQ Copilot (Preview) tool enabled (analyzes the image up front)
   - **Two models set:** orchestrator/agent → Claude Opus 4.7; Computer Use tool → Claude Sonnet 4.5
   - Web search disabled
   - Knowledge source uploaded
   - Agent Instructions configured
   - CUA Tool Instructions configured

---

## Scenario 1: Simple Hand-Drawn Two-Vehicle T-Bone (Low Risk, High Success)

**Talking Points:**
- "We have an accident sketch showing two vehicles in a T-bone collision."
- "The agent analyzes the image up front with Work IQ Copilot, confirms the key facts with me, then fills the whole insurance form in one clean pass."
- "Note how it identifies the impact type and vehicle positions from the sketch before it ever touches the app."
- "The image analysis details are hard-required — the app won't submit without them — so the Page 4 section is where that up-front analysis pays off."

**Demo Image:**
Create a simple hand-drawn sketch or provide this ASCII representation (or use a real hand-drawn one):
```
        ↑ Vehicle A (northbound)
        │     ┌────────┐
        │     │        │
        │     │   A    │
        │  ╔══════════╗
        └→ ║ ╔══════╗ ║ ← Vehicle B (eastbound)
          ║ ║   B  ║ ║
          ╚══════════╝
          Impact point: intersection, ~90 degrees
```

**Agent Prompt:**
```
I have an accident sketch showing two vehicles colliding at an intersection. 
Here's the image: <paste OneDrive/SharePoint link>. (A local copy is staged at C:\Temp\accident-sketch.jpg for the upload.)
Can you file a claim for this accident? 
Use these claimant details: Name: Alex Johnson, Phone: 555-0101.
```

**Expected Agent Flow:**

1. **Launch app**
   - Press Win+R
   - Type `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe`
   - Press Enter
   - Screenshot → Verify login screen

2. **Login**
   - Click Username field, type `adjuster1`
   - Tab, type `pass123`
   - Click Login
   - Screenshot → Verify Main Menu

3. **Navigate to New Claim**
   - Click the "New Claim" button
   - Screenshot → Verify Page 1

4. **Page 1: Claimant Info**
   - Click Name field, type `Alex Johnson`
   - Tab, type `555-0101`
   - Click Next
   - Screenshot → Verify Page 2

5. **Page 2: Incident Details**
   - Click Date field, select today's date (or 06/22/2026)
   - Tab, type location: `123 Main St, Anytown, CA 90210`
   - Click Incident Type dropdown, select `Multi-Vehicle`
   - Click Weather dropdown, select `Clear`
   - Click Road dropdown, select `Dry`
   - Click "No" for Police Report
   - Click Next
   - Screenshot → Verify Page 3

6. **Page 3: Image Upload**
   - Click the **[Add Image...]** button
   - Navigate to `C:\Temp\accident-sketch.jpg`
   - Select file, confirm upload
   - Screenshot → Verify image preview
   - Click Next
   - Screenshot → Verify Page 4

7. **Page 4: Image Analysis** ← **Core demo moment**
   - The orchestrator already analyzed the image with Work IQ Copilot up front; here the Computer Use tool simply enters the confirmed values:
     - Incident Type: `Multi-Vehicle`
     - Number of Vehicles: `2`
     - Impact Type: `T-Bone`
     - Vehicle 1 Position: `North`
     - Vehicle 1 Direction: `South` (moving north, hit from west)
     - Vehicle 2 Position: `West`
     - Vehicle 2 Direction: `East` (moving east, hit from north)
   - Check damage zones: Front, Passenger Side
   - Check road factors: Intersection
   - Confidence: `High`
   - Assumptions: `"Clear T-bone collision at 90-degree angle. Vehicles at right angles."`
   - Click Next
   - **[Narrator: "All this detail came from the up-front Work IQ analysis we confirmed earlier — the agent is just transcribing it into the form now."]**

8. **Page 5: Vehicle & Injury Info**
   - Enter Vehicle 1: Make = `Toyota`, Model = `Camry`, Year = `2022`, Damage = `Moderate`
   - Check "Multi-vehicle", enter Vehicle 2: Make = `Ford`, Model = `F-150`, Year = `2020`, Damage = `Severe`
   - Injuries: `No`
   - Witness: `No`
   - Click Next
   - Screenshot → Verify Page 6

9. **Page 6: Review & Submit**
   - Read summary to audience
   - Screenshot → Verify all data present
   - Click Submit
   - Screenshot → Verify Confirmation

10. **Confirmation**
    - Show claim number (e.g., CLM-2026-001234)
    - Message: "Claim submitted successfully"
    - **[Narrator: "And that's it. Claim filed. The agent analyzed the sketch, confirmed the details with me, filled all six pages in one pass, and submitted it."]**

**Audience Talking Points:**
- "The image analysis is the core deliverable — and it happens up front, before the app is ever touched."
- "Notice the form wouldn't submit without a complete image analysis section — that's the app enforcing that the work was done."
- "If I'd given a blurry or ambiguous image, the agent would have asked me one clarifying question before running. This sketch was clear, so we confirmed and proceeded."
- "In real insurance, this would cut claim processing time from hours (manual data entry) to minutes."

---

## Scenario 2: Photo with Confidence Check (Medium Complexity)

**Talking Points:**
- "This time we have a real accident photo that's a bit ambiguous."
- "Watch as the agent analyzes the image, recognizes it can't be 100% sure of the impact type, and asks me a clarifying question."
- "Then it completes the form with the answer I provide."

**Demo Image:**
Provide a real photo of an accident scene or a moderately ambiguous hand-drawn sketch that could be interpreted as either head-on or T-bone.

**Agent Prompt:**
```
I have a photo from a car accident. It's a bit unclear from the angle, but it looks like there could have been two cars involved.
Here's the photo: <paste OneDrive/SharePoint link>. (A local copy is staged at C:\Temp\accident-photo.jpg for the upload.)
Can you file a claim? Use claimant: Name: Sarah Davis, Phone: 555-0102.
```

**Expected Agent Flow:**

1-6. **Same as Scenario 1 (Login through Image Upload)**

7. **Page 4: Image Analysis** ← **Confidence check moment**
   - During the up-front Work IQ analysis, the agent recognizes ambiguity on impact type
   - It has most fields confidently, but impact type is only medium confidence
   - Agent **pauses and asks you (before running the form):** "I analyzed this as a possible T-Bone collision, but the angle is a bit unclear. Was this a head-on impact, a T-bone, or something else?"
   - **[You respond: "It was T-bone."]**
   - Agent updates Impact Type to `T-Bone` and continues
   - Fill remaining fields
   - Confidence: `Medium` (agent set this based on image clarity)
   - Click Next

8-10. **Same as Scenario 1 (Pages 5-6, Submit, Confirmation)**

**Audience Talking Points:**
- "The agent didn't guess. It recognized low confidence and asked."
- "This is responsible automation — the agent used its judgment to escalate the ambiguous part to me, then proceeded with the clear data."
- "In a busy claims center, this kind of smart escalation saves time because the agent handles the clear cases and flags only what needs human judgment."

---

## Scenario 3: Multi-Vehicle Complex Scenario (High Complexity, Shows Depth)

**Talking Points:**
- "The last scenario is the complex one: three vehicles, a highway pile-up, a witness on scene, and possible injuries."
- "The agent will still extract all the key facts from the image and fill the form end-to-end."
- "This shows that the agent can handle realistic incident complexity without getting confused."

**Demo Image:**
Create or provide a hand-drawn sketch of three vehicles in a pile-up or chain reaction. Example:
```
  ┌────────┐     ┌────────┐     ┌────────┐
  │   A    │     │   B    │     │   C    │
  └────────┘     └────────┘     └────────┘
    ▲ (stopped)    ▲ (moving)    ▲ (hit C)
     │              │              │
  ─────────────────────────────────── Highway (eastbound traffic)
```

**Agent Prompt:**
```
We have a multi-vehicle accident on the highway.
The image shows three cars involved. There's a witness on scene.
The claimant's vehicle (the middle one) hit the car in front.
Use these details: Claimant Name: John Martinez, Phone: 555-0103.
Can you file a claim?
Here's the image: <paste OneDrive/SharePoint link>. (A local copy is staged at C:\Temp\pileup-sketch.jpg for the upload.)
```

**Expected Agent Flow:**

1-3. **Same as Scenario 1 (Launch, Login, Navigate)**

4. **Page 1: Claimant Info**
   - Enter John Martinez, 555-0103
   - Click Next

5. **Page 2: Incident Details**
   - Date: Today (or 06/21/2026)
   - Time: 14:15 (2:15 PM)
   - Location: `Interstate 405, mile marker 42, northbound`
   - Incident Type: `Multi-Vehicle` (dropdown)
   - Weather: `Clear`
   - Road: `Dry`
   - Police Report: `Yes`
   - Police Report #: (agent asks — say you don't have it, so agent leaves blank or uses "Pending")
   - Click Next

6. **Page 3: Image Upload**
   - Upload pileup sketch
   - Click Next

7. **Page 4: Image Analysis** ← **Complex extraction moment**
   - Agent processes multi-vehicle image
   - Auto-populate:
     - Incident Type: `Multi-Vehicle`
     - Number of Vehicles: `3`
     - Impact Type: `Rear-End` (chain reaction from rear)
     - Vehicle 1 (rear) Position: `South`
     - Vehicle 1 Direction: `North` (approaching from rear)
     - Vehicle 2 (middle) Position: `Center`
     - Vehicle 2 Direction: `North` (hit by rear vehicle)
     - Vehicle 3 (front, proxy) Position: `North`
     - Vehicle 3 Direction: Stopped/Parked
   - Damage zones: Rear (vehicle 1), Front (vehicle 2), Rear (vehicle 2)
   - Road factors: Highway, possible lane merge or debris
   - Confidence: `Medium` (three vehicles harder to parse)
   - Assumptions: `"Chain-reaction rear-end on highway. Vehicle 1 hit vehicle 2. Vehicle 2 may have hit vehicle 3 or stopped after hit. Third vehicle position unclear from sketch."`
   - Click Next

8. **Page 5: Vehicle & Injury Info** ← **Multi-vehicle data entry**
   - Enter Vehicle 1 (your vehicle): Toyota RAV4, 2021, Damage = Severe
   - Check "Multi-vehicle"
   - Enter Vehicle 2 (hit from behind): Honda Accord, 2019, Damage = Moderate
   - (Note: Vehicle 3 not required in this form, agent documents in narrative if needed)
   - Injuries: `Yes` (agent asks for details or leaves as "Reported" without specifics)
   - Witness: `Yes`
   - Witness name: `(optional — agent can leave blank or you provide "John Smith")`
   - Click Next

9. **Page 6: Review & Submit**
   - Read summary
   - Click Submit

10. **Confirmation**
    - Show claim number
    - **[Narrator: "Complex incident, three vehicles, injuries, witness — all filed in one shot. The agent handled the complexity and didn't get lost."]**

**Audience Talking Points:**
- "This is where the agent really shines. It didn't panic at the complexity."
- "It extracted what it could clearly see (three vehicles, chain-reaction pattern) and flagged what was ambiguous (vehicle 3 exact role)."
- "In real claims, this kind of end-to-end handling means adjusters can focus on the investigative and underwriting parts instead of data entry."

---

## Between-Scenario Reset

After Scenario 1, 2, or 3 completes:

1. Agent logs off and returns to Login
2. You (or agent) log in as `admin` / `admin`
3. Navigate to Main Menu → System Administration → Reset Database to Defaults
4. Confirm "Are you sure?" dialog
5. Database reset message appears
6. Admin logs off
7. App ready for next scenario

**Total reset time:** ~20 seconds

---

## Troubleshooting During Live Demo

### App Won't Launch ("You don't have permission to open this file")
- **Cause:** the signed-in user the agent controls lacks execute rights on `AutoClaimsFnolApp.exe`. This is the #1 blocker and is environmental — no instruction change fixes it.
- **Fix:** correct the file/folder ACL (or deploy the exe to a per-user, executable location) before the demo. Verify by double-clicking it yourself as that user.

### Stuck on a Sign-In / MFA Prompt
- **Cause:** downloading the image or opening a OneDrive/SharePoint link triggers Entra MFA mid-run.
- **Fix:** the CUA should surface the verification number and wait — don't let it cancel/loop. Best avoided by pre-authenticating Edge on the Cloud PC and pre-downloading the local image copy before the run.

### Agent Stuck on Page 4 (Image Analysis)
- **Cause:** One field not populated or checkbox not checked
- **Fix:** Manually check the visible form, verify all required fields have values, click Next manually

### Image Upload Fails
- **Cause:** File path incorrect or file format unsupported
- **Fix:** Verify file exists at the path, is .jpg / .png / .bmp, is < 10 MB
- **Recovery:** Go back to Page 3, try uploading again

### Submit Blocked with Validation Error
- **Cause:** Required field is missing or empty
- **Fix:** Read error message, go back to indicated page, fill missing field, re-proceed to submit
- **Key:** Image analysis section is hard-required — verify all fields on Page 4 are filled

### Agent Asks Clarifying Question and Pauses
- **Normal behavior.** Respond with the information (impact type, vehicle position, etc.)
- Agent will update the field and proceed

### Database Reset Doesn't Work
- **Cause:** Admin didn't confirm the "Are you sure?" dialog twice
- **Fix:** Try reset again, confirm both prompts

---

## Tips for Success

1. **Run through one scenario in private first.** Get familiar with the flow.
2. **Have images pre-positioned** on the Cloud PC at known paths (e.g., `C:\Temp\`).
3. **Start with Scenario 1.** It's the simplest and least likely to have surprises.
4. **For live audiences, narrate what the agent is doing** as it happens. Don't let silence dominate.
5. **Emphasize the image analysis section** — that's where the agent's value is most visible.
6. **Show the database reset** at the end. Proves the demo is repeatable and safe.
7. **Keep a backup image file** in case the primary one has issues.

---

## Timing Estimates

| Scenario | Time (including agent thinking) |
|----------|--------------------------------|
| Scenario 1 (simple, high success) | 3–5 minutes |
| Scenario 2 (with clarification) | 4–6 minutes |
| Scenario 3 (complex, multi-vehicle) | 5–8 minutes |
| Database reset | ~20 seconds |

**Total full demo (all three + resets): ~20 minutes**

---

## Key Success Criteria

✅ Agent launches app autonomously  
✅ Agent logs in with provided credentials  
✅ Agent uploads image without manual intervention  
✅ Agent's image analysis extracts key facts (incident type, vehicle positions, impact)  
✅ Agent completes all form pages  
✅ Agent submits claim and displays claim number  
✅ Database resets cleanly for next demo  
✅ If confidence is low, agent asks clarifying question and proceeds on your answer  
✅ No validation errors on submit (all required fields filled)  

If all above check out, demo is successful.

