using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class NavigationScript : MonoBehaviour
{

	public string vidPath {get; set;}
	public bool paused { get; set; }

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.UI.RawImage rawimage;

    public Animator animator;
    public Animator avatorAnimator;

    public float slideInProg = 0.0f;

    private DateTime timeLastPlayed;
    private string navigationDir;

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        paused = true;
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url = "file:///home/kalcho100/Documents/UCL/Year2/COMP0016_20-21/VirtualAssistant/Navigation/1/ff.webm";
        rawimage = GetComponent<UnityEngine.UI.RawImage>();
        // videoPlayer.enabled = true;
        // videoPlayer.Play();
        // animator.SetBool("stopped", false);
        // avatorAnimator.SetBool("videoPlaying", true);
        // timeLastPlayed = DateTime.Now;
        string path = Directory.GetCurrentDirectory();
        char sep = Path.DirectorySeparatorChar;
        navigationDir = path + sep + ".." + sep + "Navigation";
        if (Directory.Exists(navigationDir)) {
            string [] subdirectoryEntries = Directory.GetDirectories(navigationDir);
            foreach(string subdirectory in subdirectoryEntries)
                Debug.Log(subdirectory);
        }
    }

    private void Update()
    {
        // if (!paused && !videoPlayer.isPlaying)
        // {
        //     videoPlayer.enabled = true;
        //     videoPlayer.Play();
        //     animator.SetBool("stopped", false);
        //     avatorAnimator.SetBool("videoPlaying", true);
        // }
        // else if (paused && videoPlayer.isPlaying)
        // {
        //     videoPlayer.Pause();
        // }

        // DateTime now = DateTime.Now;
        // double seconds = (now - timeLastPlayed).TotalSeconds;
        // if (seconds >= 7) {
        //     StopVideo();
        // }

        // RectTransform rt = GetComponent<RectTransform>();
        // float ratio = 16.0f / 9.0f;
        // float vid_width = Screen.width * 0.8f;
        // rt.sizeDelta = new Vector2(vid_width, vid_width / ratio);
        // rt.anchoredPosition = new Vector2((1-slideInProg)*Screen.width + Screen.width * 0.08f, 0);
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        paused = true;
        animator.SetBool("stopped", true);
        avatorAnimator.SetBool("videoPlaying", false);
    }

    private void OnApplicationQuit()
    {
        videoPlayer.Stop();
        WebsocketHandler.MessageReceived -= HandleMsg;
    }

    private void HandleMsg(JObject msgjson, string msgtitle, string msgtext)
    {
        if (msgtitle.Equals("NavRoom"))
        {
            string room = msgtext;

        }

    }
}
