;Include Modern UI
	!include "MUI2.nsh"

;General

  ;Name and file
  !define CORP "Innodisk"
  !define NAME "iCAP_ClientService"
  !define VERSION "1.2.1.0"

  Name "${NAME}${VERSION}"
  OutFile "${NAME}_win32_${VERSION}.exe"

  ;Default installation folder
  InstallDir "$PROGRAMFILES\${CORP}\${NAME}"

  !addplugindir "${NSISDIR}\Plugins\NSIS_Simple_Service_Plugin_1.30"

;--------------------------------
;Interface Settings
  !define MUI_HEADERIMAGE
  !define MUI_HEADERIMAGE_BITMAP "${NSISDIR}\Contrib\Graphics\Header\win.bmp" ;
  !define MUI_ABORTWARNING
  ShowInstDetails show

;--------------------------------
;Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE EULA.txt
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY
  !insertmacro MUI_PAGE_INSTFILES
  
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  
;--------------------------------
;Languages
 
  !insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections

Section "${NAME}" SecDummy

  SetOutPath "$INSTDIR"
  
  File "..\ClientService\iCAP_ClientService.exe"
  File "..\SetupAgent\iCAP_ClientService_Setting.exe"
  File "..\..\..\..\..\..\..\..\Inno\iSMART\windows\library\iSMART\lib_ismart.dll"
  File "..\..\..\..\..\..\library\lib_SysInfo\bin\lib_SysInfo.dll"
  File "..\Dependencies\Windows\GTK3\libatk-1.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libcairo-2.dll"
  File "..\Dependencies\Windows\GTK3\libcairo-gobject-2.dll"
  File "..\Dependencies\Windows\GTK3\libcairo-script-interpreter-2.dll"
  File "..\Dependencies\Windows\GTK3\libcroco-0.6-3.dll"
  File "..\Dependencies\Windows\curl-7.44.0\lib\libcurl.dll"
  File "..\Dependencies\Windows\GTK3\libffi-6.dll"
  File "..\Dependencies\Windows\GTK3\libfontconfig-1.dll"
  File "..\Dependencies\Windows\GTK3\libfreetype-6.dll"
  File "..\Dependencies\Windows\GTK3\libgailutil-3-0.dll"
  File "..\Dependencies\Windows\mingw\libgcc_s_dw2-1.dll"
  File "..\Dependencies\Windows\GTK3\libgdk_pixbuf-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libgdk-3-0.dll"
  File "..\Dependencies\Windows\GTK3\libgio-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libglib-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libgmodule-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libgobject-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libgthread-2.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libgtk-3-0.dll"
  File "..\Dependencies\Windows\GTK3\libiconv-2.dll"
  File "..\Dependencies\Windows\GTK3\libintl-8.dll"
  File "..\Dependencies\Windows\GTK3\libjasper-1.dll"
  File "..\Dependencies\Windows\GTK3\libjpeg-9.dll"
  File "..\Dependencies\Windows\json-c\libjson-c-2.dll"
  File "..\Dependencies\Windows\GTK3\liblzma-5.dll"
  File "..\Dependencies\Windows\GTK3\libpango-1.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libpangocairo-1.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libpangoft2-1.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libpangowin32-1.0-0.dll"
  File "..\Dependencies\Windows\GTK3\libpixman-1-0.dll"
  File "..\Dependencies\Windows\GTK3\libpng15-15.dll"
  File "..\Dependencies\Windows\GTK3\librsvg-2-2.dll"
  File "..\Dependencies\Windows\GTK3\libtiff-5.dll"
  File "..\Dependencies\Windows\mingw\libwinpthread-1.dll"
  File "..\Dependencies\Windows\GTK3\libxml2-2.dll"
  File "..\Dependencies\Windows\GTK3\msvcr120.dll"
  File "..\..\..\..\..\..\library\eclipse-paho-mqtt-c-windows-1.1.0\lib\paho-mqtt3a.dll"
  File "..\Dependencies\Windows\GTK3\pthreadGC2.dll"

  File "..\Dependencies\Windows\sqlite3\sqlite3.dll"
  File "..\Dependencies\Windows\GTK3\zlib1.dll"

  SetOutPath "$SYSDIR"

  File "..\Dependencies\Windows\ismart\Flash.ini"
  File "..\Dependencies\Windows\ismart\mo.txt"

  ;Store installation folder
  #WriteRegStr HKCU "Software\iCAP_ClientService" "" $INSTDIR

  ;Create uninstaller
  WriteUninstaller "$INSTDIR\Uninstall.exe"

  SetOutPath "$INSTDIR"
  CreateDirectory "$SMPROGRAMS\${CORP}"
  CreateDirectory "$SMPROGRAMS\${CORP}\${NAME}"
  CreateShortCut "$SMPROGRAMS\${CORP}\${NAME}\${NAME}.lnk" "$INSTDIR\iCAP_ClientService_Setting.exe"
  CreateShortCut "$SMPROGRAMS\${CORP}\${NAME}\Uninstall.lnk" "$INSTDIR\Uninstall.exe"

  SimpleSC::InstallService "iCAP" "iCAP Client Service" "16" "2" "$INSTDIR\iCAP_ClientService.exe" "" "" ""
  SimpleSC::SetServiceDescription "iCAP" "Innodisk iCAP client service"
  SimpleSC::SetServiceStartType "iCAP" "2"
  SimpleSC::SetServiceDelayedAutoStartInfo "iCAP" "1"

  SimpleSC::StartService "iCAP" "" 30
SectionEnd

Section "Create desktop shortcuts" CreateShortcuts

  SetOutPath "$INSTDIR"

  CreateShortCut "$DESKTOP\${NAME} Setting.lnk" "$INSTDIR\iCAP_ClientService_Setting.exe"

  SetShellVarContext all
  SetOutPath "$APPDATA\${CORP}\${NAME}"
  File "..\ClientService\ServiceSetting.json"

SectionEnd

;--------------------------------
;Descriptions

  ;Language strings
  LangString DESC_SecDummy ${LANG_ENGLISH} "iCAP ClientService."
  LangString DESC_CreateShort ${LANG_ENGLISH} "Create desktop shortcut."

  ;Assign language strings to sections
  !insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
    !insertmacro MUI_DESCRIPTION_TEXT ${SecDummy} $(DESC_SecDummy)
    !insertmacro MUI_DESCRIPTION_TEXT ${CreateShortcuts} $(DESC_CreateShort)
  !insertmacro MUI_FUNCTION_DESCRIPTION_END

;--------------------------------
;Uninstaller Section

Section "Uninstall"

  ;ADD YOUR OWN FILES HERE...

  SimpleSC::StopService "iCAP" 1 30
  SimpleSC::RemoveService "iCAP"

  Delete "$INSTDIR\Uninstall.exe"
  Delete "$INSTDIR\iCAP_ClientService.exe"
  Delete "$INSTDIR\iCAP_ClientService_Setting.exe"
  Delete "$INSTDIR\lib_ismart.dll"
  Delete "$INSTDIR\lib_SysInfo.dll"
  Delete "$INSTDIR\libatk-1.0-0.dll"
  Delete "$INSTDIR\libcairo-2.dll"
  Delete "$INSTDIR\libcairo-gobject-2.dll"
  Delete "$INSTDIR\libcairo-script-interpreter-2.dll"
  Delete "$INSTDIR\libcroco-0.6-3.dll"
  Delete "$INSTDIR\libcurl.dll"
  Delete "$INSTDIR\libffi-6.dll"
  Delete "$INSTDIR\libfontconfig-1.dll"
  Delete "$INSTDIR\libfreetype-6.dll"
  Delete "$INSTDIR\libgailutil-3-0.dll"
  Delete "$INSTDIR\libgcc_s_dw2-1.dll"
  Delete "$INSTDIR\libgdk_pixbuf-2.0-0.dll"
  Delete "$INSTDIR\libgdk-3-0.dll"
  Delete "$INSTDIR\libgio-2.0-0.dll"
  Delete "$INSTDIR\libglib-2.0-0.dll"
  Delete "$INSTDIR\libgmodule-2.0-0.dll"
  Delete "$INSTDIR\libgobject-2.0-0.dll"
  Delete "$INSTDIR\libgthread-2.0-0.dll"
  Delete "$INSTDIR\libgtk-3-0.dll"
  Delete "$INSTDIR\libiconv-2.dll"
  Delete "$INSTDIR\libintl-8.dll"
  Delete "$INSTDIR\libjasper-1.dll"
  Delete "$INSTDIR\libjpeg-9.dll"
  Delete "$INSTDIR\libjson-c-2.dll"
  Delete "$INSTDIR\liblzma-5.dll"
  Delete "$INSTDIR\libpango-1.0-0.dll"
  Delete "$INSTDIR\libpangocairo-1.0-0.dll"
  Delete "$INSTDIR\libpangoft2-1.0-0.dll"
  Delete "$INSTDIR\libpangowin32-1.0-0.dll"
  Delete "$INSTDIR\libpixman-1-0.dll"
  Delete "$INSTDIR\libpng15-15.dll"
  Delete "$INSTDIR\librsvg-2-2.dll"
  Delete "$INSTDIR\libtiff-5.dll"
  Delete "$INSTDIR\libwinpthread-1.dll"
  Delete "$INSTDIR\libxml2-2.dll"
  Delete "$INSTDIR\msvcr120.dll"
  Delete "$INSTDIR\paho-mqtt3a.dll"
  Delete "$INSTDIR\pthreadGC2.dll"
  Delete "$INSTDIR\ServiceSetting.json"
  Delete "$INSTDIR\sqlite3.dll"
  Delete "$INSTDIR\zlib1.dll"
  Delete "$INSTDIR\ProgramLog.log"
  RMDir "$INSTDIR"
  RMDir "$PROGRAMFILES\${CORP}"

  Delete "$SYSDIR\Flash.ini"
  Delete "$SYSDIR\mo.txt"

  Delete "$SMPROGRAMS\${CORP}\${NAME}\${NAME}.lnk"
  Delete "$SMPROGRAMS\${CORP}\${NAME}\Uninstall.lnk"
  Delete "$DESKTOP\${NAME} Setting.lnk"
  RMDir "$SMPROGRAMS\${CORP}\${NAME}"
  RMDir "$SMPROGRAMS\${CORP}"

SectionEnd