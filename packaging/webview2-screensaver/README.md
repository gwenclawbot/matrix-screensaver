# Packaging: WebView2 Screensaver

## Build

From the repository root (recommended):

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\bootstrap.ps1
```

From this folder only:

```powershell
.\build-and-package.ps1
```

Requires .NET 8 SDK and Inno Setup 6.

Outputs:
- `out/release/` — self-contained build (`.scr` and app files)
- `out/installer/` — `MatrixScreensaverSetup.exe`

## Installer behavior

- Per-user install (default): `.scr` copied to `%LOCALAPPDATA%\Microsoft\Windows\Screensavers`
- System-wide (admin): `.scr` copied to `%WINDIR%\System32`
- App assets installed under `{app}\app`
- WebView2 Runtime downloaded and installed automatically when not present
- MatrixScreensaver registered as the active screensaver after install
