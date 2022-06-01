#!/bin/bash
YELLOW='\033[1;33m'
GREEN='\033[1;32m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${YELLOW}Start to uninstall iCAP Server${NC}";
echo -e "${RED}It will remove all the iCAP Server dependencies data, including database";
echo -e "${YELLOW}Please make sure you want to remove all the data.${NC}";
echo -e "${YELLOW}Do you want to remove iCAP Server?(y/n)${NC}"
read input
if [ "${input}" == "y" ]; then
	echo -e "${GREEN}  Remove all containers...${NC}"
	docker rm -f website > /dev/null 2> /dev/null
	docker rm -f core_innoagemanager > /dev/null 2> /dev/null
	docker rm -f core_dlm > /dev/null 2> /dev/null
	docker rm -f core_dashboardagent > /dev/null 2> /dev/null
	docker rm -f core_notifyservice > /dev/null 2> /dev/null
	docker rm -f core_storanalyzer > /dev/null 2> /dev/null
	docker rm -f core_datahandler > /dev/null 2> /dev/null
	docker rm -f core_dm > /dev/null 2> /dev/null
	docker rm -f innoage_webservice > /dev/null 2> /dev/null
	docker rm -f innoage_gateway > /dev/null 2> /dev/null
	docker rm -f dashboardapi > /dev/null 2> /dev/null
	docker rm -f deviceapi > /dev/null 2> /dev/null
	docker rm -f authapi > /dev/null 2> /dev/null
	docker rm -f webservice_oobservice > /dev/null 2> /dev/null
	docker rm -f innoAGE-WebService > /dev/null 2> /dev/null
	docker rm -f innoAGE-Gateway > /dev/null 2> /dev/null
	docker rm -f gateway > /dev/null 2> /dev/null
	docker rm -f redis > /dev/null 2> /dev/null
	docker rm -f adminDB > /dev/null 2> /dev/null
	docker rm -f dataDB > /dev/null 2> /dev/null
	# docker rm -f icap_cluster_manager > /dev/null 2> /dev/null
	echo -e "${GREEN}  Remove iCAP network...${NC}"
	docker network rm icap_net > /dev/null 2> /dev/null
	echo -e "${GREEN}  Remove iCAP Databases...${NC}"
	rm -r /var/iCAP/AdminDB > /dev/null 2> /dev/null
	rm -r /var/iCAP/DataDB > /dev/null 2> /dev/null
	systemctl stop mongodb > /dev/null 2> /dev/null
	echo -e "${GREEN}  Remove iCAP success.${NC}"
fi