#!/bin/sh

set -e
touch /etc/mosquitto/passwd
nohup mosquitto -c /etc/mosquitto/mosquitto.conf &
output=0
until [ $output -ne "0" ]
do
	output=$(ps -ef | grep -v grep| grep mosquitto | wc -l)
	echo $output
done

exec "/root/iCAP_AuthHandler"
