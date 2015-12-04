using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using SimpleJSON;


public class GameManager : MonoBehaviour
{
    public static GameManager _Instance = null;

    public enum STEP { INIT, PLAY, PAUSE, COMPLETE, END };

    public static STEP _Step;

    public BoxMapManager _BoxMapManager = null;

    public Timer _Timer = null;

    public UILabel _UIScore = null;
    public UILabel _UIStage = null;

    public ResultPopup _Result = null;

    List<BoxMapData> _listMapData;

    JSONNode _jsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;
    int _iCount = 3;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore = 0;
    
    void Awake()
    {
        _Instance = this;

        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    void Start()
    {
        LoadMapData();

        //GoogleAds._Instance.Init();

//#if UNITY_EDITOR
//        Init();
//#else
//        GoogleAds._Instance.LoadInterstitial();
//        GoogleAds._Instance.OnInterstitialLoaded += Init;
//#endif
    }

    //public void BANNER()
    //{
    //    if (GoogleAds._Instance.IsBanner())
    //        GoogleAds._Instance.HideBanner();
    //    else
    //        GoogleAds._Instance.ShowBanner();
    //}

    //public void INTERSTITAL()
    //{
    //    if (GoogleAds._Instance._bLoadInterstital)
    //        GoogleAds._Instance.ShowInterstital();
    //}

    //public void CONNECT()
    //{
    //    GameService._Instance.Connect();
    //}

    //public void LEADERBOARD()
    //{
    //    GameService._Instance.ShowLeaderBoard();
    //}

    void LoadMapData()
    {
        TextAsset asset = (TextAsset)Resources.Load(_sMapFile);
        _jsonRoot = JSON.Parse(asset.text);

        for (int i = 0; i < _jsonRoot.Count; ++i)
        {
            BoxMapData data = new BoxMapData();
            data.idx = _jsonRoot[i]["index"].AsInt;
            data.iRow = _jsonRoot[i]["BoxW"].AsInt;
            data.iCol = _jsonRoot[i]["BoxH"].AsInt;
            data.iColorType = _jsonRoot[i]["ColorType"].AsInt;
            data.iColorVolume = _jsonRoot[i]["ColorVolume"].AsInt;
            data.iBonusTerms = _jsonRoot[i]["BonusTerms"].AsInt;
            data.iBonusTime = _jsonRoot[i]["BonusTime"].AsInt;

            _listMapData.Add(data);
        }
    }

    void Init()
    {
        _Step = STEP.INIT;

        _iStage = 0;

        _iTotalScore = 0;
        _iCurScore = 0;

        SetScore(0);

        _BoxMapManager.ClearBoxMap();
        _BoxMapManager.Init();
    }

    public void GAMESTART()
    {
        Init();

        _Timer.gameObject.SetActive(true);
        _Timer.Init();

        StartCoroutine(GameStart());
        //_GameStep = GAME_STEP.START;
        //NextStep();
    }

    public void PAUSE()
    {
        _Step = STEP.PAUSE;
        _Timer.TimerPause();
    }

    public void CONTINUE()
    {
        _Step = STEP.PLAY;

        _Timer.TimerResume();
    }

    public void MAINMENU()
    {
        _Step = STEP.END;

        _BoxMapManager.ClearBoxMap();
    }

    public void RANK()
    {
        _Step = STEP.PAUSE;

        _Timer.TimerPause();
    }

    public void SOUND()
    {
        Debug.Log("????????????");
    }

    public void COMPLETE(BoxMapData mapdata)
    {
        _Step = STEP.COMPLETE;

        _iStageScore = (mapdata.iRow * mapdata.iCol) + mapdata.idx;
        _iTotalScore += _iStageScore + (int)_Timer._fRemainTime;

        SetScore(_iTotalScore);

        _Timer.TimerStop();

        StartCoroutine(NextStage(mapdata));
    }

    public void END()
    {
        _Step = STEP.END;

        _Result.gameObject.SetActive(true);
        _Result.SetResult(_iCurScore);
    }

    public void ShowHint()
    {
        _BoxMapManager.ShowHint();
    }

    public void resetHint()
    {
        _Timer.ResetHint();
    }

    void Update()
    {

    }

    public enum GAME_STEP {START, NEXT };
    public enum START_STEP {NONE, INIT, COLOR, COUNT, SHUFFLE, PLAY, PAUSE, COMPLETE, END };

    GAME_STEP _GameStep;
    START_STEP _StartStep;

    float _fSpendTime = 0;
    float _fWaitTime = 0;

    public void NextStep()
    {
        switch (_GameStep)
        {
            case GAME_STEP.START:
                _StartStep++;
                StartSetting();
                break;
            case GAME_STEP.NEXT:
                break;
        }
    }

    void StartSetting()
    {
        Debug.Log("_StartStep : " + _StartStep);

        switch (_StartStep)
        {
            case START_STEP.INIT:
                StartCoroutine(_BoxMapManager.SetBoxMap(_listMapData[_iStage]));
                break;
            case START_STEP.COLOR:
                break;
            case START_STEP.COUNT:
                break;
            case START_STEP.SHUFFLE:
                break;
            case START_STEP.PLAY:
                break;
            case START_STEP.PAUSE:
                break;
            case START_STEP.COMPLETE:
                break;
            case START_STEP.END:
                break;
        }
    }


    IEnumerator GameStart()
    {
        yield return StartCoroutine(_BoxMapManager.SetBoxMap(_listMapData[_iStage]));

        yield return new WaitForSeconds(0.5f);

        while (_Step == STEP.PAUSE)
        {
            yield return new WaitForFixedUpdate();
        }

        _BoxMapManager.BoxColoring();

        yield return new WaitForSeconds(1f);

        while (_Step == STEP.PAUSE)
        {
            yield return new WaitForFixedUpdate();
        }

        yield return StartCoroutine(Counting());

        yield return StartCoroutine(_BoxMapManager.BoxShuffling());

        _Step = STEP.PLAY;

        _Timer.TimerStart();
    }

    IEnumerator Counting()
    {
        int time = _iCount;

        while (time > 0)
        {
            //SetText(time.ToString(), 250);

            yield return new WaitForSeconds(1f);

            while (_Step == STEP.PAUSE)
            {
                yield return new WaitForFixedUpdate();
            }

            time--;
        }
    }

    IEnumerator NextStage(BoxMapData mapdata)
    {
        yield return new WaitForSeconds(1f);

        _Timer.AddTime(mapdata, 1.00f);

        yield return new WaitForSeconds(1f);

        if (_iStage < _listMapData.Count - 1)
        {
            _iStage++;
            _UIStage.text = string.Format("Stage {0}", _iStage + 1);
        }

        StartCoroutine(GameStart());
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
