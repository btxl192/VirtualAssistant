using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emotion : MonoBehaviour
{

    public string emotionLayerName;
    private int emotionLayerIndex;
    private Animator anim;
    private string currentEmotion = null;

    private void Start()
    {
        anim = GetComponent<LipSync>().anim;
        emotionLayerIndex = anim.GetLayerIndex(emotionLayerName);
    }

    public void SetEmotion(string s)
    {
        currentEmotion = s;
    }

    public void PlayEmotion()
    {        
        if (currentEmotion != null)
        {
            anim.Play(currentEmotion, emotionLayerIndex);
            anim.SetLayerWeight(emotionLayerIndex, 1);
        }
    }

    public void StopEmotion()
    {
        anim.SetLayerWeight(emotionLayerIndex, 0);
    }
}
