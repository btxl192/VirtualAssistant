from flask import Flask, send_file
import os
import time
import json
import threading
import pvporcupine
import pyaudio
import struct
import eng_to_ipa as ipa 

def get_id():
    return round(time.time() * 1000)

#where the server looks for audio files    
audio_output_dir = "./build/bin/audioOutput"

#where the server looks for caption files    
text_output_dir = "./build/bin/textOutput"

#write to this file when "Alexa" is detected
alexa_input_dir = "./build/bin/alexaInput.txt"

stop_threads = False

#current audio file
current_result = 0
current_id = -1
app = Flask(__name__)

#returns the current id
@app.route("/")
def home():
    return json.dumps({"id": current_id, "SpeechControl": "written"})

#returns the current audio file
@app.route("/audio")
def get_audio():
    global current_result
    if current_result > 0:
        return send_file(audio_output_dir + "/result-" + str(current_result) + ".mp3")
    return ""

#returns the current caption file
@app.route("/caption")
def get_caption():
    global current_result
    if current_result > 0:
        return ipa.convert(open(text_output_dir + "/caption-" + str(current_result) + ".txt").readline())
    return ""

#continuously loops. updates current_id when latestResult.txt is updated
def count_files():
    global current_result
    global current_id
    global stop_threads
    while not stop_threads:
        try:
            count = int(open(audio_output_dir + "/latestResult.txt").readline())
            if count != current_result:
                print("files updated")
                current_result = count
                current_id = get_id()
        except:
            pass
        time.sleep(0.1)
    print("Stopping thread (2/2)")

#continuously loops. hotword detection
def get_alexa_input():   
    global stop_threads
    
    porcupine = pvporcupine.create(keywords=['alexa'])

    pa = pyaudio.PyAudio()
    audio_stream = pa.open(
        rate=porcupine.sample_rate,
        channels=1,
        format=pyaudio.paInt16,
        input=True,
        frames_per_buffer=porcupine.frame_length)    

    print("Listening for 'Alexa'")
    while not stop_threads:
        pcm = audio_stream.read(porcupine.frame_length)
        pcm = struct.unpack_from("h" * porcupine.frame_length, pcm)
            
        keyword_index = porcupine.process(pcm)
        if keyword_index >= 0:
            on_detected()
            
    print("Stopping thread (1/2)")        

    if porcupine is not None:
        porcupine.delete()

    if audio_stream is not None:
        audio_stream.close()

    if pa is not None:
        pa.terminate()

#runs when hotword is detected
def on_detected():
    print("DETECTED")
    f = open(alexa_input_dir, "w")
    f.write(str(get_id()) + "\n")
    f.close()

try:
    t1 = threading.Thread(target=get_alexa_input)       
    t2 = threading.Thread(target=count_files)
    
    t1.start()
    t2.start()
    
    app.run(host='0.0.0.0', port=5000)
    
    while True: 
        t1.join(1)
        t2.join(1)
except (KeyboardInterrupt, SystemExit):
    print("ENDING")
    stop_threads = True