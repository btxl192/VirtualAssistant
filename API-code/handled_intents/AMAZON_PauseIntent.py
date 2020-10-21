from .intent_base import *

class AMAZON_PauseIntent(intent_base):
    async def action(self, intents):
        await self.push_to_notifier("VidControl: Pause")