@echo OFF
rem b : build code
rem cr : compress file for Release
rem cd : compress file for DemoKit
set DOC=eula.md
set GATEWAY=icap_gateway.tar, innoAGE-Gateway.tar
REM set CM=icap_cluster_manager.tar
set CORE_SERVICE=icap_coreservice_datahandler.tar, icap_coreservice_dm.tar, icap_coreservice_dlm.tar, icap_coreservice_notify.tar, icap_coreservice_storanalyzer.tar, icap_coreservice_dashboardagent.tar, icap_coreservice_innoagemanager.tar
set WEB_SERVICE=icap_webservice_authapi.tar, icap_webservice_dashboardapi.tar, icap_webservice_deviceapi.tar, icap_webservice_oobservice.tar, innoAGE-WebService.tar
set WEBSITE=icap_webservice_website.tar
set IMAGE=Images.tar.gz
set CONFIG=Config, setting.json, setting.env, Log_Update.cmd
set EXE_FILE=.\dist\*
If "%1" == "cr" (
	set DATABASE=icap_admindb.tar, icap_datadb.tar, icap_redis.tar, icap_dbchecker.tar
	set MSG=Start to compress archive for Release Version
	md iCAP_Server_windows_Release
	set OUTPUT_PATH=.\iCAP_Server_windows_Release\iCAP_Server_windows_V_1_6_1.zip
	goto iCAP_windows
)
If "%1" == "cd" (
	set DATABASE=icap_admindb.tar, icap_datadb.tar, icap_redis.tar, icap_dbchecker.tar, icap_mockdata.tar
	set MSG=Start to compress archive for DemoKit Version
	md iCAP_Server_windows_DemoKit
	set OUTPUT_PATH=.\iCAP_Server_windows_DemoKit\iCAP_Server_windows_V_1_6_1.zip
	goto iCAP_windows
)

If "%1" == "b" goto build_iCAP_Installer_windows

echo usage: .\build_iCAP_Server_Installer.bat [cr, cd, b]
echo cr : compress file for Release
echo cd : compress file for DemoKit
echo b : build code
goto end

:iCAP_windows
pyinstaller -n iCAP_Server_Installer.exe -F iCAP_Server_Installer.py
pyinstaller -n iCAP_Server_Uninstall.exe -F iCAP_Server_Uninstall_WIN.py
cls
echo %MSG%
powershell Compress-Archive -Force -Path %DATABASE%, %DOC%, %GATEWAY%, %CORE_SERVICE%, ^
%WEB_SERVICE%, %WEBSITE%, %IMAGE%, %CONFIG%, %EXE_FILE% ^
-DestinationPath %OUTPUT_PATH%
goto clean

rem rem -------------------------------------------------------------------------------------------------------------------

:build_iCAP_Installer_windows
echo build_iCAP_Installer_windows
pyinstaller -n iCAP_Server_Installer.exe -F iCAP_Server_Installer.py
pyinstaller -n iCAP_Server_Uninstall.exe -F iCAP_Server_Uninstall_WIN.py
goto clean

rem -------------------------------------------------------------------------------------------------------------------

:clean
rd __pycache__ /s /q
rd build /s /q
del *.spec /q
rem -------------------------------------------------------------------------------------------------------------------
:end