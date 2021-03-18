#!/bin/bash

cd "$(dirname "$0")"

if [ ! -f setup.finished ]; then
  echo Please run ./configure first.
  exit 1
fi

HOST=$(cat setup.finished)
set -e
echo Domain is $HOST
echo Asking certbot to check if certificates needs to be renewed.
CERTBOT_DIR=$(pwd)/certbot-dir
sudo mkdir $CERTBOT_DIR || true
sudo chown 0:0 -R $CERTBOT_DIR
sudo certbot certonly --config-dir $CERTBOT_DIR --work-dir $CERTBOT_DIR --logs-dir $CERTBOT_DIR --agree-tos -n -m "comp0016-acme-account@maowtm.org" --standalone -d "$HOST"
CPATH="$CERTBOT_DIR/live/$HOST"
sudo cp "$CPATH/fullchain.pem" "/opt/ssl.crt"
sudo cp "$CPATH/privkey.pem" "/opt/ssl.key"
echo Certificates has been saved into /opt/ssl."{crt,key}"
echo Building image again just in case something changed.
sudo docker build . -t alexaskill
sudo chown 1000:1000 /opt/ssl.{key,crt}
sudo docker run --init --rm -u 1000:1000 -p 443:443 -e USE_SSL=1 -e SSL_CERT=/opt/ssl.crt -e SSL_KEY=/opt/ssl.key -v /opt/ssl.crt:/opt/ssl.crt -v /opt/ssl.key:/opt/ssl.key alexaskill
