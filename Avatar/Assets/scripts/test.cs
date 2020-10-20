using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    float miclength = 5;
    float currentmiclength = 0;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            print("RECORDING");
            GetComponent<AudioSource>().clip = Microphone.Start(null, true, 5, AudioSettings.outputSampleRate);
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().isPlaying)
        {
            if (currentmiclength < miclength)
            {
                currentmiclength += Time.deltaTime;
            }
            else
            {
                print("STOP");
                currentmiclength = 0;
                //GetComponent<AudioSource>().clip = null;
                GetComponent<AudioSource>().Stop();
            }
        }

    }
}
