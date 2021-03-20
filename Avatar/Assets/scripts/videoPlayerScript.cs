using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class videoPlayerScript : MonoBehaviour
{

	public string vidUrl {get; set;}

    private UnityEngine.Video.VideoPlayer videoPlayer;

    public Animator animator;
    public Animator avatorAnimator;

    public float slideInProg = 0.0f;

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        // videoPlayer.url = "https://" + config.domainName + "/companyVideo";
        // videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/e2cac2b3c9d4be3abac3be760b7b9c5e44330f66f9e2d902ef13dc7ea71369e2.webm";
        videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/93cc49bdf8bc759864e2c64be16e3938b205424fb9490e1814a91211440da690.mp4";

        videoPlayer.loopPointReached += EndReached;

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            Debug.Log("end reached.");
            StopVideo();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (videoPlayer.isPlaying)
            {
                PauseVideo();
            }
            else
            {
                ResumeVideo();
            }
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            StopVideo();
        }

        RectTransform rt = GetComponent<RectTransform>();
        float ratio = 16.0f / 9.0f;
        float vid_width = Screen.width * 0.8f;
        rt.sizeDelta = new Vector2(vid_width, vid_width / ratio);
        rt.anchoredPosition = new Vector2((1-slideInProg)*Screen.width + Screen.width * 0.08f, 0);
    }

    public void ResumeVideo()
    {
        videoPlayer.Play();
        animator.SetBool("stopped", false);
        avatorAnimator.SetBool("videoPlaying", true);
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        animator.SetBool("stopped", true);
        avatorAnimator.SetBool("videoPlaying", false);
    }

    private void OnApplicationQuit()
    {
        videoPlayer.Stop();
        WebsocketHandler.MessageReceived -= HandleMsg;
    }

    public void HandleMsg(JObject msgjson, string msgtitle, string msgtext)
    {
        if (msgtitle.Equals("VidControl"))
        {
            switch (msgtext)
            {
                case "Play":
                    videoPlayer.enabled = true;
                    ResumeVideo();
                    break;
                case "Pause":
                    PauseVideo();
                    break;
                case "Resume":
                    ResumeVideo();
                    break;
                case "Stop":
                    StopVideo();
                    break;
                case "Idle":
                    //thisplayer.isTalking = false;
                    //thisplayer.stopTalking = true;
                    break;
                default:
                    //thisplayer.isTalking = false;
                    //thisplayer.stopTalking = false;
                    break;
            }
        }
        else if (msgtitle.Equals("VidUrl"))
        {
            if (videoPlayer.url != msgtext)
            {
                videoPlayer.url = msgtext;
            }

            // videoPlayer.url = "file://" + Application.dataPath + "/video/nttdata-about.mp4";
        }

    }
}
