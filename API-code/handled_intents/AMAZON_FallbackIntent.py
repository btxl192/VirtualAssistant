from .intent_base import *

class AMAZON_FallbackIntent(intent_base):
    def action(self, handler_input):
        self.response = "Sorry I did not understand that, please try again"
        self.user_input = "Unknown"