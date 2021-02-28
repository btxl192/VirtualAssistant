from .intent_base import *

class AMAZON_ResumeIntent(intent_base):
    def action(self, handler_input):
        self.add_unity_msg("VidControl", "Resume")
        self.user_input = "Resumed"