# blue-assistant

Project website: https://students.cs.ucl.ac.uk/2020/group24/

## Development setup

### Skill server setup

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

### Setting up AVS client with pre-packaged zip

You can [download a zip of the compiled AVS client](https://mw-public-data.s3.eu-west-2.amazonaws.com/e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855/avs.zip). It contains only compiled files (and therefore might not be suitable for debugging) but they are ready to run.

You would still need to install [Python 3](https://www.python.org/downloads/windows/) and [msys2](https://www.msys2.org/) manually. **You should install msys2 into the default location `C:\msys64`**, otherwise you would have to change the hard-coded path in `startsample.bat`. After installing msys, open a msys terminal and enter:

```sh
pacman -S --noconfirm --needed git mingw-w64-x86_64-toolchain mingw-w64-x86_64-lld mingw-w64-x86_64-cmake msys/tar msys/make mingw-w64-x86_64-sqlite3 mingw64/mingw-w64-x86_64-gstreamer mingw64/mingw-w64-x86_64-gst-plugins-good mingw64/mingw-w64-x86_64-gst-plugins-base mingw64/mingw-w64-x86_64-gst-plugins-ugly mingw64/mingw-w64-x86_64-gst-plugins-bad mingw64/mingw-w64-x86_64-faad2 mingw64/mingw-w64-x86_64-portaudio
```

to install avs dependencies, then, unzip the avs zip to any folder, and run `startapp.bat` inside. This will open two terminal windows and both needs to be kept open before and while running the unity app.

You will need to authenticate your Amazon account and grant Alexa access by following the link printed in the terminal window with the title "startsample.bat".

### Setting up AVS client manually

See [AVSClientFiles/readme.txt](AVSClientFiles/readme.txt)

### Unity client setup

Open [Avatar/ConfigGen/index.html](Avatar/ConfigGen/index.html) in a browser to generate the config file for the unity client.

After that, the unity client should work as long as the AVS client is running.
