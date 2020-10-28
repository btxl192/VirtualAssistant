from flask import Flask
from ask_sdk_core.skill_builder import SkillBuilder
from flask_ask_sdk.skill_adapter import SkillAdapter
from copy import deepcopy

json_response_template = {
    "version":"1.0",
    # "sessionAttributes": {

    # },
    "response":{
        "outputSpeech":{
            #https://developer.amazon.com/docs/custom-skills/request-and-response-json-reference.html#outputspeech-object
            "type":"PlainText",
        #    "text":"text to output from alexa",
            "playBehaviour":"REPLACE_ENQUEUED"
        },
    #  "reprompt": {
    #   "outputSpeech": {
    #     "type": "PlainText",
    #     "text": "Plain text string to speak",
    #     "playBehavior": "REPLACE_ENQUEUED"             
    #   }
    # },
        # "directives": [
        # ],
        "shouldEndSession": False,
    }
}

def text_response(text):
    response = deepcopy(json_response_template)
    response["response"]["outputSpeech"]["text"] = text
    return response

app = Flask(__name__)
skill_builder = SkillBuilder()
# Register your intent handlers to the skill_builder object

skill_adapter = SkillAdapter(skill=skill_builder.create(), skill_id="1", app=app)

@app.route("/api/v1/blueassistant", methods=['POST'])
def invoke_skill():
    return skill_adapter.dispatch_request()

from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name
from ask_sdk_model.ui import SimpleCard

class LaunchRequestHandler(AbstractRequestHandler):
    """Handler for Skill Launch."""

    def can_handle(self, handler_input):
        # type: (HandlerInput) -> bool

        return ask_utils.is_request_type("LaunchRequest")(handler_input)

    def handle(self, handler_input):
        # type: (HandlerInput) -> Response
        _ = handler_input.attributes_manager.request_attributes["_"]
        speak_output = _("test")

        return (
            handler_input.response_builder
            .speak(speak_output)
            .ask(speak_output)
            .response
        )

skill_builder.add_request_handler(LaunchRequestHandler())