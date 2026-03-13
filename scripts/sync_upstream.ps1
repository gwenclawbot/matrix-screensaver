param(
    [string]$Ref = "master",
    [switch]$BuildIfNeeded
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$tempRoot = Join-Path $repoRoot "upstream-temp"
$upstreamDir = Join-Path $tempRoot "matrix"
$appDir = Join-Path $repoRoot "app"

if (Test-Path $tempRoot) {
    Remove-Item -Path $tempRoot -Recurse -Force
}

New-Item -ItemType Directory -Path $tempRoot | Out-Null

Write-Host "Cloning Rezmason/matrix ($Ref)..."
git clone --depth 1 --branch $Ref https://github.com/Rezmason/matrix.git $upstreamDir

$sourceDir = $upstreamDir

if ($BuildIfNeeded -and (Test-Path (Join-Path $upstreamDir "package.json"))) {
    if (Get-Command npm -ErrorAction SilentlyContinue) {
        Push-Location $upstreamDir
        npm ci
        npm run build
        Pop-Location

        if (Test-Path (Join-Path $upstreamDir "dist")) {
            $sourceDir = Join-Path $upstreamDir "dist"
        }
    }
}

if (-not (Test-Path (Join-Path $sourceDir "index.html"))) {
    throw "Could not locate index.html in upstream source directory: $sourceDir"
}

if (Test-Path $appDir) {
    Get-ChildItem $appDir -Force | Remove-Item -Recurse -Force
} else {
    New-Item -ItemType Directory -Path $appDir | Out-Null
}

Write-Host "Copying upstream static files into app/..."
Copy-Item -Path (Join-Path $sourceDir "*") -Destination $appDir -Recurse -Force

Write-Host "Sync complete."
