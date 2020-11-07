using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LipSync : MonoBehaviour
{

    class ArrChar
    {
        public float[] vals;
        public char character;

        public ArrChar(float[] vals, char character)
        {
            this.vals = vals;
            this.character = character;
        }
    }

    public SkinnedMeshRenderer MTH_DEF;
    [HideInInspector]
    public bool getAudio = false;

    private AudioSource thisaudiosource;
    private const int mthIndexA = 6;
    private const int mthIndexE = 9;
    private const int mthIndexI = 7;
    private const int mthIndexO = 10;
    private const int mthIndexU = 8;
    private const float mouthdelay = 0.15f;
    private const float punctuationdelayvowel = 0.3f;
    private const float punctuationdelayconso = 0.1f;
    private const float consonantdelay = 0.6f * mouthdelay;

    private bool receivedText = false;
    private bool receivedAudio = false;

    private Dictionary<char, float[]> charMouthMappings = new Dictionary<char, float[]>()
    {
        { 'j', aeiou(0, 0, 0, 0, 75) },
        { 'ɪ', aeiou(30, 75, 0, 0, 0) },
        { 'i', aeiou(10, 75, 0, 0, 0) },
        { 'ɛ', aeiou(10, 75, 0, 0 ,0) },
        { 'o', aeiou(0, 0, 0, 50, 0) },
        { 'ʊ', aeiou(0, 0, 0, 20 , 75) },
        { 'æ', aeiou(100, 0, 0, 0, 0 ) },
        { 'f', aeiou(0, 75, 75, 0, 0) },
        { 'θ', aeiou(0, 75, 75, 0, 0) },
        { 'a', aeiou(0, 0, 0, 75 , 0 ) },
        { 'd', aeiou(0, 0, 75, 0, 0) },
        { 'ə', aeiou(75, 0, 0, 0 , 0) },
        { 's', aeiou(0, 0, 75, 0, 0) },
        { 'ð', aeiou(0, 0, 75, 0, 0) },
        { 'p', aeiou(0, 0, 75, 0, 30) },
        { 't', aeiou(0, 0, 75, 0, 0) },
        { 'w', aeiou(0, 0, 0, 0, 75) },
        { 'e', aeiou(10, 75, 0, 0, 0) },
        { 'g', aeiou(0, 0, 75, 0, 0) },
        { 'u', aeiou(0, 0, 0 , 30, 75) }
    };

    private List<char> consonants = new List<char>() { 'f', 'θ', 'd', 's', 'ð', 'p', 't' };
    private List<char> punctuation = new List<char>() { ',', '.' };

    private Queue<ArrChar> lipsyncQueue = new Queue<ArrChar>();
    private float currentdelay = mouthdelay;
    private float[] currentvals = new float[] { 0,0,0,0,0 };

    private void Start()
    {
        thisaudiosource = GetComponent<AudioSource>();
    }

    void Update()
    {

        //check if the mouth is in the default position
        bool isIdle =
            currentvals[0] == 0 &&
            currentvals[1] == 0 &&
            currentvals[2] == 0 &&
            currentvals[3] == 0 &&
            currentvals[4] == 0;

        //only increment if currentdelay < mouthdelay
        if (currentdelay < mouthdelay)
            currentdelay += Time.deltaTime;

        //start lipsyncing when text and audio is received
        if (receivedText && receivedAudio)
        {
            if (currentdelay < mouthdelay)
            {
                //if not idle and there is still text to lipsync
                if (!isIdle && lipsyncQueue.Count > 0)
                {
                    //interpolate from the current mouth position to the next mouth position in the queue
                    float[] interpolatedvals = interpolate(currentvals, lipsyncQueue.Peek().vals, mouthdelay, currentdelay );
                    SetMouthShape(interpolatedvals);                    
                }
            }
            //when currentdelay reaches mouthdelay, and there is still text to lipsync
            else if (lipsyncQueue.Count > 0)
            {
                ArrChar a = lipsyncQueue.Dequeue();
                float[] vals = a.vals; //dequeued text's aeiou values
                SetMouthShape(vals); //set the mouth shape to the dequeued text's aeiou values
                currentdelay = 0;

                //different delay based on type of character
                if (punctuation.Contains(a.character))
                {
                    currentdelay = -punctuationdelayvowel;

                    if (lipsyncQueue.Count > 0 && consonants.Contains(lipsyncQueue.Peek().character))
                    {
                        currentdelay = -punctuationdelayconso;
                    }
                }
                else if (consonants.Contains(a.character))
                {
                    currentdelay = consonantdelay;
                }

            }
            //reset mouth when idle
            else if (!isIdle)
            {
                lipsyncQueue.Enqueue(new ArrChar(new float[] { 0, 0, 0, 0, 0 }, ' '));
                currentdelay = 0;
                receivedText = false;
                receivedAudio = false;
            }


        }
        //keep on interpolating towards default mouth position when not idle
        else if (!isIdle)
        {
            float[] interpolatedvals = interpolate(currentvals, new float[] { 0, 0, 0, 0, 0}, mouthdelay, currentdelay / 2f);
            SetMouthShape(interpolatedvals);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {

            //lipsync("hɛˈloʊ"); //hello
            //lipsync("'ˈjunɪti"); //unity
            //lipsync("hɛˈloʊfʊt");
            //lipsync("'ˈsɑri aɪ dɪd nɑt ˌəndərˈstænd ðət, pliz traɪ əˈgɛn'");
            //lipsync("haɪ, ˈwɛlkəm tɪ blu, jʊr ˈpərsɪnəl læb əˈsɪstənt. haʊ meɪ aɪ hɛlp ju təˈdeɪ");
            //lipsync("sɑri aɪ kʊd nɑt ˈrɛkəgˌnaɪz ðət ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //lipsync("ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //StartLipSync();
        }

        if (getAudio)
        {
            getAudio = false;
            StartCoroutine(GetAlexaAudio());
        }
    }

    public void lipsync(string IPA)
    {
        foreach (char c in IPA)
        {           
            if (punctuation.Contains(c))
            {
                //fixed mouth shape for punctuation
                lipsyncQueue.Enqueue(new ArrChar(new float[] { 0, 0, 100, 0, 0 }, c));
            }
            else if (charMouthMappings.ContainsKey(c))
            {
                lipsyncQueue.Enqueue(new ArrChar(charMouthMappings[c], c));
            }
        }
        receivedText = true;
    }

    public void delay(float secs)
    {
        currentdelay -= secs;
    }

    //linearly interpolate each value from a to b
    float[] interpolate(float[] a, float[] b, float totalTime, float t)
    {
        float[] outp = new float[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            outp[i] = ((b[i] - a[i])/totalTime) * Mathf.Max(t, 0) + a[i];
        }
        return outp;
    }

    void SetMouthShape(float[] AEIOU)
    {
        MTH_DEF.SetBlendShapeWeight(mthIndexA, AEIOU[0]);
        MTH_DEF.SetBlendShapeWeight(mthIndexE, AEIOU[1]);
        MTH_DEF.SetBlendShapeWeight(mthIndexI, AEIOU[2]);
        MTH_DEF.SetBlendShapeWeight(mthIndexO, AEIOU[3]);
        MTH_DEF.SetBlendShapeWeight(mthIndexU, AEIOU[4]);
        currentvals = AEIOU;
    }

    void printarr(float[] f)
    {
        string s = "";
        foreach (float g in f)
        {
            s += g.ToString() + ",";
        }
        print(s);

    }

    static float[] aeiou(float a, float e, float i, float o, float u)
    {
        return new float[] { a, e, i, o, u };
    }

    public IEnumerator GetAlexaAudio()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort.ToString(), AudioType.WAV);
        yield return www.SendWebRequest();
        thisaudiosource.clip = DownloadHandlerAudioClip.GetContent(www);
        thisaudiosource.Play();
        receivedAudio = true;
    }

}
