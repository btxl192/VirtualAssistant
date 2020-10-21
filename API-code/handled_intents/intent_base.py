from copy import deepcopy
import sys, inspect
import eng_to_ipa as ipa
import asyncio

####################
# Alexa skills API #
####################

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
    r = deepcopy(json_response_template)
    r["response"]["outputSpeech"]["text"] = text
    return r
    
class intent_base:
    response = None
    notifier = None
    
    def __init__(self, notifier):
        self.response = text_response("")
        self.notifier = notifier
    
    async def action(self, intents):
        pass

    async def run(self, intents):
        await self.action(intents)
        return self.response

    #Sends a message through the websocket to the Unity client
    async def push_to_notifier(self, text):
        print(f"Pushing [{text}] to notifier")
        await self.notifier.push(f"{text}")

    async def push_to_notifier_speech(self, text):
        t = ipa.convert(text)
        print(f"Pushing [Speech: {t}] to notifier")
        await self.notifier.push(f"Speech: {t}")    

    #Sets the Alexa response with an output speech
    def set_response(self, output_speech):
        self.response = text_response(output_speech)
