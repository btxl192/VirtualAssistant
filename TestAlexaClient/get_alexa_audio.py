from pydub import AudioSegment
from selenium import webdriver
from datetime import datetime
import requests
import os

def get_alexa_output():
    chrome_options = webdriver.ChromeOptions()
    chrome_options.add_argument("--mute-audio")
    chrome_options.add_argument('--no-sandbox')
    chrome_webdriver = webdriver.Chrome(chrome_options=chrome_options)
    chrome_webdriver.get("https://developer.amazon.com/alexa/console/ask/test/amzn1.ask.skill.fa7cfeb1-e524-4024-a258-5249bec81e5f/development/en_GB/")

    username = chrome_webdriver.find_element_by_id("ap_email")
    password = chrome_webdriver.find_element_by_id("ap_password")
    
    try:
        login_details = open("login.txt").read().splitlines()
        username.send_keys(login_details[0])
        password.send_keys(login_details[1])
    except:
        pass

    listen_length = 3000
    delay = 0.01
    current_time = 0
    prev_res = None

    js_to_run = 'return window.performance.getEntries().filter(n => n["name"].includes("mp3")).pop()'

    while listen_length == 0 or current_time < listen_length:        
        resource_json = chrome_webdriver.execute_script(js_to_run)
        if resource_json != None and resource_json != prev_res:
            prev_res = resource_json
            t = {"SpeechUrl": resource_json["name"]}
            socketio.emit("message", str(t))
            t2 = {"SpeechControl": "written"}
            socketio.emit("message", str(t2))
            print(str(datetime.now()) + "; Sent audio to Unity!")
        socketio.sleep(delay)
        if (listen_length > 0):
            current_time += delay

from flask import Flask, send_file
from flask_socketio import SocketIO, emit
from eventlet import wsgi
import eventlet

app = Flask(__name__)
socketio = SocketIO(app, async_mode = "eventlet")

@app.route("/")
def myhome():
    return send_file("alexa_audio.mp3")
    
#for testing
@app.route("/sorry")
def sorry():
    return send_file("alexa_audio_sorry.wav")

#for testing
@app.route("/hi")
def hi():
    return send_file("alexa_audio_hi.wav")

@socketio.on('connect')
def test_connect():
    print("Client connected")

@socketio.on('disconnect')
def test_disconnect():
    print('Client disconnected')
    
socketio.start_background_task(target=get_alexa_output)
wsgi.server(eventlet.listen(('', 5000)), app)