#!/bin/bash

LoadImage()
{
	echo -n "  Loading iCAP "$1" image..."
	docker load --input $2 > /dev/null 2> /dev/null
	local rc=$?
	if [ $rc -ne "0" ]; then
		printf "\E[0;31m"
		echo "[Fail]"
		printf "\E[0;33m"
		echo "  Please check the image file "$2" was exists."
		printf "\E[0m"
		exit 1
	else
		printf "\E[0;32m"
		echo "[Success]"
		printf "\E[0m"
	fi
}

PrintStartMsg()
{
	echo -n "  Mount container "$1" from image "$2"..."
}

PrintResult()
{
	if [ $1 -ne "0" ]; then
		printf "\E[0;31m"
		echo "[Fail]"
	else
		printf "\E[0;32m"
		echo "[Success]"
	fi
	printf "\E[0m"
}

RemoveContainer()
{
	echo "  Remove container "$1"..."
	docker stop $1 > /dev/null 2> /dev/null
	docker rm $1 > /dev/null 2> /dev/null
}

echo "===Start iCAP Server installation script==="
echo -n "  Checkign premission..."
if [ "$(id -u)" != "0" ]; then
	printf "\E[0;31m"
	echo "[Fail]"
	printf "\E[0;33m"
	echo "  Please run iCAP server installer with root premission" 1>&2
	echo "  Usage: sudo ./iCAP_Server_Installer.sh"
	printf "\E[0m"
	exit 1
fi

printf "\E[0;32m"
echo "[Success]"
printf "\E[0m"

echo -n "  Checking docker server..."
docker -v > /dev/null 2> /dev/null
rc=$?
if [ $rc -ne "0" ]; then
	printf "\E[0;31m"
	echo "[Fail]"
	printf "\E[0;33m"
	echo "  Please install docker server before you install iCAP server" 1>&2
	echo "  Docker installation reference: https://docs.docker.com/engine/installation/"
	printf "\E[0m"
	exit 1
fi

printf "\E[0;32m"
echo "[Success]"
printf "\E[0m"

LoadImage "EULA" icap_eula.tar
echo "iCAP Service has an EULA that needs you consent."
echo "if you use it, you grant consent as well."
echo "Press any key to print EULA..."
read input
docker run -it --name icap_eula icap_eula:v1.0.0.0
echo "--------------------Innodisk iCAP EULA--------------------"
echo "----------------------------------------------------------"
until [ "${input}" == "yes" ]
do
	echo "Do you accept the EULA?(yes/no)"
	read input
	docker stop icap_eula > /dev/null 2> /dev/null
	docker rm icap_eula > /dev/null 2> /dev/null
	if [ "${input}" == "y" ]; then
		echo "Keyin 'yes' to accept the EULA."
	elif [ "${input}" != "yes" ]; then
		echo "You disagree the EULA, exit installation script."
		exit 0
	fi
done

LoadImage "Data DB" icap_datadb.tar
LoadImage "Admin DB" icap_admindb.tar
LoadImage "Redis cache" icap_redis.tar
LoadImage "Gateway" icap_gateway.tar
LoadImage "Cluster Manager" icap_cluster_manager.tar
LoadImage "Web-service : Authentication API" icap_webservice_authapi.tar
LoadImage "Web-service : Device API" icap_webservice_deviceapi.tar
LoadImage "Web-service : Dashboard API" icap_webservice_dashboardapi.tar
LoadImage "Web-service : Website" icap_webservice_website.tar
LoadImage "Core service : Device Management" icap_coreservice_dm.tar
LoadImage "Core service : Data handler" icap_coreservice_datahandler.tar
LoadImage "Core service : Storage analyzer" icap_coreservice_storanalyzer.tar
#LoadImage "Core service : Dashboard agent" icap_coreservice_dashboardagent.tar
LoadImage "Core service : Notification service" icap_coreservice_notify.tar
echo "  Starting containers..."
docker network inspect icap_net > /dev/null 2> /dev/null
rc=$?
if [ $rc -ne "0" ]; then
	docker network create --driver bridge --subnet 172.30.0.0/16 icap_net
	mkdir -p /var/iCAP/
	cp ServiceSetting.json /var/iCAP/
	mkdir -p /var/iCAP/AdminDB
	mkdir -p /var/iCAP/DataDB
	mkdir -p /var/iCAP/images
	tar -C /var/iCAP/AdminDB -zxvf AdminDB.tar.gz
	tar -C /var/iCAP/DataDB -zxvf DataDB.tar.gz
	tar -C /var/iCAP/images -zxvf Images.tar.gz
else
	printf "\E[0;33m"
	echo "  Seems the iCAP server was installed, do you want to re-install?(y/n)"
	printf "\E[0m"
	read input
	if [ "${input}" == "y" -o "${input}" == "yes" ]; then
		echo "  Try to remove iCAP server continers..."
		RemoveContainer core_dm
		RemoveContainer core_datahandler
		RemoveContainer core_storanalyzer
		RemoveContainer core_notifyservice
#		RemoveContainer core_dashboard
		RemoveContainer dashboardapi
		RemoveContainer deviceapi
		RemoveContainer authapi
		RemoveContainer icap_cluster_manager
		RemoveContainer gateway
		RemoveContainer redis
		RemoveContainer adminDB
		RemoveContainer dataDB
		RemoveContainer website
	else
		exit 0
	fi
fi



PrintStartMsg "dataDB" "icap_datadb"
docker run -idt --network=icap_net --ip 172.30.0.2 --restart always --name dataDB -v /var/iCAP/DataDB:/data/db icap_datadb:v1.0.0.0 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "adminDB" "icap_admindb"
docker run -idt --network=icap_net --ip 172.30.0.3 --restart always --name adminDB -v /var/iCAP/AdminDB:/var/lib/mysql -e MYSQL_ROOT_PASSWORD=admin icap_admindb:v1.0.0.0 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "redis" "icap_redis"
docker run -idt --network=icap_net --ip 172.30.0.5 --restart always --name redis icap_redis:v1.0.0.0 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "gateway" "icap_gateway"
docker run -idt -p 1883:1883 --network=icap_net --ip 172.30.0.4 --restart always --name gateway -w=/root icap_gateway:v1.0.0.0 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "clustermanager" "icap_cluster_manager"
docker run -idt --network=icap_net --ip 172.30.0.15 --restart always --privileged --name icap_cluster_manager -v /var/run/docker.sock:/var/run/docker.sock -w=/root icap_cluster_manager:v1.0.0.0 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "authapi" "icap_webservice_authapi"
docker run -idt --network=icap_net --ip 172.30.0.6 --restart always --name authapi -w=/root icap_webservice_authapi:v1.0.0.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "deviceapi" "icap_webservice_deviceapi"
docker run -idt --network=icap_net --ip 172.30.0.7 --restart always --name deviceapi -w=/root -v /var/iCAP/images:/var/images icap_webservice_deviceapi:v1.0.1.3 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "dashboardapi" "icap_webservice_dashboardapi"
docker run -idt --network=icap_net --ip 172.30.0.8 --restart always --name dashboardapi -w=/root icap_webservice_dashboardapi:v1.0.0.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

sleep 3

PrintStartMsg "core_dm" "icap_coreservice_dm"
docker run -idt --network=icap_net --ip 172.30.0.10 --restart always --name core_dm -v /var/iCAP:/var/iCAP -w=/root icap_coreservice_dm:v1.0.1.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "core_datahandler" "icap_coreservice_datahandler"
docker run -idt --network=icap_net --ip 172.30.0.11 --restart always --name core_datahandler -v /var/iCAP:/var/iCAP -w=/root icap_coreservice_datahandler:v1.0.1.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "core_storanalyzer" "icap_coreservice_storanalyzer"
docker run -idt --network=icap_net --ip 172.30.0.12 --restart always --name core_storanalyzer -v /var/iCAP:/var/iCAP -w=/root icap_coreservice_storanalyzer:v1.0.1.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

PrintStartMsg "core_notifyservice" "icap_coreservice_notify"
docker run -idt --network=icap_net --ip 172.30.0.13 --name core_notifyservice -v /var/iCAP:/var/iCAP -w=/root icap_coreservice_notify:v1.0.1.1 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

#PrintStartMsg "core_dashboard" "icap_coreservice_dashboard"
#docker run -idt --network=icap_net --ip 172.30.0.14 --name core_dashboard -v /var/iCAP:/var/iCAP -w=/root icap_coreservice_dashboardagent:v1.0.0.0 > /dev/null 2> /dev/null
#rc=$?
#PrintResult $rc

PrintStartMsg "website" "icap_webservice_website"
docker run -idt --network=icap_net --ip 172.30.0.14 --restart always -p 80:80 --name website icap_webservice_website:v1.0.3.8 > /dev/null 2> /dev/null
rc=$?
PrintResult $rc

