using UnityEngine;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public UIPanel _StartPanel = null;
    public UIPanel _GamePanel = null;
    public UIPanel _ResultPanel = null;
    public UIPanel _PausePanel = null;

    public UIButton _btnStart = null;
    public UIButton _btnHelp = null;

    bool _bGoogleLoaded = false;
    bool _bUnityAdsLoaded = false;

    // Use this for initialization
    void Start()
    {
        _StartPanel.gameObject.SetActive(true);
        _GamePanel.gameObject.SetActive(false);
        _ResultPanel.gameObject.SetActive(false);
        _PausePanel.gameObject.SetActive(false);
        
#if UNITY_EDITOR        

        _btnStart.gameObject.SetActive(true);
        _btnHelp.gameObject.SetActive(true);

#else

        GoogleAds._Instance.Init();
        StartCoroutine(GoogleLoadWait());

#endif
    }

    IEnumerator GoogleLoadWait()
    {
        while (_bGoogleLoaded == false)
        {
            //if (GoogleAds._Instance.IsBannerLoaded())
            if(GoogleAds._Instance._bInterstitialLoaded)
            {
                _bGoogleLoaded = true;
                GoogleAds._Instance.HideBanner();

                _btnStart.gameObject.SetActive(true);
                _btnHelp.gameObject.SetActive(true);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    //IEnumerator UnityAdsWait()
    //{
    //    while (_bUnityAdsLoaded == false)
    //    {
    //        if(UnityAds._Instance.isActiveAndEnabled)

    //        yield return new WaitForFixedUpdate();
    //    }
    //}

    public void START()
    {
        _StartPanel.gameObject.SetActive(false);
        _GamePanel.gameObject.SetActive(true);

        GoogleAds._Instance.ShowBanner();
    }

    public void HELP()
    {
        //Debug.Log("????????????");
        UnityAds._Instance.ShowRewardedAd();
    }

    public void PAUSE()
    {
        _PausePanel.gameObject.SetActive(true);
    }

    public void CONTINUE()
    {
        _PausePanel.gameObject.SetActive(false);
    }

    public void MAINMENU()
    {
        _StartPanel.gameObject.SetActive(true);
        _GamePanel.gameObject.SetActive(false);
        _ResultPanel.gameObject.SetActive(false);
        _PausePanel.gameObject.SetActive(false);

        GoogleAds._Instance.HideBanner();
    }

    public void RANK()
    {
        Debug.Log("????????????");
    }

    public void SOUND()
    {
        Debug.Log("????????????");
    }

    public void SHOW_ORIGINAL()
    {
        if (GameManager._Step == GameManager.STEP.PLAY)
        {
#if UNITY_EDITOR
            GameManager._Instance.ShowOriginal();
#else
            if (GoogleAds._Instance._bInterstitialLoaded)
            {
                GameManager._Instance._Timer.TimerStop();
                GoogleAds._Instance.OnInterstitialClosed += new Action(GameManager._Instance.ShowOriginal);
                GoogleAds._Instance.ShowInterstital();
            }
#endif
        }
    }
    


}
