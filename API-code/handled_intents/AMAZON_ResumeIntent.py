from .intent_base import *

class AMAZON_ResumeIntent(intent_base):
    def action(self, handler_input):
        self.push_to_notifier("VidControl", "Resume")
        self.user_input = "Resumed"