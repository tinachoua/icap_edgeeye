include					../config/current.mk
include					../config/device_image.mk
Q						:=@
SETTING_FILE			:=../config/current.mk
OUTPUT_PATH				:=bin
SERVER_VERSION_MAJOR	:=1
SERVER_VERSION_MINOR	:=6
SERVER_VERSION_BUILD	:=1
RELEASE_SERVER_TAR		:=iCAP_Server_V_1_6_1.tar.gz

IMG_DATADB						:=icap_datadb:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_ADMINDB						:=icap_admindb:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_REDIS						:=icap_redis:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_GATEWAY_AUTH				:=icap_gateway:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_SOURCE			:=icap_coreservice_base:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_DM				:=icap_coreservice_dm:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_DATAHANDLER		:=icap_coreservice_datahandler:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_STORANALYZER	:=icap_coreservice_storanalyzer:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_NOTIFY			:=icap_coreservice_notify:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_DASHBOARDAGENT	:=icap_coreservice_dashboardagent:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_DLM				:=icap_coreservice_dlm:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_CORESERVICE_IAM				:=icap_coreservice_innoagemanager:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
# IMG_CLUSTER_MANAGER				:=icap_cluster_manager:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_AUTHAPI			:=icap_webservice_authapi:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_DEVICEAPI		:=icap_webservice_deviceapi:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_DASHBOARDAPI		:=icap_webservice_dashboardapi:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_OOBSERVICE		:=icap_webservice_oobservice:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_WEBSITE			:=icap_webservice_website:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_W3M_EULA					:=icap_eula:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_DBCHECKER		:=icap_dbchecker:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)
IMG_WEBSERVICE_MOCKDATA			:=icap_mockdata:v$(SERVER_VERSION_MAJOR).$(SERVER_VERSION_MINOR).$(SERVER_VERSION_BUILD)

TAR_DATADB						:=icap_datadb.tar
TAR_ADMINDB						:=icap_admindb.tar
TAR_REDIS						:=icap_redis.tar
TAR_GATEWAY_AUTH				:=icap_gateway.tar
# TAR_CLUSTER_MANAGER				:=icap_cluster_manager.tar
TAR_CORESERVICE_DM				:=icap_coreservice_dm.tar
TAR_CORESERVICE_DATAHANDLER		:=icap_coreservice_datahandler.tar
TAR_CORESERVICE_STORANALYZER	:=icap_coreservice_storanalyzer.tar
TAR_CORESERVICE_NOTIFY			:=icap_coreservice_notify.tar
TAR_CORESERVICE_DASHBOARDAGENT	:=icap_coreservice_dashboardagent.tar
TAR_CORESERVICE_DLM				:=icap_coreservice_dlm.tar
TAR_CORESERVICE_IAM				:=icap_coreservice_innoagemanager.tar
TAR_WEBSERVICE_AUTHAPI			:=icap_webservice_authapi.tar
TAR_WEBSERVICE_DEVICEAPI		:=icap_webservice_deviceapi.tar
TAR_WEBSERVICE_DASHBOARDAPI		:=icap_webservice_dashboardapi.tar
TAR_WEBSERVICE_OOBSERVICE		:=icap_webservice_oobservice.tar
TAR_WEBSERVICE_WEBSITE			:=icap_webservice_website.tar
TAR_W3M_EULA					:=icap_eula.tar
TAR_DBCHECKER					:=icap_dbchecker.tar
TAR_MOCKDATA					:=icap_mockdata.tar
TAR_IMAGE_FILE					:=Images.tar.gz

FILE_DEVICE_COUNT				:=DeviceCount.json
FILE_INSTALL_SCRIPT_WIN			:=iCAP_Server_Installer.py
FILE_UNINSTALL_SCRIPT			:=iCAP_Server_Uninstall.sh
FILE_UNINSTALL_SCRIPT_WIN		:=iCAP_Server_Uninstall_WIN.py
FILE_PACKER_WINDOWS				:=iCAP_Server_Packer_Windows.bat
FILE_PACKER_WINDOWS_README		:=iCAP_Server_Packer_Windows_README.txt
FILE_SETTING_CONFIG				:=../config/innoAge/setting.json
FILE_SETTING_ENV				:=../config/setting.env
FILE_LINUX_PACKER				:=iCAP_Server_Packer_Linux.py

PATH_LOG_UPDATE					:=common/System/ServerInstaller/Log_Update.sh
PATH_LOG_UPDATE_WIN				:=common/System/ServerInstaller/Log_Update.cmd
PATH_DATADB						:=common/System/DataDB
PATH_ADMINDB					:=common/System/AdminDB
PATH_REDIS						:=common/System/RedisCache
PATH_CORESERVICE_CM				:=common/ClusterManagement
PATH_GATEWAY_AUTH				:=common/Gateway/AuthHandler
PATH_LIBLOGGER					:=../../../../library/liblogger
PATH_LIBCRYPTO					:=../../../../library/libcrypto/src

PATH_WEBSERVICE                 :=common/WebService
PATH_INNOAGESERVICE_BIN :=
PATH_WEBSERVICE_WEBSITE :=
PATH_WEBSERVICE_OOBSERVICE :=
PATH_INNOAGESERVICE_BIN			:=common/InnoAgeService/bin
PATH_WEBSERVICE_WEBSITE         :=$(PATH_WEBSERVICE)/Website
PATH_WEBSERVICE_OOBSERVICE      :=$(PATH_WEBSERVICE)/OOBService
PATH_LIBMONGO					:=common/CoreService/libCommon/libmongodb/src
PATH_LIBMQTT					:=common/CoreService/libCommon/libMQTTagent/src
PATH_LIBWEBCOMMAND				:=common/CoreService/libCommon/libWebcommand/src
PATH_CORESERVICE_DM				:=common/CoreService/DM
PATH_CORESERVICE_DATAHANDLER	:=common/CoreService/DataHandler
PATH_CORESERVICE_STORANALYZER	:=common/CoreService/StorageAnalyzer
PATH_CORESERVICE_NOTIFY			:=common/CoreService/NotificationService
PATH_CORESERVICE_DASHBOARDAGENT	:=common/CoreService/DashboardAgent
PATH_CORESERVICE_DLM			:=common/CoreService/DLM
PATH_CORESERVICE_IAM			:=common/CoreService/InnoAgeManager
PATH_WEBSERVICE_AUTHAPI			:=$(PATH_WEBSERVICE)/AuthenticationAPI
PATH_WEBSERVICE_DEVICEAPI		:=$(PATH_WEBSERVICE)/DeviceAPI
PATH_WEBSERVICE_DASHBOARDAPI	:=$(PATH_WEBSERVICE)/DashboardAPI
# PATH_WEBSERVICE_WEBSITE			:=$(PATH_WEBSERVICE)/Website
PATH_WEBSERVICE_DBCHECKER		:=$(PATH_WEBSERVICE)/DBChecker
PATH_WEBSERVICE_MOCKDATA		:=$(PATH_WEBSERVICE)/MockDataGenerator
# PATH_WEBSERVICE_OOBSERVICE		:=$(PATH_WEBSERVICE)/OOBService
PATH_W3M_EULA					:=common/System/EULA
PATH_CORESERVICE_SOURCE			:=common/CoreService/Dependencies
PATH_CLIENTSERVICE				:=common/$(ClientDir)/ClientService
PATH_RECOVERY					:=common/$(ClientDir)/Recovery
PATH_CLIENTSERVICE_SETTING		:=common/$(ClientDir)/SetupAgent
PATH_CLIENTSERVICE_INSTALLER	:=common/$(ClientDir)/Installer
PATH_CLIENTSERVICE_DEPENDENCIES	:=common/$(ClientDir)/Installer/Dependencies.sh
PATH_CLIENTSERVICE_API			:=common/$(ClientDir)/libiCAPClient
PATH_INSTALLATION_GUIDE			:=documents/Installation_Guide/InstallationGuide.pdf
PATH_RELEASE_NOTE				:=documents/ReleaseNotes
PATH_INSTALLER					:=common/System/ServerInstaller
PATH_UNINSTALL_SCRIPT			:=iCAP_Server_Uninstall.sh
PATH_INSTALL_SCRIPT				:=common/System/ServerInstaller/iCAP_Server_Installer.py
PATH_INSTALL_SCRIPT_WIN			:=common/System/ServerInstaller/iCAP_Server_Installer.py
PATH_UNINSTALL_SCRIPT_WIN		:=common/System/ServerInstaller/iCAP_Server_Uninstall_WIN.py
PATH_DOCKER_COMPOSE_WIN			:=common/System/ServerInstaller/Config
PATH_PACKER_WINDOWS				:=common/System/ServerInstaller/iCAP_Server_Packer_Windows.bat
PATH_PACKER_WINDOWS_README		:=common/System/ServerInstaller/iCAP_Server_Packer_Windows_README.txt
PATH_PACKER						:=common/System/ServerInstaller/$(FILE_LINUX_PACKER)
PATH_DEVICE_COUNT				:=../config/DeviceCount.json
PATH_INITDATA_PACKIMAGES :=common/System/InitData/Images

INTI_IMAGE_FILE	:=common/System/InitData/Images/Images.tar.gz

.PHONY: all
ifeq ($(OS),Windows_NT)
all: init doc
else
all: check clean init initbuild libcommon core innoage web sys #pack
endif

# .PHONY: cm
# cm:
# 	$(Q)echo "  Building iCAP Cluster manager images..."
# 	$(Q)echo "  Building iCAP Cluster manager production files..."
# 	$(Q)cd $(PATH_CORESERVICE_CM) && make MACROS=$(CLUSTER_MANAGER_MACRO)
# 	$(Q)echo "  Building iCAP Cluster manager image..."
# 	$(Q)cd $(PATH_CORESERVICE_CM) && docker build -t $(IMG_CLUSTER_MANAGER) .
# 	$(Q)echo "  Save iCAP Cluster Manager: Cluster Manager image..."
# 	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CLUSTER_MANAGER) $(IMG_CLUSTER_MANAGER)

.PHONY: core
core:
	$(Q)echo "  Building iCAP Core service images..."
	$(Q)echo "  Building iCAP Core Service: Device Management production files..."
	$(Q)cd $(PATH_CORESERVICE_DM) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Device Management image..."
	$(Q)cd $(PATH_CORESERVICE_DM) && docker build -t $(IMG_CORESERVICE_DM) .
	$(Q)echo "  Building iCAP Core Service: Data Handler production files..."
	$(Q)cd $(PATH_CORESERVICE_DATAHANDLER) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Data Handler image..."
	$(Q)cd $(PATH_CORESERVICE_DATAHANDLER) && docker build -t $(IMG_CORESERVICE_DATAHANDLER) .
	$(Q)echo "  Building iCAP Core Service: Storage Analyzer production files..."
	$(Q)cd $(PATH_CORESERVICE_STORANALYZER) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Storage Analyzer image..."
	$(Q)cd $(PATH_CORESERVICE_STORANALYZER) && docker build -t $(IMG_CORESERVICE_STORANALYZER) .
	$(Q)echo "  Building iCAP Core Service: Notification Service production files..."
	$(Q)cd $(PATH_CORESERVICE_NOTIFY) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Notification Service image..."
	$(Q)cd $(PATH_CORESERVICE_NOTIFY) && docker build -t $(IMG_CORESERVICE_NOTIFY) .
	$(Q)echo "  Building iCAP Core Service: Dashboard Agent production files..."
	$(Q)cd $(PATH_CORESERVICE_DASHBOARDAGENT) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Dashboard Agent image..."
	$(Q)cd $(PATH_CORESERVICE_DASHBOARDAGENT) && docker build -t $(IMG_CORESERVICE_DASHBOARDAGENT) .
	$(Q)echo "  Building iCAP Core Service: Data Lifecycle Manager production files..."
	$(Q)cd $(PATH_CORESERVICE_DLM) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: Data Lifecycle Manager image..."
	$(Q)cd $(PATH_CORESERVICE_DLM) && docker build -t $(IMG_CORESERVICE_DLM) .
	$(Q)echo "  Building iCAP Core Service: innoAge Manager production files..."
	$(Q)cd $(PATH_CORESERVICE_IAM) && make clean && make MACROS=$(CORE_SERVICE_MACRO)
	$(Q)echo "  Building iCAP Core Service: innoAge Manager image..."
	$(Q)cd $(PATH_CORESERVICE_IAM) && docker build -t $(IMG_CORESERVICE_IAM) .
	$(Q)echo "  Save iCAP Core Service: Device Management image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_DM) $(IMG_CORESERVICE_DM)
	$(Q)echo "  Save iCAP Core Service: Data Handler image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_DATAHANDLER) $(IMG_CORESERVICE_DATAHANDLER)
	$(Q)echo "  Save iCAP Core Service: Storage Analyzer image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_STORANALYZER) $(IMG_CORESERVICE_STORANALYZER)
	$(Q)echo "  Save iCAP Core Service: Notification Service image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_NOTIFY) $(IMG_CORESERVICE_NOTIFY)
	$(Q)echo "  Save iCAP Core Service: Dashboard Agent image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_DASHBOARDAGENT) $(IMG_CORESERVICE_DASHBOARDAGENT)
	$(Q)echo "  Save iCAP Core Service: Data Lifecycle Manager image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_DLM) $(IMG_CORESERVICE_DLM)
	$(Q)echo "  Save iCAP Core Service: innoAge Manager image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_CORESERVICE_IAM) $(IMG_CORESERVICE_IAM)
	
.PYTHON: innoage
innoage:
	$(Q)echo "  Copy innoAge service..."
	$(Q)cp $(PATH_INNOAGESERVICE_BIN)/* $(OUTPUT_PATH)/iCAP_Server_Release

.PHONY: web
web:
	$(Q)echo "  Building iCAP Web service images..."
	$(Q)echo "  Building iCAP Web-service: AuthenticationAPI production files..."
	$(Q)cd $(PATH_WEBSERVICE_AUTHAPI) && dotnet restore && dotnet publish --configuration=$(WEBAPI_MACRO)
	$(Q)echo "  Building iCAP Web-service: AuthenticationAPI image..."
	$(Q)cd $(PATH_WEBSERVICE_AUTHAPI) && docker build -t $(IMG_WEBSERVICE_AUTHAPI) .
	$(Q)echo "  Building iCAP Web-service: DeviceAPI production files..."
	$(Q)cd $(PATH_WEBSERVICE_DEVICEAPI) && dotnet restore && dotnet publish --configuration=$(WEBAPI_MACRO)
	$(Q)echo "  Building iCAP Web-service: DeviceAPI image..."
	$(Q)cd $(PATH_WEBSERVICE_DEVICEAPI) && docker build -t $(IMG_WEBSERVICE_DEVICEAPI) .
	$(Q)echo "  Building iCAP Web-service: DashboardAPI production files..."
	$(Q)cd $(PATH_WEBSERVICE_DASHBOARDAPI) && dotnet restore && dotnet publish --configuration=$(WEBAPI_MACRO)
	$(Q)echo "  Building iCAP Web-service: DashboardAPI image..."
	$(Q)cd $(PATH_WEBSERVICE_DASHBOARDAPI) && docker build -t $(IMG_WEBSERVICE_DASHBOARDAPI) .
ifeq ("$(VERSION)", "DFI")
	$(Q)echo "  Building iCAP DFI_Release DB Checker production files..."
	$(Q)cd $(PATH_WEBSERVICE_DBCHECKER) && dotnet restore && dotnet publish --configuration=DFI_Release
	$(Q)echo "  Building iCAP DB Checker image..."
	$(Q)cd $(PATH_WEBSERVICE_DBCHECKER) && docker build -t $(IMG_WEBSERVICE_DBCHECKER) .
else
	$(Q)echo "  Building iCAP DB Checker production files..."
	$(Q)cd $(PATH_WEBSERVICE_DBCHECKER) && dotnet restore && dotnet publish --configuration=Release
	$(Q)echo "  Building iCAP DB Checker image..."
	$(Q)cd $(PATH_WEBSERVICE_DBCHECKER) && docker build -t $(IMG_WEBSERVICE_DBCHECKER) .
endif
	$(Q)echo "  Building iCAP Mock data generator production files..."
	$(Q)cd $(PATH_WEBSERVICE_MOCKDATA) && dotnet restore && dotnet publish --configuration=Release
	$(Q)echo "  Building iCAP Web-service: OOBService image..."
	$(Q)cd $(PATH_WEBSERVICE_OOBSERVICE) && docker build -f Dockerfile -t $(IMG_WEBSERVICE_OOBSERVICE) .

ifeq ($(WEB_SERVICE_MACRO), MOCKDATA)
	$(Q)echo "  Building iCAP Mock data generator image..."
	$(Q)cd $(PATH_WEBSERVICE_MOCKDATA) && docker build -t $(IMG_WEBSERVICE_MOCKDATA) .
endif
	$(Q)echo "  Building iCAP Web-service: Website production files..."
	$(Q)cd $(PATH_WEBSERVICE_WEBSITE) && npm install && npm run build
	$(Q)echo "  Building iCAP Web-service: Website image..."
	$(Q)cd $(PATH_WEBSERVICE_WEBSITE) && docker build -t $(IMG_WEBSERVICE_WEBSITE) .
	$(Q)echo "  Save iCAP Web-service: AuthenticationAPI image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_WEBSERVICE_AUTHAPI) $(IMG_WEBSERVICE_AUTHAPI)
	$(Q)echo "  Save iCAP Web-service: DashboardAPI image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_WEBSERVICE_DASHBOARDAPI) $(IMG_WEBSERVICE_DASHBOARDAPI)
	$(Q)echo "  Save iCAP Web-service: DeviceAPI image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_WEBSERVICE_DEVICEAPI) $(IMG_WEBSERVICE_DEVICEAPI)
	$(Q)echo "  Save iCAP DB Checker image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_DBCHECKER) $(IMG_WEBSERVICE_DBCHECKER)
ifeq ($(WEB_SERVICE_MACRO), MOCKDATA)
	$(Q)echo "  Save iCAP Mock data generator image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_MOCKDATA) $(IMG_WEBSERVICE_MOCKDATA)
	$(Q)echo "  Copy DeviceCount.json..."
	$(Q)cp $(PATH_DEVICE_COUNT) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_DEVICE_COUNT)
endif
	$(Q)echo "  Save iCAP Web-service: Website image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_WEBSERVICE_WEBSITE) $(IMG_WEBSERVICE_WEBSITE)
	$(Q)echo "  Save iCAP Web-service: OOBService image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_WEBSERVICE_OOBSERVICE) $(IMG_WEBSERVICE_OOBSERVICE)

.PHONY: sys
sys:
	$(Q)echo "  Building iCAP System images..."
	$(Q)echo "  Building iCAP Gateway: Authentication Handler production files..."
	$(Q)cd $(PATH_GATEWAY_AUTH) && make clean && make
	$(Q)echo "  Building iCAP Gateway image..."
	$(Q)cd $(PATH_GATEWAY_AUTH) && docker build -t $(IMG_GATEWAY_AUTH) .
	$(Q)echo "  Building iCAP Data DB image..."
	$(Q)cd $(PATH_DATADB) && docker build -t $(IMG_DATADB) .
	$(Q)echo "  Building iCAP Admin DB image..."
	$(Q)cd $(PATH_ADMINDB) && docker build -t $(IMG_ADMINDB) .
	$(Q)echo "  Building iCAP Redis DB image..."
	$(Q)docker pull redis:alpine3.12
	$(Q)docker image tag redis:alpine3.12 $(IMG_REDIS)
	$(Q)echo "  Building iCAP EULA image..."
	$(Q)cd $(PATH_W3M_EULA) && docker build -t $(IMG_W3M_EULA) .
	$(Q)echo "  Save iCAP Gateway image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_GATEWAY_AUTH) $(IMG_GATEWAY_AUTH)
	$(Q)echo "  Save iCAP Data DB image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_DATADB) $(IMG_DATADB)
	$(Q)echo "  Save iCAP Admin DB image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_ADMINDB) $(IMG_ADMINDB)
	$(Q)echo "  Save iCAP Redis Cache image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_REDIS) $(IMG_REDIS)
	$(Q)echo "  Save iCAP EULA image..."
	$(Q)cd $(OUTPUT_PATH)/iCAP_Server_Release && docker save -o $(TAR_W3M_EULA) $(IMG_W3M_EULA)
	$(Q)echo "  Copy images..."
	$(Q)cd $(PATH_INITDATA_PACKIMAGES) && bash ./pack.sh $(DEVICE_IMG_VERSION)
	$(Q)cp $(INTI_IMAGE_FILE) $(OUTPUT_PATH)/iCAP_Server_Release/$(TAR_IMAGE_FILE)
	$(Q)echo "  Copy setting files & script..."
	$(Q)cp $(FILE_SETTING_CONFIG) $(OUTPUT_PATH)/iCAP_Server_Release
	$(Q)cp $(FILE_SETTING_ENV) $(OUTPUT_PATH)/iCAP_Server_Release
	$(Q)echo "  Build server installer..."
	$(Q)cp $(PATH_INSTALLER)/eula.md $(OUTPUT_PATH)/iCAP_Server_Release
	$(Q)cp $(PATH_INSTALL_SCRIPT_WIN) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_INSTALL_SCRIPT_WIN)
	$(Q)cp $(PATH_INSTALLER)/$(PATH_UNINSTALL_SCRIPT) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_UNINSTALL_SCRIPT)
	$(Q)chmod a+x $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_UNINSTALL_SCRIPT)
	$(Q)cp $(PATH_UNINSTALL_SCRIPT_WIN) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_UNINSTALL_SCRIPT_WIN)
	$(Q)cp $(PATH_PACKER_WINDOWS) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_PACKER_WINDOWS)
	$(Q)cp $(PATH_PACKER_WINDOWS_README) $(OUTPUT_PATH)/iCAP_Server_Release/$(FILE_PACKER_WINDOWS_README)
	$(Q)cp -r $(PATH_DOCKER_COMPOSE_WIN) $(OUTPUT_PATH)/iCAP_Server_Release
	$(Q)cp $(PATH_LOG_UPDATE) $(OUTPUT_PATH)/iCAP_Server_Release/
	$(Q)cp $(PATH_LOG_UPDATE_WIN) $(OUTPUT_PATH)/iCAP_Server_Release/
	
.PHONY: installer
installer:
	$(Q)echo "Copy config file to bin folder"
	$(Q)cp $(SETTING_FILE) $(OUTPUT_PATH)/iCAP_Server_Release
	$(Q)echo "Building iCAP Server_Installer..."
	$(Q)cd $(PATH_INSTALLER) && pyinstaller --onefile iCAP_Server_Installer.py --hiddenimport docker
	$(Q)cp $(PATH_INSTALLER)/dist/iCAP_Server_Installer $(OUTPUT_PATH)/iCAP_Server_Release

PHONY: libcommon
libcommon:
	$(Q)echo "  Building iCAP library..."
	$(Q)echo "  Building liblogger..."
	$(Q)cd $(PATH_LIBLOGGER) && make MACROS=$(LOGGER_MACRO)
	$(Q)echo "  Building libcrypto..."
	$(Q)cd $(PATH_LIBCRYPTO) && make MACROS=$(CRYPTO_MACRO)
	$(Q)echo "  Building libmongodb..."
	$(Q)cd $(PATH_LIBMONGO) && make	
	$(Q)echo "  Building libMQTT..."
	$(Q)cd $(PATH_LIBMQTT) && make MACROS=$(MQTT_MACRO)
	$(Q)echo "  Building libWebcommand..."
	$(Q)cd $(PATH_LIBWEBCOMMAND) && make

.PHONY: doc
doc:
	$(Q)echo "  Building documents..."
	$(Q)mkdir -p $(OUTPUT_PATH)
	$(Q)cp $(PATH_INSTALLATION_GUIDE) $(OUTPUT_PATH)/
	$(Q)cp $(PATH_RELEASE_NOTE)/* $(OUTPUT_PATH)/
	$(Q)echo "  DONE"

.PHONY: pack
pack:
	$(Q)echo "  Compress iCAP Server release images..."
	$(Q)cd $(OUTPUT_PATH) && tar cf - iCAP_Server_Release | pigz > $(RELEASE_SERVER_TAR)
	$(Q)cp $(PATH_PACKER) $(OUTPUT_PATH)
	$(Q)chmod a+x $(OUTPUT_PATH)/$(FILE_LINUX_PACKER)

.PHONY: check
check:
ifeq ($(wildcard $(SETTING_FILE)),)
	$(Q)echo "Build project not setting, please run python3 build_setup.py first."
	$(Q)exit 1
endif

.PHONY: clean
clean:
	$(Q)echo "  Clean previous build files..."
	$(Q)$(RM) -r $(OUTPUT_PATH)
	
.PHONY: init
init:
	$(Q)mkdir -p $(OUTPUT_PATH)
	$(Q)mkdir -p $(OUTPUT_PATH)/iCAP_Server_Release

.PHONY: initbuild
initbuild:
	$(Q)echo "  Build initialize images for all images..."
	$(Q)echo "  Build Core Service source image..."
	$(Q)cd $(PATH_CORESERVICE_SOURCE) && docker build -t $(IMG_CORESERVICE_SOURCE) .
