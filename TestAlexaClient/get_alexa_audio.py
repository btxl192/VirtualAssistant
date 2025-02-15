from pydub import AudioSegment
from selenium import webdriver
from datetime import datetime
import time
import threading
import json
from flask import Flask, send_file
from flask_socketio import SocketIO, emit
from eventlet import wsgi
import eventlet
import pvporcupine
import pyaudio
import struct

current_msg = {}
chrome_webdriver = None

def get_alexa_output():
    global chrome_webdriver
    listen_length = 0
    delay = 0.1
    current_time = 0
    prev_res = None

    clear_resource_timer = 10 #10 seconds
    clear_resource_timer_current = 0
    js_clear_resource = 'window.performance.clearResourceTimings()'

    js_to_run = 'return window.performance.getEntries().filter(n => n["name"].includes("mp3")).pop()'

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

    while listen_length == 0 or current_time < listen_length:
        resource_json = chrome_webdriver.execute_script(js_to_run)
        if resource_json != None and resource_json != prev_res:
            prev_res = resource_json
            t = {"id": round(time.time() * 1000), "SpeechUrl": resource_json["name"], "SpeechControl": "written"}
            global current_msg
            current_msg = json.dumps(t)
            print(str(datetime.now()) + "; Sent audio to Unity!")
        time.sleep(delay)
        if listen_length > 0:
            current_time += delay
        if clear_resource_timer_current < clear_resource_timer:
            clear_resource_timer_current += delay
        else:
            try:
                print("clearing resource timings")
                chrome_webdriver.execute_script(js_clear_resource)
                clear_resource_timer_current = 0
            except:
                print("error in clearing resource timings")

detected = False
silence_timeout = 1
silence_timeout_counter = 0

#continuously loops. hotword detection
def hotword():
    global detected
    global silence_timeout_counter
    global silence_timeout
    global chrome_webdriver
    porcupine = pvporcupine.create(keywords=['alexa'])
    pa = pyaudio.PyAudio()
    audio_stream = pa.open(
        rate=porcupine.sample_rate,
        channels=1,
        format=pyaudio.paInt16,
        input=True,
        frames_per_buffer=porcupine.frame_length)

    print("Listening for 'Alexa'")
    while True:
        pcm = audio_stream.read(porcupine.frame_length)
        pcm = struct.unpack_from("h" * porcupine.frame_length, pcm)

        keyword_index = porcupine.process(pcm)
        if keyword_index >= 0:
            print("DETECTED")
            detected = True
            c = chrome_webdriver.find_element_by_class_name("icon-mic")
            webdriver.ActionChains(chrome_webdriver).click_and_hold(c).perform()

        if detected and max(pcm) < 500:
            if silence_timeout_counter >= silence_timeout:
                print("SILENT")
                detected = False
                silence_timeout_counter = 0
                webdriver.ActionChains(chrome_webdriver).release().perform()
            silence_timeout_counter += 0.05
            time.sleep(0.05)

app = Flask(__name__)
socketio = SocketIO(app, async_mode = "eventlet")

@app.route("/")
def message():
    global current_msg
    return current_msg

def start_server():
    wsgi.server(eventlet.listen(('', 5000)), app)

threading.Thread(target=start_server).start()
threading.Thread(target=get_alexa_output).start()
threading.Thread(target=hotword).start()
