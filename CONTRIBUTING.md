# Contributing

## Development flow

1. Sync upstream web assets:
   - `./scripts/sync_upstream.ps1`
2. Build and run tests:
   - `dotnet build ./Screensaver.sln -c Release`
   - `dotnet test ./Screensaver.sln -c Release`
3. Build installer artifacts:
   - `./packaging/webview2-screensaver/build-and-package.ps1`

## Coding conventions

- Keep packaging/native code under `packaging/webview2-screensaver/`.
- Keep upstream web assets under `app/` only.
- Do not modify upstream files directly unless syncing.

## Security

- Never commit signing keys or credentials.
- Use GitHub Secrets for optional signing pipeline.

## Pull requests

- Include QA evidence for `/s`, `/p`, `/c`, and multi-monitor behavior.
- Update docs when changing install paths, settings schema, or CI.
