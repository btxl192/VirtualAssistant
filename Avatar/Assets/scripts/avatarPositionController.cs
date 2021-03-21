using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avatarPositionController : MonoBehaviour
{
    public float moveToSideProg = 0.0f;

    void Start()
    {
    }

    void Update()
    {
        float screenRatio = (float) Screen.width / (float) Screen.height;
        transform.position = new Vector3(
            moveToSideProg * screenRatio * 0.658f,
            0.0f,
            1.19f - moveToSideProg * 0.3f);
    }
}
