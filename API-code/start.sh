#!/bin/bash

if [ ! -f setup.finished ]; then
  echo Please run ./configure first.
  exit 1
fi

HOST=$(cat setup.finished)
echo Domain is $HOST
echo Building image again just in case something changed.
sudo docker build . -t alexaskill
docker run -it -p 443:443 -e USE_SSL=1 -e SSL_CERT=/opt/ssl.crt -e SSL_KEY=/opt/ssl.key -v /opt/ssl.crt:/opt/ssl.crt -v /opt/ssl.key:/opt/ssl.key alexaskill
