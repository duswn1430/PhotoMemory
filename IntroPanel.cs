using UnityEngine;
using System.Collections;
using System;
using Define;

public class IntroPanel : MonoBehaviour
{
    public enum STEP {NONE, SPLASH, LOGO, LOADING, LOGIN, ENTER };

    public STEP _Step = STEP.NONE;

    public UIManager _UIManager = null;
    public StringManager _StringManager = null;


    public StartPanel _StartPanel = null;
    public HelpPanel _HelpPanel = null;
    
    public GameObject _IntroPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    public GameObject _Loading = null;
    public GameObject _GoogleLogo = null;

    bool _bDone = false;

    bool _bGoogleBannerLoaded = false;
    bool _bGoogleInsterstitialLoaded = false;
    bool _bUnityAdsLoaded = false;
    bool _bAD = false;

    string _sStep = "";

    void Awake()
    {
        Application.targetFrameRate = 60;
    }

    // Use this for initialization
    IEnumerator Start()
    {
        _StringManager.Init();
        _HelpPanel.Init();

#if UNITY_IPHONE
        _GoogleLogo.SetActive(false);
#endif

        yield return new WaitForEndOfFrame();

        _Step = STEP.SPLASH;
        StartCoroutine(IntroProcess());
    }

    // 인트로 화면 루틴(로그인, 로딩 포함).
    IEnumerator IntroProcess()
    {
        while (_bDone == false)
        {
            switch (_Step)
            {
                case STEP.SPLASH:
                    {
                        _StringManager.SetLabelsText();

                        _GamePanel.SetActive(false);
                        _ResultPanel.SetActive(false);
                        _PausePanel.SetActive(false);

                        yield return StartCoroutine(WaitSplash());
#if UNITY_IPHONE
                        _Step = STEP.LOADING;
#else
                        _Step = STEP.LOGO;
#endif
                    }                    
                    break;
                case STEP.LOGO:
                    {
                        yield return new WaitForSeconds(2f);

                        TweenAlpha.Begin(_IntroPanel, 1f, 0);

                        yield return new WaitForSeconds(1f);

#if UNITY_EDITOR
                        _Step = STEP.ENTER;
#else
                        _Step = STEP.LOADING;
#endif
                    }
                    break;
                case STEP.LOADING:
                    {
                        _Loading.SetActive(true);
#if NO_AD
                        _Step = STEP.LOGIN;
#else
                        GoogleAds._Instance.Init();

                        StartCoroutine(GoogleBannerLoading());
                        StartCoroutine(GoogleInstertitialLoading());
                        StartCoroutine(UnityAdsLoading());

                        yield return StartCoroutine(Loading());
#endif
                    }
                    break;
                case STEP.LOGIN:
                    {
                        _Step = STEP.NONE;

                        GameService._Instance.HandlePlayerConnected += new Action(Enter);
                        GameService._Instance.HandlePlayerDisconnected += new Action(Enter);
                        GameService._Instance.Connect();

                    }
                    break;
                case STEP.ENTER:
                    {
                        StartCoroutine(EnterGame());

                        _Step = STEP.NONE;
                        _bDone = true;
                    }
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    // 스플래시 기다리기.
    IEnumerator WaitSplash()
    {
        while (Application.isShowingSplashScreen)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    // 구글 광고 로딩(배너).
    IEnumerator GoogleBannerLoading()
    {
        while (_bGoogleBannerLoaded == false)
        {
            if (GoogleAds._Instance.IsBannerLoaded())
            {
                _bGoogleBannerLoaded = true;
                GoogleAds._Instance.HideBanner();
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // 구글 광고 로딩(전면).
    IEnumerator GoogleInstertitialLoading()
    {
        while (_bGoogleInsterstitialLoaded == false)
        {
            if (GoogleAds._Instance._bInterstitialLoaded)
            {
                _bGoogleInsterstitialLoaded = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // 유니티 광고 로딩.
    IEnumerator UnityAdsLoading()
    {
        while (_bUnityAdsLoaded == false)
        {
            if (UnityAds._Instance.IsLoaded())
            {
                _bUnityAdsLoaded = true;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    // 모든 광고 로딩 끝나면 로그인 하기.
    IEnumerator Loading()
    {
        while (_bAD == false)
        {
            if (_bGoogleBannerLoaded && _bGoogleInsterstitialLoaded && _bUnityAdsLoaded)
            {
                _bAD = true;
                _Step = STEP.LOGIN;
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    // 게임화면 입장.
    void Enter()
    {
        _Step = STEP.ENTER;
    }

    // 게임화면 입장.
    IEnumerator EnterGame()
    {
        BGM._Instance.PlayT5();

        yield return new WaitForSeconds(0.5f);

        _Loading.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        _StartPanel.MainLaunch();

        _IntroPanel.SetActive(false);
    }
}
