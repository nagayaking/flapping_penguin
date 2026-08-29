; flapping_penguin のインストーラー定義。
; Release ビルド済みの flapping_penguin\bin\Release\* を取り込んでインストーラーを作成する。
; バージョンは CI から /DMyAppVersion=X.Y.Z で渡される。ローカルでビルドする場合は
;   ISCC installer\flapping_penguin.iss /DMyAppVersion=0.0.0
; のように指定する(未指定時は "0.0.0" になる)。

#ifndef MyAppVersion
  #define MyAppVersion "0.0.0"
#endif

#define MyAppName "flapping_penguin"
#define MyAppPublisher "nagayaking"
#define MyAppURL "https://github.com/nagayaking/flapping_penguin"
#define MyAppExeName "flapping_penguin.exe"

[Setup]
AppId={{8EABE641-8DCB-4F68-9F6A-5866E19DF7A2}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={localappdata}\Programs\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
; 管理者権限・UACなしでインストールできるよう、ユーザー単位インストールに固定する。
PrivilegesRequired=lowest
OutputDir=Output
OutputBaseFilename=flapping_penguin-setup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\flapping_penguin\bin\Release\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
