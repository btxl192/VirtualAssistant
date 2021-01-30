from .intent_base import *

class AMAZON_FallbackIntent(intent_base):
    def action(self, intents):
        self.response = "Sorry I did not understand that, please try again"
        self.user_input = "Unknown"