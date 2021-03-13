from .intent_base import *

class AMAZON_StopIntent(intent_base):
    def action(self, handler_input):
        self.add_unity_msg("VidControl", "Stop")
        self.response = "Goodbye"
        self.should_end_session = True
        self.user_input = "Stopped skill"
        #self.response["response"]["shouldEndSession"] = True