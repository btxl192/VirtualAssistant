from .intent_base import *

class AMAZON_PauseIntent(intent_base):
    def action(self, intents):
        self.notif.append("VidControl: Pause")