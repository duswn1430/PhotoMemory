using UnityEngine;
using System.Collections;
using System;
using Define;

public class UIManager : MonoBehaviour
{
    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    public UILabel _txtGameStart = null;

    public UIButton _btnPause = null;
    public UIButton _btnContinue = null;
    public UIButton _btnMainMenu = null;

    public static BACK_STEP _BackStep;

    public void Init()
    {
        _txtGameStart.text = StringData._Instance.GetText(1001);
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
