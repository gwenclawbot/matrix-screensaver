#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Downloads and installs the latest Matrix Screensaver build from GitHub.

.DESCRIPTION
    Fetches MatrixScreensaver.zip from the latest release of
    gwenclawbot/matrix-screensaver, extracts it to Program Files, and
    registers the .scr with Windows so it appears in Screen Saver Settings.

.EXAMPLE
    irm https://raw.githubusercontent.com/gwenclawbot/matrix-screensaver/main/install.ps1 | iex
#>

$ErrorActionPreference = 'Stop'

$repo    = 'gwenclawbot/matrix-screensaver'
$tag     = 'latest'
$zipName = 'MatrixScreensaver.zip'
$installDir = "$env:ProgramFiles\MatrixScreensaver"

Write-Host "Fetching release info from github.com/$repo ..."
$apiUrl  = "https://api.github.com/repos/$repo/releases/tags/$tag"
$release = Invoke-RestMethod -Uri $apiUrl -Headers @{ 'User-Agent' = 'matrix-screensaver-installer' }

$asset = $release.assets | Where-Object { $_.name -eq $zipName } | Select-Object -First 1
if (-not $asset) {
    throw "Could not find $zipName in the latest release. Has the first build completed on GitHub Actions?"
}

$tempZip = Join-Path $env:TEMP $zipName
Write-Host "Downloading $zipName ..."
Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $tempZip

Write-Host "Installing to $installDir ..."
if (Test-Path $installDir) {
    Remove-Item $installDir -Recurse -Force
}
New-Item -ItemType Directory -Path $installDir | Out-Null
Expand-Archive -Path $tempZip -DestinationPath $installDir -Force
Remove-Item $tempZip -Force

$scrPath = Join-Path $installDir 'MatrixScreensaver.scr'
if (-not (Test-Path $scrPath)) {
    throw "MatrixScreensaver.scr not found after extraction. Something went wrong."
}

# Write app-path.txt so the screensaver can find its web assets.
Set-Content -Path (Join-Path $installDir 'app-path.txt') -Value (Join-Path $installDir 'app') -Encoding UTF8

# Register the screensaver with Windows (installs to System32 as a symlink alias).
# The simplest approach: copy to System32 and register via rundll32.
$sys32Scr = "$env:SystemRoot\System32\MatrixScreensaver.scr"
Copy-Item $scrPath $sys32Scr -Force

# Copy all required DLLs alongside the .scr in System32
Get-ChildItem $installDir -Filter '*.dll' | ForEach-Object {
    Copy-Item $_.FullName "$env:SystemRoot\System32\" -Force
}
Get-ChildItem $installDir -Filter '*.json' | ForEach-Object {
    Copy-Item $_.FullName "$env:SystemRoot\System32\" -Force
}

# Write app-path.txt into System32 pointing back to the install dir's app folder.
Set-Content -Path "$env:SystemRoot\System32\app-path.txt" -Value (Join-Path $installDir 'app') -Encoding UTF8

Write-Host ""
Write-Host "Installation complete!"
Write-Host "  Screensaver: $sys32Scr"
Write-Host "  App files:   $installDir\app"
Write-Host ""
Write-Host "To activate: right-click the desktop -> Personalize -> Lock screen -> Screen saver"
Write-Host "             and select 'MatrixScreensaver'."
Write-Host ""
Write-Host "To open settings directly, run:"
Write-Host "  $sys32Scr /c"
