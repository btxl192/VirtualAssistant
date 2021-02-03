using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EngineIOSharp.Common.Enum;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
using Newtonsoft.Json.Linq;
using System.Linq;

public class WebsocketHandler : MonoBehaviour
{

    private videoPlayerScript videoPlayer;
    private player thisplayer;
    private LipSync thislipsync;
    private Emotion thisemotion;
    private SocketIOClient alexaWs;
    private SocketIOClient localWs;

    void Start()
    {
        thisplayer = GetComponent<player>();
        thislipsync = GetComponent<LipSync>();
        thisemotion = GetComponent<Emotion>();

        videoPlayer = GameObject.Find("Video Player").GetComponent<videoPlayerScript>();

        alexaWs = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.https, config.domainName, config.vmPort));
        alexaWs.On(SocketIOEvent.CONNECTION, () => { print(config.domainName + "ws Connected!"); });
        alexaWs.On(SocketIOEvent.DISCONNECT, () => { print(config.domainName + "ws Disconnected!"); });
        alexaWs.On(SocketIOEvent.ERROR, WsHandleError);
        alexaWs.On("message", (Data) => // Argument can be used without type.
        {
            if (Data != null && Data.Length > 0 && Data[0] != null)
            {
                //print(config.domainName + "ws Message : " + Data[0]); //debug message
                HandleMsg(Data[0].ToString());
            }
        });

        localWs = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, config.alexaResponseIP, config.alexaResponsePort));
        localWs.On(SocketIOEvent.CONNECTION, () => { print(config.alexaResponseIP + "ws Connected!"); });
        localWs.On(SocketIOEvent.DISCONNECT, () => { print(config.alexaResponseIP + "ws Disconnected!"); });
        localWs.On(SocketIOEvent.ERROR, WsHandleError);
        localWs.On("message", (Data) => // Argument can be used without type.
        {
            if (Data != null && Data.Length > 0 && Data[0] != null)
            {
                //print(config.alexaResponseIP + "ws Message : " + Data[0]); //debug message
                HandleMsg(Data[0].ToString());
            }
        });

        localWs.Connect();
        alexaWs.Connect();
    }

    void WsHandleError(JToken[] Data)
    {
        if (Data != null && Data.Length > 0 && Data[0] != null) { print("Error : " + Data[0]); }
        else { print("Unkown Error"); }
    }

    void HandleMsg(string jsonmsgfull)
    {
        JObject msgjson = JObject.Parse(jsonmsgfull);
        List<string> jsonkeys = msgjson.Properties().Select(p => p.Name).ToList();
        foreach (string messageTitle in jsonkeys)
        {
            string messageText = msgjson.Value<string>(messageTitle);

            switch (messageTitle)
            {
                case "VidControl":
                    VidControl(messageText);
                    break;
                case "Speech":
                    thislipsync.hasEmotion = jsonkeys.Contains("Emotion");
                    thislipsync.lipsync(messageText);
                    break;
                case "SpeechControl":
                    if (messageText.ToLower().Equals("written"))
                    {
                        thislipsync.getAudio = true;
                    }
                    break;
                case "AlexaResponse":
                    print("GOT ALEXA RESPONSE: " + jsonmsgfull);
                    break;
                case "UserInput":
                    print("user input: " + jsonmsgfull);
                    break;
                case "Emotion":
                    if (thisemotion != null)
                    {
                        GetComponent<Emotion>().SetEmotion(messageText);
                    }
                    break;
                case "SpeechUrl":
                    thislipsync.speechurl = messageText;
                    break;
                default:
                    Debug.LogWarning("Unhandled control message:    " + jsonmsgfull);
                    break;
            }
        }     

    }

    void VidControl(string msg)
    {
        switch (msg)
        {
            case "Speaking":
                thisplayer.isTalking = true;
                thisplayer.stopTalking = false;
                break;
            case "Play":
                //Play video hosted on /companyVideo
                //videoPlayer.PlayVideo(true);
                videoPlayer.paused = false;
                break;
            case "Pause":
                //Pause video
                //videoPlayer.PauseVideo(true);
                videoPlayer.paused = true;
                break;
            case "Resume":
                //Resume video
                //videoPlayer.ResumeVideo(true);
                break;
            case "Stop":
                //Stop video
                //videoPlayer.StopVideo(true);
                videoPlayer.StopVideo();
                break;
            case "Idle":
                thisplayer.isTalking = false;
                thisplayer.stopTalking = true;
                break;
            default:
                thisplayer.isTalking = false;
                thisplayer.stopTalking = false;
                break;
        }
    }

    private void OnApplicationQuit()
    {
        localWs.Dispose();
        alexaWs.Dispose();
    }
}
