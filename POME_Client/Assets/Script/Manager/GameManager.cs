using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using SimpleJSON;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance = null;

    public enum STEP {START, PLAY, PAUSE, COMPLETE, END };

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
    public StringManager _StringManager = null;

    public Timer _Timer = null;

    public UILabel _UIScore = null;
    public UILabel _UIBest = null;
    public UILabel _UIStage = null;
    public UILabel _UIClear = null;
    public UILabel _UIOriginalCnt = null;

    public ResultPopup _ResultPop = null;

    List<BoxMapData> _listMapData;
    BoxMapData _CurMapData;

    JSONNode _JsonRoot = null;
    string _sMapFile = "Data/MapData";

    int _iStage;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore;
    long _lBestScore;

    public static bool _bPause;
    bool _bGaemReady;
    bool _bNextReady;
    bool _bOriginal;
    bool _bContinue;

    float _fProgressTime;
    float _fWaitTime;
    float _fRemainTime;

    string _sBest;
    string _sScore;
    string _sStage;

    public static int _iOriginCnt = 0;
    public static int _iADContinue = 0;


    void Awake()
    {
        _Instance = this;

        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        LoadMapData();

        _sBest = _StringManager.GetText(3002);    // 최고 점수.
        _sScore = _StringManager.GetText(3003);   // 점수.
        _sStage = "STAGE";   // 스테이지.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PASS();
        }
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

    // 게임시작 버튼.
    public void GAMESTART()
    {
        Init();

        _Timer.Init();

        _bPause = false;
        _bGaemReady = true;

        StartCoroutine(StartGame());
    }

    // 게임 스테이지 클리어.
    public void COMPLETE()
    {
        _Step = STEP.COMPLETE;

        _iStageScore = (_CurMapData.iRow * _CurMapData.iCol) + _CurMapData.idx;

        //Debug.Log(string.Format("StageScore : {0} / Time : {1}", _iStageScore, (int)_Timer._fRemainTime));

        _iTotalScore += _iStageScore + (int)_Timer._fRemainTime;

        SetScore(_iTotalScore);

        _Timer.TimerStop();

        AmiscGame.AchivStage(_iStage);
        AmiscGame.AchivScore(_iTotalScore);

        _bNextReady = true;
        _NextStep = NEXT_STEP.ADDTIME;

        WaitTimeReset(1f);

        StartCoroutine(NextGame());

        SetClear(true);
    }

    // 게임 종료.
    public void END()
    {
        _Step = STEP.END;

        _BoxMapManager.DismissHint();

        _TouchManager.Init();

        if (_iCurScore > _lBestScore)
        {
            _lBestScore = _iCurScore;

            PlayerPrefs.SetInt("BestScore", _iCurScore);            

#if !UNITY_EDITOR
            if (GameService._Instance.IsConnected())
                GameService._Instance.SetBestScore(_lBestScore);
#endif
        }

#if !UNITY_EDITOR
        if (GoogleAds._Instance._bInterstitialLoaded)
        {
            if (UnityEngine.Random.Range(1, 100) <= 30)
            {
                UIManager._BackStep = BACK_STEP.AD;

                if (AudioListener.volume > 0)
                {
                    AudioListener.volume = 0;
                    GoogleAds._Instance.OnInterstitialClosed += new Action(SoundOn);
                }
                GoogleAds._Instance.OnInterstitialClosed += new Action(ShowResultPop);
                GoogleAds._Instance.ShowInterstital();
            }
        }
#else
        ShowResultPop();
#endif
    }

    // 일시정지.
    public void PAUSE()
    {
        _bPause = true;

        if(_Step == STEP.PLAY)
        {
            _Timer.TimerStop();
        }
    }

    // 계속 버튼.
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
    
   // 메인메뉴 버튼.
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

        BGM._Instance.PlayT5();
    }

    // TEST용(스테이지 넘기기).
    public void PASS()
    {
        COMPLETE();
    }

    // 힌트 요청.
    public void ShowHint()
    {
        _BoxMapManager.ShowHint();
    }

    // 힌트 타이밍 다시 셋팅.
    public void resetHint()
    {
        _Timer.ResetHint();
    }

    // 정답 박스 요청.
    public void ShowOriginal()
    {
        if(_iOriginCnt > 0)
        {
            _bOriginal = true;
            _OriginalStep = ORIGINAL_STEP.EFFECT1;
            _Step = STEP.PAUSE;

            UIManager._BackStep = BACK_STEP.GAME;

            _Timer.TimerStop();

            _BoxMapManager.DismissHint();

            SetOriginCount(_iOriginCnt - 1);

            StartCoroutine(Original());
        }
    }

    // 광고 보고 이어하기 요청.
    public void ADContinue()
    {
        if (_iADContinue > 0)
        {
            _iADContinue = -1;

            _ResultPop.gameObject.SetActive(false);

            _bContinue = true;
            _ContinueStep = CONTINUE_STEP.ADDTIME;

            UIManager._BackStep = BACK_STEP.GAME;

            WaitTimeReset(1f);

            StartCoroutine(Continue());
        }
    }

    // 게임 시작 루틴.
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
                            if (_iStage < _listMapData.Count)
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

    // 다음 스테이지 루틴.
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
                            _iStage++;
                            _UIStage.text = string.Format("{0} {1}", _sStage, _iStage + 1);

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

    // 정답 보기 루틴.
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

                            _Step = STEP.PLAY;

                            _Timer.TimerStart();
                            _Timer.ResetHint();
                        }
                        break;
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    // 광고보고 난후 이어하기 루틴.
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

    // 루틴 도는 동안 기다리기.
    bool WaitTime()
    {
        if (_bPause == false)
        {
            if (Time.time - _fProgressTime > _fWaitTime)
            {
                return true;
            }
            else
            {
                _fRemainTime = Time.time - _fProgressTime;
            }
        }
        else
        {
            _fProgressTime = Time.time - _fRemainTime;
            return false;
        }
        return false;
    }

    // 기다리는 시간 셋팅.
    void WaitTimeReset(float wait)
    {
        _fProgressTime = Time.time;
        _fWaitTime = wait;
    }

    // 초기화.
    void Init()
    {
        _iStage = 0;
        _UIStage.text = string.Format("{0} {1}", _sStage, _iStage + 1);

        _iStageScore = 0;
        _iTotalScore = 0;
        _iCurScore = 0;

#if !UNITY_EDITOR
        if (GameService._Instance.IsConnected())
            _lBestScore = GameService._Instance.GetBestScore();
        else
            _lBestScore = PlayerPrefs.GetInt("BestScore");
#else
        _lBestScore = PlayerPrefs.GetInt("BestScore");
#endif

        _BoxMapManager.ClearBoxMap();
        _BoxMapManager.Init();

        _TouchManager.Init();

        SetScore(0);
        SetBest(_lBestScore);

        _Step = STEP.START;
        _StartStep = START_STEP.SIZE;

        SetClear(false);

        SetOriginCount(2);
        _iADContinue = 1;
    }

    // 게임시작.
    void Play()
    {
        _Step = STEP.PLAY;

        _bGaemReady = false;

        _Timer.BlinkStop();
        _Timer.TimerStart();
    }

    // 현재 스코어 셋팅.
    void SetScore(int score)
    {
        LeanTween.value(gameObject, _iCurScore, score, 1f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (float value) =>
            {
                _iCurScore = (int)value;
                //_UIScore.text = _iCurScore.ToString();
                _UIScore.text = string.Format("{0}  :  {1}", _sScore, _iCurScore);

                //if (_lBestScore < _iCurScore)
                //{
                //    _lBestScore = _iCurScore;
                //    SetBest(_lBestScore);
                //}                
            }
        );
    }

    // 베스트 스토어 셋팅.
    void SetBest(long score)
    {
        _UIBest.text = string.Format("{0}  :  {1}", _sBest, score);
    }

    // 클리어 문구 보여주기.
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

    void SetOriginCount(int count)
    {
        _iOriginCnt = count;
        _UIOriginalCnt.text = _iOriginCnt.ToString();
    }

    void ShowResultPop()
    {
        UIManager._BackStep = BACK_STEP.RESULT;

        _ResultPop.gameObject.SetActive(true);
        _ResultPop.SetResult(_lBestScore, _iCurScore);
    }

    public void SoundOn()
    {
        AudioListener.volume = 1;
    }
}
