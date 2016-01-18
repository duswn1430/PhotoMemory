﻿using UnityEngine;
using System.Collections;
using System;
using Define;

public class IntroPanel : MonoBehaviour
{
    public enum STEP {NONE, LOGO1, LOADING, LOGIN, ENTER };

    public STEP _Step = STEP.NONE;

    public UIManager _UIManager = null;


    public StartPanel _StartPanel = null;
    
    public GameObject _LoadingPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    public UILabel _UILoading = null;

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
        StringData._Instance.LoadStringData();
        StringData._Instance.SetLanguage();
        //StringData._LANGUAGE = LANGUAGE.EN;
            
        yield return new WaitForEndOfFrame();

        _Step = STEP.LOGO1;
        StartCoroutine(IntroProcess());
    }

    // 인트로 화면 루틴(로그인, 로딩 포함).
    IEnumerator IntroProcess()
    {
        while (_bDone == false)
        {
            switch (_Step)
            {
                case STEP.LOGO1:
                    {
                        _UIManager.StringInit();

                        _GamePanel.SetActive(false);
                        _ResultPanel.SetActive(false);
                        _PausePanel.SetActive(false);

                        yield return new WaitForSeconds(2f);

                        TweenAlpha.Begin(_LoadingPanel, 1f, 0);

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
                        GoogleAds._Instance.Init();

                        StartCoroutine(GoogleBannerLoading());
                        StartCoroutine(GoogleInstertitialLoading());
                        StartCoroutine(UnityAdsLoading());

                        yield return StartCoroutine(Loading());
                    }
                    break;
                case STEP.LOGIN:
                    {
                        _sStep += ".";
                        _UILoading.text = _sStep;

                        _Step = STEP.NONE;

                        GameService._Instance.HandlePlayerConnected += new Action(Enter);
                        GameService._Instance.HandlePlayerDisconnected += new Action(Enter);
                        GameService._Instance.Connect();

                    }
                    break;
                case STEP.ENTER:
                    {
                        _sStep += ".";
                        _UILoading.text = _sStep;

                        StartCoroutine(EnterGame());

                        _Step = STEP.NONE;
                        _bDone = true;
                    }
                    break;
            }

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

                _sStep += ".";
                _UILoading.text = _sStep;
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

                _sStep += ".";
                _UILoading.text = _sStep;
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

                _sStep += ".";
                _UILoading.text = _sStep;
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
        BGM._Instance.Play();

        yield return new WaitForSeconds(0.5f);

        _UILoading.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        _StartPanel.MainLaunch();

        _UILoading.gameObject.SetActive(false);
        _LoadingPanel.SetActive(false);
    }
}