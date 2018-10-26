using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour {

    public VideoClip RedVideoClip;
    public VideoClip GreenVideoClip;

    public VideoPlayer CurVideoPlayer;


    private void OnEnable()
    {
        if(CurVideoPlayer != null)
            CurVideoPlayer.loopPointReached += CurVideoPlayer_LoopPointReached;
        EventManager.OnRedTeamWin += EventManager_OnRedTeamWin;
        EventManager.OnGreenTeamWin += EventManager_OnGreenTeamWin;
    }

    private void OnDisable()
    {
        if (CurVideoPlayer != null)
            CurVideoPlayer.loopPointReached -= CurVideoPlayer_LoopPointReached;
        EventManager.OnRedTeamWin -= EventManager_OnRedTeamWin;
        EventManager.OnGreenTeamWin -= EventManager_OnGreenTeamWin;
    }

    void EventManager_OnRedTeamWin()
    {
        if (CurVideoPlayer != null) 
        {
            if (RedVideoClip != null)
            {
                if (CurVideoPlayer.isPlaying)
                    CurVideoPlayer.Stop();
                CurVideoPlayer.clip = RedVideoClip;
                CurVideoPlayer.Play();
            }
        }
    }

    void EventManager_OnGreenTeamWin()
    {
        if (CurVideoPlayer != null)
        {
            if (GreenVideoClip != null)
            {
                if (CurVideoPlayer.isPlaying)
                    CurVideoPlayer.Stop();
                CurVideoPlayer.clip = GreenVideoClip;
                CurVideoPlayer.Play();
            }
        }
    }

    /// <summary>
    /// Video player has finished playing
    /// </summary>
    /// <param name="source">Source.</param>
    void CurVideoPlayer_LoopPointReached(VideoPlayer source)
    {
        if (CurVideoPlayer != null)
            CurVideoPlayer.clip = null;
        EventManager.Instance.VideoEndEvent();
    }



}
