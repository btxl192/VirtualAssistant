using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using OpenCvSharp.Demo;


public class player : MonoBehaviour
{
    public Animator animator;
    public static GameObject activeModel { get; private set; }
    public GameObject neck;
    public FaceTracker cameraFaceTracker;

    private int y_rotation = 0;
    private int prev_rotation = 0;
    private int neckStartAngle;

    public bool faceDetected = false;

    private void OnEnable()
    {
        activeModel = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        cameraFaceTracker = GameObject.Find("RawImage").GetComponent<FaceTracker>();
        GameObject main_camera = GameObject.Find("Main Camera");
        neckStartAngle = System.Convert.ToInt32(Rotator.Rotate(neck.transform, new Vector3(0f, 0f, 0f)).y * 50);
        faceDetected = cameraFaceTracker.faceDetected;
        activeModel = gameObject;
    }
    private void Update()
    {
        faceDetected = cameraFaceTracker.faceDetected;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        y_rotation = cameraFaceTracker.rotation;
        if (y_rotation != activeModel.transform.rotation.eulerAngles.y)
        {

            int unityAngle = System.Convert.ToInt32(activeModel.transform.rotation.eulerAngles.y);
            
            int neckAngle = System.Convert.ToInt32(Rotator.Rotate(neck.transform, new Vector3(0f, 0f, 0f)).y * 50);

            if (unityAngle > 180)
            {
                unityAngle = unityAngle - 360;
            }

            if (neckStartAngle != 0)
            {
                neckAngle -= neckStartAngle;
            }


            if (y_rotation > neckAngle)
            {
                Rotator.Rotate(neck.transform, new Vector3(0f, 25f, 0f));
            }

            else if (y_rotation < neckAngle)
            {
                if (neckAngle > -7)
                {
                    Rotator.Rotate(neck.transform, new Vector3(0f, -25f, 0f));
                }
            }

            if ((y_rotation >= 6 || unityAngle != 0) && (unityAngle < y_rotation))
            {
                Rotator.Rotate(activeModel.transform, new Vector3(0f, 25f, 0f));
            }

            else if ((y_rotation <= -6 || unityAngle != 0) && (unityAngle > y_rotation))
            {
                Rotator.Rotate(activeModel.transform, new Vector3(0f, -25f, 0f));
            }

        }
    }
}
