# Contributor Workflow

## Syncing Upstream

Run:

```powershell
./scripts/sync_upstream.ps1
```

Optional build during sync:

```powershell
./scripts/sync_upstream.ps1 -BuildIfNeeded
```

## Recommended Branch Process

1. Create branch `sync/upstream-<date>`
2. Run sync script
3. Commit only `app/` changes + any required packaging compatibility updates
4. Open PR with upstream commit hash noted

## Packaging Updates

- Keep native wrapper changes in `packaging/webview2-screensaver/`
- Update tests for command parsing/settings query behavior
- Document user-visible settings changes in `README.md`
