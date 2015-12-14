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
    public UIButton _btnAudio = null;
    public UIButton _btnRank = null;

    bool _bGoogleBannerLoaded = false;
    bool _bGoogleInsterstitialLoaded = false;
    bool _bUnityAdsLoaded = false;

    // Use this for initialization
    void Start()
    {
        BGM._Instance.Play();

        _StartPanel.gameObject.SetActive(true);
        _GamePanel.gameObject.SetActive(false);
        _ResultPanel.gameObject.SetActive(false);
        _PausePanel.gameObject.SetActive(false);
        
#if UNITY_EDITOR        

        _btnStart.gameObject.SetActive(true);
        _btnHelp.gameObject.SetActive(true);
        _btnAudio.gameObject.SetActive(true);
        _btnRank.gameObject.SetActive(true);

#else

        GoogleAds._Instance.Init();
        StartCoroutine(GoogleLoadWait());
        StartCoroutine(UnityAdsWait());
        StartCoroutine(Loading());

#endif
    }

    IEnumerator GoogleLoadWait()
    {
        while (_bGoogleBannerLoaded == false)
        {
            if(GoogleAds._Instance.IsBannerLoaded())
            {
                _bGoogleBannerLoaded = true;
                GoogleAds._Instance.HideBanner();
            }
            yield return new WaitForFixedUpdate();
        }

        while (_bGoogleInsterstitialLoaded == false)
        {
            if (GoogleAds._Instance._bInterstitialLoaded)
            {
                _bGoogleInsterstitialLoaded = true;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator UnityAdsWait()
    {
        while (_bUnityAdsLoaded == false)
        {
            if (UnityAds._Instance.IsLoaded())
            {
                _bUnityAdsLoaded = true;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Loading()
    {
        while (_bGoogleBannerLoaded == false || _bGoogleInsterstitialLoaded == false || _bUnityAdsLoaded == false)
        {
            if (_bGoogleBannerLoaded && _bGoogleBannerLoaded && _bUnityAdsLoaded)
            {
                _btnStart.gameObject.SetActive(true);
                _btnHelp.gameObject.SetActive(true);
                _btnAudio.gameObject.SetActive(true);
                _btnRank.gameObject.SetActive(true);

                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

    }

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
        
#if !UNITY_EDITOR
        GoogleAds._Instance.HideBanner();
#endif
    }

    public void RANK()
    {
        Debug.Log("????????????");
    }

    public void SOUND()
    {
        Debug.Log("????????????");
        BGM._Instance.BGMChange();
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

    public void AD_CONTINUE()
    {
#if UNITY_EDITOR
        GameManager._Instance.ADContinue();
#else
        UnityAds._Instance.OnAdFinished += new Action(GameManager._Instance.ADContinue);
        UnityAds._Instance.ShowRewardedAd();
#endif
    }
}
