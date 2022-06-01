#!/bin/bash
# innoAGE web service image manual start tools.
# Description: This tools will create virtual network device and container to start innoAGE web service docker images.
# Author: IPA -Jacky
# Contact: jacky_sung@innodisk.com
# Copyright: (c) 2020 - innodisk Corporation.

echo "Create virtual network devices...\r"

if [ ! "$(docker network ls | grep innoage_webservice_net)" ]; then
    docker network create --driver=bridge --subnet=172.30.0.0/24 innoage_webservice_net
fi

echo "Import images to container and start innoAGE web service..."
docker image inspect innoage-webservice:v5 >/dev/null 2>&1 && docker run -idt -p 1883:1883 --network=innoage_webservice_net --ip 172.30.0.101 --restart always --name innoAGE-Gateway -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 eclipse-mosquitto:innoage || echo ""
docker image inspect eclipse-mosquitto:innoage >/dev/null 2>&1 && docker run -idt -p 8161:8161 --network=innoage_webservice_net --ip 172.30.0.100 --restart always --name innoAGE-WebService -e MQTT_USER=innoage -e MQTT_PASSWORD=B673AEBC6D65E7F42CFABFC7E01C02D0 -e MQTT_HOST=mqtt://172.30.0.101:1883 -e MQTT_KEEPALIVE=120 innoage-webservice:v5 || echo ""