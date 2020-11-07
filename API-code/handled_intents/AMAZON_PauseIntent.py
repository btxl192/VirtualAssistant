from .intent_base import *

class AMAZON_PauseIntent(intent_base):
    def action(self, intents):
        self.push_to_notifier("VidControl: Pause")