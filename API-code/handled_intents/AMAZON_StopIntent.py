from .intent_base import *

class AMAZON_StopIntent(intent_base):
    async def action(self, intents):
        await self.push_to_notifier("VidControl: Stop")
        await self.push_to_notifier("VidControl: Idle")
        self.response = "Goodbye"
        #self.response["response"]["shouldEndSession"] = True