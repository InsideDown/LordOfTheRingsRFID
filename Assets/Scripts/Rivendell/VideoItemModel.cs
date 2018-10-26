using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoItemModel: MonoBehaviour  {

    public string KeyIDStr;
    public bool IsOneRing = false;
    public VideoClip VideoSource;
    public Color LightRGB;
    public UnityEvent MethodToFire;
    public GameObject ClueObj;

}
