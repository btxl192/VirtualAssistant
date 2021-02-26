from .intent_base import *
import json

class CompanyInfoIntent(intent_base):
    def action(self, intents):  
        self.response = "intent worked"