using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class ChatLog : MonoBehaviour
{

    private List<string> chatlog;

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        chatlog = new List<string>();
    }

    void HandleMsg(JObject msgjson, string msgtitle, string msgtext)
    {
        switch(msgtitle)
        {
            case "UserInput":
                chatlog.Add("User: " + msgtext);
                break;
            case "AlexaResponse":
                chatlog.Add("Alexa: " + msgtext);
                break;
        }
    }

    private void OnApplicationQuit()
    {
        WebsocketHandler.MessageReceived -= HandleMsg;
    }
}
