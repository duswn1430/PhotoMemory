using UnityEngine;
using System.Collections;

public class ResultPopup : MonoBehaviour
{

    public UILabel _uiBestScore = null;
    public UILabel _uiCurScore = null;

    int _iBestScore = 0;
    int _iCurScore = 0;

    // Use this for initialization
    void Start()
    {
        _iBestScore = PlayerPrefs.GetInt("BestScore");
    }

    public void RANK()
    {

    }

    public void SOUND()
    {

    }
    
    public void SetResult(int score)
    {
        _iCurScore = score;

        if (_iBestScore < score)
        {
            _iBestScore = score;
            PlayerPrefs.SetInt("BestScore", _iBestScore);
        }

        _uiBestScore.text = _iBestScore.ToString();
        _uiCurScore.text = _iCurScore.ToString();
    }
}
