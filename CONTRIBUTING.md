# Contributing

## Development flow

1. Sync upstream web assets:
   ```powershell
   .\scripts\sync_upstream.ps1
   ```
2. Build and test:
   ```powershell
   dotnet build .\Screensaver.sln -c Release
   dotnet test .\Screensaver.sln -c Release
   ```
3. Build installer:
   ```powershell
   .\packaging\webview2-screensaver\build-and-package.ps1
   ```
   Output: `packaging/webview2-screensaver/out/installer/MatrixScreensaverSetup.exe`

## Repository layout

- `app/` — upstream web assets from Rezmason/matrix (do not edit directly)
- `packaging/webview2-screensaver/` — native WinForms/WebView2 host and installer
- `scripts/` — sync and build helpers
- `docs/` — design notes and troubleshooting
- `.github/workflows/build.yml` — CI: build, test, package, publish release

## Coding conventions

- Keep native wrapper changes under `packaging/webview2-screensaver/`.
- Keep upstream web assets under `app/` only.
- Do not modify upstream files directly — use `scripts/sync_upstream.ps1` to update them.

## Pull requests

- Include QA evidence for `/s` (fullscreen), `/p` (preview), `/c` (settings), and multi-monitor.
- Update docs when changing install paths, settings schema, or CI behavior.

## Security

- Never commit signing keys or credentials.
- Use GitHub Secrets for any optional signing pipeline.
