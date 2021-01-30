from .intent_base import *

class StopVideoIntent(intent_base):
    def action(self, intents):
        self.push_to_notifier("VidControl", "Stop")
        self.user_input = "Stopped video"