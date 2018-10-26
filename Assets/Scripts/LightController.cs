using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LightController : MonoBehaviour {

    public string LightURL = "http://10.0.0.4/api/";
    public string UserID = "EEPOy42DbV82fdvDeoIAIn12yNRqWyfZPQZlcuqe";
    public string LightID = "3";
    public float AnimPulseTime = 1;
    public float AnimPulsePauseTime = 1;
    public int TotalVideos = 2; //end when both videos are over
    public GameObject ButtonCanvas;

    private string _LightURL = "";
    private string _LightStateURL = "";
    private bool _IsPulsing = false;
    private Coroutine AnimCoroutine;
    private Coroutine LightPutCoroutine;
    private int _CurVideoEndCount = 0;

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
        EventManager.OnRedTeamWin += EventManager_OnRedTeamWin;
        EventManager.OnGreenTeamWin += EventManager_OnGreenTeamWin;
        EventManager.OnVideoEndEvent += EventManager_OnVideoEndEvent;
    }

    private void OnDisable()
    {
        EventManager.OnRedTeamWin -= EventManager_OnRedTeamWin;
        EventManager.OnGreenTeamWin -= EventManager_OnGreenTeamWin;
        StopAnim();
    }

    void EventManager_OnRedTeamWin()
    {
        _CurVideoEndCount = 0;
        SetLightRed();
    }

    void EventManager_OnGreenTeamWin()
    {
        _CurVideoEndCount = 0;
        SetLightGreen();
    }

    void EventManager_OnVideoEndEvent()
    {
        CheckRestartAnimation();
    }

    void CheckRestartAnimation()
    {
        _CurVideoEndCount++;
        Debug.Log("video over, checking restart: " + _CurVideoEndCount);
        if (_CurVideoEndCount == TotalVideos)
        {
            _CurVideoEndCount = 0;
            LightPulse();
        }
    }

    public void TurnLightOff()
    {
        StopAnim();
        string lightPayload = "{\"on\":false}";
        LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
    }

    public void TurnLightOn()
    {
        StopAnim();
        string lightPayload = "{\"on\":true}";
        LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
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

    public void LightPulse()
    {
        StopAnim();
        _IsPulsing = true;
        AnimCoroutine = StartCoroutine(AnimPulse(254));
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
            if(brightnessVal > dimVal && _IsPulsing)
            {
                StopAnim();
                _IsPulsing = true;
                AnimCoroutine = StartCoroutine(AnimPulse(dimVal));
            }else{
                StopAnim();
                _IsPulsing = true;
                AnimCoroutine = StartCoroutine(AnimPulse(254));
            }
        }
    }

    public void SetLightRed()
    {
        StopAnim();
        Debug.Log("_isPulsing: " + _IsPulsing);
        Vector2 redColor = RGBToXY(255, 0, 0);
        string xyVal = "[" + redColor.x + "," + redColor.y + "]";
        string lightPayload = "{\"on\":true, \"bri\": 254, \"sat\":0, \"xy\":" + xyVal + "}";
        LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
    }

    void SetLightGreen()
    {
        StopAnim();
        Vector2 greenColor = RGBToXY(0, 255, 0);
        string xyVal = "[" + greenColor.x + "," + greenColor.y + "]";
        string lightPayload = "{\"on\":true, \"bri\": 254, \"sat\":0, \"xy\":" + xyVal + "}";
        LightPutCoroutine = StartCoroutine(LightPutRequest(_LightStateURL, lightPayload));
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



    private Vector2 RGBToXY(int r, int g, int b)
    {
        float cR = r / 255;
        float cG = g / 255;
        float cB = b / 255;

        float red = (cR > 0.04045) ? Mathf.Pow((cR + 0.055f) / (1.0f + 0.055f), 2.4f) : (cR / 12.92f);
        float green = (cG > 0.04045) ? Mathf.Pow((cG + 0.055f) / (1.0f + 0.055f), 2.4f) : (cG / 12.92f);
        float blue = (cB > 0.04045) ? Mathf.Pow((cB + 0.055f) / (1.0f + 0.055f), 2.4f) : (cB / 12.92f);

        float X = red * 0.664511f + green * 0.154324f + blue * 0.162028f;
        float Y = red * 0.283881f + green * 0.668433f + blue * 0.047685f;
        float Z = red * 0.000088f + green * 0.072310f + blue * 0.986039f;

        float finalX = X / (X + Y + Z);
        float finalY = Y / (X + Y + Z);

        return new Vector2(finalX, finalY);
    }


}
