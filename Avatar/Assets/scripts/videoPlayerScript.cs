using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class videoPlayerScript : MonoBehaviour
{
	
	public string vidUrl {get; set;}
	public bool paused { get; set; }

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.UI.RawImage rawimage;

    void Start()
    {
        paused = true;
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.url = "https://" + config.domainName + "/companyVideo";
        rawimage = GetComponent<UnityEngine.UI.RawImage>();
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }
        if (!paused && !videoPlayer.isPlaying)
        {
            videoPlayer.enabled = true;
            rawimage.enabled = true;
            videoPlayer.Play();
        }
        else if (paused && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

        videoPlayer.loopPointReached += EndReached;

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            StopVideo(vp);
        }
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
        videoPlayer.enabled = false;
        rawimage.enabled = false;
    }

    private void StopVideo(UnityEngine.Video.VideoPlayer vp)
    {
        vp.Stop();
        videoPlayer.enabled = false;
        rawimage.enabled = false;
    }

    private void OnApplicationQuit()
    {
        videoPlayer.Stop();
    }
}
