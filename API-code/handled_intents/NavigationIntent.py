from .intent_base import *
import json

def get_route(room):
    with open("navigation.json", "r") as file:
        f = json.load(file)
        return f.get(room)

def navigation_intent(room):
    route = get_route(room)
    if route is None:
        return "Sorry I could not recognise that room, please try again"
    else:
        return route[0]

class NavigationIntent(intent_base):
    def action(self, handler_input):
        #slots = intents.to_dict().get("slots")
        #room = slots.get("Room").get("value").lower()
        room = get_slot_value(handler_input, "Room").lower()
        output_speech = navigation_intent(room)
        print(output_speech)
        self.response = output_speech
        self.user_input = "Asked for navigation to " + room
