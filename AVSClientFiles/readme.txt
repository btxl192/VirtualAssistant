HOW TO USE:

1) Set up the AVS client according to    

   https://developer.amazon.com/en-US/docs/alexa/avs-device-sdk/windows-64.html

   INSTEAD of the official repo, use the following repo

   Clone the repo from https://github.com/btxl192/CustomAVSSDK

2) Set up captions according to 

   https://developer.amazon.com/en-US/docs/alexa/avs-device-sdk/features.html#captions

   For clarification, the command in step 2 in the captions setup page should look something like:

   cmake "<absolute path to the avs-device-sdk folder>" \
-DCAPTIONS=ON \
-DLIBWEBVTT_LIB_PATH="<absolute path to liblibwebvtt.dll.a>"\
-DLIBWEBVTT_INCLUDE_DIR="<absolute path to /webvtt/include>"

3) Copy the following files to your MINGW home directory 

   host_output.py
   startapp.bat
   startaudiohost.bat
   startsample.bat

4) Use startapp.bat to run the client