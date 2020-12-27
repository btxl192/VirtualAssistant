from .intent_base import *

class AMAZON_FallbackIntent(intent_base):
    def action(self, intents):
        self.push_to_notifier_speech("Sorry I did not understand that, please try again")
        self.response = "Sorry I did not understand that, please try again"
        self.user_input = "Unknown"