using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logoScaler : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Screen.width > Screen.height)
        {
            transform.localScale = new Vector3(0.08f, 0.08f, 0f); //landscape
        }
        else
        {
            transform.localScale = new Vector3(0.05f, 0.05f, 0f); //portrait
        }
    }
}
