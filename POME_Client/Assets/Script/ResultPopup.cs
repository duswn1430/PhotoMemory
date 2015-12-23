using UnityEngine;
using System.Collections;

public class ResultPopup : MonoBehaviour
{

    public UILabel _uiBestScore = null;
    public UILabel _uiCurScore = null;
    
    public void SetResult(long bestScore, int curScore)
    {
        _uiBestScore.text = bestScore.ToString();
        _uiCurScore.text = curScore.ToString();
    }
}
