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

    List<BoxMapData> _listMapData;

    JSONNode _jsonRoot = null;
    string _sMapFile = "MapData";

    int _iStage;
    int _iCountTime = 3;

    int _iStageScore;
    int _iTotalScore;
    int _iCurScore = 0;

    void Awake()
    {
        _listMapData = new List<BoxMapData>();
    }

    // Use this for initialization
    void Start()
    {
        LoadMapData();
    }

    void LoadMapData()
    {
        TextAsset asset = (TextAsset)Resources.Load(_sMapFile);
        _jsonRoot = JSON.Parse(asset.text);

        for (int i = 0; i < _jsonRoot.Count; ++i)
        {
            BoxMapData data = new BoxMapData();
            data.idx = _jsonRoot[i]["Index"].AsInt;
            data.iRow = _jsonRoot[i]["BoxW"].AsInt;
            data.iCol = _jsonRoot[i]["BoxH"].AsInt;
            data.iColorType = _jsonRoot[i]["ColorTypes"].AsInt;
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

    }

    public void COMPLETE(BoxMapData mapdata)
    {
        _Step = STEP.COMPLETE;

        SetText(_Step.ToString(), 200);

        _iStageScore = (mapdata.iRow * mapdata.iCol) + mapdata.idx;
        int addScroe = _iStageScore + (int)_Timer._fRemainTime;

        SetScore(addScroe);

        StartCoroutine(NextStage(mapdata));
    }

    public void END()
    {
        _Step = STEP.END;

        SetText(_Step.ToString(), 200);
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

        _Timer.GameStart();
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

    IEnumerator NextStage(BoxMapData mapdata)
    {
        yield return new WaitForSeconds(1f);

        _Timer.AddTime(mapdata, 1f);

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

    void SetScore(int addScore)
    {
        _iTotalScore = _iCurScore + addScore;

        LeanTween.value(gameObject, _iCurScore, _iTotalScore, 1f).setEase(LeanTweenType.easeOutCirc).setOnUpdate(
            (float value) =>
            {
                _iCurScore = (int)value;
                _UIScore.text = _iCurScore.ToString();
            }
        );
    }
}
