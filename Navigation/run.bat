if not exist ".\firststart" pip install -r requirements.txt

echo. > ".\firststart"

if exist floorRooms rmdir /s /q floorRooms

java -jar Route-1.0-SNAPSHOT-jar-with-dependencies.jar
