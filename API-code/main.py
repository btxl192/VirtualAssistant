from fastapi import FastAPI
from starlette.requests import Request
from starlette.websockets import WebSocket, WebSocketDisconnect
from starlette.templating import Jinja2Templates
from starlette.staticfiles import StaticFiles
from starlette.responses import FileResponse
from pydantic import BaseModel
from copy import deepcopy
from handled_intents.intent_base import text_response
import json
import asyncio
import os
import importlib
import eng_to_ipa as ipa

app = FastAPI()

templates = Jinja2Templates(directory="templates")
app.mount("/static", StaticFiles(directory="static"), name="static")

logs = ""

class Notifier:
    def __init__(self):
        self.connections: List[WebSocket] = []
        self.generator = self.get_notification_generator()

    async def get_notification_generator(self):
        while True:
            message = yield
            await self._notify(message)

    async def push(self, msg: str):
        await self.generator.asend(msg)

    async def connect(self, websocket: WebSocket):
        await websocket.accept()
        self.connections.append(websocket)

    def remove(self, websocket: WebSocket):
        self.connections.remove(websocket)

    async def _notify(self, message: str):
        living_connections = []
        while len(self.connections) > 0:
            # Looping like this is necessary in case a disconnection is handled
            # during await websocket.send_text(message)
            websocket = self.connections.pop()
            await websocket.send_text(message)
            living_connections.append(websocket)
        self.connections = living_connections


notifier = Notifier()

@app.get("/")
async def root(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})

#Sends a message through the websocket to the Unity client
async def push_to_notifier(text):
    print(f"Pushing [{text}] to notifier")
    await notifier.push(f"{text}")

@app.post("/api/v1/speechLogs")
async def post_log(text: str = ""):
    # global logs
    # logs = text
    # return "ok"
    # push_to_notifier(text)
    global logs
    logs = text
    await notifier.push(f"{text}")


@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await notifier.connect(websocket)
    try:
        while True:
            data = await websocket.receive_text()
            await websocket.send_text(f"{data}")
    except WebSocketDisconnect:
        notifier.remove(websocket)

@app.on_event("startup")
async def startup():
    # Prime the push notification generator
    await notifier.generator.asend(None)

@app.get("/api/v1/speechLogs")
def get_log():
    global logs
    return logs
    
@app.get("/companyVideo")
async def video():
    response = FileResponse("./static/video.mp4")
    return response

class AlexaRequest(BaseModel):
    version: str
    session: dict
    context: dict
    request: dict

func_mappings = {}
#read the .py files in ./handled_intents and map each intent to its corresponding action
#the action returns a 2-tuple
#   - (list of notifier messages to be sent, json text response)
def create_intent_mappings():
    blacklist = ["test.py", "intent_base.py", "__init__.py"]
    for file in os.listdir("./handled_intents"):
        if file not in blacklist and file[-3:] == ".py":
            filename = file[:-3]
            intent_name = filename.replace("_",".")
            imported_intent = importlib.import_module("handled_intents." + filename)
            intent_instance = getattr(imported_intent, filename)()
            func_mappings[intent_name] = intent_instance.run

@app.post('/api/v1/blueassistant')
async def blue_assistant(baseRequest: AlexaRequest):
    if baseRequest.request.get("type") == "LaunchRequest":
        create_intent_mappings()
        t = "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"  
        await push_to_notifier("Speech: " + ipa.convert(t))
        await push_to_notifier("SpeechControl: Start")
        return text_response(t)
    elif baseRequest.request.get("type") == "SessionEndedRequest":
        await push_to_notifier("Msg: SessionEndedRequest")
        return text_response("")
        
    intents = baseRequest.request.get("intent")
    specific_intent = intents.get("name")
    
    if specific_intent in func_mappings.keys():
        #get the corresponding notifier messages and Alexa text response from an intent
        #result[0] contains a list of notifier messages to be sent
        #result[1] contains the json text response
        result = func_mappings[specific_intent](intents)
        for msg in result[0]:
            await push_to_notifier(msg)
        await push_to_notifier("SpeechControl: Start")
        return result[1]
    return text_response("ERROR")