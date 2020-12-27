using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class videoPlayerScript : MonoBehaviour
{

	public string vidUrl {get; set;}
	public bool paused { get; set; }

    private UnityEngine.Video.VideoPlayer videoPlayer;
    private UnityEngine.UI.RawImage rawimage;

    public Animator animator;
    public Animator avatorAnimator;

    void Start()
    {
        paused = true;
        videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        // videoPlayer.url = "https://" + config.domainName + "/companyVideo";
        // videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/e2cac2b3c9d4be3abac3be760b7b9c5e44330f66f9e2d902ef13dc7ea71369e2.webm";
        videoPlayer.url = "https://mw-public-data.s3.eu-west-2.amazonaws.com/93cc49bdf8bc759864e2c64be16e3938b205424fb9490e1814a91211440da690.mp4";
        rawimage = GetComponent<UnityEngine.UI.RawImage>();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            paused = !paused;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            StopVideo();
        }
        if (!paused && !videoPlayer.isPlaying)
        {
            videoPlayer.enabled = true;
            videoPlayer.Play();
            animator.SetBool("stopped", false);
            avatorAnimator.SetBool("videoPlaying", true);
        }
        else if (paused && videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }

        videoPlayer.loopPointReached += EndReached;

        void EndReached(UnityEngine.Video.VideoPlayer vp)
        {
            StopVideo();
        }
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
    }
}
