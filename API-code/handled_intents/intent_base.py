from copy import deepcopy
from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name
import sys, inspect
import eng_to_ipa as ipa
import asyncio
    
class intent_base(AbstractRequestHandler):
    response = None
    notifier = None

    def __init__(self, notifier):
        self.response = "Warning: no speech output was set to this intent"
        self.notifier = notifier
    
    def getIntentName(self):
        return self.__class__.__name__.replace("_",".")

    async def action(self, intents):
        pass

    async def run(self, intents):
        await self.action(intents)
        return self.response

    def can_handle(self, handler_input):
        return is_intent_name(self.getIntentName())(handler_input)

    def handle(self, handler_input):
        self.action(handler_input.request_envelope)
        return handler_input.response_builder.speak(self.response).response

    #Sends a message through the websocket to the Unity client
    async def push_to_notifier(self, text):
        print(f"Pushing [{text}] to notifier")
        await self.notifier.push(f"{text}")

    async def push_to_notifier_speech(self, text):
        t = ipa.convert(text)
        print(f"Pushing [Speech: {t}] to notifier")
        await self.notifier.push(f"Speech: {t}")    
