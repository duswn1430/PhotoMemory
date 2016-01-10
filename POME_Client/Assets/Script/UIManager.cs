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

    public GameObject _btnSound = null;

    public UIButton _btnPause = null;
    public UIButton _btnContinue = null;
    public UIButton _btnMainMenu = null;

    

    public Font _fontPen = null;
    public Font _fontGothic = null;

    public List<UILabel> _ListFont = null;

    public static BACK_STEP _BackStep;

    public void StringInit()
    {
        for (int i = 0; i < _ListFont.Count; ++i)
        {
            if(StringData._LANGUAGE == LANGUAGE.CN || StringData._LANGUAGE == LANGUAGE.JP)
            {
                _ListFont[i].trueTypeFont = _fontGothic;
            }
            else
            {
                _ListFont[i].trueTypeFont = _fontPen;
            }
        }

        _ListFont[0].text = StringData._Instance.GetText(1001); // 시작.
        _ListFont[1].text = StringData._Instance.GetText(1002); // 도움말.
        _ListFont[2].text = "Clear"; // Clear.
        _ListFont[3].text = StringData._Instance.GetText(3002); // 최고 점수.
        _ListFont[4].text = StringData._Instance.GetText(3003); // 점수.
        _ListFont[5].text = "CONTINUE"; // 계속.
        _ListFont[6].text = StringData._Instance.GetText(2003); // 메인 메뉴.
        _ListFont[7].text = StringData._Instance.GetText(3001); // 결과.
        _ListFont[8].text = StringData._Instance.GetText(2001); // 일시 정지.
        _ListFont[9].text = StringData._Instance.GetText(2002); // 이어 하기.
        _ListFont[10].text = StringData._Instance.GetText(1002);// 도움말.
        _ListFont[11].text = StringData._Instance.GetText(2003);// 메인 메뉴.
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
        UISprite label = _btnSound.GetComponent<UISprite>();
        UIButton button = _btnSound.GetComponent<UIButton>();

        if (AudioListener.volume >= 1)
        {
            AudioListener.volume = 0;
            label.spriteName = "btn_volume_off";
            button.normalSprite = "btn_volume_off";
        }
        else
        {
            AudioListener.volume = 1;
            label.spriteName = "btn_volume_on";
            button.normalSprite = "btn_volume_on";
        }
            
    }

    public void SHOW_ORIGINAL()
    {
        if (GameManager._Step == GameManager.STEP.PLAY)
        {
#if UNITY_EDITOR
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

                    if (AudioListener.volume >= 1)
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
#if UNITY_EDITOR
        if (GameManager._iADContinue > 0)
        {
            GameManager._Instance.ADContinue();
        }
#else
        if(GameManager._iADContinue > 0)
        {
            _BackStep = BACK_STEP.AD;

            if (AudioListener.volume >= 1)
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
