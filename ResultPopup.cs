using UnityEngine;
using System.Collections;

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
    }
}
