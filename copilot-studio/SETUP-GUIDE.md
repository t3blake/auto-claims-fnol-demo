# Copilot Studio Agent Setup Guide

This directory contains the scaffolding for the **Auto Claims FNOL Computer Use Agent**.

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

### 3. Create or Clone an Agent

#### Option A: Create a New Agent
- Open Command Palette
- Type "Copilot Studio: Create Agent"
- Name it: `AutoClaimsFnolAgent`
- Select your environment
- The agent YAML scaffold will be created

#### Option B: Use Existing YAML Files
- The agent structure is defined in `agent.yaml`
- Topic files are in `topics/` folder
- You can push these to the cloud using the extension

### 4. Configure the Agent

In Copilot Studio:
1. **Basic Settings**
   - Display Name: "Auto Claims FNOL - Computer Use Demo"
   - Description: "Computer Use Agent that processes accident images and files claims"

2. **Instructions** (from AGENT-INSTRUCTIONS.md)
   - Paste the contents into: Agent → Instructions

3. **Topics**
   - The topic files are scaffolded and ready
   - You'll need to add detailed Computer Use node configurations in the Copilot Studio UI

4. **Computer Use Configuration**
   - Add a Computer Use action/tool
   - Configure it to control the claims app
   - Test with a sample interaction

### 5. Add Knowledge Sources (Optional)

If you want the agent to reference documentation:
- Add: `copilot-studio/KNOWLEDGE.md` as embedded knowledge
- Add: `copilot-studio/CUA-TOOL-INSTRUCTIONS.md` for tool guidance

### 6. Test the Agent

Use the **Evaluate** tab in Copilot Studio:
- Import test cases from: `evaluation-*.csv` files
- Run 20+ test scenarios
- Review results and iterate

### 7. Deploy When Ready

- Publish the agent from Copilot Studio UI
- The published version is ready for Copilot integration or standalone deployment

## File Structure

```
copilot-studio/
├── agent.yaml                      # Main agent configuration
├── topics/
│   ├── InitializeAgent.yaml        # Welcome & setup topic
│   ├── FileNewClaim.yaml           # Main claim filing workflow
│   └── SearchExistingClaim.yaml    # Claim search topic
├── AGENT-INSTRUCTIONS.md           # Detailed instructions to paste into UI
├── CUA-TOOL-INSTRUCTIONS.md        # Computer Use tool reference
├── KNOWLEDGE.md                    # Reference documentation
├── EVALUATION.md                   # Evaluation plan
└── evaluation-*.csv                # Test cases (4 batches)
```

## Key Configuration Notes

- **Computer Use Enabled**: The agent uses Computer Use actions to control the WPF app
- **Temperature**: Set to 0.3 (deterministic behavior)
- **Max Tokens**: 8000 (sufficient for multi-step workflows)
- **Input Validation**: The app enforces hard validation on image analysis section
- **App Credentials**: adjuster1/pass123

## Testing the Real Workflow

1. **Launch the app** from: `d:\Git\auto-claims-fnol-demo\src\AutoClaimsFnolApp\bin\Release\net8.0-windows\AutoClaimsFnolApp.exe`
2. **Prepare a test image** (accident photo or sketch)
3. **Invoke the agent** with: "File a claim"
4. **Watch it** navigate through all 6 pages, analyze the image, and submit

## Troubleshooting

**Agent won't start**: Check that VS Code Copilot Studio extension is authenticated
**Topics not loading**: Verify YAML syntax using the extension validator
**Computer Use failing**: Ensure the app is running and visible on screen
**Validation errors**: Check logs in app admin panel (View Logs button)

See the main README.md for full context on the Auto Claims FNOL demo project.
