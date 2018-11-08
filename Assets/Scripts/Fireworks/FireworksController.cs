using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireworksController : MonoBehaviour {

    public GameObject CongratsTitle;
    public GameObject CongratsBody;
    public float ReturnToRivendellTime = 60.0f;

    private CanvasGroup CongratsTitleCanvas;
    private CanvasGroup CongratsBodyCanvas;

    private void Awake()
    {
        if (CongratsTitle == null)
            throw new Exception("A CongratsTitle must be defined");

        if (CongratsBody == null)
            throw new Exception("A CongratsBody must be defined");

        var fieldValues = this.GetType().GetFields().ToList();

        Debug.Log(fieldValues);

        CongratsTitleCanvas = CongratsTitle.GetComponent<CanvasGroup>();
        CongratsBodyCanvas = CongratsBody.GetComponent<CanvasGroup>();

        CongratsTitleCanvas.alpha = 0;
        CongratsBodyCanvas.alpha = 0;

    }

    // Use this for initialization
    void Start () {
        AnimText();
        StartCoroutine(ReturnToRivendell());
	}

    void AnimText()
    {
        float animSpeed = 0.6f;
        float delayStart = 2.5f;

        float titleYPos = CongratsTitle.transform.localPosition.y;
        float bodyYPos = CongratsBody.transform.localPosition.y;

        CongratsTitleCanvas.DOFade(1.0f, animSpeed).SetDelay(delayStart);
        CongratsBodyCanvas.DOFade(1.0f, animSpeed).SetDelay(delayStart + 0.5f);

        CongratsTitle.transform.DOLocalMoveY(titleYPos - 200f, 0f);
        CongratsTitle.transform.DOLocalMoveY(titleYPos, animSpeed).SetDelay(delayStart).SetEase(Ease.OutBack);

        CongratsBody.transform.DOLocalMoveY(bodyYPos - 200f, 0f);
        CongratsBody.transform.DOLocalMoveY(bodyYPos, animSpeed).SetDelay(delayStart + 0.5f).SetEase(Ease.OutBack);
    }

    IEnumerator ReturnToRivendell() 
    {
        yield return new WaitForSeconds(ReturnToRivendellTime);
        SceneManager.LoadScene(GlobalVars.Instance.MainScene);  
    }
	
	
}
