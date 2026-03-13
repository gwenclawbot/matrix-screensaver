# Troubleshooting

## WebView2 runtime missing

Symptom:
- Black/error screen with runtime message.

Fix:
- Install Evergreen runtime: https://go.microsoft.com/fwlink/p/?LinkId=2124703
- Re-run screensaver after installation.

## Preview mode does not render

Symptom:
- `/p` appears blank in Screen Saver Settings.

Checks:
- Ensure `/p <hwnd>` handle parsing is correct.
- Verify parent window remains alive.
- Test on both Windows 10 and Windows 11.

## Poor rendering performance / WebGL issues

Checks:
- Update GPU drivers.
- Ensure hardware acceleration is enabled in Edge/WebView2 runtime.
- Test with reduced FPS and bloom settings in `/c` dialog.

## Screensaver exits immediately

Checks:
- Confirm startup grace period is not overridden.
- Validate mouse threshold logic and high-DPI behavior.

## Build issues in CI

Checks:
- Ensure `app/index.html` exists (run sync script first).
- Verify .NET SDK 8 and Inno Setup installation in workflow.
