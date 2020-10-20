import requests
from bs4 import BeautifulSoup
from getpass import getpass
from selenium import webdriver
import time
import json

class ApiHandler:
    username = ""
    password = ""
    base_url = "https://alexa.amazon.co.uk"
    headers = {"User-Agent": "Mozilla/5.0",}
    
    def __init__(self):
        super().__init__()
        self.session = requests.Session()

    def get(self, url):
        output = ""
        try:
            r = self.session.get(self.base_url + url, headers=self.headers)
            if r.status_code == 200:
                output = r.text
        except:
            print("error in getting " + self.base_url + url)
        return output
    
    def login(self):
        try:
            output = self.get("")
            print(self.session.cookies)
            print("-----------------------------------------------------------------------")
            soup = BeautifulSoup(output, "html.parser")
            forms = soup.find_all("form")
            action = forms[0].get("action")
            if action is None:
                return False
            print(action)
            inputs = soup.find_all("input")
            self.username = input("Username: ")
            self.password = getpass()
            payload = {
                "email": self.username,
                "password": self.password,
                "create": "0",
            }
          
            for i in inputs:
                if i.get("type") == "hidden":
                    print(i)
                    payload.update({i.get("name"): i.get("value")}) 
            
            r = self.session.post(action, data=payload, headers=self.headers)
            print(r.text)
            print("login successful")
            print(api.get("/api/cards"))
            return True
        except:
            print("login unsuccessful")
            return False

            
if __name__ == "__main__":
    api = ApiHandler()
    # api.login()

    chrome_webdriver = webdriver.Chrome()
    chrome_webdriver.get("https://alexa.amazon.co.uk")
    x = input("Please login to amazon alexa api, then press enter")
    #print(chrome_webdriver.get_cookies())
    cookies = chrome_webdriver.get_cookies()
    # print(cookies)
    with open("cookies.txt", "w+") as file:
        file.write(str(cookies))
    for cookie in cookies:
        api.session.cookies.set(cookie['name'], cookie['value'])
    # print("-------------------------------------------------------")
    # print(api.session.cookies)    
    prevcookie = ""
    while True:
        curr = api.get("/api/cards?limit=1")
        if curr != prevcookie:
            prevcookie = curr
            #print(curr)
            response = json.loads(curr)
            try:
                card = response['cards'][0]
                print(card['descriptiveText'][0])
            except:
                print(curr)
            #requests.post("http://51.104.238.154:44300/api/v1/speechLogs?text=")

        time.sleep(3)
    
    # if(api.login()):
        # while(True):
        #     output = api.get("/api/cards?limit=1")
        #     print(output)
        
            
            




        