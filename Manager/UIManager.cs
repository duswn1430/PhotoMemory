using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Define;

public class UIManager : MonoBehaviour
{
    public GameManager _GameManager = null;

    public HelpPanel _HelpPanel = null;

    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

    public GameObject[] _btnSounds = null;

    public UIButton _btnPause = null;
    public UIButton _btnContinue = null;
    public UIButton _btnMainMenu = null;

    public static BACK_STEP _BackStep;

    public void START()
    {
        //PlayerPrefs.DeleteKey("TUTORIAL");
        int tutorial = PlayerPrefs.GetInt("TUTORIAL", 1);

        _StartPanel.gameObject.SetActive(false);
        _GamePanel.gameObject.SetActive(true);

        if (tutorial == 1)
        {
            _BackStep = BACK_STEP.HELP;

            _HelpPanel.Show(HELP_TYPE.TUTORIAL, true);
        }
        else
        {
            _BackStep = BACK_STEP.GAME;

            _GameManager.GAMESTART();
        }      
    }

    public void MAIN_HELP()
    {
        _BackStep = BACK_STEP.HELP;

        _HelpPanel.Show(HELP_TYPE.MAIN, false);
    }

    public void GAME_HELP()
    {
        _BackStep = BACK_STEP.HELP;

        _HelpPanel.Show(HELP_TYPE.GAME, false);
    }

    public void HELP_EXIT()
    {
        _HelpPanel.Dismiss();
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
        
//#if !UNITY_EDITOR
//        GoogleAds._Instance.HideBanner();
//#endif
    }

    public void RANK()
    {
#if !UNITY_EDITOR
        if (GameService._Instance.IsConnected())
            GameService._Instance.ShowLeaderBoard();
#endif
    }

    public void SOUND()
    {
        string sLabel = "";
        string sButton = "";

        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
            sLabel = "btn_volume_off";
            sButton = "btn_volume_off";
        }
        else
        {
            AudioListener.volume = 1;
            sLabel = "btn_volume_on";
            sButton = "btn_volume_on";
        }

        for (int i = 0; i < _btnSounds.Length; ++i)
        {
            UISprite label = _btnSounds[i].GetComponent<UISprite>();
            UIButton button = _btnSounds[i].GetComponent<UIButton>();

            label.spriteName = sLabel;
            button.normalSprite = sButton;            
        }            
    }

    public void SHOW_ORIGINAL()
    {
        if (GameManager._Step == GameManager.STEP.PLAY)
        {
#if UNITY_EDITOR || NO_AD
            if(GameManager._iOriginCnt > 0)
            {
                GameManager._Instance.ShowOriginal();
            }
#else
            if (GameManager._iOriginCnt > 0)
            {
                if (GoogleAds._Instance._bInterstitialLoaded)
                {
                    _BackStep = BACK_STEP.AD;

                    GameManager._Instance._Timer.TimerStop();

                    if (AudioListener.volume > 0)
                    {
                        AudioListener.volume = 0;
                        GoogleAds._Instance.OnInterstitialClosed += new Action(SoundOn);
                    }

                    GoogleAds._Instance.OnInterstitialClosed += new Action(GameManager._Instance.ShowOriginal);
                    GoogleAds._Instance.ShowInterstital();
                }
            }
#endif
        }
    }

    public void AD_CONTINUE()
    {
#if UNITY_EDITOR || NO_AD
        if (GameManager._iADContinue > 0)
        {
            GameManager._Instance.ADContinue();
        }
#else
        if(GameManager._iADContinue > 0)
        {
            _BackStep = BACK_STEP.AD;

            if (AudioListener.volume > 0)
            {
                AudioListener.volume = 0;
                UnityAds._Instance.OnAdFinished += new Action(SoundOn);
            }

            UnityAds._Instance.OnAdFinished += new Action(GameManager._Instance.ADContinue);
            UnityAds._Instance.ShowRewardedAd();
        }
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
                case BACK_STEP.HELP:
                    {
                        _HelpPanel.Dismiss();
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

    public void SoundOn()
    {
        AudioListener.volume = 1;
    }
}
