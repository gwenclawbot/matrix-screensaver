param(
    [string]$Ref = "master",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$NoInstaller,
    [switch]$SkipSync
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$syncScript = Join-Path $PSScriptRoot "sync_upstream.ps1"
$packageScript = Join-Path $repoRoot "packaging\webview2-screensaver\build-and-package.ps1"

if (-not (Test-Path $packageScript)) {
    throw "Could not find packaging script at: $packageScript"
}

if (-not $SkipSync) {
    if (-not (Test-Path $syncScript)) {
        throw "Could not find sync script at: $syncScript"
    }

    Write-Host "[1/2] Syncing upstream web assets..."
    & $syncScript -Ref $Ref
}
else {
    Write-Host "[1/2] Skipping upstream sync (SkipSync=true)."
}

Write-Host "[2/2] Building and packaging..."

if ($NoInstaller) {
    & $packageScript -Configuration $Configuration -Runtime $Runtime -NoInstaller
}
else {
    & $packageScript -Configuration $Configuration -Runtime $Runtime
}

 $releaseRoot = Join-Path $repoRoot "packaging\webview2-screensaver\out"
 $latestRelease = Get-ChildItem $releaseRoot -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -like "release*" } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($latestRelease) {
    Write-Host "Done. Portable output: $($latestRelease.FullName)"
}
else {
    Write-Host "Done. Portable output: packaging/webview2-screensaver/out/release"
}

Write-Host "If Inno Setup is installed, installer output: packaging/webview2-screensaver/out/installer"