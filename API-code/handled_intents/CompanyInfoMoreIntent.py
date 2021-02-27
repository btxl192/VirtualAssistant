from .intent_base import *
import json

class CompanyInfoMoreIntent(intent_base):
    def get_company_info(self, company):
            with open("companyInfo.json", "r") as file:
                f = json.load(file)
                return f.get(company)
            
    def action(self, handler_input):  
        company = get_slot_value(handler_input, "Company").lower().replace(" ", "")
        company_info = self.get_company_info(company)
        if company_info is None:
            if "current_company" in get_sess_attr(handler_input):
                company_info = self.get_company_info(get_sess_attr(handler_input)["current_company"])
            else:
                self.response = "Sorry I could not recognise that company."
                return
        self.response = company_info.get("design")[0]