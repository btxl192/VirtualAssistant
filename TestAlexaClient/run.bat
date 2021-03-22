if not exist ".\firststart" pip install -r requirements.txt
echo. > ".\firststart"

python get_alexa_audio.py
pause
