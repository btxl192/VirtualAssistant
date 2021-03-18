#!/bin/bash

HOST=$1
if [ -z "$HOST" ]; then
  echo You need to provide the domain name as an argument.
  exit 1
fi

set -e
cd "$(dirname "$0")"
echo This script assumes that you are running on debian or ubuntu.
echo I will now install docker
curl -fsSL https://get.docker.com | sudo bash -
echo I will now build the target container
sudo docker build . -t alexaskill
echo I will now install certbot with apt.
sudo apt update -y
sudo apt install python3 python3-pip certbot -y
SERVICE_NAME=skill-backend.service
cat > $SERVICE_NAME <<EOF
[Unit]
Description=Skill server
After=network.target

[Install]
WantedBy=network.target

[Service]
Type=simple
User=0
Group=0
EOF
echo ExecStart=$(pwd)/start.sh >> $SERVICE_NAME
sudo cp $SERVICE_NAME /etc/systemd/system/
sudo systemctl daemon-reload
echo "$HOST" > setup.finished
sudo systemctl enable $SERVICE_NAME
sudo systemctl start $SERVICE_NAME
echo "Configuration completed!"
echo "Skill server has been started and will automatically be started at boot."
echo "Use \"systemctl status $SERVICE_NAME to see more details.\""
echo "Now, just set your skill domain in the unity client config to $HOST and skill endpoint to https://$HOST/api/v1/blueassistant"
