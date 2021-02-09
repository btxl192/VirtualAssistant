using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orientationControl : MonoBehaviour
{
    private int height, width;
    private GameObject landscapeCamera;
    private GameObject portraitCamera;
    // Start is called before the first frame update
    void Awake()
    {
        portraitCamera = GameObject.Find("PortraitCamera");
        landscapeCamera = GameObject.Find("LandscapeCamera");
        height = Screen.currentResolution.height;
        width = Screen.currentResolution.width;
        //Screen.SetResolution(width, height, true);
        //Screen.SetResolution(height, width, false);
        if (height > width) //portrait
        {
            /*Debug.Log("portrait");
            landscapeCamera.SetActive(false);
            portraitCamera.SetActive(true);*/
            setCamera("portrait");
        }
        else //landscape
        {
/*            Debug.Log("landscape");
            portraitCamera.SetActive(false);
            landscapeCamera.SetActive(true);*/
            setCamera("landscape");
        }        
    }

    // Update is called once per frame
    void Update()
    {
        height = Screen.currentResolution.height;
        width = Screen.currentResolution.width;
        if (height > width) //portrait
        {
            setCamera("portrait");
        }
        else //landscape
        {
            setCamera("landscape");
        }
    }

    void setCamera(string s)
    {
        if (s == "portrait")
        {
            //Debug.Log("portrait");
            landscapeCamera.SetActive(false);
            portraitCamera.SetActive(true);
        }
        else if (s == "landscape")
        {
            //Debug.Log("landscape");
            portraitCamera.SetActive(false);
            landscapeCamera.SetActive(true);
        }
    }
}
