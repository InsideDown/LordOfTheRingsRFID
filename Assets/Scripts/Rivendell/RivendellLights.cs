using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RivendellLights : MonoBehaviour {

    public string LightURL = "http://10.0.0.4/api/";
    public string UserID = "EEPOy42DbV82fdvDeoIAIn12yNRqWyfZPQZlcuqe";
    public string LightID = "3";
    public float AnimPulseTime = 1;
    public float AnimPulsePauseTime = 1;

    private string _LightURL = "";
    private string _LightStateURL = "";
    private bool _IsPulsing = false;
    private Coroutine AnimCoroutine;
    private Coroutine LightPutCoroutine;

    private void Awake()
    {
        _LightURL = LightURL + UserID;
        _LightStateURL = _LightURL + "/lights/" + LightID + "/state";
        //given in multiples of 100ms, so convert from seconds
        AnimPulsePauseTime *= 10;
        //ButtonCanvas.SetActive(false);
        LightPulse();
    }

    private void OnEnable()
    {
        EventManager.OnRingPlacedEvent += EventManager_OnVideoStartEvent;
        //EventManager.OnVideoStartEvent += EventManager_OnVideoStartEvent;
        EventManager.OnVideoEndEvent += EventManager_OnVideoEndEvent;
    }

    private void OnDisable()
    {
        EventManager.OnRingPlacedEvent -= EventManager_OnVideoStartEvent;
        //EventManager.OnVideoStartEvent -= EventManager_OnVideoStartEvent;
        EventManager.OnVideoEndEvent -= EventManager_OnVideoEndEvent;
    }

    /// <summary>
    /// pulse our lights as if we are in the pillar
    /// </summary>
    private void LightPulse()
    {
        StopAnim();
        _IsPulsing = true;
        AnimCoroutine = StartCoroutine(AnimPulse(254));
    }

    /// <summary>
    /// cancel our light animation
    /// </summary>
    private void StopAnim()
    {
        _IsPulsing = false;
        if (AnimCoroutine != null)
            StopCoroutine(AnimCoroutine);

        if (LightPutCoroutine != null)
            StopCoroutine(LightPutCoroutine);

        AnimCoroutine = null;
        LightPutCoroutine = null;
    }

    private IEnumerator AnimPulse(float brightnessVal = 254)
    {
        float dimVal = 0;
        string lightPayload = "";
        if (_IsPulsing)
        {
            //lightPayload = "{\"on\":true, \"bri\": " + brightnessVal + ", \"sat\":0, \"transitiontime\":" + AnimPulsePauseTime + "}";
            lightPayload = "{\"on\":true, \"bri\": " + brightnessVal + ", \"sat\":0}";
            LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
            yield return new WaitForSeconds(AnimPulseTime);
            if (brightnessVal > dimVal && _IsPulsing)
            {
                StopAnim();
                _IsPulsing = true;
                AnimCoroutine = StartCoroutine(AnimPulse(dimVal));
            }
            else
            {
                StopAnim();
                _IsPulsing = true;
                AnimCoroutine = StartCoroutine(AnimPulse(254));
            }
        }
    }

    private IEnumerator LightPutRequest(string url, string payload)
    {
        if (!string.IsNullOrEmpty(payload))
        {
            byte[] payloadData = System.Text.Encoding.UTF8.GetBytes(payload);
            UnityWebRequest www = UnityWebRequest.Put(url, payloadData);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log("Upload complete!");
            }
        }
    }

    void RestartAnimation()
    {
        LightPulse();
    }

    void SetLightColor(Vector2 lightColor)
    {
        StopAnim();
        string xyVal = "[" + lightColor.x + "," + lightColor.y + "]";
        string lightPayload = "{\"on\":true, \"bri\": 254, \"sat\":0, \"xy\":" + xyVal + "}";
        LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
    }

    void EventManager_OnVideoStartEvent(VideoItemModel curVideoItem)
    {
        Color curColor = curVideoItem.LightRGB;

        int maxVal = 255;
        int rVal = Mathf.RoundToInt(curColor.r * maxVal);
        int gVal = Mathf.RoundToInt(curColor.g * maxVal);
        int bVal = Mathf.RoundToInt(curColor.b * maxVal);
        Vector2 newLightColor = Utils.Instance.RGBToXY(rVal,gVal,bVal);
        SetLightColor(newLightColor);

    }


    void EventManager_OnVideoEndEvent()
    {
        RestartAnimation();
    }

}
