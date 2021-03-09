if not exist .\build\bin\audioOutput\NUL mkdir .\build\bin\audioOutput
echo 1 > ".\build\bin\alexaInput.txt"
echo. >> ".\build\bin\alexaInput.txt"
start startsample.bat
start startaudiohost.bat