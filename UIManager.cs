using UnityEngine;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public UIPanel _LoadingPanel = null;
    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    bool _bGoogleBannerLoaded = false;
    bool _bGoogleInsterstitialLoaded = false;
    bool _bUnityAdsLoaded = false;

    float _fAlpha = 1;

    void Awake()
    {
#if !UNITY_EDITOR
        GoogleAds._Instance.Init();
        StartCoroutine(GoogleLoadWait());
        StartCoroutine(UnityAdsWait());
#endif
    }
    

    // Use this for initialization
    void Start()
    {
        BGM._Instance.Play();

#if UNITY_EDITOR
        StartCoroutine(EnterGame());
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
                StartCoroutine(EnterGame());
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }

    }

    IEnumerator EnterGame()
    {
        yield return new WaitForSeconds(1f);

        _GamePanel.SetActive(false);
        _ResultPanel.SetActive(false);
        _PausePanel.SetActive(false);

        while (_fAlpha >= 0)
        {
            _fAlpha -= 0.1f;
            _LoadingPanel.alpha = _fAlpha;
            yield return new WaitForSeconds(0.1f);
        }

        if (_fAlpha <= 0)
        {
            _fAlpha = 0f;
            _LoadingPanel.alpha = _fAlpha;

            _LoadingPanel.gameObject.SetActive(false);
        }
    }

    public void START()
    {
        _StartPanel.gameObject.SetActive(false);
        _GamePanel.gameObject.SetActive(true);
        
#if !UNITY_EDITOR
        GoogleAds._Instance.ShowBanner();
#endif
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
