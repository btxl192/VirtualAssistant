using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using System.IO;

public class BubblesManager : MonoBehaviour
{
    [SerializeField] private float typingSpeed = 0.01f;

    [Header("Bubbles TMP")]
    [SerializeField] private TextMeshProUGUI bubbleLeftUp;
    [SerializeField] private TextMeshProUGUI bubbleLeftDown;
    [SerializeField] private TextMeshProUGUI bubbleRightUp;
    [SerializeField] private TextMeshProUGUI bubbleRightDown;

    [Header("Animation Controllers")]
    [SerializeField] private Animator bubbleLeftUpAnimator;
    [SerializeField] private Animator bubbleLeftDownAnimator;
    [SerializeField] private Animator bubbleRightUpAnimator;
    [SerializeField] private Animator bubbleRightDownAnimator;

    [Header("Bubbles Text")]
    [TextArea]
    [SerializeField] private string bubbleLeftUpText;
    [TextArea]
    [SerializeField] private string bubbleLeftDownText;
    [TextArea]
    [SerializeField] private string bubbleRightUpText;
    [TextArea]
    [SerializeField] private string bubbleRightDownText;

    private float bubbleAnimationDelay = 0.6f;

    private GameObject thisCurrentModel;
    private player thisplayer;
    private LipSync thislipsync;
    private Animator videoAnimator;

    private bool faceDetected;
    private bool areDisplayed;
    DateTime timeLastSpoke;
    DateTime faceLastDetected;

    private void typeLeftUpText()
    {
        // foreach (char letter in bubbleLeftUpText.ToCharArray())
        // {
        //     bubbleLeftUp.text += letter;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
        bubbleLeftUp.text = bubbleLeftUpText;
    }

    private void typeLeftDownText()
    {
        // foreach (char letter in bubbleLeftDownText.ToCharArray())
        // {
        //     bubbleLeftDown.text += letter;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
        bubbleLeftDown.text = bubbleLeftDownText;
    }

    private void typeRightUpText()
    {
        // foreach (char letter in bubbleRightUpText.ToCharArray())
        // {
        //     bubbleRightUp.text += letter;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
        bubbleRightUp.text = bubbleRightUpText;
    }

    private void typeRightDownText()
    {
        // foreach (char letter in bubbleRightDownText.ToCharArray())
        // {
        //     bubbleRightDown.text += letter;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
        bubbleRightDown.text = bubbleRightDownText;
    }

    private void empty(){
        bubbleLeftUp.text = string.Empty;
    }

    private void closeBubbles()
    {
        bubbleRightUpAnimator.SetTrigger("Close");
        bubbleLeftUpAnimator.SetTrigger("Close");
        bubbleRightDownAnimator.SetTrigger("Close");
        bubbleLeftDownAnimator.SetTrigger("Close");

        bubbleLeftUp.text = string.Empty;
        bubbleLeftDown.text = string.Empty;
        bubbleRightUp.text = string.Empty;
        bubbleRightDown.text = string.Empty;

        // yield return new WaitForSeconds(bubbleAnimationDelay);
    }

    private void openBubbles()
    {
        bubbleLeftUp.text = string.Empty;
        bubbleLeftDown.text = string.Empty;
        bubbleRightUp.text = string.Empty;
        bubbleRightDown.text = string.Empty;

        // yield return new WaitForSeconds(1);

        bubbleRightUpAnimator.SetTrigger("Open");
        bubbleLeftUpAnimator.SetTrigger("Open");
        bubbleRightDownAnimator.SetTrigger("Open");
        bubbleLeftDownAnimator.SetTrigger("Open");

        // yield return new WaitForSeconds(bubbleAnimationDelay);

        // StartCoroutine(typeRightUpText());
        // StartCoroutine(typeLeftUpText());
        // StartCoroutine(typeRightDownText());
        // StartCoroutine(typeLeftDownText());
        typeRightUpText();
        typeLeftUpText();
        typeRightDownText();
        typeLeftDownText();

    }

    // Start is called before the first frame update
    void Start()
    {
        faceLastDetected = DateTime.Now.AddSeconds(-2);
        thisCurrentModel = player.activeModel;
        thislipsync = thisCurrentModel.GetComponent<LipSync>();
        thisplayer = thisCurrentModel.GetComponent<player>();
        videoAnimator = GameObject.Find("Video Player").GetComponent<Animator>();
        timeLastSpoke = DateTime.Now;
        // StartCoroutine(openBubbles());
        // openBubbles();
        // areDisplayed = true;
    }

     // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        double secondsSpeak = (now - timeLastSpoke).TotalSeconds;
        faceDetected = thisplayer.faceDetected;
        bool isTalking = !thislipsync.isSilent;
        double secondsFace = (now - faceLastDetected).TotalSeconds;
        if(faceDetected){
            faceLastDetected = DateTime.Now;
        }
        if(videoAnimator.GetBool("stopped") == false){
        	if(areDisplayed){
        		// StartCoroutine(closeBubbles());
                closeBubbles();
                areDisplayed = false;
        	}
        }
        else{
            if(secondsFace < 5){
                if(isTalking){
                    if(areDisplayed){
                        // StartCoroutine(closeBubbles());
                        closeBubbles();
                        areDisplayed = false;
                    }
                    timeLastSpoke = DateTime.Now;
                }
                else if (secondsSpeak > 30){
                    if(!areDisplayed){
                        // StartCoroutine(openBubbles());
                        openBubbles();
                        areDisplayed = true;
                    }
                }
            }
            else {
                if(areDisplayed){
                    // StartCoroutine(closeBubbles());
                    closeBubbles();
                    areDisplayed = false;
                }
            }
        }
    }
}
