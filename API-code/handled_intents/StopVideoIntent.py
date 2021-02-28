from .intent_base import *

class StopVideoIntent(intent_base):
    def action(self, handler_input):
        self.add_unity_msg("VidControl", "Stop")
        self.user_input = "Stopped video"