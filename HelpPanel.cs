using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;

public class HelpPanel : MonoBehaviour
{
    public GameManager _GameManager = null;

    public GameObject _Parent = null;

    public List<GameObject> _Pages = null;

    public GameObject _LeftBtn = null;
    public GameObject _RightBtn = null;
    public GameObject _ExitBtn = null;

    int _iMaxPage;
    int _iCurPage = 0;

    HELP_TYPE _Type;

    public void Init()
    {
        for (int i = 0; i < _Pages.Count; ++i)
        {
            _Pages[i].SetActive(false);
        }

        _Parent.SetActive(false);
    }

    public void Show(HELP_TYPE type, bool bTutorial)
    {
        _Type = type;

        if (bTutorial)
            _iMaxPage = _Pages.Count - 1;
        else
            _iMaxPage = _Pages.Count;

        _Parent.SetActive(true);

        _iCurPage = 0;
        SetPage();
    }

    public void Dismiss()
    {
        if (_Type == HELP_TYPE.TUTORIAL)
        {
            if (_iCurPage == _iMaxPage - 1)
            {
                PlayerPrefs.SetInt("TUTORIAL", 0);

                UIManager._BackStep = BACK_STEP.GAME;

                _Parent.SetActive(false);
                _GameManager.GAMESTART();

                AmiscGame.AchivUnlock(0);
            }
        }
        else if (_Type == HELP_TYPE.MAIN)
        {
            UIManager._BackStep = BACK_STEP.MAIN;
            _Parent.SetActive(false);
        }
        else if (_Type == HELP_TYPE.GAME)
        {
            UIManager._BackStep = BACK_STEP.PAUSE;
            _Parent.SetActive(false);
        }

    }

    public void NextPage()
    {
        if (_iCurPage < _iMaxPage - 1)
        {
            _iCurPage++;
            SetPage();
        }
    }

    public void PreviousPage()
    {
        if (_iCurPage > 0)
        {
            _iCurPage--;
            SetPage();
        }
    }

    void SetPage()
    {
        if (_iCurPage == 0)
        {
            _LeftBtn.SetActive(false);
            _RightBtn.SetActive(true);
            _ExitBtn.SetActive(false);
        }
        else if (_iCurPage == _iMaxPage - 1)
        {
            _LeftBtn.SetActive(true);
            _RightBtn.SetActive(false);
            _ExitBtn.SetActive(true);        
        }
        else
        {
            _LeftBtn.SetActive(true);
            _RightBtn.SetActive(true);
            _ExitBtn.SetActive(false);
        }

        for (int i = 0; i < _Pages.Count; ++i)
        {
            if (_iCurPage == i)
            {
                _Pages[i].SetActive(true);
            }
            else
            {
                _Pages[i].SetActive(false);
            }
        }
    }
}
