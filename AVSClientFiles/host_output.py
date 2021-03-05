from flask import Flask, send_file
from eventlet import wsgi
import eventlet
import os
import time
import json
import threading

audio_output_dir = "./build/bin/audioOutput"

def get_id():
    return round(time.time() * 1000)

current_result = 0
current_id = get_id()

app = Flask(__name__)

@app.route("/")
def home():
    return json.dumps({"id": current_id, "SpeechControl": "written"})

@app.route("/audio")
def get_audio():
    global current_result
    if current_result > 0:
        return send_file(audio_output_dir + "/result-" + str(current_result) + ".mp3")
    return ""
    
def start_server():
    wsgi.server(eventlet.listen(('', 5000)), app) 

def count_files():
    global current_result
    global current_id
    while True:
        #count = len(os.listdir(audio_output_dir))
        try:
            count = int(open(audio_output_dir + "/latestResult.txt").readline())
            if count != current_result:
                print("files updated")
                current_result = count
                current_id = get_id()
        except:
            pass
        time.sleep(0.1)
    
threading.Thread(target=start_server).start()
threading.Thread(target=count_files).start()