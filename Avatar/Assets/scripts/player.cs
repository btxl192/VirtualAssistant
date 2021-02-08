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

    public bool isTalking = false;
    public bool stopTalking = false;

    private float idleAnimation = 0f;
    private float goneIdle = 5f;
    private float current = 0;
    private float delayWeight = 0;
    private string response = "";  
    private int y_rotation = 0;
    private int prev_rotation = 0;

    // Start is called before the first frame update
    async void Start()
    {
        animator = GetComponent<Animator>();
        cameraFaceTracker = GameObject.Find("RawImage").GetComponent<FaceTracker>();
        unityChan = GameObject.Find("unitychan");
        //neck = GameObject.Find("Character1_Head");
        SetIdleTime();

        GameObject main_camera = GameObject.Find("Main Camera");
    }



    // Update is called once per frame
    void Update()
    {
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
                //neck.transform.Rotate(new Vector3(-25f, 0f, 0f) * Time.deltaTime);
            }
            else if (y_rotation < unityAngle)
            {
                unityChan.transform.Rotate(new Vector3(0f, -25f, 0f) * Time.deltaTime);
                //neck.transform.Rotate(new Vector3(25f, 0f, 0f) * Time.deltaTime);
            }

        }
        
        //This cycles through the idle animations to be played after a certain amount of idle time
        //idleAnimation += Time.deltaTime;
        if (isTalking)
        {
            animator.Play("WAIT00", -1, 0f);
        }

/*        if (idleAnimation > goneIdle)
        {
            idleAnimation = 0;
            SetIdleTime();
            int num = Random.Range(1, 3);
            if (num == 1)
            {
                animator.Play("WAIT01", -1, 0f);
                idleAnimation = -5 - (this.animator.GetCurrentAnimatorClipInfo(0))[0].clip.length;
            }
            else
            {
                animator.Play("WAIT02", -1, 0f);
                idleAnimation = -7 - (this.animator.GetCurrentAnimatorClipInfo(0))[0].clip.length;
            }

        }*/

        /*
        //This is the temporary toggle for the mouth animations to start
        if (isTalking)
        {
            Debug.Log("Talking");
            animator.SetBool("isTalking", isTalking);
            animator.CrossFade("MTH_A", 0);
            idleAnimation = -5;
            current = 1;
            

        }
        //Stop talking when state is IDLE
        if (stopTalking)
        {
            animator.SetBool("isTalking", isTalking);
            current = 0;
            //animator.CrossFade("default@unitychan", 0);
        }

        //fix this to default layer if you're not talking
        animator.SetLayerWeight(1, current);
        */
    }

    void SetIdleTime()
    {
        goneIdle = Random.Range(5, 8);
    }


}
