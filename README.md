# blue-assistant

Project website: https://students.cs.ucl.ac.uk/2020/group24/

Currently, this project only supports Windows.

## User setup

For user setup please consult the setup manual user-manual.pdf with detailed screenshots and instructions.

## Development setup

### Skill server setup

Setting up the skill server for development can be done in the same way as setting it up for production.

1. Create a debian cloud server.
2. Copy API-code into the server. e.g.:
```sh
scp -r API-code skill.comp0016.mww.moe:/home/azureuser/API-code
```
alternatively, clone the git repository on the server.

3. Run `./configure.sh DOMAIN_NAME` on the server with `DOMAIN_NAME` replaced with the domain name that you want to use. e.g.:
```sh
cd API-code/
./configure.sh skill.comp0016.mww.moe
```
This will take a while as it will install certbot, get HTTPS certificate from Let's Encrypt, and install Docker and build the container which will run the skill server. After this, the server would be installed as a systemd service `skill-backend.service` and will automatically be started. To rebuild the server, simply run `sudo systemctl restart skill-backend`.

### Setting up AVS client with pre-packaged zip

You can [download a zip of the compiled AVS client](https://mw-public-data.s3.eu-west-2.amazonaws.com/e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855/avs.zip). It contains only compiled files (and therefore might not be suitable for debugging) but they are ready to run.

You would still need to install [Python 3](https://www.python.org/downloads/windows/) and [msys2](https://www.msys2.org/) manually. **You should install msys2 into the default location `C:\msys64`**, otherwise you would have to change the hard-coded path in `startsample.bat`. After installing msys, open a msys terminal and run `./AVSClientFiles/mingw-setup.sh` to install avs dependencies, then, unzip the avs zip to any folder, and (outside mingw) run `startapp.bat` inside. This will open two terminal windows and both needs to be kept open before and while running the unity app.

You will need to authenticate your Amazon account and grant Alexa access by following the link printed in the terminal window with the title "startsample.bat".

### Setting up AVS client manually

See [AVSClientFiles/readme.txt](AVSClientFiles/readme.txt)

### Unity client setup

Open [Avatar/ConfigGen/index.html](Avatar/ConfigGen/index.html) in a browser to generate the config file for the unity client. Follow the instructions at the button of the page.

After that, the unity client should work as long as the AVS client is running.

When building a final executable, make sure the "Navigation" folder is inside the parent directory of the build directory. For example:

```
(project root)
├── Avatar
│   └── ... (unity project)
├── avatar-build (build folder)
│   ├── Avatar_Data
│   ├── Avatar.exe
│   └── ...
├── Navigation
│   ├── 1
│   ├── 2
│   └── ...
└── ... (other files in the repository)
```

### Using the test alexa client

The test client is located inside TestAlexaClient. It is basically a python script which controls a chrome instance visiting the [Alexa skill test page](https://developer.amazon.com/alexa/console/ask/test/amzn1.ask.skill.fa7cfeb1-e524-4024-a258-5249bec81e5f/development/en_GB). You can use it instead of the AVS client when developing the unity front-end&mdash;you just need to set `testClient` to `true` in `config.json` or ticking the "Use test client" checkbox when generating the config.

You can put your Amazon credential in the form of

```
email
password
```
into a new file `login.txt` under `TestAlexaClient`, so that the client can automatically populate the credentials.

The test client requires chrome to be installed and a correct version (corrosponding to your installed chrome version) of the "chromedriver.exe" binary. You can [download the required version of chromedriver.exe here](https://chromedriver.chromium.org/downloads). Place the downloaded executable into TestAlexaClient and rename it to be `chromedriver.exe`. Afterward, you can run `run.bat` to start the test client.

You need to repeat the previous step each time your chrome updates, if you want to continue to use the test client.

## Navigation configuration

For instruction about configurating the floor map navigation feature see section "Navigation configuration" in user manual.

## Legal

**The software is an early proof of concept for development purposes and should not be used as-is in a live environment without further redevelopment and/or testing.** No warranty is given and no real data or personally identifiable data should be stored. Usage and its liabilities are your own.

[See a list of open source libraries & assets used](https://students.cs.ucl.ac.uk/2020/group24/license-details.html)
