from .intent_base import *

class AMAZON_HelpIntent(intent_base):
    def action(self, intents):
        self.response = "Try asking about the company or play a video"