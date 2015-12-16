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

    public enum ORIGINAL_STEP { EFFECT1, ORIGIN, SHUTTER, CURRENT, END };

    ORIGINAL_STEP _OriginalStep;

    public enum CONTINUE_STEP { ADDTIME, START };

    CONTINUE_STEP _ContinueStep;

    public BoxMapManager _BoxMapManager = null;
    public TouchManager _TouchManager = null;

    public Timer _Timer = null;

    public UILabel _UIScore = null;
    public UILabel _UIBest = null;
    public UILabel _UIStage = null;
    public UILabel _UIClear = null;

    public ResultPopup _ResultPop = null;

    List<BoxMapData> _listMapData;
    BoxMapData _CurMapData;

    JSONNode _JsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore;
    int _iBestScore;

    bool _bPause;
    bool _bGaemReady;
    bool _bNextReady;
    bool _bOriginal;
    bool _bContinue;

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

        _Timer.Init();

        _bPause = false;
        _bGaemReady = true;

        StartCoroutine(StartGame());
    }

    public void COMPLETE()
    {
        _Step = STEP.COMPLETE;

        _iStageScore = (_CurMapData.iRow * _CurMapData.iCol) + _CurMapData.idx;

        //Debug.Log(string.Format("StageScore : {0} / Time : {1}", _iStageScore, (int)_Timer._fRemainTime));

        _iTotalScore += _iStageScore + (int)_Timer._fRemainTime;

        SetScore(_iTotalScore);

        _Timer.TimerStop();

        _bNextReady = true;
        _NextStep = NEXT_STEP.ADDTIME;

        WaitTimeReset(1f);

        StartCoroutine(NextGame());

        SetClear(true);
    }

    public void END()
    {
        _Step = STEP.END;

        _BoxMapManager.DismissHint();

        _TouchManager.Init();

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

        _bGaemReady = false;
        _bNextReady = false;
        _bOriginal = false;
        _bContinue = false;

        _Timer.TimerStop();
        _Timer.BlinkStop();

        _BoxMapManager.ClearBoxMap();
    }

    public void PASS()
    {
        COMPLETE();
    }

    public void ShowHint()
    {
        _BoxMapManager.ShowHint();
    }

    public void resetHint()
    {
        _Timer.ResetHint();
    }

    public void ShowOriginal()
    {
        _bOriginal = true;
        _OriginalStep = ORIGINAL_STEP.EFFECT1;

        _Timer.TimerStop();

        _BoxMapManager.DismissHint();

        StartCoroutine(Original());
    }

    public void ADContinue()
    {
        _ResultPop.gameObject.SetActive(false);

        _bContinue = true;
        _ContinueStep = CONTINUE_STEP.ADDTIME;

        WaitTimeReset(1f);

        StartCoroutine(Continue());
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
                        {
                            _CurMapData = _listMapData[_iStage];
                            StartCoroutine(_BoxMapManager.SetBoxMap(_CurMapData));

                            _StartStep = START_STEP.COLOR;

                            WaitTimeReset(1.0f);
                        }
                        break;

                    case START_STEP.COLOR:
                        {
                            _BoxMapManager.BoxColoring();

                            _StartStep = START_STEP.COUNT;

                            WaitTimeReset(1.0f);
                        }
                        break;

                    case START_STEP.COUNT:
                        {
                            _Timer.BlinkStart();

                            _StartStep = START_STEP.SHUFFLE;

                            WaitTimeReset(3.0f);
                        }
                        break;

                    case START_STEP.SHUFFLE:
                        {
                            StartCoroutine(_BoxMapManager.BoxShuffling());

                            _StartStep = START_STEP.PLAY;

                            WaitTimeReset(0.7f);
                        }
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
                            float stageSpendTime = _Timer.GetStageSpendTime();

                            if (stageSpendTime <= _CurMapData.iBonusTerms)
                            {
                                _Timer.AddTime(_CurMapData.iBonusTime);

                                WaitTimeReset(1.0f);
                            }

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
                            
                            SetClear(false);
                        }
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Original()
    {
        while (_bOriginal)
        {
            if (WaitTime())
            {
                switch (_OriginalStep)
                {
                    case ORIGINAL_STEP.EFFECT1:
                        {
                            _OriginalStep = ORIGINAL_STEP.ORIGIN;
                        }
                        break;
                    case ORIGINAL_STEP.ORIGIN:
                        {
                            _Timer.TimerStop();
                            _Timer.BlinkStart();

                            _BoxMapManager.ShowOrigin();

                            _OriginalStep = ORIGINAL_STEP.SHUTTER;

                            WaitTimeReset(3.0f);
                        }
                        break;
                    case ORIGINAL_STEP.SHUTTER:
                        {
                            _Timer.BlinkStop();

                            StartCoroutine(_BoxMapManager.ShutterPlay(true));

                            _OriginalStep = ORIGINAL_STEP.CURRENT;

                            WaitTimeReset(0.33f);
                        }
                        break;
                    case ORIGINAL_STEP.CURRENT:
                        {
                            _BoxMapManager.ShowCur();

                            StartCoroutine(_BoxMapManager.ShutterPlay(false));

                            _OriginalStep = ORIGINAL_STEP.END;

                            WaitTimeReset(0.33f);
                        }
                        break;
                    case ORIGINAL_STEP.END:
                        {
                            _bOriginal = false;

                            _Timer.TimerStart();
                            _Timer.ResetHint();
                        }
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Continue()
    {
        while (_bContinue)
        {
            if (WaitTime())
            {
                switch (_ContinueStep)
                {
                    case CONTINUE_STEP.ADDTIME:
                        {
                            _Timer.AddTime(15.0f);

                            _ContinueStep = CONTINUE_STEP.START;

                            WaitTimeReset(0.5f);
                        }
                        break;
                    case CONTINUE_STEP.START:
                        {
                            _bContinue = false;
                            _bGaemReady = true;

                            _StartStep = START_STEP.SIZE;

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
        _UIStage.text = string.Format("Stage {0}", _iStage + 1);

        _iStageScore = 0;
        _iTotalScore = 0;
        _iCurScore = 0;
        _iBestScore = PlayerPrefs.GetInt("BestScore");

        _BoxMapManager.ClearBoxMap();
        _BoxMapManager.Init();

        _TouchManager.Init();

        SetScore(0);
        SetBest(_iBestScore);

        _Step = STEP.START;
        _StartStep = START_STEP.SIZE;

        SetClear(false);
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
                _UIScore.text = string.Format("SCORE : {0}", _iCurScore);

                if(_iBestScore < _iCurScore)
                {
                    _iBestScore = _iCurScore;
                    SetBest(_iBestScore);
                }                
            }
        );
    }

    void SetBest(int score)
    {
        _UIBest.text = string.Format("BEST    : {0}", score);
    }

    void SetClear(bool show)
    {
        if (show)
        {
            LeanTween.scale(_UIClear.gameObject, new Vector3(1, 1, 1), 0.4f).setEase(LeanTweenType.easeSpring).setOnStart(
                () => {
                    _UIStage.gameObject.SetActive(false);
                });
        }
        else
        {
            LeanTween.scale(_UIClear.gameObject, Vector3.zero, 0.4f).setEase(LeanTweenType.easeSpring).setOnComplete(
                () => { _UIStage.gameObject.SetActive(true); 
                });
        }
    }
}
