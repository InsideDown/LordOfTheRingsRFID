using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class RivendellController : MonoBehaviour {

    public AudioSource RivendellAudio;
    public VideoPlayer CameraVideoPlayer;
    public VideoClip RivendellVideoClip;
    public VideoClip MordorVideoClip;
    public GameObject ParticleSystemHolder;
    public MainClueController MainClueController;

    public List<VideoItemModel> VideoItems = new List<VideoItemModel>();
    public List<GameObject> ClueGameObjects = new List<GameObject>();
  

    //private bool _IsMordor = false;
    private bool _IsExternalClipPlaying = false;
    private bool _IsOneRingFound = false;
    private float _CluePauseTime = 8.0f;


    private void Awake()
    {
        foreach(GameObject curClue in ClueGameObjects){
            curClue.SetActive(false);
        }
        PlayRivendell();
    }

    private void OnEnable()
    {
        CameraVideoPlayer.loopPointReached += CameraVideoPlayer_LoopPointReached;
        EventManager.OnVideoStartEvent += EventManager_OnVideoStartEvent;
    }

    private void OnDisable()
    {
        CameraVideoPlayer.loopPointReached -= CameraVideoPlayer_LoopPointReached;
        EventManager.OnVideoStartEvent -= EventManager_OnVideoStartEvent;
    }

    public void OnRingPlaced(VideoItemModel curVideoItemModel)
    {
        if (curVideoItemModel.ClueObj != null)
        {
            StartCoroutine(DisplayClue(curVideoItemModel));
        }
        else
        {
            _IsOneRingFound = curVideoItemModel.IsOneRing;
            EventManager.Instance.VideoStartEvent(curVideoItemModel);
        }
    }

    IEnumerator DisplayClue(VideoItemModel curVideoItemModel)
    {
        GameObject clueGameObj = curVideoItemModel.ClueObj;
        CanvasGroup clueCanvas = clueGameObj.GetComponent<CanvasGroup>();
        if (clueCanvas != null)
            clueCanvas.alpha = 0;
        clueCanvas.DOFade(1.0f, 0.3f);
        clueGameObj.SetActive(true);
        yield return new WaitForSeconds(_CluePauseTime);
        clueGameObj.SetActive(false);
        //determine the video clip to use
        VideoClip videoClip = MainClueController.GetVideoById(curVideoItemModel.KeyIDStr);
        curVideoItemModel.VideoSource = videoClip;

        EventManager.Instance.VideoStartEvent(curVideoItemModel);
    }


    void EventManager_OnVideoStartEvent(VideoItemModel curVideoItem)
    {
        PlayExternalClip(curVideoItem.VideoSource);
    }

    void PlayExternalClip(VideoClip curVideoClip)
    {
        if (curVideoClip != null)
        {
            if (RivendellAudio.isPlaying)
                RivendellAudio.Pause();

            ParticleSystemHolder.SetActive(false);
            _IsExternalClipPlaying = true;
            CameraVideoPlayer.clip = curVideoClip;
            CameraVideoPlayer.isLooping = false;
            CameraVideoPlayer.Play();
        }
    }


    void PlayRivendell()
    {
        //_IsMordor = false;
        ParticleSystemHolder.SetActive(true);
        _IsExternalClipPlaying = false;

        if (!RivendellAudio.isPlaying)
            RivendellAudio.Play();

        CameraVideoPlayer.clip = RivendellVideoClip;
        CameraVideoPlayer.isLooping = true;
        CameraVideoPlayer.Play();
    }

    void CameraVideoPlayer_LoopPointReached(VideoPlayer source)
    {
        if(_IsExternalClipPlaying)
        {
            _IsExternalClipPlaying = false;
            EventManager.Instance.VideoEndEvent();
            if(_IsOneRingFound)
            {
                SceneManager.LoadScene(GlobalVars.Instance.EndScene);    
            }else{
                PlayRivendell();    
            }

        }
    }

}
