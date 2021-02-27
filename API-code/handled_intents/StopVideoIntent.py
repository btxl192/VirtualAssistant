from .intent_base import *

class StopVideoIntent(intent_base):
    def action(self, handler_input):
        self.push_to_notifier("VidControl", "Stop")
        self.user_input = "Stopped video"