#!/bin/bash
docker rm -f core_dm > /dev/null 2> /dev/null
make
docker build -t icap_coreservice_dm:v1.6.1 .
docker run -idt --network=icap_net --ip 172.30.0.10 --restart always --name core_dm -v /var/iCAP:/var/iCAP -v /etc/localtime:/etc/localtime:ro -w=/root icap_coreservice_dm:v1.6.1