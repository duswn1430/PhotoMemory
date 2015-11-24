using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum STEP {INIT, PLAY, COMPLETE };

    public static STEP _Step;

    public BoxMapManager _BoxMap = null;

    public GameObject _BtnStart = null;
    public GameObject _BtnRestart = null;
    public GameObject _BtnBack = null;

    public UILabel _UIText = null;
    public UILabel _UIScore = null;

    int _iGameStage = 1;
    int _iCountTime = 3;

    bool _bGiveUp = false;
    float _fGiveUpTime = 5;
    float _fGiveUpSpendTime = 5;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    void Update()
    {
        if (_bGiveUp)
        {
            if (Time.time - _fGiveUpSpendTime > _fGiveUpTime)
            {
                _bGiveUp = false;
                _fGiveUpSpendTime = Time.time;

                SetText("Remember ???", 130);

                _BtnRestart.SetActive(true);
                _BtnBack.SetActive(true);
            }
        }
    }

    void Init()
    {
        _Step = STEP.INIT;

        _iGameStage = 1;

        SetScore(0);

        _bGiveUp = false;

        SetText("", 0);

        _BtnStart.SetActive(true);
        _BtnRestart.SetActive(false);
        _BtnBack.SetActive(false);
        
        _BoxMap.StartCoroutine(_BoxMap.InitBoxMap());
    }

    public void START()
    {
        _BoxMap.BoxColoring();

        StartCoroutine(StartProcses());

        _BtnStart.gameObject.SetActive(false);
    }

    public void RESTART()
    {
        _BoxMap.RefreshBoxMap();
        StartCoroutine(StartProcses());

        _BtnRestart.SetActive(false);
        _BtnBack.SetActive(false);
    }

    public void BACK()
    {
        _BoxMap.ClearBoxMap();

        Init();
    }

    public void COMPLETE()
    {
        _Step = STEP.COMPLETE;

        SetText(_Step.ToString(), 200);

        _bGiveUp = false;
        _fGiveUpSpendTime = Time.time;
        
        _BtnRestart.SetActive(false);
        _BtnBack.SetActive(false);

        SetScore(_iGameStage++);

        StartCoroutine(NEXT());
    }

    public IEnumerator NEXT()
    {
        yield return new WaitForSeconds(2f);

        //SetScore(_iGameStage);

        //_iGameStage++;

        yield return StartCoroutine(LevelCheck());

        _BoxMap.BoxColoring();

        StartCoroutine(StartProcses());

    }

    IEnumerator LevelCheck()
    {
        int level = AmiscGame.GetLevel(_iGameStage);

        if (_BoxMap._iLevel != level)
        {
            _BoxMap._iLevel = level;
            yield return _BoxMap.StartCoroutine(_BoxMap.GenerateBoxMap());
        }
    }

    IEnumerator StartProcses()
    {
        yield return StartCoroutine(Counting());

        yield return _BoxMap.StartCoroutine(_BoxMap.BoxShuffling());

        _Step = STEP.PLAY;

        _bGiveUp = true;
        _fGiveUpSpendTime = Time.time;

        SetText(_Step.ToString(), 200);
    }

    IEnumerator Counting()
    {
        int time = _iCountTime;
        
        while (time > 0)
        {
            SetText(time.ToString(), 250);

            yield return new WaitForSeconds(1f);

            time--;
        }

        SetText("", 0);
    }

    void SetText(string text, int size)
    {
        _UIText.fontSize = size;
        _UIText.text = text;
    }

    void SetScore(int score)
    {
        string txt = string.Format("Score : {0}", score);
        _UIScore.text = txt;
    }
}
