using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;

public class WebsocketHandler : MonoBehaviour
{

    private WebSocket websocket;
    private videoPlayerScript videoPlayer;
    private player thisplayer;
    private LipSync thislipsync;

    async void Start()
    {
        thisplayer = GetComponent<player>();
        thislipsync = GetComponent<LipSync>();

        //videoPlayer = GameObject.Find("Video Player").GetComponent<videoPlayerScript>();

        websocket = new WebSocket("wss://" + config.domainName + "/ws");
        websocket.OnOpen += () => { Debug.Log("Connection open!"); };
        websocket.OnError += (e) => { Debug.LogError("Error! " + e); };
        websocket.OnClose += (e) => { Debug.LogWarning("Connection closed!"); };

        websocket.OnMessage += (bytes) =>
        {
            // getting the message as a string
            string line = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("SocketMessage! " + line);
            HandleMsg(line);
        };

        // waiting for messages
        await websocket.Connect();
    }

    void HandleMsg(string msg)
    {
        string control = msg.Split(':')[0].Trim();
        string text = msg.Split(':')[1].Trim();

        switch (control)
        {
            case "VidControl":
                VidControl(text);
                break;
            case "Speech":
                thislipsync.delayText(text);
                thislipsync.lipsync(text);
                break;
            case "SpeechControl":
                if (text.Equals("Start"))
                    thislipsync.StartLipSync();
                break;
            default:
                Debug.LogWarning("Unhandled control message:    " + msg);
                break;
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

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
