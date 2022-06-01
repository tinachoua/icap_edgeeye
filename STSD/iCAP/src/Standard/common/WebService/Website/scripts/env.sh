#!/usr/bin/env bash

# setup version
export VERSION=$1
export OOB=$2
export SSL=$3

if [ -z "$VERSION" ] || [ "$VERSION" == " " ]; then
  VERSION=STD
fi