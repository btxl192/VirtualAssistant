using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private player thisplayer;
    private bool areDisplayed;
    DateTime timeLastSpoke;

    private IEnumerator typeLeftUpText()
    {
        foreach (char letter in bubbleLeftUpText.ToCharArray())
        {
            bubbleLeftUp.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator typeLeftDownText()
    {
        foreach (char letter in bubbleLeftDownText.ToCharArray())
        {
            bubbleLeftDown.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator typeRightUpText()
    {
        foreach (char letter in bubbleRightUpText.ToCharArray())
        {
            bubbleRightUp.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private IEnumerator typeRightDownText()
    {
        foreach (char letter in bubbleRightDownText.ToCharArray())
        {
            bubbleRightDown.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    private void empty(){
        bubbleLeftUp.text = string.Empty;
    }

    private IEnumerator closeBubbles()
    {
        bubbleRightUpAnimator.SetTrigger("Close");
        bubbleLeftUpAnimator.SetTrigger("Close");
        bubbleRightDownAnimator.SetTrigger("Close");
        bubbleLeftDownAnimator.SetTrigger("Close");

        bubbleLeftUp.text = string.Empty;
        bubbleLeftDown.text = string.Empty;
        bubbleRightUp.text = string.Empty;
        bubbleRightDown.text = string.Empty;

        yield return new WaitForSeconds(bubbleAnimationDelay);
    }

    private IEnumerator openBubbles()
    {

        yield return new WaitForSeconds(1);

        bubbleRightUpAnimator.SetTrigger("Open");
        bubbleLeftUpAnimator.SetTrigger("Open");
        bubbleRightDownAnimator.SetTrigger("Open");
        bubbleLeftDownAnimator.SetTrigger("Open");

        yield return new WaitForSeconds(bubbleAnimationDelay);

        StartCoroutine(typeRightUpText());
        StartCoroutine(typeLeftUpText());
        StartCoroutine(typeRightDownText());
        StartCoroutine(typeLeftDownText());

    }

    // Start is called before the first frame update
    void Start()
    {
        thisplayer = GetComponent<player>();
        timeLastSpoke = DateTime.Now;
        StartCoroutine(openBubbles());
        areDisplayed = true;
    }

     // Update is called once per frame
    void Update()
    {
        DateTime now = DateTime.Now;
        double seconds = (now - timeLastSpoke).TotalSeconds;
        bool isTalking = thisplayer.isTalking;
        if(isTalking){
            if(areDisplayed){
                StartCoroutine(closeBubbles());
                areDisplayed = false;
            }
            timeLastSpoke = DateTime.Now;
        }
        else if (seconds > 30){
            StartCoroutine(openBubbles());
            areDisplayed = true;
        }
    }
}
