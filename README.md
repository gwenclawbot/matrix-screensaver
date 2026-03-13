# Matrix Screensaver

A Windows screensaver based on [Rezmason/matrix](https://github.com/Rezmason/matrix), packaged as a native `.scr` with full Screen Saver Settings integration.

## Install

1. Go to [Releases](https://github.com/gwenclawbot/matrix-screensaver/releases/latest)
2. Download `MatrixScreensaverSetup.exe`
3. Run the installer — prerequisites (WebView2 Runtime) are installed automatically if needed
4. Open **Screen Saver Settings** and select **MatrixScreensaver**

## Features

- Full-screen animation across all monitors
- Preview in the Screen Saver Settings panel
- Settings dialog for speed, FPS, bloom, columns, palette, and more
- Per-user install (no admin required) or system-wide install

## Settings

Right-click the desktop → Personalize → Lock screen → Screen saver → **Settings**

Or run directly:

```
MatrixScreensaver.scr /c
```

Two layers are available:
- **Core controls** — common daily tuning (version, columns, speed, FPS, bloom, etc.)
- **Custom query** — pass any upstream URL parameters directly (e.g. `palette=operator&numColumns=60`)

## Building from Source

Requirements: Windows, .NET 8 SDK, Inno Setup 6

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\bootstrap.ps1
```

Outputs:
- `packaging/webview2-screensaver/out/release/` — portable build
- `packaging/webview2-screensaver/out/installer/` — setup EXE

See [packaging/webview2-screensaver/README.md](packaging/webview2-screensaver/README.md) and [CONTRIBUTING.md](CONTRIBUTING.md) for full contributor workflow.

## License

- Upstream [Rezmason/matrix](https://github.com/Rezmason/matrix) is MIT licensed.
- Packaging in this repository is MIT licensed.
- See `LICENSE` and `NOTICE` for attribution details.
