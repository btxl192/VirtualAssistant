from .intent_base import *
import json

class CompanyInfoIntent(intent_base):
    def get_company_info(self, company):
        with open("companyInfo.json", "r") as file:
            f = json.load(file)
            return f.get(company)

    def company_info_intent(self, company, sector):
        company_info = self.get_company_info(company)
        if company_info is None:
            self.emotion = "ASHAMED"
            return "Sorry I could not recognise that company, please try again"
        if sector is None:
            return company_info.get("about")[0]
        elif sector in company_info.keys():
            return company_info.get(sector)[0]
        else:
            self.emotion = "ASHAMED"
            return "Sorry I could not recognise that sector, please try again"

    def action(self, handler_input):  
        #slots = handler_input.request_envelope.request.intent.to_dict().get("slots")        
        #company = slots.get("Company").get("value").lower().replace(" ", "")
        company = get_slot_value(handler_input, "Company").lower().replace(" ", "")
        try:
            #sector = slots.get("Sector").get("resolutions").get("resolutionsPerAuthority")[0].get("values")[0].get("value").get("name")
            sector = get_slot_value(handler_input, "Sector")
        except (TypeError, AttributeError) as e:
            sector = None
    
        self.response = self.company_info_intent(company, sector) + "What else would you like to know about " + company + "?"
        self.user_input = "Asked for info about " + company
        
        set_dismissal_msg(handler_input, "No more questions about " + company + " then.")
        add_answer_intent(handler_input, "CompanyInfoMoreIntent")
        set_sess_attr(handler_input, "current_company", company)