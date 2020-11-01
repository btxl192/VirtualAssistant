from .intent_base import *

class AMAZON_CancelIntent(intent_base):
    def action(self, intents):
        self.response = "Goodbye"
        self.should_end_session = True