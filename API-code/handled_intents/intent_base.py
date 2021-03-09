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

#returns the previous intent that was called
def get_previous_intent(handler_input):
    s = get_sess_attr(handler_input)
    if "previous_intent" in s:
        return s["previous_intent"]
    return None

#add an "answer intent"
def add_answer_intent(handler_input, intent_name):
    if "answer_intents" not in get_sess_attr(handler_input):
        set_sess_attr(handler_input, "answer_intents", [])
    if intent_name not in get_sess_attr(handler_input)["answer_intents"]:
        get_sess_attr(handler_input)["answer_intents"].append(intent_name)

def add_answer_intent_many(handler_input, intent_names):
    for i in intent_names:
        add_answer_intent(handler_input, i)

#if the next response is not an "answer intent", prepend the dismissal message
def set_dismissal_msg(handler_input, msg):
    set_sess_attr(handler_input, "dismissal_msg", msg)  

def set_sess_attr(handler_input, attr_name, val):
    get_sess_attr(handler_input)[attr_name] = val

def get_sess_attr(handler_input):
    return handler_input.attributes_manager.session_attributes

#returns the slots of the intent
def get_slot_dict(handler_input):
    return handler_input.request_envelope.request.intent.to_dict().get("slots")

class intent_base(AbstractRequestHandler):
    response = None
    emotion = None
    unity_msg = None
    user_input = None
    should_end_session = False

    def __init__(self, unity_msg):
        self.response = "Warning: no speech output was set to this intent"
        self.user_input = "No user input was defined in this intent"
        self.unity_msg = unity_msg
    
    def getIntentName(self):
        return self.__class__.__name__.replace("_",".")

    def action(self, handler_input):
        pass

    def can_handle(self, handler_input):
        return is_intent_name(self.getIntentName())(handler_input)

    def handle(self, handler_input):
        dismissal_msg = ""
        
        #prepend the dismissal message if the intent is not an answer intent
        if "answer_intents" in get_sess_attr(handler_input):
            if len(get_sess_attr(handler_input)["answer_intents"]) > 0 and self.getIntentName() not in get_sess_attr(handler_input)["answer_intents"]:
                dismissal_msg = get_sess_attr(handler_input)["dismissal_msg"]
                set_sess_attr(handler_input, "answer_intents", [])
        
        #run the intent
        self.action(handler_input)
        
        total_response = dismissal_msg + " " + self.response
        
        #add speech and emotion to unity message
        t = ipa.convert(total_response)
        self.add_unity_msg("Speech", t)
        if self.emotion != None:
            self.add_unity_msg("Emotion", self.emotion)
            self.emotion = None       
               
        resp = handler_input.response_builder       
            
        resp.speak(total_response).set_should_end_session(self.should_end_session)
            
        self.add_unity_msg("AlexaResponse", total_response)
        self.add_unity_msg("UserInput", self.user_input)  
        self.set_unity_msg(self.unity_msg.msg)
        
        set_sess_attr(handler_input, "previous_intent", self.getIntentName())
        
        return resp.response

    def add_unity_msg(self, message_title, message_text):
        self.unity_msg.msg[message_title] = message_text        

    #takes a dictionary as a parameter
    def set_unity_msg(self, messages):
        messages["id"] = round(time.time() * 1000)
        print(f"Setting [{messages}] as unity_msg")       
        self.unity_msg.msg = messages