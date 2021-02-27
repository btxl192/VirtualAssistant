from .intent_base import *

class AMAZON_StopIntent(intent_base):
    def action(self, handler_input):
        self.push_to_notifier("VidControl", "Stop")
        self.push_to_notifier("VidControl", "Idle")
        self.response = "Goodbye"
        self.should_end_session = True
        self.user_input = "Stopped skill"
        #self.response["response"]["shouldEndSession"] = True