from .intent_base import *

class AMAZON_PauseIntent(intent_base):
    def action(self, handler_input):
        self.add_unity_msg("VidControl", "Pause")
        self.user_input = "Paused"
        self.response = "Pausing video"