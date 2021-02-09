using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HttpPoll
{
    private string url;
    private float delaySeconds; 
    private long prevmsgid = -1;

    private handlerDelegate handler;
    public delegate void handlerDelegate(JObject j);

    public HttpPoll(string url, float delaySeconds, handlerDelegate handler)
    {
        this.url = url;
        this.delaySeconds = delaySeconds;
        this.handler = handler;
    }

    public IEnumerator poll()
    {        
        while (true)
        {
            UnityEngine.Networking.UnityWebRequest r = UnityEngine.Networking.UnityWebRequest.Get(url);
            yield return r.SendWebRequest();
            if (r.downloadHandler.text != "")
            {
                JObject msgjson = JObject.Parse(r.downloadHandler.text);
                if (msgjson.Properties().Select(p => p.Name).ToList().Count > 0)
                {
                    long currentId = msgjson.Value<long>("id");

                    if (prevmsgid != currentId)
                    {
                        handler(msgjson);
                        prevmsgid = currentId;
                    }
                }
            }
            yield return new WaitForSeconds(delaySeconds);
        }
    }
}
