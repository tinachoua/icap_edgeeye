#!/bin/bash

docker build -t test .
docker rm -f webservice_oobservice
sudo docker run -idt --network=icap_net --ip 172.30.0.22 --restart always --name webservice_oobservice -v /etc/localtime:/etc/localtime:ro test prodMod
