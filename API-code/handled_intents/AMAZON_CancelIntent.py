from .intent_base import *

class AMAZON_CancelIntent(intent_base):
    def action(self, handler_input):
        self.response = "Goodbye"
        self.user_input = "Ended session"
        self.should_end_session = True