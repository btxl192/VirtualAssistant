from copy import deepcopy
from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name
import sys, inspect
import eng_to_ipa as ipa
import asyncio
import json
    
class intent_base(AbstractRequestHandler):
    response = None
    emotion = None
    notifier = None
    user_input = None
    should_end_session = False

    def __init__(self, notifier):
        self.response = "Warning: no speech output was set to this intent"
        self.user_input = "No user input was defined in this intent"
        self.notifier = notifier
    
    def getIntentName(self):
        return self.__class__.__name__.replace("_",".")

    def action(self, intents):
        pass

    def can_handle(self, handler_input):
        return is_intent_name(self.getIntentName())(handler_input)

    def handle(self, handler_input):
        self.action(handler_input.request_envelope.request.intent)
        self.push_to_notifier("AlexaResponse", self.response)
        self.push_to_notifier("UserInput", self.user_input)
        
        t = ipa.convert(self.response)
        unity_speech = {"Speech": t}
        if self.emotion != None:
            unity_speech["Emotion"] = self.emotion
        self.push_to_notifier_dict(unity_speech)
        self.emotion = None
        return handler_input.response_builder.speak(self.response).set_should_end_session(self.should_end_session).response

    #Sends a message through the websocket to the Unity client
    def push_to_notifier(self, message_title, message_text):
        t = json.dumps({message_title: message_text})
        print(f"Pushing [{t}] to notifier")       
        self.notifier.emit("message", f"{t}")  

    #takes a dictionary as a parameter
    def push_to_notifier_dict(self, messages):
        t = json.dumps(messages)
        print(f"Pushing [{t}] to notifier")       
        self.notifier.emit("message", f"{t}") 