using UnityEngine;
using System.Collections;
using Define;
using System;

public class ResultPopup : MonoBehaviour
{
    public GameObject _goContinue = null;

    public UILabel _uiBestScore = null;
    public UILabel _uiCurScore = null;
    
    public void SetResult(long bestScore, int curScore)
    {
        if (GameManager._iADContinue > 0)
            _goContinue.SetActive(true);
        else
            _goContinue.SetActive(false);

        _uiBestScore.text = bestScore.ToString();
        _uiCurScore.text = curScore.ToString();
        
#if !UNITY_EDITOR
        Invoke("ShowAd", 0.5f);
#endif
    }

    void ShowAd()
    {
        UIManager._BackStep = BACK_STEP.AD;

        if (AudioListener.volume > 0)
        {
            AudioListener.volume = 0;
            GoogleAds._Instance.OnInterstitialClosed += new Action(SoundOn);
        }
        GoogleAds._Instance.OnInterstitialClosed += new Action(() =>
        {
            UIManager._BackStep = BACK_STEP.RESULT;
        });
        GoogleAds._Instance.ShowInterstital();
    }

    public void SoundOn()
    {
        AudioListener.volume = 1;
    }
}
