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
    private string room;
    private int blueFloor;
    private char sep;
    private bool onCurrentFloor;
    private int secondFloor;
    private bool isDisplayed;

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        isDisplayed = false;
        secondFloor = -100;
        onCurrentFloor = true;
        paused = true;
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        rawimage = GetComponent<UnityEngine.UI.RawImage>();

        string path = Directory.GetCurrentDirectory();
        sep = Path.DirectorySeparatorChar;
        navigationDir = path + sep + ".." + sep + "Navigation";
        room = "-";
        string text = System.IO.File.ReadAllText(navigationDir + sep + "Blue.json");
        char[] chars = text.ToCharArray();
        foreach (char ch in chars) {
            if (Char.IsDigit(ch)){
                blueFloor = int.Parse(ch.ToString());
                break;
            }
        }
    }

    private void Update()
    {
        if (videoPlayer.isPlaying && isDisplayed) {
            DateTime now = DateTime.Now;
            double seconds = (now - timeLastPlayed).TotalSeconds;
            if (seconds >= 5) {
                if (onCurrentFloor) {
                    StopVideo();
                    room = "-";
                    isDisplayed = false;
                } else {
                    videoPlayer.Stop();
                    string url = navigationDir + sep + secondFloor.ToString() + sep + room + ".webm";
                    videoPlayer.url = "file://" + url;
                    videoPlayer.enabled = true;
                    videoPlayer.Play();
                    timeLastPlayed = DateTime.Now;
                    onCurrentFloor = true;
                    secondFloor = -100;
                }
            }
        }
        else {
            if (secondFloor != -100) {
                if (!room.Equals("-")){
                    string url = navigationDir + sep + blueFloor.ToString() + sep + "lift.webm";
                    if(File.Exists(url)) {
                        videoPlayer.url = "file://" + url;
                        videoPlayer.enabled = true;
                        ResumeVideo();
                        timeLastPlayed = DateTime.Now;
                        onCurrentFloor = false;
                        isDisplayed = true;
                    }
                }
            }
            else {
                if (!room.Equals("-")){
                    string url = navigationDir + sep + blueFloor.ToString() + sep + room + ".webm";
                    if(File.Exists(url)) {
                        videoPlayer.url = "file://" + url;
                        videoPlayer.enabled = true;
                        ResumeVideo();
                        timeLastPlayed = DateTime.Now;
                        onCurrentFloor = true;
                        isDisplayed = true;
                    }
                }
            }
        }
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
        animator.SetBool("stopped", true);
        avatorAnimator.SetBool("videoPlaying", false);
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
            room = msgtext;
        }
        if (msgtitle.Equals("NavFloor"))
        {
            secondFloor = Int32.Parse(msgtext);
            onCurrentFloor = false;
        }    
    }
}