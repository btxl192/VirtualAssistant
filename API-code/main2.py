from flask import Flask
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter
from copy import deepcopy

from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name, is_request_type

import importlib
import os

class Notifier:
    def __init__(self):
        self.connections: List[WebSocket] = []
        self.generator = self.get_notification_generator()

    async def get_notification_generator(self):
        while True:
            message = yield
            await self._notify(message)

    async def push(self, msg: str):
        await self.generator.asend(msg)

    async def connect(self, websocket: WebSocket):
        await websocket.accept()
        self.connections.append(websocket)

    def remove(self, websocket: WebSocket):
        self.connections.remove(websocket)

    async def _notify(self, message: str):
        living_connections = []
        while len(self.connections) > 0:
            # Looping like this is necessary in case a disconnection is handled
            # during await websocket.send_text(message)
            websocket = self.connections.pop()
            await websocket.send_text(message)
            living_connections.append(websocket)
        self.connections = living_connections

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

notifier = Notifier()

app = Flask(__name__)
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
        intent_instance = getattr(imported_intent, filename)(notifier)
        skill_builder.add_request_handler(intent_instance)

skill_adapter = SkillAdapter(skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant", methods=['POST'])
def invoke_skill():
    return skill_adapter.dispatch_request()