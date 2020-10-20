echo "-------------------installing dependencies-----------------------"
sudo apt-get install mpg123 mpv
sudo apt-get install gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gir1.2-gstreamer-1.0 python-gi python-gst-1.0
sudo apt-get install python-pyaudio
sudo apt-get install portaudio19-dev
python3 -m pip install -r requirements.txt
echo "--------------------installed dependencies------------------------"
echo "--------------------checking alexa libs------------------------"
alexa-audio-check
alexa-auth
