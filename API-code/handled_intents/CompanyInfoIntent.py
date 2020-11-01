from .intent_base import *
import json

def get_company_info(company):
    with open("companyInfo.json", "r") as file:
        f = json.load(file)
        return f.get(company)

def company_info_intent(company, sector):
    company_info = get_company_info(company)
    if company_info is None:
        return "Sorry I could not recognise that company, please try again"
    if sector is None:
        return company_info["about"][0]
    elif sector in company_info.keys():
        return company_info[sector][0]
    else:
        return "Sorry I could not recognise that sector, please try again"

class CompanyInfoIntent(intent_base):
    def action(self, intents):  
        slots = intents["slots"]
        company = slots["Company"]["value"].lower().replace(" ", "")
        try:
            sector = slots["Sector"]["resolutions"]["resolutionsPerAuthority"][0]["values"][0]["value"]["name"]
        except (TypeError, AttributeError) as e:
            sector = None
    
        output_speech = company_info_intent(company, sector)
        self.push_to_notifier_speech(output_speech)
        self.response = output_speech