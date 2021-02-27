from .intent_base import *

class AMAZON_HelpIntent(intent_base):
    def action(self, handler_input):
        self.response = "Try asking about the company or play a video"
        self.user_input = "Asked for help"