using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class WebsocketHandler : MonoBehaviour
{

    public delegate void MessageReceivedDelegate(JObject msgJson, string msgTitle, string msgText);
    public static event MessageReceivedDelegate MessageReceived;

    private MsgPriorityComparer msgcomp = new MsgPriorityComparer();

    //higher priority is processed first
    public static Dictionary<string, int> msgPriority = new Dictionary<string, int>()
    {
        { "UserInput", 1}
    };

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
        //sort keys according to priority
        jsonkeys.Sort(msgcomp);
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
    public class MsgPriorityComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
        {
            int xprior = -1;
            int yprior = -1;
            if (WebsocketHandler.msgPriority.ContainsKey(x))
            {
                xprior = msgPriority[x];
            }
            if (WebsocketHandler.msgPriority.ContainsKey(y))
            {
                yprior = msgPriority[y];
            }
            return yprior.CompareTo(xprior);
        }
    }
}
