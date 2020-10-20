from .intent_base import *

class AMAZON_StopIntent(intent_base):
    def action(self, intents):
        self.add_notif("VidControl: Stop")
        self.add_notif("VidControl: Idle")
        self.set_response("Goodbye")
        self.response["response"]["shouldEndSession"] = True