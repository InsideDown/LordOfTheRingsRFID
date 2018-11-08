using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivendellKeyListener : MonoBehaviour
{
    //one ring key - 880006529255
    public RivendellController RivendellController;
    //public List<string> OneRingKeys = new List<string>();

    private string _CurInputStr = "";
    private float _StopTypingTime;
    private float _CurTime;
    //if there is a pause of this many seconds, we stopped typing
    private float _PauseTime = 0.5f;
    private List<VideoItemModel> VideoItemsList;


    private void Start()
    {
        VideoItemsList = new List<VideoItemModel>(RivendellController.VideoItems);
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeyCode();
    }

    /// <summary>
    /// We know our final keyed in value, check against our keys to see what we should do
    /// </summary>
    void CheckFinalCode()
    {
        for (int i = 0; i < VideoItemsList.Count;i++)
        {
            VideoItemModel curVideoItem = VideoItemsList[i];
            if(_CurInputStr == curVideoItem.KeyIDStr)
            {
                curVideoItem.MethodToFire.Invoke();
                //EventManager.Instance.SauronVideoStart();
            }
        }
    }

    private void OnGUI()
    {
        Event curEvent = Event.current;
        if(curEvent.isKey)
        {
            if (curEvent.type == EventType.KeyDown)
            {
                char curChar = curEvent.character;
                if (System.Char.IsDigit(curChar))
                {
                    _StopTypingTime = Time.time;
                    _CurInputStr += curChar.ToString();
                }
            }
        }
    }

    void ResetInput()
    {
        _CurInputStr = "";
    }


    void CheckKeyCode()
    {

        _CurTime = Time.time;
           

        //we're not null, so we must have typed something
        if(_StopTypingTime != null && _CurInputStr.Length > 0)
        {
            //we have paused long enough to be considered "done" typing
            if(_CurTime - _StopTypingTime > _PauseTime)
            {
                Debug.Log(_CurInputStr);
                CheckFinalCode();
                ResetInput();
            }
            
        }
    }
}

