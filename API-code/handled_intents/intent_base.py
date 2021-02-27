from copy import deepcopy
from ask_sdk_core.dispatch_components import AbstractRequestHandler
from ask_sdk_core.utils import is_intent_name, get_slot_value
from ask_sdk_model.dialog.delegate_directive import DelegateDirective
from ask_sdk_model.intent import Intent
import sys, inspect
import eng_to_ipa as ipa
import asyncio
import json
import time

def set_dismissal_msg(handler_input, msg):
    get_sess_attr(handler_input)["dismissal_msg"] = msg

def add_answer_intent(handler_input, intent_name):
    if "answer_intents" not in get_sess_attr(handler_input):
        get_sess_attr(handler_input)["answer_intents"] = {}
    get_sess_attr(handler_input)["answer_intents"].add(intent_name)   

def add_answer_intent_many(handler_input, intent_names):
    for i in intent_names:
        add_answer_intent(handler_input, i)

def get_sess_attr(handler_input):
    return handler_input.attributes_manager.session_attributes

def get_slot_dict(handler_input):
    return handler_input.request_envelope.request.intent.to_dict().get("slots")

class intent_base(AbstractRequestHandler):
    response = None
    emotion = None
    notifier = None
    user_input = None
    should_end_session = False
    #chained_intent_name = ""
    #chained_intent_slots = {}

    def __init__(self, notifier):
        self.response = "Warning: no speech output was set to this intent"
        self.user_input = "No user input was defined in this intent"
        self.notifier = notifier
    
    def getIntentName(self):
        return self.__class__.__name__.replace("_",".")

    def action(self, handler_input):
        pass

    def can_handle(self, handler_input):
        return is_intent_name(self.getIntentName())(handler_input)

    def handle(self, handler_input):
        dismissal_msg = ""
        if "answer_intents" in get_sess_attr(handler_input):
            ans_intents = get_sess_attr(handler_input)["answer_intents"]
            if self.getIntentName() not in ans_intents:
                dismissal_msg = get_sess_attr(handler_input)["dismissal_msg"]
                ans_intents = {}
            
        self.action(handler_input)
        
        total_response = dismissal_msg + self.response
        
        self.push_to_notifier("AlexaResponse", total_response)
        self.push_to_notifier("UserInput", self.user_input)
        
        t = ipa.convert(total_response)
        unity_speech = {"Speech": t}
        if self.emotion != None:
            unity_speech["Emotion"] = self.emotion
            self.emotion = None       
               
        resp = handler_input.response_builder       
        
        #if self.chained_intent_name != "":
        #    resp.add_directive(DelegateDirective(Intent(name=self.chained_intent_name, slots=self.chained_intent_slots, confirmation_status = "NONE")))
        #    self.chained_intent_name = ""
        #    self.chained_intent_slots = {}
            
        resp.speak(total_response).set_should_end_session(self.should_end_session)
            
        self.push_to_notifier_dict(unity_speech)
        return resp.response

    #Sends a message through the websocket to the Unity client
    def push_to_notifier(self, message_title, message_text):
        self.push_to_notifier_dict({message_title: message_text})          

    #takes a dictionary as a parameter
    def push_to_notifier_dict(self, messages):
        messages["id"] = round(time.time() * 1000)
        t = json.dumps(messages)       
        print(f"Pushing [{t}] to notifier")       
        #self.notifier.emit("message", f"{t}") 
        self.notifier.msg = t