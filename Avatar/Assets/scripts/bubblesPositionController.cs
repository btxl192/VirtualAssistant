using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubblesPositionController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Screen.width > Screen.height)
        {
            transform.position = new Vector3(-0.1f, 1.05f, -2.1f); //landscape
        }
        else
        {
            transform.position = new Vector3(-0.11f, 1.42f, -3.2f); //portrait
        }
    }
}
