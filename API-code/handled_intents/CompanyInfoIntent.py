from .intent_base import *
import json

def get_company_info(company):
    with open("companyInfo.json", "r") as file:
        f = json.load(file)
        return f.get(company)

def company_info_intent(company, sector):
    company_info = get_company_info(company)
    if company_info is None:
        return text_response("Sorry I could not recognise that company, please try again")
    if sector is None:
        return text_response(company_info.get("about")[0])
    elif sector in companyInfo.keys():
        return text_response(company_info.get(sector)[0])
    else:
        return text_resposne("Sorry I could not recognise that sector, please try again")

class CompanyInfoIntent(intent_base):
    def action(self, intents):
    
        slots = intents.get("slots")
        company = slots.get("Company").get("value").lower().replace(" ", "")
        try:
            sector = slots.get("Sector").get("resolutions").get("resolutionsPerAuthority")[0].get("values")[0].get("value").get("name")
        except (TypeError, AttributeError) as e:
            sector = None
    
        r = company_info_intent(company, sector)
        if r != None:
            t = r["response"]["outputSpeech"]["text"]
            self.notif.append(f"Speech: {t}")
            self.response = r
        else:
            t = "Sorry I could not recognise that company, please try again"
            self.notif.append(f"Speech: {t}")
            self.response = text_response(t)