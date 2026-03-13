#define MyAppName "Matrix Screensaver"
#define MyAppVersion "0.1.0"
#define MyAppPublisher "Community"

#ifndef SourceDir
  #define SourceDir "..\\out\\release"
#endif

[Setup]
AppId={{7ACB0A39-B8A0-4C57-BB17-76553B87FC2E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\MatrixScreensaver
DefaultGroupName=Matrix Screensaver
DisableProgramGroupPage=yes
OutputDir=..\out\installer
OutputBaseFilename=MatrixScreensaverSetup
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
CloseApplications=yes
RestartApplications=no
UninstallDisplayIcon={app}\MatrixScreensaver.scr

[Tasks]
Name: "setactive"; Description: "Set as active screen saver for current account"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: recursesubdirs createallsubdirs
Source: "{#SourceDir}\MatrixScreensaver.scr"; DestDir: "{code:GetScrTarget}"; Flags: ignoreversion restartreplace
Source: "{#SourceDir}\MicrosoftEdgeWebView2Setup.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Registry]
Root: HKCU; Subkey: "Control Panel\Desktop"; ValueType: string; ValueName: "SCRNSAVE.EXE"; ValueData: "{code:GetScrTarget}\MatrixScreensaver.scr"; Flags: uninsdeletevalue; Tasks: setactive; Check: not IsAdminInstallMode
Root: HKLM; Subkey: "SOFTWARE\Microsoft\Windows NT\CurrentVersion\ScreenSavers"; ValueType: string; ValueName: "MatrixScreensaverPath"; ValueData: "{app}\app"; Flags: uninsdeletevalue; Check: IsAdminInstallMode
Root: HKCU; Subkey: "Software\MatrixScreensaver"; ValueType: string; ValueName: "AppPath"; ValueData: "{app}\app"; Flags: uninsdeletevalue; Check: not IsAdminInstallMode

[Code]
function GetScrTarget(Default: string): string;
begin
  if IsAdminInstallMode then
    Result := ExpandConstant('{sys}')
  else
    Result := ExpandConstant('{localappdata}\Microsoft\Windows\Screensavers');
end;

function IsWebView2RuntimeInstalled(): Boolean;
var
  VersionValue: string;
begin
  Result :=
    RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', VersionValue)
    or RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\EdgeUpdate\Clients\{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}', 'pv', VersionValue);
end;

procedure StopRunningMatrixProcesses();
var
  ResultCode: Integer;
begin
  Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM MatrixScreensaver.scr /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM MatrixScreensaver.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
begin
  StopRunningMatrixProcesses();
  Result := '';
end;

[Run]
Filename: "{tmp}\MicrosoftEdgeWebView2Setup.exe"; Parameters: "/silent /install"; Flags: waituntilterminated; StatusMsg: "Installing Microsoft Edge WebView2 Runtime..."; Check: not IsWebView2RuntimeInstalled
; Notify Windows to refresh display/screensaver settings so the new .scr
; appears in Screen Saver Settings without requiring a logoff/logon cycle.
Filename: "rundll32.exe"; Parameters: "user32.dll,UpdatePerUserSystemParameters"; Flags: nowait; StatusMsg: "Refreshing display settings..."

[Icons]
Name: "{group}\Configure"; Filename: "{code:GetScrTarget}\MatrixScreensaver.scr"; Parameters: "/c"
Name: "{group}\Run"; Filename: "{code:GetScrTarget}\MatrixScreensaver.scr"; Parameters: "/s"
Name: "{group}\Uninstall"; Filename: "{uninstallexe}"
