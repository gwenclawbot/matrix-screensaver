# Contributor Workflow

## Syncing upstream

Run from the repo root:

```powershell
.\scripts\sync_upstream.ps1
```

Optionally build the upstream project before syncing (requires npm):

```powershell
.\scripts\sync_upstream.ps1 -BuildIfNeeded
```

## Recommended branch process

1. Create a branch: `sync/upstream-<date>`
2. Run the sync script
3. Commit only `app/` changes and any required packaging compatibility updates
4. Open a PR noting the upstream commit hash

## Packaging updates

- Keep native wrapper changes in `packaging/webview2-screensaver/`
- Update tests for command parsing and settings query behavior
- Document user-visible settings changes in `README.md`
