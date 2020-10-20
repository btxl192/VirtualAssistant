from copy import deepcopy
import sys, inspect
import eng_to_ipa as ipa

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
    notif = []
    
    def __init__(self):
        self.response = text_response("")
    
    def action(self, intents):
        pass

    #Send a message to Unity
    def add_notif(self, to_send):
        self.notif.append(to_send)

    #Send a speech message to Unity. Text is converted into phonetic alphabet
    def add_notif_speech(self, to_say):
        self.notif.append("Speech: " + ipa.convert(to_send))

    #Sets the Alexa response with an output speech
    def set_response(self, output_speech):
        self.response = text_response(output_speech)

    def run(self, intents):
        self.notif.clear()
        self.action(intents)
        return (self.notif, self.response)