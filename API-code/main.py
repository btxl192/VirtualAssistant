from flask import Flask, send_file, request
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter
from copy import deepcopy
from eventlet import wsgi
import eng_to_ipa as ipa

from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name, is_request_type

import eventlet
import importlib
import os
import sys
import json
import time

class msg_container:
    msg = ""

#class to handle the launch intent
class LaunchRequestHandler(AbstractRequestHandler):
    """Handler for Skill Launch."""
    def can_handle(self, handler_input):
        return is_request_type("LaunchRequest")(handler_input)

    def handle(self, handler_input):
        speech_text = "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"
        t = {"id": round(time.time() * 1000), "Speech": ipa.convert(speech_text), "AlexaResponse": speech_text}
        current_msg.msg = t
        return handler_input.response_builder.speak(speech_text).set_should_end_session(False).response

#class to handle the session end intent
class SessionEndedRequest(AbstractRequestHandler):
    """Handler for Skill End."""
    def can_handle(self, handler_input):
        return is_request_type("SessionEndedRequest")(handler_input)

    def handle(self, handler_input):
        speech_text = ""
        return handler_input.response_builder.speak(speech_text).set_should_end_session(False).response

logs = ["start of logs"]
app = Flask(__name__)
skill_builder = SkillBuilder()
current_msg = msg_container()

#get path to ssl certificate and key from console
cert = sys.argv[1]
key = sys.argv[2]

# Register your intent handlers to the skill_builder object
skill_builder.add_request_handler(LaunchRequestHandler())
skill_builder.add_request_handler(SessionEndedRequest())

#add the intents from the ./handled_intents folder to the skill_builder
blacklist = ["test.py", "intent_base.py", "__init__.py"]
for file in os.listdir("./handled_intents"):
    if file not in blacklist and file[-3:] == ".py":
        filename = file[:-3]
        intent_name = filename.replace("_",".")
        imported_intent = importlib.import_module("handled_intents." + filename)
        intent_instance = getattr(imported_intent, filename)(current_msg)
        skill_builder.add_request_handler(intent_instance)

skill_adapter = SkillAdapter(skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant", methods=['POST'])
def invoke_skill():
    print("skill started")
    return skill_adapter.dispatch_request()

@app.route("/companyVideo")
def video():
    response = send_file("./static/video.mp4")
    return response

@app.route("/api/v1/speechLogs", methods=["GET", "POST"])
def speechlogs(text: str = ""):
    if request.method == 'POST':
        logs.append(text)
    else:
        return "\n".join(logs)
    
@app.route("/msg")
def get_msg():
    return current_msg.msg

wsgi.server(eventlet.wrap_ssl(eventlet.listen(('', 4430)), certfile=cert, keyfile=key, server_side=True), app)