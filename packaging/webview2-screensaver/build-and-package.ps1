param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64",
    [switch]$NoInstaller
)

$ErrorActionPreference = "Stop"

function Resolve-DotnetPath {
    $cmd = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($cmd) {
        return $cmd.Source
    }

    $defaultPath = "C:\Program Files\dotnet\dotnet.exe"
    if (Test-Path $defaultPath) {
        return $defaultPath
    }

    return $null
}

$dotnetPath = Resolve-DotnetPath
if (-not $dotnetPath) {
    throw "dotnet SDK is required. Install .NET 8 SDK and retry."
}

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$projectDir = Join-Path $PSScriptRoot "MatrixScreensaver"
$publishDir = Join-Path $PSScriptRoot "out\publish"
$releaseDir = Join-Path $PSScriptRoot "out\release"
$effectiveReleaseDir = $releaseDir
$appSource = Join-Path $repoRoot "app"

if (-not (Test-Path (Join-Path $appSource "index.html"))) {
    throw "app/index.html not found. Run scripts/sync_upstream.ps1 first."
}

Write-Host "Publishing MatrixScreensaver..."
& $dotnetPath publish "$projectDir\MatrixScreensaver.csproj" -c $Configuration -r $Runtime --self-contained false -p:PublishSingleFile=false -o $publishDir

try {
    if (Test-Path $effectiveReleaseDir) {
        Remove-Item $effectiveReleaseDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $effectiveReleaseDir | Out-Null
}
catch {
    $timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $effectiveReleaseDir = Join-Path $PSScriptRoot ("out\release-" + $timestamp)
    Write-Warning "Could not clean default release folder (possibly locked). Using: $effectiveReleaseDir"
    if (Test-Path $effectiveReleaseDir) {
        Remove-Item $effectiveReleaseDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $effectiveReleaseDir | Out-Null
}

$appTarget = Join-Path $effectiveReleaseDir "app"
New-Item -ItemType Directory -Path $appTarget -Force | Out-Null

Copy-Item -Path (Join-Path $publishDir "*") -Destination $effectiveReleaseDir -Recurse -Force
Copy-Item -Path (Join-Path $appSource "*") -Destination $appTarget -Recurse -Force

$exePath = Join-Path $effectiveReleaseDir "MatrixScreensaver.exe"
if (-not (Test-Path $exePath)) {
    throw "Expected published executable not found: $exePath"
}

$scrPath = Join-Path $effectiveReleaseDir "MatrixScreensaver.scr"
Copy-Item $exePath $scrPath -Force

# Allows .scr in System32 to find app files under Program Files or LocalAppData.
Set-Content -Path (Join-Path $effectiveReleaseDir "app-path.txt") -Value $appTarget -Encoding UTF8

if (-not $NoInstaller) {
    if (-not (Get-Command iscc -ErrorAction SilentlyContinue)) {
        Write-Warning "Inno Setup Compiler (iscc) not found; skipping installer."
    } else {
        $iss = Join-Path $PSScriptRoot "installer\MatrixScreensaver.iss"
        & iscc "/DSourceDir=$effectiveReleaseDir" $iss
    }
}

Write-Host "Packaging complete. Output: $effectiveReleaseDir"
