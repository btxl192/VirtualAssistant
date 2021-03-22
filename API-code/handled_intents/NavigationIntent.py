from .intent_base import *
import json

with open("navigation.json", "r") as file:
        f = json.load(file)

def get_floors():
        return list(f.keys())

def get_blue_floor():
    return f.get("Blue")

def get_rooms(floor):
    return list(f.get(floor))

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
        room = room.replace(" ", "")
        num = get_slot_value(handler_input, "Number")
        if num is not None:
            room += str(num).lower()
        blueFloor = get_blue_floor()
        if room in get_rooms(str(blueFloor)):
            self.response = f.get(str(blueFloor)).get(room)
            self.user_input = "Asked for navigation to " + room
            self.add_unity_msg("NavRoom", room)
            return
        floors = get_floors()
        output_speech = ""
        for i in range (1, len(floors)):
            if(str(blueFloor + i)) in floors:
                if room in get_rooms(str(blueFloor + i)):
                    output_speech = "Take the lift to " + str(blueFloor + i) + "th floor. "
                    output_speech += f.get(str(blueFloor)).get("lift") + " then "
                    output_speech += f.get(str(blueFloor + i)).get(room)
                    self.response = output_speech
                    self.user_input = "Asked for navigation to " + room
                    self.add_unity_msg("NavRoom", room)
                    self.add_unity_msg("NavFloor", blueFloor + i)
                    return
            if(str(blueFloor - i)) in floors:
                if room in get_rooms(blueFloor - i):
                    output_speech = "Take the lift to " + str(blueFloor - i) +"th floor. "
                    output_speech += f.get(str(blueFloor)).get("lift") + " then "
                    output_speech += f.get(str(blueFloor - i)).get(room)
                    self.response = output_speech
                    self.user_input = "Asked for navigation to " + room
                    self.add_unity_msg("NavRoom", room)
                    self.add_unity_msg("NavFloor", blueFloor - i)
                    return
        self.response = "Sorry I could not recognise that room, please try again"
        self.user_input = "Asked for navigation to " + room

                
