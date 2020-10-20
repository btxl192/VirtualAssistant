echo "--------------------installing keyword detection-----------------------"
git clone --depth 1 https://github.com/Kitt-AI/snowboy.git snowboy_github
cd snowboy_github
sudo apt install libatlas-base-dev swig
python3 setup.py build
python3 -m pip install .
python3 -m pip install voice-engine
