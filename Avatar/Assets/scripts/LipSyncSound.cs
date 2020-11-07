using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LipSyncSound : MonoBehaviour
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

    private const int mthIndexA = 6;
    private const int mthIndexE = 9;
    private const int mthIndexI = 7;
    private const int mthIndexO = 10;
    private const int mthIndexU = 8;
    private const float mouthdelay = 0.15f;
    private const float punctuationdelay = 0.2f;
    private const float consonantdelay = 0.6f * mouthdelay;

    private bool start = false;
    private bool listen = false;
  
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
    //haɪ, ˈwɛlkəm tɪ blu, jʊr ˈpərsɪnəl læb əˈsɪstənt. haʊ meɪ aɪ hɛlp ju təˈdeɪ
    //sɑri aɪ kʊd nɑt ˈrɛkəgˌnaɪz ðət ˈkəmpəˌni, pliz traɪ əˈgɛn
    private List<char> consonants = new List<char>() { 'f', 'θ', 'd', 's', 'ð', 'p', 't' };
    private List<char> punctuation = new List<char>() { ',', '.' };

    private Queue<ArrChar> lipsyncQueue = new Queue<ArrChar>();
    private float currentdelay = mouthdelay;
    private float[] currentvals = new float[] { 0,0,0,0,0};

    int miclength = 5;
    float currentmiclength = 0;

    public PitchTracker AudioSourceReference;
    PitchTracker thispitchtracker;
    AudioSource thisaudiosource;

    private void Start()
    {
        thispitchtracker = GetComponent<PitchTracker>();
        thisaudiosource = GetComponent<AudioSource>();
        thispitchtracker.enabled = false;
        thisaudiosource.clip = Microphone.Start(null, true, miclength, AudioSettings.outputSampleRate);
    }

    void Update()
    {
        if (thisaudiosource.isPlaying)
        {
            if (currentmiclength < miclength)
            {
                currentmiclength += Time.deltaTime;
                float referencePitch = AudioSourceReference.avgPitch;
                float thisPitch = thispitchtracker.pitchValue;
                if (referencePitch * 0.9 <= thisPitch && thisPitch <= referencePitch * 1.1)
                {
                    print("heard");
                    start = true;
                    currentmiclength = 0;
                    thisaudiosource.Stop();
                    thispitchtracker.enabled = false;
                }
            }
            else
            {
                print("STOP");
                currentmiclength = 0;
                thisaudiosource.Stop();
                thispitchtracker.enabled = false;
            }
        }

        //check if the mouth is in the default position
        bool isIdle =
            currentvals[0] == 0 &&
            currentvals[1] == 0 &&
            currentvals[2] == 0 &&
            currentvals[3] == 0 &&
            currentvals[4] == 0;

        if (currentdelay < mouthdelay)
            currentdelay += Time.deltaTime;

        if (start)
        {
            if (currentdelay < mouthdelay)
            {
                //currentdelay += Time.deltaTime;
                if (!isIdle && lipsyncQueue.Count > 0)
                {
                    //interpolate from the current mouth position to the next mouth position in the queue
                    float[] interpolatedvals = interpolate(currentvals, lipsyncQueue.Peek().vals, mouthdelay, Mathf.Max(currentdelay, 0) );
                    SetMouthShape(interpolatedvals);
                    
                }
            }
            else if (lipsyncQueue.Count > 0)
            {
                ArrChar a = lipsyncQueue.Dequeue();
                float[] vals = a.vals;
                SetMouthShape(vals);
                currentdelay = 0;
                if (punctuation.Contains(a.character))
                {
                    currentdelay = -punctuationdelay;
                }
                else if (consonants.Contains(a.character))
                {
                    currentdelay = consonantdelay;
                }

            }
            else if (!isIdle)
            {
                lipsyncQueue.Enqueue(new ArrChar(new float[] { 0, 0, 0, 0, 0 }, ' '));
                currentdelay = 0;
                start = false;
            }


        }
        else if (!isIdle)
        {
            float[] interpolatedvals = interpolate(currentvals, new float[] { 0, 0, 0, 0, 0}, mouthdelay, Mathf.Max(currentdelay / 2f, 0));
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
            StartLipSync();
        }

    }

    public void lipsync(string IPA)
    {
        foreach (char c in IPA)
        {
            if (punctuation.Contains(c))
            {
                lipsyncQueue.Enqueue(new ArrChar(new float[] { 0, 0, 100, 0, 0 }, c));
            }
            else if (charMouthMappings.ContainsKey(c))
            {
                lipsyncQueue.Enqueue(new ArrChar(charMouthMappings[c], c));
            }
        }
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
            outp[i] = ((b[i] - a[i])/totalTime) * t + a[i];
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

    public void StartLipSync()
    {
        print("RECORDING");
        //thispitchtracker.enabled = true;       
        thisaudiosource.Play();        
    }
}
