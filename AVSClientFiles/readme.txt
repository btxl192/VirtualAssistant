1) Install Python 3.6.8

NOTE: Python MUST be added to your PATH

2) Install MINGW

3) Run MINGW 64 bit

4) In the MINGW terminal, run

wget https://raw.githubusercontent.com/btxl192/CustomAVSSDK/master/setup.sh \
wget https://raw.githubusercontent.com/alexa/avs-device-sdk/master/tools/Install/genConfig.sh \
wget https://raw.githubusercontent.com/alexa/avs-device-sdk/master/tools/Install/mingw.sh

5) In the MINGW terminal, run 

setup.sh <path to config.json>

   For more information about config.json:
   https://developer.amazon.com/en-US/docs/alexa/alexa-voice-service/register-a-product-with-avs.html

6) After the setup is done, run sampleapp.bat
   Register the AVS client with the displayed code

   For more information about registering your product:
   https://developer.amazon.com/en-US/docs/alexa/alexa-voice-service/register-a-product-with-avs.html

7) After registering your product, close sampleapp.bat

8) From now on, use startapp.bat to run the AVS client