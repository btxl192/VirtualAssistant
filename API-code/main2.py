from flask import Flask, send_file, request
from flask_socketio import SocketIO, emit
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter
from copy import deepcopy

from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name, is_request_type

import importlib
import os

class LaunchRequestHandler(AbstractRequestHandler):
    """Handler for Skill Launch."""
    def can_handle(self, handler_input):
        return is_request_type("LaunchRequest")(handler_input)

    def handle(self, handler_input):
        print(handler_input.request_envelope)
        speech_text = "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"
        return handler_input.response_builder.speak(speech_text).response

class SessionEndedRequest(AbstractRequestHandler):
    """Handler for Skill End."""
    def can_handle(self, handler_input):
        return is_request_type("SessionEndedRequest")(handler_input)

    def handle(self, handler_input):
        speech_text = ""
        return handler_input.response_builder.speak(speech_text).response

logs = []
app = Flask(__name__)
socketio = SocketIO(app)
skill_builder = SkillBuilder()
# Register your intent handlers to the skill_builder object

skill_builder.add_request_handler(LaunchRequestHandler())
skill_builder.add_request_handler(SessionEndedRequest())

blacklist = ["test.py", "intent_base.py", "__init__.py"]
for file in os.listdir("./handled_intents"):
    if file not in blacklist and file[-3:] == ".py":
        filename = file[:-3]
        intent_name = filename.replace("_",".")
        imported_intent = importlib.import_module("handled_intents." + filename)
        intent_instance = getattr(imported_intent, filename)(socketio)
        skill_builder.add_request_handler(intent_instance)

skill_adapter = SkillAdapter(skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant", methods=['POST'])
def invoke_skill():
    print("skill started")
    return skill_adapter.dispatch_request()

@app.route("/companyVideo")
async def video():
    response = send_file("./static/video.mp4")
    return response

@app.route("/api/v1/speechLogs", methods=["GET", "POST"])
async def speechlogs(text: str = ""):
    if request.method == 'POST':
        logs.append(text)
        socketio.emit("message", f"{text}")
    else:
        return "\n".join(logs)

@socketio.on('connect')
def client_connect():
    print("Client connected")

@socketio.on('disconnect')
def client_disconnect():
    print('Client disconnected')