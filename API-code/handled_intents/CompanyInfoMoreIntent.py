from .intent_base import *
import json

class CompanyInfoMoreIntent(intent_base):
    def action(self, handler_input):  
        self.response = "intent worked"