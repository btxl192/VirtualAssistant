from .intent_base import *

class AMAZON_ResumeIntent(intent_base):
    def action(self, intents):
        self.add_notif("VidControl: Resume")