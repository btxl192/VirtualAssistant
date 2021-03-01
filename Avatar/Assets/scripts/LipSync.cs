using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
public class LipSync : MonoBehaviour
{

    private bool hasEmotion = false;
    private string speechurl = "";

    private bool receivedText = false;
    private bool receivedAudio = false;

    public Animator anim { get; private set; }
    private Queue<char> lipsyncQueue = new Queue<char>();
    private AudioSource thisaudiosource;
    private Emotion thisemotion;

    private float timer = 0;
    private const float syllabletime = 0.05f;
    private const float crossfadetime = 0.25f;

    private float timeoutTimer = 0;
    private const float timeoutTime = 1f; //in seconds
    private bool startTimeout = false;

    private const float waitAnimTimer = 0.2f;
    private float waitAnimTimerTime = 0f;

    private float currentAverageVolume = 0;
    private const float volumeThreshold = 0.0001f;
    public bool isSilent { get => currentAverageVolume < volumeThreshold; }

    private bool getAudio;

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

    //for debugging
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

    private void Awake()
    {
        WebsocketHandler.MessageReceived += HandleMsg;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        thisaudiosource = GetComponent<AudioSource>();
        thisemotion = GetComponent<Emotion>();
        //StartCoroutine(GetAlexaAudio());
    }

    //if a character has a time modifier
    float GetTimeMod(char c)
    {
        if (IPATimeMod.ContainsKey(c))
        {
            return IPATimeMod[c];
        }
        return 0;
    }

    private void lipsync(string IPA)
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

        if (receivedText && !receivedAudio)
        {
            if (waitAnimTimerTime < waitAnimTimer)
            {
                waitAnimTimerTime += Time.deltaTime;
            }
            else
            {
                anim.SetBool("isWaiting", true);
            }
        }

        if (receivedText && receivedAudio && (!hasEmotion || thisemotion.currentEmotion != null) && timer >= syllabletime)
        {
            if (isSilent)
            {
                SetMouthShape("none", crossfadetime / 2f);
            }
            else
            {
                timeoutTimer = 0;                                
                if (lipsyncQueue.Count > 0)
                {

                    char c = lipsyncQueue.Dequeue();
                    if (supportedIPA.Contains(c))
                    {
                        //print(c);
                        timer = 0;
                        SetMouthShape(c.ToString(), crossfadetime + GetTimeMod(c));
                    }


                }
            }

            if (lipsyncQueue.Count == 0)
            {
                //lipsync ended
                receivedAudio = false;
                receivedText = false;
                anim.SetBool("isTalking", false);
                SetMouthShape("default", crossfadetime);
                thisemotion.StopEmotion();
                startTimeout = false;
            }
            else if (!startTimeout)
            {
                //lipsync started
                anim.SetBool("isWaiting", false);
                waitAnimTimerTime = 0;
                timeoutTimer = 0;
                startTimeout = true;
                thisaudiosource.Play();
                if (hasEmotion)
                {
                    thisemotion.PlayEmotion();
                }
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

    void HandleMsg(JObject msgjson, string msgtitle, string msgtext)
    {
        switch(msgtitle)
        {
            case "Speech":
                print("received text");
                hasEmotion = WebsocketHandler.GetJsonKeys(msgjson).Contains("Emotion");
                if (hasEmotion)
                {
                    thisemotion.SetEmotion(msgjson.Value<string>("Emotion"));
                }
                lipsync(msgtext);
                break;
            case "SpeechControl":
                print("received audio");
                if (msgtext.ToLower().Equals("written"))
                {
                    getAudio = true;
                }
                break;
            case "SpeechUrl":
                speechurl = msgtext;
                break;
        }
    }

    void SetMouthShape(string c, float crossfade)
    {
        anim.SetBool("isTalking", true);
        if (new List<string>() { "a", "e", "i", "o", "u" }.Contains(c))
        {
            anim.CrossFade(("MTH_" + c).ToUpper(), crossfade);
        }
        else
        {
            anim.CrossFade("MTH_" + c, crossfade);
        }
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
        //thisaudiosource.Play();
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

    private void OnApplicationQuit()
    {
        WebsocketHandler.MessageReceived -= HandleMsg;
    }
}
