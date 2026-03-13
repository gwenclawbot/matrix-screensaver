# Matrix Screensaver (WebView2 Native .scr)

Production-oriented Windows screensaver packaging of [Rezmason/matrix](https://github.com/Rezmason/matrix) as a native `.scr` with full Screen Saver Settings support:

- `/s` full-screen run mode across all monitors.
- `/p <hwnd>` Control Panel preview embedding.
- `/c` configuration dialog with persisted settings.

## Requirements

- Windows 10/11
- .NET 8 SDK (build only)
- Microsoft Edge WebView2 Runtime (Evergreen)
- Inno Setup 6+ (installer build)

If WebView2 Runtime is missing at run time, the screensaver shows a direct install link.

## Repository Layout

- `app/`: upstream web assets from Rezmason/matrix
- `packaging/webview2-screensaver/`: native WinForms/WebView2 host
- `scripts/sync_upstream.ps1`: pull and copy upstream static files
- `.github/workflows/build.yml`: CI build + test + package
- `docs/`: architecture, troubleshooting, contributor workflow

## Quick Start

Run everything with one command from the repo root:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\bootstrap.ps1
```

This performs upstream sync and packaging in one run.

Output:
- `packaging/webview2-screensaver/out/release/` (portable)
- `packaging/webview2-screensaver/out/installer/` (setup EXE, when Inno Setup is installed)

Optional flags:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\bootstrap.ps1 -SkipSync -NoInstaller
```

## Command-Line Semantics

- `/s` or `-s`: run as screensaver (default when no args)
- `/p <hwnd>` or `-p <hwnd>`: preview inside Screen Saver Settings
- `/c` or `-c`: open settings UI
- `/t`: test mode for development (Ctrl+O opens settings)

## Settings Model

The Windows settings dialog intentionally has two layers:

- Core controls for common day-to-day tuning (version, columns, speed, FPS, bloom, etc.)
- Advanced controls and raw query passthrough for upstream parameters

If upstream adds new options before this wrapper adds dedicated controls, use the
`Custom query (any key=value&...)` field in settings. The wrapper appends this
query exactly as provided, so all upstream URL options remain available.

## Install Modes

- Per-user (no admin): `.scr` in `%LOCALAPPDATA%\Microsoft\Windows\Screensavers`
- System-wide (admin): `.scr` in `%WINDIR%\System32`
- App assets under chosen install root (`{app}\app`), with `app-path.txt` sidecar support

## Testing Checklist

See `docs/troubleshooting.md` and QA checklist in `docs/design.md`.

## License and Attribution

- Upstream `Rezmason/matrix` is MIT licensed.
- Packaging layer in this repo is MIT licensed.
- Keep attribution in `LICENSE` and `NOTICE` when redistributing.

## Security/Privacy Notes

- Screensaver reads local files and local settings JSON.
- If users add remote URLs via custom query options, remote resources may be fetched by the embedded web app.

## Contributing

See `docs/contributor.md`.
