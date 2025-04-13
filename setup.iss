[Setup]
AppName=DisplayCountWatcherService
AppVersion=1.0.0
DefaultDirName={pf}\DisplayCountWatcherService
DefaultGroupName=DisplayCountWatcherService
OutputBaseFilename=DisplayCountWatcherServiceInstaller
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin

[Files]
Source: ".\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\DisplayCountWatcherService"; Filename: "{app}\DisplayCountWatcherService.exe"
Name: "{group}\Uninstall DisplayCountWatcherService"; Filename: "{uninstallexe}"

[Run]
; Register the service using sc.exe
Filename: "sc.exe"; Parameters: "create DisplayCountWatcherService binPath= ""{app}\DisplayCountWatcherService.exe"" start= auto"; Flags: runhidden

[UninstallRun]
; Unregister the service during uninstallation
Filename: "sc.exe"; Parameters: "delete DisplayCountWatcherService"; Flags: runhidden
