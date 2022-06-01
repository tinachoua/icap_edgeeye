#!/usr/bin/env bash

CONFIG_PATH=./config/current_config.json
VERSION_PATH=./config/version.json

echo "Parsing current_config.json"
json5 -c $CONFIG_PATH
VERSION=$(jq .VERSION_NUMBER $VERSION_PATH)
SSL=$(jq .SSL $CONFIG_PATH)
OOB=$(jq .OOB $CONFIG_PATH)
echo -e "bundle...\nVERSION=$VERSION\nOOB=$OOB\nSSL=$SSL"

if [ "$SSL" == true ]; then
  cp ./https/Dockerfile Dockerfile
  cp ./https/nginx.conf nginx.conf
  cp ./https/innodisk.com.crt innodisk.com.crt
  cp ./https/innodisk.com.key innodisk.com.key
else
  cp ./http/Dockerfile Dockerfile
  cp ./http/nginx.conf nginx.conf
  rm innodisk.com.crt 2> /dev/null
  rm innodisk.com.key 2> /dev/null
fi

webpack --config $PWD/config/webpack.config.prod.js