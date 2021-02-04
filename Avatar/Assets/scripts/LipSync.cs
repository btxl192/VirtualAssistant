using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LipSync : MonoBehaviour
{

    [HideInInspector]
    public bool hasEmotion = false;

    [HideInInspector]
    public string speechurl = "";

    private bool receivedText = false;
    private bool receivedAudio = false;

    public Animator anim { get; private set; }
    private Queue<char> lipsyncQueue = new Queue<char>();
    private AudioSource thisaudiosource;

    private float timer = 0;
    private float syllabletime = 0.05f;
    private float crossfadetime = 0.25f;

    private float timeoutTimer = 0;
    private float timeoutTime = 1f; //in seconds
    private bool startTimeout = false;

    private float currentAverageVolume = 0;
    private float volumeThreshold = 0.0001f;
    private bool isSilent { get => currentAverageVolume < volumeThreshold; }

    private Emotion thisemotion;

    [HideInInspector]
    public bool getAudio;
    private List<char> supportedIPA = new List<char>()
    {   'j',
        'ɪ',
        'i',
        'ɛ',
        'o',
        'ʊ',
        'æ',
        'f',
        'θ',
        'a',
        'd',
        'ə',
        's',
        'ð',
        'p',
        't',
        'w',
        'e',
        'g',
        'u',
        'n',
        'm'
    };

    private Dictionary<char, float> IPATimeMod = new Dictionary<char, float>()
    {
        {'p', -0.1f},
        {'s', -0.15f},
        {'n', -0.1f},
        {'u', -0.1f},
        {'t', -0.1f},
        {'ʊ', -0.1f}
    };

    void printarr<T>(T[] arr)
    {
        string s = "";
        for (int i = 10000; i < 15000; i++)
        {
            if (i < arr.Length)
            {
                s += arr[i].ToString() + ", ";
            }

        }
        print(s);
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        thisaudiosource = GetComponent<AudioSource>();
        thisemotion = GetComponent<Emotion>();
        //StartCoroutine(GetAlexaAudio());
    }
    float GetTimeMod(char c)
    {
        if (IPATimeMod.ContainsKey(c))
        {
            return IPATimeMod[c];
        }
        return 0;
    }

    public void lipsync(string IPA)
    {
        foreach (char c in IPA)
        {
            lipsyncQueue.Enqueue(c);
        }
        receivedText = true;
    }

    float getAverageVolume()
    {
        float[] clipSampleData = new float[1024];
        thisaudiosource.GetSpectrumData(clipSampleData, 0, FFTWindow.Rectangular);
        return System.Linq.Enumerable.Average(clipSampleData);
    }

    void Update()
    {
        currentAverageVolume = getAverageVolume();

        if (getAudio)
        {
            getAudio = false;
            StartCoroutine(GetAlexaAudio());
        }
        if (timer < syllabletime)
        {
            timer += Time.deltaTime;
        }

        if (receivedText && receivedAudio && (!hasEmotion || thisemotion.currentEmotion != null) && timer >= syllabletime)
        {
            if (!startTimeout)
            {
                timeoutTimer = 0;
                startTimeout = true;
                if (hasEmotion)
                {
                    thisemotion.PlayEmotion();
                }
                
            }

            if (isSilent)
            {
                SetMouthShape('\0', crossfadetime / 2f);
            }
            else
            {
                timeoutTimer = 0;
                if (lipsyncQueue.Count > 0)
                {

                    char c = lipsyncQueue.Dequeue();
                    //print(c);
                    if (supportedIPA.Contains(c))
                    {
                        //print(c);
                        timer = 0;
                        SetMouthShape(c, crossfadetime + GetTimeMod(c));
                    }


                }
            }

            if (lipsyncQueue.Count == 0)
            {
                startTimeout = false;
                receivedAudio = false;
                receivedText = false;
                anim.SetBool("isTalking", false);
                SetMouthShape("default", crossfadetime);
                thisemotion.StopEmotion();
            }
        }
        if (startTimeout)
        {
            if (timeoutTimer < timeoutTime)
            {
                timeoutTimer += Time.deltaTime;
            }
            else
            {
                startTimeout = false;
                lipsyncQueue.Clear();
            }
        }

        //for testing
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(GetAlexaAudioHi());
            //lipsync("hɛˈloʊ"); //hello
            //lipsync("'ˈjunɪti"); //unity
            //lipsync("hɛˈloʊfʊt");
            //lipsync("'ˈsɑri aɪ dɪd nɑt ˌəndərˈstænd ðət, pliz traɪ əˈgɛn'");
            lipsync("haɪ, ˈwɛlkəm tɪ blu, jʊr ˈpərsɪnəl læb əˈsɪstənt. haʊ meɪ aɪ hɛlp ju təˈdeɪ");
            //lipsync("sɑri aɪ kʊd nɑt ˈrɛkəgˌnaɪz ðət ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //lipsync("kʊd");
            //lipsync("ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //StartLipSync();
            //GetComponent<OVRLipSyncContext>().audioSource = thisaudiosource;
            //receivedAudio = true;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(GetAlexaAudioSorry());
            //lipsync("hɛˈloʊ"); //hello
            //lipsync("'ˈjunɪti"); //unity
            //lipsync("hɛˈloʊfʊt");
            //lipsync("'ˈsɑri aɪ dɪd nɑt ˌəndərˈstænd ðət, pliz traɪ əˈgɛn'");
            //lipsync("haɪ, ˈwɛlkəm tɪ blu, jʊr ˈpərsɪnəl læb əˈsɪstənt. haʊ meɪ aɪ hɛlp ju təˈdeɪ");
            lipsync("sɑri aɪ kʊd nɑt ˈrɛkəgˌnaɪz ðət ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //lipsync("kʊd");
            //lipsync("ˈkəmpəˌni, pliz traɪ əˈgɛn");
            //StartLipSync();
            //GetComponent<OVRLipSyncContext>().audioSource = thisaudiosource;
            //receivedAudio = true;
        }
    }

    void SetMouthShape(char c, float crossfade)
    {
        anim.SetBool("isTalking", true);
        if (new List<char>() { 'a', 'e', 'i', 'o', 'u' }.Contains(c))
        {
            anim.CrossFade(("MTH_" + c).ToUpper(), crossfade);
        }
        else
        {
            if (c.Equals('\0'))
            {
                anim.CrossFade("MTH_none", crossfade);
            }
            else
            {
                anim.CrossFade("MTH_" + c, crossfade);
            }

        }
    }

    void SetMouthShape(string c, float crossfade)
    {
        anim.SetBool("isTalking", true);
        anim.CrossFade("MTH_" + c, crossfade);
    }

    public IEnumerator GetAlexaAudio()
    {
        //UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort.ToString(), AudioType.MPEG);
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(speechurl, AudioType.MPEG);
        yield return www.SendWebRequest();
        //#if UNITY_EDITOR
        thisaudiosource.clip = NAudioPlayer.FromMp3Data(www.downloadHandler.data);
        //#else
        //        thisaudiosource.clip = DownloadHandlerAudioClip.GetContent(www);
        //#endif
        www.Abort();
        thisaudiosource.Play();
        receivedAudio = true;
    }

    //for testing
    public IEnumerator GetAlexaAudioHi()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort.ToString() + "/hi", AudioType.WAV);
        yield return www.SendWebRequest();
        thisaudiosource.clip = DownloadHandlerAudioClip.GetContent(www);
        www.Abort();

        thisaudiosource.Play();
        receivedAudio = true;
    }

    //for testing
    public IEnumerator GetAlexaAudioSorry()
    {
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("http://" + config.alexaResponseIP + ":" + config.alexaResponsePort.ToString() + "/sorry", AudioType.WAV);
        yield return www.SendWebRequest();
        thisaudiosource.clip = DownloadHandlerAudioClip.GetContent(www);
        www.Abort();

        thisaudiosource.Play();
        receivedAudio = true;
    }
}
