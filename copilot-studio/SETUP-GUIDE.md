# Copilot Studio Agent Setup Guide

This directory contains everything to build the **Auto Claims FNOL Computer Use Agent**.

## Two representations in this folder

- **Canonical markdown (edit these):** [AGENT-INSTRUCTIONS.md](AGENT-INSTRUCTIONS.md), [CUA-TOOL-INSTRUCTIONS.md](CUA-TOOL-INSTRUCTIONS.md), and [KNOWLEDGE.md](KNOWLEDGE.md) are the tenant-agnostic source of truth. They paste/upload directly into the three Copilot Studio fields.
- **Generated YAML export (don't hand-edit for content):** `Claims Intake Agent/` is the VS Code extension's clone of the deployed agent. Its `instructions:` blocks are generated from the markdown above, and its connection references + `cre3e_` schema names are bound to the source environment.

When you change instruction content, edit the markdown first, then re-paste/sync and pull the YAML so the export stays current.

## Quick Setup

### 1. Install Copilot Studio Extension (if not already installed)

In VS Code:
- Open Extensions (Ctrl+Shift+X)
- Search for "Copilot Studio"
- Install the official Microsoft extension

### 2. Authenticate with Your Environment

In VS Code:
- Open Command Palette (Ctrl+Shift+P)
- Type "Copilot Studio: Authenticate"
- Follow the browser login flow
- Select your organization/environment

### 3. Create or Clone the Agent

**Option A — Clone the included agent (if you use the extension):**
- The deployable agent lives under `Claims Intake Agent/` (`agent.mcs.yml`, actions, settings, connection references).
- Push it to your environment with the extension, then reconnect the tools (step 4). These YAML files are a **generated export from the source environment** — connection references and `cre3e_` schema names are tenant-specific and must be re-created/reconnected in yours.

**Option B — Build it by hand from the canonical markdown:**
- Create a new agent, then paste the three markdown files into the matching fields below. This path is tenant-agnostic and is the one the root README documents.

### 4. Configure the Agent (the settled wiring)

1. **Instructions** — paste [AGENT-INSTRUCTIONS.md](AGENT-INSTRUCTIONS.md) into **Agent → Instructions** (skip the header).
2. **Knowledge** — upload [KNOWLEDGE.md](KNOWLEDGE.md) as a knowledge source.
3. **Computer Use tool** — add it, point it at the Windows 365 Cloud PC, and paste [CUA-TOOL-INSTRUCTIONS.md](CUA-TOOL-INSTRUCTIONS.md) into its **Tool Instructions** (skip the header).
4. **Work IQ Copilot (Preview) tool** — add it; this analyzes the accident image from its OneDrive/SharePoint link, up front, before Computer Use runs.
5. **Set the two models:**
   - **Orchestrator / agent model → Claude Opus 4.7** (does the up-front image analysis; vision quality matters).
   - **Computer Use tool model → Claude Sonnet 4.5** (fast form navigation; don't put a reasoning model here).
6. **Disable web search** — everything the agent needs is in Knowledge + instructions.

### 5. Test the Agent

Use the **Evaluate** tab in Copilot Studio:
- Import the `evaluation-*.csv` batches one at a time (run in order; don't run in parallel — they share the CUA connection).
- Review results and iterate. See [EVALUATION.md](EVALUATION.md).

### 6. Deploy When Ready

- Publish the agent from the Copilot Studio UI (or push via the extension).
- After changing any instruction content, edit the **markdown** first, re-paste/sync, then pull the YAML so the export stays current.

## File Structure

```
copilot-studio/
├── AGENT-INSTRUCTIONS.md           # CANONICAL — paste into Agent → Instructions
├── CUA-TOOL-INSTRUCTIONS.md        # CANONICAL — paste into Computer Use → Tool Instructions
├── KNOWLEDGE.md                    # CANONICAL — upload as knowledge source
├── EVALUATION.md                   # Evaluation plan & troubleshooting
├── evaluation-*.csv                # Test cases (4 batches)
└── Claims Intake Agent/            # Generated export (tenant-specific) for the VS Code extension
    ├── agent.mcs.yml               #   instructions: block generated from AGENT-INSTRUCTIONS.md
    ├── settings.mcs.yml
    ├── connectionreferences.mcs.yml
    ├── actions/                    #   Computer Use tool (instructions from CUA-TOOL-INSTRUCTIONS.md)
    ├── knowledge/                  #   knowledge file metadata (content uploaded from KNOWLEDGE.md)
    └── topics/                     #   system topics (greeting, fallback, etc.)
```

> The markdown files are canonical and tenant-agnostic. The `Claims Intake Agent/` tree is a generated export bound to the source environment — treat it as derived, not as the place to edit instruction content.

## Key Configuration Notes

- **Two models, two jobs**: orchestrator = Claude Opus 4.7 (up-front image analysis); Computer Use tool = Claude Sonnet 4.5 (form navigation). See step 4 above.
- **App must be runnable by the signed-in user**: the interactive user needs execute rights on `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe`. A "you don't have permission to open this file" error blocks the entire run and no instruction change fixes it.
- **Stage the image twice**: reachable as a OneDrive/SharePoint link (Work IQ Copilot) *and* present locally (Page 3 upload). Pre-downloading avoids a mid-run MFA prompt.
- **Input validation**: the app hard-blocks submit until the Page 4 image-analysis section is complete.
- **App credentials**: adjuster1 / pass123 (admin / admin for reset).

## Testing the Real Workflow

1. **Launch the app** on the Cloud PC from `C:\AutoClaimsFNOL\AutoClaimsFnolApp.exe` (verify it opens as the signed-in user — see the execute-permission note above).
2. **Stage a test image** in OneDrive/SharePoint (for Work IQ Copilot) and pre-download it locally (for the Page 3 upload).
3. **Invoke the agent** with the claim details + the image link.
4. **Watch it** analyze the image up front with Work IQ Copilot, confirm the fields with you, then run Computer Use once to fill all 6 pages and submit.

## Troubleshooting

**Agent won't start**: Check that the VS Code Copilot Studio extension is authenticated
**"You don't have permission to open this file" on launch**: the signed-in user lacks execute rights on the exe — fix the file/folder ACL or deploy to a per-user, executable location
**Stuck on a sign-in / MFA prompt**: the CUA should surface the number and wait; pre-authenticate Edge on the Cloud PC before the run to avoid it
**Computer Use failing**: ensure the app is running and visible on screen
**Validation errors**: check logs in the app admin panel (View Logs button)

See the main README.md for full context on the Auto Claims FNOL demo project.
