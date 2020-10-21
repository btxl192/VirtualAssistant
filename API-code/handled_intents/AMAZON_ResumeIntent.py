from .intent_base import *

class AMAZON_ResumeIntent(intent_base):
    async def action(self, intents):
        await self.push_to_notifier("VidControl: Resume")