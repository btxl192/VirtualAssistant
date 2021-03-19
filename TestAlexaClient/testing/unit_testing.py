from selenium import webdriver
from selenium.webdriver.common.keys import Keys
import time

def log(s):
    print(s)
    logs = open("./logs.txt", "a")
    logs.write(str(s) + "\n")
    logs.close()

def find_elem(chrome_webdriver, elem_class):
    while True:
        try:
            e = chrome_webdriver.find_elements_by_class_name(elem_class)
            if len(e) > 0:
                return e
        except:
            time.sleep(0.1)

def test(inp):
    chrome_options = webdriver.ChromeOptions()
    chrome_options.add_argument("--mute-audio")
    chrome_options.add_argument('--no-sandbox')
    chrome_webdriver = webdriver.Chrome(chrome_options=chrome_options)
    chrome_webdriver.get("https://developer.amazon.com/alexa/console/ask/test/amzn1.ask.skill.fa7cfeb1-e524-4024-a258-5249bec81e5f/development/en_GB/")
    chrome_webdriver.minimize_window()

    username = chrome_webdriver.find_element_by_id("ap_email")
    password = chrome_webdriver.find_element_by_id("ap_password")
    sign_in_button = chrome_webdriver.find_element_by_id("signInSubmit")

    login_details = open("../login.txt").read().splitlines()
    username.send_keys(login_details[0])
    password.send_keys(login_details[1])

    webdriver.ActionChains(chrome_webdriver).click(sign_in_button).perform()

    input_text_box = find_elem(chrome_webdriver, "askt-utterance__input")[0]

    for i in range(len(inp)):
        msgs = len(find_elem(chrome_webdriver, "askt-dialog__message"))
        
        input_text_box.send_keys(inp[i][0])
        input_text_box.send_keys(Keys.RETURN)       
        
        while msgs + 1 >= len(find_elem(chrome_webdriver, "askt-dialog__message")):
            pass

        msg_boxes = find_elem(chrome_webdriver, "askt-dialog__message")

        msgs = len(msg_boxes)
        
        output_text_box = msg_boxes[-1] 
        
        test_passed = output_text_box.text.lower().strip() == inp[i][1].lower().strip()
        
        if not test_passed:
            log(f"Test FAILED for '{inp}'") 
            log(f"=====Test FAILED for entry===== \n'{inp[i]}'\n=====got output===== \n'{output_text_box.text}'\n=====BUT expected=====\n'{inp[i][1]}'")
            chrome_webdriver.close()
            return test_passed

    log(f"Test PASSED for '{inp}'")     
    chrome_webdriver.close()    
    return test_passed

def main():

    ntt_data_about = "NTT DATA can help you navigate today’s world of fast-growing technological complexity, ever-rising customer expectations and rapidly changing business environments. Through innovation, in-depth industry expertise and dedicated onshore and offshore teams of experts, we provide the capabilities, resources and experience to guide your digital development. What else would you like to know about nttdata?"
    ntt_data_design = "NTT Data's Design team love to work on complex large-scale projects, delivering deeply functional tools with powerfully intuitive user experiences. The award-winning digital solutions they create have set new standards since 2005, mixing sound strategy, innovative design, and applied technology."

    test_cases = [
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?")
        ], 
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"), 
            ("play a video about ntt data", "Playing video"),
            ("show me a video about ntt data", "Playing video"),
            ("pause", "pausing video"),
            ("resume", "resuming video"),
            ("stop video", "stopped video")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("cancel", "goodbye")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("help", "Try asking about the company or play a video")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("close blue assistant", "goodbye")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("what is ntt data", ntt_data_about),
            ("what does ntt data do", ntt_data_about),
            ("tell me about ntt data", ntt_data_about),
            ("i would like to know more about ntt data", ntt_data_about),
            ("what's ntt data", ntt_data_about),
            ("what does ntt data work on", ntt_data_design),
            ("where is aousnhfola", "No more questions about nttdata then. Take the second corridor to your right. In the end turn right and it will be right in front of you.")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("translate apple to french", "Pomme.")
        ],
        [
            ("open blue assistant", "Hi, welcome to Blue, your personal lab assistant. How may I help you today?"),
            ("where is osidanhbvaol", "Take the second corridor to your right. In the end turn right and it will be right in front of you.")
        ]
    ]

    num_tests = len(test_cases)
    num_passed = 0

    starting_test = 0

    try:
        f = open("./testcount.txt", "r")
        starting_test = int(f.readline()) - 1
        num_passed = int(f.readline())
    except:
        f = open("./logs.txt", "w")
        f.close()

    for i in range(starting_test, len(test_cases)):
        log(f"=================================Running test ({i + 1} / {num_tests})=================================")
        f = open("./testcount.txt", "w")
        f.write(str(i + 1) + "\n")
        f.close()
        if test(test_cases[i]):
            num_passed += 1
            f = open("./testcount.txt", "a")
            f.write(str(num_passed))
            f.close()

    f = open("./testcount.txt", "w")
    f.write("a\n0")
    f.close()

    log(f"Tests passed ({num_passed} / {num_tests}) [{num_passed/num_tests * 100}%]")

if __name__ == "__main__":
    main()