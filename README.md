# blue-assistant

Project website: https://students.cs.ucl.ac.uk/2020/group24/

To get the system ready, two separate set-ups need to be done.

## Skill server setup

1. Create a debian cloud server.
2. Copy API-code into the server. e.g.:
```sh
scp -r API-code skill.comp0016.mww.moe:/home/mao/API-code
```
3. Run `./configure.sh DOMAIN_NAME` on the server with `DOMAIN_NAME` replaced with the domain name that you want to use. e.g.:
```sh
cd API-code/
./configure.sh skill.comp0016.mww.moe
```
This will take a while as it will install certbot, get HTTPS certificate from Let's Encrypt, and install Docker and build the container which will run the skill server.

After the setup finishes, the skill server can be started by running `./start.sh`. To keep the certificate renewed, just run `./configure.sh DOMAIN_NAME` every 3 months.

## Client setup

// TODO
