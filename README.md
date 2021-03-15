# blue-assistant

Project website: https://students.cs.ucl.ac.uk/2020/group24/

To get the system ready, two separate set-ups need to be done.

## Skill server setup

1. Create a debian cloud server.
2. Copy API-code into the server. e.g.:
```sh
scp -r API-code skill.comp0016.mww.moe:/home/azureuser/API-code
```
3. Run `./configure.sh DOMAIN_NAME` on the server with `DOMAIN_NAME` replaced with the domain name that you want to use. e.g.:
```sh
cd API-code/
./configure.sh skill.comp0016.mww.moe
```
This will take a while as it will install certbot, get HTTPS certificate from Let's Encrypt, and install Docker and build the container which will run the skill server.

After the setup finishes, the skill server can be started by running `./start.sh`. To keep the certificate renewed, just run `./configure.sh DOMAIN_NAME` every 3 months.

## Setting up AVS client with pre-packaged zip

You can [download a zip of the compiled AVS client](https://mw-public-data.s3.eu-west-2.amazonaws.com/upload-space/e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855/avs.zip). It contains only compiled files but they are already patched for use by the assistant.

You would still need to install [Python 3](https://www.python.org/downloads/windows/) and [msys2](https://www.msys2.org/) manually. **You should install msys2 into the default location `C:\msys64`**, otherwise you would have to change the hard-coded path in `startsample.bat`. You would also need to install python dependencies by:

1. Running `pip install flask eventlet pvporcupine eng_to_ipa`.
2. Install pyaudio: you can try `pip install pyaudio`, but this failed when we tried it. Instead, you could [download third-party compiled packages here](https://www.lfd.uci.edu/~gohlke/pythonlibs/#pyaudio). Just download `PyAudio‑0.2.11‑cp39‑cp39‑win_amd64.whl` and run <code>pip install <b>&lt;path to downloaded file&gt;</b></code>.

After installing these dependencies, unzip the avs zip to any folder, then run `startapp.bat` inside. This will open two terminal windows and both needs to be kept open before and while running the unity app.

The client in the packaged zip is authenticated to my temproary account, but in the future you might need to re-authenticate. Keep an eye on the terminal window with the title "startsample.bat" for an auth link.

## Setting up AVS client manually

See [AVSClientFiles/readme.txt](AVSClientFiles/readme.txt)
