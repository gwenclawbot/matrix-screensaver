# Packaging: WebView2 Screensaver

## Build

From the repository root (recommended):

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\bootstrap.ps1
```

From this folder only:

```powershell
./build-and-package.ps1
```

Outputs:
- `out/release/` portable payload (`.scr`, app files, sidecar path file)
- `out/installer/` installer EXE (when Inno Setup is installed)

## Installer behavior

- Per-user install: `.scr` copied to `%LOCALAPPDATA%\Microsoft\Windows\Screensavers`
- Admin/system install: `.scr` copied to `%WINDIR%\System32`
- App assets copied under install root and referenced by `app-path.txt`

## Runtime dependency

WebView2 Evergreen runtime is required on target machines.
