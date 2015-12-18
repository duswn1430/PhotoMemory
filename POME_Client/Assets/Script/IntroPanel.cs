using UnityEngine;
using System.Collections;
using System;
using Define;

public class IntroPanel : MonoBehaviour
{
    public enum STEP {NONE, LOGO1, LOGO2, LOADING, LOGIN, ENTER };

    public STEP _Step = STEP.NONE;

    public UIPanel _LoadingPanel = null;
    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    public UILabel _text = null;

    bool _bDone = false;

    float _fProgressTime;
    float _fWaitTime;

    bool _bGoogleBannerLoaded = false;
    bool _bGoogleInsterstitialLoaded = false;
    bool _bUnityAdsLoaded = false;
    bool _bAD = false;

    float _fAlpha = 1;

    string _sStep = "";

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        StartCoroutine(EnterGame());
#else
        _Step = STEP.LOGO1;
        WaitTimeReset(1f);
        StartCoroutine(IntroProcess());
#endif

    }

    bool WaitTime()
    {
        if (Time.time - _fProgressTime > _fWaitTime)
        {
            return true;
        }
        return false;
    }

    void WaitTimeReset(float wait)
    {
        _fProgressTime = Time.time;
        _fWaitTime = wait;
    }

    IEnumerator IntroProcess()
    {
        while (_bDone == false)
        {
            if (WaitTime())
            {
                switch (_Step)
                {
                    case STEP.LOGO1:
                        {
                            _text.text = "LOGO 1";

                            _Step = STEP.LOGO2;

                            WaitTimeReset(2f);
                        }
                        break;
                    case STEP.LOGO2:
                        {
                            _text.text = "LOGO 2";

                            _Step = STEP.LOADING;

                            WaitTimeReset(2f);
                        }
                        break;
                    case STEP.LOADING:
                        {
                            _sStep = "LOADING";
                            _text.text = _sStep;

                            GoogleAds._Instance.Init();
                            StartCoroutine(GoogleLoadWait());
                            StartCoroutine(UnityAdsWait());
                            StartCoroutine(Loading());

                            _Step = STEP.NONE;
                        }
                        break;
                    case STEP.LOGIN:
                        {
                            _sStep = "LOGIN";
                            _text.text = _sStep;

                            GameService._Instance.HandlePlayerConnected += new Action(Enter);
                            GameService._Instance.Connect();

                            _Step = STEP.NONE;
                        }
                        break;
                    case STEP.ENTER:
                        {
                            _sStep = "ENTER";
                            _text.text = _sStep;

                            StartCoroutine(EnterGame());

                            _bDone = true;
                        }
                        break;
                }
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator GoogleLoadWait()
    {
        while (_bGoogleBannerLoaded == false)
        {
            if (GoogleAds._Instance.IsBannerLoaded())
            {
                _bGoogleBannerLoaded = true;
                GoogleAds._Instance.HideBanner();

                _sStep += ".";
                _text.text = _sStep;
            }
            yield return new WaitForEndOfFrame();
        }

        while (_bGoogleInsterstitialLoaded == false)
        {
            if (GoogleAds._Instance._bInterstitialLoaded)
            {
                _bGoogleInsterstitialLoaded = true;

                _sStep += ".";
                _text.text = _sStep;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator UnityAdsWait()
    {
        while (_bUnityAdsLoaded == false)
        {
            if (UnityAds._Instance.IsLoaded())
            {
                _bUnityAdsLoaded = true;

                _sStep += ".";
                _text.text = _sStep;
            }
            yield return new WaitForEndOfFrame();
        }
    }

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

    void Enter()
    {
        _Step = STEP.ENTER;
    }

    IEnumerator EnterGame()
    {
        yield return new WaitForSeconds(1f);

        BGM._Instance.Play();

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

            UIManager._BackStep = BACK_STEP.MAIN;

            _LoadingPanel.gameObject.SetActive(false);
        }
    }
}
