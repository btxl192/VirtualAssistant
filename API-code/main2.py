from flask import Flask
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter
from copy import deepcopy

from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name
from ask_sdk_model.ui import SimpleCard
import ask_sdk_core.utils as ask_utils

class LaunchRequestHandler(AbstractRequestHandler):
    """Handler for Skill Launch."""

    def can_handle(self, handler_input):
        # type: (HandlerInput) -> bool

        return ask_utils.is_request_type("LaunchRequest")(handler_input)

    def handle(self, handler_input):
        # type: (HandlerInput) -> Response
        speech_text = "test"

        return handler_input.response_builder.speak(speech_text).set_card(SimpleCard("Hello World", speech_text)).response

class SessionEndedRequest(AbstractRequestHandler):
    """Handler for Skill End."""

    def can_handle(self, handler_input):
        # type: (HandlerInput) -> bool

        return ask_utils.is_request_type("SessionEndedRequest")(handler_input)

    def handle(self, handler_input):
        # type: (HandlerInput) -> Response
        speech_text = "test2"

        return handler_input.response_builder.speak(speech_text).set_card(SimpleCard("Hello World", speech_text)).response

app = Flask(__name__)
skill_builder = SkillBuilder()
# Register your intent handlers to the skill_builder object

skill_builder.add_request_handler(LaunchRequestHandler())
skill_builder.add_request_handler(SessionEndedRequest())

skill_adapter = SkillAdapter(skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant", methods=['POST'])
def invoke_skill():
    return skill_adapter.dispatch_request()