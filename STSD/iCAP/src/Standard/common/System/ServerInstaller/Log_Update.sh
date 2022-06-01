#!/bin/bash
YELLOW='\033[1;33m'
GREEN='\033[1;32m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${YELLOW}Start to update the setting of logs for core-service${NC}";
echo -e "${YELLOW}Do you want to update the setting of logs?(y/n)${NC}"
read input
if [ "${input}" == "y" ]; then
	echo -e "${GREEN}  Start to update...${NC}"
    docker restart core_dm > /dev/null 2> /dev/null
    docker restart core_datahandler > /dev/null 2> /dev/null
    docker restart core_storanalyzer > /dev/null 2> /dev/null
    docker restart core_notifyservice > /dev/null 2> /dev/null
    docker restart core_dashboardagent > /dev/null 2> /dev/null
    docker restart core_dlm > /dev/null 2> /dev/null
    docker restart core_innoagemanager > /dev/null 2> /dev/null
	
	echo -e "${GREEN}  Update the setting success.${NC}"
fi