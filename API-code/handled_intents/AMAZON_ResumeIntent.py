from .intent_base import *

class AMAZON_ResumeIntent(intent_base):
    def action(self, intents):
        self.push_to_notifier("VidControl: Resume")
        self.user_input = "Resumed"