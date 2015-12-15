using UnityEngine;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    public GameObject _StartPanel = null;
    public GameObject _GamePanel = null;
    public GameObject _ResultPanel = null;
    public GameObject _PausePanel = null;

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
