HOW TO USE:

1) Set up the AVS client according to 

   https://developer.amazon.com/en-US/docs/alexa/avs-device-sdk/windows-64.html

2) Replace the MediaPlayer.cpp in 

   \avs-device-sdk\MediaPlayer\GStreamerMediaPlayer\src

   with the MediaPlayer.cpp in this folder (AVSClientFiles)

3) Replace the UserInputManager.cpp in

   \avs-device-sdk\SampleApp\src

   with the UserInputManager.cpp in this folder (AVSClientFiles)

3) Run make again to build the new media player

4) Copy the following files to your MINGW home directory 

   host_output.py
   startapp.bat
   startaudiohost.bat
   startsample.bat

5) Use startapp.bat to run the client