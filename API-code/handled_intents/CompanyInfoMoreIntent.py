from .intent_base import *
import json

class CompanyInfoMoreIntent(intent_base):
    def action(self, intents):  
        self.response = "intent worked"