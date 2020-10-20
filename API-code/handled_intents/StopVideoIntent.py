from .intent_base import *

class StopVideoIntent(intent_base):
    def action(self, intents):
        self.add_notif("VidControl: Stop")