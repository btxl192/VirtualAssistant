using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json.Linq;

public class Config : MonoBehaviour
{
    public string domainName { get; private set; }

    public string alexaResponseIP { get; private set; }
    public ushort alexaResponsePort { get; private set; }

    public bool testClient { get; private set; }

    public GameObject[] avatars;
    public GameObject activeAvatar;

    void Start() {
        using (StreamReader r = new StreamReader(Path.Combine(Application.dataPath, "config.json"))) {
            JObject json = JObject.Parse(r.ReadToEnd());
            Debug.Log("config: " + json);
            this.domainName = json["domainName"].Value<string>();
            this.alexaResponseIP = json["alexaResponseIP"].Value<string>();
            this.alexaResponsePort = json["alexaResponsePort"].Value<ushort>();
            this.testClient = json["testClient"].Value<bool>();
            int activeAvatarIndex = json["avatar"].Value<int>();
            activeAvatar = avatars[activeAvatarIndex];
        }
        activeAvatar.SetActive(true);
    }
}
