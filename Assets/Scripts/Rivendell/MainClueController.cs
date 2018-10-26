using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MainClueController : MonoBehaviour {

    public List<VideoClip> VideoSources = new List<VideoClip>();


    private string TotalClaimedVideosKey = "TotalClaimedVideosKey";

    /// <summary>
    /// Gets the video by identifier. Check local storage to see if we have already stored a video with this ID. If so, return that.
    /// If not, store this video against this ID
    /// </summary>
    /// <returns>The video by identifier.</returns>
    /// <param name="keyID">Key identifier.</param>
    public VideoClip GetVideoById(string keyID)
    {
        int curVideo;
        //if we already have a key associated with this ID, use that
        if(PlayerPrefs.HasKey(keyID))
        {
            curVideo = PlayerPrefs.GetInt(keyID);
        }else{
            //we don't have an item saved to local that matches this ID
            curVideo = SetPlayerPrefVideo(keyID);
        }
        //return the video that aligns with that index
        return VideoSources[curVideo];
    }

    private int SetPlayerPrefVideo(string keyID)
    {
        int totalVideoCount;
        if (PlayerPrefs.HasKey(TotalClaimedVideosKey))
        {
            totalVideoCount = PlayerPrefs.GetInt(TotalClaimedVideosKey);
            totalVideoCount++;
        }
        else
        {
            totalVideoCount = 0;
        }
        PlayerPrefs.SetInt(TotalClaimedVideosKey, totalVideoCount);
        PlayerPrefs.SetInt(keyID, totalVideoCount);
        return totalVideoCount;
    }

}
