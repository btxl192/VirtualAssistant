#!/bin/bash

HOST=$1
if [ -z "$HOST" ]; then
  echo You need to provide the domain name as an argument.
  exit 1
fi

set -e
echo This script assumes that you are running on debian or ubuntu.
echo I will now install docker
curl -fsSL https://get.docker.com | sudo bash -
echo I will now build the target container
sudo docker build . -t alexaskill
echo I will now install certbot with apt.
sudo apt update -y
sudo apt install python3 python3-pip certbot -y
echo I will now ask certbot for a certificate.
CERTBOT_DIR=/tmp/certbot
sudo mkdir $CERTBOT_DIR || true
sudo chown 0:0 -R $CERTBOT_DIR
sudo certbot certonly --config-dir $CERTBOT_DIR --work-dir $CERTBOT_DIR --logs-dir $CERTBOT_DIR --agree-tos -n -m "comp0016-acme-account@maowtm.org" --standalone -d "$HOST"
CPATH="$CERTBOT_DIR/live/$HOST"
sudo cp "$CPATH/fullchain.pem" "/opt/ssl.crt"
sudo cp "$CPATH/privkey.pem" "/opt/ssl.key"
echo Certificates has been saved into /opt/ssl."{crt,key}"
echo "$HOST" > setup.finished
