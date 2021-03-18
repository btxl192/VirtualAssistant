using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


public class weatherControl : MonoBehaviour
{
    private static string API_KEY = "c9a3173c6264df62ecb1f0cc390ed75d";
    private string weather;

    public Config config;

    public Material clear;
    public Material cloudy;

    public ParticleSystem rain;
    public ParticleSystem snow;

    private float timer;
    private float callFrequency = 10; //calls api every 10 minutes

    void Update()
    {
        if (timer <= 0)
        {
            StartCoroutine(GetWeather());
            timer = callFrequency * 60;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private IEnumerator GetWeather()
    {
        string location = config.weatherLocation;
        string URL = "api.openweathermap.org/data/2.5/weather?q=" + location + "&appid=" + API_KEY;
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();
        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);

            var websiteJSON = JSON.Parse(www.downloadHandler.text);
            weather = websiteJSON["weather"][0]["main"].Value;
            //Debug.Log(weather);
            changeState(weather);
        }
    }

    private void changeState(string APIweather)
    {
        if ((APIweather == "Thunderstorm") || (APIweather == "Drizzle") || (APIweather == "Rain"))
        {
            RenderSettings.skybox = cloudy;
            if (snow.isPlaying)
            {
                snow.Stop();
            }
            rain.Play();
        }
        else if (APIweather == "Snow")
        {
            RenderSettings.skybox = cloudy;
            if (rain.isPlaying)
            {
                rain.Stop();
            }
            snow.Play();
        }
        else
        {
            if (rain.isPlaying) //cloudy and clear both do not require any particle effects
            {
                rain.Stop();
            }
            if (snow.isPlaying)
            {
                snow.Stop();
            }
            if (APIweather == "Clouds")
            {
                RenderSettings.skybox = cloudy;
            }
            else {
                RenderSettings.skybox = clear;  //if it's not rainy, snowy or cloudy then the weather will be clear
            }
        }
    }

}
