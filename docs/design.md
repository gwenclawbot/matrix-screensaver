# Design

## Architecture

- Native host: WinForms + WebView2 (`MatrixScreensaver`)
- Web content: local `app/index.html` from upstream Rezmason/matrix
- Settings: `%APPDATA%\matrix-screensaver\settings.json` with schema version
- Installer: Inno Setup script under `packaging/webview2-screensaver/installer/`

## Runtime Modes

- `/s`: one borderless top-most window per monitor (`Screen.AllScreens`)
- `/p <hwnd>`: embeds preview form into supplied parent HWND
- `/c`: opens settings dialog and persists configuration
- `/t`: development mode (windowed, Ctrl+O opens config)

## Input Exit Behavior

- Startup grace period: 200ms
- Exit if keyboard/mouse click occurs after grace period
- Exit if mouse moves beyond threshold (10 px)

## Multi-monitor Strategy

- Create one `ScreensaverForm` per display
- Shared settings/query for initial release
- Exit request from any form tears down all forms via app context

## QA Smoke Checklist

- [ ] `/s` covers all displays
- [ ] `/p <hwnd>` renders in Screen Saver Settings preview pane
- [ ] `/c` opens, saves, and restores defaults
- [ ] Cursor hidden during `/s`
- [ ] Exit-on-input behavior matches grace/threshold rules
- [ ] WebGL renders visual output equal to upstream app
- [ ] Missing WebView2 runtime produces clear guidance
