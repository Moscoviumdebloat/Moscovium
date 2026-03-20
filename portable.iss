; Moscovium Portable — Self-Extracting Executable
#define MyAppName "Moscovium Portable"
#define MyAppVersion "3.4.0"
#define MyAppExeName "MoscoviumThree.exe"
#define PublishDir "bin\Release\net10.0-windows10.0.19041.0\win-x64\publish"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={tmp}\Moscovium
UsePreviousAppDir=no
CreateAppDir=no
Uninstallable=no
OutputDir=Output
OutputBaseFilename=MoscoviumPortable
Compression=lzma2/ultra64
SolidCompression=yes
PrivilegesRequired=admin
DisableDirPage=yes
DisableProgramGroupPage=yes
DisableReadyPage=yes
DisableFinishedPage=yes
DisableWelcomePage=yes
WizardStyle=modern
SetupIconFile=Assets\Logo.ico

[Files]
Source: "{#PublishDir}\*"; DestDir: "{tmp}\Moscovium"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
Filename: "{tmp}\Moscovium\{#MyAppExeName}"; Flags: waituntilterminated skipifsilent
