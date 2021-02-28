using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OpenCvSharp.Demo;


public class player : MonoBehaviour
{
    public Animator animator;
    public AnimationClip[] animations;
    public GameObject unityChan;
    public FaceTracker cameraFaceTracker;

    private int y_rotation = 0;
    private int prev_rotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cameraFaceTracker = GameObject.Find("RawImage").GetComponent<FaceTracker>();
        GameObject main_camera = GameObject.Find("Main Camera");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(cameraFaceTracker.faceDetected);
        //Face tracking
        y_rotation = cameraFaceTracker.rotation;
        //Debug.Log("Received: " + y_rotation);

        if (y_rotation != unityChan.transform.rotation.eulerAngles.y)
        {
            //Debug.Log("y rotation: " + y_rotation);

            int unityAngle = System.Convert.ToInt32(unityChan.transform.rotation.eulerAngles.y);
            if (unityAngle > 180)
            {
                unityAngle = unityAngle - 360;
            }

            if (y_rotation > unityAngle)
            {
                unityChan.transform.Rotate(new Vector3(0f, 25f, 0f) * Time.deltaTime);
            }
            else if (y_rotation < unityAngle)
            {
                unityChan.transform.Rotate(new Vector3(0f, -25f, 0f) * Time.deltaTime);
            }

        }
    }


}
