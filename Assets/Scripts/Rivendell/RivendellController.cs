using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Linq;

public class RivendellController : MonoBehaviour {

    public AudioSource RivendellAudio;
    public VideoPlayer CameraVideoPlayer;
    public VideoClip RivendellVideoClip;
    public VideoClip MordorVideoClip;
    public GameObject ParticleSystemHolder;
    public MainClueController MainClueController;
    public List<GameObject> ClueGameObjects = new List<GameObject>();

    [HideInInspector]
    public List<VideoItemModel> VideoItems;
  

    //private bool _IsMordor = false;
    private bool _IsExternalClipPlaying = false;
    private bool _IsOneRingFound = false;
    private float _CluePauseTime = 8.0f;


    private void Awake()
    {
        foreach(GameObject curClue in ClueGameObjects){
            curClue.SetActive(false);
        }

        VideoItems = this.gameObject.GetComponents<VideoItemModel>().ToList();
     
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
        EventManager.Instance.RingPlacedEvent(curVideoItemModel);
        if (curVideoItemModel.ClueObj != null)
        {
            StartCoroutine(DisplayClue(curVideoItemModel, curVideoItemModel.IsClue));
        }
        else
        {
            _IsOneRingFound = curVideoItemModel.IsOneRing;
            EventManager.Instance.VideoStartEvent(curVideoItemModel);
        }
    }

    IEnumerator DisplayClue(VideoItemModel curVideoItemModel, bool isClue)
    {
        float animSpeed = 0.4f;
 
        GameObject clueGameObj = curVideoItemModel.ClueObj;
        CanvasGroup clueCanvas = clueGameObj.GetComponent<CanvasGroup>();
        if (clueCanvas != null)
            clueCanvas.alpha = 0;
        clueCanvas.DOFade(1.0f, animSpeed);
        clueGameObj.transform.DOLocalMoveY(-200f, 0f);
        clueGameObj.transform.DOLocalRotate(new Vector3(0f,50f,0f), 0f);
        clueGameObj.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), animSpeed).SetDelay(0.1f);
        clueGameObj.transform.DOLocalMoveY(0, animSpeed).SetDelay(0.1f).SetEase(Ease.OutBack);
        DOTween.To(() => CameraVideoPlayer.targetCameraAlpha, x => CameraVideoPlayer.targetCameraAlpha = x, 0f, animSpeed);
        clueGameObj.SetActive(true);
        yield return new WaitForSeconds(_CluePauseTime);
        clueGameObj.SetActive(false);
        //if we're a clue we get a random video clip instead of the assigned one
        if (isClue)
        {
            //determine the video clip to use
            VideoClip videoClip = MainClueController.GetVideoById(curVideoItemModel.KeyIDStr);
            curVideoItemModel.VideoSource = videoClip;
        }

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

            QualitySettings.vSyncCount = 2;
            ParticleSystemHolder.SetActive(false);
            _IsExternalClipPlaying = true;
            CameraVideoPlayer.clip = curVideoClip;
            CameraVideoPlayer.isLooping = false;
            CameraVideoPlayer.Play();
            DOTween.To(() => CameraVideoPlayer.targetCameraAlpha, x => CameraVideoPlayer.targetCameraAlpha = x, 1f, 0.2f);
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
            QualitySettings.vSyncCount = 0;
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
