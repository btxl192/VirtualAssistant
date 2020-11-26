using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSync2 : MonoBehaviour
{

    private Animator anim;
    private Queue<char> lipsyncQueue = new Queue<char>();
    private bool receivedText = false;
    private bool receivedAudio = false;
    public void lipsync(string IPA)
    {
        foreach (char c in IPA)
        {
            /*
            if (punctuation.Contains(c))
            {
                //fixed mouth shape for punctuation
                lipsyncQueue.Enqueue(new ArrChar(new float[] { 0, 0, 100, 0, 0 }, c));
            }
            else if (charMouthMappings.ContainsKey(c))
            {
                lipsyncQueue.Enqueue(new ArrChar(charMouthMappings[c], c));
            }
            */
            lipsyncQueue.Enqueue(c);
        }
        receivedText = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (receivedText && receivedAudio)
        {
            if (lipsyncQueue.Count > 0)
            {
                SetMouthShape(lipsyncQueue.Dequeue());
            }
            else
            {
                anim.SetBool("IsTalking", false);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {

            lipsync("hɛˈloʊ"); //hello
            //lipsync("'ˈjunɪti"); //unity
            //lipsync("hɛˈloʊfʊt");
            //lipsync("'ˈsɑri aɪ dɪd nɑt ˌəndərˈstænd ðət, pliz traɪ əˈgɛn'");
            //lipsync("haɪ, ˈwɛlkəm tɪ blu, jʊr ˈpərsɪnəl læb əˈsɪstənt. haʊ meɪ aɪ hɛlp ju təˈdeɪ");
            //lipsync("sɑri aɪ kʊd nɑt ˈrɛkəgˌnaɪz ðət ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //lipsync("ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //StartLipSync();
            receivedAudio = true;
        }
    }

    void SetMouthShape(char c)
    {
        anim.SetBool("IsTalking", true);
        if (new List<char>() { 'a', 'e', 'i', 'o', 'u' }.Contains(c))
        {
            anim.SetTrigger(("MTH_" + c).ToUpper());
        }
        else
        {
            anim.SetTrigger("MTH_" + c);
        }
        print(("MTH_" + c).ToUpper());
    }
}
