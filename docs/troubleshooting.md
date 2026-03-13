# Troubleshooting

## WebView2 runtime missing

**Symptom:** Black or error screen showing a runtime message.  
**Fix:** The installer handles this automatically. If you see this after a manual install, download and install the [WebView2 Evergreen Runtime](https://go.microsoft.com/fwlink/p/?LinkId=2124703) manually.

## Screensaver not visible in Screen Saver Settings

**Symptom:** MatrixScreensaver does not appear in the Screen Saver dropdown after install.  
**Fix:** Close Screen Saver Settings and reopen it. If the screensaver still does not appear, try reinstalling using the latest `MatrixScreensaverSetup.exe`.

## Preview mode does not render

**Symptom:** The preview pane in Screen Saver Settings is blank.  
**Checks:**
- Ensure WebView2 Runtime is installed.
- Update GPU drivers and verify hardware acceleration is enabled.
- Try a clean reinstall.

## Poor rendering performance

**Checks:**
- Update GPU drivers.
- Reduce FPS and bloom in the settings dialog (`Settings` button in Screen Saver Settings).

## Screensaver exits immediately

**Checks:**
- Check that no mouse movement is occurring during the startup grace period (200 ms).
- Verify the mouse movement threshold (10 px) is not being triggered by a gamepad or input device.

## Settings not saving

**Symptom:** Settings revert after closing the dialog.  
**Check:** Settings are stored in `%APPDATA%\matrix-screensaver\settings.json`. Verify the folder is writable.
