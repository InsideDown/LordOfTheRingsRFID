using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager> {

    protected EventManager() {}

    public delegate void VideoAction();
    public static event VideoAction OnVideoPlayEvent;
    public static event VideoAction OnVideoEndEvent;
    public static event VideoAction OnSauronVideoStartEvent;
    public static event VideoAction OnSauronVideoEndEvent;

    public delegate void VideoStartAction(VideoItemModel curVideoItem);
    public static event VideoStartAction OnVideoStartEvent;

    public delegate void KeyboardAction();
    public static event KeyboardAction OnRedTeamWin;
    public static event KeyboardAction OnGreenTeamWin;

    public void VideoPlayEvent()
    {
        if (OnVideoPlayEvent != null)
            OnVideoPlayEvent();
    }

    public void RedTeamWin()
    {
        if (OnRedTeamWin != null)
            OnRedTeamWin();
    }

    public void GreenTeamWin()
    {
        if (OnGreenTeamWin != null)
            OnGreenTeamWin();
    }

    public void VideoStartEvent(VideoItemModel curVideoItem)
    {
        if (OnVideoStartEvent != null)
            OnVideoStartEvent(curVideoItem);
    }

    public void VideoEndEvent()
    {
        if (OnVideoEndEvent != null)
            OnVideoEndEvent();
    }

    public void SauronVideoStart() 
    {
        if (OnSauronVideoStartEvent != null)
            OnSauronVideoStartEvent();    
    }

    public void SauronVideoEnd()
    {
        if (OnSauronVideoEndEvent != null)
            OnSauronVideoEndEvent();
    }
}
