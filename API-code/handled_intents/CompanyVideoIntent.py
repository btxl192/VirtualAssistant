from .intent_base import *
import json
from pytube import YouTube

def uploadVideo(url):
    yt = YouTube(url)
    yt.streams.filter(mime_type="video/mp4", res="720p", progressive=True)[0].download(output_path="./static", filename="video")

def get_company_info(company):
    with open("companyInfo.json", "r") as file:
        f = json.load(file)
        return f.get(company)

class CompanyVideoIntent(intent_base):
    videoUrl = ""
    
    def company_video_intent(self, company, sector):
        company_videos = None
        company_info = get_company_info(company)
        output_speech = ""
        if company_info != None:
            company_videos = company_info.get("videos")
            output_speech = "Playing "
            if company_videos is None:
                output_speech = "I couldn't find a video for that"
            else:
                if sector is None:
                    self.videoUrl = company_videos.get("about").get("url")
                else:
                    if sector in company_videos.keys():
                        #outputSpeech = outputSpeech + companyVideos.get(sector).get("title")
                        self.videoUrl = company_videos.get(sector).get("url")
                    else:
                        output_speech = "I couldn't find a video for that"
        else:
            output_speech = "I couldn't recognise that company"
        return output_speech

    def action(self, handler_input):
        #slots = intents.to_dict().get("slots")
        #company = slots.get("Company").get("value").lower().replace(" ", "")
        company = get_slot_value(handler_input, "Company")
        try:
            #sector = slots.get("Sector").get("resolutions").get("resolutionsPerAuthority")[0].get("values")[0].get("value").get("name")
            sector = get_slot_value(handler_input, Sector)
        except (TypeError, AttributeError) as e:
            sector = None

        self.response = self.company_video_intent(company, sector)
        
        if "couldn't" not in self.response:
            uploadVideo(self.videoUrl)
            self.add_unity_msg("VidControl", "Play")
            self.response = "Playing video"
        self.user_input = "Asked to play a video about " + company