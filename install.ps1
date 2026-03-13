<#
.SYNOPSIS
    Downloads and runs the latest Matrix Screensaver installer from GitHub.

.DESCRIPTION
    Fetches MatrixScreensaverSetup.exe from the latest release of
    gwenclawbot/matrix-screensaver and runs it silently. The installer
    copies the .scr, installs prerequisites when needed, and refreshes
    Windows Screen Saver Settings.

.EXAMPLE
    irm https://raw.githubusercontent.com/gwenclawbot/matrix-screensaver/main/install.ps1 | iex
#>

$ErrorActionPreference = 'Stop'

$repo    = 'gwenclawbot/matrix-screensaver'
$tag     = 'latest'
$installerName = 'MatrixScreensaverSetup.exe'

Write-Host "Fetching release info from github.com/$repo ..."
$apiUrl  = "https://api.github.com/repos/$repo/releases/tags/$tag"
$release = Invoke-RestMethod -Uri $apiUrl -Headers @{ 'User-Agent' = 'matrix-screensaver-installer' }

$asset = $release.assets | Where-Object { $_.name -eq $installerName } | Select-Object -First 1
if (-not $asset) {
    throw "Could not find $installerName in the latest release. Has the first build completed on GitHub Actions?"
}

$tempInstaller = Join-Path $env:TEMP $installerName
Write-Host "Downloading $installerName ..."
Invoke-WebRequest -Uri $asset.browser_download_url -OutFile $tempInstaller

Write-Host "Running installer ..."
$arguments = '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART /TASKS="setactive"'
$process = Start-Process -FilePath $tempInstaller -ArgumentList $arguments -Wait -PassThru
Remove-Item $tempInstaller -Force

if ($process.ExitCode -ne 0) {
    throw "Installer failed with exit code $($process.ExitCode)."
}

Write-Host ""
Write-Host "Installation complete!"
Write-Host ""
Write-Host "To activate: right-click the desktop -> Personalize -> Lock screen -> Screen saver"
Write-Host "             and select 'MatrixScreensaver'."
Write-Host ""
Write-Host "To open settings directly, run:"
Write-Host "  %WINDIR%\System32\MatrixScreensaver.scr /c"
