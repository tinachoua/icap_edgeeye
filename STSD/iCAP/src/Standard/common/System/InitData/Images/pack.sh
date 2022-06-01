#!/usr/bin/env bash

VERSION=$1

if [ -z "$VERSION" ] || [ "$VERSION" == " " ]; then
  VERSION=STD
fi

echo "Packing $VERSION images"

rm -r Images 2> /dev/null
rm Images.tar.gz 2> /dev/null

mkdir -p Images/devices
mkdir -p Images/assets/images/devices
mkdir -p Images/maps

cp ./devices/$VERSION/* ./Images/assets/images/devices
cp ./devices/00.png ./Images/devices

tar cf - Images | pigz > Images.tar.gz