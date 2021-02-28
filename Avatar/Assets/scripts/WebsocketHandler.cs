using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class WebsocketHandler : MonoBehaviour
{

    public delegate void MessageReceivedDelegate(JObject msgJson, string msgTitle, string msgText);
    public static event MessageReceivedDelegate MessageReceived;

    void Start()
    {

        HttpPoll pollAlexaSkill = new HttpPoll("https://" + config.domainName + "/msg", 0.1f, HandleMsg);
        StartCoroutine(pollAlexaSkill.poll());

        HttpPoll pollAlexaLocalClient = new HttpPoll("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort + "/msg", 0.1f, HandleMsg);
        StartCoroutine(pollAlexaLocalClient.poll());
        
    }

    void HandleMsg(JObject msgjson)
    {
        List<string> jsonkeys = GetJsonKeys(msgjson);
        foreach (string messageTitle in jsonkeys)
        {
            string messageText = msgjson.Value<string>(messageTitle);
            //print("msg - " + messageTitle + ": " + messageText);
            if (MessageReceived != null)
            {
                //Call the MessageReceived event
                //Event handlers should subscribe to WebsocketHandler.MessageReceived
                //Event handlers must check message titles on their own
                MessageReceived(msgjson, messageTitle, messageText);
            }
        }
    }

    public static List<string> GetJsonKeys(JObject msgjson)
    {
        return msgjson.Properties().Select(p => p.Name).ToList();
    }
}
