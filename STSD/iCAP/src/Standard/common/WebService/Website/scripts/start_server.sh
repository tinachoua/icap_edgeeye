#!/usr/bin/env bash
source .env
source VERSION
# setup version
echo "running $CONFIG_PATH"

webpack-dev-server --progress --colors --config $PWD/config/webpack.config.dev.js