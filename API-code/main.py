from fastapi import FastAPI
from starlette.requests import Request
from starlette.websockets import WebSocket, WebSocketDisconnect
from starlette.templating import Jinja2Templates
from starlette.staticfiles import StaticFiles
from starlette.responses import FileResponse
from pydantic import BaseModel
import json
from copy import deepcopy
from pytube import YouTube 

app = FastAPI()

templates = Jinja2Templates(directory="templates")
app.mount("/static", StaticFiles(directory="static"), name="static")

logs = ""
videoUrl = ""

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

def uploadVideo(url):
    yt = YouTube(url)
    yt.streams.filter(mime_type="video/mp4", res="720p", progressive=True)[0].download(output_path="./static", filename="video")

@app.get("/")
async def root(request: Request):
    return templates.TemplateResponse("index.html", {"request": request})

async def push_to_notifier(text):
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

@app.get("/api/v1/video")
def get_video():
    global videoUrl
    return videoUrl

@app.delete("/api/v1/video")
def reset_video():
    global videoUrl
    videoUrl = ""
    return ""

@app.get("/companyVideo")
async def video():
    response = FileResponse("./static/video.mp4")
    return response

@app.post('/videoPost')
async def video_post(video:str):
    await uploadVideo(video)

@app.post('/videoControl')
async def video_control(control:str):
    await notifier.push(f"{control}")

####################
# Alexa skills API #
####################

json_response_template = {
    "version":"1.0",
    # "sessionAttributes": {

    # },
    "response":{
        "outputSpeech":{
            #https://developer.amazon.com/docs/custom-skills/request-and-response-json-reference.html#outputspeech-object
            "type":"PlainText",
        #    "text":"text to output from alexa",
            "playBehaviour":"REPLACE_ENQUEUED"
        },
    #  "reprompt": {
    #   "outputSpeech": {
    #     "type": "PlainText",
    #     "text": "Plain text string to speak",
    #     "playBehavior": "REPLACE_ENQUEUED"             
    #   }
    # },
        # "directives": [
        # ],
        "shouldEndSession": False,
    }
}

class AlexaRequest(BaseModel):
    version: str
    session: dict
    context: dict
    request: dict

@app.post('/api/v1/blueassistant')
async def blue_assistant(baseRequest: AlexaRequest):
    if baseRequest.request.get("type") == "LaunchRequest":
        return text_response("Hi, welcome to Blue, your personal lab assistant. How may I help you today?")
    elif baseRequest.request.get("type") == "SessionEndedRequest":
        return text_response("")
    print(baseRequest.request)
    intents = baseRequest.request.get("intent")
    specific_intent = intents.get("name")
    if specific_intent == "AMAZON.PauseIntent":
        await video_control_intent("Pause")
        return text_response("")
    elif specific_intent == "AMAZON.ResumeIntent":
        await video_control_intent("Resume")
        return text_response("")
    elif specific_intent == "AMAZON.FallbackIntent":
        response = deepcopy(json_response_template)
        response["response"]["outputSpeech"]["text"] = ""
        response["response"]["repromt"] = {"outputSpeech":""}
        return response
    elif specific_intent == "StopVideoIntent":
        await video_control_intent("Stop")
        return text_response("")
    elif specific_intent == "AMAZON.StopIntent":
        await video_control_intent("Stop")
        await notifier.push("Idle")
        r = text_response("Goodbye")
        r["response"]["shouldEndSession"] = True
        return r
    elif specific_intent == "AMAZON.FallbackIntent":
        return text_response("Sorry I did not understand that, please try again")
    slots = intents.get("slots")
    company = slots.get("Company").get("value").lower().replace(" ", "")
    try:
        sector = slots.get("Sector").get("resolutions").get("resolutionsPerAuthority")[0].get("values")[0].get("value").get("name")
    except (TypeError, AttributeError) as e:
        sector = None
    if specific_intent == "CompanyInfoIntent":
        return company_info_intent(company, sector)
    elif specific_intent == "CompanyVideoIntent":
        response = company_video_intent(company, sector)
        if "I couldn't find a video for that" not in response["response"]["outputSpeech"]["text"]:
            await video_control_intent("Playing video")
        return response

def text_response(text):
    response = deepcopy(json_response_template)
    response["response"]["outputSpeech"]["text"] = text
    return response

async def video_control_intent(intent_type):
    await notifier.push(f"{intent_type}")

def company_info_intent(company, sector):
    response = deepcopy(json_response_template)
    with open("companyInfo.json", "r") as file:
        f = json.load(file)
        companyInfo = f.get(company)
    if companyInfo is None:
        return text_response("Sorry I could not recognise that company, please try again")
    if sector is None:
        response["response"]["outputSpeech"]["text"] = companyInfo.get("about")[0]
    elif sector in companyInfo.keys():
        response["response"]["outputSpeech"]["text"] = companyInfo.get(sector)[0] 
    return response

def company_video_intent(company, sector):
    response = deepcopy(json_response_template)
    with open("companyInfo.json", "r") as file:
        f = json.load(file)
        companyVideos = f.get(company).get("videos")
    outputSpeech = "Playing "
    global videoUrl
    if sector is None:
        #outputSpeech = outputSpeech + companyVideos.get("about").get("title")
        videoUrl = companyVideos.get("about").get("url")
        uploadVideo(videoUrl)
    else:
        if sector in companyVideos.keys():
            #outputSpeech = outputSpeech + companyVideos.get(sector).get("title")
            videoUrl = companyVideos.get(sector).get("url")
            uploadVideo(videoUrl)
        else:
            outputSpeech = "I couldn't find a video for that"
    response["response"]["outputSpeech"]["text"] = outputSpeech
    return response
