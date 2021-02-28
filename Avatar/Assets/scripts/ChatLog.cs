using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class ChatLog : MonoBehaviour
{
    public Text chatlogobj;
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
        chatlogobj.text = string.Join("\n", chatlog);
    }

    private void OnApplicationQuit()
    {
        WebsocketHandler.MessageReceived -= HandleMsg;
    }
}
