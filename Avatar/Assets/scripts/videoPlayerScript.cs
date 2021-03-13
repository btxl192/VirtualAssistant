using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class videoPlayerScript : MonoBehaviour
{

	public string vidUrl {get; set;}
	public bool paused;

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.UI.RawImage rawimage;

    public Animator animator;
    public Animator avatorAnimator;

    public float slideInProg = 0.0f;

    private bool videoPlayerStarted = false;

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        paused = true;
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        // videoPlayer.url = "https://" + config.domainName + "/companyVideo";
        // videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/e2cac2b3c9d4be3abac3be760b7b9c5e44330f66f9e2d902ef13dc7ea71369e2.webm";
        videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/93cc49bdf8bc759864e2c64be16e3938b205424fb9490e1814a91211440da690.mp4";
        rawimage = GetComponent<UnityEngine.UI.RawImage>();

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
            paused = !paused;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            StopVideo();
        }
        if (paused) {
            videoPlayerStarted = false;
            animator.SetBool("stopped", true);
            avatorAnimator.SetBool("videoPlaying", false);
        } else {
            animator.SetBool("stopped", false);
            avatorAnimator.SetBool("videoPlaying", true);
        }
        RectTransform rt = GetComponent<RectTransform>();
        float ratio = 16.0f / 9.0f;
        float vid_width = Screen.width * 0.8f;
        rt.sizeDelta = new Vector2(vid_width, vid_width / ratio);
        rt.anchoredPosition = new Vector2((1-slideInProg)*Screen.width + Screen.width * 0.08f, 0);
        if (!paused && !videoPlayerStarted)
        {
            videoPlayer.enabled = true;
            videoPlayer.Play();
            videoPlayerStarted = true;
            Debug.Log("Started");
        }
        if (paused && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

    }

    public void StopVideo()
    {
        Debug.Log("Stopped");
        videoPlayer.Stop();
        paused = true;
    }

    private void OnApplicationQuit()
    {
        videoPlayer.Stop();
        WebsocketHandler.MessageReceived -= HandleMsg;
    }

    private void HandleMsg(JObject msgjson, string msgtitle, string msgtext)
    {
        if (msgtitle.Equals("VidControl"))
        {
            switch (msgtext)
            {
                case "Play":
                    //Play video hosted on /companyVideo
                    //videoPlayer.PlayVideo(true);
                    paused = false;
                    break;
                case "Pause":
                    //Pause video
                    //videoPlayer.PauseVideo(true);
                    paused = true;
                    break;
                case "Resume":
                    //Resume video
                    //videoPlayer.ResumeVideo(true);
                    break;
                case "Stop":
                    //Stop video
                    //videoPlayer.StopVideo(true);
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
        } else if (msgtitle.Equals("VidUrl")) {
            videoPlayer.url = msgtext;
            // videoPlayer.url = "file://" + Application.dataPath + "/video/nttdata-about.mp4";
        }

    }
}
