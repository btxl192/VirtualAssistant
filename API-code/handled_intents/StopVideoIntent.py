from .intent_base import *

class StopVideoIntent(intent_base):
    async def action(self, intents):
        await self.push_to_notifier("VidControl: Stop")