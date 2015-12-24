using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Define;

public class UIManager : MonoBehaviour
{
    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    //public UILabel _txtGameStart = null;

    public UIButton _btnPause = null;
    public UIButton _btnContinue = null;
    public UIButton _btnMainMenu = null;

    /* 
     * [0] : 시작 : 1001.
     * [1] : 도움말 : 1002.
     * [2] : 일시 정지 : 2001.
     * [3] : 이어 하기 : 2002.
     * [4] : 메인 메뉴 : 2003.
     * [5] : 결과 : 3001.
     * [6] : 최고 점수 : 3002.
     * [7] : 점수 : 3003.
     * [8] : 광고 보기 : 4001.
     * [9] : 광고 보기하면 15초 추가 진행 : 4002.
     * [10] : 광고 보기하면 3초 정답 확인 : 4003.
     * 
     */
    public List<UILabel> _ListFont = null;

    public static BACK_STEP _BackStep;

    public void StringInit()
    {
        _ListFont[0].text = StringData._Instance.GetText(1001);
        //_ListFont[1].text = StringData._Instance.GetText(1002);
        //_ListFont[2].text = StringData._Instance.GetText(2001);
        //_ListFont[3].text = StringData._Instance.GetText(2002);
        //_ListFont[4].text = StringData._Instance.GetText(2003);
        //_ListFont[5].text = StringData._Instance.GetText(3001);
        //_ListFont[6].text = StringData._Instance.GetText(3002);
        //_ListFont[7].text = StringData._Instance.GetText(3003);
        //_ListFont[8].text = StringData._Instance.GetText(4001);
        //_ListFont[9].text = StringData._Instance.GetText(4002);
        //_ListFont[10].text = StringData._Instance.GetText(4003);
    }

    public void START()
    {
        _BackStep = BACK_STEP.GAME;

        _StartPanel.gameObject.SetActive(false);
        _GamePanel.gameObject.SetActive(true);
        
#if !UNITY_EDITOR
        GoogleAds._Instance.ShowBanner();
#endif
    }

    public void HELP()
    {
        Debug.Log("????????????");
    }

    public void PAUSE()
    {
        _BackStep = BACK_STEP.PAUSE;

        _PausePanel.gameObject.SetActive(true);
    }

    public void CONTINUE()
    {
        _BackStep = BACK_STEP.GAME;

        _PausePanel.gameObject.SetActive(false);
    }

    public void MAINMENU()
    {
        _BackStep = BACK_STEP.MAIN;

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
#if !UNITY_EDITOR
        GameService._Instance.ShowLeaderBoard();
#endif
    }

    public void SOUND()
    {
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
                _BackStep = BACK_STEP.AD;

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
        _BackStep = BACK_STEP.AD;

        UnityAds._Instance.OnAdFinished += new Action(GameManager._Instance.ADContinue);
        UnityAds._Instance.ShowRewardedAd();
#endif
    }

    void Update()
    {
        UpdateBack();
    }

    void UpdateBack()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (_BackStep)
            {
                case BACK_STEP.QUIT:
                    {
                        Application.Quit();
                    }
                    break;
                case BACK_STEP.MAIN:
                    {
                        _BackStep = BACK_STEP.QUIT;

                        // 토스트 띄우기.

                        Invoke("CancelQuit", 3.0f);
                    }
                    break;
                case BACK_STEP.GAME:
                    {
                        _btnPause.OnClick();
                    }
                    break;
                case BACK_STEP.PAUSE:
                    {
                        _btnContinue.OnClick();
                    }
                    break;
                case BACK_STEP.RESULT:
                    {
                        _btnMainMenu.OnClick();
                    }
                    break;
            }
        }
    }

    void CancelQuit()
    {
        _BackStep = BACK_STEP.MAIN;

        // 토스트 감추기.
    }
}
