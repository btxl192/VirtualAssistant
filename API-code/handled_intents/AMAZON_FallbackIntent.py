from .intent_base import *

class AMAZON_FallbackIntent(intent_base):
    async def action(self, intents):
        self.set_response("Sorry I did not understand that, please try again")