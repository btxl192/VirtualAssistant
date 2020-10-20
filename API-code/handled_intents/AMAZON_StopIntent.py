from .intent_base import *

class AMAZON_StopIntent(intent_base):
    def action(self, intents):
        self.notif.append("VidControl: Stop")
        self.notif.append("VidControl: Idle")
        self.response = text_response("Goodbye")
        self.response["response"]["shouldEndSession"] = True