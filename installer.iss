; Moscovium v3.0 — Inno Setup Installer Script

#define MyAppName "Moscovium"
#define MyAppVersion "3.0.1"
#define MyAppPublisher "Unknown Cyberia"
#define MyAppExeName "MoscoviumThree.exe"
#define MyAppDescription "Windows utility toolbox"

#define PublishDir "bin\x64\Release\net10.0-windows10.0.19041.0\win-x64\publish"

[Setup]
AppId={{D4A3E2B1-7C5F-4E8D-9A6B-3F1C2D5E8A7B}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL=https://github.com/Moscoviumdebloat
AppSupportURL=https://github.com/Moscoviumdebloat
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
OutputDir=Output
OutputBaseFilename=MoscoviumSetup-{#MyAppVersion}
SetupIconFile=Assets\Logo.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardImageFile=Assets\WizardImage.bmp
WizardSmallImageFile=Assets\WizardImage.bmp
Compression=lzma2/normal
SolidCompression=yes
LZMANumBlockThreads=4
WizardStyle=modern
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
MinVersion=10.0.17763

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startmenu"; Description: "Create a Start Menu shortcut"; GroupDescription: "{cm:AdditionalIcons}"

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startmenu
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"; Tasks: startmenu
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
