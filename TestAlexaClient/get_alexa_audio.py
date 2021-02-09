from pydub import AudioSegment
from selenium import webdriver
from datetime import datetime
import requests
import os
import time
import threading
import websockets
import asyncio
import json

async def get_audio(ws, path):
    listen_length = 0
    delay = 0.1
    current_time = 0
    prev_res = None
    
    js_to_run = 'return window.performance.getEntries().filter(n => n["name"].includes("mp3")).pop()'
    
    print("running")
    while listen_length == 0 or current_time < listen_length:        
        resource_json = chrome_webdriver.execute_script(js_to_run)
        if resource_json != None and resource_json != prev_res:
            prev_res = resource_json
            t = {"id": round(time.time() * 1000), "SpeechUrl": resource_json["name"], "SpeechControl": "written"}
            await ws.send(json.dumps(t))
            print(str(datetime.now()) + "; Sent audio to Unity!")
        time.sleep(delay)
        if (listen_length > 0):
            current_time += delay

def start_thread():
    asyncio.set_event_loop(asyncio.new_event_loop())    
    start_server = websockets.serve(get_audio, "localhost", 5000)
    asyncio.get_event_loop().run_until_complete(start_server)
    asyncio.get_event_loop().run_forever()

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
    
x = threading.Thread(target=start_thread)
x.start()