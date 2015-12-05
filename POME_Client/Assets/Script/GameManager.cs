using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using SimpleJSON;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance = null;

    public enum STEP {START, PLAY, COMPLETE, END };

    public static STEP _Step;

    public enum START_STEP {INIT, SIZE, COLOR, COUNT, SHUFFLE, PLAY };

    START_STEP _StartStep;

    public enum NEXT_STEP { ADDTIME, NEXT };

    NEXT_STEP _NextStep;

    public BoxMapManager _BoxMapManager = null;

    public Timer _Timer = null;

    public UILabel _UIScore = null;
    public UILabel _UIStage = null;

    public ResultPopup _ResultPop = null;

    List<BoxMapData> _listMapData;
    BoxMapData _CurMapData;

    JSONNode _JsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;
    int _iCount = 3;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore;

    bool _bPause;
    bool _bGaemReady;
    bool _bNextReady;

    float _fProgressTime;
    float _fWaitTime;


    void Awake()
    {
        _Instance = this;

        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    void Start()
    {
        LoadMapData();
    }

    // json에서 맵 데이터 가져오기.
    void LoadMapData()
    {
        TextAsset asset = (TextAsset)Resources.Load(_sMapFile);
        _JsonRoot = JSON.Parse(asset.text);

        for (int i = 0; i < _JsonRoot.Count; ++i)
        {
            BoxMapData data = new BoxMapData();
            data.idx = _JsonRoot[i]["index"].AsInt;
            data.iRow = _JsonRoot[i]["BoxW"].AsInt;
            data.iCol = _JsonRoot[i]["BoxH"].AsInt;
            data.iColorType = _JsonRoot[i]["ColorType"].AsInt;
            data.iColorVolume = _JsonRoot[i]["ColorVolume"].AsInt;
            data.iBonusTerms = _JsonRoot[i]["BonusTerms"].AsInt;
            data.iBonusTime = _JsonRoot[i]["BonusTime"].AsInt;

            _listMapData.Add(data);
        }
    }

    public void GAMESTART()
    {
        Init();

        _Timer.gameObject.SetActive(true);
        _Timer.Init();

        _bPause = false;
        _bGaemReady = true;

        StartCoroutine(StartGame());
    }

    public void COMPLETE()
    {
        _Step = STEP.COMPLETE;

        _iStageScore = (_CurMapData.iRow * _CurMapData.iCol) + _CurMapData.idx;

        Debug.Log(string.Format("StageScore : {0} / Time : {1}", _iStageScore, (int)_Timer._fRemainTime));

        _iTotalScore += _iStageScore + (int)_Timer._fRemainTime;

        SetScore(_iTotalScore);

        _Timer.TimerStop();

        _bNextReady = true;
        _NextStep = NEXT_STEP.ADDTIME;

        WaitTimeReset(1f);

        StartCoroutine(NextGame());
    }

    public void END()
    {
        _Step = STEP.END;

        _ResultPop.gameObject.SetActive(true);
        _ResultPop.SetResult(_iCurScore);
    }

    public void PAUSE()
    {
        _bPause = true;

        if(_Step == STEP.PLAY)
        {
            _Timer.TimerStop();
        }
    }

    public void CONTINUE()
    {
        _bPause = false;

        if(_Step == STEP.PLAY)
        {
            _Timer.TimerStart();
        }
        else if(_Step == STEP.START)
        {
            StartCoroutine(StartGame());
        }        
    }

    public void MAINMENU()
    {
        _Step = STEP.END;

        _BoxMapManager.ClearBoxMap();
    }

    public void ShowHint()
    {
        _BoxMapManager.ShowHint();
    }

    public void resetHint()
    {
        _Timer.ResetHint();
    }

    IEnumerator StartGame()
    {
        while (_bGaemReady)
        {
            if (WaitTime())
            {
                switch (_StartStep)
                {
                    case START_STEP.INIT:
                        Init();
                        break;

                    case START_STEP.SIZE:
                        SetMapSize();
                        WaitTimeReset(1.0f);
                        break;

                    case START_STEP.COLOR:
                        SetMapColoring();
                        WaitTimeReset(1.0f);
                        break;

                    case START_STEP.COUNT:
                        Count();
                        WaitTimeReset(3.0f);
                        break;

                    case START_STEP.SHUFFLE:
                        SetMapShuffing();
                        WaitTimeReset(0.7f);
                        break;

                    case START_STEP.PLAY:
                        Play();
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator NextGame()
    {
        while (_bNextReady)
        {
            if (WaitTime())
            {
                switch (_NextStep)
                {
                    case NEXT_STEP.ADDTIME:
                        {
                            _Timer.AddTime(_CurMapData, 1.0f);
                            WaitTimeReset(1.0f);

                            _NextStep = NEXT_STEP.NEXT;
                        }
                        break;
                    case NEXT_STEP.NEXT:
                        {
                            if(_iStage < _listMapData.Count - 1)
                            {
                                _iStage++;
                                _UIStage.text = string.Format("Stage {0}", _iStage + 1);
                            }

                            _bNextReady = false;
                            _bGaemReady = true;

                            _StartStep = START_STEP.SIZE;

                            WaitTimeReset(0.0f);
                            StartCoroutine(StartGame());

                        }
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    bool WaitTime()
    {
        if (_bPause == false)
        {
            if (Time.time - _fProgressTime > _fWaitTime)
            {
                return true;
            }
        }
        else
        {
            _fProgressTime = Time.time;
            return false;
        }
        return false;
    }

    void WaitTimeReset(float wait)
    {
        _fProgressTime = Time.time;
        _fWaitTime = wait;
    }

    void Init()
    {
        _iStage = 0;

        _iStageScore = 0;
        _iTotalScore = 0;
        _iCurScore = 0;

        _BoxMapManager.ClearBoxMap();
        _BoxMapManager.Init();

        SetScore(0);

        _Step = STEP.START;
        _StartStep = START_STEP.SIZE;
    }

    void SetMapSize()
    {
        _CurMapData = _listMapData[_iStage];
        StartCoroutine(_BoxMapManager.SetBoxMap(_CurMapData));

        _StartStep = START_STEP.COLOR;
    }

    void SetMapColoring()
    {
        _BoxMapManager.BoxColoring();

        _StartStep = START_STEP.COUNT;
    }

    void SetMapShuffing()
    {
        StartCoroutine(_BoxMapManager.BoxShuffling());

        _StartStep = START_STEP.PLAY;
    }

    void Count()
    {
        _Timer.BlinkStart();

        _StartStep = START_STEP.SHUFFLE;
    }

    void Play()
    {
        _Step = STEP.PLAY;

        _bGaemReady = false;

        _Timer.BlinkStop();
        _Timer.TimerStart();
    }

    void SetScore(int score)
    {
        LeanTween.value(gameObject, _iCurScore, score, 1f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (float value) =>
            {
                _iCurScore = (int)value;
                //_UIScore.text = _iCurScore.ToString();
                _UIScore.text = string.Format("Score : {0}", _iCurScore);
            }
        );
    }
}
