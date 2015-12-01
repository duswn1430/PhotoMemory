﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Define;
using SimpleJSON;


public class GameManager : MonoBehaviour
{
    public enum STEP { INIT, PLAY, COMPLETE, END };

    public static STEP _Step;

    public BoxMapManager _BoxMapManager = null;

    public Timer _Timer = null;

    public UILabel _UIText = null;
    public UILabel _UIScore = null;

    public GameObject _btnStart = null;
    public GameObject _btnBack = null;

    List<BoxMapData> _listMapData;

    JSONNode _jsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;
    int _iCount = 3;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore = 0;

    bool _bHint = false;
    float _fHintTime = 5;
    float _fHintWaitTime = 0;
    
    void Awake()
    {
        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    void Start()
    {
        LoadMapData();

        GoogleAds._Instance.Init();

#if UNITY_EDITOR
        Init();
#else
        GoogleAds._Instance.LoadInterstitial();
        GoogleAds._Instance.OnInterstitialLoaded += Init;
#endif
    }

    public void BANNER()
    {
        if (GoogleAds._Instance.IsBanner())
            GoogleAds._Instance.HideBanner();
        else
            GoogleAds._Instance.ShowBanner();
    }

    public void INTERSTITAL()
    {
        if (GoogleAds._Instance._bLoadInterstital)
            GoogleAds._Instance.ShowInterstital();
    }

    public void CONNECT()
    {
        GameService._Instance.Connect();
    }

    public void LEADERBOARD()
    {
        GameService._Instance.ShowLeaderBoard();
    }

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

    void Update()
    {
        if (_bHint)
        {
            if (Time.time - _fHintWaitTime >= _fHintTime)
            {
                _bHint = false;

                ShowHint();
            }
        }
    }

    void Init()
    {
        _Step = STEP.INIT;

        _iStage = 0;

        _iTotalScore = 0;
        _iCurScore = 0;

        SetScore(0);

        _bHint = false;
        _fHintWaitTime = Time.time;

        _BoxMapManager.ClearBoxMap();
        _BoxMapManager.Init();

        SetText("", 200);

        _btnStart.SetActive(true);
    }

    public void START()
    {
        _btnStart.SetActive(false);

        _Timer.gameObject.SetActive(true);
        _Timer.Init();

        StartCoroutine(GameStart());
    }

    public void RESTART()
    {

    }

    public void BACK()
    {
        _btnBack.SetActive(false);

        GoogleAds._Instance.LoadInterstitial();
    }

    public void COMPLETE(BoxMapData mapdata)
    {
        _Step = STEP.COMPLETE;

        _bHint = false;

        SetText(_Step.ToString(), 200);

        _iStageScore = (mapdata.iRow * mapdata.iCol) + mapdata.idx;
        _iTotalScore += _iStageScore + (int)_Timer._fRemainTime;

        SetScore(_iTotalScore);

        _Timer.TimerStop();

        StartCoroutine(NextStage(mapdata));
    }

    public void END()
    {
        _Step = STEP.END;

        _bHint = false;

        SetText(_Step.ToString(), 200);

        _Timer.gameObject.SetActive(false);

        _btnBack.SetActive(true);
    }

    public void ShowHint()
    {
        _BoxMapManager.ShowHint();
    }

    public void resetHint()
    {
        _bHint = true;
        _fHintWaitTime = Time.time;
    }

    IEnumerator GameStart()
    {
        yield return StartCoroutine(_BoxMapManager.SetBoxMap(_listMapData[_iStage]));

        yield return new WaitForSeconds(0.5f);

        _BoxMapManager.BoxColoring();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Counting());

        yield return StartCoroutine(_BoxMapManager.BoxShuffling());

        _Step = STEP.PLAY;

        SetText(_Step.ToString(), 200);

        _Timer.TimerStart();

        _bHint = true;
        _fHintWaitTime = Time.time;
    }

    IEnumerator Counting()
    {
        int time = _iCount;

        while (time > 0)
        {
            SetText(time.ToString(), 250);

            yield return new WaitForSeconds(1f);

            time--;
        }

        SetText("", 0);
    }

    IEnumerator NextStage(BoxMapData mapdata)
    {
        yield return new WaitForSeconds(1f);

        _Timer.AddTime(mapdata, 1.00f);

        yield return new WaitForSeconds(1f);

        if (_iStage < _listMapData.Count - 1)
            _iStage++;

        StartCoroutine(GameStart());
    }

    void SetText(string text, int size)
    {
        _UIText.fontSize = size;
        _UIText.text = text;
    }

    void SetScore(int score)
    {
        LeanTween.value(gameObject, _iCurScore, score, 1f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (float value) =>
            {
                _iCurScore = (int)value;
                _UIScore.text = _iCurScore.ToString();
            }
        );
    }
}