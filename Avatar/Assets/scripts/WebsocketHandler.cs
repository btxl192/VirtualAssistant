using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;

public class WebsocketHandler : MonoBehaviour
{

    private videoPlayerScript videoPlayer;
    private player thisplayer;
    private LipSync thislipsync;
    private Emotion thisemotion;

    void Start()
    {
        
        thisplayer = GetComponent<player>();
        thislipsync = GetComponent<LipSync>();
        thisemotion = GetComponent<Emotion>();

        videoPlayer = GameObject.Find("Video Player").GetComponent<videoPlayerScript>();

        HttpPoll pollAlexaSkill = new HttpPoll("https://" + config.domainName + "/msg", 0.1f, HandleMsg);
        StartCoroutine(pollAlexaSkill.poll());

        HttpPoll pollAlexaLocalClient = new HttpPoll("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort + "/msg", 0.1f, HandleMsg);
        StartCoroutine(pollAlexaLocalClient.poll());
        
    }

    void HandleMsg(JObject msgjson)
    {
        HandleMsgHelper(msgjson);
    }

    void HandleMsg(string jsonmsgfull)
    {
        JObject msgjson = JObject.Parse(jsonmsgfull);
        HandleMsgHelper(msgjson);

    }

    void HandleMsgHelper(JObject msgjson)
    {
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
                    print("GOT ALEXA RESPONSE: " + msgjson);
                    break;
                case "UserInput":
                    print("user input: " + msgjson);
                    break;
                case "Emotion":
                    if (thisemotion != null)
                    {
                        thisemotion.SetEmotion(messageText);
                    }
                    break;
                case "SpeechUrl":
                    thislipsync.speechurl = messageText;
                    break;
                default:
                    Debug.LogWarning("Unhandled control message:    " + msgjson);
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
}
